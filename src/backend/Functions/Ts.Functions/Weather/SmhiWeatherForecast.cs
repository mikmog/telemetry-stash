using Microsoft.Azure.Functions.Worker;
using TelemetryStash.Functions.Weather.Logic;

namespace TelemetryStash.Functions.Weather;

public class SmhiWeatherForecast(IWeatherPrognosisService weatherPrognosisService)
{
    private const string RunEveryHour = "0 0 * * * *";

    [Function("SmhiWeatherForecast")]
    public async Task Run([TimerTrigger(RunEveryHour, RunOnStartup = false)] TimerInfo timer, CancellationToken cancellationToken)
    {
        await weatherPrognosisService.ProcessSmhiPrognosis(cancellationToken);
    }
}
