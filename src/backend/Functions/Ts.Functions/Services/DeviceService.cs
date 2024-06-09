using Microsoft.Extensions.Caching.Hybrid;
using TelemetryStash.Database.Models;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Functions.Services;

public interface IDeviceService
{
    Task<Device> GetOrAdd(string deviceId, CancellationToken token = default);
}

public class DeviceService(IDeviceRepository deviceRepository, HybridCache cache)
{
    public async Task<Device> GetOrAdd(string deviceId, CancellationToken token = default)
    {
        return await cache.GetOrCreateAsync(
            $"{nameof(Device)}{deviceId}",
            async token => await GetOrAdd(deviceId, deviceRepository, token),
            token: token);
    }

    private static async Task<Device> GetOrAdd(string deviceId, IDeviceRepository repository, CancellationToken token)
    {
        var device = await repository.GetByDeviceId(deviceId, opt => opt.AsNoTracking(), token);
        if (device == null)
        {
            device = repository.Add(new Device(deviceId));
            await repository.SaveChangesAsync(token);
            repository.StopTracking(device);
        }

        return device;
    }
}
