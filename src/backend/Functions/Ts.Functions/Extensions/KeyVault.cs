using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace TelemetryStash.Functions.Extensions;

public static class KeyVault
{
    public static IConfigurationBuilder AddKeyVault<TProgram>(this IConfigurationBuilder builder)
        where TProgram : class
    {
        const string configurationName = "KeyVault";

        var builtConfiguration = builder
            .AddUserSecrets<TProgram>()
            .Build();

        var keyVaultUrl = builtConfiguration[configurationName] ?? throw new Exception($"Missing key vault configuration: {configurationName}");

        var credentials = new DefaultAzureCredential();
        builder.AddAzureKeyVault(new Uri(keyVaultUrl), credentials);

        return builder;
    }
}
