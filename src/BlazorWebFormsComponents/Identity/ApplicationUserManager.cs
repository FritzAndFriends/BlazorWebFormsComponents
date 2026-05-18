using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents.Identity;

/// <summary>
/// Compatibility shim for the application's UserManager (typically derived from
/// <c>Microsoft.AspNet.Identity.UserManager&lt;TUser&gt;</c>).
/// <para>
/// All methods are no-op stubs that return success or null. This provides enough API
/// surface for migrated Account page code-behinds to compile.
/// </para>
/// <para>
/// Future: Will delegate to <c>Microsoft.AspNetCore.Identity.UserManager&lt;TUser&gt;</c>
/// once identity delegation is implemented (see GitHub issue #525).
/// </para>
/// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators
public class ApplicationUserManager
{
    public virtual Task<IdentityUser?> FindByIdAsync(string userId)
        => Task.FromResult<IdentityUser?>(null);

    public virtual Task<IdentityUser?> FindByNameAsync(string name)
        => Task.FromResult<IdentityUser?>(null);

    public virtual Task<IdentityResult> CreateAsync(IdentityUser user, string password)
        => Task.FromResult(IdentityResult.Success);

    public virtual Task<IdentityResult> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        => Task.FromResult(IdentityResult.Success);

    public virtual Task<IdentityResult> AddPasswordAsync(string userId, string password)
        => Task.FromResult(IdentityResult.Success);

    public virtual Task<IdentityResult> RemovePasswordAsync(string userId)
        => Task.FromResult(IdentityResult.Success);

    public virtual Task<bool> HasPasswordAsync(string userId)
        => Task.FromResult(false);

    public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(string userId)
        => Task.FromResult<IList<UserLoginInfo>>(new List<UserLoginInfo>());

    public virtual Task<IdentityResult> RemoveLoginAsync(string userId, string provider, string key)
        => Task.FromResult(IdentityResult.Success);

    public virtual Task<string> GenerateChangePhoneNumberTokenAsync(string userId, string phoneNumber)
        => Task.FromResult(string.Empty);

    public virtual Task<IdentityResult> ChangePhoneNumberAsync(string userId, string phoneNumber, string code)
        => Task.FromResult(IdentityResult.Success);

    public virtual Task<IdentityResult> SetTwoFactorEnabledAsync(string userId, bool enabled)
        => Task.FromResult(IdentityResult.Success);

    public virtual Task<string> GeneratePasswordResetTokenAsync(string userId)
        => Task.FromResult(string.Empty);

    public virtual Task<IdentityResult> ResetPasswordAsync(string userId, string code, string newPassword)
        => Task.FromResult(IdentityResult.Success);

    public virtual Task<string> GenerateEmailConfirmationTokenAsync(string userId)
        => Task.FromResult(string.Empty);

    public virtual Task<IdentityResult> ConfirmEmailAsync(string userId, string code)
        => Task.FromResult(IdentityResult.Success);

    public virtual ClaimsIdentity CreateIdentity(IdentityUser user, string authenticationType)
        => new(authenticationType);
}
#pragma warning restore CS1998
