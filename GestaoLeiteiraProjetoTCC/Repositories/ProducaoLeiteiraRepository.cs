using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using GestaoLeiteiraProjetoTCC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Repositories
{
    public class ProducaoLeiteiraRepository : IProducaoLeiteiraRepository
    {
        private readonly DatabaseService _databaseService;
        private readonly ISyncMetadataService _syncMetadataService;

        public ProducaoLeiteiraRepository(DatabaseService databaseService, ISyncMetadataService syncMetadataService)
        {
            _databaseService = databaseService;
            _syncMetadataService = syncMetadataService;
        }

        public async Task<int> CriarProducaoLeiteiraDb(ProducaoLeiteira producaoLeiteira)
        {
            var db = await _databaseService.GetConnectionAsync();
            SyncEntityHelper.Touch(producaoLeiteira, _syncMetadataService.GetDeviceId());
            return await db.InsertAsync(producaoLeiteira);
        }

        public async Task AtualizarProducaoLeiteiraDb(ProducaoLeiteira producaoLeiteira)
        {
            var db = await _databaseService.GetConnectionAsync();
            SyncEntityHelper.Touch(producaoLeiteira, _syncMetadataService.GetDeviceId());
            await db.UpdateAsync(producaoLeiteira);
        }

        public async Task<List<ProducaoLeiteira>> ObterPorLactacaoAsync(int lactacaoId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<ProducaoLeiteira>()
                           .Where(p => p.LactacaoId == lactacaoId && !p.IsDeleted)
                           .ToListAsync();
        }

        public async Task<List<ProducaoLeiteira>> ObterPorPropriedadeDb(int propriedadeId, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var db = await _databaseService.GetConnectionAsync();
            var query = db.Table<ProducaoLeiteira>()
                          .Where(p => p.PropriedadeId == propriedadeId && !p.IsDeleted);

            if (dataInicio.HasValue)
            {
                query = query.Where(p => p.Data.Date >= dataInicio.Value.Date);
            }

            if (dataFim.HasValue)
            {
                query = query.Where(p => p.Data.Date <= dataFim.Value.Date);
            }

            return await query.OrderByDescending(p => p.Data).ToListAsync();
        }

        public async Task<List<ProducaoLeiteira>> ObterProducoesPorLactacaoNoDiaAsync(int lactacaoId, DateTime dia)
        {
            var db = await _databaseService.GetConnectionAsync();
            var inicioDoDia = dia.Date;
            var fimDoDia = dia.Date.AddDays(1).AddTicks(-1);

            return await db.Table<ProducaoLeiteira>()
                           .Where(p => p.LactacaoId == lactacaoId &&
                                       !p.IsDeleted &&
                                       p.Data >= inicioDoDia &&
                                       p.Data <= fimDoDia)
                           .ToListAsync();
        }
    }
}
