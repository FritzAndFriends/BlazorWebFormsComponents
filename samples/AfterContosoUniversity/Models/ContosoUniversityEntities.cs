using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Models;

public partial class ContosoUniversityEntities : DbContext
{
    public ContosoUniversityEntities(DbContextOptions<ContosoUniversityEntities> options) : base(options)
    {
    }

    public DbSet<Cours> Courses { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<Enrollment> Enrollments { get; set; } = null!;
    public DbSet<Instructor> Instructors { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cours>(entity =>
        {
            entity.HasKey(e => e.CourseID);
            entity.ToTable("Courses");
            entity.HasOne(d => d.Department)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.DepartmentID)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.Instructor)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.InstructorID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasOne(d => d.Cours)
                .WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseID)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.Student)
                .WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.StudentID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.ToTable("Instructors");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students");
        });

    }
}
