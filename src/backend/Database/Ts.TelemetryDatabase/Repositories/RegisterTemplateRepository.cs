using TelemetryStash.Database.Models;

namespace TelemetryStash.Database.Repositories;

public interface IRegisterTemplateRepository
{
    Task<RegisterTemplate?> GetRegister(int registerSetId, string registerIdentifier, CancellationToken token = default);

    Task<RegisterTemplate> Upsert(int registerSetId, string registerIdentifier, CancellationToken token = default);
}

public class RegisterTemplateRepository(IDbProvider dbProvider) : IRegisterTemplateRepository
{
    public async Task<RegisterTemplate?> GetRegister(int registerSetId, string registerIdentifier, CancellationToken token = default)
    {
        return await dbProvider.ExecuteStoredProcedure<RegisterTemplate?>("dbo.GetRegisterTemplate", new { RegisterSetId = registerSetId, RegisterIdentifier = registerIdentifier }, token);
    }

    // TODO: Handle missing fields
    public async Task<RegisterTemplate> Upsert(int registerSetId, string registerIdentifier, CancellationToken token = default)
    {
        return await dbProvider
            .ExecuteStoredProcedure<RegisterTemplate>("dbo.UpsertRegisterTemplate", new { RegisterSetId = registerSetId, RegisterIdentifier = registerIdentifier }, token)
            ?? throw new InvalidOperationException("UpsertRegisterTemplate not null expected");
    }
}
