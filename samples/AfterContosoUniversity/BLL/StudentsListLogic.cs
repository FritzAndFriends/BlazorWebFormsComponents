using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Bll
{
    public class StudentsListLogic
    {
        private readonly IDbContextFactory<ContosoUniversityEntities> _dbFactory;

        public StudentsListLogic(IDbContextFactory<ContosoUniversityEntities> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public List<object> GetJoinedTableData()
        {
            using var context = _dbFactory.CreateDbContext();
            var joinedTab = context.Students
                .Include(s => s.Enrollments)
                .SelectMany(s => s.Enrollments, (stud, enr) => new { stud, enr });

            var groupedTab = joinedTab
                .GroupBy(x => new { x.enr.Date, x.stud.LastName, x.stud.FirstName, x.stud.Email, x.stud.StudentID })
                .Select(g => new
                {
                    ID = g.Key.StudentID,
                    Date = g.Key.Date,
                    FirstName = g.Key.FirstName,
                    LastName = g.Key.LastName,
                    Email = g.Key.Email,
                    Count = g.Count()
                })
                .ToList();

            var newTab = new List<object>();
            foreach (var entry in groupedTab)
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
            using var context = _dbFactory.CreateDbContext();
            var student = context.Students.First(stud => stud.StudentID == id);

            if (name != null)
            {
                var arr = name.Split(' ');
                if (arr.Length > 1)
                {
                    student.FirstName = arr[0];
                    student.LastName = arr[1];
                    student.Email = email;
                    context.SaveChanges();
                }
            }
        }

        public void DeleteStudent(int id)
        {
            using var context = _dbFactory.CreateDbContext();
            var student = context.Students
                .Include(s => s.Enrollments)
                .First(stud => stud.StudentID == id);
            context.Enrollments.RemoveRange(student.Enrollments);
            context.Students.Remove(student);
            context.SaveChanges();
        }

        public void InsertNewEntry(string firstName, string lastName, DateTime birthDate, string course, string email = "Has not specified")
        {
            using var context = _dbFactory.CreateDbContext();

            var student = context.Students
                .FirstOrDefault(s => s.FirstName == firstName && s.LastName == lastName && s.BirthDate == birthDate && s.Email == email);

            int studentId;
            if (student == null)
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
                studentId = student.StudentID;
            }

            var courseEntity = context.Courses.FirstOrDefault(c => c.CourseName == course);
            if (courseEntity != null)
            {
                var newEnrollment = new Enrollment
                {
                    CourseID = courseEntity.CourseID,
                    Date = DateTime.Now,
                    StudentID = studentId
                };
                context.Enrollments.Add(newEnrollment);
                context.SaveChanges();
            }
        }

        public List<object> GetStudents(string name)
        {
            using var context = _dbFactory.CreateDbContext();
            var studentsInfo = new List<object>();

            if (!string.IsNullOrEmpty(name))
            {
                var arr = name.Split(' ');
                if (arr.Length > 1)
                {
                    var firstName = arr[0];
                    var lastName = arr[1];

                    var students = context.Students
                        .Where(s => s.FirstName == firstName && s.LastName == lastName)
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

        public List<string> SearchStudentNames(string prefixText)
        {
            using var context = _dbFactory.CreateDbContext();
            return context.Students
                .Where(s => s.FirstName.StartsWith(prefixText))
                .Select(s => s.FirstName + " " + s.LastName)
                .Take(20)
                .ToList();
        }
    }
}

