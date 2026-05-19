using ContosoUniversity.BLL;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Components;

namespace ContosoUniversity;

public partial class Instructors : WebFormsPageBase
{
    private GridView<Instructor> grvInstructors = default!;
    private List<Instructor> _instructors = [];
    private string _sortDirection = "desc";

    [Inject]
    protected Instructors_Logic instructorsLogic { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _instructors = instructorsLogic.getInstructors();
    }

    protected void grvInstructors_Sorting(GridViewSortEventArgs e)
    {
        _instructors = instructorsLogic.GetSortedInstrucors(e.SortExpression, _sortDirection);
        ChangeSortDirection();
    }

    private void ChangeSortDirection()
    {
        _sortDirection = _sortDirection == "asc" ? "desc" : "asc";
    }
}
