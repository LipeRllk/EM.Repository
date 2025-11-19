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
            ViewBag.Cidades = cidades.ToDictionary(c => c.Id, c => c.Descricao);
            ViewBag.Search = search;
            
            return View(alunos);
        }

        public IActionResult AlunoCreate()
        {
            ViewBag.Cidades = new SelectList(_cidadeRepo.ListarTodas(), "Id", "Descricao");
            ViewData["Action"] = "AlunoCreate";
            return View(new Aluno());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AlunoCreate(Aluno aluno)
        {
            if (ModelState.IsValid)
            {
                aluno.Cpf ??= string.Empty;
                _repo.Inserir(aluno);
                return RedirectToAction(nameof(AlunoList));
            }
            ViewBag.Cidades = new SelectList(_cidadeRepo.ListarTodas(), "Id", "Descricao");
            ViewData["Action"] = "AlunoCreate";
            return View(aluno);
        }

        public IActionResult AlunoEdit(int? AlunoMatricula)
        {
            if (AlunoMatricula == null) return NotFound();

            var aluno = _repo.BuscarPorMatriculaTradicional(AlunoMatricula.Value);
            if (aluno == null) return NotFound();

            ViewBag.Cidades = new SelectList(_cidadeRepo.ListarTodas(), "Id", "Descricao", aluno.AlunoCidaCodigo);
            ViewData["Action"] = "AlunoEdit";
            return View(aluno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AlunoEdit(int AlunoMatricula, Aluno aluno)
        {
            if (AlunoMatricula != aluno.Matricula)
                return NotFound();

            if (ModelState.IsValid)
            {
                aluno.Cpf ??= string.Empty;
                _repo.Atualizar(aluno);
                return RedirectToAction(nameof(AlunoList));
            }
            
            ViewBag.Cidades = new SelectList(_cidadeRepo.ListarTodas(), "Id", "Descricao", aluno.AlunoCidaCodigo);
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
                cidacodigo = cidade.Id,
                cidadescricao = cidade.Descricao,
                cidauf = cidade.Uf 
            });
        }

        public IActionResult AlunosPorCidade(int cidadeId)
        {
            var alunos = _repo.Get(a => a.AlunoCidaCodigo == cidadeId);
            var cidade = _cidadeRepo.BuscarPorId(cidadeId);
            
            ViewBag.NomeCidade = cidade?.Descricao ?? "Cidade não encontrada";
            ViewBag.Cidades = new SelectList(_cidadeRepo.ListarTodas(), "Id", "Descricao");
            return View("AlunoList", alunos);
        }
    }
}