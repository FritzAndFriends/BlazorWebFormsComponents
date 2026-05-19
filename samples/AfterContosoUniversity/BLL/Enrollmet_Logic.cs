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
            var allEnrollments = _contosoUniversityEntities.Enrollments.ToList();

            Dictionary<string, int> entries = new Dictionary<string, int>();

            foreach (var group in allEnrollments.GroupBy(e => e.Date.ToShortDateString()))
            {
                entries[group.Key] = group.Count();
            }

            return entries;
        }
        #endregion
    }
}
