// Business logic — migrated from Web Forms to use injected DbContext

using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Models;

namespace ContosoUniversity.BLL
{
    public class Instructors_Logic
    {
        private readonly ContosoUniversityEntities _db;
        public Instructors_Logic(ContosoUniversityEntities db) { _db = db; }

        public List<Instructor> getInstructors()
        {
            return _db.Instructors.ToList();
        }

        public List<Instructor> GetSortedInstrucors(string expression, string direction)
        {
            // TODO: Convert raw SQL sorting to EF Core LINQ OrderBy
            // Original used raw SqlConnection — migrated to EF Core queries
            var query = _db.Instructors.AsQueryable();
            
            if (!string.IsNullOrEmpty(expression))
            {
                query = expression.ToLower() switch
                {
                    "instructorid" => direction == "asc" ? query.OrderBy(i => i.InstructorID) : query.OrderByDescending(i => i.InstructorID),
                    "firstname" => direction == "asc" ? query.OrderBy(i => i.FirstName) : query.OrderByDescending(i => i.FirstName),
                    "lastname" => direction == "asc" ? query.OrderBy(i => i.LastName) : query.OrderByDescending(i => i.LastName),
                    _ => query
                };
            }

            return query.ToList();
        }
    }
}
