// EF Core DbContext — migrated from EF6 database-first model

namespace ContosoUniversity.Models
{
    using Microsoft.EntityFrameworkCore;
    
    public partial class ContosoUniversityEntities : DbContext
    {
        public ContosoUniversityEntities(DbContextOptions<ContosoUniversityEntities> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // TODO: Add any EF Core model configuration (indexes, relationships, etc.)
        }
    
        public virtual DbSet<Cours> Courses { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Enrollment> Enrollments { get; set; }
        public virtual DbSet<Instructor> Instructors { get; set; }
        public virtual DbSet<Student> Students { get; set; }
    }
}

