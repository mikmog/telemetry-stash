using TelemetryStash.Database.Models;

namespace TelemetryStash.Database.Repositories;

public interface IRegisterSetRepository
{
    Task<RegisterSet?> GetByDeviceAndIdentifier(int deviceId, string identifier, CancellationToken token = default);

    Task<RegisterSet> Upsert(int deviceId, string identifier, CancellationToken token = default);
}

public class RegisterSetRepository(IDbProvider dbProvider) : IRegisterSetRepository
{
    public async Task<RegisterSet?> GetByDeviceAndIdentifier(int deviceId, string identifier, CancellationToken token = default)
    {
        return await dbProvider
            .ExecuteScalar<RegisterSet>("dbo.GetRegisterSet", new { DeviceId = deviceId, Identifier = identifier }, token);
    }

    public async Task<RegisterSet> Upsert(int deviceId, string identifier, CancellationToken token = default)
    {
        return await dbProvider
            .ExecuteScalar<RegisterSet>("dbo.UpsertRegisterSet", new { DeviceId = deviceId, Identifier = identifier }, token)
            ?? throw new Exception("UpsertRegisterSet not null expected");
    }
}
