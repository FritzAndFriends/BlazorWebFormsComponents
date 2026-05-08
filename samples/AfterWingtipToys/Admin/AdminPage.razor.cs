namespace WingtipToys.Admin;

public partial class AdminPage
{
    protected override Task OnInitializedAsync()
    {
        Title = "Administration";
        return base.OnInitializedAsync();
    }
}
