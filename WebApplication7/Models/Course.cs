namespace WebApplication7.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Enrolments for this course. The many-to-many relationship with
        /// Student is expressed through the Enrollment join entity.
        /// </summary>
        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
