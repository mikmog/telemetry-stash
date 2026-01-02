using Microsoft.Extensions.Caching.Hybrid;
using TelemetryStash.Database;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Functions.TelemetryTrigger.Logic;

public interface IRegisterService
{
    Task<Dictionary<string, RegisterRow>> GetOrCreate(int deviceId, string registerSet, IEnumerable<string> registers, CancellationToken token = default);
}

public class RegisterService(IRegisterRepository registerRepository, HybridCache cache) : IRegisterService
{
    public async Task<Dictionary<string, RegisterRow>> GetOrCreate(int deviceId, string registerSet, IEnumerable<string> registers, CancellationToken token = default)
    {
        var key = $"{nameof(RegisterRow)}{deviceId}{registerSet}{string.Concat(registers.Order())}";

        var response = await cache.GetOrCreateAsync(
            key,
            async innerToken => await registerRepository.GetOrCreate(deviceId, registerSet, registers, innerToken),
            cancellationToken: token);

        return response.ToDictionary(r => r.Register, r => r);
    }
}
