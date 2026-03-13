using Microsoft.AspNetCore.Components;
using BlazorWebFormsComponents;
using ContosoUniversity.BLL;
using ContosoUniversity.Models;

namespace ContosoUniversity
{
    public partial class Instructors
    {
        [Inject] private InstructorsLogic InstructorsLogic { get; set; } = default!;

        private List<Instructor> instructors = new();
        private string sortDirection = "desc";

        protected override void OnInitialized()
        {
            instructors = InstructorsLogic.GetInstructors();
        }

        private void HandleSorting(GridViewSortEventArgs e)
        {
            instructors = InstructorsLogic.GetSortedInstructors(e.SortExpression, sortDirection);
            sortDirection = sortDirection == "asc" ? "desc" : "asc";
        }

        public IQueryable<Instructor> GetInstructorData(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
        {
            totalRowCount = instructors.Count;
            return instructors.AsQueryable();
        }
    }
}
