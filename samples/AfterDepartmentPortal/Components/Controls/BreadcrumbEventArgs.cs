using System;

namespace AfterDepartmentPortal.Components.Controls
{
    public class BreadcrumbEventArgs : EventArgs
    {
        public int DepartmentId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string NavigationLevel { get; set; } = string.Empty;
    }
}
