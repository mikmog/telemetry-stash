using Microsoft.Extensions.Caching.Hybrid;
using TelemetryStash.Database;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Functions.Services;

public interface IRegisterTemplateService
{
    Task<RegisterTemplate> GetOrCreate(int registerSetId, string registerIdentifier, CancellationToken token = default);
}

public class RegisterTemplateService(IRegisterTemplateRepository registerTemplateRepository, HybridCache cache) : IRegisterTemplateService
{
    public async Task<RegisterTemplate> GetOrCreate(int registerSetId, string registerIdentifier, CancellationToken token = default)
    {
        return await cache.GetOrCreateAsync(
            $"{nameof(RegisterTemplate)}{registerSetId}{registerIdentifier}",
            async token => await registerTemplateRepository.Upsert(registerSetId, registerIdentifier, token),
            token: token);
    }
}
