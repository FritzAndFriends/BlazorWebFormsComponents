using System;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Models
{
    public class ContosoUniversityEntities : DbContext
    {
        public ContosoUniversityEntities(DbContextOptions<ContosoUniversityEntities> options) : base(options) { }

        public DbSet<Cours> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cours>(entity =>
            {
                entity.ToTable("Courses");

                entity.HasOne(d => d.Instructor)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.InstructorID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.DepartmentID)
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
}

