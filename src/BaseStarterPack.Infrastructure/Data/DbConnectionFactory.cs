using BaseStarterPack.Application.Interfaces.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BaseStarterPack.Infrastructure.Data;

public class DbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    public IDbConnection CreateConnection(string connectionName)
    {
        var connectionString = ResolveConnectionString(connectionName);
        var dbType = ResolveDatabaseType(connectionName, connectionString);

        return dbType switch
        {
            DatabaseType.SqlServer => new SqlConnection(connectionString),
            DatabaseType.Ase => CreateAseConnection(connectionString),
            _ => throw new NotSupportedException($"Database type '{dbType}' is not supported.")
        };
    }

    private string ResolveConnectionString(string connectionName)
    {
        var conn = configuration.GetConnectionString(connectionName);

        if (!string.IsNullOrWhiteSpace(conn))
            return conn;

        // Fallback to direct environment variable names
        var directEnv = Environment.GetEnvironmentVariable(connectionName);
        if (!string.IsNullOrWhiteSpace(directEnv))
            return directEnv;

        // Common key mapping fallback
        var mapped = connectionName switch
        {
            "DefaultConnection" => Environment.GetEnvironmentVariable("DB_DEFAULT"),
            "ReportingDb" => Environment.GetEnvironmentVariable("DB_REPORTING"),
            "AseDb" => Environment.GetEnvironmentVariable("DB_ASE"),
            _ => null
        };

        if (!string.IsNullOrWhiteSpace(mapped))
            return mapped;

        throw new InvalidOperationException($"Connection string '{connectionName}' was not found.");
    }

    private DatabaseType ResolveDatabaseType(string connectionName, string connectionString)
    {
        var configuredType = configuration[$"DatabaseProviders:{connectionName}"];
        if (Enum.TryParse<DatabaseType>(configuredType, true, out var parsed))
            return parsed;

        if (connectionName.Contains("ase", StringComparison.OrdinalIgnoreCase))
            return DatabaseType.Ase;

        // safe default
        return DatabaseType.SqlServer;
    }

    private static IDbConnection CreateAseConnection(string connectionString)
    {
        var aseType =
            Type.GetType("AdoNetCore.AseClient.AseConnection, AdoNetCore.AseClient", throwOnError: false)
            ?? Type.GetType("Sybase.Data.AseClient.AseConnection, Sybase.Data.AseClient", throwOnError: false);

        if (aseType is null)
        {
            throw new InvalidOperationException(
                "ASE provider selected but ASE client assembly is not available. " +
                "Install an ASE ADO.NET provider package (e.g. AdoNetCore.AseClient) and configure AseDb.");
        }

        var instance = Activator.CreateInstance(aseType, connectionString);
        if (instance is not IDbConnection dbConnection)
        {
            throw new InvalidOperationException("Resolved ASE connection type does not implement IDbConnection.");
        }

        return dbConnection;
    }
}
