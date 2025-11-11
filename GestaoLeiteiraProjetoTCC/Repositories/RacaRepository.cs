using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using GestaoLeiteiraProjetoTCC.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Repositories
{
    public class RacaRepository : IRacaRepository
    {
        private readonly DatabaseService _databaseService;
        private readonly ISyncMetadataService _syncMetadataService;

        public RacaRepository(DatabaseService databaseService, ISyncMetadataService syncMetadataService)
        {
            _databaseService = databaseService;
            _syncMetadataService = syncMetadataService;
        }

        public async Task<bool> ExisteRacaAsync(string nomeRaca)
        {
            var db = await _databaseService.GetConnectionAsync();
            var lowerNome = nomeRaca?.ToLowerInvariant() ?? string.Empty;

            var count = await db.Table<Raca>()
                                .Where(r => !r.IsDeleted && r.NomeRaca.ToLower() == lowerNome)
                                .CountAsync();

            return count > 0;
        }

        public async Task CriarAsync(Raca raca)
        {
            var db = await _databaseService.GetConnectionAsync();
            SyncEntityHelper.Touch(raca, _syncMetadataService.GetDeviceId());
            await db.InsertAsync(raca);
        }

        public async Task<List<Raca>> ObterTodasOrdenadasPorNomeAsync()
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Raca>()
                           .Where(r => !r.IsDeleted)
                           .OrderBy(r => r.NomeRaca)
                           .ToListAsync();
        }

        public async Task<Raca> ObterPorIdAsync(int id)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<Raca>()
                           .Where(r => r.Id == id && !r.IsDeleted)
                           .FirstOrDefaultAsync();
        }

        public async Task AtualizarAsync(Raca raca)
        {
            var db = await _databaseService.GetConnectionAsync();
            SyncEntityHelper.Touch(raca, _syncMetadataService.GetDeviceId());
            await db.UpdateAsync(raca);
        }

        public async Task ExcluirAsync(int id)
        {
            var db = await _databaseService.GetConnectionAsync();
            var raca = await db.Table<Raca>()
                               .Where(r => r.Id == id && !r.IsDeleted)
                               .FirstOrDefaultAsync();

            if (raca == null)
            {
                return;
            }

            SyncEntityHelper.MarkDeleted(raca, _syncMetadataService.GetDeviceId());
            await db.UpdateAsync(raca);
        }
    }
}
