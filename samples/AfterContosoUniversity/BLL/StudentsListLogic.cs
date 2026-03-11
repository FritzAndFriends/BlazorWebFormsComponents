using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.BLL;

public class StudentsListLogic
{
    private readonly IDbContextFactory<ContosoUniversityContext> _dbFactory;

    public StudentsListLogic(IDbContextFactory<ContosoUniversityContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public List<StudentViewModel> GetJoinedTableData()
    {
        using var context = _dbFactory.CreateDbContext();

        var grouped = from e in context.Enrollments.Include(e => e.Student)
                      group e by new
                      {
                          e.Date,
                          e.Student!.LastName,
                          e.Student.FirstName,
                          e.Student.Email,
                          e.Student.StudentID
                      } into g
                      select new StudentViewModel
                      {
                          ID = g.Key.StudentID,
                          Date = g.Key.Date.ToShortDateString(),
                          FullName = g.Key.FirstName + " " + g.Key.LastName,
                          Email = g.Key.Email,
                          Count = g.Count()
                      };

        return grouped.ToList();
    }

    public void UpdateStudentData(int id, string name, string? email = null)
    {
        using var context = _dbFactory.CreateDbContext();
        var student = context.Students.First(s => s.StudentID == id);

        if (name != null)
        {
            var arr = name.Split(' ');
            if (arr.Length > 1)
            {
                student.FirstName = arr[0];
                student.LastName = arr[1];
                if (email != null) student.Email = email;
                context.SaveChanges();
            }
        }
    }

    public void DeleteStudent(int id)
    {
        using var context = _dbFactory.CreateDbContext();
        var student = context.Students.First(s => s.StudentID == id);
        context.Students.Remove(student);
        context.SaveChanges();
    }

    public void InsertNewEntry(string firstName, string lastName, DateTime birthDate, string course, string email = "Has not specified")
    {
        using var context = _dbFactory.CreateDbContext();

        var existingStudent = context.Students
            .FirstOrDefault(s => s.FirstName == firstName && s.LastName == lastName
                && s.BirthDate == birthDate && s.Email == email);

        int studentId;

        if (existingStudent == null)
        {
            var newStudent = new Student
            {
                FirstName = firstName,
                LastName = lastName,
                BirthDate = birthDate,
                Email = email
            };
            context.Students.Add(newStudent);
            context.SaveChanges();
            studentId = newStudent.StudentID;
        }
        else
        {
            studentId = existingStudent.StudentID;
        }

        var courseEntity = context.Courses.FirstOrDefault(c => c.CourseName == course);
        if (courseEntity != null)
        {
            context.Enrollments.Add(new Enrollment
            {
                StudentID = studentId,
                CourseID = courseEntity.CourseID,
                Date = DateTime.Now
            });
            context.SaveChanges();
        }
    }

    public List<Student> GetStudents(string name)
    {
        using var context = _dbFactory.CreateDbContext();

        if (string.IsNullOrEmpty(name)) return new List<Student>();

        var arr = name.Split(' ');
        if (arr.Length > 1)
        {
            var firstName = arr[0];
            var lastName = arr[1];
            return context.Students
                .Where(s => s.FirstName == firstName && s.LastName == lastName)
                .ToList();
        }

        return new List<Student>();
    }

    public List<string> GetCourseNames()
    {
        using var context = _dbFactory.CreateDbContext();
        return context.Courses.Select(c => c.CourseName).ToList();
    }
}
