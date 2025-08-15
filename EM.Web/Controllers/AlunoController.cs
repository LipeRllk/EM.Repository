using EM.Domain.Models;
using EM.Domain.Utilitarios;
using EM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EM.Web.Controllers
{
    public class AlunoController : Controller
    {
        private readonly AlunoRepository _repo = new AlunoRepository();
        private readonly CidadeRepository _cidadeRepo = new CidadeRepository();

        public IActionResult AlunoList(string search)
        {
            var alunos = _repo.BuscarAlunos(search);
            
            // Carrega todas as cidades para evitar múltiplas consultas
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
                // Uso de método de extensão para limpar CPF
                aluno.AlunoCPF = aluno.AlunoCPF.LimparCPF();
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
                // Uso de método de extensão para limpar CPF
                aluno.AlunoCPF = aluno.AlunoCPF.LimparCPF();
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

        // API endpoint para buscar cidade por ID (para usar no JavaScript)
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

        // Exemplo de uso de métodos genéricos e LINQ - demonstra recursos da linguagem
        public IActionResult AlunosPorCidade(int cidadeId)
        {
            var alunos = _repo.Get(a => a.AlunoCidaCodigo == cidadeId);
            var cidade = _cidadeRepo.BuscarPorId(cidadeId);
            
            ViewBag.NomeCidade = cidade?.CIDADESCRICAO ?? "Cidade não encontrada";
            return View("AlunoList", alunos);
        }

        // Exemplo de uso de métodos de extensão
        public IActionResult AlunosMaioresDeIdade()
        {
            var alunos = _repo.BuscarMaioresDeIdade();
            ViewBag.Titulo = "Alunos Maiores de Idade";
            return View("AlunoList", alunos);
        }
    }
}