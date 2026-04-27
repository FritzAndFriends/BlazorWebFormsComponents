namespace WingtipToys.Account;

public partial class Register
{
    private string? ErrorMessage => Request.QueryString["error"];
}
