using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Dac;
using RepoDb;
using Testcontainers.MsSql;

namespace TelemetryStash.Database.Tests;

// Single instance of the database for all tests
[CollectionDefinition(CollectionState.SharedTestDbServer)]
public class CollectionState : ICollectionFixture<SharedTestDbFixture>
{
    public const string SharedTestDbServer = "SharedTestDbServer";
}

public class SharedTestDbFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer;
    private readonly Dictionary<string, IDbProvider> _dbProviders = [];
    private readonly DacPackage _dacPackage;

    public SharedTestDbFixture()
    {
        TestcontainersSettings.DockerHostOverride = "127.0.0.1";

        // https://hub.docker.com/r/microsoft/mssql-server
        _sqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04")

            // https://github.com/testcontainers/testcontainers-dotnet/issues/1220
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("/opt/mssql-tools18/bin/sqlcmd", "-C", "-Q", "SELECT 1;"))
            .Build();

        _dacPackage = DacPackage.Load("../../../../Ts.TelemetryDatabase.Sql/bin/Ts.TelemetryDatabase.Sql.dacpac");
    }

    public IDbProvider GetTestDbProvider(string databaseName)
    {
        if (!_dbProviders.TryGetValue(databaseName, out var value))
        {
            var masterDbConnectionString = _sqlContainer.GetConnectionString();
            using var connection = new SqlConnection(masterDbConnectionString);

            // Drop database if exist
            // Drop database if exist
            const string sql =
                """
                    IF DB_ID(@DatabaseName) IS NOT NULL
                    BEGIN
                        DROP DATABASE [@DatabaseName]
                    END
                """;
                connection.ExecuteScalar(sql, new { DatabaseName = databaseName });

            // Replace master with databaseName
            var connectionString = new SqlConnectionStringBuilder(masterDbConnectionString)
            {
                InitialCatalog = databaseName
            };

            var services = new DacServices(connectionString.ToString());

            services.Message += (sender, args) => Console.WriteLine(args.Message);
            services.ProgressChanged += (sender, args) => Console.WriteLine(args.Status);

            // Create database and apply dacpac SQL schema
            services.Deploy(_dacPackage, databaseName);

            value = new DbProvider(connectionString.ToString());
            _dbProviders[databaseName] = value;
        }
        return value;
    }

    public async Task InitializeAsync()
    {
        GlobalConfiguration.Setup().UseSqlServer();
        await _sqlContainer.StartAsync().ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        await _sqlContainer.DisposeAsync().ConfigureAwait(false);
    }
}
