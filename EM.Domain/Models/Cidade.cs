using EM.Domain.Interface;
using System.ComponentModel.DataAnnotations;

namespace EM.Domain.Models
{
    public class Cidade : IEntidade
    {
        public int Id { get; set; }

        [Display(Name = "Nome da cidade")]
        [Required(ErrorMessage = "O nome da cidade é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da cidade deve ter no máximo 100 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        [Display(Name = "UF")]
        [Required(ErrorMessage = "O UF é obrigatório.")]
        public string Uf { get; set; } = string.Empty;

        [Display(Name = "Codigo do IBGE")]
        [Required(ErrorMessage = "O código do IBGE é obrigatório.")]
        [StringLength(7, ErrorMessage = "O código do IBGE deve ter no máximo 7 caracteres.")]
        public string Ibge { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            if (obj is Cidade cidade)
                return Id == cidade.Id;
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Descricao} - {Uf}";
        }
    }
}
