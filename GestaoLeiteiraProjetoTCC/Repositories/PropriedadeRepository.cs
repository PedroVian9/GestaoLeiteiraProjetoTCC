using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace GestaoLeiteiraProjetoTCC.Repositories
{
    public class PropriedadeRepository : IPropriedadeRepository
    {
        private readonly SQLiteAsyncConnection _database;

        public PropriedadeRepository(DatabaseService databaseService)
        {
            _database = databaseService.GetConnectionAsync().Result;
        }

        public async Task<Propriedade> CadastrarPropriedadeDb(Propriedade propriedade)
        {
            propriedade.DataCadastro = DateTime.Now;
            await _database.InsertAsync(propriedade);
            return propriedade;
        }

        public async Task<List<Propriedade>> ObterTodasPropriedadesDb()
        {
            return await _database.Table<Propriedade>().ToListAsync();
        }

        public async Task<Propriedade> ObterPropriedadePorIdDb(int id)
        {
            return await _database.FindAsync<Propriedade>(id);
        }

        public async Task<Propriedade> ValidarLoginDb(string nomeProprietario, string senha)
        {
            return await _database.Table<Propriedade>()
                .FirstOrDefaultAsync(p => p.NomeProprietario == nomeProprietario && p.Senha == senha);
        }

        public async Task<List<Animal>> ObterAnimaisPorPropriedadeIdDb(int propriedadeId)
        {
            return await _database.Table<Animal>()
                .Where(a => a.PropriedadeId == propriedadeId)
                .ToListAsync();
        }
    }
}