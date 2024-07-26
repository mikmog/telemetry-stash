using Microsoft.Extensions.Caching.Hybrid;
using TelemetryStash.Database;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Functions.Services;

public interface IRegisterService
{
    Task<Register> GetOrCreate(int registerId, string registerSubset, CancellationToken token = default);
}

public class RegisterService(IRegisterRepository registerRepository, HybridCache cache) : IRegisterService
{
    public async Task<Register> GetOrCreate(int registerId, string registerSubset, CancellationToken token = default)
    {
        return await cache.GetOrCreateAsync(
            $"{nameof(Register)}{registerId}{registerSubset}",
            async token => await registerRepository.Upsert(registerId, registerSubset, token),
            token: token);
    }
}
