using ContosoUniversity.Models;
using ContosoUniversity.BLL;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity;

public partial class Instructors
{
    [Inject] private IDbContextFactory<ContosoUniversityEntities> DbFactory { get; set; } = default!;

    private Instructors_Logic? instructorsLogic;
    private List<object> _instructors = new();
    private string _sortDirection = "desc";

    protected override async Task OnInitializedAsync()
    {
        await using var db = await DbFactory.CreateDbContextAsync();
        instructorsLogic = new Instructors_Logic(db);
        _instructors = instructorsLogic.getInstructors().Cast<object>().ToList();
    }

    // TODO: Wire _instructors to GridView Items parameter
    // TODO: Implement sorting — BWFC GridView Sorting parameter is EventCallback<GridViewSortEventArgs>
    //   private void HandleSorting(GridViewSortEventArgs e) { ... }
}
