using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using GestaoLeiteiraProjetoTCC.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Repositories
{
    public class LactacaoRepository : ILactacaoRepository
    {
        private readonly DatabaseService _databaseService;
        private readonly ISyncMetadataService _syncMetadataService;

        public LactacaoRepository(DatabaseService databaseService, ISyncMetadataService syncMetadataService)
        {
            _databaseService = databaseService;
            _syncMetadataService = syncMetadataService;
        }

        public async Task<int> CriarLactacaoDb(Lactacao lactacao)
        {
            var db = await _databaseService.GetConnectionAsync();
            SyncEntityHelper.Touch(lactacao, _syncMetadataService.GetDeviceId());
            return await db.InsertAsync(lactacao);
        }

        public async Task<List<Lactacao>> ObterLactacoesPorAnimalDb(int animalId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Lactacao>()
                           .Where(l => l.AnimalId == animalId && !l.IsDeleted)
                           .ToListAsync();
        }

        public async Task<Lactacao> ObterLactacaoPorIdDb(int id)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Lactacao>()
                           .Where(l => l.Id == id && !l.IsDeleted)
                           .FirstOrDefaultAsync();
        }

        public async Task AtualizarLactacaoDb(Lactacao lactacao)
        {
            var db = await _databaseService.GetConnectionAsync();
            SyncEntityHelper.Touch(lactacao, _syncMetadataService.GetDeviceId());
            await db.UpdateAsync(lactacao);
        }

        public async Task<List<Lactacao>> ObterTodasLactacoesAtivasDb(int propriedadeId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Lactacao>()
                           .Where(l => l.PropriedadeId == propriedadeId &&
                                       l.DataFim == null &&
                                       !l.IsDeleted)
                           .ToListAsync();
        }

        public async Task<List<Lactacao>> ObterLactacoesDaPropriedadeDb(int propriedadeId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Lactacao>()
                           .Where(l => l.PropriedadeId == propriedadeId && !l.IsDeleted)
                           .ToListAsync();
        }
    }
}
