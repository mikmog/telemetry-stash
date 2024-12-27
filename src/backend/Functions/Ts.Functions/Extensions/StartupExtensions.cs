using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using TelemetryStash.Functions.Services;

namespace TelemetryStash.Functions.Extensions;

public static class StartupExtensions
{
    public static void AddFunctionServices(this IServiceCollection services)
    {
#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromDays(1),
                LocalCacheExpiration = TimeSpan.FromDays(1)
            };
        });
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        services.AddTransient<ITelemetryService, TelemetryService>();
        services.AddTransient<IDeviceService, DeviceService>();
        services.AddTransient<IRegisterService, RegisterService>();
    }
}
