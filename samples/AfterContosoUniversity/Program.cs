using BlazorWebFormsComponents;
using Microsoft.EntityFrameworkCore;

using ContosoUniversity.BLL;
using ContosoUniversity.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("ContosoUniversityEntities")
    ?? throw new InvalidOperationException("Connection string 'ContosoUniversityEntities' was not found.");
builder.Services.AddDbContext<ContosoUniversityEntities>(options =>
    options.UseSqlServer(connectionString));

// Service classes discovered with constructor injection — registered for DI
builder.Services.AddScoped<Courses_Logic>();
builder.Services.AddScoped<Enrollmet_Logic>();
builder.Services.AddScoped<Instructors_Logic>();
builder.Services.AddScoped<StudentsListLogic>();
builder.Services.AddScoped<Enrollmet_Logic>();
builder.Services.AddBlazorWebFormsComponents();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContosoUniversityEntities>();
    db.Database.EnsureCreated();

    if (!db.Instructors.Any())
    {
        db.Instructors.AddRange(
            new Instructor
            {
                FirstName = "Alan",
                LastName = "Turing",
                Email = "alan@test.com",
                BirthDate = new DateTime(1912, 6, 23)
            },
            new Instructor
            {
                FirstName = "Marie",
                LastName = "Curie",
                Email = "marie@test.com",
                BirthDate = new DateTime(1867, 11, 7)
            });
        db.SaveChanges();
    }

    if (!db.Departments.Any())
    {
        var engineeringInstructor = db.Instructors.First();
        var scienceInstructor = db.Instructors.Skip(1).FirstOrDefault() ?? engineeringInstructor;

        db.Departments.AddRange(
            new Department
            {
                DepartmentName = "Engineering",
                BuildingNumber = 1,
                ManagingInstructorID = engineeringInstructor.InstructorID
            },
            new Department
            {
                DepartmentName = "Science",
                BuildingNumber = 2,
                ManagingInstructorID = scienceInstructor.InstructorID
            });
        db.SaveChanges();
    }

    if (!db.Courses.Any())
    {
        var engineering = db.Departments.First(d => d.DepartmentName == "Engineering");
        var science = db.Departments.First(d => d.DepartmentName == "Science");
        var engineeringInstructor = db.Instructors.First(i => i.LastName == "Turing");
        var scienceInstructor = db.Instructors.First(i => i.LastName == "Curie");

        db.Courses.AddRange(
            new Cours
            {
                CourseName = "Mathematics",
                DepartmentID = engineering.DepartmentID,
                InstructorID = engineeringInstructor.InstructorID,
                StudentsMax = 30
            },
            new Cours
            {
                CourseName = "Physics",
                DepartmentID = science.DepartmentID,
                InstructorID = scienceInstructor.InstructorID,
                StudentsMax = 30
            },
            new Cours
            {
                CourseName = "Chemistry",
                DepartmentID = science.DepartmentID,
                InstructorID = scienceInstructor.InstructorID,
                StudentsMax = 30
            });
        db.SaveChanges();
    }

    if (!db.Students.Any())
    {
        db.Students.AddRange(
            new Student
            {
                FirstName = "John",
                LastName = "Smith",
                Email = "john@test.com",
                BirthDate = new DateTime(1995, 1, 15)
            },
            new Student
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane@test.com",
                BirthDate = new DateTime(1996, 3, 20)
            });
        db.SaveChanges();

        var john = db.Students.First(s => s.FirstName == "John");
        var math = db.Courses.First(c => c.CourseName == "Mathematics");

        db.Enrollments.Add(new Enrollment
        {
            StudentID = john.StudentID,
            CourseID = math.CourseID,
            Date = DateTime.Now
        });
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

app.MapRazorComponents<ContosoUniversity.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
