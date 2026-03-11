using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using global::ContosoUniversity.Models;

namespace ContosoUniversity.ContosoUniversity
{
    /// <summary>
    /// View model for instructor data displayed in the grid
    /// </summary>
    public class InstructorViewModel
    {
        public int InstructorID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
    }

    public partial class Instructors : ComponentBase
    {
        [Inject]
        private IDbContextFactory<SchoolContext> DbFactory { get; set; } = default!;

        /// <summary>
        /// List of instructors for the GridView
        /// </summary>
        private List<InstructorViewModel> InstructorList { get; set; } = new();

        /// <summary>
        /// Current sort expression
        /// </summary>
        private string SortExpression { get; set; } = "InstructorID";

        /// <summary>
        /// Sort direction (ASC or DESC)
        /// </summary>
        private bool SortAscending { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadInstructors();
        }

        private async Task LoadInstructors()
        {
            using var context = await DbFactory.CreateDbContextAsync();

            var query = context.Instructors.AsQueryable();

            // Apply sorting based on current expression and direction
            query = SortExpression switch
            {
                "FirstName" => SortAscending ? query.OrderBy(i => i.FirstName) : query.OrderByDescending(i => i.FirstName),
                "LastName" => SortAscending ? query.OrderBy(i => i.LastName) : query.OrderByDescending(i => i.LastName),
                _ => SortAscending ? query.OrderBy(i => i.InstructorId) : query.OrderByDescending(i => i.InstructorId)
            };

            InstructorList = await query
                .Select(i => new InstructorViewModel
                {
                    InstructorID = i.InstructorId,
                    FirstName = i.FirstName,
                    LastName = i.LastName,
                    Email = i.Email
                })
                .ToListAsync();
        }

        /// <summary>
        /// Handle sort event from GridView
        /// </summary>
        private async Task OnSort(string sortExpression)
        {
            if (SortExpression == sortExpression)
            {
                // Toggle direction if same column
                SortAscending = !SortAscending;
            }
            else
            {
                SortExpression = sortExpression;
                SortAscending = true;
            }

            await LoadInstructors();
        }
    }
}
