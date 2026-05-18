using System;

namespace BlazorWebFormsComponents.Identity;

/// <summary>
/// Compatibility shim for Microsoft.AspNet.Identity.EntityFramework.IdentityDbContext&lt;TUser&gt;.
/// <para>
/// In EF6, Identity stored its tables (AspNetUsers, AspNetRoles, etc.) in a DbContext
/// derived from <c>IdentityDbContext&lt;TUser&gt;</c>. In EF Core, use
/// <c>Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext&lt;TUser&gt;</c>
/// from the <c>Microsoft.AspNetCore.Identity.EntityFrameworkCore</c> package instead.
/// </para>
/// <para>
/// This shim allows migrated code to compile at L1. Layer 2 should replace it with the
/// real EF Core Identity DbContext and add the Microsoft.AspNetCore.Identity.EntityFrameworkCore
/// NuGet package.
/// </para>
/// </summary>
[Obsolete("Replace with Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<TUser>")]
public class IdentityDbContext<TUser> where TUser : IdentityUser
{
    public IdentityDbContext() { }
}
