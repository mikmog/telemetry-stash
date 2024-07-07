using TelemetryStash.Database.Models;

namespace TelemetryStash.Database.Repositories;

public interface IDeviceRepository
{
    Task<Device?> GetByDeviceId(string deviceId, CancellationToken token = default);

    Task<Device> Upsert(string deviceId, CancellationToken token = default);
}

public class DeviceRepository(IDbProvider dbProvider) : IDeviceRepository
{
    public async Task<Device?> GetByDeviceId(string deviceId, CancellationToken token = default)
    {
        return await dbProvider
            .ExecuteScalar<Device>("dbo.GetDevice", new { DeviceId = deviceId }, token);
    }

    public async Task<Device> Upsert(string deviceId, CancellationToken token = default)
    {
        return await dbProvider
            .ExecuteScalar<Device>("dbo.UpsertDevice", new { DeviceId = deviceId }, token)
            ?? throw new Exception("UpsertDevice not null expected");
    }
}
