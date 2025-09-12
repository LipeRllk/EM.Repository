using FirebirdSql.Data.FirebirdClient;

namespace EM.Repository
{
    public interface IDbConnectionFactory
    {
        FbConnection CreateConnection();
    }
}