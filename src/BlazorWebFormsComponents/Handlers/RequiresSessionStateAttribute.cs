using System;

namespace BlazorWebFormsComponents;

/// <summary>
/// Marker attribute indicating that an <see cref="HttpHandlerBase"/> subclass requires
/// session state access. When present, the <c>MapHandler</c> endpoint shim automatically
/// calls <c>HttpContext.Session.LoadAsync()</c> before invoking
/// <see cref="HttpHandlerBase.ProcessRequestAsync"/>.
/// </summary>
/// <remarks>
/// This is the ASP.NET Core equivalent of implementing <c>IRequiresSessionState</c>
/// in Web Forms. Requires ASP.NET Core session middleware to be configured:
/// <code>
/// builder.Services.AddDistributedMemoryCache();
/// builder.Services.AddSession();
/// // ...
/// app.UseSession();
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class RequiresSessionStateAttribute : Attribute
{
}
