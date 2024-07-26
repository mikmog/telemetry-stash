using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace TelemetryStash.Functions.Extensions;

public static class AppSettings
{
    public static IConfigurationBuilder AddAppSettings(this IConfigurationBuilder builder, IHostEnvironment hostingEnvironment)
    {
        builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        builder.AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);

        return builder;
    }
}
