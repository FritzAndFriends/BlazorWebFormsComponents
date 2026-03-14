using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    [Table("Courses")]
    public class Cours
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseID { get; set; }

        [Required]
        [MaxLength(30)]
        public string CourseName { get; set; }

        public int StudentsMax { get; set; }

        public int DepartmentID { get; set; }

        public int InstructorID { get; set; }

        public virtual Department Department { get; set; }

        public virtual Instructor Instructor { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }

    }
}

