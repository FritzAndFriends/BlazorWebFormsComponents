namespace AfterDepartmentPortal.Models;

public class Announcement
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public bool IsActive { get; set; }
}
