using EducationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EducationSystem.Data
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
        public DbSet<WorkerTopic> WorkerTopics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkerTopic>().HasKey(wt => new { wt.WorkerId, wt.TopicId});
            modelBuilder.Entity<Topic>().HasMany(t => t.SubTopics).WithOne(t => t.Parent);
        }
    }
}
