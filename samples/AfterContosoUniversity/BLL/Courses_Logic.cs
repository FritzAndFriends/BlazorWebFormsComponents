using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Models;

namespace ContosoUniversity.BLL
{
    public class Courses_Logic
    {
        private readonly ContosoUniversityEntities _db;

        public Courses_Logic(ContosoUniversityEntities db)
        {
            _db = db;
        }

        #region Get Courses By Department
        public List<Cours> GetCourses(string department)
        {
            var courses = (from cours in _db.Courses
                           where cours.Department.DepartmentName == department
                           select cours).ToList<Cours>();

            return courses;
        }
        #endregion

        #region Get Course By CourseName
        public List<Cours> GetCourse(string courseName)                   
        {
            var course = (from crs in _db.Courses
                          where crs.CourseName == courseName
                          select crs).ToList<Cours>();

            return course;
        }   
        #endregion
    }
}