// Auto-generated API-compatible stub. Original referenced legacy Web Forms APIs.
// TODO(bwfc-general): Rebuild method bodies using ASP.NET Core equivalents.
namespace WingtipToys.Account;
public partial class Register
{
	protected string Title { get; set; } = "Register";

    private TextBox ConfirmPassword = default!;
    private TextBox Email = default!;
    private Literal ErrorMessage = default!;
    private TextBox Password = default!;


    protected void CreateUser_Click(object? sender, EventArgs e)
    {
        // TODO: migrate from Web Forms code-behind
    }

    [Parameter, SupplyParameterFromQuery(Name = "error")] public string? Error { get; set; }
    [Parameter, SupplyParameterFromQuery(Name = "returnUrl")] public string? ReturnUrl { get; set; }
}
