namespace WingtipToys.Account;

public partial class Login
{
    private string? ErrorMessage => Request.QueryString["error"];
}
