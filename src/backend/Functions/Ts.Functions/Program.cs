using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TelemetryStash.Database;
using TelemetryStash.Functions.Extensions;
using TelemetryStash.Functions.TelemetryTrigger.Services;

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

        services.AddTransient<CachedDeviceService>();
        services.AddTransient<CachedRegisterService>();
        services.AddTransient<CachedRegisterSetService>();
        services.AddTransient<TelemetryService>();

        // Domain services
        services.AddTelemetryDatabase(context.Configuration);
    })
    .Build();

host.Run();
