using Microsoft.AspNetCore.Mvc;
using EM.Montador.PDF;
using EM.Montador.PDF.Components;
using EM.Domain.Models;
using EM.Repository;
using EM.Domain.Utilitarios;

namespace EM.Web.Controllers
{
    public class RelatoriosController : Controller
    {
        private readonly IServicePDF _pdfService;
        private readonly AlunoRepository _alunoRepo;
        private readonly CidadeRepository _cidadeRepo;

        public RelatoriosController(IServicePDF pdfService)
        {
            _pdfService = pdfService;
            _alunoRepo = new AlunoRepository();
            _cidadeRepo = new CidadeRepository();
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Central de Relatórios";
            return View();
        }

        // Actions para cada tipo de relatório específico
        public IActionResult RelatorioGeralAlunos()
        {
            var model = new RelatorioFiltroModel
            {
                TipoRelatorio = "RelatorioGeralAlunos",
                TituloRelatorio = "Relatório Geral de Alunos"
            };

            CarregarDadosFormulario(model);
            return View(model);
        }

        public IActionResult RelatorioCidades()
        {
            var model = new RelatorioFiltroModel
            {
                TipoRelatorio = "RelatorioCidades",
                TituloRelatorio = "Lista de Cidades"
            };

            CarregarDadosFormulario(model);
            return View(model);
        }

        public IActionResult CertificadoIndividual()
        {
            var model = new RelatorioFiltroModel
            {
                TipoRelatorio = "CertificadoIndividual",
                TituloRelatorio = "Certificado Individual"
            };

            CarregarDadosFormulario(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GerarRelatorio(RelatorioFiltroModel filtros)
        {
            if (string.IsNullOrEmpty(filtros.TipoRelatorio))
                return RedirectToAction(nameof(Index));

            try
            {
                byte[] pdfBytes = filtros.TipoRelatorio switch
                {
                    "RelatorioGeralAlunos" => GerarRelatorioGeralAlunos(filtros),
                    "RelatorioCidades" => GerarRelatorioCidades(filtros),
                    _ => throw new ArgumentException("Tipo de relatório inválido")
                };

                var nomeArquivo = $"{filtros.TipoRelatorio}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                return File(pdfBytes, "application/pdf", nomeArquivo);
            }
            catch (Exception ex)
            {
                TempData["Erro"] = $"Erro ao gerar relatório: {ex.Message}";
                CarregarDadosFormulario(filtros);
                
                // Redireciona para a view correta baseada no tipo
                return filtros.TipoRelatorio switch
                {
                    "RelatorioGeralAlunos" => View("RelatorioGeralAlunos", filtros),
                    "RelatorioCidades" => View("RelatorioCidades", filtros),
                    "CertificadoIndividual" => View("CertificadoIndividual", filtros),
                    _ => RedirectToAction(nameof(Index))
                };
            }
        }

        private byte[] GerarRelatorioGeralAlunos(RelatorioFiltroModel filtros)
        {
            var alunos = _alunoRepo.BuscarAlunos(filtros.FiltroNome);
            alunos = AplicarFiltrosAlunos(alunos, filtros);
            return _pdfService.GerarRelatorioAlunos(alunos);
        }

        private byte[] GerarRelatorioCidades(RelatorioFiltroModel filtros)
        {
            var cidades = _cidadeRepo.BuscarCidades(filtros.FiltroNomeCidade);
            
            if (!string.IsNullOrEmpty(filtros.FiltroUF))
                cidades = cidades.Where(c => c.CIDAUF == filtros.FiltroUF).ToList();

            if (!string.IsNullOrEmpty(filtros.FiltroCodigoIBGE))
                cidades = cidades.Where(c => c.CIDACODIGOIBGE.Contains(filtros.FiltroCodigoIBGE)).ToList();

            var config = CriarConfiguracao(filtros);
            return _pdfService.GerarDocumentoPersonalizado(config, new TabelaCidadesComponent(cidades));
        }


        private List<Aluno> AplicarFiltrosAlunos(List<Aluno> alunos, RelatorioFiltroModel filtros)
        {
            var query = alunos.AsQueryable();

            if (!string.IsNullOrEmpty(filtros.FiltroSexo))
                query = query.Where(a => a.AlunoSexo == filtros.FiltroSexo);

            if (filtros.FiltroCidadeId.HasValue)
                query = query.Where(a => a.AlunoCidaCodigo == filtros.FiltroCidadeId.Value);

            if (filtros.FiltroDataNascimentoDe.HasValue)
                query = query.Where(a => a.AlunoNascimento >= filtros.FiltroDataNascimentoDe.Value);

            if (filtros.FiltroDataNascimentoAte.HasValue)
                query = query.Where(a => a.AlunoNascimento <= filtros.FiltroDataNascimentoAte.Value);

            if (filtros.FiltroIdadeMinima.HasValue || filtros.FiltroIdadeMaxima.HasValue)
            {
                var hoje = DateTime.Today;
                var alunosComIdade = query.Select(a => new 
                { 
                    Aluno = a, 
                    Idade = hoje.Year - a.AlunoNascimento.Year - 
                           (hoje.DayOfYear < a.AlunoNascimento.DayOfYear ? 1 : 0) 
                });

                if (filtros.FiltroIdadeMinima.HasValue)
                    alunosComIdade = alunosComIdade.Where(x => x.Idade >= filtros.FiltroIdadeMinima.Value);

                if (filtros.FiltroIdadeMaxima.HasValue)
                    alunosComIdade = alunosComIdade.Where(x => x.Idade <= filtros.FiltroIdadeMaxima.Value);

                query = alunosComIdade.Select(x => x.Aluno).AsQueryable();
            }

            return query.ToList();
        }

        private void CarregarDadosFormulario(RelatorioFiltroModel model)
        {
            model.ListaCidades = _cidadeRepo.ListarTodas();

            model.ListaUFs = model.ListaCidades.Select(c => c.CIDAUF).Distinct().OrderBy(u => u).ToList();
        }

        private EM.Montador.PDF.Models.ConfigModelPDF CriarConfiguracao(RelatorioFiltroModel filtros)
        {
            return new EM.Montador.PDF.Models.ConfigModelPDF
            {
                Titulo = filtros.TituloRelatorio,
                IncluirCabecalho = filtros.IncluirCabecalho,
                IncluirRodape = filtros.IncluirRodape,
                IncluirNumeroPagina = filtros.IncluirNumeroPagina,
                Paisagem = filtros.OrientacaoPaisagem
            };
        }
    }
}
