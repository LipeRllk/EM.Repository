using System.ComponentModel.DataAnnotations;

namespace EM.Domain.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DataNascimentoAttribute : ValidationAttribute
    {
        public int MinAge { get; set; } = 0;
        public int MaxAge { get; set; } = 120;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Aceita ausência aqui; [Required] cuida disso se necessário
            if (value is not DateTime dt) return ValidationResult.Success;

            if (dt == default)
                return new ValidationResult("A data de nascimento é obrigatória.");

            var hoje = DateTime.Today;

            if (dt.Date > hoje)
                return new ValidationResult("A data de nascimento não pode ser futura.");

            var idade = hoje.Year - dt.Year;
            if (dt.Date > hoje.AddYears(-idade)) idade--;

            if (idade < MinAge)
                return new ValidationResult($"O aluno deve ter pelo menos {MinAge} ano(s).");

            if (idade > MaxAge)
                return new ValidationResult($"O aluno não pode ter mais de {MaxAge} ano(s).");

            return ValidationResult.Success;
        }
    }
}