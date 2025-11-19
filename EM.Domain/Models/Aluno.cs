using EM.Domain.Interface;
using EM.Domain.Models.Attributes;
using EM.Domain.Helpers;
using System.ComponentModel.DataAnnotations;

namespace EM.Domain.Models
{
    public class Aluno : IEntidade
    {
        public int Matricula { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
        [Display(Name = "NOME")]
        public string Nome { get; set; } = string.Empty;

        [CpfAttribute(ErrorMessage = "CPF inválido.")]
        [Display(Name = "CPF")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public string? Cpf { get; set; } 

        [Required(ErrorMessage = "O sexo é obrigatório.")]
        [Display(Name = "Sexo")]
        public string Sexo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [Display(Name = "Data de nascimento")]
        public DateTime DataNascimento { get; set; }
    
        [Required(ErrorMessage = "A cidade é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma cidade válida.")]
        [Display(Name = "Cidade")]
        public int AlunoCidaCodigo { get; set; }

        public EnumeradorSexo EnumSexo
        {
            get => SexoHelper.ParseOrDefault(Sexo);
            set => Sexo = SexoHelper.ToStorageString(value);
        }

        public int Idade => CalcularIdade();
        public override bool Equals(object? obj) =>
            obj is Aluno aluno && Matricula == aluno.Matricula;

        public override int GetHashCode() => Matricula.GetHashCode();

        public override string ToString() => $"{Nome} - Matrícula: {Matricula}";

        public int CalcularIdade()
        {
            if (DataNascimento == default) return 0;
            var hoje = DateTime.Today;
            var idade = hoje.Year - DataNascimento.Year;
            if (DataNascimento.Date > hoje.AddYears(-idade))
                idade--;
            return idade;
        }
    }
}
