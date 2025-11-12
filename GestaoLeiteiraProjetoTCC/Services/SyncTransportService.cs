using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#if WINDOWS
using Windows.Storage;
#endif

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class SyncTransportService : ISyncTransportService
    {
        private const string SyncFolderName = "GestaoLeiteiraSync";
        private const string ReadyMarkerPrefix = "device_";
        private const string ReadyMarkerExtension = ".ready";

        private readonly SemaphoreSlim _pathSemaphore = new(1, 1);
        private string? _cachedPath;

        public async Task EnsureSharedFolderExistsAsync()
        {
            var path = await GetSharedFolderPathAsync();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public async Task<string?> TryGetSharedFolderPathAsync()
        {
            try
            {
                return await ResolveSharedFolderPathAsync();
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetSharedFolderPathAsync()
        {
            var path = await ResolveSharedFolderPathAsync();
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new InvalidOperationException("Pasta de sincronização indisponível.");
            }

            return path;
        }

        public async Task<string> WriteOutboundPayloadAsync(string fileName, string content)
        {
            var folder = await GetSharedFolderPathAsync();
            var path = Path.Combine(folder, fileName);
            await File.WriteAllTextAsync(path, content);
            return path;
        }

        public async Task<IReadOnlyList<string>> GetAvailablePayloadFilesAsync()
        {
            var folder = await GetSharedFolderPathAsync();
            if (!Directory.Exists(folder))
            {
                return Array.Empty<string>();
            }

            try
            {
                var files = Directory.GetFiles(folder, "*.json")
                                     .OrderByDescending(File.GetCreationTimeUtc)
                                     .ToList();
                return files;
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        public async Task<string?> ReadPayloadAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            return await File.ReadAllTextAsync(filePath);
        }

        public Task DeletePayloadAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }

        public async Task WriteReadyMarkerAsync(string deviceId)
        {
            var folder = await GetSharedFolderPathAsync();
            Directory.CreateDirectory(folder);
            var markerPath = Path.Combine(folder, $"{ReadyMarkerPrefix}{deviceId}{ReadyMarkerExtension}");
            await File.WriteAllTextAsync(markerPath, DateTime.UtcNow.ToString("O"));
        }

        public async Task<bool> HasRemoteReadyMarkerAsync(string deviceId)
        {
            var folder = await GetSharedFolderPathAsync();
            if (!Directory.Exists(folder))
            {
                return false;
            }

            var markers = Directory.GetFiles(folder, $"{ReadyMarkerPrefix}*{ReadyMarkerExtension}");
            return markers.Any(marker =>
            {
                var name = Path.GetFileNameWithoutExtension(marker);
                if (string.IsNullOrWhiteSpace(name))
                {
                    return false;
                }

                var id = name.Replace(ReadyMarkerPrefix, string.Empty, StringComparison.OrdinalIgnoreCase);
                return !string.Equals(id, deviceId, StringComparison.OrdinalIgnoreCase);
            });
        }

        private async Task<string?> ResolveSharedFolderPathAsync()
        {
            if (_cachedPath != null && Directory.Exists(_cachedPath))
            {
                return _cachedPath;
            }

            await _pathSemaphore.WaitAsync();
            try
            {
                if (_cachedPath != null && Directory.Exists(_cachedPath))
                {
                    return _cachedPath;
                }

                var resolved = await DiscoverSharedFolderPathAsync();
                _cachedPath = resolved;
                return resolved;
            }
            finally
            {
                _pathSemaphore.Release();
            }
        }

        private Task<string?> DiscoverSharedFolderPathAsync()
        {
#if ANDROID
            var basePath = Android.OS.Environment.ExternalStorageDirectory?.AbsolutePath;
            if (string.IsNullOrWhiteSpace(basePath))
            {
                basePath = FileSystem.AppDataDirectory;
            }

            var fullPath = Path.Combine(basePath, SyncFolderName);
            Directory.CreateDirectory(fullPath);
            return Task.FromResult<string?>(fullPath);
#elif WINDOWS
            return DiscoverWindowsFolderAsync();
#else
            var fallback = Path.Combine(FileSystem.AppDataDirectory, SyncFolderName);
            Directory.CreateDirectory(fallback);
            return Task.FromResult<string?>(fallback);
#endif
        }

#if WINDOWS
        private static async Task<string?> DiscoverWindowsFolderAsync()
        {
            try
            {
                var removable = KnownFolders.RemovableDevices;
                var folders = await removable.GetFoldersAsync();
                foreach (var deviceFolder in folders)
                {
                    var syncFolder = await deviceFolder.CreateFolderAsync(SyncFolderName, CreationCollisionOption.OpenIfExists);
                    return syncFolder.Path;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SyncTransport Windows: {ex}");
            }

            return null;
        }
#endif
    }
}
