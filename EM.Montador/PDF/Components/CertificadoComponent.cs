using iTextSharp.text;
using iTextSharp.text.pdf;

namespace EM.Montador.PDF.Components
{
    public class CertificadoComponent : IComponentPDF
    {
        private readonly string _nomeAluno;
        private readonly string _curso;
        private readonly DateTime _dataConclusao;

        public CertificadoComponent(string nomeAluno, string curso, DateTime dataConclusao)
        {
            _nomeAluno = nomeAluno;
            _curso = curso;
            _dataConclusao = dataConclusao;
        }

        public void AdicionarAoDocumento(Document document)
        {
            // Espaçamento inicial
            document.Add(new Paragraph(" ") { SpacingAfter = 50f });

            // Texto de certificação
            var fonteTexto = FontFactory.GetFont("Arial", 14);
            var textoCertificacao = new Paragraph("Certificamos que", fonteTexto)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 30f
            };
            document.Add(textoCertificacao);

            // Nome do aluno
            var fonteNome = FontFactory.GetFont("Arial", 20, Font.BOLD);
            var nomeAluno = new Paragraph(_nomeAluno.ToUpper(), fonteNome)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 30f
            };
            document.Add(nomeAluno);

            // Texto conclusão
            var textoConclusao = new Paragraph($"concluiu com êxito o {_curso}", fonteTexto)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 50f
            };
            document.Add(textoConclusao);

            // Data
            var fonteData = FontFactory.GetFont("Arial", 12);
            var dataTexto = new Paragraph($"Em {_dataConclusao:dd/MM/yyyy}", fonteData)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 80f
            };
            document.Add(dataTexto);

            // Linha para assinatura
            var linhAssinatura = new Paragraph("_".PadRight(50, '_'), fonteData)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 10f
            };
            document.Add(linhAssinatura);

            var textoAssinatura = new Paragraph("Coordenação do Curso", fonteData)
            {
                Alignment = Element.ALIGN_CENTER
            };
            document.Add(textoAssinatura);
        }
    }
}