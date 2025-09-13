using EM.Montador.PDF.Components;
using EM.Montador.PDF.Models;
using EM.Domain.Models;

namespace EM.Montador.PDF
{
    public interface IServicePDF
    {
        byte[] GerarRelatorioAlunos(IEnumerable<Aluno> alunos);
        byte[] GerarDocumentoPersonalizado(ConfigModelPDF config, params IComponentPDF[] componentes);
    }
}