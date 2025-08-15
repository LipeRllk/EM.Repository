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
        /// Extensão para verificar se o aluno é maior de idade
        /// </summary>
        /// <param name="aluno">Aluno a ser verificado</param>
        /// <returns>True se for maior de idade</returns>
        public static bool EhMaiorDeIdade(this Aluno aluno)
        {
            var idade = DateTime.Today.Year - aluno.AlunoNascimento.Year;
            if (aluno.AlunoNascimento.Date > DateTime.Today.AddYears(-idade))
                idade--;
            return idade >= 18;
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
    }
}