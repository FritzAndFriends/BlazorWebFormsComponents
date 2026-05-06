using Microsoft.AspNetCore.Components;

namespace WingtipToys
{
    public partial class ViewSwitcher
    {
        [Parameter] public string CurrentView { get; set; } = "Desktop";
        [Parameter] public string SwitchUrl { get; set; } = "?view=mobile";
        [Parameter] public string AlternateView { get; set; } = "Mobile";
    }
}
