using BlazorWebFormsComponents;

namespace WingtipToys.Account;

public partial class OpenAuthProviders
{
	private ListView<string> providerDetails = default!;

	private IQueryable<string> GetProviderNames(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
	{
		totalRowCount = 0;
		return Array.Empty<string>().AsQueryable();
	}
}