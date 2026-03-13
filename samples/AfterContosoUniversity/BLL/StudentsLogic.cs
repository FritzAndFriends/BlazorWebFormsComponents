using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.BLL
{
    public class StudentsLogic
    {
        private readonly IDbContextFactory<ContosoUniversityEntities> _contextFactory;

        public StudentsLogic(IDbContextFactory<ContosoUniversityEntities> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public List<StudentListItem> GetJoinedTableData()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Enrollments
                .Include(e => e.Student)
                .GroupBy(e => new
                {
                    e.Student.StudentID,
                    e.Student.FirstName,
                    e.Student.LastName,
                    e.Student.Email,
                    e.Date
                })
                .Select(g => new
                {
                    ID = g.Key.StudentID,
                    Date = g.Key.Date,
                    FirstName = g.Key.FirstName,
                    LastName = g.Key.LastName,
                    Email = g.Key.Email,
                    Count = g.Count()
                })
                .AsEnumerable()
                .Select(e => new StudentListItem
                {
                    ID = e.ID,
                    Date = e.Date.ToShortDateString(),
                    FullName = $"{e.FirstName} {e.LastName}",
                    Email = e.Email,
                    Count = e.Count
                })
                .ToList();
        }

        public void UpdateStudentData(int id, string name, string? email = null)
        {
            using var context = _contextFactory.CreateDbContext();
            var student = context.Students.First(s => s.StudentID == id);

            if (name != null)
            {
                var arr = name.Split(' ');
                if (arr.Length > 1)
                {
                    student.FirstName = arr[0];
                    student.LastName = arr[1];
                    student.Email = email ?? student.Email;
                    context.SaveChanges();
                }
            }
        }

        public void DeleteStudent(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var student = context.Students.First(s => s.StudentID == id);
            context.Students.Remove(student);
            context.SaveChanges();
        }

        public void InsertNewEntry(string firstName, string lastName, DateTime birthDate, string course, string email = "Has not specified")
        {
            using var context = _contextFactory.CreateDbContext();

            var student = context.Students
                .FirstOrDefault(s => s.FirstName == firstName && s.LastName == lastName
                    && s.BirthDate == birthDate && s.Email == email);

            if (student == null)
            {
                student = new Student
                {
                    FirstName = firstName,
                    LastName = lastName,
                    BirthDate = birthDate,
                    Email = email
                };
                context.Students.Add(student);
                context.SaveChanges();
            }

            var courseEntity = context.Courses.FirstOrDefault(c => c.CourseName == course);
            if (courseEntity != null)
            {
                context.Enrollments.Add(new Enrollment
                {
                    StudentID = student.StudentID,
                    CourseID = courseEntity.CourseID,
                    Date = DateTime.Now
                });
                context.SaveChanges();
            }
        }

        public List<StudentInfo> GetStudentsByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return new List<StudentInfo>();

            using var context = _contextFactory.CreateDbContext();
            var arr = name.Split(' ');
            if (arr.Length < 2) return new List<StudentInfo>();

            var firstName = arr[0];
            var lastName = arr[1];

            return context.Students
                .Where(s => s.FirstName == firstName && s.LastName == lastName)
                .AsEnumerable()
                .Select(s => new StudentInfo
                {
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    BirthDate = s.BirthDate.ToShortDateString(),
                    StudentID = s.StudentID
                })
                .ToList();
        }

        public List<string> GetCourseNames()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Courses.Select(c => c.CourseName).ToList();
        }
    }
}
