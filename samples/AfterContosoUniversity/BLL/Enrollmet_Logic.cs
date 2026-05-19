using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Models;

namespace ContosoUniversity.BLL
{
    public class Enrollmet_Logic
    {
    private readonly ContosoUniversityEntities _contosoUniversityEntities;

    public Enrollmet_Logic(ContosoUniversityEntities contosoUniversityEntities)
    {
        _contosoUniversityEntities = contosoUniversityEntities;
    }

        #region Get Enrollments List
        public Dictionary<string, int> Get_Enrollment_ByDate()
        {
            var enrollments = from enrl in _contosoUniversityEntities.Enrollments
                              group enrl by enrl.Date into d
                              select new { Date = d.Key, Count = d.Select(enrl => enrl.EnrollmentID).Count() };

            Dictionary<string, int> entries = new Dictionary<string, int>();

            foreach (var entry in enrollments)
            {
                var key = entry.Date.ToShortDateString();
                if (entries.ContainsKey(key))
                    entries[key] += entry.Count;
                else
                    entries.Add(key, entry.Count);
            }

            return entries;

        }
        #endregion
    }
}