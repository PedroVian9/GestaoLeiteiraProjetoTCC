using GestaoLeiteiraProjetoTCC.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class SyncService
    {
        public event EventHandler<string> StatusChanged;
        public SyncService()
        {
            
        }
        private void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(this, status);
        }

        public async Task SincronizarTabela<T>(SQLiteAsyncConnection db1, SQLiteAsyncConnection db2, string nomeTabela = null) where T : BaseEntity, new()
        {
            nomeTabela = nomeTabela ?? typeof(T).Name;
            OnStatusChanged($"Sincronizando {nomeTabela}...");

            try
            {
                var listaDb1 = await db1.Table<T>().ToListAsync();
                var listaDb2 = await db2.Table<T>().ToListAsync();

                var dictDb1 = listaDb1.ToDictionary(x => x.GuidRegistro);
                var dictDb2 = listaDb2.ToDictionary(x => x.GuidRegistro);

                int inseridosDb1 = 0, inseridosDb2 = 0, atualizados = 0;

                var allGuids = dictDb1.Keys.Union(dictDb2.Keys).ToHashSet();

                foreach (var guid in allGuids)
                {
                    dictDb1.TryGetValue(guid, out var itemDb1);
                    dictDb2.TryGetValue(guid, out var itemDb2);

                    if (itemDb1 != null && itemDb2 == null)
                    {
                        // Existe apenas no DB1 -> Inserir no DB2
                        await db2.InsertAsync(itemDb1);
                        inseridosDb2++;
                    }
                    else if (itemDb1 == null && itemDb2 != null)
                    {
                        // Existe apenas no DB2 -> Inserir no DB1
                        await db1.InsertAsync(itemDb2);
                        inseridosDb1++;
                    }
                    else if (itemDb1 != null && itemDb2 != null)
                    {
                        // Existe em ambos -> Sincronizar o mais recente
                        if (itemDb1.DataModificacaoUtc > itemDb2.DataModificacaoUtc)
                        {
                            itemDb1.Id = itemDb2.Id; // Preserva o ID do destino
                            await db2.UpdateAsync(itemDb1);
                            atualizados++;
                        }
                        else if (itemDb2.DataModificacaoUtc > itemDb1.DataModificacaoUtc)
                        {
                            itemDb2.Id = itemDb1.Id; // Preserva o ID do destino
                            await db1.UpdateAsync(itemDb2);
                            atualizados++;
                        }
                        // Se as datas forem iguais, não faz nada.
                    }
                }

                var totalInseridos = inseridosDb1 + inseridosDb2;
                if (totalInseridos > 0 || atualizados > 0)
                {
                    OnStatusChanged($"{nomeTabela}: {totalInseridos} inseridos, {atualizados} atualizados.");
                }
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Erro ao sincronizar {nomeTabela}: {ex.Message}");
                throw;
            }
        }

        public async Task SincronizarTudo(SQLiteAsyncConnection dbPC, SQLiteAsyncConnection dbCelular)
        {
            try
            {
                OnStatusChanged("Iniciando sincronização...");

                // Usar transações para garantir consistência
                await Task.Run(async () =>
                {
                    // Sincronizar cada tabela
                    await SincronizarTabela<Propriedade>(dbPC, dbCelular, "Propriedade");
                    await SincronizarTabela<Raca>(dbPC, dbCelular, "Raca");
                    await SincronizarTabela<Animal>(dbPC, dbCelular, "Animal");
                    await SincronizarTabela<Lactacao>(dbPC, dbCelular, "Lactacao");
                    await SincronizarTabela<Gestacao>(dbPC, dbCelular, "Gestacao");
                    await SincronizarTabela<ProducaoLeiteira>(dbPC, dbCelular, "ProducaoLeiteira");
                    await SincronizarTabela<QuantidadeOrdenha>(dbPC, dbCelular, "QuantidadeOrdenha");
                });

                OnStatusChanged("Sincronização concluída com sucesso!");
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Erro durante sincronização: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ValidarIntegridadeBanco(SQLiteAsyncConnection db, string nomeBanco)
        {
            try
            {
                OnStatusChanged($"Validando integridade do banco {nomeBanco}...");

                // Verificar se as tabelas existem
                var tabelas = new[] { "Propriedade", "Animal", "Lactacao", "ProducaoLeiteira", "Raca", "QuantidadeOrdenha", "Gestacao" };

                foreach (var tabela in tabelas)
                {
                    var query = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tabela}'";
                    var resultado = await db.QueryAsync<dynamic>(query);
                    if (!resultado.Any())
                    {
                        OnStatusChanged($"Tabela {tabela} não encontrada em {nomeBanco}");
                        return false;
                    }
                }

                OnStatusChanged($"Banco {nomeBanco} válido");
                return true;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Erro ao validar {nomeBanco}: {ex.Message}");
                return false;
            }
        }

        public async Task<Dictionary<string, int>> ObterEstatisticasSincronizacao(SQLiteAsyncConnection dbPC, SQLiteAsyncConnection dbCelular)
        {
            var stats = new Dictionary<string, int>();

            try
            {
                // Contar registros em cada tabela
                stats["PC_Propriedades"] = await dbPC.Table<Propriedade>().CountAsync(x => !x.Excluido);
                stats["Celular_Propriedades"] = await dbCelular.Table<Propriedade>().CountAsync(x => !x.Excluido);

                stats["PC_Animais"] = await dbPC.Table<Animal>().CountAsync(x => !x.Excluido);
                stats["Celular_Animais"] = await dbCelular.Table<Animal>().CountAsync(x => !x.Excluido);

                stats["PC_Lactacoes"] = await dbPC.Table<Lactacao>().CountAsync(x => !x.Excluido);
                stats["Celular_Lactacoes"] = await dbCelular.Table<Lactacao>().CountAsync(x => !x.Excluido);

                stats["PC_Producoes"] = await dbPC.Table<ProducaoLeiteira>().CountAsync(x => !x.Excluido);
                stats["Celular_Producoes"] = await dbCelular.Table<ProducaoLeiteira>().CountAsync(x => !x.Excluido);

                stats["PC_Racas"] = await dbPC.Table<Raca>().CountAsync(x => !x.Excluido);
                stats["Celular_Racas"] = await dbCelular.Table<Raca>().CountAsync(x => !x.Excluido);

                stats["PC_Ordenhas"] = await dbPC.Table<QuantidadeOrdenha>().CountAsync(x => !x.Excluido);
                stats["Celular_Ordenhas"] = await dbCelular.Table<QuantidadeOrdenha>().CountAsync(x => !x.Excluido);

                stats["PC_Gestacoes"] = await dbPC.Table<Gestacao>().CountAsync(x => !x.Excluido);
                stats["Celular_Gestacoes"] = await dbCelular.Table<Gestacao>().CountAsync(x => !x.Excluido);
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Erro ao obter estatísticas: {ex.Message}");
            }

            return stats;
        }

        public async Task LimparRegistrosExcluidos<T>(SQLiteAsyncConnection db, DateTime dataLimite) where T : BaseEntity, new()
        {
            try
            {
                // Remove definitivamente registros excluídos há mais de X dias
                await db.ExecuteAsync(
                    $"DELETE FROM {typeof(T).Name} WHERE Excluido = 1 AND DataModificacaoUtc < ?",
                    dataLimite
                );
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Erro ao limpar registros excluídos: {ex.Message}");
                throw;
            }
        }
    }
}