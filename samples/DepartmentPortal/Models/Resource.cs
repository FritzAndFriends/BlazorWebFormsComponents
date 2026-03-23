namespace DepartmentPortal.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Url { get; set; }
        public string FileType { get; set; }
    }
}
