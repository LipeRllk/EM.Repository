using EM.Domain.Models;
using System.Linq.Expressions;

namespace EM.Repository
{
    public class CidadeRepository(IDbConnectionFactory connectionFactory) : RepositorioAbstrato<Cidade>(connectionFactory)
    {
        public override void Add(Cidade cidade)
        {
            Inserir(cidade);
        }

        public override void Remove(Cidade cidade)
        {
            Excluir(cidade.Id);
        }

        public override void Update(Cidade cidade)
        {
            Atualizar(cidade);
        }

        public override IEnumerable<Cidade> GetAll()
        {
            return BuscarCidades().AsEnumerable();
        }

        public override IEnumerable<Cidade> Get(Expression<Func<Cidade, bool>> predicate)
        {
            var func = predicate.Compile();
            return GetAll().Where(func);
        }

        public List<Cidade> BuscarCidades(string? search = null)
        {
            List<Cidade> cidades = [];
            
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = cn.CreateCommand();
            
            var sql = "SELECT * FROM TBCIDADE";

            if (!string.IsNullOrWhiteSpace(search))
            {
                cmd.Parameters.AddWithValue("@search", $"%{search.ToLower()}%");
                sql += " WHERE LOWER(CIDADESCRICAO) LIKE @search";
                
                if (int.TryParse(search, out int codigo))
                {
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    sql = sql.Replace("WHERE", "WHERE CIDACODIGO = @codigo OR");
                }
            }

            sql += " ORDER BY CIDADESCRICAO";
            cmd.CommandText = sql;

            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                cidades.Add(new()
                {
                    Id = dr.GetInt32(dr.GetOrdinal("CIDACODIGO")),
                    Descricao = dr.GetString(dr.GetOrdinal("CIDADESCRICAO")),
                    Uf = dr.GetString(dr.GetOrdinal("CIDAUF")),
                    Ibge = dr.GetString(dr.GetOrdinal("CIDACODIGOIBGE"))
                });
            }
            
            return [.. cidades.OrderBy(c => c.Descricao)];
        }

        public List<Cidade> ListarTodas()
        {
            return BuscarCidades();
        }

        public void Inserir(Cidade cidade)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = cn.CreateCommand();
            
            cmd.CommandText = @"INSERT INTO TBCIDADE (CIDADESCRICAO, CIDAUF, CIDACODIGOIBGE) 
                              VALUES (@CIDADESCRICAO, @CIDAUF, @CIDACODIGOIBGE)";
            
            cmd.Parameters.AddWithValue("@CIDADESCRICAO", cidade.Descricao);
            cmd.Parameters.AddWithValue("@CIDAUF", cidade.Uf);
            cmd.Parameters.AddWithValue("@CIDACODIGOIBGE", cidade.Ibge);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public Cidade? BuscarPorId(int id)
        {
            return GetSingle(c => c.Id == id);
        }

        public Cidade? BuscarPorIdTradicional(int id)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = cn.CreateCommand();
            
            cmd.CommandText = "SELECT * FROM TBCIDADE WHERE CIDACODIGO = @CIDACODIGO";
            cmd.Parameters.AddWithValue("@CIDACODIGO", id);

            cn.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return new()
                {
                    Id = dr.GetInt32(dr.GetOrdinal("CIDACODIGO")),
                    Descricao = dr.GetString(dr.GetOrdinal("CIDADESCRICAO")),
                    Uf = dr.GetString(dr.GetOrdinal("CIDAUF")),
                    Ibge = dr.GetString(dr.GetOrdinal("CIDACODIGOIBGE"))
                };
            }
            return null;
        }

        public void Atualizar(Cidade cidade)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = cn.CreateCommand();
            
            cmd.CommandText = @"UPDATE TBCIDADE SET 
                              CIDADESCRICAO = @CIDADESCRICAO, 
                              CIDAUF = @CIDAUF, 
                              CIDACODIGOIBGE = @CIDACODIGOIBGE 
                              WHERE CIDACODIGO = @CIDACODIGO";

            cmd.Parameters.AddWithValue("@CIDACODIGO", cidade.Id);
            cmd.Parameters.AddWithValue("@CIDADESCRICAO", cidade.Descricao);
            cmd.Parameters.AddWithValue("@CIDAUF", cidade.Uf);
            cmd.Parameters.AddWithValue("@CIDACODIGOIBGE", cidade.Ibge);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Excluir(int id)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = cn.CreateCommand();
            
            cmd.CommandText = "DELETE FROM TBCIDADE WHERE CIDACODIGO = @CIDACODIGO";
            cmd.Parameters.AddWithValue("@CIDACODIGO", id);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public IEnumerable<Cidade> BuscarPorUF(string uf)
        {
            return Get(c => c.Uf.Equals(uf, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Cidade> BuscarPorConteudoNome(string conteudo)
        {
            return Get(c => c.Descricao.Contains(conteudo, StringComparison.OrdinalIgnoreCase));
        }
    }
}