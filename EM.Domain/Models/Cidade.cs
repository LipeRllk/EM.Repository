using System.ComponentModel.DataAnnotations;

namespace EM.Domain.Models
{
    public class Cidade
    {
        public int CIDACODIGO { get; set; }

        [Display(Name = "Nome da cidade")]
        [Required(ErrorMessage = "O nome da cidade é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da cidade deve ter no máximo 100 caracteres.")]
        public string CIDADESCRICAO { get; set; } = string.Empty;

        [Display(Name = "UF")]
        [Required(ErrorMessage = "O UF é obrigatório.")]
        public string CIDAUF { get; set; } = string.Empty;

        [Display(Name = "Codigo do IBGE")]
        [Required(ErrorMessage = "O código do IBGE é obrigatório.")]
        [StringLength(7, ErrorMessage = "O código do IBGE deve ter no máximo 7 caracteres.")]
        public string CIDACODIGOIBGE { get; set; } = string.Empty;
    }
}
