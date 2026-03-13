using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    public class Cours
    {
        public Cours()
        {
            Enrollments = new HashSet<Enrollment>();
        }

        [Key]
        public int CourseID { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int StudentsMax { get; set; }
        public int DepartmentID { get; set; }
        public int InstructorID { get; set; }

        public virtual Department Department { get; set; } = null!;
        public virtual Instructor Instructor { get; set; } = null!;
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}

