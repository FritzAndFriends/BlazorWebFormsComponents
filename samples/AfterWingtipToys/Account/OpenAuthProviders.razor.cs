namespace WingtipToys.Account;

public partial class OpenAuthProviders
{
    private ListView<string> providerDetails = default!;

    [Parameter] public string? ReturnUrl { get; set; }

    public IEnumerable<string> GetProviderNames() => Array.Empty<string>();
}
