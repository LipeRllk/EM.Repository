using FirebirdSql.Data.FirebirdClient;

namespace EM.Repository
{
    internal static class DbHelper
    {
        private static string ConnectionString = "User=SYSDBA;Password=masterkey;Database=C:\\Bancos\\TESTE.FDB;DataSource=localhost;Port=3055;Dialect=3;";

        public static FbConnection CreateConnection()
        {
            return new FbConnection(ConnectionString);
        }
    }
}