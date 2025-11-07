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
            tabelaCabecalho.SpacingBefore = 0f;

            if (_config.Logo != null)
            {
                var imagem = Image.GetInstance(_config.Logo);
                imagem.ScaleToFit(120f, 60f);

                var cellLogo = new PdfPCell(imagem, true)
                {
                    Border = Rectangle.NO_BORDER,
                    FixedHeight = 80f,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Padding = 0
                };
                tabelaCabecalho.AddCell(cellLogo);
            }
            else
            {
                tabelaCabecalho.AddCell("");
            }

            var fonteNome = FontFactory.GetFont("Helvetica", 14f, Font.BOLD);
            var fonteEndereco = FontFactory.GetFont("Arial", 10f, Font.NORMAL);

            var phraseInfo = new Phrase
            {
                new Chunk(_config.NomeColegio ?? string.Empty, fonteNome),
                Chunk.NEWLINE,
                new Chunk(_config.Endereco ?? string.Empty, fonteEndereco)
            };
            phraseInfo.SetLeading(0f, 1.0f);

            var cellInfo = new PdfPCell(phraseInfo)
            {
                Border = Rectangle.NO_BORDER,
                VerticalAlignment = Element.ALIGN_TOP,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                PaddingTop = 0f,
                PaddingBottom = 0f,
                PaddingLeft = 8f,
                MinimumHeight = 80f
            };
            tabelaCabecalho.AddCell(cellInfo);

            if (!string.IsNullOrWhiteSpace(_config.Titulo))
            {
                var fonteTitulo = FontFactory.GetFont("Helvetica", 16f, Font.BOLD);
                var tituloPhrase = new Phrase(_config.Titulo, fonteTitulo);
                tituloPhrase.SetLeading(0f, 1.0f);

                var tituloCell = new PdfPCell(tituloPhrase)
                {
                    Border = Rectangle.NO_BORDER,
                    Colspan = 2,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_TOP,
                    PaddingTop = 4f,
                    PaddingBottom = 4f
                };
                tabelaCabecalho.AddCell(tituloCell);
            }

            document.Add(tabelaCabecalho);
            AdicionarLinhaSeparadora(document);
        }

        private static void AdicionarLinhaSeparadora(Document document)
        {
            var linha = new Paragraph("_".PadRight(83, '_'))
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20f,
                Font = FontFactory.GetFont("Arial", 8,5)
            };
            document.Add(linha);
        }
    }
}