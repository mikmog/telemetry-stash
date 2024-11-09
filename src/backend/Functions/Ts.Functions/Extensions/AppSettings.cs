using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace TelemetryStash.Functions.Extensions;

public static class AppSettings
{
    public static IConfigurationBuilder AddAppSettings(this IConfigurationBuilder builder, IHostEnvironment hostingEnvironment)
    {
        builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        builder.AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: false);

        return builder;
    }

    public static string GetRequiredConnectionString(this IConfiguration configuration, string name)
    {
        return configuration.GetConnectionString(name) ?? throw new ConfigurationErrorException($"Missing required connection string '{name}'");
    }

    public static string GetRequiredSetting(this IConfiguration configuration, string name) 
    {
        return configuration[name]  ?? throw new ConfigurationErrorException($"Missing required configuration '{name}'");
    }
}

public class ConfigurationErrorException(string message) : Exception(message) { }
