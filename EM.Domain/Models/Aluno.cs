namespace EM.Domain.Models
{
    public class Aluno
    {
        public int AlunoMatricula { get; set; }
        public string AlunoNome { get; set; }
        public string AlunoCPF { get; set; }
        public DateTime AlunoNascimento { get; set; }
        public string AlunoSexo { get; set; }
        public int? AlunoCidaCodigo { get; set; }
    }
}
