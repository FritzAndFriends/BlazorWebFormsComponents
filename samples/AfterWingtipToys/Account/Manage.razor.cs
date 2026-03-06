using Microsoft.AspNetCore.Components;

namespace WingtipToys.Account;

public partial class Manage : ComponentBase
{
    private string Title => "Manage Account";
    private int LoginsCount { get; set; }
}
