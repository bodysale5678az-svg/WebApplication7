using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication7.Models
{
    /// <summary>
    /// Enrollment — the join entity between Student and Course.
    ///
    /// A student can enrol in many courses and a course can have many
    /// students. That many-to-many relationship is represented by this
    /// explicit class instead of the implicit junction table EF Core
    /// would create on its own.
    ///
    /// Having the entity lets us attach extra data to the relationship
    /// later (grade, enrolment date, attendance…) without redesigning
    /// the schema.
    /// </summary>
    public class Enrollment
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        /// <summary>When the student joined the course.</summary>
        [DataType(DataType.Date)]
        [Display(Name = "Enrolled On")]
        public DateTime EnrolledOn { get; set; } = DateTime.Today;

        // ---- navigation properties ----
        [ForeignKey(nameof(StudentId))]
        public Student? Student { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course? Course { get; set; }
    }
}
