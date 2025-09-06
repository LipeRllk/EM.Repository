using iTextSharp.text;
using iTextSharp.text.pdf;
using EM.Montador.PDF.Models;
using EM.Montador.PDF.Components;

<<<<<<< HEAD
namespace EM.Montador.PDF.Components
=======
namespace EM.Services.PDF.Components
>>>>>>> -Adicao da biblioteca ItextSharp
{
    public class CabecalhoComponent : IComponentPDF
    {
        private readonly ConfigModelPDF _config;

        public CabecalhoComponent(ConfigModelPDF config)
        {
            _config = config;
        }

        public void AdicionarAoDocumento(Document document)
        {
            if (!_config.IncluirCabecalho) return;

            // Tabela para organizar logo e informações da empresa
            var tabelaCabecalho = new PdfPTable(2) { WidthPercentage = 100 };
            tabelaCabecalho.SetWidths(new float[] { 1f, 3f });

            document.Add(tabelaCabecalho);

            // Linha separadora
            AdicionarLinhaSeparadora(document);
        }

        private void AdicionarLinhaSeparadora(Document document)
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