using FirebirdSql.Data.FirebirdClient;

namespace EM.Repository
{
    internal static class DbHelper
    {
        private static string ConnectionString = "User=SYSDBA;Password=masterkey;Database=C:\\Users\\Fellype\\Projetos\\TESTE.FDB;DataSource=localhost;Port=3050;Dialect=3;";

        public static FbConnection CreateConnection()
        {
            return new FbConnection(ConnectionString);
        }
    }
}