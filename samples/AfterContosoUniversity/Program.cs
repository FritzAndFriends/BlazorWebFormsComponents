using BlazorWebFormsComponents;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;
using ContosoUniversity.BLL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorWebFormsComponents();

builder.Services.AddDbContextFactory<ContosoUniversityEntities>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ContosoUniversity;Trusted_Connection=True;MultipleActiveResultSets=true"));

builder.Services.AddScoped<EnrollmentLogic>();
builder.Services.AddScoped<CoursesLogic>();
builder.Services.AddScoped<InstructorsLogic>();
builder.Services.AddScoped<StudentsLogic>();

var app = builder.Build();

// Ensure the database is created with the correct schema
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ContosoUniversityEntities>>();
    using var context = factory.CreateDbContext();
    var created = context.Database.EnsureCreated();
    if (!created)
    {
        // Database already exists — verify schema is complete
        try
        {
            _ = context.Students.Any();
            _ = context.Enrollments.Any();
            _ = context.Instructors.Any();
            _ = context.Courses.Any();
            _ = context.Departments.Any();
        }
        catch
        {
            // Schema mismatch — recreate to match current model
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            created = true;
        }
    }
    // Seed data if database was just created and is empty
    if (created || !context.Departments.Any())
    {
        SeedData(context);
    }
}

static void SeedData(ContosoUniversityEntities context)
{
    if (context.Departments.Any()) return;

    var dept = new Department { DepartmentName = "Computer Science", BuildingNumber = 1, ManagingInstructorID = 0 };
    context.Departments.Add(dept);
    context.SaveChanges();

    var instructor1 = new Instructor { FirstName = "John", LastName = "Smith", BirthDate = new DateTime(1975, 3, 15), Email = "john.smith@contoso.edu" };
    var instructor2 = new Instructor { FirstName = "Jane", LastName = "Doe", BirthDate = new DateTime(1980, 7, 22), Email = "jane.doe@contoso.edu" };
    context.Instructors.AddRange(instructor1, instructor2);
    context.SaveChanges();

    dept.ManagingInstructorID = instructor1.InstructorID;
    context.SaveChanges();

    var course1 = new Cours { CourseName = "Introduction to Programming", StudentsMax = 30, DepartmentID = dept.DepartmentID, InstructorID = instructor1.InstructorID };
    var course2 = new Cours { CourseName = "Data Structures", StudentsMax = 25, DepartmentID = dept.DepartmentID, InstructorID = instructor2.InstructorID };
    var course3 = new Cours { CourseName = "Web Development", StudentsMax = 30, DepartmentID = dept.DepartmentID, InstructorID = instructor1.InstructorID };
    context.Courses.AddRange(course1, course2, course3);
    context.SaveChanges();

    var student1 = new Student { FirstName = "Alice", LastName = "Johnson", BirthDate = new DateTime(2000, 1, 10), Email = "alice.johnson@contoso.edu" };
    var student2 = new Student { FirstName = "Bob", LastName = "Williams", BirthDate = new DateTime(1999, 5, 20), Email = "bob.williams@contoso.edu" };
    var student3 = new Student { FirstName = "Charlie", LastName = "Brown", BirthDate = new DateTime(2001, 9, 1), Email = "charlie.brown@contoso.edu" };
    context.Students.AddRange(student1, student2, student3);
    context.SaveChanges();

    context.Enrollments.AddRange(
        new Enrollment { StudentID = student1.StudentID, CourseID = course1.CourseID, Date = new DateTime(2024, 9, 1) },
        new Enrollment { StudentID = student1.StudentID, CourseID = course2.CourseID, Date = new DateTime(2024, 9, 1) },
        new Enrollment { StudentID = student2.StudentID, CourseID = course1.CourseID, Date = new DateTime(2024, 9, 1) },
        new Enrollment { StudentID = student2.StudentID, CourseID = course3.CourseID, Date = new DateTime(2024, 9, 15) },
        new Enrollment { StudentID = student3.StudentID, CourseID = course2.CourseID, Date = new DateTime(2024, 9, 15) },
        new Enrollment { StudentID = student3.StudentID, CourseID = course3.CourseID, Date = new DateTime(2024, 9, 15) }
    );
    context.SaveChanges();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorWebFormsComponents();
app.MapStaticAssets();
app.UseAntiforgery();

app.MapRazorComponents<ContosoUniversity.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
