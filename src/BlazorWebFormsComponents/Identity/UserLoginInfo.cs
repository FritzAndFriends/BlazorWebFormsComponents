namespace BlazorWebFormsComponents.Identity;

/// <summary>
/// Compatibility shim for Microsoft.AspNet.Identity.UserLoginInfo.
/// Represents an external login (e.g., Google, Facebook) associated with a user account.
/// </summary>
public class UserLoginInfo
{
    public string LoginProvider { get; set; }
    public string ProviderKey { get; set; }

    public UserLoginInfo(string loginProvider, string providerKey)
    {
        LoginProvider = loginProvider;
        ProviderKey = providerKey;
    }
}
