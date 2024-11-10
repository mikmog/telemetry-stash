using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;

namespace TelemetryStash.Functions.Extensions;

public static class TelemetryProcessor
{
    public static IServiceCollection ConfigureApplicationInsightsTelemetryFilter(this IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetryProcessor<TelemetryProcessorFilter>();
        return services;
    }
}

internal class TelemetryProcessorFilter(ITelemetryProcessor next) : ITelemetryProcessor
{
    public void Process(ITelemetry telemetry)
    {
        if (ShouldProcess(telemetry))
        {
            next.Process(telemetry);
        }
    }

    private static bool ShouldProcess(ITelemetry telemetry)
    {
        if (telemetry is DependencyTelemetry dependency)
        {
            // Only send failed dependencies
            return dependency.Success.HasValue && dependency.Success.Value == false;
        }

        // TODO: Does not work
        if (telemetry is TraceTelemetry trace)
        {
            const string eventName = "FunctionStarted";
            if (trace.Properties.TryGetValue("EventName", out var name))
            {
                return name != eventName;
            }
        }

        // TODO: Does not work
        // Filter out all metrics for now
        // TelemetryTrigger SuccessRate
        // TelemetryTrigger Failures
        // TelemetryTrigger Successes
        // TelemetryTrigger MinDurationMs
        // TelemetryTrigger MaxDurationMs
        // TelemetryTrigger AvgDurationMs
        // TelemetryTrigger Count
        if (telemetry is MetricTelemetry)
        {
            return false;
        }

        return true;
    }
}
