using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using global::ContosoUniversity.Models;

namespace ContosoUniversity.ContosoUniversity
{
    public partial class About : ComponentBase
    {
        [Inject]
        private IDbContextFactory<SchoolContext> DbFactory { get; set; } = default!;

        /// <summary>
        /// Enrollment statistics for the GridView - Key is the date, Value is the student count
        /// </summary>
        private List<KeyValuePair<string, int>> EnrollmentStats { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            using var context = await DbFactory.CreateDbContextAsync();

            // Groups enrollments by Date and counts them (same logic as Enrollmet_Logic.Get_Enrollment_ByDate)
            var enrollments = await context.Enrollments
                .GroupBy(e => e.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(e => e.Date)
                .ToListAsync();

            EnrollmentStats = enrollments
                .Select(e => new KeyValuePair<string, int>(e.Date.ToString("d"), e.Count))
                .ToList();
        }
    }
}
