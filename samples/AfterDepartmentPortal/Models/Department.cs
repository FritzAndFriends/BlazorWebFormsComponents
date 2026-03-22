namespace AfterDepartmentPortal.Models;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DivisionName { get; set; } = string.Empty;
    public int ManagerId { get; set; }
}
