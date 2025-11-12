using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class SyncAutomationService : ISyncAutomationService, IDisposable
    {
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan ForceSyncInterval = TimeSpan.FromMinutes(5);

        private readonly ISyncService _syncService;
        private readonly ISyncMetadataService _metadataService;
        private readonly ISyncTransportService _transportService;
        private readonly string _deviceId;
        private readonly CancellationTokenSource _cts = new();

        private PeriodicTimer? _timer;
        private Task? _loopTask;
        private bool _started;
        private bool _syncInProgress;
        private bool _remoteReady;
        private bool _connected;
        private string? _sharedFolderPath;

        private SyncAutomationStatus _currentStatus = SyncAutomationStatus.Initial("Pressione 'Conectar dispositivo' para iniciar a sincronizacao.");

        public SyncAutomationService(
            ISyncService syncService,
            ISyncMetadataService metadataService,
            ISyncTransportService transportService)
        {
            _syncService = syncService;
            _metadataService = metadataService;
            _transportService = transportService;
            _deviceId = _metadataService.GetDeviceId();
        }

        public SyncAutomationStatus CurrentStatus => _currentStatus;

        public event EventHandler<SyncAutomationStatus>? StatusChanged;

        public async Task<SyncAutomationStatus> ConnectAsync(string? reason = null)
        {
            var message = reason ?? "Iniciando conexao com o dispositivo...";
            UpdateStatus(SyncAutomationState.Initializing, message, false, false);

            var available = await PrepareSharedFolderAsync(isRetry: false);
            if (!available)
            {
                UpdateStatus(SyncAutomationState.NotConnected, "Nao foi possivel localizar a pasta de sincronizacao. Conecte o celular via USB e certifique-se de que a pasta GestaoLeiteiraSync exista.", false, false);
                return _currentStatus;
            }

            _connected = true;
            await EnsureLoopStartedAsync();
            UpdateStatus(SyncAutomationState.WaitingForDevice, "Aguardando o outro dispositivo abrir o aplicativo.", _remoteReady, false);
            return _currentStatus;
        }

        public Task StartAsync()
        {
            if (_started)
            {
                return Task.CompletedTask;
            }

            _started = true;
            _timer = new PeriodicTimer(PollInterval);
            _loopTask = Task.Run(() => RunAsync(_cts.Token));
            return Task.CompletedTask;
        }

        public async Task TriggerSyncNowAsync(string? reason = null)
        {
            if (!_connected)
            {
                UpdateStatus(SyncAutomationState.NotConnected, "Conecte o dispositivo antes de sincronizar.", false, false);
                return;
            }

            await EnsureLoopStartedAsync();

            var available = await PrepareSharedFolderAsync(isRetry: true);
            if (!available)
            {
                _connected = false;
                return;
            }

            await ExecuteSyncAsync(reason ?? "Sincronizacao solicitada pelo usuario");
        }

        private async Task RunAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await TickAsync(token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    UpdateStatus(SyncAutomationState.Error, $"Erro na rotina automatica: {ex.Message}", false, false);
                }

                if (_timer == null)
                {
                    break;
                }

                bool shouldContinue;
                try
                {
                    shouldContinue = await _timer.WaitForNextTickAsync(token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                if (!shouldContinue)
                {
                    break;
                }
            }
        }

        private async Task TickAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (!_connected)
            {
                UpdateStatus(SyncAutomationState.NotConnected, "Pressione 'Conectar dispositivo' antes de sincronizar.", false, false);
                return;
            }

            var folderAvailable = await PrepareSharedFolderAsync(isRetry: true);
            if (!folderAvailable)
            {
                _connected = false;
                UpdateStatus(SyncAutomationState.NotConnected, "Conexao perdida. Reconecte o dispositivo e tente novamente.", false, false);
                return;
            }

            _remoteReady = await _transportService.HasRemoteReadyMarkerAsync(_deviceId);
            var pendingPayload = await HasRemotePayloadAsync();

            if (!_remoteReady)
            {
                UpdateStatus(SyncAutomationState.WaitingForDevice, "Aguardando o outro dispositivo abrir o aplicativo.", false, pendingPayload);
                return;
            }

            if (!pendingPayload && !ShouldForceSync())
            {
                UpdateStatus(SyncAutomationState.Idle, "Sincronizacao automatica ativa.", true, false);
                return;
            }

            await ExecuteSyncAsync(pendingPayload ? "Atualizacao recebida do outro dispositivo" : "Rotina periodica automatica");
        }

        private async Task<bool> PrepareSharedFolderAsync(bool isRetry)
        {
            try
            {
                var path = await _transportService.GetSharedFolderPathAsync();
                _sharedFolderPath = path;
                await _transportService.EnsureSharedFolderExistsAsync();
                await _transportService.WriteReadyMarkerAsync(_deviceId);
                return true;
            }
            catch (Exception ex)
            {
                if (isRetry)
                {
                    UpdateStatus(SyncAutomationState.NotConnected, $"Conecte o outro dispositivo via USB. Detalhes: {ex.Message}", false, false);
                }
                return false;
            }
        }

        private bool ShouldForceSync()
        {
            var lastSuccess = _metadataService.GetLastSuccessfulSyncUtc();
            if (!lastSuccess.HasValue)
            {
                return true;
            }

            return DateTime.UtcNow - lastSuccess.Value > ForceSyncInterval;
        }

        private async Task<bool> HasRemotePayloadAsync()
        {
            try
            {
                var files = await _transportService.GetAvailablePayloadFilesAsync();
                return files.Any(file =>
                {
                    var name = Path.GetFileName(file);
                    return !string.IsNullOrWhiteSpace(name) &&
                           !name.Contains(_deviceId, StringComparison.OrdinalIgnoreCase);
                });
            }
            catch
            {
                return false;
            }
        }

        private async Task ExecuteSyncAsync(string reason)
        {
            if (!_connected)
            {
                UpdateStatus(SyncAutomationState.NotConnected, "Conecte o dispositivo antes de sincronizar.", false, false);
                return;
            }

            if (_syncInProgress)
            {
                return;
            }

            _syncInProgress = true;
            UpdateStatus(SyncAutomationState.SyncInProgress, $"{reason}...", _remoteReady, false);

            try
            {
                var result = await _syncService.SynchronizeAsync();
                var lastSuccess = _metadataService.GetLastSuccessfulSyncUtc();
                UpdateStatus(
                    SyncAutomationState.Idle,
                    result.Message,
                    _remoteReady,
                    false,
                    lastSuccessUtc: lastSuccess,
                    lastAttemptUtc: DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                UpdateStatus(SyncAutomationState.Error, $"Erro ao sincronizar automaticamente: {ex.Message}", _remoteReady, false);
            }
            finally
            {
                _syncInProgress = false;
            }
        }

        private void UpdateStatus(
            SyncAutomationState state,
            string message,
            bool remoteReady,
            bool pendingPayload,
            DateTime? lastSuccessUtc = null,
            DateTime? lastAttemptUtc = null)
        {
            var newStatus = new SyncAutomationStatus(
                state,
                message,
                lastSuccessUtc ?? _metadataService.GetLastSuccessfulSyncUtc(),
                lastAttemptUtc,
                remoteReady,
                pendingPayload,
                _sharedFolderPath);

            _currentStatus = newStatus;
            StatusChanged?.Invoke(this, newStatus);
        }

        private async Task EnsureLoopStartedAsync()
        {
            if (_started)
            {
                return;
            }

            await StartAsync();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _timer?.Dispose();
            _loopTask?.Dispose();
            _cts.Dispose();
        }
    }
}
