using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ContosoUniversity.BLL;
using ContosoUniversity.Models;

namespace ContosoUniversity
{
    public partial class Students
    {
        [Inject] private StudentsLogic StudentsLogic { get; set; } = default!;

        private List<StudentListItem> studentData = new();
        private List<string> courseNames = new();
        private List<StudentInfo>? searchResults;
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string birthDate = string.Empty;
        private string email = string.Empty;
        private string selectedCourse = string.Empty;
        private string searchText = string.Empty;

        protected override void OnInitialized()
        {
            courseNames = StudentsLogic.GetCourseNames();
            studentData = StudentsLogic.GetJoinedTableData();
        }

        public IQueryable<StudentListItem> GetStudentData(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
        {
            totalRowCount = studentData.Count;
            return studentData.AsQueryable();
        }

        private void InsertStudent(MouseEventArgs e)
        {
            if (DateTime.TryParse(birthDate, out var birth))
            {
                StudentsLogic.InsertNewEntry(firstName, lastName, birth, selectedCourse,
                    string.IsNullOrEmpty(email) ? "Has not specified" : email);
                studentData = StudentsLogic.GetJoinedTableData();
                ClearForm(e);
            }
        }

        private void ClearForm(MouseEventArgs e)
        {
            firstName = string.Empty;
            lastName = string.Empty;
            birthDate = string.Empty;
            email = string.Empty;
            selectedCourse = courseNames.FirstOrDefault() ?? string.Empty;
        }

        private void SearchStudent(MouseEventArgs e)
        {
            searchResults = StudentsLogic.GetStudentsByName(searchText);
            searchText = string.Empty;
        }
    }
}
