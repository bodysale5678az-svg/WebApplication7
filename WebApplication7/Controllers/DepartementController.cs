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

        // GET: Departement
        public IActionResult Index()
        {
            List<Departement> departements = _context.Departements.Include(d => d.Students).ToList();
            return View(departements);
        }

        // GET: Departement/Details/5
        public IActionResult Details(int id)
        {
            var departement = _context.Departements
                .Include(d => d.Students)
                .FirstOrDefault(d => d.Id == id);

            if (departement == null)
            {
                return NotFound();
            }

            return View(departement);
        }

        // GET: Departement/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Departement departement)
        {
            if (departement == null)
            {
                return NotFound();
            }

            _context.Departements.Add(departement);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Departement/Edit/5
        public IActionResult Edit(int id)
        {
            var departement = _context.Departements.Find(id);

            if (departement == null)
            {
                return NotFound();
            }

            return View(departement);
        }

        // POST: Departement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Departement departement)
        {
            if (id != departement.Id)
            {
                return NotFound();
            }

            if (departement == null)
            {
                return NotFound();
            }

            _context.Departements.Update(departement);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Departement/Delete/5
        public IActionResult Delete(int id)
        {
            var departement = _context.Departements
                .Include(d => d.Students)
                .FirstOrDefault(d => d.Id == id);

            if (departement == null)
            {
                return NotFound();
            }

            return View(departement);
        }

        // POST: Departement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var departement = _context.Departements.Find(id);

            if (departement != null)
            {
                _context.Departements.Remove(departement);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
