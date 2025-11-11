using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services.Interfaces
{
    public interface ISyncTransportService
    {
        Task EnsureSharedFolderExistsAsync();
        Task<string> WriteOutboundPayloadAsync(string fileName, string content);
        Task<IReadOnlyList<string>> GetAvailablePayloadFilesAsync();
        Task<string?> ReadPayloadAsync(string filePath);
        Task DeletePayloadAsync(string filePath);
        Task<string> GetSharedFolderPathAsync();
    }
}
