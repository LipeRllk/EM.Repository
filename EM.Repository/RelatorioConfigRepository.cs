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

            cmd.CommandText = "DELETE FROM RelatorioConfig WHERE Id = 1";
            cn.Open();
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"INSERT INTO RelatorioConfig (Id, NomeColegio, Endereco, Logo)
                                VALUES (1, @NomeColegio, @Endereco, @Logo)";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@NomeColegio", config.NomeColegio ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Endereco", config.Endereco ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Logo", (object?)config.Logo ?? DBNull.Value);

            int rowsAffected = cmd.ExecuteNonQuery();
            Console.WriteLine($"Linhas afetadas: {rowsAffected}");
        }

        public void Upsert(RelatorioConfig config)
        {
            using var cn = _connectionFactory.CreateConnection();
            cn.Open();
            using var cmd = cn.CreateCommand();

            cmd.CommandText = @"UPDATE RelatorioConfig 
                                SET NomeColegio = @NomeColegio, 
                                    Endereco = @Endereco, 
                                    Logo = COALESCE(@Logo, Logo)
                                WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", config.Id == 0 ? 1 : config.Id);
            cmd.Parameters.AddWithValue("@NomeColegio", (object?)config.NomeColegio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Endereco", (object?)config.Endereco ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Logo", (object?)config.Logo ?? DBNull.Value);

            var rows = cmd.ExecuteNonQuery();

            if (rows == 0)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = @"INSERT INTO RelatorioConfig (Id, NomeColegio, Endereco, Logo)
                                    VALUES (@Id, @NomeColegio, @Endereco, @Logo)";
                cmd.Parameters.AddWithValue("@Id", config.Id == 0 ? 1 : config.Id);
                cmd.Parameters.AddWithValue("@NomeColegio", (object?)config.NomeColegio ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Endereco", (object?)config.Endereco ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Logo", (object?)config.Logo ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public RelatorioConfig? BuscarPorId(int id)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = cn.CreateCommand();

            cmd.CommandText = "SELECT Id, NomeColegio, Endereco, Logo FROM RelatorioConfig WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            cn.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return new RelatorioConfig
                {
                    Id = dr.GetInt32(dr.GetOrdinal("Id")),
                    NomeColegio = dr.IsDBNull(dr.GetOrdinal("NomeColegio")) ? string.Empty : dr.GetString(dr.GetOrdinal("NomeColegio")),
                    Endereco = !dr.IsDBNull(dr.GetOrdinal("Endereco")) ? dr.GetString(dr.GetOrdinal("Endereco")) : string.Empty,
                    Logo = dr.IsDBNull(dr.GetOrdinal("Logo")) ? null : (byte[])dr["Logo"]
                };
            }
            return null;
        }
    }
}

