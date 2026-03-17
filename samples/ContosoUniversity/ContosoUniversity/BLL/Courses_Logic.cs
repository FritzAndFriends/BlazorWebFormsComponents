using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ContosoUniversity.Models;

namespace ContosoUniversity.BLL
{
    public class Courses_Logic
    {
        #region Get Courses By Department
        public List<Cours> GetCourses(string department)
        {
            var courses = (from cours in new ContosoUniversityEntities().Courses
                           where cours.Department.DepartmentName == department
                           select cours).ToList<Cours>();

            return courses;
        }
        #endregion

        #region Get Course By CourseName
        public List<Cours> GetCourse(string courseName)                   
        {
            var course = (from crs in new ContosoUniversityEntities().Courses
                          where crs.CourseName == courseName
                          select crs).ToList<Cours>();

            return course;
        }   
        #endregion
    }
}