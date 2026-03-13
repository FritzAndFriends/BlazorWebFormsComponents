namespace ContosoUniversity.Models
{
    using Microsoft.EntityFrameworkCore;

    public partial class ContosoUniversityEntities : DbContext
    {
        public ContosoUniversityEntities(DbContextOptions<ContosoUniversityEntities> options) : base(options) { }

        public virtual DbSet<Cours> Courses { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Enrollment> Enrollments { get; set; }
        public virtual DbSet<Instructor> Instructors { get; set; }
        public virtual DbSet<Student> Students { get; set; }
    }
}


