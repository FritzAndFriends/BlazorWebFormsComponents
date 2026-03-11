namespace WingtipToys.Account;

// TODO: Requires ASP.NET Core Identity migration — Login page needs SignInManager/UserManager
public partial class Login
{
    private string Email { get; set; } = "";
    private string Password { get; set; } = "";

    private void LogIn(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
    {
        // TODO: Implement with SignInManager
    }
}
