# Service Registration

BlazorWebFormsComponents provides a single extension method to register all required services in your ASP.NET Core application. This is the recommended way to set up the library.

## Setup

In your `Program.cs`, call `AddBlazorWebFormsComponents()` on your service collection:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBlazorWebFormsComponents();

// ... rest of your setup
```

That's it — one line handles everything the library needs.

## What Gets Registered

The `AddBlazorWebFormsComponents()` method registers the following services:

| Service | Lifetime | Purpose |
|---------|----------|---------|
| `IHttpContextAccessor` | Singleton | Used by `BaseWebFormsComponent` for route URL helpers and request context. Registered via reflection so the library doesn't need a framework reference. |
| `BlazorWebFormsJsInterop` | Scoped | Handles automatic JavaScript module loading for features like `OnClientClick` and page title management. |
| `IPageService` / `PageService` | Scoped | Provides the page lifecycle and metadata services used by `WebFormsPageBase`. |

### IHttpContextAccessor — Automatic Registration

In earlier versions, consuming applications had to manually call `services.AddHttpContextAccessor()` in their `Program.cs`. This is no longer required.

The library now registers `IHttpContextAccessor` automatically using reflection. Since every ASP.NET Core application has the `Microsoft.AspNetCore.Http` assembly loaded at runtime, the reflection call succeeds transparently. If the assembly isn't available (e.g., in a unit test host), the call is silently skipped — no exceptions are thrown.

!!! tip "Migration Tip"
    If your `Program.cs` already has `services.AddHttpContextAccessor()`, you can safely remove it. The library's registration uses `TryAddSingleton` internally, so duplicate calls are harmless but unnecessary.

## Usage with WebFormsPageBase

If your pages inherit from `WebFormsPageBase` (the recommended base class for migrated pages), the services registered here provide the underlying infrastructure:

```razor
@* In your _Imports.razor *@
@inherits BlazorWebFormsComponents.WebFormsPageBase

@* Your pages automatically get access to: *@
@* - Page.Title (via IPageService) *@
@* - Request context (via IHttpContextAccessor) *@
@* - NamingContainer support *@
@* - Theme provider integration *@
```

See [WebFormsPage](WebFormsPage.md) for more details on the page base class.

## See Also

- [JavaScript Setup](JavaScriptSetup.md) — Details on how JavaScript loading works
- [WebFormsPage](WebFormsPage.md) — The recommended page base class
- [Page System](PageService.md) — The page service and lifecycle features
