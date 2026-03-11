using System;
using System.Collections.Generic;

namespace ContosoUniversity.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = null!;

    public int BuildingNumber { get; set; }

    public int ManagingInstructorId { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
