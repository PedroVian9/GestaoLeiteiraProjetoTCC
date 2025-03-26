using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Repositories
{
    public class LactacaoRepository : ILactacaoRepository
    {
        private readonly SQLiteAsyncConnection _database;

        public LactacaoRepository(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public async Task<int> CriarLactacaoDb(Lactacao lactacao)
        {
            return await _database.InsertAsync(lactacao);
        }

        public async Task<List<Lactacao>> ObterLactacoesPorAnimalDb(int animalId)
        {
            return await _database.Table<Lactacao>()
                .Where(l => l.AnimalId == animalId)
                .ToListAsync();
        }

        public async Task<Lactacao> ObterLactacaoPorIdDb(int id)
        {
            return await _database.Table<Lactacao>()
                .Where(l => l.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task AtualizarLactacaoDb(Lactacao lactacao)
        {
            await _database.UpdateAsync(lactacao);
        }
    }
}