namespace ContosoUniversity.Models
{
    public class StudentListItem
    {
        public int ID { get; set; }
        public string Date { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
