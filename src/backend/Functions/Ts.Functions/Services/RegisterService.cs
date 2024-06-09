using Microsoft.Extensions.Caching.Hybrid;
using TelemetryStash.Database.Models;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Functions.Services;

public interface IRegisterService
{
    Task<Register> GetOrAdd(int registerSetId, string registerIdentifier, CancellationToken token = default);
}

public class RegisterService(IRegisterRepository registerRepository, HybridCache cache)
{
    public async Task<Register> GetOrAdd(int registerSetId, string registerIdentifier, CancellationToken token = default)
    {
        return await cache.GetOrCreateAsync(
            $"{nameof(Register)}{registerSetId}{registerIdentifier}",
            async token => await GetOrAdd(registerSetId, registerIdentifier, registerRepository, token),
            token: token);
    }

    private static async Task<Register> GetOrAdd(int registerSetId, string registerIdentifier, IRegisterRepository repository, CancellationToken token)
    {
        var register = await repository.GetRegister(registerSetId, registerIdentifier, opts => opts.AsNoTracking(), token);
        if (register == null)
        {
            register = repository.Add(new Register(registerSetId, registerIdentifier));
            await repository.SaveChangesAsync(token);
            repository.StopTracking(register);
        }

        return register;
    }
}
