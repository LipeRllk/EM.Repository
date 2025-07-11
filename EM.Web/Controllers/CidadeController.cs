using EM.Domain.Context;
using EM.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EM.Web.Controllers
{
    public class CidadeController : Controller
    {
        private readonly EscolaDbContext _context;

        public CidadeController(EscolaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CidadeList()
        {
            return View(await _context.Cidades.ToListAsync());
        }

        public IActionResult CidadeCreate()
        {
            ViewData["Action"] = "CidadeCreate";
            var cidade = new Cidade();
            return View(cidade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CidadeCreate([Bind("CIDADESCRICAO,CIDAUF,CIDACODIGOIBGE")] Cidade cidade)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cidade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CidadeList));
            }
            return View(cidade);
        }

        public async Task<IActionResult> CidadeEdit(int? id)
        {
            if (id == null) return NotFound();

            var cidade = await _context.Cidades.FindAsync(id);
            if (cidade == null) return NotFound();

            ViewData["Action"] = "CidadeEdit";
            return View(cidade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CidadeEdit(int id, [Bind("CIDACODIGO,CIDADESCRICAO,CIDAUF,CIDACODIGOIBGE")] Cidade cidade)
        {
            if (id != cidade.CIDACODIGO)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cidade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Cidades.Any(e => e.CIDACODIGO == cidade.CIDACODIGO))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(CidadeList));
            }
            return View(cidade);
        }

        public async Task<IActionResult> CidadeDelete(int? id)
        {
            if (id == null) return NotFound();

            var cidade = await _context.Cidades.FirstOrDefaultAsync(c => c.CIDACODIGO == id);
            if (cidade == null) return NotFound();

            // Verificar se existem alunos vinculados a esta cidade
            var quantidadeAlunos = await _context.Alunos.CountAsync(a => a.AlunoCidaCodigo == id);
            ViewBag.TemAlunos = quantidadeAlunos > 0;
            ViewBag.QuantidadeAlunos = quantidadeAlunos;

            return View(cidade);
        }

        [HttpPost, ActionName("CidadeDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int CIDACODIGO)
        {
            var cidade = await _context.Cidades.FindAsync(CIDACODIGO);
            if (cidade != null)
            {
                // Verificar novamente se existem alunos vinculados
                var alunosVinculados = await _context.Alunos.CountAsync(a => a.AlunoCidaCodigo == CIDACODIGO);
                
                if (alunosVinculados > 0)
                {
                    // Se existem alunos vinculados, não permitir a exclusão
                    ViewBag.ErroExclusao = $"Não é possível excluir esta cidade pois existem {alunosVinculados} aluno(s) cadastrado(s) nela.";
                    ViewBag.TemAlunos = true;
                    ViewBag.QuantidadeAlunos = alunosVinculados;
                    return View("CidadeDelete", cidade);
                }

                try
                {
                    _context.Cidades.Remove(cidade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    // Capturar erros de integridade referencial
                    ViewBag.ErroExclusao = "Erro ao excluir cidade: Esta cidade está sendo referenciada por outros registros no sistema.";
                    ViewBag.TemAlunos = true;
                    return View("CidadeDelete", cidade);
                }
            }
            return RedirectToAction(nameof(CidadeList));
        }
    }
}