using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Bll
{
    public class Enrollmet_Logic
    {
        private readonly IDbContextFactory<ContosoUniversityEntities> _dbFactory;

        public Enrollmet_Logic(IDbContextFactory<ContosoUniversityEntities> dbFactory)
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
}

