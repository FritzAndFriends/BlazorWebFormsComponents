namespace DepartmentPortal.Models
{
    public class TrainingCourse
    {
        public int Id { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public string Instructor { get; set; }
        public int DurationHours { get; set; }
        public string Category { get; set; }
    }
}
