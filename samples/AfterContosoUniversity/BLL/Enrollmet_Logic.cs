// Business logic — migrated from Web Forms to use injected DbContext

using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Bll
{
    public class Enrollmet_Logic
    {
        private readonly ContosoUniversityEntities _db;
        public Enrollmet_Logic(ContosoUniversityEntities db) { _db = db; }

        public Dictionary<string, int> Get_Enrollment_ByDate()
        {
            var enrollments = from enrl in _db.Enrollments
                              group enrl by enrl.Date into d
                              select new { Date = d.Key, Count = d.Select(enrl => enrl.EnrollmentID).Count() };

            var entries = new Dictionary<string, int>();

            foreach (var entry in enrollments)
            {
                entries.Add(entry.Date.ToShortDateString(), entry.Count);
            }

            return entries;
        }
    }
}
