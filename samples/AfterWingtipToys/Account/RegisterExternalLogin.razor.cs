// TODO(bwfc-general): This helper class referenced legacy Web Forms APIs and was stubbed during migration.
// Rebuild using ASP.NET Core equivalents (Identity, Authorization, Configuration).
namespace WingtipToys.Account;
public partial class RegisterExternalLogin
{
    private BlazorWebFormsComponents.TextBox email = default!;

    protected string ProviderName { get; set; } = "External";

    protected void LogIn_Click()
    {
        // TODO: Implement external login registration
    }
}
