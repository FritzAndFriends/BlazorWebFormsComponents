using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.BLL;

public class StudentsListLogic
{
    private readonly ContosoUniversityEntities contextObj;

    public StudentsListLogic(ContosoUniversityEntities contextObjParam)
    {
        contextObj = contextObjParam;
    }

    public List<object> GetJoinedTableData()
    {
        return contextObj.Students
            .AsNoTracking()
            .GroupJoin(
                contextObj.Enrollments.AsNoTracking(),
                student => student.StudentID,
                enrollment => enrollment.StudentID,
                (student, enrollments) => new { student, enrollments })
            .OrderBy(entry => entry.student.LastName)
            .ThenBy(entry => entry.student.FirstName)
            .AsEnumerable()
            .Select(entry =>
            {
                var latestEnrollment = entry.enrollments.OrderByDescending(enrollment => enrollment.Date).FirstOrDefault();

                return (object)new
                {
                    ID = entry.student.StudentID,
                    Date = latestEnrollment?.Date.ToShortDateString() ?? string.Empty,
                    FullName = $"{entry.student.FirstName} {entry.student.LastName}",
                    Email = entry.student.Email,
                    Count = entry.enrollments.Count()
                };
            })
            .ToList();
    }

    public void UpdateStudentData(int id, string? name, string? email = null)
    {
        Student student = contextObj.Students.First(stud => stud.StudentID == id);

        if (!string.IsNullOrWhiteSpace(name))
        {
            string[] arr = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length > 0)
            {
                student.FirstName = arr[0];
                student.LastName = arr.Length > 1 ? string.Join(' ', arr.Skip(1)) : student.LastName;
            }
        }

        student.Email = email ?? string.Empty;
        contextObj.SaveChanges();
    }

    public void DeleteStudent(int id)
    {
        contextObj.Students.Remove(contextObj.Students.First(stud => stud.StudentID == id));
        contextObj.SaveChanges();
    }

    public void InsertNewEntry(string firstName, string lastName, DateTime birthDate, string course, string email = "Has not specified")
    {
        int courseId;
        int studentId;

        Student? student = contextObj.Students.FirstOrDefault(stud =>
            stud.FirstName == firstName &&
            stud.LastName == lastName &&
            stud.BirthDate == birthDate &&
            stud.Email == email);

        if (student is null)
        {
            student = AddingNewStudent(firstName, lastName, birthDate, email);
        }

        studentId = student.StudentID;
        courseId = contextObj.Courses.First(crs => crs.CourseName == course).CourseID;

        AddingNewEnrollment(studentId, courseId);
    }

    private Student AddingNewStudent(string firstName, string lastName, DateTime birthDate, string email = "Has not specified")
    {
        Student newStudent = new()
        {
            FirstName = firstName,
            LastName = lastName,
            BirthDate = birthDate,
            Email = email
        };

        contextObj.Students.Add(newStudent);
        contextObj.SaveChanges();
        return newStudent;
    }

    private void AddingNewEnrollment(int studentId, int courseId)
    {
        Enrollment newEnrollment = new()
        {
            CourseID = courseId,
            Date = DateTime.Now,
            StudentID = studentId
        };

        contextObj.Enrollments.Add(newEnrollment);
        contextObj.SaveChanges();
    }

    public List<object> GetStudents(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return [];
        }

        string[] parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        IQueryable<Student> query = contextObj.Students.AsNoTracking();

        if (parts.Length == 1)
        {
            string term = parts[0];
            query = query.Where(stud => stud.FirstName.Contains(term) || stud.LastName.Contains(term));
        }
        else
        {
            string firstName = parts[0];
            string lastName = string.Join(' ', parts.Skip(1));
            query = query.Where(stud => stud.FirstName.Contains(firstName) && stud.LastName.Contains(lastName));
        }

        return query
            .OrderBy(stud => stud.LastName)
            .ThenBy(stud => stud.FirstName)
            .Select(stud => (object)new
            {
                FirstName = stud.FirstName,
                LastName = stud.LastName,
                Email = stud.Email,
                BirthDate = stud.BirthDate.ToShortDateString(),
                StudentID = stud.StudentID
            })
            .ToList();
    }
}
