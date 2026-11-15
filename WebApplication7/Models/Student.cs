using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication7.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        [ForeignKey(nameof(Departement))]
        public int DepartementId { get; set; }

        public Departement? Departement { get; set; }

        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
