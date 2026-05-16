using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.BLL;

public partial class Instructors_Logic
{
    private readonly ContosoUniversityEntities _db;

    public Instructors_Logic(ContosoUniversityEntities db)
    {
        _db = db;
    }

    public List<Instructor> getInstructors()
    {
        return _db.Instructors.ToList();
    }

    public List<Instructor> GetSortedInstrucors(string expression, string direction)
    {
        var instructors = _db.Instructors.AsQueryable();

        instructors = expression switch
        {
            "InstructorID" => direction == "asc" ? instructors.OrderBy(i => i.InstructorID) : instructors.OrderByDescending(i => i.InstructorID),
            "FirstName" => direction == "asc" ? instructors.OrderBy(i => i.FirstName) : instructors.OrderByDescending(i => i.FirstName),
            "LastName" => direction == "asc" ? instructors.OrderBy(i => i.LastName) : instructors.OrderByDescending(i => i.LastName),
            _ => instructors.OrderBy(i => i.InstructorID)
        };

        return instructors.ToList();
    }
}
