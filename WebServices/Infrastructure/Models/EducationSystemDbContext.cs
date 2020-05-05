using Infrastructure.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    public class EducationSystemDbContext : DbContext
    {
        public EducationSystemDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Topic> Topics { get; set; }
        public DbSet<LearningDay> LearningDays { get; set; }
        public DbSet<Restriction> Restrictions { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Worker> Workers { get; set; }
    }
}
