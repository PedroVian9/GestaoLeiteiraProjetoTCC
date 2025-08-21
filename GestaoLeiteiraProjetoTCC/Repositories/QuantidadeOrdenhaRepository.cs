using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;

namespace GestaoLeiteiraProjetoTCC.Repositories
{
    public class QuantidadeOrdenhaRepository : IQuantidadeOrdenhaRepository
    {
        private readonly DatabaseService _databaseService;

        public QuantidadeOrdenhaRepository(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<QuantidadeOrdenha> CadastrarAsync(QuantidadeOrdenha quantidadeOrdenha)
        {
            var db = await _databaseService.GetConnectionAsync();
            quantidadeOrdenha.DataModificacaoUtc = DateTime.UtcNow;
            await db.InsertAsync(quantidadeOrdenha);
            return quantidadeOrdenha;
        }

        public async Task<QuantidadeOrdenha> ObterMaisRecenteAsync()
        {
            // Ordena os registros pela DataRegistro em ordem decrescente e pega o primeiro
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<QuantidadeOrdenha>()
                                  .OrderByDescending(q => q.DataRegistro)
                                  .FirstOrDefaultAsync();
        }
    }
}