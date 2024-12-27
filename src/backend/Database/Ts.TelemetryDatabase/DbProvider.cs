using Microsoft.Data.SqlClient;
using RepoDb;
using System.Data;
using System.Linq.Expressions;

namespace TelemetryStash.Database;

public interface IDbProvider
{
    IDbConnection CreateConnection();
    Task<T> ExecuteSingle<T>(string storedProcedure, object? parameters, CancellationToken cancellationToken);
    Task<List<T>> ExecuteMultiple<T>(string storedProcedure, object? parameters, CancellationToken cancellationToken);
    Task ExecuteScalar(string storedProcedure, object? parameters, CancellationToken cancellationToken);
    Task<T> QuerySingle<T>(Expression<Func<T, bool>> where, string? tableName = null, CancellationToken cancellationToken = default) where T : class;
}

internal class DbProvider(string connectionString) : IDbProvider
{
    public IDbConnection CreateConnection()
    {
        return new SqlConnection(connectionString);
    }

    public async Task<T> ExecuteSingle<T>(string storedProcedure, object? parameters, CancellationToken cancellationToken)
    {
        var response = await ExecuteMultiple<T>(storedProcedure, parameters, cancellationToken);

        return response.SingleOrDefault()
            ?? throw new Exception($"Unexpected null response from ExecuteSingle {storedProcedure}, entity {typeof(T).FullName}");
    }

    public async Task<List<T>> ExecuteMultiple<T>(string storedProcedure, object? parameters, CancellationToken cancellationToken)
    {
        using var connection = CreateConnection();
        var response = await connection.ExecuteQueryAsync<T>
        (
            commandText: storedProcedure,
            param: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        return response.ToList();
    }

    public async Task ExecuteScalar(string storedProcedure, object? parameters, CancellationToken cancellationToken)
    {
        using var connection = CreateConnection();
        await connection.ExecuteNonQueryAsync
        (
            commandText: storedProcedure,
            param: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );
    }

    public async Task<T> QuerySingle<T>(Expression<Func<T, bool>> where, string? tableName = null, CancellationToken cancellationToken = default) where T : class
    {
        tableName ??= $"[dbo].[{typeof(T).Name}s]";
        using var connection = CreateConnection();

        var response = await connection
            .QueryAsync<T>(tableName, where, cancellationToken: cancellationToken);

        return response.SingleOrDefault()
            ?? throw new Exception($"Unexpected null response from QuerySingle on table {tableName}, entity {typeof(T).FullName}");
    }
}
