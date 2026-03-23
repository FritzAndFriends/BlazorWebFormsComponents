using System;

namespace AfterDepartmentPortal.Components.Controls
{
    public class PollVoteEventArgs : EventArgs
    {
        public int SelectedIndex { get; set; }
        public string OptionText { get; set; } = string.Empty;
    }
}
