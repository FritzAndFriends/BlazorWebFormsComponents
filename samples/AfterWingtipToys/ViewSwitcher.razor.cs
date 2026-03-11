using Microsoft.AspNetCore.Components;

namespace WingtipToys
{
    // ViewSwitcher is a legacy mobile/desktop view toggle from Web Forms FriendlyUrls.
    // In Blazor, responsive design replaces this pattern. Kept as a no-op component.
    public partial class ViewSwitcher
    {
        protected string CurrentView { get; private set; } = "Desktop";
        protected string AlternateView { get; private set; } = "Mobile";
        protected string SwitchUrl { get; private set; } = "#";
    }
}
