using System;

namespace DepartmentPortal.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
    }
}
