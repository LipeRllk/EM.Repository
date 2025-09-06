using EM.Montador.PDF.Components;
using EM.Montador.PDF.Models;
using EM.Services.PDF.Components;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EM.Montador.PDF.Components.ComponentRodape;

namespace EM.Montador.PDF
{
    public class PdfDocumentBuilder
    {
        private readonly List<IComponentPDF> _componentes;
        private readonly ConfigModelPDF _config;

        public PdfDocumentBuilder(ConfigModelPDF config = null)
        {
            _config = config ?? new ConfigModelPDF();
            _componentes = new List<IComponentPDF>();
        }

        public PdfDocumentBuilder ComCabecalho()
        {
            _componentes.Add(new CabecalhoComponent(_config));
            return this;
        }

        public PdfDocumentBuilder ComTitulo(string titulo, string subtitulo = "")
        {
            _componentes.Add(new TituloComponent(titulo, subtitulo));
            return this;
        }

        public PdfDocumentBuilder ComConteudo(IComponentPDF conteudo)
        {
            _componentes.Add(conteudo);
            return this;
        }

        public PdfDocumentBuilder ComRodape()
        {
            _componentes.Add(new RodapeComponent(_config));
            return this;
        }

        public byte[] Construir()
        {
            using (var memoryStream = new MemoryStream())
            {
                // Configurar documento
                var tamanho = _config.Paisagem ? _config.TamanhoPagina.Rotate() : _config.TamanhoPagina;
                var document = new Document(tamanho,
                    _config.MargemEsquerda,
                    _config.MargemDireita,
                    _config.MargemSuperior,
                    _config.MargemInferior);

                var writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Adicionar todos os componentes
                foreach (var componente in _componentes)
                {
                    componente.AdicionarAoDocumento(document);
                }

                document.Close();
                return memoryStream.ToArray();
            }
        }
    }
}
