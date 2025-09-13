using EM.Domain.Interface;
using EM.Domain.Models.Attributes;
using System.ComponentModel.DataAnnotations;

namespace EM.Domain.Models
{
    public class Aluno : IEntidade
    {
        public int AlunoMatricula { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
        [Display(Name = "NOME")]
        public string AlunoNome { get; set; } = string.Empty;

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

        public EnumeradorSexo Sexo
        {
            get => Enum.TryParse<EnumeradorSexo>(AlunoSexo, out var sexo) ? sexo : EnumeradorSexo.Masculino;
            set => AlunoSexo = ((int)value).ToString();
        }

        public override bool Equals(object? obj)
        {
            if (obj is Aluno aluno)
                return AlunoMatricula == aluno.AlunoMatricula;
            return false;
        }

        public override int GetHashCode()
        {
            return AlunoMatricula.GetHashCode();
        }

        public override string ToString()
        {
            return $"{AlunoNome} - Matrícula: {AlunoMatricula}";
        }
    }
}
