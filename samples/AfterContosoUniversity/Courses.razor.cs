using ContosoUniversity.BLL;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Components;

namespace ContosoUniversity;

public partial class Courses : ComponentBase
{
    [Inject] private Courses_Logic CourseLogic { get; set; } = default!;

    private List<string> _departmentNames = new();
    private string _selectedDepartment = string.Empty;
    private List<Cours> _courses = new();
    private List<Cours> _searchResults = new();
    private string _searchText = string.Empty;

    protected override void OnInitialized()
    {
        _departmentNames = CourseLogic.GetDepartmentNames();
        if (_departmentNames.Count > 0)
            _selectedDepartment = _departmentNames[0];
    }

    private void HandleSearchByDepartment(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
    {
        _courses = CourseLogic.GetCourses(_selectedDepartment);
    }

    private void HandleSearchByName(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
    {
        _searchResults = CourseLogic.GetCourse(_searchText);
        _searchText = string.Empty;
    }
}

