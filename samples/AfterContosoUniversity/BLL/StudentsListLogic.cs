using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.BLL
{
    public class StudentsListLogic
    {
        private readonly ContosoUniversityEntities _context;

        public StudentsListLogic(ContosoUniversityEntities context)
        {
            _context = context;
        }

        public List<Object> GetJoinedTableData()
        {
            List<Object> newTab = new List<object>();

            var joinedTab = _context.Students
                .Join(_context.Enrollments, stud => stud.StudentID, enr => enr.StudentID, (stud, enr) => enr);

            var groupedTab = from tab in joinedTab.Include(e => e.Student)
                             group tab by new { tab.Date, tab.Student.LastName, tab.Student.FirstName, tab.Student.Email, tab.Student.StudentID } into grpTab
                             select new { ID = grpTab.Key.StudentID, Date = grpTab.Key.Date, FirstName = grpTab.Key.FirstName, LastName = grpTab.Key.LastName, Email = grpTab.Key.Email, Count = grpTab.Count() };

            foreach (var entry in groupedTab.AsEnumerable())
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
            Student student = _context.Students.First(stud => stud.StudentID == id);

            if (name != null)
            {
                string[] arr = name.Split(' ');
                if (arr.Length > 1)
                {
                    student.FirstName = arr[0];
                    student.LastName = arr[1];
                    student.Email = email;
                    _context.SaveChanges();
                }
            }
        }

        public void DeleteStudent(int id)
        {
            _context.Students.Remove(_context.Students.First(stud => stud.StudentID == id));
            _context.SaveChanges();
        }

        public void InsertNewEntry(string firstName, string lastName, DateTime birthDate, string course, string email = "Has not specified")
        {
            int courseId, studentId = 0;

            var student = (from stud in _context.Students
                           where stud.FirstName == firstName && stud.LastName == lastName && stud.BirthDate == birthDate && stud.Email == email
                           select stud).FirstOrDefault();

            if (student == null)
            {
                AddingNewStudent(firstName, lastName, birthDate, email);
                studentId = _context.Students.OrderByDescending(s => s.StudentID).First().StudentID;
                courseId = _context.Courses.First(c => c.CourseName == course).CourseID;
                AddingNewEnrollment(studentId, courseId);
            }
            else
            {
                studentId = student.StudentID;
                courseId = _context.Courses.First(c => c.CourseName == course).CourseID;
                AddingNewEnrollment(studentId, courseId);
            }
        }

        private void AddingNewStudent(string firstName, string lastName, DateTime birthDate, string email = "Has not specified")
        {
            Student newStudent = new Student();
            newStudent.FirstName = firstName;
            newStudent.LastName = lastName;
            newStudent.BirthDate = birthDate;
            newStudent.Email = email;
            _context.Students.Add(newStudent);
            _context.SaveChanges();
        }

        private void AddingNewEnrollment(int studentId, int courseId)
        {
            Enrollment newEnrollment = new Enrollment();
            newEnrollment.CourseID = courseId;
            newEnrollment.Date = DateTime.Now;
            newEnrollment.StudentID = studentId;
            _context.Enrollments.Add(newEnrollment);
            _context.SaveChanges();
        }

        public List<object> GetStudents(string name)
        {
            string[] arr;
            List<object> studentsInfo = new List<object>();

            if (!String.IsNullOrEmpty(name))
            {
                arr = name.Split(' ');
                if (arr.Length > 1)
                {
                    var firstName = arr[0];
                    var lastName = arr[1];

                    var students = (from stud in _context.Students
                                    where stud.FirstName == firstName && stud.LastName == lastName
                                    select stud).ToList<Student>();

                    if (students != null)
                    {
                        foreach (Student stud in students)
                        {
                            var studentObj = new
                            {
                                FirstName = stud.FirstName,
                                LastName = stud.LastName,
                                Email = stud.Email,
                                BirthDate = stud.BirthDate.ToShortDateString(),
                                StudentID = stud.StudentID
                            };
                            studentsInfo.Add(studentObj);
                        }
                    }
                }
            }
            return studentsInfo;
        }
    }
}