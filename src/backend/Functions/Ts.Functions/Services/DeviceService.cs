using Microsoft.Extensions.Caching.Hybrid;
using TelemetryStash.Database;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Functions.Services;

public interface IDeviceService
{
    Task<Device> GetOrCreate(string deviceId, CancellationToken token = default);
}

public class DeviceService(IDeviceRepository deviceRepository, HybridCache cache) : IDeviceService
{
    public async Task<Device> GetOrCreate(string deviceId, CancellationToken token = default)
    {
        return await cache.GetOrCreateAsync(
            $"{nameof(Device)}{deviceId}",
            async innerToken => await deviceRepository.Upsert(deviceId, innerToken),
            token: token);
    }
}
