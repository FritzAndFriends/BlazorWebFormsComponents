using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Models
{
    public class ContosoUniversityEntities : DbContext
    {
        public ContosoUniversityEntities(DbContextOptions<ContosoUniversityEntities> options)
            : base(options) { }

        public DbSet<Cours> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Cours)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseID);
        }
    }
}

