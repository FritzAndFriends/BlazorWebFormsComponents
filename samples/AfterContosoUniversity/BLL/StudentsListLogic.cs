using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Models;
using ContosoUniversity;
using Microsoft.EntityFrameworkCore;
namespace ContosoUniversity.Bll
{
    public class StudentsListLogic
    {
        private readonly ContosoUniversityEntities _db;

        public StudentsListLogic(ContosoUniversityEntities db)
        {
            _db = db;
        }

        #region GettingJoinedTables
        public List<Object> GetJoinedTableData()
        {
            List<Object> newTab = new List<object>();

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
        #endregion

        #region UpdateStudentData
        public void UpdateStudentData(int id, string name, string email = null)
        {
            Student student = _db.Students.First(stud => stud.StudentID == id);

            if (name != null)
            {
                string[] arr = name.Split(' ');

                if (arr.Length > 1)
                {
                    student.FirstName = arr[0];
                    student.LastName = arr[1];
                    student.Email = email;

                    _db.SaveChanges();
                }
            }
        }
        #endregion

        #region Delete Students
        public void DeleteStudent(int id)
        {
            _db.Students.Remove(_db.Students.First(stud => stud.StudentID == id));
            _db.SaveChanges();
        }
        #endregion

        #region Inserting new entry to Student and Enrollment Table
        public void InsertNewEntry(string firstName, string lastName, DateTime birthDate, string course, string email = "Has not specified")
        {
            int courseId, studentId = 0;

            Student newStudent = new Student();
            Enrollment newEnrollment = new Enrollment();

            var student = (from stud in _db.Students
                           where stud.FirstName == firstName && stud.LastName == lastName && stud.BirthDate == birthDate && stud.Email == email
                           select stud).FirstOrDefault();

            if (student as Student == null)
            {
                AddingNewStudent(firstName, lastName, birthDate, email);

                foreach (var stud in _db.Students)
                {
                    studentId = stud.StudentID;
                }
                courseId = (from crs in _db.Courses
                            where crs.CourseName == course
                            select crs).FirstOrDefault().CourseID;

                AddingNewEnrollment(studentId, courseId);
            }
            else
            {
                studentId = (student as Student).StudentID;

                courseId = (from crs in _db.Courses
                            where crs.CourseName == course
                            select crs).FirstOrDefault().CourseID;

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

            _db.Students.Add(newStudent);
            _db.SaveChanges();
        }

        private void AddingNewEnrollment(int studentId, int courseId)
        {
            Enrollment newEnrollment = new Enrollment();

            newEnrollment.CourseID = courseId;
            newEnrollment.Date = DateTime.Now;
            newEnrollment.StudentID = studentId;

            _db.Enrollments.Add(newEnrollment);
            _db.SaveChanges();
        }

        #endregion

        #region Getting Student Info
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

                    var students = (from stud in _db.Students
                                    where stud.FirstName == firstName && stud.LastName == lastName
                                    select stud).ToList<Student>();

                    if (students != null)
                    {
                        foreach (Student stud in students)
                        {
                            var student = new
                            {
                                FirstName = stud.FirstName,
                                LastName = stud.LastName,
                                Email = stud.Email,
                                BirthDate = stud.BirthDate.ToShortDateString(),
                                StudentID = stud.StudentID
                            };
                            studentsInfo.Add(student);
                        }
                    }
                }
            }
            return studentsInfo;
        }
        #endregion
    }
}
