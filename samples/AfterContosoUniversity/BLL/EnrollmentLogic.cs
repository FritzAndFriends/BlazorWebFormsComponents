using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.BLL
{
    public class EnrollmentLogic
    {
        private readonly IDbContextFactory<ContosoUniversityEntities> _contextFactory;

        public EnrollmentLogic(IDbContextFactory<ContosoUniversityEntities> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public List<EnrollmentStat> GetEnrollmentsByDate()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Enrollments
                .GroupBy(e => e.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .AsEnumerable()
                .Select(e => new EnrollmentStat
                {
                    Key = e.Date.ToShortDateString(),
                    Value = e.Count
                })
                .ToList();
        }
    }
}
