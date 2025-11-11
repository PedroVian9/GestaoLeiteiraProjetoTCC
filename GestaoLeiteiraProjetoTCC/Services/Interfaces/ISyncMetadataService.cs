using System;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services.Interfaces
{
    public interface ISyncMetadataService
    {
        string GetDeviceId();
        string GetDeviceName();
        string GetPlatform();
        DateTime? GetLastSuccessfulSyncUtc();
        Task SetLastSuccessfulSyncUtcAsync(DateTime? timestampUtc);
    }
}
