public class Aluno
{
    public int AlunoMatricula { get; set; }

    [Required(ErrorMessage = "O nome � obrigat�rio.")]
    public string AlunoNome { get; set; }

    [Required(ErrorMessage = "O CPF � obrigat�rio.")]
    public string AlunoCPF { get; set; }

    [Required(ErrorMessage = "O sexo � obrigat�rio.")]
    public string AlunoSexo { get; set; }

    [Required(ErrorMessage = "A data de nascimento � obrigat�ria.")]
    public DateTime AlunoNascimento { get; set; }

    [Required(ErrorMessage = "A cidade � obrigat�ria.")]
    public int AlunoCidaCodigo { get; set; }
}