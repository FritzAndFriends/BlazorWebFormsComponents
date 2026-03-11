using System;
using System.Collections.Generic;

namespace ContosoUniversity.Models;

public partial class Enrollment
{
    public int EnrollmentId { get; set; }

    public DateOnly Date { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
