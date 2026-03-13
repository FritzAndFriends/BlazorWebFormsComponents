using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.Bll
{
    public class StudentsListLogic
    {
        private readonly IDbContextFactory<ContosoUniversityEntities> _factory;

        public StudentsListLogic(IDbContextFactory<ContosoUniversityEntities> factory)
        {
            _factory = factory;
        }

        public List<object> GetJoinedTableData()
        {
            using var db = _factory.CreateDbContext();
            var newTab = new List<object>();

            var joinedTab = db.Students
                .Include(s => s.Enrollments)
                .SelectMany(s => s.Enrollments.Select(e => new { Student = s, Enrollment = e }));

            var groupedTab = from tab in joinedTab
                             group tab by new { tab.Enrollment.Date, tab.Student.LastName, tab.Student.FirstName, tab.Student.Email, tab.Student.StudentID } into grpTab
                             select new { ID = grpTab.Key.StudentID, Date = grpTab.Key.Date, FirstName = grpTab.Key.FirstName, LastName = grpTab.Key.LastName, Email = grpTab.Key.Email, Count = grpTab.Count() };

            foreach (var entry in groupedTab.ToList())
            {
                newTab.Add(new
                {
                    ID = entry.ID,
                    Date = entry.Date.ToShortDateString(),
                    FullName = string.Format("{0} {1}", entry.FirstName, entry.LastName),
                    Email = entry.Email,
                    Count = entry.Count,
                });
            }

            return newTab;
        }

        public void UpdateStudentData(int id, string name, string email = null)
        {
            using var db = _factory.CreateDbContext();
            var student = db.Students.First(stud => stud.StudentID == id);

            if (name != null)
            {
                var arr = name.Split(' ');
                if (arr.Length > 1)
                {
                    student.FirstName = arr[0];
                    student.LastName = arr[1];
                    student.Email = email;
                    db.SaveChanges();
                }
            }
        }

        public void DeleteStudent(int id)
        {
            using var db = _factory.CreateDbContext();
            db.Students.Remove(db.Students.First(stud => stud.StudentID == id));
            db.SaveChanges();
        }

        public void InsertNewEntry(string firstName, string lastName, DateTime birthDate, string course, string email = "Has not specified")
        {
            using var db = _factory.CreateDbContext();
            int courseId, studentId = 0;

            var student = db.Students
                .FirstOrDefault(stud => stud.FirstName == firstName && stud.LastName == lastName && stud.BirthDate == birthDate && stud.Email == email);

            if (student == null)
            {
                var newStudent = new Student
                {
                    FirstName = firstName,
                    LastName = lastName,
                    BirthDate = birthDate,
                    Email = email
                };
                db.Students.Add(newStudent);
                db.SaveChanges();
                studentId = newStudent.StudentID;
            }
            else
            {
                studentId = student.StudentID;
            }

            var courseEntity = db.Courses.FirstOrDefault(crs => crs.CourseName == course);
            if (courseEntity != null)
            {
                courseId = courseEntity.CourseID;
                var newEnrollment = new Enrollment
                {
                    CourseID = courseId,
                    Date = DateTime.Now,
                    StudentID = studentId
                };
                db.Enrollments.Add(newEnrollment);
                db.SaveChanges();
            }
        }

        public List<object> GetStudents(string name)
        {
            using var db = _factory.CreateDbContext();
            var studentsInfo = new List<object>();

            if (!string.IsNullOrEmpty(name))
            {
                var arr = name.Split(' ');
                if (arr.Length > 1)
                {
                    var firstName = arr[0];
                    var lastName = arr[1];

                    var students = db.Students
                        .Where(stud => stud.FirstName == firstName && stud.LastName == lastName)
                        .ToList();

                    foreach (var stud in students)
                    {
                        studentsInfo.Add(new
                        {
                            FirstName = stud.FirstName,
                            LastName = stud.LastName,
                            Email = stud.Email,
                            BirthDate = stud.BirthDate.ToShortDateString(),
                            StudentID = stud.StudentID
                        });
                    }
                }
            }
            return studentsInfo;
        }

        public List<string> GetCourseNames()
        {
            using var db = _factory.CreateDbContext();
            return db.Courses.Select(c => c.CourseName).ToList();
        }

        public List<string> GetStudentAutoComplete(string prefixText)
        {
            using var db = _factory.CreateDbContext();
            return db.Students
                .Where(s => s.FirstName.StartsWith(prefixText))
                .Select(s => s.FirstName + " " + s.LastName)
                .Take(20)
                .ToList();
        }

        public List<string> GetDepartmentNames()
        {
            using var db = _factory.CreateDbContext();
            return db.Departments.Select(d => d.DepartmentName).ToList();
        }

        public List<string> GetCourseAutoComplete(string prefixText)
        {
            using var db = _factory.CreateDbContext();
            return db.Courses
                .Where(c => c.CourseName.StartsWith(prefixText))
                .Select(c => c.CourseName)
                .Take(20)
                .ToList();
        }
    }
}
