using Microsoft.AspNetCore.Mvc;
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

      
        public IActionResult Index()
        {
            List<Student> students = _context.Students.Include(s => s.Departement).ToList();
            return View(students);
        }

      
        public IActionResult Create()
        {
            ViewBag.Departements = _context.Departements.ToList();
            return View();
        }

        // حفظ الطالب الجديد في الداتا بيز
        [HttpPost]
        public IActionResult Create(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        
        public IActionResult Edit(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }

            ViewBag.Departements = _context.Departements.ToList();
            return View(student);
        }

        
        [HttpPost]
        public IActionResult Edit(int id, Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            _context.Students.Update(student);
            _context.SaveChanges();
            ViewBag.Departements = _context.Departements.ToList();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var student = _context.Students
                .Include(s => s.Departement)
                .FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

 
        public IActionResult Delete(int id)
        {
            var student = _context.Students
                .Include(s => s.Departement)
                .FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var student = _context.Students.Find(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
