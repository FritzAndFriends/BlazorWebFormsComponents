using ContosoUniversity.Models;
using ContosoUniversity.BLL;
using Microsoft.AspNetCore.Components;

namespace ContosoUniversity;

public partial class Instructors : BlazorWebFormsComponents.WebFormsPageBase
{
    [Inject] private Instructors_Logic instructorsLogic { get; set; } = default!;

    private GridView<Instructor> grvInstructors = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        ViewState["SortDirection"] = "desc";
    }

    public List<Instructor> grvInstructors_GetData()
    {
        return instructorsLogic.getInstructors();
    }

    private void grvInstructors_Sorting(BlazorWebFormsComponents.GridViewSortEventArgs e)
    {
        grvInstructors.DataSource = instructorsLogic.GetSortedInstrucors(e.SortExpression, ViewState["SortDirection"]?.ToString() ?? "asc");
        ChangeSortDirection();
    }

    private void ChangeSortDirection()
    {
        if (ViewState["SortDirection"]?.ToString() == "asc")
        {
            ViewState["SortDirection"] = "desc";
        }
        else
        {
            ViewState["SortDirection"] = "asc";
        }
    }
}
