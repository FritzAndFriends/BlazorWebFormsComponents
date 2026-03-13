namespace ContosoUniversity.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Department
    {
        public Department()
        {
            this.Courses = new HashSet<Cours>();
        }

        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int BuildingNumber { get; set; }
        public int ManagingInstructorID { get; set; }

        public virtual ICollection<Cours> Courses { get; set; }
    }
}


