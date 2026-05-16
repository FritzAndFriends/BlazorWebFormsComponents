using BlazorWebFormsComponents;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();

var connectionString = builder.Configuration.GetConnectionString("ContosoUniversityEntities")
    ?? throw new InvalidOperationException("Connection string 'ContosoUniversityEntities' was not found.");
builder.Services.AddDbContext<global::ContosoUniversity.Models.ContosoUniversityEntities>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<ContosoUniversity.BLL.StudentsListLogic>();
builder.Services.AddScoped<ContosoUniversity.BLL.Courses_Logic>();
builder.Services.AddScoped<ContosoUniversity.BLL.Enrollmet_Logic>();
builder.Services.AddScoped<ContosoUniversity.BLL.Instructors_Logic>();

builder.Services.AddBlazorWebFormsComponents();

var app = builder.Build();

// Ensure database tables exist and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<global::ContosoUniversity.Models.ContosoUniversityEntities>();
    db.Database.EnsureCreated();

    if (!db.Students.Any())
    {
        var dept1 = new ContosoUniversity.Models.Department { DepartmentName = "Engineering", BuildingNumber = 1, ManagingInstructorID = 0 };
        var dept2 = new ContosoUniversity.Models.Department { DepartmentName = "Mathematics", BuildingNumber = 2, ManagingInstructorID = 0 };
        var dept3 = new ContosoUniversity.Models.Department { DepartmentName = "Economics", BuildingNumber = 3, ManagingInstructorID = 0 };
        db.Departments.AddRange(dept1, dept2, dept3);
        db.SaveChanges();

        var instr1 = new ContosoUniversity.Models.Instructor { FirstName = "Kim", LastName = "Abercrombie", BirthDate = new DateTime(1970, 3, 15), Email = "kabercrombie@contoso.com" };
        var instr2 = new ContosoUniversity.Models.Instructor { FirstName = "Fadi", LastName = "Fakhouri", BirthDate = new DateTime(1965, 7, 22), Email = "ffakhouri@contoso.com" };
        var instr3 = new ContosoUniversity.Models.Instructor { FirstName = "Roger", LastName = "Harui", BirthDate = new DateTime(1975, 1, 10), Email = "rharui@contoso.com" };
        db.Instructors.AddRange(instr1, instr2, instr3);
        db.SaveChanges();

        var course1 = new ContosoUniversity.Models.Cours { CourseName = "Chemistry", StudentsMax = 30, DepartmentID = dept1.DepartmentID, InstructorID = instr1.InstructorID };
        var course2 = new ContosoUniversity.Models.Cours { CourseName = "Calculus", StudentsMax = 25, DepartmentID = dept2.DepartmentID, InstructorID = instr2.InstructorID };
        var course3 = new ContosoUniversity.Models.Cours { CourseName = "Microeconomics", StudentsMax = 35, DepartmentID = dept3.DepartmentID, InstructorID = instr3.InstructorID };
        db.Courses.AddRange(course1, course2, course3);
        db.SaveChanges();

        var stud1 = new ContosoUniversity.Models.Student { FirstName = "Carson", LastName = "Alexander", BirthDate = new DateTime(2000, 9, 1), Email = "calexander@contoso.com" };
        var stud2 = new ContosoUniversity.Models.Student { FirstName = "Meredith", LastName = "Alonso", BirthDate = new DateTime(1999, 11, 15), Email = "malonso@contoso.com" };
        var stud3 = new ContosoUniversity.Models.Student { FirstName = "Arturo", LastName = "Anand", BirthDate = new DateTime(2001, 3, 22), Email = "aanand@contoso.com" };
        db.Students.AddRange(stud1, stud2, stud3);
        db.SaveChanges();

        db.Enrollments.AddRange(
            new ContosoUniversity.Models.Enrollment { StudentID = stud1.StudentID, CourseID = course1.CourseID, Date = new DateTime(2024, 9, 1) },
            new ContosoUniversity.Models.Enrollment { StudentID = stud1.StudentID, CourseID = course2.CourseID, Date = new DateTime(2024, 9, 15) },
            new ContosoUniversity.Models.Enrollment { StudentID = stud2.StudentID, CourseID = course2.CourseID, Date = new DateTime(2025, 1, 10) },
            new ContosoUniversity.Models.Enrollment { StudentID = stud3.StudentID, CourseID = course3.CourseID, Date = new DateTime(2025, 1, 20) }
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
