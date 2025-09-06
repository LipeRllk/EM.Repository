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

        public IActionResult SelecionarRelatorio(string tipo)
        {
            if (string.IsNullOrEmpty(tipo))
                return RedirectToAction(nameof(Index));

            var model = new RelatorioFiltroModel
            {
                TipoRelatorio = tipo,
                TituloRelatorio = ObterTituloRelatorio(tipo)
            };

            CarregarDadosFormulario(model);
            ViewData["Title"] = $"Filtros - {model.TituloRelatorio}";
            
            return View("FiltrosRelatorio", model);
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
                    "RelatorioAlunosPorCidade" => GerarRelatorioAlunosPorCidade(filtros),
                    "RelatorioAlunosPorIdade" => GerarRelatorioAlunosPorIdade(filtros),
                    "RelatorioCidades" => GerarRelatorioCidades(filtros),
                    "RelatorioCidadesPorUF" => GerarRelatorioCidadesPorUF(filtros),
                    "CertificadoIndividual" => GerarCertificadoIndividual(filtros),
                    _ => throw new ArgumentException("Tipo de relatório inválido")
                };

                var nomeArquivo = $"{filtros.TipoRelatorio}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                return File(pdfBytes, "application/pdf", nomeArquivo);
            }
            catch (Exception ex)
            {
                TempData["Erro"] = $"Erro ao gerar relatório: {ex.Message}";
                CarregarDadosFormulario(filtros);
                return View("FiltrosRelatorio", filtros);
            }
        }

        private byte[] GerarRelatorioGeralAlunos(RelatorioFiltroModel filtros)
        {
            var alunos = _alunoRepo.BuscarAlunos(filtros.FiltroNome);
            alunos = AplicarFiltrosAlunos(alunos, filtros);
            return _pdfService.GerarRelatorioAlunos(alunos);
        }

        private byte[] GerarRelatorioAlunosPorCidade(RelatorioFiltroModel filtros)
        {
            var alunos = _alunoRepo.BuscarAlunos();
            
            if (filtros.FiltroCidadeId.HasValue)
                alunos = alunos.Where(a => a.AlunoCidaCodigo == filtros.FiltroCidadeId.Value).ToList();
                
            alunos = AplicarFiltrosAlunos(alunos, filtros);
            return _pdfService.GerarRelatorioAlunos(alunos);
        }

        private byte[] GerarRelatorioAlunosPorIdade(RelatorioFiltroModel filtros)
        {
            var alunos = _alunoRepo.BuscarAlunos();
            alunos = AplicarFiltrosAlunos(alunos, filtros);
            return _pdfService.GerarRelatorioAlunos(alunos);
        }

        private byte[] GerarRelatorioCidades(RelatorioFiltroModel filtros)
        {
            var cidades = _cidadeRepo.BuscarCidades(filtros.FiltroNomeCidade);
            
            if (!string.IsNullOrEmpty(filtros.FiltroUF))
                cidades = cidades.Where(c => c.CIDAUF == filtros.FiltroUF).ToList();

            var config = CriarConfiguracao(filtros);
            return _pdfService.GerarDocumentoPersonalizado(config, new TabelaCidadesComponent(cidades));
        }

        private byte[] GerarRelatorioCidadesPorUF(RelatorioFiltroModel filtros)
        {
            var cidades = _cidadeRepo.BuscarCidades();
            
            if (!string.IsNullOrEmpty(filtros.FiltroUF))
                cidades = cidades.Where(c => c.CIDAUF == filtros.FiltroUF).ToList();

            var config = CriarConfiguracao(filtros);
            return _pdfService.GerarDocumentoPersonalizado(config, new TabelaCidadesComponent(cidades));
        }

        private byte[] GerarCertificadoIndividual(RelatorioFiltroModel filtros)
        {
            if (!filtros.FiltroCidadeId.HasValue)
                throw new ArgumentException("Selecione um aluno para gerar o certificado");

            var aluno = _alunoRepo.BuscarPorMatriculaTradicional(filtros.FiltroCidadeId.Value);
            if (aluno == null)
                throw new ArgumentException("Aluno não encontrado");

            return _pdfService.GerarCertificado(aluno.AlunoNome, "Curso de Programação", DateTime.Now);
        }

        private List<Aluno> AplicarFiltrosAlunos(List<Aluno> alunos, RelatorioFiltroModel filtros)
        {
            var query = alunos.AsQueryable();

            if (!string.IsNullOrEmpty(filtros.FiltroSexo))
                query = query.Where(a => a.AlunoSexo == filtros.FiltroSexo);

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

        private string ObterTituloRelatorio(string tipo)
        {
            return tipo switch
            {
                "RelatorioGeralAlunos" => "Relatório Geral de Alunos",
                "RelatorioAlunosPorCidade" => "Relatório de Alunos por Cidade",
                "RelatorioAlunosPorIdade" => "Relatório de Alunos por Faixa Etária",
                "RelatorioCidades" => "Lista de Cidades",
                "RelatorioCidadesPorUF" => "Relatório de Cidades por UF",
                "CertificadoIndividual" => "Certificado Individual",
                _ => "Relatório"
            };
        }
    }
}
