namespace WingtipToys.Account;

// TODO: Requires ASP.NET Core Identity migration — external login registration needs UserManager/SignInManager
public partial class RegisterExternalLogin
{
    private string ProviderName { get; set; } = "";
    private string Email { get; set; } = "";

    private void LogIn_Click(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
    {
        // TODO: Implement with UserManager/SignInManager
    }
}
