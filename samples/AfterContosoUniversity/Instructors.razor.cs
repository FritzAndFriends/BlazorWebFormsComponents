using ContosoUniversity.Models;
using ContosoUniversity.BLL;

namespace ContosoUniversity;

public partial class Instructors : BlazorWebFormsComponents.WebFormsPageBase
{
    [Inject] private Instructors_Logic instructorsLogic { get; set; } = default!;

    private GridView<Instructor> grvInstructors = default!;
    private List<Instructor> instructorData = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        ViewState["SortDirection"] = "desc";
        instructorData = instructorsLogic.getInstructors();
    }
}