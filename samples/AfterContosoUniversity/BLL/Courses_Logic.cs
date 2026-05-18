using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.BLL
{
    public class Courses_Logic
    {
        private readonly ContosoUniversityEntities _context;

        public Courses_Logic(ContosoUniversityEntities context)
        {
            _context = context;
        }

        #region Get Courses By Department
        public List<Cours> GetCourses(string department)
        {
            var courses = (from cours in _context.Courses.Include(c => c.Department)
                           where cours.Department.DepartmentName == department
                           select cours).ToList<Cours>();

            return courses;
        }
        #endregion

        #region Get Course By CourseName
        public List<Cours> GetCourse(string courseName)                   
        {
            var course = (from crs in _context.Courses
                          where crs.CourseName == courseName
                          select crs).ToList<Cours>();

            return course;
        }   
        #endregion
    }
}