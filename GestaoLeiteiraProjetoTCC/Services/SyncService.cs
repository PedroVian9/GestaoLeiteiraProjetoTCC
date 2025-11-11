using GestaoLeiteiraProjetoTCC.DTOs;
using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class SyncService : ISyncService
    {
        private readonly DatabaseService _databaseService;
        private readonly ISyncMetadataService _metadataService;
        private readonly ISyncTransportService _transportService;

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public SyncService(
            DatabaseService databaseService,
            ISyncMetadataService metadataService,
            ISyncTransportService transportService)
        {
            _databaseService = databaseService;
            _metadataService = metadataService;
            _transportService = transportService;
        }

        public async Task<SyncResult> SynchronizeAsync(CancellationToken cancellationToken = default)
        {
            var connection = await _databaseService.GetConnectionAsync();
            var envelope = await BuildPayloadAsync(connection, cancellationToken);

            var payloadContent = JsonSerializer.Serialize(envelope, SerializerOptions);
            var fileName = $"sync_{_metadataService.GetDeviceId()}_{DateTime.UtcNow:yyyyMMddHHmmss}.json";
            var payloadPath = await _transportService.WriteOutboundPayloadAsync(fileName, payloadContent);

            var importedRecords = await ImportRemotePayloadAsync(connection, cancellationToken);
            await _metadataService.SetLastSuccessfulSyncUtcAsync(DateTime.UtcNow);

            return new SyncResult
            {
                Success = true,
                Message = importedRecords > 0
                    ? "Sincronizacao concluida com sucesso."
                    : "Exportacao concluida. Aguardando pacote do outro dispositivo.",
                RecordsExported = CountRecords(envelope.Data),
                RecordsImported = importedRecords,
                PayloadPath = payloadPath,
                RemoteExportedAtUtc = null
            };
        }

        private static int CountRecords(SyncData data)
        {
            return data.Propriedades.Count +
                   data.Racas.Count +
                   data.Animais.Count +
                   data.Lactacoes.Count +
                   data.Producoes.Count +
                   data.Gestacoes.Count +
                   data.QuantidadesOrdenha.Count;
        }

        private async Task<SyncEnvelope> BuildPayloadAsync(SQLiteAsyncConnection connection, CancellationToken ct)
        {
            var data = new SyncData
            {
                Propriedades = await connection.Table<Propriedade>().ToListAsync(),
                Racas = await connection.Table<Raca>().ToListAsync(),
                Animais = await connection.Table<Animal>().ToListAsync(),
                Lactacoes = await connection.Table<Lactacao>().ToListAsync(),
                Producoes = await connection.Table<ProducaoLeiteira>().ToListAsync(),
                Gestacoes = await connection.Table<Gestacao>().ToListAsync(),
                QuantidadesOrdenha = await connection.Table<QuantidadeOrdenha>().ToListAsync()
            };

            return new SyncEnvelope
            {
                SourceDeviceId = _metadataService.GetDeviceId(),
                SourceDeviceName = _metadataService.GetDeviceName(),
                SourcePlatform = _metadataService.GetPlatform(),
                ExportedAtUtc = DateTime.UtcNow,
                Data = data
            };
        }

        private async Task<int> ImportRemotePayloadAsync(SQLiteAsyncConnection connection, CancellationToken ct)
        {
            var files = await _transportService.GetAvailablePayloadFilesAsync();
            var localDeviceId = _metadataService.GetDeviceId();
            var remoteFile = files.FirstOrDefault(path =>
                !Path.GetFileName(path).Contains(localDeviceId, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(remoteFile))
            {
                return 0;
            }

            var content = await _transportService.ReadPayloadAsync(remoteFile);
            if (string.IsNullOrWhiteSpace(content))
            {
                return 0;
            }

            var envelope = JsonSerializer.Deserialize<SyncEnvelope>(content, SerializerOptions);
            if (envelope?.Data == null)
            {
                return 0;
            }

            var imported = await MergePayloadAsync(connection, envelope, ct);
            await _transportService.DeletePayloadAsync(remoteFile);
            return imported;
        }

        private async Task<int> MergePayloadAsync(SQLiteAsyncConnection connection, SyncEnvelope envelope, CancellationToken ct)
        {
            var context = await SyncMergeContext.CreateAsync(connection);
            var imported = 0;

            imported += await UpsertSimpleEntitiesAsync(connection, envelope.Data.Propriedades, context.Propriedades);
            imported += await UpsertSimpleEntitiesAsync(connection, envelope.Data.Racas, context.Racas);
            imported += await UpsertSimpleEntitiesAsync(connection, envelope.Data.QuantidadesOrdenha, context.QuantidadesOrdenha);

            var remotePropriedadeIdToSync = envelope.Data.Propriedades.ToDictionary(p => p.Id, p => p.SyncId);
            var remoteRacaIdToSync = envelope.Data.Racas.ToDictionary(r => r.Id, r => r.SyncId);
            var remoteAnimalIdToSync = envelope.Data.Animais.ToDictionary(a => a.Id, a => a.SyncId);
            var remoteLactacaoIdToSync = envelope.Data.Lactacoes.ToDictionary(l => l.Id, l => l.SyncId);

            imported += await UpsertAnimalsAsync(connection, envelope.Data.Animais, context, remotePropriedadeIdToSync, remoteRacaIdToSync, remoteAnimalIdToSync);
            imported += await UpsertLactacoesAsync(connection, envelope.Data.Lactacoes, context, remoteAnimalIdToSync, remotePropriedadeIdToSync);
            imported += await UpsertProducoesAsync(connection, envelope.Data.Producoes, context, remoteAnimalIdToSync, remoteLactacaoIdToSync, remotePropriedadeIdToSync);
            imported += await UpsertGestacoesAsync(connection, envelope.Data.Gestacoes, context, remoteAnimalIdToSync);

            return imported;
        }

        private async Task<int> UpsertSimpleEntitiesAsync<T>(
            SQLiteAsyncConnection connection,
            IEnumerable<T> entities,
            Dictionary<Guid, int> localMap)
            where T : class, ISyncEntity, new()
        {
            var affected = 0;
            foreach (var entity in entities)
            {
                var existing = await connection.Table<T>()
                    .Where(e => e.SyncId == entity.SyncId)
                    .FirstOrDefaultAsync();

                if (existing == null)
                {
                    entity.Id = 0;
                    await connection.InsertAsync(entity);
                    localMap[entity.SyncId] = entity.Id;
                    affected++;
                }
                else if (entity.UpdatedAt > existing.UpdatedAt)
                {
                    entity.Id = existing.Id;
                    await connection.UpdateAsync(entity);
                    affected++;
                    localMap[entity.SyncId] = existing.Id;
                }
                else
                {
                    localMap[entity.SyncId] = existing.Id;
                }
            }

            return affected;
        }

        private async Task<int> UpsertAnimalsAsync(
            SQLiteAsyncConnection connection,
            IEnumerable<Animal> animals,
            SyncMergeContext context,
            Dictionary<int, Guid> remotePropriedadeIdToSync,
            Dictionary<int, Guid> remoteRacaIdToSync,
            Dictionary<int, Guid> remoteAnimalIdToSync)
        {
            var pendingRelationships = new List<AnimalRelationshipSync>();

            foreach (var animal in animals)
            {
                animal.PropriedadeId = MapForeignKey(animal.PropriedadeId, remotePropriedadeIdToSync, context.Propriedades);
                animal.RacaId = MapForeignKey(animal.RacaId, remoteRacaIdToSync, context.Racas);

                var maeSyncId = ResolveSyncId(animal.MaeId, remoteAnimalIdToSync);
                var paiSyncId = ResolveSyncId(animal.PaiId, remoteAnimalIdToSync);

                if (maeSyncId.HasValue && context.Animais.TryGetValue(maeSyncId.Value, out var localMaeId))
                {
                    animal.MaeId = localMaeId;
                }
                else
                {
                    animal.MaeId = null;
                }

                if (paiSyncId.HasValue && context.Animais.TryGetValue(paiSyncId.Value, out var localPaiId))
                {
                    animal.PaiId = localPaiId;
                }
                else
                {
                    animal.PaiId = null;
                }

                pendingRelationships.Add(new AnimalRelationshipSync
                {
                    AnimalSyncId = animal.SyncId,
                    MaeSyncId = maeSyncId,
                    PaiSyncId = paiSyncId
                });
            }

            var affected = await UpsertSimpleEntitiesAsync(connection, animals, context.Animais);

            //foreach (var pending in pendingRelationships)
            //{
            //    var hasMae = pending.MaeSyncId.HasValue && context.Animais.TryGetValue(pending.MaeSyncId.Value, out var maeId);
            //    var hasPai = pending.PaiSyncId.HasValue && context.Animais.TryGetValue(pending.PaiSyncId.Value, out var paiId);

            //    if (!hasMae && !hasPai)
            //    {
            //        continue;
            //    }

            //    await connection.ExecuteAsync(
            //        "UPDATE Animal SET MaeId = ?, PaiId = ? WHERE SyncId = ?",
            //        hasMae ? maeId : (int?)null,
            //        hasPai ? paiId : (int?)null,
            //        pending.AnimalSyncId.ToString());
            //}

            return affected;
        }

        private async Task<int> UpsertLactacoesAsync(
            SQLiteAsyncConnection connection,
            IEnumerable<Lactacao> lactacoes,
            SyncMergeContext context,
            Dictionary<int, Guid> remoteAnimalIdToSync,
            Dictionary<int, Guid> remotePropriedadeIdToSync)
        {
            foreach (var lactacao in lactacoes)
            {
                lactacao.AnimalId = MapForeignKey(lactacao.AnimalId, remoteAnimalIdToSync, context.Animais);
                lactacao.PropriedadeId = MapForeignKey(lactacao.PropriedadeId, remotePropriedadeIdToSync, context.Propriedades);
            }

            return await UpsertSimpleEntitiesAsync(connection, lactacoes, context.Lactacoes);
        }

        private async Task<int> UpsertProducoesAsync(
            SQLiteAsyncConnection connection,
            IEnumerable<ProducaoLeiteira> producoes,
            SyncMergeContext context,
            Dictionary<int, Guid> remoteAnimalIdToSync,
            Dictionary<int, Guid> remoteLactacaoIdToSync,
            Dictionary<int, Guid> remotePropriedadeIdToSync)
        {
            foreach (var producao in producoes)
            {
                producao.AnimalId = MapForeignKey(producao.AnimalId, remoteAnimalIdToSync, context.Animais);
                producao.LactacaoId = MapForeignKey(producao.LactacaoId, remoteLactacaoIdToSync, context.Lactacoes);
                producao.PropriedadeId = MapForeignKey(producao.PropriedadeId, remotePropriedadeIdToSync, context.Propriedades);
            }

            return await UpsertSimpleEntitiesAsync(connection, producoes, context.Producoes);
        }

        private async Task<int> UpsertGestacoesAsync(
            SQLiteAsyncConnection connection,
            IEnumerable<Gestacao> gestacoes,
            SyncMergeContext context,
            Dictionary<int, Guid> remoteAnimalIdToSync)
        {
            foreach (var gestacao in gestacoes)
            {
                gestacao.VacaId = MapForeignKey(gestacao.VacaId, remoteAnimalIdToSync, context.Animais);
                gestacao.TouroId = MapForeignKeyNullable(gestacao.TouroId, remoteAnimalIdToSync, context.Animais);
                gestacao.CriaId = MapForeignKeyNullable(gestacao.CriaId, remoteAnimalIdToSync, context.Animais);
            }

            return await UpsertSimpleEntitiesAsync(connection, gestacoes, context.Gestacoes);
        }

        private static int MapForeignKey(
            int remoteId,
            Dictionary<int, Guid> remoteIdToSync,
            Dictionary<Guid, int> localMap)
        {
            if (remoteId == 0)
            {
                return 0;
            }

            if (!remoteIdToSync.TryGetValue(remoteId, out var syncId))
            {
                return remoteId;
            }

            if (localMap.TryGetValue(syncId, out var localId))
            {
                return localId;
            }

            return remoteId;
        }

        private static int? MapForeignKeyNullable(
            int? remoteId,
            Dictionary<int, Guid> remoteIdToSync,
            Dictionary<Guid, int> localMap)
        {
            if (!remoteId.HasValue)
            {
                return null;
            }

            var mapped = MapForeignKey(remoteId.Value, remoteIdToSync, localMap);
            return mapped == 0 ? null : mapped;
        }

        private static Guid? ResolveSyncId(int? remoteId, Dictionary<int, Guid> remoteMap)
        {
            if (!remoteId.HasValue)
            {
                return null;
            }

            return remoteMap.TryGetValue(remoteId.Value, out var syncId) ? syncId : null;
        }

        private class AnimalRelationshipSync
        {
            public Guid AnimalSyncId { get; set; }
            public Guid? MaeSyncId { get; set; }
            public Guid? PaiSyncId { get; set; }
        }

        private class SyncMergeContext
        {
            public Dictionary<Guid, int> Propriedades { get; } = new();
            public Dictionary<Guid, int> Racas { get; } = new();
            public Dictionary<Guid, int> Animais { get; } = new();
            public Dictionary<Guid, int> Lactacoes { get; } = new();
            public Dictionary<Guid, int> Producoes { get; } = new();
            public Dictionary<Guid, int> Gestacoes { get; } = new();
            public Dictionary<Guid, int> QuantidadesOrdenha { get; } = new();

            public static async Task<SyncMergeContext> CreateAsync(SQLiteAsyncConnection connection)
            {
                var context = new SyncMergeContext();
                await context.PopulateAsync(connection);
                return context;
            }

            private async Task PopulateAsync(SQLiteAsyncConnection connection)
            {
                await PopulateMapAsync(connection.Table<Propriedade>(), Propriedades);
                await PopulateMapAsync(connection.Table<Raca>(), Racas);
                await PopulateMapAsync(connection.Table<Animal>(), Animais);
                await PopulateMapAsync(connection.Table<Lactacao>(), Lactacoes);
                await PopulateMapAsync(connection.Table<ProducaoLeiteira>(), Producoes);
                await PopulateMapAsync(connection.Table<Gestacao>(), Gestacoes);
                await PopulateMapAsync(connection.Table<QuantidadeOrdenha>(), QuantidadesOrdenha);
            }

            private static async Task PopulateMapAsync<T>(AsyncTableQuery<T> query, Dictionary<Guid, int> map)
                where T : ISyncEntity, new()
            {
                var items = await query.ToListAsync();
                foreach (var item in items)
                {
                    if (item.SyncId == Guid.Empty)
                    {
                        continue;
                    }

                    map[item.SyncId] = item.Id;
                }
            }
        }
    }
}
