namespace WingtipToys;

public partial class About
{
    protected override async Task OnInitializedAsync()
    {
        Page.Title = "About";
        await Task.CompletedTask;
    }
}
