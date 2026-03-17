using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ContosoUniversity.Models;
using ContosoUniversity;
using System.Web.UI.WebControls;

namespace ContosoUniversity.Bll
{
    public class StudentsListLogic
    {
        #region GettingJoinedTables
        public List<Object> GetJoinedTableData()
        {
            ContosoUniversityEntities contextObj = new ContosoUniversityEntities();
            List<Object> newTab = new List<object>();

            var joinedTab = contextObj.Students.Join(contextObj.Enrollments, stud => stud.StudentID, enr => enr.StudentID, (stud, enr) => enr); //Joining two tables by StudentID

            var groupedTab = from tab in joinedTab
                             group tab by new { tab.Date, tab.Student.LastName, tab.Student.FirstName, tab.Student.Email, tab.Student.StudentID } into grpTab
                             select new { ID = grpTab.Key.StudentID, Date = grpTab.Key.Date, FirstName = grpTab.Key.FirstName, LastName = grpTab.Key.LastName, Email = grpTab.Key.Email, Count = grpTab.Select(cache => cache).Count() };

            foreach (var entry in groupedTab)
            {
                newTab.Add(new    //Creating new anonymous object in order to change the date for ShortDateString
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
            ContosoUniversityEntities contextObj = new ContosoUniversityEntities();

            Student student = contextObj.Students.First(stud => stud.StudentID == id);

            if (name != null)
            {
                string[] arr = name.Split(' ');

                if (arr.Length > 1)
                {
                    student.FirstName = arr[0];
                    student.LastName = arr[1];
                    student.Email = email;

                    contextObj.SaveChanges();
                }
            }
        }
        #endregion

        #region Delete Students
        public void DeleteStudent(int id)
        {
            ContosoUniversityEntities contextObj = new ContosoUniversityEntities();
            contextObj.Students.Remove(contextObj.Students.First(stud => stud.StudentID == id));
            contextObj.SaveChanges();
        }
        #endregion

        #region Inserting new entry to Student and Enrollment Table
        public void InsertNewEntry(string firstName, string lastName, DateTime birthDate, string course, string email = "Has not specified")
        {
            int courseId, studentId = 0;

            Student newStudent = new Student();
            Enrollment newEnrollment = new Enrollment();

            ContosoUniversityEntities contextObj = new ContosoUniversityEntities();

            var student = (from stud in contextObj.Students
                           where stud.FirstName == firstName && stud.LastName == lastName && stud.BirthDate == birthDate && stud.Email == email
                           select stud).FirstOrDefault();

            //In case student doesn't exist
            if (student as Student == null)
            {
                AddingNewStudent(firstName, lastName, birthDate, email);

                //Getting the lastly added student ID
                foreach (var stud in contextObj.Students)
                {
                    studentId = stud.StudentID;
                }
                //Finding Course ID
                courseId = (from crs in contextObj.Courses
                            where crs.CourseName == course
                            select crs).FirstOrDefault().CourseID;

                AddingNewEnrollment(studentId, courseId);
            }

            //In case Student allready exist
            else
            {
                studentId = (student as Student).StudentID;

                //Finding Course ID
                courseId = (from crs in contextObj.Courses
                            where crs.CourseName == course
                            select crs).FirstOrDefault().CourseID;

                AddingNewEnrollment(studentId, courseId);
            }
        }

        private void AddingNewStudent(string firstName, string lastName, DateTime birthDate, string email = "Has not specified")
        {
            Student newStudent = new Student();
            ContosoUniversityEntities contextObj = new ContosoUniversityEntities();

            //Adding new Student                     
            newStudent.FirstName = firstName;
            newStudent.LastName = lastName;
            newStudent.BirthDate = birthDate;
            newStudent.Email = email;

            contextObj.Students.Add(newStudent);
            contextObj.SaveChanges();
        }

        private void AddingNewEnrollment(int studentId, int courseId)
        {
            Enrollment newEnrollment = new Enrollment();
            ContosoUniversityEntities contextObj = new ContosoUniversityEntities();

            //Adding new entry to Enrollment
            newEnrollment.CourseID = courseId;
            newEnrollment.Date = DateTime.Now;
            newEnrollment.StudentID = studentId;

            contextObj.Enrollments.Add(newEnrollment);
            contextObj.SaveChanges();

        }

        #endregion

        #region Getting Student Info
        public List<object> GetStudents (string name)
        {           
            ContosoUniversityEntities contextObj = new ContosoUniversityEntities();           
            string[] arr;
            List<object> studentsInfo = new List<object>();

            if (!String.IsNullOrEmpty(name))
            {
                arr = name.Split(' ');
                if (arr.Length > 1)
                {
                    var firstName = arr[0];
                    var lastName = arr[1];


                   var students = (from stud in contextObj.Students
                                   where stud.FirstName == firstName && stud.LastName == lastName
                                   select stud).ToList<Student>();

                    if (students != null)
                    {
                        foreach (Student stud in students)
                        {
                            var student = new  //Creating new anonymous object in order to change the date for ShortDateString
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