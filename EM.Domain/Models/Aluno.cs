using EM.Domain.Interface;
using EM.Domain.Models.Attributes;
using EM.Domain.Helpers;
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
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public string? AlunoCPF { get; set; } 

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
            get => SexoHelper.ParseOrDefault(AlunoSexo);
            set => AlunoSexo = SexoHelper.ToStorageString(value);
        }

        public int Idade => CalcularIdade();
        public override bool Equals(object? obj) =>
            obj is Aluno aluno && AlunoMatricula == aluno.AlunoMatricula;

        public override int GetHashCode() => AlunoMatricula.GetHashCode();

        public override string ToString() => $"{AlunoNome} - Matrícula: {AlunoMatricula}";

        public int CalcularIdade()
        {
            if (AlunoNascimento == default) return 0;
            var hoje = DateTime.Today;
            var idade = hoje.Year - AlunoNascimento.Year;
            if (AlunoNascimento.Date > hoje.AddYears(-idade))
                idade--;
            return idade;
        }
    }
}
