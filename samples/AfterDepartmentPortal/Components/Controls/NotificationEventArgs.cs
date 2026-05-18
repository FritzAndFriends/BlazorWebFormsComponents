using System;

namespace AfterDepartmentPortal.Components.Controls
{
    public class NotificationEventArgs : EventArgs
    {
        public int NotificationId { get; set; }
        public string NotificationText { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
