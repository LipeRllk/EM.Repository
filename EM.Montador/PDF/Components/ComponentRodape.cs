using iTextSharp.text;
using iTextSharp.text.pdf;
using EM.Montador.PDF.Models;
using EM.Montador.PDF.Components;

namespace EM.Montador.PDF.Components
{
    internal class ComponentRodape
    {
        public class RodapeComponent : IComponentPDF
        {
            private readonly ConfigModelPDF _config;

            public RodapeComponent(ConfigModelPDF config)
            {
                _config = config;
            }

            public void AdicionarAoDocumento(Document document)
            {
                if (!_config.IncluirRodape) return;

                // Espaço antes do rodapé
                document.Add(new Paragraph(" ") { SpacingBefore = 30f });

                // Linha separadora
                var linha = new Paragraph("_".PadRight(80, '_'))
                {
                    Alignment = Element.ALIGN_CENTER,
                    Font = FontFactory.GetFont("Arial", 8)
                };
                document.Add(linha);

                // Tabela do rodapé
                var tabelaRodape = new PdfPTable(2) { WidthPercentage = 100 };
                tabelaRodape.SetWidths(new float[] { 1f, 1f });

                // Data e hora de geração
                var fonteRodape = FontFactory.GetFont("Arial", 8);
                var dataGeracao = new Paragraph($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}", fonteRodape);
                var celulaData = new PdfPCell(dataGeracao)
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                tabelaRodape.AddCell(celulaData);

                // Número da página (se habilitado)
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
}
