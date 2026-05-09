using Microsoft.AspNetCore.Components;

namespace WingtipToys;

public partial class Default
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    private void Page_Error()
    {
        var exc = Server.GetLastError();
        if (exc is InvalidOperationException)
        {
            NavigationManager.NavigateTo("/ErrorPage?handler=Page_Error%20-%20Default.aspx");
        }
    }
}
