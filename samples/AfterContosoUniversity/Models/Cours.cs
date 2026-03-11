using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models;

[Table("Courses")]
public class Cours
{
    [Key]
    public int CourseID { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int StudentsMax { get; set; }
    public int DepartmentID { get; set; }
    public int InstructorID { get; set; }

    [ForeignKey(nameof(DepartmentID))]
    public Department? Department { get; set; }

    [ForeignKey(nameof(InstructorID))]
    public Instructor? Instructor { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
}


