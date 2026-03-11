using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;
using ContosoUniversity.Bll;

namespace ContosoUniversity;

public partial class Students : ComponentBase
{
    [Inject]
    public ContosoUniversityEntities? Db { get; set; }

    private StudentsListLogic? studLogic;

    // Form fields (converted from control references)
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SelectedCourse { get; set; } = string.Empty;
    public string SearchText { get; set; } = string.Empty;

    // Data collections
    public List<Cours> CoursesList { get; set; } = new();
    public List<object> EnrollmentData { get; set; } = new();
    public List<object> StudentSearchResults { get; set; } = new();
    private List<Student> StudentsList { get; set; } = new();
    private Student? SelectedStudent { get; set; }

    protected override async Task OnInitializedAsync()
    {
        studLogic = new StudentsListLogic();

        if (Db != null)
        {
            CoursesList = await Db.Courses.ToListAsync();
        }

        EnrollmentData = GetEnrollmentData();
    }

    public List<object> GetEnrollmentData()
    {
        if (studLogic == null) return new List<object>();
        return studLogic.GetJoinedTableData().ToList();
    }

    public async Task<List<string>> GetStudentAutocomplete(string prefixText)
    {
        if (Db == null) return new List<string>();

        return await Db.Students
            .Where(s => s.FirstName != null && s.FirstName.StartsWith(prefixText))
            .Select(s => $"{s.FirstName} {s.LastName}")
            .ToListAsync();
    }

    public void UpdateStudent(int id, string name, string email)
    {
        studLogic?.UpdateStudentData(id, name, email);
        EnrollmentData = GetEnrollmentData();
    }

    public void DeleteStudent(int id)
    {
        studLogic?.DeleteStudent(id);
        EnrollmentData = GetEnrollmentData();
    }

    public void InsertStudent()
    {
        if (!DateTime.TryParse(BirthDate, out DateTime birth))
        {
            throw new FormatException("Wrong Date Format!");
        }

        studLogic?.InsertNewEntry(FirstName, LastName, birth, SelectedCourse, Email);
        ClearForm();
        EnrollmentData = GetEnrollmentData();
    }

    public void ClearForm()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        BirthDate = string.Empty;
        Email = string.Empty;
        SelectedCourse = string.Empty;
    }

    private void btnInsert_Click() { InsertStudent(); }
    private void btnClear_Click() { ClearForm(); }
    private void btnSearch_Click() { SearchStudents(); }

    public void SearchStudents()
    {
        if (studLogic != null && !string.IsNullOrEmpty(SearchText))
        {
            StudentSearchResults = studLogic.GetStudents(SearchText).ToList();
            SearchText = string.Empty;
        }
    }
}

