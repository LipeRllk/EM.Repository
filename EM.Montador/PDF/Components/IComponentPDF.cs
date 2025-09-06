using iTextSharp.text;

namespace EM.Montador.PDF.Components
{
    public interface IComponentPDF
    {
        void AdicionarAoDocumento(Document document);
    }
}
