using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class PropriedadeService : IPropriedadeService
    {
        private readonly SQLiteAsyncConnection _database;
        private Propriedade _propriedadeAtual;

        public PropriedadeService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Propriedade>().Wait();
        }

        public async Task<Propriedade> CadastrarPropriedadeAsync(Propriedade propriedade)
        {
            propriedade.DataCadastro = DateTime.Now;
          
            await _database.InsertAsync(propriedade);
            _propriedadeAtual = propriedade;
            return propriedade;
        }

        public async Task<List<Propriedade>> ObterTodasPropriedadesAsync()
        {
            return await _database.Table<Propriedade>().ToListAsync();
        }

        public async Task<Propriedade> ObterPropriedadePorIdAsync(int id)
        {
            return await _database.FindAsync<Propriedade>(id);
        }
    }
}
