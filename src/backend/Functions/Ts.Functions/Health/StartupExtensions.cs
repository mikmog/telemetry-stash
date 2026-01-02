using Microsoft.Extensions.DependencyInjection;
using TelemetryStash.Functions.Health.HealtChecks;

namespace TelemetryStash.Functions.Health;

internal static class StartupExtensions
{
    public static void AddHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("SqlServerHealthCheck")
            .AddCheck<IotHubHealthCheck>("IoTHubHealthCheck");
    }
}
