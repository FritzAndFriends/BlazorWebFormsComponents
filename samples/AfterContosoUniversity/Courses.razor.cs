using ContosoUniversity.Models;
using ContosoUniversity.BLL;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace ContosoUniversity;

public partial class Courses : BlazorWebFormsComponents.WebFormsPageBase
{
    [Inject] private Courses_Logic coursLogic { get; set; } = default!;
    [Inject] private ContosoUniversityEntities _db { get; set; } = default!;

    private DropDownList<string> drpDepartments = default!;
    private GridView<Cours> grvCourses = default!;
    private TextBox txtCourse = default!;
    private DetailsView<Cours> dtlCourses = default!;

    private List<string> _departmentNames = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _departmentNames = _db.Departments.Select(d => d.DepartmentName).ToList();
    }

    private void btnSearchCourse_Click()
    {
        grvCourses.DataSource = coursLogic.GetCourses(drpDepartments.SelectedValue);
    }

    private void search_Click()
    {
        dtlCourses.DataSource = coursLogic.GetCourse(txtCourse.Text);
        txtCourse.Text = string.Empty;
    }
}
