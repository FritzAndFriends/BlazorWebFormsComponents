using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ContosoUniversity.BLL;
using ContosoUniversity.Models;

namespace ContosoUniversity
{
    public partial class Courses
    {
        [Inject] private CoursesLogic CoursesLogic { get; set; } = default!;

        private List<string> departments = new();
        private string selectedDepartment = string.Empty;
        private List<Cours> filteredCourses = new();
        private string courseSearchText = string.Empty;
        private Cours? selectedCourseDetail;

        protected override void OnInitialized()
        {
            departments = CoursesLogic.GetDepartmentNames();
        }

        private void SearchCourses(MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedDepartment))
            {
                filteredCourses = CoursesLogic.GetCourses(selectedDepartment);
            }
        }

        private void SearchByName(MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(courseSearchText))
            {
                var courses = CoursesLogic.GetCourse(courseSearchText);
                selectedCourseDetail = courses.FirstOrDefault();
                courseSearchText = string.Empty;
            }
        }

        public IQueryable<Cours> GetCourseData(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
        {
            totalRowCount = filteredCourses.Count;
            if (maxRows > 0)
                return filteredCourses.AsQueryable().Skip(startRowIndex).Take(maxRows);
            return filteredCourses.AsQueryable();
        }
    }
}
