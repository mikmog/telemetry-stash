using System.Collections.Frozen;
using TelemetryStash.Database.Models;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Functions.TelemetryTrigger.Services
{
    public class CachedDeviceService(IDeviceRepository deviceRepository)
    {
        private static FrozenDictionary<string, Device>? _devices;

        public static Device? Get(string deviceId)
        {
            return _devices?.GetValueRefOrNullRef(deviceId);
        }

        public async Task<Device> AddAndGet(string deviceId)
        {
            var device = await deviceRepository.GetByDeviceId(deviceId, opt => opt.AsNoTracking());

            if (device == null)
            {
                device = deviceRepository.Add(new Device(deviceId));
                await deviceRepository.SaveChangesAsync();
            }

            var devices = await deviceRepository.All(opt => opt.AsNoTracking());
            _devices = FrozenDictionary.ToFrozenDictionary(devices, d => d.DeviceId, d => d);

            return _devices[deviceId];
        }
    }
}
