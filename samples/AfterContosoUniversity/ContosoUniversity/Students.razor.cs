using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using global::ContosoUniversity.Models;

namespace ContosoUniversity.ContosoUniversity
{
    /// <summary>
    /// View model for student enrollment data in the grid
    /// </summary>
    public class StudentEnrollmentViewModel
    {
        public int ID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Date { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    /// <summary>
    /// View model for student details in the DetailsView
    /// </summary>
    public class StudentDetailViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string BirthDate { get; set; } = string.Empty;
        public int StudentID { get; set; }
    }

    public partial class Students : ComponentBase
    {
        [Inject]
        private IDbContextFactory<SchoolContext> DbFactory { get; set; } = default!;

        /// <summary>
        /// Student enrollment data for the GridView
        /// </summary>
        private List<StudentEnrollmentViewModel> StudentEnrollments { get; set; } = new();

        /// <summary>
        /// Course names for the dropdown
        /// </summary>
        private List<string> CourseNames { get; set; } = new();

        // Form fields for adding new student
        private string FirstName { get; set; } = string.Empty;
        private string LastName { get; set; } = string.Empty;
        private string BirthDate { get; set; } = string.Empty;
        private string Email { get; set; } = string.Empty;
        private string SelectedCourse { get; set; } = string.Empty;

        // Search fields
        private string SearchName { get; set; } = string.Empty;

        /// <summary>
        /// Student details for the DetailsView
        /// </summary>
        private List<StudentDetailViewModel> SearchResults { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            using var context = await DbFactory.CreateDbContextAsync();

            // Load course names for dropdown
            CourseNames = await context.Courses
                .Select(c => c.CourseName)
                .OrderBy(c => c)
                .ToListAsync();

            await LoadStudentEnrollments();
        }

        private async Task LoadStudentEnrollments()
        {
            using var context = await DbFactory.CreateDbContextAsync();

            // Join Students with Enrollments and group by student/date
            // Same logic as StudentsListLogic.GetJoinedTableData()
            var enrollments = await context.Enrollments
                .Include(e => e.Student)
                .ToListAsync();

            StudentEnrollments = enrollments
                .GroupBy(e => new { e.StudentId, e.Date, e.Student.FirstName, e.Student.LastName, e.Student.Email })
                .Select(g => new StudentEnrollmentViewModel
                {
                    ID = g.Key.StudentId,
                    Date = g.Key.Date.ToString("d"),
                    FullName = $"{g.Key.FirstName} {g.Key.LastName}",
                    Email = g.Key.Email,
                    Count = g.Count()
                })
                .ToList();
        }

        /// <summary>
        /// Insert new student and enrollment (btnInsert click handler)
        /// </summary>
        private async Task OnInsertClick()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
                return;

            // Try multiple date formats for flexibility
            DateOnly birthDate;
            if (!DateOnly.TryParse(BirthDate, out birthDate))
            {
                // Try MM/dd/yyyy format explicitly
                if (!DateOnly.TryParseExact(BirthDate, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out birthDate))
                {
                    // If date is empty or invalid, use a default date
                    if (string.IsNullOrWhiteSpace(BirthDate))
                    {
                        birthDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-20));
                    }
                    else
                    {
                        return; // Invalid date provided
                    }
                }
            }

            using var context = await DbFactory.CreateDbContextAsync();

            var newStudent = new Student
            {
                FirstName = FirstName,
                LastName = LastName,
                BirthDate = birthDate,
                Email = string.IsNullOrWhiteSpace(Email) ? "Has not specified" : Email
            };

            context.Students.Add(newStudent);
            await context.SaveChangesAsync();

            // Add enrollment if course selected
            if (!string.IsNullOrEmpty(SelectedCourse))
            {
                var course = await context.Courses
                    .FirstOrDefaultAsync(c => c.CourseName == SelectedCourse);

                if (course != null)
                {
                    var enrollment = new Enrollment
                    {
                        StudentId = newStudent.StudentId,
                        CourseId = course.CourseId,
                        Date = DateOnly.FromDateTime(DateTime.Now)
                    };

                    context.Enrollments.Add(enrollment);
                    await context.SaveChangesAsync();
                }
            }

            await OnClearClick();
            await LoadStudentEnrollments();
        }

        /// <summary>
        /// Clear form fields (btnClear click handler)
        /// </summary>
        private Task OnClearClick()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            BirthDate = string.Empty;
            Email = string.Empty;
            SelectedCourse = string.Empty;
            StateHasChanged();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Search student by name (btnSearch click handler)
        /// </summary>
        private async Task OnSearchClick()
        {
            if (string.IsNullOrWhiteSpace(SearchName))
            {
                SearchResults = new List<StudentDetailViewModel>();
                return;
            }

            using var context = await DbFactory.CreateDbContextAsync();

            var searchTerm = SearchName.Trim().ToLower();
            var nameParts = SearchName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
            var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

            var query = context.Students.AsQueryable();

            // Use Contains for partial matching (like Web Forms autocomplete)
            if (!string.IsNullOrEmpty(firstName))
            {
                query = query.Where(s => 
                    s.FirstName.ToLower().Contains(firstName.ToLower()) ||
                    s.LastName.ToLower().Contains(firstName.ToLower()));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query = query.Where(s => 
                    s.LastName.ToLower().Contains(lastName.ToLower()));
            }

            SearchResults = await query
                .Select(s => new StudentDetailViewModel
                {
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    BirthDate = s.BirthDate.ToString("d"),
                    StudentID = s.StudentId
                })
                .ToListAsync();
        }

        /// <summary>
        /// Get autocomplete suggestions for student names
        /// </summary>
        public async Task<List<string>> GetStudentAutocomplete(string prefixText)
        {
            using var context = await DbFactory.CreateDbContextAsync();

            return await context.Students
                .Where(s => s.FirstName.StartsWith(prefixText))
                .Select(s => $"{s.FirstName} {s.LastName}")
                .Take(20)
                .ToListAsync();
        }
    }
}
