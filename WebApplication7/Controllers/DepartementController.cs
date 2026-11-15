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
            var departments = _context.Departements.Include(d => d.Students).ToList();

            return View(departments);
        }

        // GET: Departement/Details/5
        public IActionResult Details(int id)
        {
            var department = _context.Departements
                .Include(d => d.Students)
                .FirstOrDefault(d => d.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Department/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Department/Create
        [HttpPost]
        public IActionResult Create(Departement department)
        {
            _context.Departements.Add(department);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Department/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var department = _context.Departements.Find(id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Department/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, Departement department)
        {
            _context.Departements.Update(department);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Department/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var department = _context.Departements
                .Include(d => d.Students)
                .FirstOrDefault(d => d.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var department = _context.Departements.Find(id);

            if (department != null)
            {
                _context.Departements.Remove(department);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
