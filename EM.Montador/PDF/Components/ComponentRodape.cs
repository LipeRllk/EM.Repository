using System;
using iTextSharp.text;
using iTextSharp.text.pdf;
using EM.Montador.PDF.Models;

namespace EM.Montador.PDF.Components
{
    public class RodapeComponent(ConfigModelPDF config) : IComponentPDF
    {
        private readonly ConfigModelPDF _config = config;
        private static readonly float[] s_widths = [1f, 1f];
        private bool _usarEvento = false;

        public void RegistrarRodapeFixo(PdfWriter writer)
        {
            _usarEvento = true;
            writer.PageEvent = new RodapePageEvent(_config);
        }

        public void ReservarEspacoInferior(Document document, float alturaMinima = 50f)
        {
            if (document.IsOpen()) return;
            document.SetMargins(document.LeftMargin, document.RightMargin, document.TopMargin, Math.Max(_config.MargemInferior, alturaMinima));
        }

        public void AdicionarAoDocumento(Document document)
        {
            if (!_config.IncluirRodape) return;
            if (_usarEvento) return;

            document.Add(new Paragraph(" ") { SpacingBefore = 30f });

            var linha = new Paragraph("_".PadRight(83, '_'))
            {
                Alignment = Element.ALIGN_CENTER,
                Font = FontFactory.GetFont("Arial", 8,3)
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

        private sealed class RodapePageEvent : PdfPageEventHelper
        {
            private readonly ConfigModelPDF _config;
            private readonly float[] _widths = [1f, 1f];
            private readonly Font _font = FontFactory.GetFont("Arial", 8, Font.NORMAL);

            public RodapePageEvent(ConfigModelPDF config) => _config = config;

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                if (!_config.IncluirRodape) return;

                var page = document.PageSize;
                float leftX = page.GetLeft(document.LeftMargin);
                float rightX = page.GetRight(document.RightMargin);

                var table = new PdfPTable(2) { TotalWidth = rightX - leftX };
                table.SetWidths(_widths);

                var data = new Paragraph($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}", _font);
                table.AddCell(new PdfPCell(data) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT });

                var paginaTxt = _config.IncluirNumeroPagina ? $"Página {writer.PageNumber}" : string.Empty;
                table.AddCell(new PdfPCell(new Paragraph(paginaTxt, _font)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT });

                table.CalculateHeights();

                float footerTopY = page.GetBottom(document.BottomMargin) + table.TotalHeight;
                table.WriteSelectedRows(0, -1, leftX, footerTopY, writer.DirectContent);

                var cb = writer.DirectContent;
                cb.MoveTo(leftX, footerTopY + 2f);
                cb.LineTo(rightX, footerTopY + 2f);
                cb.Stroke();
            }
        }
    }
}
