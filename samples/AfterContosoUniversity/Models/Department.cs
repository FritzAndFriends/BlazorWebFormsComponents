using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models;

[Table("Departments")]
public class Department
{
    [Key]
    public int DepartmentID { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int BuildingNumber { get; set; }
    public int ManagingInstructorID { get; set; }

    public ICollection<Cours> Courses { get; set; } = new HashSet<Cours>();
}


