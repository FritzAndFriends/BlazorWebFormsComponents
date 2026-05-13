# Recipe: OAuth / External Auth Page Stubs

## Error Signature

```
CS0103: The name 'providerDetails' does not exist in the current context
CS0103: The name 'ProviderName' does not exist in the current context
CS0103: The name 'LogIn_Click' does not exist in the current context
```

Multiple `CS0103` errors in Account pages related to OAuth, OWIN, or external login.

## Detection

```powershell
Select-String -Path **/Account/*.cs,**/Account/*.razor -Pattern "Microsoft\.Owin|OpenAuth|ExternalLogin|GetOwinContext"
```

## Root Cause

Web Forms ASP.NET Identity uses OWIN middleware (`Microsoft.Owin.Security`) for external OAuth providers (Google, Facebook, etc.). These assemblies don't exist in modern .NET. The CLI migrates the `.aspx` markup but the code-behind has unresolvable OWIN dependencies.

Common affected pages:
- `Account/OpenAuthProviders.razor` — lists external login buttons
- `Account/RegisterExternalLogin.razor` — handles OAuth callback
- `Account/ManageLogins.razor` — manage linked external accounts

## Fix — Create Compile-Safe Stubs

The goal is **compile safety**, not functional OAuth. Full external auth is an L3 architectural task.

### OpenAuthProviders Stub

```csharp
// Account/OpenAuthProviders.razor.cs
using Microsoft.AspNetCore.Components;

namespace MyApp.Account;

public partial class OpenAuthProviders : ComponentBase
{
    [Parameter] public string ReturnUrl { get; set; } = "/";
    private List<object> providerDetails { get; set; } = new();
}
```

```razor
@* Account/OpenAuthProviders.razor *@
<div class="external-login-providers">
    @if (!providerDetails.Any())
    {
        <p>No external login providers configured.</p>
    }
</div>
```

### RegisterExternalLogin Stub

```csharp
// Account/RegisterExternalLogin.razor.cs
using Microsoft.AspNetCore.Components;

namespace MyApp.Account;

public partial class RegisterExternalLogin : ComponentBase
{
    private string ProviderName { get; set; } = "";
    private string email { get; set; } = "";

    private void LogIn_Click() { /* TODO: implement external auth */ }
}
```

### ManageLogins Stub

```csharp
// Account/ManageLogins.razor.cs
using Microsoft.AspNetCore.Components;

namespace MyApp.Account;

public partial class ManageLogins : ComponentBase
{
    private List<object> CurrentLogins { get; set; } = new();
    private List<object> OtherLogins { get; set; } = new();
}
```

## Key Principle

These stubs make the app **compile and run** without external OAuth. The pages render harmlessly with "no providers configured" messages. When the team is ready for L3 identity work, they replace stubs with ASP.NET Core Identity external auth configuration.

## Verification

After creating stubs for all OAuth-dependent pages, the Account folder should compile cleanly. Verify with `dotnet build`.
