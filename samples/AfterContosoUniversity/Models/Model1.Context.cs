using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Models;

public partial class ContosoUniversityEntities : DbContext
{
    private static string? _connectionString;

    public static void SetConnectionString(string connectionString)
    {
        _connectionString = connectionString;
    }

    public ContosoUniversityEntities() : base()
    {
    }

    public ContosoUniversityEntities(DbContextOptions<ContosoUniversityEntities> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(_connectionString))
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }

    public virtual DbSet<Cours> Courses { get; set; } = null!;
    public virtual DbSet<Department> Departments { get; set; } = null!;
    public virtual DbSet<Enrollment> Enrollments { get; set; } = null!;
    public virtual DbSet<Instructor> Instructors { get; set; } = null!;
    public virtual DbSet<Student> Students { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cours>(entity =>
        {
            entity.HasKey(e => e.CourseID);
            entity.ToTable("Courses");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentID);
            entity.ToTable("Departments");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentID);
            entity.ToTable("Enrollment");
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(e => e.InstructorID);
            entity.ToTable("Instructors");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentID);
            entity.ToTable("Students");
        });
    }
}

