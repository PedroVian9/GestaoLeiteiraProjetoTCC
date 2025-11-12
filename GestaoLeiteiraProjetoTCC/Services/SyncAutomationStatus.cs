using System;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public enum SyncAutomationState
    {
        NotConnected,
        Initializing,
        WaitingForDevice,
        Idle,
        SyncInProgress,
        Error
    }

    public record SyncAutomationStatus(
        SyncAutomationState State,
        string Message,
        DateTime? LastSuccessfulSyncUtc,
        DateTime? LastAttemptUtc,
        bool RemoteDeviceReady,
        bool PendingPayload,
        string? SharedFolderPath)
    {
        public static SyncAutomationStatus Initial(string message) =>
            new(SyncAutomationState.NotConnected, message, null, null, false, false, null);

        public static SyncAutomationStatus NotConnected(string message) =>
            new(SyncAutomationState.NotConnected, message, null, null, false, false, null);
    }
}
