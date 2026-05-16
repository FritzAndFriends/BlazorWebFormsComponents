using ContosoUniversity.Models;
using ContosoUniversity.BLL;

namespace ContosoUniversity;

public partial class Students : BlazorWebFormsComponents.WebFormsPageBase
{
    [Inject] private StudentsListLogic studLogic { get; set; } = default!;

    private GridView<object> grv = default!;
    private List<object> studentData = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        studentData = studLogic.GetJoinedTableData();
    }
}