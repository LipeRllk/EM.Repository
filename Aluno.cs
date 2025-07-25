public class Aluno
{
    public int AlunoMatricula { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string AlunoNome { get; set; }

    [Required(ErrorMessage = "O CPF é obrigatório.")]
    public string AlunoCPF { get; set; }

    [Required(ErrorMessage = "O sexo é obrigatório.")]
    public string AlunoSexo { get; set; }

    [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
    public DateTime AlunoNascimento { get; set; }

    [Required(ErrorMessage = "A cidade é obrigatória.")]
    public int AlunoCidaCodigo { get; set; }
}