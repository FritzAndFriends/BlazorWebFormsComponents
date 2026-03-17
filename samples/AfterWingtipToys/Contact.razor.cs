namespace WingtipToys;

public partial class Contact
{
    protected override async Task OnInitializedAsync()
    {
        Page.Title = "Contact";
        await Task.CompletedTask;
    }
}
