using Microsoft.AspNetCore.Components;

namespace WingtipToys.Account;

public partial class OpenAuthProviders : ComponentBase
{
    [Parameter] public string? ReturnUrl { get; set; }

    private List<string> _providers = new();
}
