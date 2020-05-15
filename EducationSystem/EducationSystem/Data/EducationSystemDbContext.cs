using EducationSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EducationSystem.Data
{
    public class EducationSystemDbContext : IdentityDbContext<ApplicationUser>
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
            modelBuilder.Entity<Topic>();
            modelBuilder.Entity<Team>();
            modelBuilder.Entity<Goal>();
            modelBuilder.Entity<Worker>();
            modelBuilder.Entity<LearningDay>();
            modelBuilder.Entity<Restriction>();
            modelBuilder.Entity<WorkerTopic>().HasKey(wt => new { wt.WorkerId, wt.TopicId });
            modelBuilder.Entity<ApplicationUser>().HasIndex(wt => wt.workerId).IsUnique();
            base.OnModelCreating(modelBuilder);

        }
    }
}
