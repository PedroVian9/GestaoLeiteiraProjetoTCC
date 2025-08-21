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
            propriedade.DataModificacaoUtc = DateTime.UtcNow;
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

        public async Task<bool> EditarPropriedadeDb(Propriedade propriedade)
        {
            var existente = await _database.FindAsync<Propriedade>(propriedade.Id);
            if (existente == null) return false;

            existente.NomeProprietario = propriedade.NomeProprietario;
            existente.NomePropriedade = propriedade.NomePropriedade;
            existente.Localizacao = propriedade.Localizacao;
            existente.AreaTotal = propriedade.AreaTotal;
            existente.TipoUnidade = propriedade.TipoUnidade;
            existente.DataModificacaoUtc = DateTime.UtcNow;

            return await _database.UpdateAsync(existente) > 0;
        }

        public async Task<bool> AlterarSenhaDb(int id, string novaSenha)
        {
            var existente = await _database.FindAsync<Propriedade>(id);
            if (existente == null) return false;

            existente.Senha = novaSenha;
            existente.DataModificacaoUtc = DateTime.UtcNow;
            return await _database.UpdateAsync(existente) > 0;
        }

        public async Task<List<ProducaoLeiteira>> ObterProducoesPorLactacaoNoDiaAsync(int lactacaoId, DateTime dia)
        {
            // Garante que estamos comparando apenas a data, ignorando a hora.
            var inicioDoDia = dia.Date;
            var fimDoDia = dia.Date.AddDays(1).AddTicks(-1);

            return await _database.Table<ProducaoLeiteira>()
                                  .Where(p => p.LactacaoId == lactacaoId && p.Data >= inicioDoDia && p.Data <= fimDoDia)
                                  .ToListAsync();
        }
    }
}