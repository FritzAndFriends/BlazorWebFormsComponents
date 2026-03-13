namespace ContosoUniversity.Models
{
    using System;
    using System.Collections.Generic;

    public partial class Instructor
    {
        public Instructor()
        {
            this.Courses = new HashSet<Cours>();
        }

        public int InstructorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public System.DateTime BirthDate { get; set; }
        public string Email { get; set; }

        public virtual ICollection<Cours> Courses { get; set; }
    }
}


