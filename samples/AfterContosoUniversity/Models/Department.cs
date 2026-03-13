namespace ContosoUniversity.Models
{
    public class Department
    {
        public Department()
        {
            Courses = new HashSet<Cours>();
        }

        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int BuildingNumber { get; set; }
        public int ManagingInstructorID { get; set; }

        public virtual ICollection<Cours> Courses { get; set; }
    }
}

