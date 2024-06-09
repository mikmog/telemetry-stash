using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using TelemetryStash.Functions.TelemetryTrigger.Services;

namespace TelemetryStash.Functions.TelemetryHistoryManager;

public class TelemetryHistoryManager(ILoggerFactory loggerFactory, TelemetryService telemetryService)
{
    private const string RunEveryDayAt03Utc = "0 0 3 * * *";
    private readonly ILogger _logger = loggerFactory.CreateLogger<TelemetryHistoryManager>();

    [Function("TelemetryHistoryManager")]
    public async Task Run([TimerTrigger(RunEveryDayAt03Utc, RunOnStartup = true)] TimerInfo timer)
    {
        _logger.LogInformation($"Timer trigger function executed");

        await Task.CompletedTask;
    }
}
