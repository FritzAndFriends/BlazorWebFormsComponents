using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.BLL
{
    public class CoursesLogic
    {
        private readonly IDbContextFactory<ContosoUniversityEntities> _contextFactory;

        public CoursesLogic(IDbContextFactory<ContosoUniversityEntities> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public List<Cours> GetCourses(string department)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Courses
                .Include(c => c.Department)
                .Where(c => c.Department.DepartmentName == department)
                .ToList();
        }

        public List<Cours> GetCourse(string courseName)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Courses
                .Where(c => c.CourseName == courseName)
                .ToList();
        }

        public List<string> GetDepartmentNames()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Departments.Select(d => d.DepartmentName).ToList();
        }

        public List<string> GetCourseNameSuggestions(string prefix)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Courses
                .Where(c => c.CourseName.StartsWith(prefix))
                .Select(c => c.CourseName)
                .Take(20)
                .ToList();
        }
    }
}
