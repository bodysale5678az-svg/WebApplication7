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

        public async Task<IActionResult> Index()
        {
            var courses = await _context.Course
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .ToListAsync();

            return View(courses);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.AllStudents = await _context.Students
                .OrderBy(s => s.Name)
                .ToListAsync();

            ViewBag.EnrolledIds = course.Enrollments
                .Select(e => e.StudentId)
                .ToList();

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int id, int[]? studentIds)
        {
            var course = await _context.Course
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            var wanted = (studentIds ?? Array.Empty<int>()).ToHashSet();
            var existing = course.Enrollments.ToDictionary(e => e.StudentId);

            foreach (var enrollment in course.Enrollments.Where(e => !wanted.Contains(e.StudentId)).ToList())
            {
                _context.Enrollments.Remove(enrollment);
            }

            var toAdd = wanted.Where(studentId => !existing.ContainsKey(studentId)).ToList();

            if (toAdd.Count > 0)
            {
                var validIds = await _context.Students
                    .Where(s => toAdd.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToListAsync();

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

            await _context.SaveChangesAsync();

            TempData["Message"] = $"Enrollment updated: {wanted.Count} student(s) in this course.";

            return RedirectToAction(nameof(Details), new { id = course.Id });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Course course)
        {
            if (!ModelState.IsValid)
            {
                return View(course);
            }

            _context.Course.Add(course);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Name")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(course);
            }

            try
            {
                _context.Course.Update(course);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(course.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var course = await _context.Course.FindAsync(id);

            if (course != null)
            {
                _context.Course.Remove(course);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int? id)
        {
            return _context.Course.Any(c => c.Id == id);
        }
    }
}
