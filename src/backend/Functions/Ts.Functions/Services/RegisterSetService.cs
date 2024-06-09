using Microsoft.Extensions.Caching.Hybrid;
using TelemetryStash.Database.Models;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Functions.Services;

public interface IRegisterSetService
{
    Task<RegisterSet> GetOrAdd(int deviceId, string registerSet, CancellationToken token = default);
}

public class RegisterSetService(IRegisterSetRepository registerSetRepository, HybridCache cache)
{
    public async Task<RegisterSet> GetOrAdd(int deviceId, string registerSet, CancellationToken token = default)
    {
        return await cache.GetOrCreateAsync(
            $"{nameof(RegisterSet)}{deviceId}{registerSet}",
            async token => await GetOrAdd(deviceId, registerSet, registerSetRepository, token),
            token: token);
    }

    private static async Task<RegisterSet> GetOrAdd(int deviceId, string registerSet, IRegisterSetRepository repository, CancellationToken token)
    {
        var set = await repository.GetRegisterSet(deviceId, registerSet, opts => opts.AsNoTracking(), token);
        if (set == null)
        {
            set = repository.Add(new RegisterSet(deviceId, registerSet));
            await repository.SaveChangesAsync(token);
            repository.StopTracking(set);
        }

        return set;
    }
}
