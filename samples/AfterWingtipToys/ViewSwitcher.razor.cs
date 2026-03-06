using Microsoft.AspNetCore.Components;

namespace WingtipToys;

public partial class ViewSwitcher : ComponentBase
{
    private string CurrentView => "Desktop";
    private string AlternateView => "Mobile";
    private string SwitchUrl => "#";
}
