namespace WingtipToys.Models;

/// <summary>
/// Stub model for external login info, replacing Microsoft.AspNet.Identity.UserLoginInfo.
/// </summary>
public class UserLoginInfo
{
    public string LoginProvider { get; set; } = string.Empty;
    public string ProviderKey { get; set; } = string.Empty;
}
