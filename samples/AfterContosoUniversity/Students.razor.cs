using System.Data;
using System.Data.SqlClient;
using BlazorAjaxToolkitComponents;
using ConfigurationManager = BlazorWebFormsComponents.ConfigurationManager;
using ContosoUniversity.BLL;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Components;

namespace ContosoUniversity;

public partial class Students : WebFormsPageBase
{
    [SupplyParameterFromQuery(Name = "editId")]
    public int? EditId { get; set; }

    [SupplyParameterFromQuery(Name = "deleteId")]
    public int? DeleteId { get; set; }

    [SupplyParameterFromQuery(Name = "saveEdit")]
    public bool? SaveEdit { get; set; }

    [SupplyParameterFromQuery(Name = "editedName")]
    public string? EditedName { get; set; }

    [SupplyParameterFromQuery(Name = "editedEmail")]
    public string? EditedEmail { get; set; }

    private AutoCompleteExtender AutoCompleteExtender1 = default!;
    private ScriptManager ScriptManager2 = default!;
    private Table tabAddStud = default!;

    private List<Cours> _availableCourses = [];
    private List<StudentGridRow> _studentGridData = [];
    private int? _editingStudentId;
    private string _editFullName = string.Empty;
    private string _editEmail = string.Empty;
    private string _selectedCourseName = string.Empty;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _birthDateText = string.Empty;
    private string _email = string.Empty;
    private string _searchText = string.Empty;
    private List<StudentSearchRow> _studentSearchResults = [];
    private string? _lastHandledQueryAction;

    [Inject]
    protected StudentsListLogic studLogic { get; set; } = default!;

    [Inject]
    protected ContosoUniversityEntities _contosoUniversityEntities { get; set; } = default!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        LoadPageData();
        HandleQueryActions();
        ApplyEditStateFromQuery();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        LoadPageData();
        HandleQueryActions();
        ApplyEditStateFromQuery();
    }

    public IQueryable<StudentGridRow> grv_GetData() => _studentGridData.AsQueryable();

    public void grv_DeleteItem(int id)
    {
        studLogic.DeleteStudent(id);
        _editingStudentId = null;
        RefreshStudentGrid();
        _studentSearchResults = [];
    }

    protected void btnInsert_Click(EventArgs e)
    {
        if (!DateTime.TryParse(_birthDateText, out var birth))
        {
            throw new Exception("Wrong Date Format !!!");
        }

        var selectedCourse = _selectedCourseName;

        if (string.IsNullOrWhiteSpace(selectedCourse))
        {
            selectedCourse = _availableCourses.FirstOrDefault()?.CourseName;
        }

        if (string.IsNullOrWhiteSpace(selectedCourse))
        {
            return;
        }

        studLogic.InsertNewEntry(
            _firstName,
            _lastName,
            birth,
            selectedCourse,
            _email);

        RefreshStudentGrid();
        btnClear_Click(EventArgs.Empty);
    }

    protected void btnClear_Click(EventArgs e)
    {
        _firstName = string.Empty;
        _lastName = string.Empty;
        _birthDateText = string.Empty;
        _email = string.Empty;
        _selectedCourseName = _availableCourses.FirstOrDefault()?.CourseName ?? string.Empty;
    }

    public static List<string> GetCompletionList(string prefixText, int count)
    {
        List<string> students = [];
        string conString = ConfigurationManager.ConnectionStrings["ContosoUniversity"].ConnectionString;
        const string sqlQuery = "SELECT FirstName,LastName FROM dbo.Students WHERE FirstName LIKE @SearchText + '%'";

        using (SqlConnection con = new(conString))
        using (SqlCommand cmd = new(sqlQuery, con))
        {
            SqlParameter param = new()
            {
                ParameterName = "@SearchText",
                Value = prefixText,
                SqlDbType = SqlDbType.NVarChar,
                Size = 30,
                Direction = ParameterDirection.Input
            };

            cmd.Parameters.Add(param);
            con.Open();

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                students.Add($"{dr["FirstName"]} {dr["LastName"]}");
            }
        }

        return students;
    }

    protected void btnSearch_Click(EventArgs e)
    {
        _studentSearchResults = studLogic.GetStudents(_searchText)
            .Select(item => new StudentSearchRow
            {
                FirstName = item.GetType().GetProperty("FirstName")?.GetValue(item)?.ToString() ?? string.Empty,
                LastName = item.GetType().GetProperty("LastName")?.GetValue(item)?.ToString() ?? string.Empty,
                Email = item.GetType().GetProperty("Email")?.GetValue(item)?.ToString() ?? string.Empty,
                BirthDate = item.GetType().GetProperty("BirthDate")?.GetValue(item)?.ToString() ?? string.Empty,
                StudentID = Convert.ToInt32(item.GetType().GetProperty("StudentID")?.GetValue(item) ?? 0)
            })
            .ToList();
        _searchText = string.Empty;
    }

    public void grv_UpdateItem(int id)
    {
        RefreshStudentGrid();
    }

    private void BeginEdit(StudentGridRow student)
    {
        NavigationManager.NavigateTo($"/Students?editId={student.ID}", forceLoad: false);
    }

    private void CancelEdit()
    {
        _editingStudentId = null;
        _editFullName = string.Empty;
        _editEmail = string.Empty;
    }

    private Task OnEditFullNameChanged(ChangeEventArgs e)
    {
        _editFullName = e.Value?.ToString() ?? string.Empty;
        return Task.CompletedTask;
    }

    private Task OnEditEmailChanged(ChangeEventArgs e)
    {
        _editEmail = e.Value?.ToString() ?? string.Empty;
        return Task.CompletedTask;
    }

    private Task SaveEditAsync()
    {
        if (_editingStudentId is int id)
        {
            studLogic.UpdateStudentData(id, _editFullName, _editEmail);
            RefreshStudentGrid();
        }

        CancelEdit();
        NavigationManager.NavigateTo("/Students", forceLoad: false);
        return Task.CompletedTask;
    }

    private Task DeleteStudentAsync(int id)
    {
        grv_DeleteItem(id);
        return Task.CompletedTask;
    }

    private void HandleQueryActions()
    {
        var actionSignature = $"edit:{EditId};delete:{DeleteId};save:{SaveEdit};name:{EditedName};email:{EditedEmail}";
        if (_lastHandledQueryAction == actionSignature)
        {
            return;
        }

        _lastHandledQueryAction = actionSignature;

        if (DeleteId is int deleteId)
        {
            grv_DeleteItem(deleteId);
            return;
        }

        if (SaveEdit == true && EditId is int editId)
        {
            studLogic.UpdateStudentData(editId, EditedName, EditedEmail);
            RefreshStudentGrid();
            CancelEdit();
        }
    }

    private void ApplyEditStateFromQuery()
    {
        if (DeleteId is not null || SaveEdit == true)
        {
            CancelEdit();
            return;
        }

        if (EditId is not int editId)
        {
            CancelEdit();
            return;
        }

        var student = _studentGridData.FirstOrDefault(item => item.ID == editId);
        if (student is null)
        {
            CancelEdit();
            return;
        }

        _editingStudentId = student.ID;
        _editFullName = student.FullName;
        _editEmail = student.Email;
    }

    private void LoadPageData()
    {
        _availableCourses = _contosoUniversityEntities.Courses
            .OrderBy(course => course.CourseName)
            .ToList();

        if (string.IsNullOrWhiteSpace(_selectedCourseName) && _availableCourses.Count > 0)
        {
            _selectedCourseName = _availableCourses[0].CourseName;
        }

        _studentSearchResults = [];
        RefreshStudentGrid();
    }

    private void RefreshStudentGrid()
    {
        _studentGridData = studLogic.GetJoinedTableData()
            .Select(item => new StudentGridRow
            {
                ID = Convert.ToInt32(item.GetType().GetProperty("ID")?.GetValue(item) ?? 0),
                FullName = item.GetType().GetProperty("FullName")?.GetValue(item)?.ToString() ?? string.Empty,
                Email = item.GetType().GetProperty("Email")?.GetValue(item)?.ToString() ?? string.Empty,
                Date = item.GetType().GetProperty("Date")?.GetValue(item)?.ToString() ?? string.Empty,
                Count = Convert.ToInt32(item.GetType().GetProperty("Count")?.GetValue(item) ?? 0)
            })
            .ToList();
    }

    private Task OnSelectedCourseChanged(string value)
    {
        _selectedCourseName = value;
        return Task.CompletedTask;
    }

    private Task OnSelectedCourseInput(ChangeEventArgs e) => OnSelectedCourseChanged(e.Value?.ToString() ?? string.Empty);

    private Task OnFirstNameChanged(string value)
    {
        _firstName = value;
        return Task.CompletedTask;
    }

    private Task OnFirstNameInput(ChangeEventArgs e) => OnFirstNameChanged(e.Value?.ToString() ?? string.Empty);

    private Task OnLastNameChanged(string value)
    {
        _lastName = value;
        return Task.CompletedTask;
    }

    private Task OnLastNameInput(ChangeEventArgs e) => OnLastNameChanged(e.Value?.ToString() ?? string.Empty);

    private Task OnBirthDateChanged(string value)
    {
        _birthDateText = value;
        return Task.CompletedTask;
    }

    private Task OnBirthDateInput(ChangeEventArgs e) => OnBirthDateChanged(e.Value?.ToString() ?? string.Empty);

    private Task OnEmailChanged(string value)
    {
        _email = value;
        return Task.CompletedTask;
    }

    private Task OnEmailInput(ChangeEventArgs e) => OnEmailChanged(e.Value?.ToString() ?? string.Empty);

    private Task OnSearchTextChanged(string value)
    {
        _searchText = value;
        return Task.CompletedTask;
    }

    private Task OnSearchTextInput(ChangeEventArgs e) => OnSearchTextChanged(e.Value?.ToString() ?? string.Empty);

    public sealed class StudentGridRow
    {
        public int ID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public sealed class StudentSearchRow
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
        public int StudentID { get; set; }
    }
}
