using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelemetryStash.Database.Repositories;

namespace TelemetryStash.Database.Extensions;

public static class StartupExtensions
{
    public static void AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TelemetryDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("TelemetryStashDatabase");
            options.UseSqlServer(connectionString, builder =>
                builder.MigrationsAssembly(typeof(TelemetryDbContext).Assembly.FullName)
            );
        });

        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(TelemetryDbContext).Assembly);

        services.AddTransient<IDeviceRepository, DeviceRepository>();
        services.AddTransient<IRegisterRepository, RegisterTemplateRepository>();
        services.AddTransient<IRegisterKeyRepository, RegisterKeyRepository>();
        services.AddTransient<IRegisterSetRepository, RegisterSetRepository>();
        services.AddTransient<ITelemetryRepository, TelemetryRepository>();
    }
}
