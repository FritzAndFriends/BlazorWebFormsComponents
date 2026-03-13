namespace ContosoUniversity.Models
{
    public class Instructor
    {
        public Instructor()
        {
            Courses = new HashSet<Cours>();
        }

        public int InstructorID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Email { get; set; } = string.Empty;

        public virtual ICollection<Cours> Courses { get; set; }
    }
}

