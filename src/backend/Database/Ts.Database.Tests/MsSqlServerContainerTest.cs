using DotNet.Testcontainers.Containers;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Dac;
using System.Data;
using System.Data.Common;
using Testcontainers.MsSql;

namespace TelemetryStash.Database.Tests;

public sealed class MsSqlServerContainerTest : IAsyncLifetime
{
    private const string Database = "FooBarBaz";

    // https://hub.docker.com/r/microsoft/mssql-server
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest").Build();

    private readonly DbConnectionFactory _dbConnectionFactory;

    public MsSqlServerContainerTest()
    {
        _dbConnectionFactory = new DbConnectionFactory(_msSqlContainer, Database);
    }


    //[Fact]
    //public async Task ReadFromMsSqlDatabase()
    //{
    //    var str = _msSqlContainer.GetConnectionString();
    //    await using var connection = new SqlConnection(str);
    //    await connection.OpenAsync();

    //    var services = new DacServices(str);
    //    services.Message += (sender, args) => Console.WriteLine(args.Message);
    //    services.ProgressChanged += (sender, args) => Console.WriteLine(args.Status);

    //    var script = services.GenerateDeployScript(
    //        DacPackage.Load(@"C:\Git\telemetry-stash\src\backend\Database\Ts.TelemetryDatabase.Sql\bin\Debug\Ts.TelemetryDatabase.Sql.dacpac"),
    //        "master");

    //    await using var command = connection.CreateCommand();
    //    command.CommandText = "SELECT 1;";

    //    var actual = await command.ExecuteScalarAsync() as int?;
    //    Assert.Equal(1, actual.GetValueOrDefault());
    //}

    //[Fact]
    public void Question77511865()
    {
        // Given
        using var connection = _dbConnectionFactory.CustomDbConnection;

        // When
        connection.Open();

        // Then 
        Assert.Equal(ConnectionState.Open, connection.State);

        var services = new DacServices(_dbConnectionFactory.CustomDbConnectionString);
        services.Deploy(
            DacPackage.Load(@"C:\Git\telemetry-stash\src\backend\Database\Ts.TelemetryDatabase.Sql\bin\Debug\Ts.TelemetryDatabase.Sql.dacpac"),
            Database,
            upgradeExisting: true);
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();

        using var connection = _dbConnectionFactory.MasterDbConnection;

        // TODO: Add your database migration here.
        using var command = connection.CreateCommand();
        command.CommandText = "CREATE DATABASE " + Database;

        await connection.OpenAsync()
            .ConfigureAwait(false);

        await command.ExecuteNonQueryAsync()
            .ConfigureAwait(false);
    }

    public Task DisposeAsync()
    {
        return _msSqlContainer.DisposeAsync().AsTask();
    }

    private sealed class DbConnectionFactory
    {
        private readonly IDatabaseContainer _databaseContainer;

        private readonly string _database;

        public DbConnectionFactory(IDatabaseContainer databaseContainer, string database)
        {
            _databaseContainer = databaseContainer;
            _database = database;
        }

        public DbConnection MasterDbConnection
        {
            get
            {
                return new SqlConnection(_databaseContainer.GetConnectionString());
            }
        }

        public DbConnection CustomDbConnection
        {
            get
            {
                return new SqlConnection(CustomDbConnectionString);
            }
        }

        public string CustomDbConnectionString
        {
            get
            {
                var connectionString = new SqlConnectionStringBuilder(_databaseContainer.GetConnectionString());
                connectionString.InitialCatalog = _database;
                return connectionString.ToString();
            }
        }
    }
}
