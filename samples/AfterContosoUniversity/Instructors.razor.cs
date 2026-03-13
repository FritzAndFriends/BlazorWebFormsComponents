using Microsoft.AspNetCore.Components;
using ContosoUniversity.Models;
using ContosoUniversity.BLL;
using BlazorWebFormsComponents;

namespace ContosoUniversity
{
    public partial class Instructors
    {
        [Inject] private Instructors_Logic InstructorsLogic { get; set; }

        private List<Instructor> _instructors;
        private string _sortDirection = "desc";

        protected override async Task OnInitializedAsync()
        {
            _instructors = InstructorsLogic.getInstructors();
        }

        private void HandleSorting(GridViewSortEventArgs e)
        {
            _instructors = InstructorsLogic.GetSortedInstrucors(e.SortExpression, _sortDirection);
            _sortDirection = _sortDirection == "asc" ? "desc" : "asc";
        }
    }
}

