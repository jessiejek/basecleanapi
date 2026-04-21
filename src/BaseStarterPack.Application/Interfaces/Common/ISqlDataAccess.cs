using System.Data;

namespace BaseStarterPack.Application.Interfaces.Common;

public interface ISqlDataAccess
{
    Task<IEnumerable<T>> LoadDataAsync<T, U>(
        string sql,
        U parameters,
        string connectionName,
        bool isStoredProcedure = false,
        IDbTransaction? transaction = null);

    Task<T?> LoadFirstAsync<T, U>(
        string sql,
        U parameters,
        string connectionName,
        bool isStoredProcedure = false,
        IDbTransaction? transaction = null);

    Task<bool> ExecuteAsync<T>(
        string sql,
        T parameters,
        string connectionName,
        bool isStoredProcedure = false,
        IDbTransaction? transaction = null);
}
