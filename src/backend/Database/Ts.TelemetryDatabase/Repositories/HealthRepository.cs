namespace TelemetryStash.Database.Repositories;

public interface IHealthRepository
{
    public Task HealthCheck(CancellationToken token);
}

public class HealthRepository(IDbProvider dbProvider) : IHealthRepository
{
    public async Task HealthCheck(CancellationToken token)
    {
        await dbProvider
            .ExecuteScalar
            (
                storedProcedure: "[dbo].[HealthCheck]",
                parameters: null,
                cancellationToken: token
            );
    }
}
