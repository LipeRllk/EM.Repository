using System.ComponentModel.DataAnnotations;

namespace EM.Domain.Models
{
    public class Cidade
    {
        public int CIDACODIGO { get; set; }

        [Display(Name = "Nome da cidade")]
        [Required(ErrorMessage = "O nome da cidade é obrigatório.")]
        public string CIDADESCRICAO { get; set; } = string.Empty; // Adicionado valor padrão

        [Display(Name = "UF")]
        [Required(ErrorMessage = "O UF é obrigatório.")]
        public string CIDAUF { get; set; } = string.Empty; // Adicionado valor padrão

        [Display(Name = "Codigo do IBGE")]
        [Required(ErrorMessage = "O código do IBGE é obrigatório.")]
        public string CIDACODIGOIBGE { get; set; } = string.Empty; // Adicionado valor padrão
    }
}
