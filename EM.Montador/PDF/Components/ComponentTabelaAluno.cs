using iTextSharp.text;
using iTextSharp.text.pdf;
using EM.Domain.Models;

namespace EM.Montador.PDF.Components
{
    public class TabelaAlunosComponent : IComponentPDF
    {
        private readonly IEnumerable<Aluno> _alunos;

        public TabelaAlunosComponent(IEnumerable<Aluno> alunos)
        {
            _alunos = alunos;
        }

        public void AdicionarAoDocumento(Document document)
        {
            if (!_alunos.Any()) return;

            // Criar tabela com 4 colunas
            var tabela = new PdfPTable(4) { WidthPercentage = 100 };
            tabela.SetWidths(new float[] { 1f, 3f, 2f, 2f });

            // Cabeçalho
            var fonteCabecalho = FontFactory.GetFont("Arial", 10, Font.BOLD);
            var cabecalhos = new[] { "Matrícula", "Nome", "CPF", "Nascimento" };

            foreach (var cabecalho in cabecalhos)
            {
                var celula = new PdfPCell(new Phrase(cabecalho, fonteCabecalho))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5f
                };
                tabela.AddCell(celula);
            }

            // Dados
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
