using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Models;

namespace ContosoUniversity.BLL
{
    public class Enrollmet_Logic
    {
        private readonly ContosoUniversityEntities _context;

        public Enrollmet_Logic(ContosoUniversityEntities context)
        {
            _context = context;
        }

        #region Get Enrollments List
        public Dictionary<string, int> Get_Enrollment_ByDate()
        {
            var enrollments = _context.Enrollments
                .GroupBy(e => e.Date.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            Dictionary<string, int> entries = new Dictionary<string, int>();

            foreach (var entry in enrollments)
            {
                entries.Add(entry.Date.ToShortDateString(), entry.Count);
            }

            return entries;
        }
        #endregion
    }
}