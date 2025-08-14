using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;

namespace GestaoLeiteiraProjetoTCC.Repositories
{
    public class ProducaoLeiteiraRepository : IProducaoLeiteiraRepository
    {
        private readonly DatabaseService _databaseService;

        public ProducaoLeiteiraRepository(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<int> CriarProducaoLeiteiraDb(ProducaoLeiteira producaoLeiteira)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.InsertAsync(producaoLeiteira);
        }

        public async Task AtualizarProducaoLeiteiraDb(ProducaoLeiteira producaoLeiteira)
        {
            var db = await _databaseService.GetConnectionAsync();
            await db.UpdateAsync(producaoLeiteira);
        }

        public async Task<List<ProducaoLeiteira>> ObterPorLactacaoAsync(int lactacaoId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<ProducaoLeiteira>()
                           .Where(p => p.LactacaoId == lactacaoId)
                           .ToListAsync();
        }

        public async Task<List<ProducaoLeiteira>> ObterPorPropriedadeAsync(int propriedadeId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<ProducaoLeiteira>()
                           .Where(p => p.PropriedadeId == propriedadeId)
                           .ToListAsync();
        }

        public async Task<List<ProducaoLeiteira>> ObterProducoesPorLactacaoNoDiaAsync(int lactacaoId, DateTime dia)
        {
            // Garante que estamos comparando apenas a data, ignorando a hora.
            var db = await _databaseService.GetConnectionAsync();
            var inicioDoDia = dia.Date;
            var fimDoDia = dia.Date.AddDays(1).AddTicks(-1);

            return await db.Table<ProducaoLeiteira>()
                                  .Where(p => p.LactacaoId == lactacaoId && p.Data >= inicioDoDia && p.Data <= fimDoDia)
                                  .ToListAsync();
        }
    }
}