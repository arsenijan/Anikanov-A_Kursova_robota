using API.Models;
using Microsoft.EntityFrameworkCore;


namespace API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Disciplines> Disciplines { get; set; }
        public DbSet<StudentAchievement> StudentAchievements { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<OlympiadParticipation> OlympiadParticipations { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
         
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          
            modelBuilder.Entity<User>()
                .HasMany(u => u.Disciplines)
                .WithMany(d => d.Users)
                .UsingEntity(j => j.ToTable("UserDisciplines"));

        }
    }
}
