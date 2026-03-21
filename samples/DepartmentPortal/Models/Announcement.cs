using System;

namespace DepartmentPortal.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public DateTime PublishDate { get; set; }
        public bool IsActive { get; set; }
    }
}
