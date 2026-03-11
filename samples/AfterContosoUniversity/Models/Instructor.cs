using System;
using System.Collections.Generic;

namespace ContosoUniversity.Models;

public partial class Instructor
{
    public int InstructorId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
