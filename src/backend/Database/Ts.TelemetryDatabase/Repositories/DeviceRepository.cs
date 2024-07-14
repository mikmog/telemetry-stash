using RepoDb;
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
        using var connection = dbProvider.CreateConnection();
        var devices = await connection
            .QueryAsync<Device>("Devices", d => d.DeviceId == deviceId, cancellationToken: token);

        return devices.SingleOrDefault();
    }

    public async Task<Device> Upsert(string deviceId, CancellationToken token = default)
    {
        return await dbProvider
            .ExecuteStoredProcedure<Device>("dbo.UpsertDevice", new { DeviceId = deviceId }, token)
            ?? throw new Exception("UpsertDevice not null expected");
    }
}
