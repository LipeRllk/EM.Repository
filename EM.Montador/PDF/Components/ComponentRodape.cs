using iTextSharp.text;
using iTextSharp.text.pdf;
using EM.Montador.PDF.Models;

namespace EM.Montador.PDF.Components
{
    public class RodapeComponent(ConfigModelPDF config) : IComponentPDF
    {
        private readonly ConfigModelPDF _config = config;

        private static readonly float[] s_widths = [1f, 1f];

        public void AdicionarAoDocumento(Document document)
        {
            if (!_config.IncluirRodape) return;

            document.Add(new Paragraph(" ") { SpacingBefore = 30f });

            var linha = new Paragraph("_".PadRight(80, '_'))
            {
                Alignment = Element.ALIGN_CENTER,
                Font = FontFactory.GetFont("Arial", 8)
            };
            document.Add(linha);

            var tabelaRodape = new PdfPTable(2) { WidthPercentage = 100 };
            tabelaRodape.SetWidths(s_widths);

            var fonteRodape = FontFactory.GetFont("Arial", 8);
            var dataGeracao = new Paragraph($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}", fonteRodape);
            var celulaData = new PdfPCell(dataGeracao)
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            tabelaRodape.AddCell(celulaData);

            if (_config.IncluirNumeroPagina)
            {
                var numeroPagina = new Paragraph("Página 1", fonteRodape);
                var celulaPagina = new PdfPCell(numeroPagina)
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                tabelaRodape.AddCell(celulaPagina);
            }
            else
            {
                tabelaRodape.AddCell(new PdfPCell(new Phrase(""))
                {
                    Border = Rectangle.NO_BORDER
                });
            }

            document.Add(tabelaRodape);
        }
    }
}
