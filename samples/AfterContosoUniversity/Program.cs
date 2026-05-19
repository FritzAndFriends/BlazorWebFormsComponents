using BlazorWebFormsComponents;
using Microsoft.EntityFrameworkCore;
 
using ContosoUniversity.BLL;
using ContosoUniversity.Models;
var builder = WebApplication.CreateBuilder(args);
 
// Add services to the container.
builder.Services.AddRazorComponents();
 
var connectionString = builder.Configuration.GetConnectionString("ContosoUniversityEntities")
    ?? throw new InvalidOperationException("Connection string 'ContosoUniversityEntities' was not found.");
builder.Services.AddDbContext<global::ContosoUniversity.Models.ContosoUniversityEntities>(options =>
    options.UseSqlServer(connectionString));
 
 
// Service classes discovered with constructor injection — registered for DI
builder.Services.AddScoped<Courses_Logic>();
builder.Services.AddScoped<Enrollmet_Logic>();
builder.Services.AddScoped<Instructors_Logic>();
builder.Services.AddScoped<StudentsListLogic>();
builder.Services.AddBlazorWebFormsComponents();
 
var app = builder.Build();
 
// Ensure database tables exist for all registered DbContexts
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<global::ContosoUniversity.Models.ContosoUniversityEntities>();
    db.Database.EnsureCreated();

    // Seed data if empty
    if (!db.Students.Any())
    {
        var instructor = new Instructor { FirstName = "John", LastName = "Smith", BirthDate = new DateTime(1970, 5, 1), Email = "john.smith@contoso.edu" };
        db.Instructors.Add(instructor);
        db.SaveChanges();

        var department = new Department { DepartmentName = "Computer Science", BuildingNumber = 1, ManagingInstructorID = instructor.InstructorID };
        db.Departments.Add(department);
        db.SaveChanges();

        var course1 = new Cours { CourseName = "C# Programming", StudentsMax = 30, DepartmentID = department.DepartmentID, InstructorID = instructor.InstructorID };
        var course2 = new Cours { CourseName = "Web Development", StudentsMax = 25, DepartmentID = department.DepartmentID, InstructorID = instructor.InstructorID };
        db.Courses.AddRange(course1, course2);
        db.SaveChanges();

        var student1 = new Student { FirstName = "Alice", LastName = "Johnson", BirthDate = new DateTime(2000, 3, 15), Email = "alice@contoso.edu" };
        var student2 = new Student { FirstName = "Bob", LastName = "Williams", BirthDate = new DateTime(1999, 7, 22), Email = "bob@contoso.edu" };
        db.Students.AddRange(student1, student2);
        db.SaveChanges();

        db.Enrollments.AddRange(
            new Enrollment { StudentID = student1.StudentID, CourseID = course1.CourseID, Date = new DateTime(2024, 9, 1) },
            new Enrollment { StudentID = student2.StudentID, CourseID = course2.CourseID, Date = new DateTime(2024, 9, 2) }
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
