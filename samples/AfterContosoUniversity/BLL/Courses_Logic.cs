// Business logic — migrated from Web Forms to use injected DbContext

using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Models;

namespace ContosoUniversity.BLL
{
    public class Courses_Logic
    {
        private readonly ContosoUniversityEntities _db;
        public Courses_Logic(ContosoUniversityEntities db) { _db = db; }

        public List<Cours> GetCourses(string department)
        {
            return (from cours in _db.Courses
                    where cours.Department.DepartmentName == department
                    select cours).ToList();
        }

        public List<Cours> GetCourse(string courseName)                   
        {
            return (from crs in _db.Courses
                    where crs.CourseName == courseName
                    select crs).ToList();
        }   
    }
}
