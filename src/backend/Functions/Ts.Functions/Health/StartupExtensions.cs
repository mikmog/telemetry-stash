using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelemetryStash.Functions.Extensions;
using TelemetryStash.Functions.Health.HealtChecks;

namespace TelemetryStash.Functions.Health;
internal static class StartupExtensions
{
    public static void AddHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddSqlServer(configuration.GetRequiredConnectionString("TelemetryStashDatabase"), name: "SqlServerHealthCheck")
            .AddCheck<IotHubHealthCheck>("IoTHubHealthCheck");
    }
}
