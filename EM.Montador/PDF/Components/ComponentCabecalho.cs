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

            if (_config.Logo != null)
            {
                var imagem = Image.GetInstance(_config.Logo);
                imagem.ScaleToFit(60f, 60f);
                tabelaCabecalho.AddCell(imagem);
            }
            else
            {
                tabelaCabecalho.AddCell("");
            }

            var info = $"{_config.NomeColegio}\n{_config.Endereco}\nEmissão: {DateTime.Now:dd/MM/yyyy HH:mm}";
            var cellInfo = new PdfPCell(new Phrase(info, FontFactory.GetFont("Arial", 10)))
            {
                Border = Rectangle.NO_BORDER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            tabelaCabecalho.AddCell(cellInfo);

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