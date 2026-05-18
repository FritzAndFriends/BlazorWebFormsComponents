namespace DepartmentPortal.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DivisionName { get; set; }
        public int ManagerId { get; set; }
    }
}
