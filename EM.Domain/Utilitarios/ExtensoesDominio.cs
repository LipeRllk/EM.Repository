using EM.Domain.Models;

namespace EM.Domain.Utilitarios
{
    public static class ExtensoesDominio
    {
        public static string FormatarCPF(this string cpf)
        {
            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11)
                return cpf;

            return Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
        }

        public static string LimparCPF(this string cpf)
        {
            return string.IsNullOrEmpty(cpf) ? string.Empty : cpf.Replace(".", "").Replace("-", "");
        }

        public static int ObterIdade(this Aluno aluno)
        {
            var idade = DateTime.Today.Year - aluno.AlunoNascimento.Year;
            if (aluno.AlunoNascimento.Date > DateTime.Today.AddYears(-idade))
                idade--;
            return idade;
        }

        public static string ObterIdadeFormatada(this Aluno aluno)
        {
            if (aluno == null || aluno.AlunoNascimento == default)
                return "Idade não informada";

            var hoje = DateTime.Today;
            var nascimento = aluno.AlunoNascimento.Date;

            if (nascimento > hoje)
                return "Data inválida";

            var idadeAnos = aluno.ObterIdade();

            if (idadeAnos >= 1)
            {
                return idadeAnos == 1 ? "1 ano" : $"{idadeAnos} anos";
            }

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

        public static string ObterDescricao(this EnumeradorSexo sexo)
        {
            return sexo switch
            {
                EnumeradorSexo.Masculino => "Masculino",
                EnumeradorSexo.Feminino => "Feminino",
                _ => "Não informado"
            };
        }

        public static IEnumerable<Aluno> PorCidade(this IEnumerable<Aluno> alunos, int cidadeCodigo)
        {
            return alunos.Where(a => a.AlunoCidaCodigo == cidadeCodigo);
        }

        public static IEnumerable<Aluno> OrdenarPorNome(this IEnumerable<Aluno> alunos)
        {
            return alunos.OrderBy(a => a.AlunoNome);
        }

        public static IEnumerable<Aluno> PorUF(this IEnumerable<Aluno> alunos, string ufCodigo, IEnumerable<Cidade> cidades)
        {
            ArgumentNullException.ThrowIfNull(alunos);
            ArgumentNullException.ThrowIfNull(cidades);
            if (string.IsNullOrEmpty(ufCodigo)) return alunos;

            var cidadesDaUF = cidades.Where(c => c.CIDAUF == ufCodigo)
                                      .Select(c => c.CIDACODIGO)
                                      .ToList();

            return alunos.Where(a => cidadesDaUF.Contains(a.AlunoCidaCodigo));
        }
    }
}