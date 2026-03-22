namespace AfterDepartmentPortal.Models;

public class TrainingCourse
{
    public int Id { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public int DurationHours { get; set; }
    public string Category { get; set; } = string.Empty;
}
