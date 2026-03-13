namespace ContosoUniversity.Models
{
    using System;
    using System.Collections.Generic;

    public partial class Enrollment
    {
        public int EnrollmentID { get; set; }
        public DateTime Date { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }

        public virtual Cours Cours { get; set; }
        public virtual Student Student { get; set; }
    }
}


