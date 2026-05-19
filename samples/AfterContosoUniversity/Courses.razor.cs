using System.Data;
using System.Data.SqlClient;
using BlazorAjaxToolkitComponents;
using ConfigurationManager = BlazorWebFormsComponents.ConfigurationManager;
using ContosoUniversity.BLL;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Components;

namespace ContosoUniversity;

public partial class Courses : WebFormsPageBase
{
    private AutoCompleteExtender AutoCompleteExtender1 = default!;
    private Button btnSearchCourse = default!;
    private DropDownList<Department> drpDepartments = default!;
    private DetailsView<Cours> dtlCourses = default!;
    private GridView<Cours> grvCourses = default!;
    private Button search = default!;
    private TextBox txtCourse = default!;

    private List<Department> _departments = [];
    private List<Cours> _courses = [];
    private IEnumerable<Cours> _courseDetails = Array.Empty<Cours>();
    private string _selectedDepartment = string.Empty;

    [Inject]
    protected Courses_Logic coursLogic { get; set; } = default!;

    [Inject]
    protected ContosoUniversityEntities _contosoUniversityEntities { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _departments = _contosoUniversityEntities.Departments
            .OrderBy(department => department.DepartmentName)
            .ToList();

        if (string.IsNullOrEmpty(_selectedDepartment) && _departments.Count > 0)
        {
            _selectedDepartment = _departments[0].DepartmentName;
        }
    }

    public static List<string> GetList(string prefixText, int count)
    {
        List<string> name = [];

        const string query = "select CourseName from dbo.[Courses] where CourseName like @SearchText + '%'";
        string connectionStr = ConfigurationManager.ConnectionStrings["ContosoUniversity"].ConnectionString;

        using (SqlConnection con = new(connectionStr))
        using (SqlCommand cmd = new(query, con))
        {
            SqlParameter param = new()
            {
                Direction = ParameterDirection.Input,
                DbType = DbType.String,
                ParameterName = "@SearchText",
                Value = prefixText
            };

            cmd.Parameters.Add(param);
            con.Open();

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                name.Add(dr["CourseName"].ToString() ?? string.Empty);
            }
        }

        return name;
    }

    protected void btnSearchCourse_Click(EventArgs e)
    {
        _courses = string.IsNullOrWhiteSpace(_selectedDepartment)
            ? []
            : coursLogic.GetCourses(_selectedDepartment);
    }

    protected void search_Click(EventArgs e)
    {
        _courseDetails = coursLogic.GetCourse(txtCourse.Text);
        txtCourse.Text = string.Empty;
    }

    protected void grvCourses_PageIndexChanging(PageChangedEventArgs e)
    {
        grvCourses.PageIndex = e.NewPageIndex;
        btnSearchCourse_Click(EventArgs.Empty);
    }

    private Task OnSelectedDepartmentChanged(string value)
    {
        _selectedDepartment = value;
        return Task.CompletedTask;
    }
}
