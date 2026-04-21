namespace WingtipToys.Account;

// TODO: Requires ASP.NET Core Identity migration — registration needs UserManager
public partial class Register
{
    private string Email { get; set; } = "";
    private string Password { get; set; } = "";

    private void CreateUser_Click(EventArgs e)
    {
        // TODO: Implement with UserManager
    }
}
