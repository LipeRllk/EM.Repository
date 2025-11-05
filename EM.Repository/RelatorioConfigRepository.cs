using EM.Domain.Models;
using System.Data;

namespace EM.Repository
{
    public class RelatorioConfigRepository(IDbConnectionFactory connectionFactory)
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        public void Add(RelatorioConfig config)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = cn.CreateCommand();

            cmd.CommandText = @"INSERT INTO RelatorioConfig (NomeColegio, Endereco, Logo)
                                VALUES (@NomeColegio, @Endereco, @Logo)";
            cmd.Parameters.AddWithValue("@NomeColegio", config.NomeColegio);
            cmd.Parameters.AddWithValue("@Endereco", config.Endereco);
            cmd.Parameters.AddWithValue("@Logo", (object?)config.Logo ?? DBNull.Value);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public RelatorioConfig? BuscarPorId(int id)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = cn.CreateCommand();

            cmd.CommandText = "SELECT * FROM RelatorioConfig WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            cn.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return new RelatorioConfig
                {
                    Id = dr.GetInt32(dr.GetOrdinal("Id")),
                    NomeColegio = dr.GetString(dr.GetOrdinal("NomeColegio")),
                    Endereco = dr.GetString(dr.GetOrdinal("Endereco")),
                    Logo = dr.IsDBNull(dr.GetOrdinal("Logo")) ? null : (byte[])dr["Logo"]
                };
            }
            return null;
        }
    }
}