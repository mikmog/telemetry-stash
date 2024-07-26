using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Dac;
using RepoDb;
using Testcontainers.MsSql;

namespace TelemetryStash.Database.Tests;

// Single instance of the database for all tests
[CollectionDefinition("SharedTestDbServer")]
public class CollectionState : ICollectionFixture<SharedTestDbFixture> { }

public class SharedTestDbFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer;
    private readonly Dictionary<string, IDbProvider> _dbProviders = [];

    public SharedTestDbFixture()
    {
        // https://hub.docker.com/r/microsoft/mssql-server
        _msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();
    }

    public IDbProvider GetTestDbProvider(string databaseName)
    {
        if (!_dbProviders.TryGetValue(databaseName, out var value))
        {
            lock (_msSqlContainer)
            {
                if (!_dbProviders.TryGetValue(databaseName, out value))
                {
                    var masterDbConnectionString = _msSqlContainer.GetConnectionString();
                    using var connection = new SqlConnection(masterDbConnectionString);

                    // Create database if it doesn't exist
                    using var createDbCommand = connection.CreateCommand();
                    createDbCommand.CommandText =
                        $"""
                            IF DB_ID('{databaseName}') IS NOT NULL
                            BEGIN
                                DROP DATABASE {databaseName}
                            END
                            CREATE DATABASE {databaseName}
                        """;

                    connection.Open();
                    createDbCommand.ExecuteNonQuery();

                    // Replace master with databaseName
                    var connectionString = new SqlConnectionStringBuilder(masterDbConnectionString)
                    {
                        InitialCatalog = databaseName
                    };

                    var services = new DacServices(connectionString.ToString());

                    services.Message += (sender, args) => Console.WriteLine(args.Message);
                    services.ProgressChanged += (sender, args) => Console.WriteLine(args.Status);

#if DEBUG
                    var buildConfiguration = "Debug";
#else
                        var buildConfiguration = "Release";
#endif

                    // Apply dacpac SQL schema
                    services.Deploy(
                        DacPackage.Load($@"..\..\..\..\Ts.TelemetryDatabase.Sql\bin\{buildConfiguration}\Ts.TelemetryDatabase.Sql.dacpac"),
                        databaseName,
                        upgradeExisting: true);

                    value = new DbConnectionProvider(connectionString.ToString());
                    _dbProviders[databaseName] = value;
                }
            }
        }
        return value;
    }

    public async Task InitializeAsync()
    {
        GlobalConfiguration.Setup().UseSqlServer();

        await _msSqlContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }
}
