using ContosoUniversity.Models;
using ContosoUniversity.Bll;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity;

public partial class Students
{
    [Inject] private IDbContextFactory<ContosoUniversityEntities> DbFactory { get; set; } = default!;

    private StudentsListLogic? studLogic;
    private List<object> _students = new();
    private List<object> _studentDetail = new();
    private List<string> _courseNames = new();
    private string _searchText = "";

    // Form fields
    private string _firstName = "";
    private string _lastName = "";
    private string _birthDate = "";
    private string _email = "";
    private string _selectedCourse = "";

    protected override async Task OnInitializedAsync()
    {
        await using var db = await DbFactory.CreateDbContextAsync();
        studLogic = new StudentsListLogic(db);
        _students = studLogic.GetJoinedTableData();
        _courseNames = db.Courses.Select(c => c.CourseName).ToList();
    }

    private void HandleInsert()
    {
        if (studLogic == null) return;

        if (!DateTime.TryParse(_birthDate, out var birth))
        {
            // TODO: Show validation error to user
            return;
        }

        studLogic.InsertNewEntry(_firstName, _lastName, birth, _selectedCourse, 
            string.IsNullOrEmpty(_email) ? "Has not specified" : _email);
        _students = studLogic.GetJoinedTableData();
        HandleClear();
    }

    private void HandleClear()
    {
        _firstName = "";
        _lastName = "";
        _birthDate = "";
        _email = "";
    }

    private void HandleSearch()
    {
        if (studLogic != null && !string.IsNullOrEmpty(_searchText))
        {
            _studentDetail = studLogic.GetStudents(_searchText);
            _searchText = "";
        }
    }

    private void HandleDelete(int id)
    {
        studLogic?.DeleteStudent(id);
        if (studLogic != null) _students = studLogic.GetJoinedTableData();
    }

    // TODO: Wire _students to GridView Items, _studentDetail to DetailsView Items
    // TODO: Implement row update (grv_UpdateItem / grv_RowUpdating) 
    // TODO: AutoComplete search was a Web Service (ASMX WebMethod) — convert to component-local search
}

