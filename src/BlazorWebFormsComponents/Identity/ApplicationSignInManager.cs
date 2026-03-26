using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents.Identity;

/// <summary>
/// Compatibility shim for the application's SignInManager (typically derived from
/// <c>Microsoft.AspNet.Identity.Owin.SignInManager&lt;TUser, TKey&gt;</c>).
/// <para>
/// All methods are no-op stubs that return success. This provides enough API surface
/// for migrated Account page code-behinds to compile.
/// </para>
/// <para>
/// Future: Will delegate to <c>Microsoft.AspNetCore.Identity.SignInManager&lt;TUser&gt;</c>
/// once identity delegation is implemented (see GitHub issue #525).
/// </para>
/// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators
public class ApplicationSignInManager
{
    public virtual Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        => Task.FromResult(SignInStatus.Success);

    public virtual Task<bool> HasBeenVerifiedAsync()
        => Task.FromResult(false);

    public virtual Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser)
        => Task.FromResult(SignInStatus.Success);

    public virtual Task<string?> GetVerifiedUserIdAsync()
        => Task.FromResult<string?>(null);

    public virtual Task<IList<string>> GetValidTwoFactorProvidersAsync(string userId)
        => Task.FromResult<IList<string>>(new List<string>());

    public virtual Task<bool> SendTwoFactorCodeAsync(string provider)
        => Task.FromResult(false);

    public virtual Task SignInAsync(IdentityUser user, bool isPersistent, bool rememberBrowser)
        => Task.CompletedTask;
}
#pragma warning restore CS1998
