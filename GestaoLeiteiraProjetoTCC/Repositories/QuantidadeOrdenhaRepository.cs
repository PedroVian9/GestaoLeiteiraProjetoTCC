using GestaoLeiteiraProjetoTCC.Models;
using GestaoLeiteiraProjetoTCC.Repositories.Interfaces;
using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using GestaoLeiteiraProjetoTCC.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Repositories
{
    public class QuantidadeOrdenhaRepository : IQuantidadeOrdenhaRepository
    {
        private readonly DatabaseService _databaseService;
        private readonly ISyncMetadataService _syncMetadataService;

        public QuantidadeOrdenhaRepository(DatabaseService databaseService, ISyncMetadataService syncMetadataService)
        {
            _databaseService = databaseService;
            _syncMetadataService = syncMetadataService;
        }

        public async Task<QuantidadeOrdenha> CadastrarAsync(QuantidadeOrdenha quantidadeOrdenha)
        {
            var db = await _databaseService.GetConnectionAsync();
            quantidadeOrdenha.DataRegistro = DateTime.UtcNow;
            SyncEntityHelper.Touch(quantidadeOrdenha, _syncMetadataService.GetDeviceId());
            await db.InsertAsync(quantidadeOrdenha);
            return quantidadeOrdenha;
        }

        public async Task<QuantidadeOrdenha> ObterMaisRecenteAsync()
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.Table<QuantidadeOrdenha>()
                           .Where(q => !q.IsDeleted)
                           .OrderByDescending(q => q.DataRegistro)
                           .FirstOrDefaultAsync();
        }
    }
}
