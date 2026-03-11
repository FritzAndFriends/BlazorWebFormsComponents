using ContosoUniversity.BLL;
using Microsoft.AspNetCore.Components;

namespace ContosoUniversity;

public partial class About : ComponentBase
{
    [Inject] private Enrollmet_Logic EnrollmentLogic { get; set; } = default!;

    private List<KeyValuePair<string, int>> _enrollmentData = new();

    protected override void OnInitialized()
    {
        var data = EnrollmentLogic.Get_Enrollment_ByDate();
        _enrollmentData = data.ToList();
    }
}
