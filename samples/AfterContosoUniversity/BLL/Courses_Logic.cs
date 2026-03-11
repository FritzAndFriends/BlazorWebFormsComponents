using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.BLL;

public class Courses_Logic
{
    private readonly IDbContextFactory<ContosoUniversityContext> _dbFactory;

    public Courses_Logic(IDbContextFactory<ContosoUniversityContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public List<Cours> GetCourses(string department)
    {
        using var context = _dbFactory.CreateDbContext();
        return context.Courses
            .Include(c => c.Department)
            .Where(c => c.Department != null && c.Department.DepartmentName == department)
            .ToList();
    }

    public List<Cours> GetCourse(string courseName)
    {
        using var context = _dbFactory.CreateDbContext();
        return context.Courses
            .Where(c => c.CourseName == courseName)
            .ToList();
    }

    public List<string> GetDepartmentNames()
    {
        using var context = _dbFactory.CreateDbContext();
        return context.Departments.Select(d => d.DepartmentName).ToList();
    }
}
