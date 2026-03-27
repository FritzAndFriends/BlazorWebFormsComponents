using System;

namespace BlazorWebFormsComponents.Identity;

/// <summary>
/// Compatibility shim for Microsoft.AspNet.Identity.EntityFramework.IdentityUser.
/// <para>
/// Provides the same property surface as the ASP.NET Identity v2 IdentityUser so that
/// migrated code-behinds compile without changes. All properties are functional for
/// in-memory use; persistence requires Layer 2 migration to ASP.NET Core Identity.
/// </para>
/// <para>
/// Future: This type will delegate to <c>Microsoft.AspNetCore.Identity.IdentityUser</c>
/// once identity delegation is implemented (see GitHub issue #525).
/// </para>
/// </summary>
public class IdentityUser
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
}
