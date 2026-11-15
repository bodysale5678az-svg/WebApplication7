
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication7.Models;
using WebApplication7.Data;

public class CoursesController : Controller
{
    private readonly AplicationDbContext _context;

    public CoursesController(AplicationDbContext context)
    {
        _context = context;
    }

    // GET: COURSES
    public async Task<IActionResult> Index()
    {
        return View(await _context.Course
            .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
            .ToListAsync());
    }

    // GET: COURSES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _context.Course
            .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (course == null)
        {
            return NotFound();
        }

        // كل الطلاب في النظام عشان نعرضهم في checkboxes
        ViewBag.AllStudents = await _context.Students
            .OrderBy(s => s.Name)
            .ToListAsync();

        // أرقام الطلاب المسجلين فعلاً في الكورس ده
        ViewBag.EnrolledIds = course.Enrollments
            .Select(e => e.StudentId)
            .ToList();

        return View(course);
    }

    // POST: COURSES/Enroll/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(int id, int[] studentIds)
    {
        var course = await _context.Course
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            return NotFound();
        }

        // نبني مجموعتين: المطلوب والموجود حالياً
        var wanted = (studentIds ?? Array.Empty<int>()).ToHashSet();
        var existing = course.Enrollments
            .ToDictionary(e => e.StudentId, e => e);

        // 1) احذف الي اتشال
        var toRemove = course.Enrollments
            .Where(e => !wanted.Contains(e.StudentId))
            .ToList();

        foreach (var enrollment in toRemove)
        {
            _context.Enrollments.Remove(enrollment);
        }

        // 2) ضيف الجديد بس
        var toAdd = wanted.Where(sid => !existing.ContainsKey(sid)).ToList();

        if (toAdd.Count > 0)
        {
            // نتأكد إن الطلاب موجودين فعلاً
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

    // GET: COURSES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: COURSES/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name")] Course course)
    {
        if (ModelState.IsValid)
        {
            _context.Add(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(course);
    }

    // GET: COURSES/Edit/5
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

    // POST: COURSES/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Name")] Course course)
    {
        if (id != course.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(course.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(course);
    }

    // GET: COURSES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _context.Course
            .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    // POST: COURSES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var course = await _context.Course.FindAsync(id);
        if (course != null)
        {
            _context.Course.Remove(course);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CourseExists(int? id)
    {
        return _context.Course.Any(e => e.Id == id);
    }
}
