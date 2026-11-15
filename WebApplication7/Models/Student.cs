using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication7.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        [ForeignKey("Departement")]
        public int DepartementId { get; set; }

        public Departement Departement { get; set; }

        /// <summary>
        /// Enrolments for this student. The many-to-many relationship with
        /// Course is expressed through the Enrollment join entity.
        /// </summary>
        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
