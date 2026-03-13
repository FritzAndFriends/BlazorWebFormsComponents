using ContosoUniversity.BLL;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity;

public partial class Courses
{
    [Inject] private IDbContextFactory<ContosoUniversityEntities> DbFactory { get; set; } = default!;

    private Courses_Logic? coursLogic;
    private List<string> _departmentNames = new();
    private List<object> _courses = new();
    private List<object> _courseDetail = new();
    private string _selectedDepartment = "";
    private string _courseSearchText = "";

    protected override async Task OnInitializedAsync()
    {
        await using var db = await DbFactory.CreateDbContextAsync();
        coursLogic = new Courses_Logic(db);

        _departmentNames = db.Departments.Select(d => d.DepartmentName).ToList();
    }

    private void HandleSearchCourse()
    {
        if (coursLogic != null)
        {
            _courses = coursLogic.GetCourses(_selectedDepartment).Cast<object>().ToList();
        }
    }

    private void HandleCourseByName()
    {
        if (coursLogic != null && !string.IsNullOrEmpty(_courseSearchText))
        {
            _courseDetail = coursLogic.GetCourse(_courseSearchText).Cast<object>().ToList();
            _courseSearchText = string.Empty;
        }
    }

    // TODO: Wire _courses and _courseDetail to GridView/DetailsView Items parameters
    // TODO: AutoComplete search was a Web Service (ASMX WebMethod) — convert to component-local search
    // TODO: Paging via GridView PageIndexChanging event
}
