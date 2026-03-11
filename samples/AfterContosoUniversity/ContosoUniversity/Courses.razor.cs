using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using global::ContosoUniversity.Models;

namespace ContosoUniversity.ContosoUniversity
{
    public partial class Courses : ComponentBase
    {
        [Inject]
        private IDbContextFactory<SchoolContext> DbFactory { get; set; } = default!;

        /// <summary>
        /// List of department names for the dropdown
        /// </summary>
        private List<string> Departments { get; set; } = new();

        /// <summary>
        /// Selected department for filtering courses
        /// </summary>
        private string SelectedDepartment { get; set; } = string.Empty;

        /// <summary>
        /// Courses filtered by selected department for the GridView
        /// </summary>
        private List<Course> CourseList { get; set; } = new();

        /// <summary>
        /// Course name for text search
        /// </summary>
        private string SearchCourseName { get; set; } = string.Empty;

        /// <summary>
        /// Course details for the DetailsView
        /// </summary>
        private List<Course> CourseDetails { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            using var context = await DbFactory.CreateDbContextAsync();

            // Load departments for the dropdown
            Departments = await context.Departments
                .Select(d => d.DepartmentName.Trim())
                .OrderBy(d => d)
                .ToListAsync();
        }

        /// <summary>
        /// Search courses by selected department (btnSearchCourse click handler)
        /// </summary>
        private async Task OnSearchCourseClick()
        {
            if (string.IsNullOrEmpty(SelectedDepartment))
            {
                CourseList = new List<Course>();
                return;
            }

            using var context = await DbFactory.CreateDbContextAsync();

            CourseList = await context.Courses
                .Include(c => c.Department)
                .Where(c => c.Department.DepartmentName.Trim() == SelectedDepartment)
                .ToListAsync();
        }

        /// <summary>
        /// Search course by name (search button click handler)
        /// </summary>
        private async Task OnSearchClick()
        {
            if (string.IsNullOrEmpty(SearchCourseName))
            {
                CourseDetails = new List<Course>();
                return;
            }

            using var context = await DbFactory.CreateDbContextAsync();

            CourseDetails = await context.Courses
                .Where(c => c.CourseName == SearchCourseName)
                .ToListAsync();

            SearchCourseName = string.Empty;
        }

        /// <summary>
        /// Get autocomplete suggestions for course names
        /// </summary>
        public async Task<List<string>> GetCourseAutocomplete(string prefixText)
        {
            using var context = await DbFactory.CreateDbContextAsync();

            return await context.Courses
                .Where(c => c.CourseName.StartsWith(prefixText))
                .Select(c => c.CourseName)
                .Take(20)
                .ToListAsync();
        }
    }
}
