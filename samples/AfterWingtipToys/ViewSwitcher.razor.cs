namespace WingtipToys;

public partial class ViewSwitcher
{
	protected string CurrentView { get; private set; } = "Desktop";
	protected string AlternateView { get; private set; } = "Mobile";
	protected string SwitchUrl { get; private set; } = "#";
}