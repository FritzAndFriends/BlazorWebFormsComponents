using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.BLL
{
    public class Instructors_Logic
    {
        private readonly IDbContextFactory<ContosoUniversityEntities> _factory;

        public Instructors_Logic(IDbContextFactory<ContosoUniversityEntities> factory)
        {
            _factory = factory;
        }

        public List<Instructor> getInstructors()
        {
            using var db = _factory.CreateDbContext();
            return db.Instructors.ToList();
        }

        public List<Instructor> GetSortedInstrucors(string expression, string direction)
        {
            using var db = _factory.CreateDbContext();
            IQueryable<Instructor> query = db.Instructors;

            if (!string.IsNullOrEmpty(expression))
            {
                query = expression switch
                {
                    "InstructorID" => direction == "asc" ? query.OrderBy(i => i.InstructorID) : query.OrderByDescending(i => i.InstructorID),
                    "FirstName" => direction == "asc" ? query.OrderBy(i => i.FirstName) : query.OrderByDescending(i => i.FirstName),
                    "LastName" => direction == "asc" ? query.OrderBy(i => i.LastName) : query.OrderByDescending(i => i.LastName),
                    _ => query
                };
            }

            return query.ToList();
        }
    }
}
