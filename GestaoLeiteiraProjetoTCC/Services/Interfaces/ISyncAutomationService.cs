using GestaoLeiteiraProjetoTCC.Services;
using System;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services.Interfaces
{
    public interface ISyncAutomationService
    {
        SyncAutomationStatus CurrentStatus { get; }
        event EventHandler<SyncAutomationStatus>? StatusChanged;
        Task<SyncAutomationStatus> ConnectAsync(string? reason = null);
        Task StartAsync();
        Task TriggerSyncNowAsync(string? reason = null);
    }
}
