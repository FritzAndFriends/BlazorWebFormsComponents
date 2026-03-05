using WingtipToys.Models;

namespace WingtipToys.Account;

public partial class ManageLogins
{
    private List<UserLoginInfo> _logins = new();
    private bool _canRemoveExternalLogins = false;
    private string _successMessage = string.Empty;

    protected override void OnInitialized()
    {
        // Stub: no external login providers configured in this migration
        _logins = new List<UserLoginInfo>();
    }
}
