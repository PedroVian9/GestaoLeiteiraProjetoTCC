using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using GestaoLeiteiraProjetoTCC.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Repositories
{
    public class GestacaoRepository : IGestacaoRepository
    {
        private readonly DatabaseService _databaseService;
        private readonly ISyncMetadataService _syncMetadataService;

        public GestacaoRepository(DatabaseService databaseService, ISyncMetadataService syncMetadataService)
        {
            _databaseService = databaseService;
            _syncMetadataService = syncMetadataService;
        }

        public async Task<List<Gestacao>> ObterCiclosAtivosPorPropriedadeDb(List<int> idsAnimaisDaPropriedade)
        {
            var db = await _databaseService.GetConnectionAsync();
            var activeStatuses = new List<string> { "Em Cobertura", "Gesta\u00E7\u00E3o Ativa" };

            return await db.Table<Gestacao>()
                           .Where(g => !g.IsDeleted &&
                                       idsAnimaisDaPropriedade.Contains(g.VacaId) &&
                                       activeStatuses.Contains(g.Status))
                           .ToListAsync();
        }

        public async Task<Gestacao> IniciarGestacaoDb(Gestacao gestacao)
        {
            var db = await _databaseService.GetConnectionAsync();
            SyncEntityHelper.Touch(gestacao, _syncMetadataService.GetDeviceId());
            await db.InsertAsync(gestacao);
            return gestacao;
        }

        public async Task<Gestacao> AtualizarGestacaoDb(Gestacao gestacao)
        {
            var db = await _databaseService.GetConnectionAsync();
            SyncEntityHelper.Touch(gestacao, _syncMetadataService.GetDeviceId());
            await db.UpdateAsync(gestacao);
            return gestacao;
        }

        public async Task<Gestacao> ObterGestacaoPorIdDb(int gestacaoId)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Gestacao>()
                           .Where(g => g.Id == gestacaoId && !g.IsDeleted)
                           .FirstOrDefaultAsync();
        }
    }
}
