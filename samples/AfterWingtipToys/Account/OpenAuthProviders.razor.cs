// TODO(bwfc-general): This helper class referenced legacy Web Forms APIs and was stubbed during migration.
// Rebuild using ASP.NET Core equivalents (Identity, Authorization, Configuration).
using System.Collections.Generic;

namespace WingtipToys.Account;
public partial class OpenAuthProviders
{
    private BlazorWebFormsComponents.ListView<string> providerDetails = default!;

    public List<string> GetProviderNames()
    {
        return new List<string>();
    }
}
