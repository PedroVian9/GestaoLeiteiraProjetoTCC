using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
#if WINDOWS
using Windows.Storage;
#endif

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class SyncTransportService : ISyncTransportService
    {
        private readonly string _sharedFolderPath;

        public SyncTransportService()
        {
            _sharedFolderPath = ResolveSharedFolderPath();
        }

        public async Task EnsureSharedFolderExistsAsync()
        {
            if (!Directory.Exists(_sharedFolderPath))
            {
                Directory.CreateDirectory(_sharedFolderPath);
            }

            await Task.CompletedTask;
        }

        public Task<string> GetSharedFolderPathAsync()
        {
            return Task.FromResult(_sharedFolderPath);
        }

        public async Task<string> WriteOutboundPayloadAsync(string fileName, string content)
        {
            await EnsureSharedFolderExistsAsync();
            var path = Path.Combine(_sharedFolderPath, fileName);
            await File.WriteAllTextAsync(path, content);
            return path;
        }

        public async Task<IReadOnlyList<string>> GetAvailablePayloadFilesAsync()
        {
            await EnsureSharedFolderExistsAsync();
            var files = Directory.GetFiles(_sharedFolderPath, "*.json")
                                 .OrderByDescending(File.GetCreationTimeUtc)
                                 .ToList();
            return files;
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

        private static string ResolveSharedFolderPath()
        {
            string basePath;

#if ANDROID
            basePath = Android.OS.Environment.ExternalStorageDirectory?.AbsolutePath ??
                       FileSystem.AppDataDirectory;
#elif WINDOWS
            var removable = TryGetRemovableSyncFolder();
            if (!string.IsNullOrEmpty(removable))
            {
                return removable;
            }

            basePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
#else
            basePath = FileSystem.AppDataDirectory;
#endif

            return Path.Combine(basePath, "GestaoLeiteiraSync");
        }

#if WINDOWS
        private static string? TryGetRemovableSyncFolder()
        {
            try
            {
                var removableRoot = KnownFolders.RemovableDevices;
                var folders = removableRoot.GetFoldersAsync().AsTask().GetAwaiter().GetResult();
                foreach (var folder in folders)
                {
                    var syncFolder = folder.TryGetItemAsync("GestaoLeiteiraSync").AsTask().GetAwaiter().GetResult();
                    if (syncFolder is StorageFolder storageFolder)
                    {
                        return storageFolder.Path;
                    }
                }
            }
            catch
            {
                // Ignorar e usar fallback
            }

            return null;
        }
#endif
    }
}
