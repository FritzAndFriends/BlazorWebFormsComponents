using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Enrollment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EnrollmentID { get; set; }

        public DateTime Date { get; set; }

        public int StudentID { get; set; }

        public int CourseID { get; set; }

        public virtual Cours Cours { get; set; }

        public virtual Student Student { get; set; }

    }
}

