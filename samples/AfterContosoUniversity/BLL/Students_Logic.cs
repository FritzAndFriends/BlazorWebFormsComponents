// Business logic  migrated from Web Forms to use injected DbContext

using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Models;

namespace ContosoUniversity.Bll
{
    public class StudentsListLogic
    {
        private readonly ContosoUniversityEntities _db;
        public StudentsListLogic(ContosoUniversityEntities db) { _db = db; }

        public List<object> GetJoinedTableData()
        {
            var newTab = new List<object>();

            var joinedTab = _db.Students.Join(_db.Enrollments, stud => stud.StudentID, enr => enr.StudentID, (stud, enr) => enr);

            var groupedTab = from tab in joinedTab
                             group tab by new { tab.Date, tab.Student.LastName, tab.Student.FirstName, tab.Student.Email, tab.Student.StudentID } into grpTab
                             select new { ID = grpTab.Key.StudentID, Date = grpTab.Key.Date, FirstName = grpTab.Key.FirstName, LastName = grpTab.Key.LastName, Email = grpTab.Key.Email, Count = grpTab.Select(cache => cache).Count() };

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
            var student = _db.Students.First(stud => stud.StudentID == id);

            if (name != null)
            {
                var arr = name.Split(' ');
                if (arr.Length > 1)
                {
                    student.FirstName = arr[0];
                    student.LastName = arr[1];
                    student.Email = email;
                    _db.SaveChanges();
                }
            }
        }

        public void DeleteStudent(int id)
        {
            _db.Students.Remove(_db.Students.First(stud => stud.StudentID == id));
            _db.SaveChanges();
        }

        public void InsertNewEntry(string firstName, string lastName, DateTime birthDate, string course, string email = "Has not specified")
        {
            int courseId, studentId = 0;

            var student = (from stud in _db.Students
                           where stud.FirstName == firstName && stud.LastName == lastName && stud.BirthDate == birthDate && stud.Email == email
                           select stud).FirstOrDefault();

            if (student == null)
            {
                AddingNewStudent(firstName, lastName, birthDate, email);
                foreach (var stud in _db.Students) { studentId = stud.StudentID; }
                courseId = (from crs in _db.Courses where crs.CourseName == course select crs).FirstOrDefault().CourseID;
                AddingNewEnrollment(studentId, courseId);
            }
            else
            {
                studentId = student.StudentID;
                courseId = (from crs in _db.Courses where crs.CourseName == course select crs).FirstOrDefault().CourseID;
                AddingNewEnrollment(studentId, courseId);
            }
        }

        private void AddingNewStudent(string firstName, string lastName, DateTime birthDate, string email = "Has not specified")
        {
            var newStudent = new Student
            {
                FirstName = firstName,
                LastName = lastName,
                BirthDate = birthDate,
                Email = email
            };
            _db.Students.Add(newStudent);
            _db.SaveChanges();
        }

        private void AddingNewEnrollment(int studentId, int courseId)
        {
            var newEnrollment = new Enrollment
            {
                CourseID = courseId,
                Date = DateTime.Now,
                StudentID = studentId
            };
            _db.Enrollments.Add(newEnrollment);
            _db.SaveChanges();
        }

        public List<object> GetStudents(string name)
        {
            var studentsInfo = new List<object>();

            if (!string.IsNullOrEmpty(name))
            {
                var arr = name.Split(' ');
                if (arr.Length > 1)
                {
                    var firstName = arr[0];
                    var lastName = arr[1];

                    var students = (from stud in _db.Students
                                    where stud.FirstName == firstName && stud.LastName == lastName
                                    select stud).ToList();

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
    }
}