using ContosoUniversity.Models;
using ContosoUniversity.Bll;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity;

public partial class About
{
    [Inject] private IDbContextFactory<ContosoUniversityEntities> DbFactory { get; set; } = default!;

    private List<object> _enrollmentStats = new();

    protected override async Task OnInitializedAsync()
    {
        await using var db = await DbFactory.CreateDbContextAsync();
        var logic = new Enrollmet_Logic(db);
        var stats = logic.Get_Enrollment_ByDate();
        _enrollmentStats = stats.Select(kvp => (object)new { Key = kvp.Key, Value = kvp.Value }).ToList();
    }

    // TODO: Wire _enrollmentStats to GridView Items parameter once SelectMethod delegate is implemented
}
