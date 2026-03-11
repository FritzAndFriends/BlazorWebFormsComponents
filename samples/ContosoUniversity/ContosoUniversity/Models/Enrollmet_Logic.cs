using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoUniversity.Models
{
    public class Enrollmet_Logic
    {
        public Dictionary<string, int> Get_Enrollment_ByDate()
        {
            var enrollments = from enrl in new ContosoUniversityEntities().Enrollments
                              group enrl by enrl.Date into d
                              select new { Date = d.Key, Count = d.Select(enrl => enrl.EnrollmentID).Count() };

            Dictionary<string, int> entries = new Dictionary<string, int>();

            foreach (var entry in enrollments)
            {
                entries.Add(entry.Date.ToShortDateString(), entry.Count);
            }

            return entries;

        }
    }
}