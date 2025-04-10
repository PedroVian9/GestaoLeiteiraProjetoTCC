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
    }
}