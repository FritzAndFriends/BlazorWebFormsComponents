using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.BLL;

public class Instructors_Logic
{
    private readonly ContosoUniversityEntities _contosoUniversityEntities;

    public Instructors_Logic(ContosoUniversityEntities contosoUniversityEntities)
    {
        _contosoUniversityEntities = contosoUniversityEntities;
    }

    public List<Instructor> getInstructors()
    {
        return _contosoUniversityEntities.Instructors
            .AsNoTracking()
            .OrderBy(instructor => instructor.InstructorID)
            .ToList();
    }

    public List<Instructor> GetSortedInstrucors(string expression, string direction)
    {
        bool ascending = string.Equals(direction, "asc", StringComparison.OrdinalIgnoreCase);
        IQueryable<Instructor> query = _contosoUniversityEntities.Instructors.AsNoTracking();

        query = expression switch
        {
            nameof(Instructor.FirstName) => ascending
                ? query.OrderBy(instructor => instructor.FirstName)
                : query.OrderByDescending(instructor => instructor.FirstName),
            nameof(Instructor.LastName) => ascending
                ? query.OrderBy(instructor => instructor.LastName)
                : query.OrderByDescending(instructor => instructor.LastName),
            _ => ascending
                ? query.OrderBy(instructor => instructor.InstructorID)
                : query.OrderByDescending(instructor => instructor.InstructorID)
        };

        return query.ToList();
    }
}
