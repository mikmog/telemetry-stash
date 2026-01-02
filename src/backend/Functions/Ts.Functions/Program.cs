using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TelemetryStash.Database;
using TelemetryStash.Functions.Extensions;
using TelemetryStash.Functions.Health;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddAppSettings(context.HostingEnvironment);
        //builder.AddKeyVault<Program>();
        builder.AddUserSecrets<Program>();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsights(context.Configuration);
        services.ConfigureApplicationInsightsTelemetryFilter();

        services.AddFunctionServices();
        services.AddTelemetryDatabase(context.Configuration);
        services.AddHealthCheck();
    })
    .Build();

host.Run();
