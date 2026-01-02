using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TelemetryStash.Functions.Weather.Logic;

public static class Startup
{
    public static IServiceCollection AddWeatherPrognosis(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection("WeatherPrognosis");
        services.Configure<WeatherPrognosisConfiguration>(section);

        services.AddTransient<IWeatherPrognosisService, WeatherPrognosisService>();

        const string baseUrl = "https://opendata-download-metfcst.smhi.se/api/category/snow1g/version/1/";
        services.AddSingleton(new SmhiHttpClient(baseUrl));

        return services;
    }
}

public class SmhiHttpClient : HttpClient
{
    public SmhiHttpClient(string baseUrl) : base()
    {
        BaseAddress = new Uri(baseUrl);
    }
}
