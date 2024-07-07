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
            options.MaximumPayloadBytes = 1024 * 1024;
            options.MaximumKeyLength = 1024;
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(55),
                LocalCacheExpiration = TimeSpan.FromMinutes(55)
            };
        });

        services.AddTransient<ITelemetryService, TelemetryService>();
        services.AddTransient<IDeviceService, DeviceService>();
        services.AddTransient<IRegisterSetService, RegisterSetService>();
        services.AddTransient<IRegisterTemplateService, RegisterTemplateService>();
        services.AddTransient<IRegisterService, RegisterService>();
    }
}
