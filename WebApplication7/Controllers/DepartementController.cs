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

        
        public IActionResult Index()
        {
            List<Departement> departements = _context.Departements.Include(d => d.Students).ToList();
            return View(departements);
        }

        // صفحة إضافة قسم جديد
        public IActionResult Create()
        {
            return View();
        }

   
        [HttpPost]
        public IActionResult Create(Departement departement)
        {
            _context.Departements.Add(departement);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

       
        public IActionResult Edit(int id)
        {
            var departement = _context.Departements.Find(id);
            if (departement == null)
            {
                return NotFound();
            }

            return View(departement);
        }

       
        [HttpPost]
        public IActionResult Edit(int id, Departement departement)
        {
            if (id != departement.Id)
            {
                return NotFound();
            }

            _context.Departements.Update(departement);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        
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

        // الحذف الفعلي
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var departement = _context.Departements.Find(id);
            if (departement != null)
            {
                _context.Departements.Remove(departement);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
