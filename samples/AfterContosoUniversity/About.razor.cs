using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.BLL;
using Microsoft.AspNetCore.Components;

namespace ContosoUniversity;

public partial class About : WebFormsPageBase
{
    private GridView<object> EnrollmentsStat = default!;
    private List<object> _enrollmentStats = [];

    [Inject]
    protected Enrollmet_Logic _enrollmet_Logic { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _enrollmentStats = _enrollmet_Logic
            .Get_Enrollment_ByDate()
            .Select(entry => (object)new { entry.Key, entry.Value })
            .ToList();
    }
}
