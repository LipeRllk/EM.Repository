using iTextSharp.text;
using iTextSharp.text.pdf;
using EM.Domain.Models;

namespace EM.Montador.PDF.Components
{
    public class TabelaCidadesComponent(IEnumerable<Cidade> cidades) : IComponentPDF
    {
        private readonly IEnumerable<Cidade> _cidades = cidades;

        private static readonly float[] s_widths = [1f, 3f, 1f, 2f];
        private static readonly string[] s_cabecalhos = ["Código", "Cidade", "UF", "Código IBGE"];

        public void AdicionarAoDocumento(Document document)
        {
            if (!_cidades.Any()) return;

            var tabela = new PdfPTable(4) { WidthPercentage = 100 };
            tabela.SetWidths(s_widths);

            var fonteCabecalho = FontFactory.GetFont("Arial", 10, Font.BOLD);

            foreach (var cabecalho in s_cabecalhos)
            {
                var celula = new PdfPCell(new Phrase(cabecalho, fonteCabecalho))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5f
                };
                tabela.AddCell(celula);
            }

            var fonteConteudo = FontFactory.GetFont("Arial", 9);
            foreach (var cidade in _cidades)
            {
                tabela.AddCell(new PdfPCell(new Phrase(cidade.CIDACODIGO.ToString(), fonteConteudo))
                { Padding = 3f, HorizontalAlignment = Element.ALIGN_CENTER });
                
                tabela.AddCell(new PdfPCell(new Phrase(cidade.CIDADESCRICAO, fonteConteudo))
                { Padding = 3f });
                
                tabela.AddCell(new PdfPCell(new Phrase(cidade.CIDAUF, fonteConteudo))
                { Padding = 3f, HorizontalAlignment = Element.ALIGN_CENTER });
                
                tabela.AddCell(new PdfPCell(new Phrase(cidade.CIDACODIGOIBGE, fonteConteudo))
                { Padding = 3f, HorizontalAlignment = Element.ALIGN_CENTER });
            }

            document.Add(tabela);
        }
    }
}