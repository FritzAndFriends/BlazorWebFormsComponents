using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.BLL;

public partial class Instructors_Logic
{
    private readonly ContosoUniversityEntities _context;

    public Instructors_Logic(ContosoUniversityEntities context)
    {
        _context = context;
    }

    public List<Instructor> getInstructors() =>
        _context.Instructors.Include(i => i.Courses).ToList();

    public List<Instructor> GetSortedInstrucors(string expression, string direction)
    {
        var query = _context.Instructors.Include(i => i.Courses).AsQueryable();
        if (direction == "ASC")
            query = query.OrderBy(i => EF.Property<object>(i, expression));
        else
            query = query.OrderByDescending(i => EF.Property<object>(i, expression));
        return query.ToList();
    }
}
