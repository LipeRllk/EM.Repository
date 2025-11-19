using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EM.Montador.PDF;
using EM.Montador.PDF.Components;
using EM.Domain.Models;
using EM.Repository;

namespace EM.Web.Controllers
{
    public class RelatoriosController(
        IPDFService pdfService,
        AlunoRepository alunoRepository,
        CidadeRepository cidadeRepository,
        RelatorioConfigRepository relatorioConfigRepository,
        ILogger<RelatoriosController> logger) : Controller
    {
        private readonly IPDFService _pdfService = pdfService;
        private readonly AlunoRepository _alunoRepo = alunoRepository;
        private readonly CidadeRepository _cidadeRepo = cidadeRepository;
        private readonly RelatorioConfigRepository _configRepo = relatorioConfigRepository;
        private readonly ILogger<RelatoriosController> _logger = logger;

        public IActionResult Index()
        {
            ViewData["Title"] = "Central de Relatórios";
            return View();
        }

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

        [HttpGet]
        public IActionResult ConfiguracaoCabecalho()
        {
            _logger.LogInformation("GET ConfiguracaoCabecalho chamado.");
            var salvo = _configRepo.BuscarPorId(1);

            var model = new EM.Montador.PDF.Models.ConfigModelPDF();
            if (salvo != null)
            {
                model.NomeColegio = salvo.NomeColegio;
                model.Endereco = salvo.Endereco;
                model.Logo = salvo.Logo;
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult LogoAtual()
        {
            var salvo = _configRepo.BuscarPorId(1);
            if (salvo?.Logo == null || salvo.Logo.Length == 0)
                return NotFound();

            var mime = ObterMimeTypeImagem(salvo.Logo);
            return File(salvo.Logo, mime);
        }

        private static string ObterMimeTypeImagem(byte[] bytes)
        {
            if (bytes.Length >= 12)
            {
                if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47) return "image/png";
                if (bytes[0] == 0xFF && bytes[1] == 0xD8) return "image/jpeg";
                if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46) return "image/gif";
                if (bytes[0] == 0x42 && bytes[1] == 0x4D) return "image/bmp";
                if (bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 &&
                    bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50) return "image/webp";
            }
            return "image/png";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfiguracaoCabecalho(EM.Montador.PDF.Models.ConfigModelPDF config, IFormFile? LogoFile)
        {
            _logger.LogInformation("POST ConfiguracaoCabecalho chamado. Arquivo recebido: {HasFile}", LogoFile is { Length: > 0 });

            if (!ModelState.IsValid)
                return View(config);

            var salvo = _configRepo.BuscarPorId(1);
            byte[]? logoFinal = config.Logo;

            if (LogoFile is { Length: > 0 })
            {
                if (!LogoFile.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(nameof(LogoFile), "O arquivo de logo deve ser uma imagem.");
                    return View(config);
                }

                using var memoryStream = new MemoryStream();
                LogoFile.CopyTo(memoryStream);
                logoFinal = memoryStream.ToArray();
            }
            else if (logoFinal == null && salvo?.Logo != null)
            {
                logoFinal = salvo.Logo;
            }

            _configRepo.Upsert(new RelatorioConfig
            {
                Id = 1,
                NomeColegio = config.NomeColegio ?? string.Empty,
                Endereco = config.Endereco ?? string.Empty,
                Logo = logoFinal
            });

            TempData["Sucesso"] = "Configuração salva com sucesso!";
            return RedirectToAction(nameof(ConfiguracaoCabecalho));
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

                return filtros.TipoRelatorio switch
                {
                    "RelatorioGeralAlunos" => View("RelatorioGeralAlunos", filtros),
                    "RelatorioCidades" => View("RelatorioCidades", filtros),
                    _ => RedirectToAction(nameof(Index))
                };
            }
        }

        private byte[] GerarRelatorioGeralAlunos(RelatorioFiltroModel filtros)
        {
            var alunos = _alunoRepo.BuscarAlunos(filtros.FiltroNome);
            alunos = AplicarFiltrosAlunos(alunos, filtros);

            return _pdfService.GerarDocumentoPersonalizado(
                CriarConfiguracao(filtros),
                [new TabelaAlunosComponent(alunos)]
            );
        }

        private byte[] GerarRelatorioCidades(RelatorioFiltroModel filtros)
        {
            var cidades = _cidadeRepo.BuscarCidades(filtros.FiltroNomeCidade);

            if (!string.IsNullOrEmpty(filtros.FiltroUF))
                cidades = [.. cidades.Where(c => c.Uf == filtros.FiltroUF)];

            if (!string.IsNullOrEmpty(filtros.FiltroCodigoIBGE))
                cidades = [.. cidades.Where(c => c.Ibge.Contains(filtros.FiltroCodigoIBGE))];

            var config = CriarConfiguracao(filtros);

            return _pdfService.GerarDocumentoPersonalizado(config, [new TabelaCidadesComponent(cidades)]);
        }

        private static List<Aluno> AplicarFiltrosAlunos(List<Aluno> alunos, RelatorioFiltroModel filtros)
        {
            var query = alunos.AsQueryable();

            if (!string.IsNullOrEmpty(filtros.FiltroSexo))
                query = query.Where(a => a.Sexo == filtros.FiltroSexo);

            if (filtros.FiltroCidadeId.HasValue)
                query = query.Where(a => a.AlunoCidaCodigo == filtros.FiltroCidadeId.Value);

            if (filtros.FiltroDataNascimentoDe.HasValue)
                query = query.Where(a => a.DataNascimento >= filtros.FiltroDataNascimentoDe.Value);

            if (filtros.FiltroDataNascimentoAte.HasValue)
                query = query.Where(a => a.DataNascimento <= filtros.FiltroDataNascimentoAte.Value);

            if (filtros.FiltroIdadeMinima.HasValue || filtros.FiltroIdadeMaxima.HasValue)
            {
                var hoje = DateTime.Today;
                var alunosComIdade = query.Select(a => new
                {
                    Aluno = a,
                    Idade = hoje.Year - a.DataNascimento.Year -
                           (hoje.DayOfYear < a.DataNascimento.DayOfYear ? 1 : 0)
                });

                if (filtros.FiltroIdadeMinima.HasValue)
                    alunosComIdade = alunosComIdade.Where(x => x.Idade >= filtros.FiltroIdadeMinima.Value);

                if (filtros.FiltroIdadeMaxima.HasValue)
                    alunosComIdade = alunosComIdade.Where(x => x.Idade <= filtros.FiltroIdadeMaxima.Value);

                query = alunosComIdade.Select(x => x.Aluno).AsQueryable();
            }

            return [.. query];
        }

        private void CarregarDadosFormulario(RelatorioFiltroModel model)
        {
            model.ListaCidades = _cidadeRepo.ListarTodas();
            model.ListaUFs = [.. model.ListaCidades.Select(c => c.Uf).Distinct().OrderBy(u => u)];
        }

        private EM.Montador.PDF.Models.ConfigModelPDF CriarConfiguracao(RelatorioFiltroModel filtros)
        {
            var cfg = new EM.Montador.PDF.Models.ConfigModelPDF
            {
                Titulo = filtros.TituloRelatorio,
                IncluirCabecalho = filtros.IncluirCabecalho,
                IncluirRodape = filtros.IncluirRodape,
                IncluirNumeroPagina = filtros.IncluirNumeroPagina,
                Paisagem = filtros.OrientacaoPaisagem
            };

            try
            {
                var salvo = _configRepo.BuscarPorId(1);
                if (salvo != null)
                {
                    cfg.NomeColegio = salvo.NomeColegio;
                    cfg.Endereco = salvo.Endereco;
                    cfg.Logo = salvo.Logo;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar configuração do cabeçalho.");
                TempData["Erro"] = "Não foi possível carregar as configurações do cabeçalho. Verifique os dados salvos.";
            }

            return cfg;
        }
    }
}