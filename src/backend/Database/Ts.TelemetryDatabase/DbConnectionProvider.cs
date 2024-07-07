using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace TelemetryStash.Database;

#pragma warning disable IDE0290 // Use primary constructor

public interface IDbProvider
{
    IDbConnection CreateConnection();
    Task<T?> ExecuteScalar<T>(string storedProcedure, object? parameters, CancellationToken token);
    Task Execute(string storedProcedure, object? parameters, CancellationToken token);
}

public class DbConnectionProvider : IDbProvider
{
    private readonly string _connectionString;

    public DbConnectionProvider(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("TelemetryStashDatabase") ??
            throw new Exception("Missing TelemetryStashDatabase connection string");
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public async Task<T?> ExecuteScalar<T>(string storedProcedure, object? parameters, CancellationToken token)
    {
        var definition = new CommandDefinition(
            commandText: storedProcedure,
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: token);

        using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<T>(definition);
    }

    public async Task Execute(string storedProcedure, object? parameters, CancellationToken token)
    {
        var definition = new CommandDefinition(
            commandText: storedProcedure,
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: token);

        using var connection = CreateConnection();
        await connection.ExecuteAsync(definition);
    }
}
