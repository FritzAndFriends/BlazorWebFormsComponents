using ContosoUniversity.Models;
using ContosoUniversity.Bll;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace ContosoUniversity;

public partial class Students : BlazorWebFormsComponents.WebFormsPageBase
{
    [Inject] private StudentsListLogic studLogic { get; set; } = default!;
    [Inject] private ContosoUniversityEntities _db { get; set; } = default!;

    private GridView<object> grv = default!;
    private TextBox txtFirstName = default!;
    private TextBox txtLastName = default!;
    private TextBox txtBirthDate = default!;
    private TextBox txtEmail = default!;
    private DropDownList<string> dropListCourses = default!;
    private TextBox txtSearch = default!;
    private DetailsView<object> studentData = default!;

    private List<string> _courseNames = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _courseNames = _db.Courses.Select(c => c.CourseName).ToList();
    }

    public List<object> grv_GetData()
    {
        return studLogic.GetJoinedTableData();
    }

    private void btnInsert_Click()
    {
        DateTime birth;
        try
        {
            birth = DateTime.Parse(txtBirthDate.Text);
        }
        catch
        {
            return;
        }

        studLogic.InsertNewEntry(txtFirstName.Text, txtLastName.Text, birth, dropListCourses.SelectedValue, txtEmail.Text);
        grv.DataSource = studLogic.GetJoinedTableData();
    }

    private void btnClear_Click()
    {
        txtFirstName.Text = string.Empty;
        txtLastName.Text = string.Empty;
        txtBirthDate.Text = string.Empty;
        txtEmail.Text = string.Empty;
    }

    private void btnSearch_Click()
    {
        studentData.DataSource = studLogic.GetStudents(txtSearch.Text);
        txtSearch.Text = string.Empty;
    }
}
