using iTextSharp.text;

namespace EM.Montador.PDF.Models
{
    public class ConfigModelPDF
    {
        public string Titulo { get; set; } = "";
        public Rectangle TamanhoPagina { get; set; } = PageSize.A4;
        public bool Paisagem { get; set; } = false;
        public float MargemEsquerda { get; set; } = 20f;
        public float MargemDireita { get; set; } = 20f;
        public float MargemSuperior { get; set; } = 30f;
        public float MargemInferior { get; set; } = 30f;
        public bool IncluirCabecalho { get; set; } = true;
        public bool IncluirRodape { get; set; } = true;
        public bool IncluirNumeroPagina { get; set; } = true;
        public string? NomeColegio { get; set; }
        public string? Endereco { get; set; }
        public byte[]? Logo { get; set; }
    }
}
