using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;

namespace EM.Repository
{
    public class FirebirdConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
    {
        private readonly string _connectionString = configuration["DbHelper:ConnectionString"] 
            ?? throw new InvalidOperationException("A string de conexão 'DbHelper:ConnectionString' não foi encontrada na configuração.");

        public FbConnection CreateConnection()
        {
            return new FbConnection(_connectionString);
        }
    }
}