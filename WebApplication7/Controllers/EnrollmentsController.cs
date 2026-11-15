using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication7.Data;
using WebApplication7.Models;

namespace WebApplication7.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly AplicationDbContext _context;

        public EnrollmentsController(AplicationDbContext context)
        {
            _context = context;
        }

        // GET: Enrollments
        public IActionResult Index(int? courseId)
        {
            var query = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsQueryable();

            if (courseId.HasValue)
            {
                query = query.Where(e => e.CourseId == courseId.Value);
            }

            ViewBag.Courses = _context.Course.OrderBy(c => c.Name).ToList();
            ViewBag.SelectedCourseId = courseId;

            List<Enrollment> enrollments = query
                .OrderBy(e => e.Course!.Name)
                .ThenBy(e => e.Student!.Name)
                .ToList();

            return View(enrollments);
        }

        // GET: Enrollments/Details/5
        public IActionResult Details(int id)
        {
            var enrollment = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefault(e => e.Id == id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        public IActionResult Create()
        {
            PopulateDropDowns(null, null);
            return View(new Enrollment { EnrolledOn = DateTime.Today });
        }

        // POST: Enrollments/Create
        [HttpPost]
        public IActionResult Create(Enrollment enrollment)
        {
            if (enrollment == null)
            {
                return NotFound();
            }

            ValidatePair(enrollment, null);

            if (!ModelState.IsValid)
            {
                PopulateDropDowns(enrollment.StudentId, enrollment.CourseId);
                return View(enrollment);
            }

            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();

            TempData["Message"] = "Enrolment created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Enrollments/Edit/5
        public IActionResult Edit(int id)
        {
            var enrollment = _context.Enrollments.Find(id);

            if (enrollment == null)
            {
                return NotFound();
            }

            PopulateDropDowns(enrollment.StudentId, enrollment.CourseId);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            if (enrollment == null)
            {
                return NotFound();
            }

            ValidatePair(enrollment, enrollment.Id);

            if (!ModelState.IsValid)
            {
                PopulateDropDowns(enrollment.StudentId, enrollment.CourseId);
                return View(enrollment);
            }

            _context.Enrollments.Update(enrollment);
            _context.SaveChanges();

            TempData["Message"] = "Enrolment updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Enrollments/Delete/5
        public IActionResult Delete(int id)
        {
            var enrollment = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefault(e => e.Id == id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var enrollment = _context.Enrollments.Find(id);

            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                _context.SaveChanges();

                TempData["Message"] = "Enrolment deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropDowns(int? studentId, int? courseId)
        {
            List<Student> students = _context.Students.OrderBy(s => s.Name).ToList();
            ViewBag.Students = new SelectList(students, "Id", "Name", studentId);

            List<Course> courses = _context.Course.OrderBy(c => c.Name).ToList();
            ViewBag.Courses = new SelectList(courses, "Id", "Name", courseId);
        }

        private void ValidatePair(Enrollment enrollment, int? ignoreId)
        {
            bool alreadyExists = _context.Enrollments.Any(e =>
                e.StudentId == enrollment.StudentId &&
                e.CourseId == enrollment.CourseId &&
                (ignoreId == null || e.Id != ignoreId));

            if (alreadyExists)
            {
                ModelState.AddModelError(string.Empty,
                    "This student is already enrolled in that course.");
            }
        }
    }
}
