using BlazorWebFormsComponents;

namespace WingtipToys.Account;

public partial class RegisterExternalLogin
{
	private TextBox email = default!;

	protected string ProviderName { get; private set; } = string.Empty;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		ProviderName = Request.QueryString["providerName"].ToString();
		if (string.IsNullOrWhiteSpace(ProviderName))
		{
			ProviderName = "External";
		}
	}

	private void LogIn_Click()
	{
		Response.Redirect("/Account/Login");
	}
}