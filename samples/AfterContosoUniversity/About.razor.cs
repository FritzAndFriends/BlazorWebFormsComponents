using Microsoft.AspNetCore.Components;
using ContosoUniversity.Models;
using ContosoUniversity.Bll;

namespace ContosoUniversity;

public partial class About : ComponentBase
{
    [Inject]
    public ContosoUniversityEntities? Db { get; set; }

    public Dictionary<string, int> EnrollmentStats { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await Task.Run(() =>
        {
            EnrollmentStats = new Enrollmet_Logic().Get_Enrollment_ByDate();
        });
    }

    public Dictionary<string, int> EnrollmentsStat_GetData()
    {
        return EnrollmentStats;
    }
}
