using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using GestaoLeiteiraProjetoTCC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Repositories
{
    public class PropriedadeRepository : IPropriedadeRepository
    {
        private readonly DatabaseService _databaseService;
        private readonly ISyncMetadataService _syncMetadataService;

        public PropriedadeRepository(DatabaseService databaseService, ISyncMetadataService syncMetadataService)
        {
            _databaseService = databaseService;
            _syncMetadataService = syncMetadataService;
        }

        public async Task<Propriedade> CadastrarPropriedadeDb(Propriedade propriedade)
        {
            var db = await _databaseService.GetConnectionAsync();
            propriedade.DataCadastro = DateTime.UtcNow;
            SyncEntityHelper.Touch(propriedade, _syncMetadataService.GetDeviceId());
            await db.InsertAsync(propriedade);
            return propriedade;
        }

        public async Task<List<Propriedade>> ObterTodasPropriedadesDb()
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Propriedade>()
                           .Where(p => !p.IsDeleted)
                           .ToListAsync();
        }

        public async Task<Propriedade> ObterPropriedadePorIdDb(int id)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Propriedade>()
                           .Where(p => p.Id == id && !p.IsDeleted)
                           .FirstOrDefaultAsync();
        }

        public async Task<Propriedade> ValidarLoginDb(string nomeProprietario, string senha)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Propriedade>()
                           .FirstOrDefaultAsync(p => p.NomeProprietario == nomeProprietario &&
                                                     p.Senha == senha &&
                                                     !p.IsDeleted);
        }

        public async Task<List<Animal>> ObterAnimaisPorPropriedadeIdDb(int propriedadeId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Animal>()
                           .Where(a => a.PropriedadeId == propriedadeId && !a.IsDeleted)
                           .ToListAsync();
        }

        public async Task<bool> EditarPropriedadeDb(Propriedade propriedade)
        {
            var db = await _databaseService.GetConnectionAsync();
            var existente = await db.Table<Propriedade>()
                                    .Where(p => p.Id == propriedade.Id && !p.IsDeleted)
                                    .FirstOrDefaultAsync();

            if (existente == null)
            {
                return false;
            }

            existente.NomeProprietario = propriedade.NomeProprietario;
            existente.NomeSocial = propriedade.NomeSocial;
            existente.Sexo = propriedade.Sexo;
            existente.NomePropriedade = propriedade.NomePropriedade;
            existente.Localizacao = propriedade.Localizacao;
            existente.AreaTotal = propriedade.AreaTotal;
            existente.TipoUnidade = propriedade.TipoUnidade;

            SyncEntityHelper.Touch(existente, _syncMetadataService.GetDeviceId());
            return await db.UpdateAsync(existente) > 0;
        }

        public async Task<bool> AlterarSenhaDb(int id, string novaSenha)
        {
            var db = await _databaseService.GetConnectionAsync();
            var existente = await db.Table<Propriedade>()
                                    .Where(p => p.Id == id && !p.IsDeleted)
                                    .FirstOrDefaultAsync();

            if (existente == null)
            {
                return false;
            }

            existente.Senha = novaSenha;
            SyncEntityHelper.Touch(existente, _syncMetadataService.GetDeviceId());
            return await db.UpdateAsync(existente) > 0;
        }
    }
}
