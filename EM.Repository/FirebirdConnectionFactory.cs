using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;

namespace EM.Repository
{
    public class FirebirdConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public FirebirdConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration["DbHelper:ConnectionString"] 
                ?? throw new InvalidOperationException("A string de conex�o 'DbHelper:ConnectionString' n�o foi encontrada na configura��o.");
        }

        public FbConnection CreateConnection()
        {
            return new FbConnection(_connectionString);
        }
    }
}