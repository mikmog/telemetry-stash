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
    private readonly DacPackage _dacPackage;

    public SharedTestDbFixture()
    {
        // https://hub.docker.com/r/microsoft/mssql-server
        _msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();

        _dacPackage = DacPackage.Load($@"..\..\..\..\Ts.TelemetryDatabase.Sql\bin\Ts.TelemetryDatabase.Sql.dacpac");
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

                    // Drop database if exist
                    using var createDbCommand = connection.CreateCommand();
                    createDbCommand.CommandText =
                        $"""
                            IF DB_ID('{databaseName}') IS NOT NULL
                            BEGIN
                                DROP DATABASE {databaseName}
                            END
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

                    // Create database and apply dacpac SQL schema
                    services.Deploy(_dacPackage, databaseName);

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
