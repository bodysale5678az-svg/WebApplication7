using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication7.Data;
using WebApplication7.Models;

namespace WebApplication7.Controllers
{
    public class StudentController : Controller
    {
        private readonly AplicationDbContext _context;

        public StudentController(AplicationDbContext context)
        {
            _context = context;
        }

        // GET: Student
        public IActionResult Index()
        {
            List<Student> students = _context.Students.Include(s => s.Departement).ToList();
            return View(students);
        }

        // GET: Student/Details/5
        public IActionResult Details(int id)
        {
            var student = _context.Students
                .Include(s => s.Departement)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Student/Create
        public IActionResult Create()
        {
            PopulateDepartements(null);
            return View();
        }

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student student)
        {
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Add(student);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Student/Edit/5
        public IActionResult Edit(int id)
        {
            var student = _context.Students.Find(id);

            if (student == null)
            {
                return NotFound();
            }

            PopulateDepartements(student.DepartementId);
            return View(student);
        }

        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Update(student);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Student/Delete/5
        public IActionResult Delete(int id)
        {
            var student = _context.Students
                .Include(s => s.Departement)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var student = _context.Students.Find(id);

            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        private void PopulateDepartements(int? selectedId)
        {
            List<Departement> departements = _context.Departements.OrderBy(d => d.Name).ToList();
            ViewBag.Departements = new SelectList(departements, "Id", "Name", selectedId);
        }
    }
}
