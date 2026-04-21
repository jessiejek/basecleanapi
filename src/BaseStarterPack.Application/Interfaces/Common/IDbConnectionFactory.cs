using System.Data;

namespace BaseStarterPack.Application.Interfaces.Common;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection(string connectionName);
}
