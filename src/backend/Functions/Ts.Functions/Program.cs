using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TelemetryStash.Database;
using TelemetryStash.Functions.Extensions;
using TelemetryStash.Functions.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, builder) =>
    {
        // Configuration
        builder.AddAppSettings(context.HostingEnvironment);
        //builder.AddKeyVault<Program>();
        builder.AddUserSecrets<Program>();
    })
    .ConfigureServices((context, services) =>
    {
        // Infrastructure services
        services.AddApplicationInsights(context.Configuration);
        services.AddHttpClientLogger();
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

        // Domain services
        services.AddTelemetryDatabase(context.Configuration);
    })
    .Build();

host.Run();
