using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models;

[Table("Enrollments")]
public class Enrollment
{
    [Key]
    public int EnrollmentID { get; set; }
    public DateTime Date { get; set; }
    public int StudentID { get; set; }
    public int CourseID { get; set; }

    [ForeignKey(nameof(CourseID))]
    public Cours? Cours { get; set; }

    [ForeignKey(nameof(StudentID))]
    public Student? Student { get; set; }
}


