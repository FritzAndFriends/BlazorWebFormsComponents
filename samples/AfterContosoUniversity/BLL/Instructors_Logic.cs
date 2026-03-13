using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.BLL
{
    public class Instructors_Logic
    {
        private readonly IDbContextFactory<ContosoUniversityEntities> _dbFactory;

        public Instructors_Logic(IDbContextFactory<ContosoUniversityEntities> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public List<Instructor> getInstructors()
        {
            using var context = _dbFactory.CreateDbContext();
            return context.Instructors.ToList();
        }

        public List<Instructor> GetSortedInstrucors(string expression, string direction)
        {
            using var context = _dbFactory.CreateDbContext();
            IQueryable<Instructor> query = context.Instructors;

            if (!string.IsNullOrEmpty(expression))
            {
                query = expression switch
                {
                    "InstructorID" => direction == "asc" ? query.OrderBy(i => i.InstructorID) : query.OrderByDescending(i => i.InstructorID),
                    "FirstName" => direction == "asc" ? query.OrderBy(i => i.FirstName) : query.OrderByDescending(i => i.FirstName),
                    "LastName" => direction == "asc" ? query.OrderBy(i => i.LastName) : query.OrderByDescending(i => i.LastName),
                    _ => query.OrderBy(i => i.InstructorID)
                };
            }

            return query.ToList();
        }
    }
}

