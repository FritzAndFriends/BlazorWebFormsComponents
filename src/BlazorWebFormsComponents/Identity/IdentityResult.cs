using System;
using System.Collections.Generic;

namespace BlazorWebFormsComponents.Identity;

/// <summary>
/// Compatibility shim for Microsoft.AspNet.Identity.IdentityResult.
/// <para>
/// Represents the result of an identity operation (create user, change password, etc.).
/// Matches the ASP.NET Identity v2 API surface so migrated code compiles without changes.
/// </para>
/// </summary>
public class IdentityResult
{
    public bool Succeeded { get; set; }
    public IEnumerable<string> Errors { get; set; } = Array.Empty<string>();

    public static IdentityResult Success => new() { Succeeded = true };

    public static IdentityResult Failed(params string[] errors)
        => new() { Succeeded = false, Errors = errors };
}
