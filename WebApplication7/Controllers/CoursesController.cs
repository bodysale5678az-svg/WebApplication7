using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication7.Data;
using WebApplication7.Models;

namespace WebApplication7.Controllers
{
    public class CoursesController : Controller
    {
        private readonly AplicationDbContext _context;

        public CoursesController(AplicationDbContext context)
        {
            _context = context;
        }

        // GET: Courses
        public IActionResult Index()
        {
            List<Course> courses = _context.Course
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .ToList();

            return View(courses);
        }

        // GET: Courses/Details/5
        public IActionResult Details(int id)
        {
            var course = _context.Course
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefault(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.AllStudents = _context.Students.OrderBy(s => s.Name).ToList();
            ViewBag.EnrolledIds = course.Enrollments.Select(e => e.StudentId).ToList();

            return View(course);
        }

        // POST: Courses/Enroll/5
        [HttpPost]
        public IActionResult Enroll(int id, int[] studentIds)
        {
            var course = _context.Course
                .Include(c => c.Enrollments)
                .FirstOrDefault(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            HashSet<int> wanted = (studentIds ?? Array.Empty<int>()).ToHashSet();
            Dictionary<int, Enrollment> existing = course.Enrollments.ToDictionary(e => e.StudentId);

            List<Enrollment> toRemove = course.Enrollments
                .Where(e => !wanted.Contains(e.StudentId))
                .ToList();

            foreach (var enrollment in toRemove)
            {
                _context.Enrollments.Remove(enrollment);
            }

            List<int> toAdd = wanted
                .Where(studentId => !existing.ContainsKey(studentId))
                .ToList();

            if (toAdd.Count > 0)
            {
                List<int> validIds = _context.Students
                    .Where(s => toAdd.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToList();

                foreach (var studentId in validIds)
                {
                    _context.Enrollments.Add(new Enrollment
                    {
                        CourseId = course.Id,
                        StudentId = studentId,
                        EnrolledOn = DateTime.Today
                    });
                }
            }

            _context.SaveChanges();

            TempData["Message"] = $"Enrollment updated: {wanted.Count} student(s) in this course.";

            return RedirectToAction(nameof(Details), new { id = course.Id });
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        public IActionResult Create(Course course)
        {
            if (course == null)
            {
                return NotFound();
            }

            _context.Course.Add(course);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Courses/Edit/5
        public IActionResult Edit(int id)
        {
            var course = _context.Course.Find(id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (course == null)
            {
                return NotFound();
            }

            _context.Course.Update(course);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Courses/Delete/5
        public IActionResult Delete(int id)
        {
            var course = _context.Course
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefault(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var course = _context.Course.Find(id);

            if (course != null)
            {
                _context.Course.Remove(course);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
