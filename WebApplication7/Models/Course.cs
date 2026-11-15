namespace WebApplication7.Models
{
    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
