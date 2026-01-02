using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Functions.Weather.Logic;

public interface IWeatherPrognosisService
{
    public Task ProcessSmhiPrognosis(CancellationToken cancellationToken);
}

public class WeatherPrognosisService(
    IOptions<WeatherPrognosisConfiguration> configuration,
    IWeatherPrognosisRepository weatherPrognosisRepository,
    SmhiHttpClient httpClient) : IWeatherPrognosisService
{
    private static DateTimeOffset _prognosisReferenceTime = DateTimeOffset.MinValue;

    public async Task ProcessSmhiPrognosis(CancellationToken cancellationToken)
    {
        var config = configuration.Value;
        if (!config.Enabled)
        {
            return;
        }

        var referenceTime = await GetPrognosisReferenceTime(cancellationToken);
        if (referenceTime != _prognosisReferenceTime)
        {
            var prognosis = await GetPrognosis(config.Longitude, config.Latitude, cancellationToken);
            await weatherPrognosisRepository.AddOrUpdate(config.Name, referenceTime, prognosis, cancellationToken);
            _prognosisReferenceTime = referenceTime;
        }
    }

    private async Task<DateTimeOffset> GetPrognosisReferenceTime(CancellationToken cancellationToken)
    {
        // Last updated
        // https://opendata-download-metfcst.smhi.se/api/category/snow1g/version/1/createdtime.json

        const string url = "createdtime.json";
        var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, DateTimeOffset>>(cancellationToken: cancellationToken)
            ?? throw new Exception($"Error reading content from {url}");

        if (!content.TryGetValue("referenceTime", out var referenceTime))
        {
            throw new Exception($"Missing 'referenceTime' key in response from {url}");
        }
        return referenceTime;
    }

    private async Task<string> GetPrognosis(string longitude, string latitude, CancellationToken cancellationToken)
    {
        // Data model
        // https://opendata.smhi.se/metfcst/snow1gv1/parameters

        var url = $"geotype/point/lon/{longitude}/lat/{latitude}/data.json";
        var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return content;
    }
}
