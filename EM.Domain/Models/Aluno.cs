using EM.Domain.Models.Attributes;
using System.ComponentModel.DataAnnotations;

namespace EM.Domain.Models
{
    public class Aluno
    {
        public int AlunoMatricula { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [Display(Name = "NOME")]
        public string AlunoNome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [CpfAttribute(ErrorMessage = "CPF inválido.")]
        [Display(Name = "CPF")]
        public string AlunoCPF { get; set; } = string.Empty;

        [Required(ErrorMessage = "O sexo é obrigatório.")]
        [Display(Name = "Sexo")]
        public string AlunoSexo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [Display(Name = "Data de nascimento")]
        public DateTime AlunoNascimento { get; set; }
    
        [Required(ErrorMessage = "A cidade é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma cidade válida.")]
        [Display(Name = "Cidade")]
        public int AlunoCidaCodigo { get; set; }
    }
}
