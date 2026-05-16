using BlazorWebFormsComponents;
using Microsoft.EntityFrameworkCore;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();

var connectionString = builder.Configuration.GetConnectionString("ContosoUniversityEntities")
    ?? throw new InvalidOperationException("Connection string 'ContosoUniversityEntities' was not found.");
builder.Services.AddDbContext<global::ContosoUniversity.Models.ContosoUniversityEntities>(options =>
    options.UseSqlServer(connectionString));

// Register BLL services
builder.Services.AddScoped<global::ContosoUniversity.Bll.Enrollmet_Logic>();
builder.Services.AddScoped<global::ContosoUniversity.Bll.StudentsListLogic>();
builder.Services.AddScoped<global::ContosoUniversity.BLL.Courses_Logic>();
builder.Services.AddScoped<global::ContosoUniversity.BLL.Instructors_Logic>();

builder.Services.AddBlazorWebFormsComponents();

var app = builder.Build();

// Ensure database tables exist for all registered DbContexts
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<global::ContosoUniversity.Models.ContosoUniversityEntities>();
    db.Database.EnsureCreated();

    // Seed data if empty
    if (!db.Departments.Any())
    {
        db.Departments.AddRange(
            new global::ContosoUniversity.Models.Department { DepartmentName = "Computer Science" },
            new global::ContosoUniversity.Models.Department { DepartmentName = "Mathematics" },
            new global::ContosoUniversity.Models.Department { DepartmentName = "Engineering" }
        );
        db.SaveChanges();
    }
    if (!db.Instructors.Any())
    {
        db.Instructors.AddRange(
            new global::ContosoUniversity.Models.Instructor { FirstName = "John", LastName = "Smith", Email = "john.smith@contoso.edu" },
            new global::ContosoUniversity.Models.Instructor { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@contoso.edu" },
            new global::ContosoUniversity.Models.Instructor { FirstName = "Bob", LastName = "Johnson", Email = "bob.johnson@contoso.edu" }
        );
        db.SaveChanges();
    }
    if (!db.Courses.Any())
    {
        var csDept = db.Departments.First(d => d.DepartmentName == "Computer Science");
        var mathDept = db.Departments.First(d => d.DepartmentName == "Mathematics");
        var instructor1 = db.Instructors.First();
        db.Courses.AddRange(
            new global::ContosoUniversity.Models.Cours { CourseName = "Intro to CS", StudentsMax = 30, DepartmentID = csDept.DepartmentID, InstructorID = instructor1.InstructorID },
            new global::ContosoUniversity.Models.Cours { CourseName = "Data Structures", StudentsMax = 25, DepartmentID = csDept.DepartmentID, InstructorID = instructor1.InstructorID },
            new global::ContosoUniversity.Models.Cours { CourseName = "Calculus", StudentsMax = 35, DepartmentID = mathDept.DepartmentID, InstructorID = instructor1.InstructorID }
        );
        db.SaveChanges();
    }
    if (!db.Students.Any())
    {
        db.Students.AddRange(
            new global::ContosoUniversity.Models.Student { FirstName = "Alice", LastName = "Wonder", BirthDate = new System.DateTime(2000, 1, 15), Email = "alice@contoso.edu" },
            new global::ContosoUniversity.Models.Student { FirstName = "Charlie", LastName = "Brown", BirthDate = new System.DateTime(1999, 5, 20), Email = "charlie@contoso.edu" }
        );
        db.SaveChanges();
    }
    if (!db.Enrollments.Any())
    {
        var student1 = db.Students.First();
        var student2 = db.Students.Skip(1).First();
        var course1 = db.Courses.First();
        var course2 = db.Courses.Skip(1).First();
        db.Enrollments.AddRange(
            new global::ContosoUniversity.Models.Enrollment { StudentID = student1.StudentID, CourseID = course1.CourseID, Date = new System.DateTime(2024, 9, 1) },
            new global::ContosoUniversity.Models.Enrollment { StudentID = student1.StudentID, CourseID = course2.CourseID, Date = new System.DateTime(2024, 9, 15) },
            new global::ContosoUniversity.Models.Enrollment { StudentID = student2.StudentID, CourseID = course1.CourseID, Date = new System.DateTime(2025, 1, 10) }
        );
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseBlazorWebFormsComponents();
app.UseAntiforgery();

app.MapRazorComponents<ContosoUniversity.Components.App>();

app.Run();
