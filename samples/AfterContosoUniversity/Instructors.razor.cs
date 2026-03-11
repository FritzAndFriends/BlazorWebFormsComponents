using ContosoUniversity.BLL;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Components;
using BlazorWebFormsComponents;

namespace ContosoUniversity;

public partial class Instructors : ComponentBase
{
    [Inject] private Instructors_Logic InstructorLogic { get; set; } = default!;

    private List<Instructor> _instructors = new();
    private string _sortDirection = "desc";

    protected override void OnInitialized()
    {
        _instructors = InstructorLogic.GetInstructors();
    }

    private void HandleSorting(GridViewSortEventArgs e)
    {
        _instructors = InstructorLogic.GetSortedInstructors(e.SortExpression, _sortDirection);
        _sortDirection = _sortDirection == "asc" ? "desc" : "asc";
    }
}

