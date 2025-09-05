using EM.Domain.Models;

namespace EM.Domain.Utilitarios
{
    /// <summary>
    /// Métodos de extensão para demonstrar o uso conforme requisitos
    /// </summary>
    public static class ExtensoesDominio
    {
        /// <summary>
        /// Extensão para formatar CPF com máscara
        /// </summary>
        /// <param name="cpf">CPF sem formatação</param>
        /// <returns>CPF formatado com máscara</returns>
        public static string FormatarCPF(this string cpf)
        {
            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11)
                return cpf;

            return Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
        }

        /// <summary>
        /// Extensão para remover formatação do CPF
        /// </summary>
        /// <param name="cpf">CPF formatado</param>
        /// <returns>CPF apenas com números</returns>
        public static string LimparCPF(this string cpf)
        {
            return string.IsNullOrEmpty(cpf) ? string.Empty : cpf.Replace(".", "").Replace("-", "");
        }

        /// <summary>
        /// Extensão para obter a idade do aluno
        /// </summary>
        /// <param name="aluno">Aluno</param>
        /// <returns>Idade em anos</returns>
        public static int ObterIdade(this Aluno aluno)
        {
            var idade = DateTime.Today.Year - aluno.AlunoNascimento.Year;
            if (aluno.AlunoNascimento.Date > DateTime.Today.AddYears(-idade))
                idade--;
            return idade;
        }

        /// <summary>
        /// Extensão para obter a idade formatada do aluno
        /// </summary>
        /// <param name="aluno">Aluno</param>
        /// <returns>Idade formatada (ex: "20 anos", "1 ano", "9 meses", "1 mês")</returns>
        public static string ObterIdadeFormatada(this Aluno aluno)
        {
            if (aluno?.AlunoNascimento == default(DateTime))
                return "Idade não informada";

            var hoje = DateTime.Today;
            var nascimento = aluno.AlunoNascimento.Date;
            
            // Verifica se a data é válida
            if (nascimento > hoje)
                return "Data inválida";

            var idadeAnos = aluno.ObterIdade();
            
            // Se tem 1 ano ou mais, mostra em anos
            if (idadeAnos >= 1)
            {
                return idadeAnos == 1 ? "1 ano" : $"{idadeAnos} anos";
            }
            
            // Se tem menos de 1 ano, calcula os meses
            var meses = 0;
            var dataTemp = nascimento;
            
            while (dataTemp.AddMonths(1) <= hoje)
            {
                meses++;
                dataTemp = dataTemp.AddMonths(1);
            }
            
            return meses switch
            {
                0 => "Recém-nascido",
                1 => "1 mês",
                _ => $"{meses} meses"
            };
        }

        /// <summary>
        /// Extensão para obter descrição do sexo
        /// </summary>
        /// <param name="sexo">Enumerador de sexo</param>
        /// <returns>Descrição textual</returns>
        public static string ObterDescricao(this EnumeradorSexo sexo)
        {
            return sexo switch
            {
                EnumeradorSexo.Masculino => "Masculino",
                EnumeradorSexo.Feminino => "Feminino",
                _ => "Não informado"
            };
        }

        /// <summary>
        /// Extensão para filtrar alunos por cidade usando LINQ
        /// </summary>
        /// <param name="alunos">Lista de alunos</param>
        /// <param name="cidadeCodigo">Código da cidade</param>
        /// <returns>Alunos da cidade especificada</returns>
        public static IEnumerable<Aluno> PorCidade(this IEnumerable<Aluno> alunos, int cidadeCodigo)
        {
            return alunos.Where(a => a.AlunoCidaCodigo == cidadeCodigo);
        }

        /// <summary>
        /// Extensão para ordenar alunos por nome usando LINQ
        /// </summary>
        /// <param name="alunos">Lista de alunos</param>
        /// <returns>Alunos ordenados por nome</returns>
        public static IEnumerable<Aluno> OrdenarPorNome(this IEnumerable<Aluno> alunos)
        {
            return alunos.OrderBy(a => a.AlunoNome);
        }

        /// <summary>
        /// Extensão para filtrar alunos por UF usando LINQ
        /// </summary>
        /// <param name="alunos">Lista de alunos</param>
        /// <param name="ufCodigo">Código da UF</param>
        /// <param name="cidades">Lista de cidades disponíveis para filtro</param>
        /// <returns>Alunos que pertencem a cidades da UF especificada</returns>
        public static IEnumerable<Aluno> PorUF(this IEnumerable<Aluno> alunos, string ufCodigo, IEnumerable<Cidade> cidades)
        {
            if (alunos == null) throw new ArgumentNullException(nameof(alunos));
            if (cidades == null) throw new ArgumentNullException(nameof(cidades));
            if (string.IsNullOrEmpty(ufCodigo)) return alunos;

            //as cidades pelo UF
            var cidadesDaUF = cidades.Where(c => c.CIDAUF == ufCodigo)
                                      .Select(c => c.CIDACODIGO)
                                      .ToList();

            // filtra os alunos que pertencem a essas cidades
            return alunos.Where(a => cidadesDaUF.Contains(a.AlunoCidaCodigo));
        }
    }
}