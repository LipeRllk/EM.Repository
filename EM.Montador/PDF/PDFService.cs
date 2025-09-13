using EM.Domain.Models;
using EM.Montador.PDF.Components;
using EM.Montador.PDF.Models;

namespace EM.Montador.PDF
{
    public class PDFService : IPDFService
    {
        public byte[] GerarRelatorioAlunos(IEnumerable<Aluno> alunos)
        {
            var config = new ConfigModelPDF
            {
                Titulo = "Relatório de Alunos",
                IncluirCabecalho = true,
                IncluirRodape = true,
                IncluirNumeroPagina = true
            };

            var builder = new PdfDocumentBuilder(config)
                .ComCabecalho()
                .ComTitulo("Relatório de Alunos", "Lista completa de alunos cadastrados")
                .ComConteudo(new TabelaAlunosComponent(alunos))
                .ComRodape();

            return builder.Construir();
        }

        public byte[] GerarDocumentoPersonalizado(ConfigModelPDF config, params IComponentPDF[] componentes)
        {
            var builder = new PdfDocumentBuilder(config);

            if (config.IncluirCabecalho)
                builder.ComCabecalho();

            foreach (var componente in componentes)
            {
                builder.ComConteudo(componente);
            }

            if (config.IncluirRodape)
                builder.ComRodape();

            return builder.Construir();
        }
    }
}