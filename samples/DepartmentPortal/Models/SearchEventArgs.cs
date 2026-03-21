using System;

namespace DepartmentPortal.Models
{
    public class SearchEventArgs : EventArgs
    {
        public string SearchTerm { get; set; }
        public string Category { get; set; }
    }
}
