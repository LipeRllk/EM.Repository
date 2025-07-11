using EM.Domain.Context;
using EM.Domain.Models;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> AlunoList()
        {
            return View(await _context.Alunos.ToListAsync());
        }

        public IActionResult AlunoCreate()
        {
            ViewData["Action"] = "AlunoCadastrar";
            var aluno = new Aluno();
            return View(aluno);
        }

        [HttpPost]
        public async Task<IActionResult> AlunoCreate(Aluno aluno)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aluno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AlunoList));
            }
            return View(aluno);
        }

        public async Task<IActionResult> AlunoEdit(int? AlunoMatricula)
        {
            if (AlunoMatricula == null) return NotFound();

            var aluno = await _context.Alunos.FindAsync(AlunoMatricula);
            if (aluno == null) return NotFound();

            ViewData["Action"] = "AlunoEdit";
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
