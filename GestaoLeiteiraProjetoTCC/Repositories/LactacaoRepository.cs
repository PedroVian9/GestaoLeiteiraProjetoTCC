using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;

namespace GestaoLeiteiraProjetoTCC.Repositories
{
    public class LactacaoRepository : ILactacaoRepository
    {
        private readonly DatabaseService _databaseService;

        public LactacaoRepository(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<int> CriarLactacaoDb(Lactacao lactacao)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.InsertAsync(lactacao);
        }

        public async Task<List<Lactacao>> ObterLactacoesPorAnimalDb(int animalId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Lactacao>()
                .Where(l => l.AnimalId == animalId)
                .ToListAsync();
        }

        public async Task<Lactacao> ObterLactacaoPorIdDb(int id)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Lactacao>()
                .Where(l => l.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task AtualizarLactacaoDb(Lactacao lactacao)
        {
            var db = await _databaseService.GetConnectionAsync();
            await db.UpdateAsync(lactacao);
        }

        public async Task<List<Lactacao>> ObterTodasLactacoesAtivasDb(int propriedadeId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Lactacao>()
                                  .Where(l => l.PropriedadeId == propriedadeId && l.DataFim == null)
                                  .ToListAsync();
        }
    }
}