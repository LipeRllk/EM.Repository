using iTextSharp.text;

namespace EM.Montador.PDF.Components
{
    public class TituloComponent : IComponentPDF
    {
        private readonly string _titulo;
        private readonly string _subtitulo;

        public TituloComponent(string titulo, string subtitulo = "")
        {
            _titulo = titulo;
            _subtitulo = subtitulo;
        }

        public void AdicionarAoDocumento(Document document)
        {
            if (string.IsNullOrEmpty(_titulo)) return;

            var fonteTitulo = FontFactory.GetFont("Arial", 18, Font.BOLD);
            var paragraphTitulo = new Paragraph(_titulo, fonteTitulo)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = string.IsNullOrEmpty(_subtitulo) ? 20f : 10f
            };
            document.Add(paragraphTitulo);

            if (!string.IsNullOrEmpty(_subtitulo))
            {
                var fonteSubtitulo = FontFactory.GetFont("Arial", 14);
                var paragraphSubtitulo = new Paragraph(_subtitulo, fonteSubtitulo)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                };
                document.Add(paragraphSubtitulo);
            }
        }
    }
}
