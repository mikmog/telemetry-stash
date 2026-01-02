using Azure.Identity;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TelemetryStash.Functions.Extensions;

namespace TelemetryStash.Functions.Health.HealthChecks;
public class IotHubHealthCheck(IConfiguration configuration) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        var ioTHubHostName = configuration.GetRequiredSetting("IoTHubHostName");

        var manager = RegistryManager.Create(ioTHubHostName, new DefaultAzureCredential());
        var query = manager.CreateQuery("SELECT COUNT()");

        var response = await query.GetNextAsJsonAsync();
        if (!response.Any())
        {
            return HealthCheckResult.Unhealthy();
        }

        return HealthCheckResult.Healthy();
    }
}
