using Microsoft.AspNetCore.Components;
using ContosoUniversity.Models;
using ContosoUniversity.BLL;

namespace ContosoUniversity;

public partial class Instructors : ComponentBase
{
    [Inject]
    public ContosoUniversityEntities? Db { get; set; }

    private Instructors_Logic? instructorsLogic;

    // Converted from ViewState to private field
    private string sortDirection = "desc";

    public List<Instructor> InstructorsList { get; set; } = new();

    protected override Task OnInitializedAsync()
    {
        instructorsLogic = new Instructors_Logic();
        InstructorsList = instructorsLogic.getInstructors().ToList();
        return Task.CompletedTask;
    }

    public void OnSorting(string sortExpression)
    {
        if (instructorsLogic != null)
        {
            InstructorsList = instructorsLogic.GetSortedInstrucors(sortExpression, sortDirection).ToList();
            ChangeSortDirection();
        }
    }

    private void ChangeSortDirection()
    {
        sortDirection = sortDirection == "asc" ? "desc" : "asc";
    }
}
