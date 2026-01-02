using Microsoft.Extensions.Caching.Hybrid;
using TelemetryStash.Database;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Functions.TelemetryTrigger.Logic;

public interface IDeviceService
{
    Task<DeviceRow> GetOrCreate(string deviceIdentifier, CancellationToken token = default);
}

public class DeviceService(IDeviceRepository deviceRepository, HybridCache cache) : IDeviceService
{
    public async Task<DeviceRow> GetOrCreate(string deviceIdentifier, CancellationToken token = default)
    {
        return await cache.GetOrCreateAsync(
            $"{nameof(DeviceRow)}{deviceIdentifier}",
            async innerToken => await deviceRepository.GetOrCreate(deviceIdentifier, innerToken),
            cancellationToken: token);
    }
}
