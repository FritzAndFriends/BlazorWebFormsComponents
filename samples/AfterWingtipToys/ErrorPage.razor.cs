namespace WingtipToys;

public partial class ErrorPage
{
    protected override Task OnInitializedAsync()
    {
        Title = "Error";
        return base.OnInitializedAsync();
    }
}
