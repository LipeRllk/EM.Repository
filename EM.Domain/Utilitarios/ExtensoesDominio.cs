using EM.Domain.Models;

namespace EM.Domain.Utilitarios
{
    /// <summary>
    /// M�todos de extens�o para demonstrar o uso conforme requisitos
    /// </summary>
    public static class ExtensoesDominio
    {
        /// <summary>
        /// Extens�o para formatar CPF com m�scara
        /// </summary>
        /// <param name="cpf">CPF sem formata��o</param>
        /// <returns>CPF formatado com m�scara</returns>
        public static string FormatarCPF(this string cpf)
        {
            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11)
                return cpf;

            return Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
        }

        /// <summary>
        /// Extens�o para remover formata��o do CPF
        /// </summary>
        /// <param name="cpf">CPF formatado</param>
        /// <returns>CPF apenas com n�meros</returns>
        public static string LimparCPF(this string cpf)
        {
            return string.IsNullOrEmpty(cpf) ? string.Empty : cpf.Replace(".", "").Replace("-", "");
        }

        /// <summary>
        /// Extens�o para verificar se o aluno � maior de idade
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
        /// Extens�o para obter a idade do aluno
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
        /// Extens�o para obter descri��o do sexo
        /// </summary>
        /// <param name="sexo">Enumerador de sexo</param>
        /// <returns>Descri��o textual</returns>
        public static string ObterDescricao(this EnumeradorSexo sexo)
        {
            return sexo switch
            {
                EnumeradorSexo.Masculino => "Masculino",
                EnumeradorSexo.Feminino => "Feminino",
                _ => "N�o informado"
            };
        }

        /// <summary>
        /// Extens�o para filtrar alunos por cidade usando LINQ
        /// </summary>
        /// <param name="alunos">Lista de alunos</param>
        /// <param name="cidadeCodigo">C�digo da cidade</param>
        /// <returns>Alunos da cidade especificada</returns>
        public static IEnumerable<Aluno> PorCidade(this IEnumerable<Aluno> alunos, int cidadeCodigo)
        {
            return alunos.Where(a => a.AlunoCidaCodigo == cidadeCodigo);
        }

        /// <summary>
        /// Extens�o para ordenar alunos por nome usando LINQ
        /// </summary>
        /// <param name="alunos">Lista de alunos</param>
        /// <returns>Alunos ordenados por nome</returns>
        public static IEnumerable<Aluno> OrdenarPorNome(this IEnumerable<Aluno> alunos)
        {
            return alunos.OrderBy(a => a.AlunoNome);
        }
    }
}