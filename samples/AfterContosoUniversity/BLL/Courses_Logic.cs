using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.BLL
{
    public class Courses_Logic
    {
        private readonly IDbContextFactory<ContosoUniversityEntities> _factory;

        public Courses_Logic(IDbContextFactory<ContosoUniversityEntities> factory)
        {
            _factory = factory;
        }

        public List<Cours> GetCourses(string department)
        {
            using var db = _factory.CreateDbContext();
            return db.Courses
                .Include(c => c.Department)
                .Where(c => c.Department.DepartmentName == department)
                .ToList();
        }

        public List<Cours> GetCourse(string courseName)
        {
            using var db = _factory.CreateDbContext();
            return db.Courses
                .Where(c => c.CourseName == courseName)
                .ToList();
        }
    }
}
