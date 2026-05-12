namespace WingtipToys.Account;

public partial class RegisterExternalLogin
{
    private TextBox email = default!;

    protected string ProviderName { get; set; } = "external";

    protected void LogIn_Click()
    {
        Response.Redirect("/Account/Login");
    }
}
