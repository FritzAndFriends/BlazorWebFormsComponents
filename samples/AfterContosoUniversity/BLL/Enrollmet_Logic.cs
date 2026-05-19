using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.BLL;

public class Enrollmet_Logic
{
    private readonly ContosoUniversityEntities _contosoUniversityEntities;

    public Enrollmet_Logic(ContosoUniversityEntities contosoUniversityEntities)
    {
        _contosoUniversityEntities = contosoUniversityEntities;
    }

    public Dictionary<string, int> Get_Enrollment_ByDate()
    {
        var enrollments = _contosoUniversityEntities.Enrollments
            .AsNoTracking()
            .ToList();

        return enrollments
            .GroupBy(enrollment => enrollment.Date.Date)
            .OrderBy(group => group.Key)
            .ToDictionary(group => group.Key.ToShortDateString(), group => group.Count());
    }
}
