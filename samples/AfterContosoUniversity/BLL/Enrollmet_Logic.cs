using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.Bll
{
    public class Enrollmet_Logic
    {
        private readonly IDbContextFactory<ContosoUniversityEntities> _factory;

        public Enrollmet_Logic(IDbContextFactory<ContosoUniversityEntities> factory)
        {
            _factory = factory;
        }

        public Dictionary<string, int> Get_Enrollment_ByDate()
        {
            using var db = _factory.CreateDbContext();
            var enrollments = from enrl in db.Enrollments
                              group enrl by enrl.Date into d
                              select new { Date = d.Key, Count = d.Count() };

            var entries = new Dictionary<string, int>();

            foreach (var entry in enrollments.ToList())
            {
                entries.Add(entry.Date.ToShortDateString(), entry.Count);
            }

            return entries;
        }
    }
}
