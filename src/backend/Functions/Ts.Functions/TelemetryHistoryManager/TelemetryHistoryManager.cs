using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using TelemetryStash.Functions.Services;

namespace TelemetryStash.Functions.TelemetryHistoryManager;

public class TelemetryHistoryManager(ILoggerFactory loggerFactory, ITelemetryService telemetryService)
{
    private const string RunEveryDayAt03Utc = "0 0 3 * * *";
    private readonly ILogger _logger = loggerFactory.CreateLogger<TelemetryHistoryManager>();

    [Function("TelemetryHistoryManager")]
    public async Task Run([TimerTrigger(RunEveryDayAt03Utc, RunOnStartup = true)] TimerInfo timerInfo)
    {
        _logger.LogInformation($"Timer trigger function executed");

        await Task.CompletedTask;
    }
}
