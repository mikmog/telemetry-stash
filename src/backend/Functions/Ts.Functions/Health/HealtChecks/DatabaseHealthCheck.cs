using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Functions.Health.HealtChecks;

public class DatabaseHealthCheck(IHealthRepository healthRepository, ILogger<DatabaseHealthCheck> logger) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            await healthRepository.HealthCheck(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database health check failed.");
            return HealthCheckResult.Unhealthy();
        }
    }
}
