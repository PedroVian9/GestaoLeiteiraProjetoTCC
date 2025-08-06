using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using SQLite;

namespace GestaoLeiteiraProjetoTCC.Repositories
{
    public class RacaRepository : IRacaRepository
    {
        private readonly SQLiteAsyncConnection _database;

        public RacaRepository(DatabaseService databaseService)
        {
            _database = databaseService.GetConnectionAsync().Result;
        }

        public async Task<bool> ExisteRacaAsync(string nomeRaca)
        {
            var count = await _database.Table<Raca>()
                .Where(r => r.NomeRaca.ToLower() == nomeRaca.ToLower())
                .CountAsync();

            return count > 0;
        }

        public async Task CriarAsync(Raca raca)
        {
            await _database.InsertAsync(raca);
        }

        public async Task<List<Raca>> ObterTodasOrdenadasPorNomeAsync()
        {
            return await _database.Table<Raca>()
                .OrderBy(r => r.NomeRaca)
                .ToListAsync();
        }

        public async Task<Raca> ObterPorIdAsync(int id)
        {
            return await _database.FindAsync<Raca>(id);
        }

        public async Task AtualizarAsync(Raca raca)
        {
            await _database.UpdateAsync(raca);
        }

        public async Task ExcluirAsync(int id)
        {
            var raca = await ObterPorIdAsync(id);
            if (raca != null)
                await _database.DeleteAsync(raca);
        }
    }
}