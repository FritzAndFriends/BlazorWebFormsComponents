using System;

namespace DepartmentPortal.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsAdmin { get; set; }
    }
}
