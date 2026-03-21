using System;

namespace DepartmentPortal.Models
{
    public class NotificationEventArgs : EventArgs
    {
        public int NotificationId { get; set; }
        public string NotificationText { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
