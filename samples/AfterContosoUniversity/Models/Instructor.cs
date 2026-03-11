using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models;

[Table("Instructors")]
public class Instructor
{
    [Key]
    public int InstructorID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Email { get; set; } = string.Empty;

    public ICollection<Cours> Courses { get; set; } = new HashSet<Cours>();
}


