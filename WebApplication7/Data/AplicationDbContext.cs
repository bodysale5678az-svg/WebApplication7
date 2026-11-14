using Microsoft.EntityFrameworkCore;
using WebApplication7.Models;

namespace WebApplication7.Data
{
    public class AplicationDbContext : DbContext
    {
        public DbSet<WebApplication7.Models.Course> Course { get; set; } = default!;
        public AplicationDbContext(DbContextOptions<AplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Departement> Departements { get; set; }
    }
}
