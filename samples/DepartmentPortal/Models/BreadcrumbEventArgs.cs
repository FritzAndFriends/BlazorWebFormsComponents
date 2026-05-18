using System;

namespace DepartmentPortal.Models
{
    public class BreadcrumbEventArgs : EventArgs
    {
        public int DepartmentId { get; set; }
        public string ItemName { get; set; }
        public string NavigationLevel { get; set; }
    }
}
