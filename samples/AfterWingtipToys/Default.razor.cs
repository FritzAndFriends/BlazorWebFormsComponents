namespace WingtipToys;

public partial class Default
{
    protected override async Task OnInitializedAsync()
    {
        Page.Title = "Home Page";
        await Task.CompletedTask;
    }
}
