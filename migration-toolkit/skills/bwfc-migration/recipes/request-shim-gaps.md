# Recipe: RequestShim / ResponseShim Missing Members

## Error Signature

```
CS1061: 'RequestShim' does not contain a definition for 'IsLocal'
CS1061: 'ResponseShim' does not contain a definition for 'StatusCode'
CS1061: 'RequestShim' does not contain a definition for 'ServerVariables'
```

## Detection

```powershell
Select-String -Path **/*.cs -Pattern "Request\.\w+" | Where-Object {
    $_ -match "Request\.(IsLocal|ServerVariables|ClientCertificate|PhysicalPath|RawUrl)"
}
```

## Root Cause

BWFC's `RequestShim` wraps common `HttpRequest` properties (`QueryString`, `Cookies`, `Url`, `Form`, `HttpMethod`) but doesn't cover every member of the original `System.Web.HttpRequest`. When code accesses uncommon properties, the shim doesn't have them.

## Fix — Per Property

### `Request.IsLocal`
```csharp
// BEFORE:
if (Request.IsLocal) { /* show debug info */ }

// AFTER — safe dev-mode fallback:
var isLocal = true; // In production, use IHostEnvironment.IsDevelopment()
```

Or inject environment:
```csharp
[Inject] public IHostEnvironment Env { get; set; }
// Then: if (Env.IsDevelopment()) { ... }
```

### `Request.ServerVariables["REMOTE_ADDR"]`
```csharp
// AFTER — use HttpContext:
[Inject] public IHttpContextAccessor HttpContextAccessor { get; set; }
var ip = HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
```

### `Request.PhysicalPath` / `Request.PhysicalApplicationPath`
```csharp
// AFTER — use IWebHostEnvironment:
[Inject] public IWebHostEnvironment WebEnv { get; set; }
var path = Path.Combine(WebEnv.ContentRootPath, "subpath");
```

### `Request.RawUrl`
```csharp
// RequestShim provides Url.PathAndQuery — use that:
var rawUrl = Request.Url.PathAndQuery;
```

### `Response.StatusCode`
```csharp
// ResponseShim doesn't expose StatusCode.
// For error pages, use IHttpContextAccessor:
[Inject] public IHttpContextAccessor HttpContextAccessor { get; set; }
var statusCode = HttpContextAccessor.HttpContext?.Response.StatusCode ?? 200;
```

## Missing Utility Classes

Sometimes the error isn't about the shim but about app-specific utility classes that the CLI didn't migrate:

```
CS0103: The name 'ExceptionUtility' does not exist in the current context
```

**Fix:** Create a minimal stub:
```csharp
namespace MyApp.Logic
{
    public static class ExceptionUtility
    {
        public static void LogException(Exception exc, string source)
        {
            System.Diagnostics.Debug.WriteLine($"[{source}] {exc.Message}");
        }
    }
}
```

## Verification

After addressing each missing member, `dotnet build` should clear the `CS1061` errors. The stubs provide compile safety — full functional equivalence may require L3 architecture work.
