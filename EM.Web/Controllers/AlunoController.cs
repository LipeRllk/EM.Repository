using EM.Domain.Models;
using EM.Domain.Utilitarios;
using EM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EM.Web.Controllers
{
    public class AlunoController(AlunoRepository alunoRepository, CidadeRepository cidadeRepository) : Controller
    {
        private readonly AlunoRepository _repo = alunoRepository;
        private readonly CidadeRepository _cidadeRepo = cidadeRepository;

        public IActionResult AlunoList(string search)
        {
            var alunos = _repo.BuscarAlunos(search);
            
            var cidades = _cidadeRepo.ListarTodas();
            ViewBag.Cidades = cidades.ToDictionary(c => c.CIDACODIGO, c => c.CIDADESCRICAO);
            ViewBag.Search = search;
            
            return View(alunos);
        }

        public IActionResult AlunoCreate()
        {
            ViewBag.Cidades = new SelectList(_cidadeRepo.ListarTodas(), "CIDACODIGO", "CIDADESCRICAO");
            ViewData["Action"] = "AlunoCreate";
            return View(new Aluno());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AlunoCreate(Aluno aluno)
        {
            if (ModelState.IsValid)
            {
                aluno.AlunoCPF = (aluno.AlunoCPF ?? string.Empty).LimparCPF();
                _repo.Inserir(aluno);
                return RedirectToAction(nameof(AlunoList));
            }
            ViewBag.Cidades = new SelectList(_cidadeRepo.ListarTodas(), "CIDACODIGO", "CIDADESCRICAO");
            ViewData["Action"] = "AlunoCreate";
            return View(aluno);
        }

        public IActionResult AlunoEdit(int? AlunoMatricula)
        {
            if (AlunoMatricula == null) return NotFound();

            var aluno = _repo.BuscarPorMatriculaTradicional(AlunoMatricula.Value);
            if (aluno == null) return NotFound();

            ViewBag.Cidades = new SelectList(_cidadeRepo.ListarTodas(), "CIDACODIGO", "CIDADESCRICAO", aluno.AlunoCidaCodigo);
            ViewData["Action"] = "AlunoEdit";
            return View(aluno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AlunoEdit(int AlunoMatricula, Aluno aluno)
        {
            if (AlunoMatricula != aluno.AlunoMatricula)
                return NotFound();

            if (ModelState.IsValid)
            {
                aluno.AlunoCPF = (aluno.AlunoCPF ?? string.Empty).LimparCPF();
                _repo.Atualizar(aluno);
                return RedirectToAction(nameof(AlunoList));
            }
            
            ViewBag.Cidades = new SelectList(_cidadeRepo.ListarTodas(), "CIDACODIGO", "CIDADESCRICAO", aluno.AlunoCidaCodigo);
            ViewData["Action"] = "AlunoEdit";
            return View(aluno);
        }

        public IActionResult AlunoDelete(int? AlunoMatricula)
        {
            if (AlunoMatricula == null) return NotFound();

            var aluno = _repo.BuscarPorMatriculaTradicional(AlunoMatricula.Value);
            if (aluno == null) return NotFound();

            return View(aluno);
        }

        [HttpPost, ActionName("AlunoDelete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int AlunoMatricula)
        {
            var aluno = _repo.BuscarPorMatriculaTradicional(AlunoMatricula);
            if (aluno != null)
            {
                _repo.Excluir(AlunoMatricula);
            }
            return RedirectToAction(nameof(AlunoList));
        }

        [HttpGet]
        public JsonResult BuscarCidadePorId(int id)
        {
            var cidade = _cidadeRepo.BuscarPorId(id);
            if (cidade == null)
            {
                return Json(new { erro = "Cidade não encontrada" });
            }
            
            return Json(new { 
                cidacodigo = cidade.CIDACODIGO,
                cidadescricao = cidade.CIDADESCRICAO,
                cidauf = cidade.CIDAUF 
            });
        }

        public IActionResult AlunosPorCidade(int cidadeId)
        {
            var alunos = _repo.Get(a => a.AlunoCidaCodigo == cidadeId);
            var cidade = _cidadeRepo.BuscarPorId(cidadeId);
            
            ViewBag.NomeCidade = cidade?.CIDADESCRICAO ?? "Cidade não encontrada";
            return View("AlunoList", alunos);
        }
    }
}