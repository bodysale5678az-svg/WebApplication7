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

        public async Task<IActionResult> Index(int? courseId)
        {
            var query = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsQueryable();

            if (courseId.HasValue)
            {
                query = query.Where(e => e.CourseId == courseId.Value);
            }

            ViewBag.Courses = await _context.Course.OrderBy(c => c.Name).ToListAsync();
            ViewBag.SelectedCourseId = courseId;

            var enrollments = await query
                .OrderBy(e => e.Course!.Name)
                .ThenBy(e => e.Student!.Name)
                .ToListAsync();

            return View(enrollments);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropDowns();
            return View(new Enrollment { EnrolledOn = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,CourseId,EnrolledOn")] Enrollment enrollment)
        {
            await ValidatePair(enrollment);

            if (!ModelState.IsValid)
            {
                await PopulateDropDowns(enrollment.StudentId, enrollment.CourseId);
                return View(enrollment);
            }

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Enrolment created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);

            if (enrollment == null)
            {
                return NotFound();
            }

            await PopulateDropDowns(enrollment.StudentId, enrollment.CourseId);
            return View(enrollment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,StudentId,CourseId,EnrolledOn")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            await ValidatePair(enrollment, enrollment.Id);

            if (!ModelState.IsValid)
            {
                await PopulateDropDowns(enrollment.StudentId, enrollment.CourseId);
                return View(enrollment);
            }

            try
            {
                _context.Enrollments.Update(enrollment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnrollmentExists(enrollment.Id))
                {
                    return NotFound();
                }

                throw;
            }

            TempData["Message"] = "Enrolment updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
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

        private async Task PopulateDropDowns(int? studentId = null, int? courseId = null)
        {
            ViewBag.Students = new SelectList(
                await _context.Students.OrderBy(s => s.Name).ToListAsync(),
                "Id", "Name", studentId);

            ViewBag.Courses = new SelectList(
                await _context.Course.OrderBy(c => c.Name).ToListAsync(),
                "Id", "Name", courseId);
        }

        private async Task ValidatePair(Enrollment enrollment, int? ignoreId = null)
        {
            var alreadyExists = await _context.Enrollments.AnyAsync(e =>
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
        {
            return _context.Enrollments.Any(e => e.Id == id);
        }
    }
}
