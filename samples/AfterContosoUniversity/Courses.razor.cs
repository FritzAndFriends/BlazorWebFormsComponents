using ContosoUniversity.Models;
using ContosoUniversity.BLL;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity;

public partial class Courses : BlazorWebFormsComponents.WebFormsPageBase
{
    [Inject] private Courses_Logic coursLogic { get; set; } = default!;
    [Inject] private ContosoUniversityEntities _context { get; set; } = default!;

    private DropDownList<string> drpDepartments = default!;
    private GridView<Cours> grvCourses = default!;
    private List<string> departments = new();
    private List<Cours> courseResults = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        departments = _context.Departments.Select(d => d.DepartmentName).ToList();
    }

    public IEnumerable<Cours> grvCourses_GetData()
    {
        return courseResults;
    }
}