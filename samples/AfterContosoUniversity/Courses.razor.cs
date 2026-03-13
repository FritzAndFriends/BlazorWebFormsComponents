using Microsoft.AspNetCore.Components;
using ContosoUniversity.Models;
using ContosoUniversity.BLL;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity
{
    public partial class Courses
    {
        [Inject] private Courses_Logic CoursesLogic { get; set; }
        [Inject] private IDbContextFactory<ContosoUniversityEntities> DbFactory { get; set; }

        private List<string> _departments;
        private List<Cours> _courses;
        private List<Cours> _courseDetails;
        private string _selectedDepartment = "";
        private string _searchCourseName = "";

        protected override async Task OnInitializedAsync()
        {
            using var context = DbFactory.CreateDbContext();
            _departments = context.Departments.Select(d => d.DepartmentName).ToList();
        }

        private void HandleSearchCourse()
        {
            if (!string.IsNullOrEmpty(_selectedDepartment))
            {
                _courses = CoursesLogic.GetCourses(_selectedDepartment);
            }
        }

        private void HandleSearchByName()
        {
            if (!string.IsNullOrEmpty(_searchCourseName))
            {
                _courseDetails = CoursesLogic.GetCourse(_searchCourseName);
            }
            _searchCourseName = "";
        }
    }
}

