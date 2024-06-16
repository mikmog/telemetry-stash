using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using TelemetryStash.Functions.Services;

namespace TelemetryStash.Functions.Extensions;
public static class StartupExtensions
{
    public static void AddFunctionServices(this IServiceCollection services)
    {
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromDays(30)
            };
        });

        services.AddTransient<ITelemetryService, TelemetryService>();
        services.AddTransient<IDeviceService, DeviceService>();
        services.AddTransient<IRegisterSetService, RegisterSetService>();
        services.AddTransient<IRegisterService, RegisterService>();
        services.AddTransient<IRegisterKeyService, RegisterKeyService>();
    }
}
