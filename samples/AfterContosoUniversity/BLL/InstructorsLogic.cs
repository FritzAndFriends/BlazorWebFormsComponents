using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.BLL
{
    public class InstructorsLogic
    {
        private readonly IDbContextFactory<ContosoUniversityEntities> _contextFactory;

        public InstructorsLogic(IDbContextFactory<ContosoUniversityEntities> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public List<Instructor> GetInstructors()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Instructors.ToList();
        }

        public List<Instructor> GetSortedInstructors(string expression, string direction)
        {
            using var context = _contextFactory.CreateDbContext();
            IQueryable<Instructor> query = context.Instructors;

            query = expression switch
            {
                "InstructorID" => direction == "asc"
                    ? query.OrderBy(i => i.InstructorID)
                    : query.OrderByDescending(i => i.InstructorID),
                "FirstName" => direction == "asc"
                    ? query.OrderBy(i => i.FirstName)
                    : query.OrderByDescending(i => i.FirstName),
                "LastName" => direction == "asc"
                    ? query.OrderBy(i => i.LastName)
                    : query.OrderByDescending(i => i.LastName),
                _ => query.OrderBy(i => i.InstructorID)
            };

            return query.ToList();
        }
    }
}
