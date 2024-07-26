using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace TelemetryStash.Functions.TelemetryHistoryManager;

public class TelemetryHistoryManager(ILoggerFactory loggerFactory)
{
    private const string RunEveryDayAt03Utc = "0 0 3 * * *";
    private readonly ILogger _logger = loggerFactory.CreateLogger<TelemetryHistoryManager>();

    [Function("TelemetryHistoryManager")]
    public async Task Run([TimerTrigger(RunEveryDayAt03Utc, RunOnStartup = false)] TimerInfo timerInfo)
    {
        _logger.LogInformation($"Timer trigger function executed");

        await Task.CompletedTask;
    }
}
