using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using SQLite;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Repositories
{
    public class ProducaoLeiteiraRepository : IProducaoLeiteiraRepository
    {
        private readonly SQLiteAsyncConnection _database;

        public ProducaoLeiteiraRepository(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public async Task<int> CriarProducaoLeiteiraDb(ProducaoLeiteira producaoLeiteira)
        {
            return await _database.InsertAsync(producaoLeiteira);
        }

        public async Task AtualizarProducaoLeiteiraDb(ProducaoLeiteira producaoLeiteira)
        {
            await _database.UpdateAsync(producaoLeiteira);
        }
    }
}