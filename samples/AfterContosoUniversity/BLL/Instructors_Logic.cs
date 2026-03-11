using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.BLL;

public class Instructors_Logic
{
    private readonly IDbContextFactory<ContosoUniversityContext> _dbFactory;

    public Instructors_Logic(IDbContextFactory<ContosoUniversityContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public List<Instructor> GetInstructors()
    {
        using var context = _dbFactory.CreateDbContext();
        return context.Instructors.ToList();
    }

    public List<Instructor> GetSortedInstructors(string expression, string direction)
    {
        using var context = _dbFactory.CreateDbContext();
        IQueryable<Instructor> query = context.Instructors;

        var isAsc = string.Equals(direction, "asc", StringComparison.OrdinalIgnoreCase);

        query = expression switch
        {
            "InstructorID" => isAsc ? query.OrderBy(i => i.InstructorID) : query.OrderByDescending(i => i.InstructorID),
            "FirstName" => isAsc ? query.OrderBy(i => i.FirstName) : query.OrderByDescending(i => i.FirstName),
            "LastName" => isAsc ? query.OrderBy(i => i.LastName) : query.OrderByDescending(i => i.LastName),
            _ => query.OrderBy(i => i.InstructorID)
        };

        return query.ToList();
    }
}
