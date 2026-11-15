namespace WebApplication7.Models
{
    public class Departement
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<Student> Students { get; set; } = new List<Student>();
    }
}
