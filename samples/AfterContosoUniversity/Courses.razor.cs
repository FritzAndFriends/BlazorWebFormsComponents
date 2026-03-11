using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.BLL;
using ContosoUniversity.Models;

namespace ContosoUniversity;

public partial class Courses : ComponentBase
{
    [Inject]
    public ContosoUniversityEntities? Db { get; set; }

    private Courses_Logic? coursLogic;

    public List<Department> Departments { get; set; } = new();
    public List<Cours> CoursesByDepartment { get; set; } = new();
    public List<Cours> CoursesByName { get; set; } = new();
    public string SelectedDepartment { get; set; } = string.Empty;
    public string CourseSearchText { get; set; } = string.Empty;
    private Cours? SelectedCourse { get; set; }

    protected override async Task OnInitializedAsync()
    {
        coursLogic = new Courses_Logic();

        if (Db != null)
        {
            Departments = await Db.Departments.ToListAsync();
        }
    }

    public async Task<List<string>> GetCourseAutocomplete(string prefixText)
    {
        if (Db == null) return new List<string>();

        return await Db.Courses
            .Where(c => c.CourseName != null && c.CourseName.StartsWith(prefixText))
            .Select(c => c.CourseName!)
            .ToListAsync();
    }

    public void SearchCoursesByDepartment()
    {
        if (coursLogic != null && !string.IsNullOrEmpty(SelectedDepartment))
        {
            CoursesByDepartment = coursLogic.GetCourses(SelectedDepartment).ToList();
        }
    }

    public void SearchCoursesByName()
    {
        if (coursLogic != null && !string.IsNullOrEmpty(CourseSearchText))
        {
            CoursesByName = coursLogic.GetCourse(CourseSearchText).ToList();
            CourseSearchText = string.Empty;
        }
    }

    public void OnPageIndexChanging(int newPageIndex)
    {
        // Page index handling for GridView - implement pagination logic here
        SearchCoursesByDepartment();
    }

    private void btnSearchCourse_Click() { SearchCoursesByDepartment(); }
    private void search_Click() { SearchCoursesByName(); }
}
