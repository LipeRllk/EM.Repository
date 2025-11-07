using EM.Domain.Models;
using EM.Domain.Utilitarios;
using System.Linq.Expressions;

namespace EM.Repository
{
    public class AlunoRepository(IDbConnectionFactory connectionFactory) : RepositorioAbstrato<Aluno>(connectionFactory)
    {
        public override void Add(Aluno aluno)
        {
            Inserir(aluno);
        }

        public override void Remove(Aluno aluno)
        {
            Excluir(aluno.AlunoMatricula);
        }

        public override void Update(Aluno aluno)
        {
            Atualizar(aluno);
        }

        public override IEnumerable<Aluno> GetAll()
        {
            return BuscarAlunos().AsEnumerable();
        }

        public override IEnumerable<Aluno> Get(Expression<Func<Aluno, bool>> predicate)
        {
            var func = predicate.Compile();
            return GetAll().Where(func);
        }

        public List<Aluno> BuscarAlunos(string? search = null)
        {
            List<Aluno> alunos = [];
            
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = cn.CreateCommand();
            
            var sql = "SELECT A.*, C.CIDADESCRICAO FROM TBALUNO A " +
                     "LEFT JOIN TBCIDADE C ON C.CIDACODIGO = A.CIDACODIGO ";

            if (!string.IsNullOrWhiteSpace(search))
            {
                cmd.Parameters.AddWithValue("@search", $"%{search.ToLower()}%");
                sql += " WHERE LOWER(A.NOME) LIKE @search";
                
                if (int.TryParse(search, out int matricula))
                {
                    cmd.Parameters.AddWithValue("@matricula", matricula);
                    sql = sql.Replace("WHERE", "WHERE A.MATRICULA = @matricula OR");
                }
            }

            sql += " ORDER BY A.NOME";
            cmd.CommandText = sql;

            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                alunos.Add(new()
                {
                    AlunoMatricula = dr.GetInt32(dr.GetOrdinal("MATRICULA")),
                    AlunoNome = dr.GetString(dr.GetOrdinal("NOME")),
                    AlunoCPF = dr.IsDBNull(dr.GetOrdinal("CPF")) ? "" : dr.GetString(dr.GetOrdinal("CPF")),
                    AlunoSexo = dr.GetString(dr.GetOrdinal("SEXO")),
                    AlunoNascimento = dr.GetDateTime(dr.GetOrdinal("NASCIMENTO")),
                    AlunoCidaCodigo = dr.GetInt32(dr.GetOrdinal("CIDACODIGO"))
                });
            }
            
            return [.. alunos.OrdenarPorNome()];
        }

        public void Inserir(Aluno aluno)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = cn.CreateCommand();
            
            cmd.CommandText = @"INSERT INTO TBALUNO (NOME, CPF, SEXO, NASCIMENTO, CIDACODIGO) 
                              VALUES (@NOME, @CPF, @SEXO, @NASCIMENTO, @CIDACODIGO)";
            
            cmd.Parameters.AddWithValue("@NOME", aluno.AlunoNome);
            var cpfLimpo = (aluno.AlunoCPF ?? string.Empty).LimparCPF();
            cmd.Parameters.AddWithValue("@CPF", string.IsNullOrEmpty(cpfLimpo) ? DBNull.Value : (object)cpfLimpo);
            cmd.Parameters.AddWithValue("@SEXO", aluno.AlunoSexo);
            cmd.Parameters.AddWithValue("@NASCIMENTO", aluno.AlunoNascimento);
            cmd.Parameters.AddWithValue("@CIDACODIGO", aluno.AlunoCidaCodigo);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public Aluno? BuscarPorMatricula(int matricula)
        {
            return GetSingle(a => a.AlunoMatricula == matricula);
        }

        public Aluno? BuscarPorMatriculaTradicional(int matricula)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = cn.CreateCommand();
            
            cmd.CommandText = "SELECT * FROM TBALUNO WHERE MATRICULA = @MATRICULA";
            cmd.Parameters.AddWithValue("@MATRICULA", matricula);

            cn.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return new()
                {
                    AlunoMatricula = dr.GetInt32(dr.GetOrdinal("MATRICULA")),
                    AlunoNome = dr.GetString(dr.GetOrdinal("NOME")),
                    AlunoCPF = dr.IsDBNull(dr.GetOrdinal("CPF")) ? "" : dr.GetString(dr.GetOrdinal("CPF")),
                    AlunoSexo = dr.GetString(dr.GetOrdinal("SEXO")),
                    AlunoNascimento = dr.GetDateTime(dr.GetOrdinal("NASCIMENTO")),
                    AlunoCidaCodigo = dr.GetInt32(dr.GetOrdinal("CIDACODIGO"))
                };
            }
            return null;
        }

        public void Atualizar(Aluno aluno)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = cn.CreateCommand();
            
            cmd.CommandText = @"UPDATE TBALUNO SET 
                              NOME = @NOME, 
                              CPF = @CPF, 
                              SEXO = @SEXO, 
                              NASCIMENTO = @NASCIMENTO, 
                              CIDACODIGO = @CIDACODIGO 
                              WHERE MATRICULA = @MATRICULA";

            cmd.Parameters.AddWithValue("@MATRICULA", aluno.AlunoMatricula);
            cmd.Parameters.AddWithValue("@NOME", aluno.AlunoNome);
            var cpfLimpo = (aluno.AlunoCPF ?? string.Empty).LimparCPF();
            cmd.Parameters.AddWithValue("@CPF", string.IsNullOrEmpty(cpfLimpo) ? DBNull.Value : (object)cpfLimpo);
            cmd.Parameters.AddWithValue("@SEXO", aluno.AlunoSexo);
            cmd.Parameters.AddWithValue("@NASCIMENTO", aluno.AlunoNascimento);
            cmd.Parameters.AddWithValue("@CIDACODIGO", aluno.AlunoCidaCodigo);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Excluir(int matricula)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = cn.CreateCommand();
            
            cmd.CommandText = "DELETE FROM TBALUNO WHERE MATRICULA = @MATRICULA";
            cmd.Parameters.AddWithValue("@MATRICULA", matricula);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public int ContarPorCidade(int cidadeId)
        {
            return Count(a => a.AlunoCidaCodigo == cidadeId);
        }

        public int ContarPorCidadeTradicional(int cidadeId)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = cn.CreateCommand();
            
            cmd.CommandText = "SELECT COUNT(*) FROM TBALUNO WHERE CIDACODIGO = @CIDACODIGO";
            cmd.Parameters.AddWithValue("@CIDACODIGO", cidadeId);

            cn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public IEnumerable<Aluno> BuscarPorConteudoDoNome(string conteudo)
        {
            return Get(a => a.AlunoNome.Contains(conteudo, StringComparison.OrdinalIgnoreCase));
        }
    }
}
