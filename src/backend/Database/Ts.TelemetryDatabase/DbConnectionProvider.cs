using Microsoft.Data.SqlClient;
using RepoDb;
using System.Data;

namespace TelemetryStash.Database;

public interface IDbProvider
{
    IDbConnection CreateConnection();
    Task<T?> ExecuteStoredProcedure<T>(string storedProcedure, object? parameters, CancellationToken token);
    Task ExecuteStoredProcedure(string storedProcedure, object? parameters, CancellationToken token);
}

public class DbConnectionProvider(string connectionString) : IDbProvider
{
    public IDbConnection CreateConnection()
    {
        return new SqlConnection(connectionString);
    }

    public async Task<T?> ExecuteStoredProcedure<T>(string storedProcedure, object? parameters, CancellationToken token)
    {
        using var connection = CreateConnection();
        return (await connection.ExecuteQueryAsync<T>(
            commandText: storedProcedure,
            param: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: token
            )).FirstOrDefault();
    }

    public async Task ExecuteStoredProcedure(string storedProcedure, object? parameters, CancellationToken token)
    {
        using var connection = CreateConnection();
        await connection.ExecuteNonQueryAsync(
            commandText: storedProcedure,
            param: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: token
            );
    }
}
