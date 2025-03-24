using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Repositories
{
    public class PropriedadeRepository : IPropriedadeRepository
    {
        private readonly SQLiteAsyncConnection _database;

        public PropriedadeRepository(DatabaseService databaseService)
        {
            _database = databaseService.GetConnection();
        }

        public async Task<Propriedade> CadastrarPropriedadeDb(Propriedade propriedade)
        {
            propriedade.DataCadastro = DateTime.Now;
            await _database.InsertAsync(propriedade);
            return propriedade; // 🔹 Agora retorna a propriedade cadastrada
        }

        public async Task<List<Propriedade>> ObterTodasPropriedadesDb()
        {
            return await _database.Table<Propriedade>().ToListAsync();
        }

        public async Task<Propriedade> ObterPropriedadePorIdDb(int id)
        {
            return await _database.FindAsync<Propriedade>(id);
        }
    }
}
