using System.Linq;

namespace WingtipToys.Account;

// TODO: Requires ASP.NET Core Identity migration — external auth providers need AuthenticationSchemeProvider
public partial class OpenAuthProviders
{
    private IQueryable<string> GetProviderNames(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
    {
        // TODO: Implement with AuthenticationSchemeProvider
        totalRowCount = 0;
        return Array.Empty<string>().AsQueryable();
    }
}
