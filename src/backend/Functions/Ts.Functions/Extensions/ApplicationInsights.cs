using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

namespace TelemetryStash.Functions.Extensions;

public static class ApplicationInsights
{
    public static IServiceCollection AddApplicationInsights(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationInsightsTelemetryWorkerService(options =>
        {
            options.EnableAdaptiveSampling = false; // Disables adaptive sampling in the worker (host is configured in host.json)
        });
        services.ConfigureFunctionsApplicationInsights();

        ConfigureLogLevels(services, configuration);

        return services;
    }

    private static void ConfigureLogLevels(IServiceCollection services, IConfiguration configuration)
    {
        const string configurationName = "Logging:LogLevel";

        services.Configure<LoggerFilterOptions>(options =>
        {
            var providerName = typeof(ApplicationInsightsLoggerProvider).FullName;

            // Remove Application Insights default logging filter (only logs warnings as default)
            options.Rules.Remove(options.Rules.Single(rule => rule.ProviderName == providerName));

            // Add default rules to also apply to Application Insights
            var loggingConfiguration = configuration.GetSection(configurationName);
            foreach (var (key, value) in loggingConfiguration.AsEnumerable(true))
            {
                if (Enum.TryParse<LogLevel>(value, out var logLevel))
                {
                    options.Rules.Add(new LoggerFilterRule(providerName, key, logLevel, null));
                }
            }
        });
    }
}
