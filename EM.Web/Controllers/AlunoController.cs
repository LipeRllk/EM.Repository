using EM.Domain.Context;
using EM.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EM.Web.Controllers
{
    public class AlunoController : Controller
    {
        private readonly EscolaDbContext _context;

        public AlunoController(EscolaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AlunoList(string search)
        {
            var alunos = _context.Alunos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim();
                
                // Tentar converter para int para pesquisa por matrícula
                if (int.TryParse(searchTerm, out int matricula))
                {
                    // Se é um número, pesquisa por matrícula OU por nome
                    alunos = alunos.Where(a => a.AlunoMatricula == matricula || 
                                             EF.Functions.Like(a.AlunoNome.ToLower(), $"%{searchTerm.ToLower()}%"));
                }
                else
                {
                    // Dividir o termo de busca em palavras
                    var palavras = searchTerm.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    
                    // Para cada palavra, o nome deve conter ela
                    foreach (var palavra in palavras)
                    {
                        alunos = alunos.Where(a => EF.Functions.Like(a.AlunoNome.ToLower(), $"%{palavra}%"));
                    }
                }
            }
            //adiciona ordenação a lista por ordem alfabetica
            alunos = alunos.OrderBy(a => a.AlunoNome);

            ViewBag.Search = search;

            return View(await alunos.ToListAsync());
        }

        public IActionResult AlunoCreate()
        {
            ViewBag.Cidades = new SelectList(_context.Cidades, "CIDACODIGO", "CIDADESCRICAO");
            var aluno = new Aluno();
            return View(aluno);
        }

        [HttpPost]
        public async Task<IActionResult> AlunoCreate(Aluno aluno)
        {
            if (ModelState.IsValid)
            {
                aluno.AlunoCPF = aluno.AlunoCPF.Replace(".", "").Replace("-", "");
                _context.Add(aluno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AlunoList));
            }
            ViewBag.Cidades = new SelectList(_context.Cidades, "CIDACODIGO", "CIDADESCRICAO");
            return View(aluno);
        }

        public async Task<IActionResult> AlunoEdit(int? AlunoMatricula)
        {
            if (AlunoMatricula == null) return NotFound();

            var aluno = await _context.Alunos.FindAsync(AlunoMatricula);
            if (aluno == null) return NotFound();

            ViewData["Action"] = "AlunoEdit";
            ViewBag.Cidades = new SelectList(_context.Cidades, "CIDACODIGO", "CIDADESCRICAO", aluno.AlunoCidaCodigo);
            return View(aluno);
        }

        [HttpPost]
        public async Task<IActionResult> AlunoEdit(int AlunoMatricula, Aluno aluno)
        {
            if (AlunoMatricula != aluno.AlunoMatricula) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Limpar formatação do CPF antes de salvar
                    aluno.AlunoCPF = aluno.AlunoCPF.Replace(".", "").Replace("-", "");
                    
                    _context.Update(aluno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Alunos.Any(e => e.AlunoMatricula == AlunoMatricula)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(AlunoList));
            }
            ViewBag.Cidades = new SelectList(_context.Cidades, "CIDACODIGO", "CIDADESCRICAO", aluno.AlunoCidaCodigo);
            return View(aluno);
        }

        public async Task<IActionResult> AlunoDelete(int? AlunoMatricula)
        {
            if (AlunoMatricula == null) return NotFound();

            var aluno = await _context.Alunos.FirstOrDefaultAsync(a => a.AlunoMatricula == AlunoMatricula);
            if (aluno == null) return NotFound();

            return View(aluno);
        }

        [HttpPost, ActionName("AlunoDelete")]
        public async Task<IActionResult> DeleteConfirmed(int AlunoMatricula)
        {
            var aluno = await _context.Alunos.FindAsync(AlunoMatricula);
            if (aluno != null)
            {
                _context.Alunos.Remove(aluno);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(AlunoList));
        }
    }
}