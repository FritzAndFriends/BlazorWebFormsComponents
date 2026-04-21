namespace WingtipToys.Account;

// TODO: Requires ASP.NET Core Identity migration — password reset needs UserManager
public partial class ResetPassword
{
    private string Email { get; set; } = "";
    private string Password { get; set; } = "";

    private void Reset_Click(EventArgs e)
    {
        // TODO: Implement with UserManager
    }
}
