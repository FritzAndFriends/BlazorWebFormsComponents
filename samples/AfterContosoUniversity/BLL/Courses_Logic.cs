using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.BLL
{
    public class Courses_Logic
    {
        private readonly IDbContextFactory<ContosoUniversityEntities> _dbFactory;

        public Courses_Logic(IDbContextFactory<ContosoUniversityEntities> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public List<Cours> GetCourses(string department)
        {
            using var context = _dbFactory.CreateDbContext();
            return context.Courses
                .Include(c => c.Department)
                .Where(c => c.Department.DepartmentName == department)
                .ToList();
        }

        public List<Cours> GetCourse(string courseName)
        {
            using var context = _dbFactory.CreateDbContext();
            return context.Courses
                .Where(c => c.CourseName == courseName)
                .ToList();
        }
    }
}

