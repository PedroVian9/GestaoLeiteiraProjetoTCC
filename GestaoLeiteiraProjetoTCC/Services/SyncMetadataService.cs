using GestaoLeiteiraProjetoTCC.Services.Interfaces;
using Microsoft.Maui.Storage;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace GestaoLeiteiraProjetoTCC.Services
{
    public class SyncMetadataService : ISyncMetadataService
    {
        private const string DeviceIdKey = "sync_device_id";
        private const string LastSyncKey = "sync_last_success_utc";

        public SyncMetadataService()
        {
            EnsureDeviceId();
        }

        public string GetDeviceId()
        {
            return Preferences.Get(DeviceIdKey, string.Empty);
        }

        public string GetDeviceName()
        {
            return DeviceInfo.Current.Name;
        }

        public string GetPlatform()
        {
            return DeviceInfo.Current.Platform.ToString();
        }

        public DateTime? GetLastSuccessfulSyncUtc()
        {
            var value = Preferences.Get(LastSyncKey, string.Empty);
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var parsed))
            {
                return DateTime.SpecifyKind(parsed, DateTimeKind.Utc);
            }

            return null;
        }

        public Task SetLastSuccessfulSyncUtcAsync(DateTime? timestampUtc)
        {
            if (timestampUtc.HasValue)
            {
                Preferences.Set(LastSyncKey, timestampUtc.Value.ToUniversalTime().ToString("o"));
            }
            else
            {
                Preferences.Remove(LastSyncKey);
            }

            return Task.CompletedTask;
        }

        private void EnsureDeviceId()
        {
            var deviceId = Preferences.Get(DeviceIdKey, string.Empty);
            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                return;
            }

            Preferences.Set(DeviceIdKey, Guid.NewGuid().ToString());
        }
    }
}
