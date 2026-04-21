using BaseStarterPack.Application.Interfaces.Common;
using Dapper;
using System.Data;
using System.Data.Common;

namespace BaseStarterPack.Infrastructure.Data;

public class SqlDataAccess(IDbConnectionFactory connectionFactory) : ISqlDataAccess
{
    public async Task<IEnumerable<T>> LoadDataAsync<T, U>(
        string sql,
        U parameters,
        string connectionName,
        bool isStoredProcedure = false,
        IDbTransaction? transaction = null)
    {
        var commandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;

        if (transaction?.Connection is not null)
        {
            return await transaction.Connection.QueryAsync<T>(
                sql,
                parameters,
                transaction: transaction,
                commandType: commandType);
        }

        using var connection = connectionFactory.CreateConnection(connectionName);
        if (connection is DbConnection dbConnection)
        {
            await dbConnection.OpenAsync();
            return await dbConnection.QueryAsync<T>(
                sql,
                parameters,
                commandType: commandType);
        }

        connection.Open();
        return await connection.QueryAsync<T>(
            sql,
            parameters,
            commandType: commandType);
    }

    public async Task<T?> LoadFirstAsync<T, U>(
        string sql,
        U parameters,
        string connectionName,
        bool isStoredProcedure = false,
        IDbTransaction? transaction = null)
    {
        var commandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;

        if (transaction?.Connection is not null)
        {
            return await transaction.Connection.QueryFirstOrDefaultAsync<T>(
                sql,
                parameters,
                transaction: transaction,
                commandType: commandType);
        }

        using var connection = connectionFactory.CreateConnection(connectionName);
        if (connection is DbConnection dbConnection)
        {
            await dbConnection.OpenAsync();
            return await dbConnection.QueryFirstOrDefaultAsync<T>(
                sql,
                parameters,
                commandType: commandType);
        }

        connection.Open();
        return await connection.QueryFirstOrDefaultAsync<T>(
            sql,
            parameters,
            commandType: commandType);
    }

    public async Task<bool> ExecuteAsync<T>(
        string sql,
        T parameters,
        string connectionName,
        bool isStoredProcedure = false,
        IDbTransaction? transaction = null)
    {
        var commandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;

        int affectedRows;
        if (transaction?.Connection is not null)
        {
            affectedRows = await transaction.Connection.ExecuteAsync(
                sql,
                parameters,
                transaction: transaction,
                commandType: commandType);
            return affectedRows > 0;
        }

        using var connection = connectionFactory.CreateConnection(connectionName);
        if (connection is DbConnection dbConnection)
        {
            await dbConnection.OpenAsync();
            affectedRows = await dbConnection.ExecuteAsync(
                sql,
                parameters,
                commandType: commandType);
            return affectedRows > 0;
        }

        connection.Open();
        affectedRows = await connection.ExecuteAsync(
            sql,
            parameters,
            commandType: commandType);
        return affectedRows > 0;
    }
}
