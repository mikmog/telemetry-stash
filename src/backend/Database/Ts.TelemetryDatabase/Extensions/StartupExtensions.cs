using Microsoft.Extensions.DependencyInjection;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Database.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddTelemetryDatabase(this IServiceCollection services)
    {
        services.AddSingleton<IDbProvider, DbConnectionProvider>();

        services.AddSingleton<IDeviceRepository, DeviceRepository>();
        services.AddSingleton<IRegisterRepository, RegisterRepository>();
        services.AddSingleton<IRegisterSetRepository, RegisterSetRepository>();
        services.AddSingleton<IRegisterTemplateRepository, RegisterTemplateRepository>();
        services.AddSingleton<ITelemetryRepository, TelemetryRepository>();

        return services;
    }
}
