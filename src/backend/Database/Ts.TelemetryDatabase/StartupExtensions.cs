using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RepoDb;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Database;

public static class StartupExtensions
{
    public static IServiceCollection AddTelemetryDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        GlobalConfiguration.Setup().UseSqlServer();

        var connectionString = configuration.GetConnectionString("TelemetryStashDatabase") ?? throw new Exception("Missing TelemetryStashDatabase connection string");
        services.AddSingleton<IDbProvider>(_ => new DbProvider(connectionString));

        services.AddSingleton<IDeviceRepository, DeviceRepository>();
        services.AddSingleton<IRegisterRepository, RegisterRepository>();
        services.AddSingleton<IRegisterSetRepository, RegisterSetRepository>();
        services.AddSingleton<IRegisterTemplateRepository, RegisterTemplateRepository>();
        services.AddSingleton<ITelemetryRepository, TelemetryRepository>();

        return services;
    }
}
