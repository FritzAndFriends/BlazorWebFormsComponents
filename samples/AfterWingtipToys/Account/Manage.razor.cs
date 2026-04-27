namespace WingtipToys.Account;

public partial class Manage
{
    private string? CurrentUserEmail => Session.Get<string>(Services.UserStoreService.CurrentUserSessionKey);
}
