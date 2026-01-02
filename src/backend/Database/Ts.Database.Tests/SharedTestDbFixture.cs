using DotNet.Testcontainers.Configurations;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Dac;
using RepoDb;
using System.Security.Cryptography;
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
    private readonly string _sqlDbPassword;
    private readonly Dictionary<string, IDbProvider> _dbProviders = [];
    private readonly DacPackage _dacPackage;

    public SharedTestDbFixture()
    {
        TestcontainersSettings.DockerHostOverride = "127.0.0.1";
        _sqlDbPassword = $"{Convert.ToBase64String(RandomNumberGenerator.GetBytes(10))}aA1-";

        // https://hub.docker.com/r/microsoft/mssql-server
        _sqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
            .WithPassword(_sqlDbPassword)
            .Build();

        _dacPackage = DacPackage.Load("Sql/Ts.TelemetryDatabase.Sql.dacpac");

    }

    public IDbProvider GetTestDbProvider(string databaseName)
    {
        if (!_dbProviders.TryGetValue(databaseName, out var value))
        {
            var masterDbConnectionString = _sqlContainer.GetConnectionString();
            using var connection = new SqlConnection(masterDbConnectionString);

            // Drop database if exist
            var dropDbSql =
                $"""
                    IF DB_ID('{databaseName}') IS NOT NULL
                    BEGIN
                        DROP DATABASE [{databaseName}]
                    END
                """;
            connection.ExecuteNonQuery(dropDbSql);

            // Create database and apply dacpac SQL schema
            var services = new DacServices(masterDbConnectionString.ToString());
            services.Deploy(_dacPackage, databaseName);

            // Create user
            const string userId = "ts_test_user";
            var createUserSql =
                $"""
                    USE [{databaseName}]

                    IF SUSER_ID ('{userId}') IS NULL
                    CREATE LOGIN [{userId}] WITH PASSWORD = '{_sqlDbPassword}'

                    CREATE USER [{userId}] FOR LOGIN [{userId}]
                    ALTER ROLE [db_execute_procedure_role] ADD MEMBER [{userId}]
                    EXEC sp_addrolemember 'db_datareader', [{userId}]
                """;

            connection.ExecuteNonQuery(createUserSql);

            // Build connection string
            var connectionString = new SqlConnectionStringBuilder(masterDbConnectionString)
            {
                InitialCatalog = databaseName,
                UserID = userId,
                Password = _sqlDbPassword
            };

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
