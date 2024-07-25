using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TelemetryStash.Database;
using TelemetryStash.Functions.Extensions;

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

        services.AddFunctionServices();
        services.AddTelemetryDatabase(context.Configuration);
    })
    .Build();

host.Run();
