using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelemetryStash.Functions.EnergySpotPrice.Logic;

namespace TelemetryStash.Functions.EnergySpotPrice;

public static class Startup
{
    public static IServiceCollection AddEnergySpotPrice(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection("EnergySpotPrice");
        services.Configure<EnergySpotPriceConfiguration>(section);

        services.AddTransient<IEnergySpotPriceService, EnergySpotPriceService>();

        const string baseUrl = "https://selfserviceapi.www.vattenfall.se/elements/nordpool/aggregatedspotprices/";
        services.AddSingleton(new EnergySpotPriceHttpClient(baseUrl));

        return services;
    }
}

public class EnergySpotPriceHttpClient : HttpClient
{
    public EnergySpotPriceHttpClient(string baseUrl) : base()
    {
        BaseAddress = new Uri(baseUrl);
    }
}
