using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Montador.PDF.Components
{
    public interface IComponentPDF
    {
        void AdicionarAoDocumento(Document document);
    }
}
