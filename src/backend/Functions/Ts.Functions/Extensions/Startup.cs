using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using TelemetryStash.Functions.TelemetryTrigger.Logic;

namespace TelemetryStash.Functions.Extensions;

public static class Startup
{
    public static void AddFunctionServices(this IServiceCollection services)
    {
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromDays(1),
                LocalCacheExpiration = TimeSpan.FromDays(1)
            };
        });

        services.AddTransient<ITelemetryService, TelemetryService>();
        services.AddTransient<IDeviceService, DeviceService>();
        services.AddTransient<IRegisterService, RegisterService>();
    }
}
