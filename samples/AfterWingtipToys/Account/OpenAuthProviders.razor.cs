using Microsoft.AspNetCore.Components;

namespace WingtipToys.Account;

public partial class OpenAuthProviders
{
    [Parameter] public string ReturnUrl { get; set; } = "/";

    private List<object> providerDetails { get; } = new();
}
