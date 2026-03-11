using ContosoUniversity.BLL;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Components;

namespace ContosoUniversity;

public partial class Students : ComponentBase
{
    [Inject] private StudentsListLogic StudentLogic { get; set; } = default!;

    private List<StudentViewModel> _studentData = new();
    private List<string> _courseNames = new();
    private List<Student> _searchResults = new();

    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _birthDate = string.Empty;
    private string _email = string.Empty;
    private string _selectedCourse = string.Empty;
    private string _searchText = string.Empty;

    protected override void OnInitialized()
    {
        _courseNames = StudentLogic.GetCourseNames();
        if (_courseNames.Count > 0)
            _selectedCourse = _courseNames[0];
        LoadStudentData();
    }

    private void LoadStudentData()
    {
        _studentData = StudentLogic.GetJoinedTableData();
    }

    private void HandleInsert(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
    {
        if (!DateTime.TryParse(_birthDate, out var birth))
            return;

        StudentLogic.InsertNewEntry(_firstName, _lastName, birth, _selectedCourse, _email);
        HandleClear(args);
        LoadStudentData();
    }

    private void HandleClear(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
    {
        _firstName = string.Empty;
        _lastName = string.Empty;
        _birthDate = string.Empty;
        _email = string.Empty;
    }

    private void HandleSearch(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
    {
        _searchResults = StudentLogic.GetStudents(_searchText);
        _searchText = string.Empty;
    }
}


