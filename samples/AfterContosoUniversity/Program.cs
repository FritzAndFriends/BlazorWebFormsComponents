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
builder.Services.AddScoped<ContosoUniversity.BLL.Enrollmet_Logic>();
builder.Services.AddScoped<Instructors_Logic>();
builder.Services.AddScoped<StudentsListLogic>();
builder.Services.AddBlazorWebFormsComponents();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContosoUniversityEntities>();
    db.Database.EnsureCreated();

    if (!db.Enrollments.Any())
    {
        if (db.Students.Any())
        {
            db.Students.RemoveRange(db.Students);
            db.SaveChanges();
        }

        var instructor = new Instructor { FirstName = "John", LastName = "Doe", BirthDate = new DateTime(1970, 1, 1), Email = "john.doe@contoso.edu" };
        db.Instructors.Add(instructor);
        db.SaveChanges();

        var department = new Department { DepartmentName = "Computer Science", BuildingNumber = 1, ManagingInstructorID = instructor.InstructorID };
        var department2 = new Department { DepartmentName = "Mathematics", BuildingNumber = 2, ManagingInstructorID = instructor.InstructorID };
        db.Departments.AddRange(department, department2);
        db.SaveChanges();

        var course1 = new Cours { CourseName = "Intro to Programming", StudentsMax = 30, DepartmentID = department.DepartmentID, InstructorID = instructor.InstructorID };
        var course2 = new Cours { CourseName = "Data Structures", StudentsMax = 25, DepartmentID = department.DepartmentID, InstructorID = instructor.InstructorID };
        db.Courses.AddRange(course1, course2);
        db.SaveChanges();

        var student1 = new Student { FirstName = "Alice", LastName = "Johnson", BirthDate = new DateTime(2000, 5, 15), Email = "alice@contoso.edu" };
        var student2 = new Student { FirstName = "Bob", LastName = "Smith", BirthDate = new DateTime(1999, 8, 22), Email = "bob@contoso.edu" };
        db.Students.AddRange(student1, student2);
        db.SaveChanges();

        var enrollment1 = new Enrollment { StudentID = student1.StudentID, CourseID = course1.CourseID, Date = new DateTime(2024, 9, 1) };
        var enrollment2 = new Enrollment { StudentID = student2.StudentID, CourseID = course2.CourseID, Date = new DateTime(2024, 9, 2) };
        db.Enrollments.AddRange(enrollment1, enrollment2);
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
