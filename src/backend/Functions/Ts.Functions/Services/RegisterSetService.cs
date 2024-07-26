using Microsoft.Extensions.Caching.Hybrid;
using TelemetryStash.Database;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Functions.Services;

public interface IRegisterSetService
{
    Task<RegisterSet> GetOrCreate(int deviceId, string registerSet, CancellationToken token = default);
}

public class RegisterSetService(IRegisterSetRepository registerSetRepository, HybridCache cache) : IRegisterSetService
{
    public async Task<RegisterSet> GetOrCreate(int deviceId, string registerSet, CancellationToken token = default)
    {
        return await cache.GetOrCreateAsync(
            $"{nameof(RegisterSet)}{deviceId}{registerSet}",
            async token => await registerSetRepository.Upsert(deviceId, registerSet, token),
            token: token);
    }
}
