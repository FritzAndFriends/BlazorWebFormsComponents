using System.Data;
using System.Data.SqlClient;
using BlazorAjaxToolkitComponents;
using ConfigurationManager = BlazorWebFormsComponents.ConfigurationManager;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.BLL;

public class Courses_Logic
{
    private readonly ContosoUniversityEntities _contosoUniversityEntities;

    public Courses_Logic(ContosoUniversityEntities contosoUniversityEntities)
    {
        _contosoUniversityEntities = contosoUniversityEntities;
    }

    public List<Cours> GetCourses(string department)
    {
        if (string.IsNullOrWhiteSpace(department))
        {
            return [];
        }

        return _contosoUniversityEntities.Courses
            .AsNoTracking()
            .Include(course => course.Department)
            .Where(course => course.Department != null && course.Department.DepartmentName == department)
            .OrderBy(course => course.CourseID)
            .ToList();
    }

    public List<Cours> GetCourse(string courseName)
    {
        if (string.IsNullOrWhiteSpace(courseName))
        {
            return [];
        }

        courseName = courseName.Trim();

        return _contosoUniversityEntities.Courses
            .AsNoTracking()
            .Include(course => course.Department)
            .Where(course => course.CourseName.Contains(courseName))
            .OrderBy(course => course.CourseName)
            .ToList();
    }
}
