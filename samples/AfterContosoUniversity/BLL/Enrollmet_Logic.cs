using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.BLL;

public class Enrollmet_Logic
{
    private readonly IDbContextFactory<ContosoUniversityContext> _dbFactory;

    public Enrollmet_Logic(IDbContextFactory<ContosoUniversityContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public Dictionary<string, int> Get_Enrollment_ByDate()
    {
        using var context = _dbFactory.CreateDbContext();

        var enrollments = context.Enrollments
            .GroupBy(e => e.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToList();

        var entries = new Dictionary<string, int>();
        foreach (var entry in enrollments)
        {
            entries.Add(entry.Date.ToShortDateString(), entry.Count);
        }

        return entries;
    }
}
