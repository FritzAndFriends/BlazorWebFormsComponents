// Auto-generated API-compatible stub. Original referenced legacy Web Forms APIs.
// TODO(bwfc-general): Rebuild method bodies using ASP.NET Core equivalents.
namespace WingtipToys.Account;
public partial class Login
{
	protected string Title { get; set; } = "Log in";

    private TextBox Email = default!;
    private PlaceHolder ErrorMessage = default!;
    private Literal FailureText = default!;
    private HyperLink ForgotPasswordHyperLink = default!;
    private OpenAuthProviders OpenAuthLogin = default!;
    private TextBox Password = default!;
    private HyperLink RegisterHyperLink = default!;
    private CheckBox RememberMe = default!;


    protected void LogIn(object? sender, EventArgs e)
    {
        // TODO: migrate from Web Forms code-behind
    }

    [Parameter, SupplyParameterFromQuery(Name = "error")] public string? Error { get; set; }
    [Parameter, SupplyParameterFromQuery(Name = "registered")] public int? Registered { get; set; }
    [Parameter, SupplyParameterFromQuery(Name = "returnUrl")] public string? ReturnUrl { get; set; }
}
