using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication7.Data;
using WebApplication7.Models;

namespace WebApplication7.Controllers
{
    /// <summary>
    /// Full CRUD for the Enrollment join entity.
    ///
    /// CoursesController still exposes Enroll() so a course page can manage
    /// its own roster, but this controller is the dedicated place to view
    /// every enrolment in the system and to create / edit / delete one row
    /// at a time (for example to correct a mistake).
    /// </summary>
    public class EnrollmentsController : Controller
    {
        private readonly AplicationDbContext _context;

        public EnrollmentsController(AplicationDbContext context)
        {
            _context = context;
        }

        // GET: Enrollments
        public async Task<IActionResult> Index(int? courseId)
        {
            var query = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsQueryable();

            if (courseId.HasValue)
                query = query.Where(e => e.CourseId == courseId.Value);

            ViewBag.Courses = await _context.Course.OrderBy(c => c.Name).ToListAsync();
            ViewBag.SelectedCourseId = courseId;

            var enrollments = await query
                .OrderBy(e => e.Course!.Name)
                .ThenBy(e => e.Student!.Name)
                .ToListAsync();

            return View(enrollments);
        }

        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null) return NotFound();

            return View(enrollment);
        }

        // GET: Enrollments/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropDowns();
            return View(new Enrollment { EnrolledOn = DateTime.Today });
        }

        // POST: Enrollments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,CourseId,EnrolledOn")] Enrollment enrollment)
        {
            await ValidatePair(enrollment);

            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Enrolment created successfully.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropDowns(enrollment.StudentId, enrollment.CourseId);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null) return NotFound();

            await PopulateDropDowns(enrollment.StudentId, enrollment.CourseId);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,CourseId,EnrolledOn")] Enrollment enrollment)
        {
            if (id != enrollment.Id) return NotFound();

            await ValidatePair(enrollment, enrollment.Id);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id)) return NotFound();
                    throw;
                }

                TempData["Message"] = "Enrolment updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropDowns(enrollment.StudentId, enrollment.CourseId);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null) return NotFound();

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Enrolment deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------- helpers ----------
        private async Task PopulateDropDowns(int? studentId = null, int? courseId = null)
        {
            ViewBag.Students = new SelectList(
                await _context.Students.OrderBy(s => s.Name).ToListAsync(),
                "Id", "Name", studentId);

            ViewBag.Courses = new SelectList(
                await _context.Course.OrderBy(c => c.Name).ToListAsync(),
                "Id", "Name", courseId);
        }

        /// <summary>The unique index already enforces this, but a friendly
        /// ModelState error is nicer than a database exception.</summary>
        private async Task ValidatePair(Enrollment enrollment, int? ignoreId = null)
        {
            bool alreadyExists = await _context.Enrollments.AnyAsync(e =>
                e.StudentId == enrollment.StudentId &&
                e.CourseId == enrollment.CourseId &&
                (ignoreId == null || e.Id != ignoreId));

            if (alreadyExists)
            {
                ModelState.AddModelError(string.Empty,
                    "This student is already enrolled in that course.");
            }
        }

        private bool EnrollmentExists(int id)
            => _context.Enrollments.Any(e => e.Id == id);
    }
}
