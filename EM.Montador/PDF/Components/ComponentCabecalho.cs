using iTextSharp.text;
using iTextSharp.text.pdf;
using EM.Montador.PDF.Models;

namespace EM.Montador.PDF.Components
{
    public class CabecalhoComponent(ConfigModelPDF config) : IComponentPDF
    {
        private readonly ConfigModelPDF _config = config;
        private static readonly float[] s_widths = [1f, 3f];

        public void AdicionarAoDocumento(Document document)
        {
            if (!_config.IncluirCabecalho) return;

            var tabelaCabecalho = new PdfPTable(2) { WidthPercentage = 100 };
            tabelaCabecalho.SetWidths(s_widths);

            document.Add(tabelaCabecalho);

            AdicionarLinhaSeparadora(document);
        }

        private static void AdicionarLinhaSeparadora(Document document)
        {
            var linha = new Paragraph("_".PadRight(80, '_'))
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20f,
                Font = FontFactory.GetFont("Arial", 8)
            };
            document.Add(linha);
        }
    }
}