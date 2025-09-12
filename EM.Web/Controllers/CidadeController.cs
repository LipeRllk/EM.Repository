using EM.Domain.Models;
using EM.Repository; // Certifique-se de ter o repositório implementado
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EM.Web.Controllers
{
    public class CidadeController : Controller
    {
        private readonly CidadeRepository _repo;
        private readonly AlunoRepository _alunoRepo;

        public CidadeController(CidadeRepository cidadeRepository, AlunoRepository alunoRepository)
        {
            _repo = cidadeRepository;
            _alunoRepo = alunoRepository;
        }

        public IActionResult CidadeList(string search)
        {
            var cidades = _repo.BuscarCidades(search);
            ViewBag.Search = search;
            return View(cidades);
        }

        public IActionResult CidadeCreate()
        {
            ViewData["Action"] = "CidadeCreate";
            return View(new Cidade());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CidadeCreate(Cidade cidade)
        {
            if (ModelState.IsValid)
            {
                _repo.Inserir(cidade);
                return RedirectToAction(nameof(CidadeList));
            }
            ViewData["Action"] = "CidadeCreate";
            return View(cidade);
        }

        public IActionResult CidadeEdit(int? id)
        {
            if (id == null) return NotFound();

            var cidade = _repo.BuscarPorId(id.Value);
            if (cidade == null) return NotFound();

            ViewData["Action"] = "CidadeEdit";
            return View(cidade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CidadeEdit(int id, Cidade cidade)
        {
            if (id != cidade.CIDACODIGO)
                return NotFound();

            if (ModelState.IsValid)
            {
                _repo.Atualizar(cidade);
                return RedirectToAction(nameof(CidadeList));
            }
            ViewData["Action"] = "CidadeEdit";
            return View(cidade);
        }

        public IActionResult CidadeDelete(int? id)
        {
            if (id == null) return NotFound();

            var cidade = _repo.BuscarPorId(id.Value);
            if (cidade == null) return NotFound();

            // Verificar se existem alunos vinculados a esta cidade
            var quantidadeAlunos = _alunoRepo.ContarPorCidadeTradicional(id.Value);
            ViewBag.TemAlunos = quantidadeAlunos > 0;
            ViewBag.QuantidadeAlunos = quantidadeAlunos;

            return View(cidade);
        }

        [HttpPost, ActionName("CidadeDelete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int CIDACODIGO)
        {
            var cidade = _repo.BuscarPorId(CIDACODIGO);
            if (cidade != null)
            {
                var alunosVinculados = _alunoRepo.ContarPorCidadeTradicional(CIDACODIGO);

                if (alunosVinculados > 0)
                {
                    ViewBag.ErroExclusao = $"Não é possível excluir esta cidade pois existem {alunosVinculados} aluno(s) cadastrado(s) nela.";
                    ViewBag.TemAlunos = true;
                    ViewBag.QuantidadeAlunos = alunosVinculados;
                    return View("CidadeDelete", cidade);
                }

                try
                {
                    _repo.Excluir(CIDACODIGO);
                }
                catch (Exception)
                {
                    ViewBag.ErroExclusao = "Erro ao excluir cidade: Esta cidade está sendo referenciada por outros registros no sistema.";
                    ViewBag.TemAlunos = true;
                    return View("CidadeDelete", cidade);
                }
            }
            return RedirectToAction(nameof(CidadeList));
        }

        // API endpoint para buscar cidade por ID
        [HttpGet]
        public JsonResult BuscarCidadePorId(int id)
        {
            var cidade = _repo.BuscarPorId(id);
            if (cidade == null)
            {
                return Json(new { erro = "Cidade não encontrada" });
            }
            
            return Json(new { 
                cidacodigo = cidade.CIDACODIGO,
                cidadescricao = cidade.CIDADESCRICAO,
                cidauf = cidade.CIDAUF,
                cidacodigoibge = cidade.CIDACODIGOIBGE
            });
        }

        // Método para compatibilidade com scripts existentes
        [HttpGet]
        public JsonResult BuscarCidadePorCodigo(int codigo)
        {
            return BuscarCidadePorId(codigo);
        }
    }
}