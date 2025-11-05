using System;

namespace EM.Domain.Models
{
    public class RelatorioConfig
    {
        public int Id { get; set; }
        public string NomeColegio { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public byte[]? Logo { get; set; }
    }
}