namespace ContosoUniversity.BLL;

/// <summary>
/// View model for the Students GridView joined data display.
/// </summary>
public class StudentViewModel
{
    public int ID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Date { get; set; } = string.Empty;
    public int Count { get; set; }
}
