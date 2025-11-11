using GestaoLeiteiraProjetoTCC.Services;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services.Interfaces
{
    public interface ISyncService
    {
        Task<SyncResult> SynchronizeAsync(CancellationToken cancellationToken = default);
    }
}
