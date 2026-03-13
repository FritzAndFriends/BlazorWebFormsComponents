using Microsoft.AspNetCore.Components;
using ContosoUniversity.Models;
using ContosoUniversity.Bll;
using Microsoft.EntityFrameworkCore;
using BlazorWebFormsComponents;

namespace ContosoUniversity
{
    public partial class Students
    {
        [Inject] private StudentsListLogic StudLogic { get; set; }
        [Inject] private IDbContextFactory<ContosoUniversityEntities> DbFactory { get; set; }

        private List<object> _studentData;
        private List<string> _courseNames;
        private List<object> _searchResults;

        private string _firstName = "";
        private string _lastName = "";
        private string _birthDate = "";
        private string _email = "";
        private string _searchText = "";
        private string _selectedCourse = "";

        protected override async Task OnInitializedAsync()
        {
            LoadStudentData();
            LoadCourseNames();
        }

        private void LoadStudentData()
        {
            _studentData = StudLogic.GetJoinedTableData();
        }

        private void LoadCourseNames()
        {
            using var context = DbFactory.CreateDbContext();
            _courseNames = context.Courses.Select(c => c.CourseName).ToList();
        }

        private void HandleInsert()
        {
            DateTime birth;
            try
            {
                birth = DateTime.Parse(_birthDate);
            }
            catch
            {
                return;
            }

            var selectedCourse = !string.IsNullOrEmpty(_selectedCourse) ? _selectedCourse : (_courseNames?.FirstOrDefault() ?? "");
            StudLogic.InsertNewEntry(_firstName, _lastName, birth, selectedCourse, 
                string.IsNullOrEmpty(_email) ? "Has not specified" : _email);
            LoadStudentData();
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
            _searchResults = StudLogic.GetStudents(_searchText);
            _searchText = "";
        }

        private void HandleDelete(GridViewDeleteEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _studentData.Count)
            {
                var row = _studentData[e.RowIndex];
                var idProp = row.GetType().GetProperty("ID");
                if (idProp != null)
                {
                    var id = (int)idProp.GetValue(row);
                    StudLogic.DeleteStudent(id);
                    LoadStudentData();
                }
            }
        }

        private void HandleEdit(GridViewEditEventArgs e)
        {
            // GridView handles edit mode
        }

        private void HandleUpdate(GridViewUpdateEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _studentData.Count)
            {
                var row = _studentData[e.RowIndex];
                var idProp = row.GetType().GetProperty("ID");
                if (idProp != null)
                {
                    var id = (int)idProp.GetValue(row);
                    // GridView inline editing not fully supported yet — reload data
                    LoadStudentData();
                }
            }
        }

        private void HandleCancelEdit(GridViewCancelEditEventArgs e)
        {
            // GridView handles cancel
        }
    }
}

