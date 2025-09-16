using iTextSharp.text;
using iTextSharp.text.pdf;
using EM.Domain.Models;

namespace EM.Montador.PDF.Components
{
    public class TabelaAlunosComponent(IEnumerable<Aluno> alunos) : IComponentPDF
    {
        private readonly IEnumerable<Aluno> _alunos = alunos;
        private static readonly float[] s_widths = [1f, 3f, 2f, 2f];
        private static readonly string[] s_cabecalhos = ["Matrícula", "Nome", "CPF", "Nascimento"];

        public void AdicionarAoDocumento(Document document)
        {
            if (!_alunos.Any()) return;

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
            foreach (var aluno in _alunos)
            {
                tabela.AddCell(new PdfPCell(new Phrase(aluno.AlunoMatricula.ToString(), fonteConteudo))
                { Padding = 3f });
                tabela.AddCell(new PdfPCell(new Phrase(aluno.AlunoNome, fonteConteudo))
                { Padding = 3f });
                tabela.AddCell(new PdfPCell(new Phrase(aluno.AlunoCPF, fonteConteudo))
                { Padding = 3f });
                tabela.AddCell(new PdfPCell(new Phrase(aluno.AlunoNascimento.ToString("dd/MM/yyyy"), fonteConteudo))
                { Padding = 3f });
            }

            document.Add(tabela);
        }
    }
}
