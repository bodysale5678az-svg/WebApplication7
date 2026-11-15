using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication7.Data;
using WebApplication7.Models;

namespace WebApplication7.Controllers
{
    public class DepartementController : Controller
    {
        private readonly AplicationDbContext _context;

        public DepartementController(AplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var departements = await _context.Departements
                .Include(d => d.Students)
                .ToListAsync();

            return View(departements);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Departement departement)
        {
            if (!ModelState.IsValid)
            {
                return View(departement);
            }

            _context.Departements.Add(departement);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departement = await _context.Departements.FindAsync(id);

            if (departement == null)
            {
                return NotFound();
            }

            return View(departement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Name")] Departement departement)
        {
            if (id != departement.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(departement);
            }

            try
            {
                _context.Update(departement);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartementExists(departement.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departement = await _context.Departements
                .Include(d => d.Students)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (departement == null)
            {
                return NotFound();
            }

            return View(departement);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departement = await _context.Departements
                .Include(d => d.Students)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (departement == null)
            {
                return NotFound();
            }

            return View(departement);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var departement = await _context.Departements.FindAsync(id);

            if (departement != null)
            {
                _context.Departements.Remove(departement);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DepartementExists(int id)
        {
            return _context.Departements.Any(d => d.Id == id);
        }
    }
}
