using Microsoft.Extensions.Caching.Hybrid;
using TelemetryStash.Database.Models;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Functions.Services;

public interface IRegisterKeyService
{
    Task<RegisterKey> GetOrAdd(int registerId, string registerSubset, CancellationToken token = default);
}

public class RegisterKeyService(IRegisterKeyRepository registerKeyRepository, HybridCache cache) : IRegisterKeyService
{
    public async Task<RegisterKey> GetOrAdd(int registerId, string registerSubset, CancellationToken token = default)
    {
        return await cache.GetOrCreateAsync(
            $"{nameof(RegisterKey)}{registerId}{registerSubset}",
            async token => await GetOrAdd(registerId, registerSubset, registerKeyRepository, token),
            token: token);
    }

    private static async Task<RegisterKey> GetOrAdd(int registerId, string registerSubset, IRegisterKeyRepository repository, CancellationToken token)
    {
        var register = await repository.GetRegisterKey(registerId, registerSubset, opts => opts.AsNoTracking(), token);
        if (register == null)
        {
            register = repository.Add(new RegisterKey(registerId, registerSubset));
            await repository.SaveChangesAsync(token);
            repository.StopTracking(register);
        }

        return register;
    }
}
