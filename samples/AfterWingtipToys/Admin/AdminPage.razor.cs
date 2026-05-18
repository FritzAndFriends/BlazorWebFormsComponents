namespace WingtipToys.Admin;

public partial class AdminPage : BlazorWebFormsComponents.WebFormsPageBase
{
    protected override Task OnInitializedAsync()
    {
        Title = "Administration";
        return base.OnInitializedAsync();
    }
}
