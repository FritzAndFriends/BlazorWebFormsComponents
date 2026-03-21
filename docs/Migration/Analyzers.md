# Roslyn Analyzers for Web Forms Migration

The **BlazorWebFormsComponents.Analyzers** NuGet package includes 8 Roslyn analyzers with automatic code fixes that detect leftover Web Forms patterns in your C# code after migration. They run live in Visual Studio and `dotnet build`, giving you a guided checklist for cleaning up migrated code-behind.

!!! tip "When to Use These Analyzers"
    Install the analyzer package **after** you've run the automated migration script (`bwfc-migrate.ps1`) and the Copilot skill transforms. The analyzers catch code-behind patterns that markup-level tools can't address — things like `ViewState["key"]` usage, `IsPostBack` checks, and `Response.Redirect()` calls that survived the initial migration.

---

## Installation

Add the analyzer package to your Blazor project:

```shell
dotnet add package BlazorWebFormsComponents.Analyzers
```

Or add the reference directly to your `.csproj`:

```xml
<PackageReference Include="BlazorWebFormsComponents.Analyzers" Version="*" />
```

Once installed, diagnostics appear automatically in Visual Studio's Error List and during `dotnet build`. Each diagnostic includes a code fix you can apply with **Ctrl+.** (Quick Actions).

---

## Analyzer Summary

| Rule ID | Severity | Name | What It Detects |
|---------|----------|------|-----------------|
| [BWFC001](#bwfc001-missing-parameter-attribute) | ⚠️ Warning | Missing `[Parameter]` Attribute | Public properties on WebControl subclasses without `[Parameter]` |
| [BWFC002](#bwfc002-viewstate-usage) | ⚠️ Warning | ViewState Usage | `ViewState["key"]` access patterns |
| [BWFC003](#bwfc003-ispostback-usage) | ⚠️ Warning | IsPostBack Usage | `IsPostBack` or `Page.IsPostBack` checks |
| [BWFC004](#bwfc004-responseredirect-usage) | ⚠️ Warning | Response.Redirect Usage | `Response.Redirect()` calls |
| [BWFC005](#bwfc005-session-state-usage) | ⚠️ Warning | Session State Usage | `Session["key"]` and `HttpContext.Current` access |
| [BWFC010](#bwfc010-required-attribute-missing) | ℹ️ Info | Required Attribute Missing | BWFC components instantiated without critical properties |
| [BWFC011](#bwfc011-event-handler-signature) | ℹ️ Info | Event Handler Signature | Methods with `(object sender, EventArgs e)` signature |
| [BWFC012](#bwfc012-runatserver-leftover) | ⚠️ Warning | runat="server" Leftover | String literals containing `runat="server"` |
| [BWFC013](#bwfc013-response-object-usage) | ⚠️ Warning | Response Object Usage | `Response.Write()`, `Response.WriteFile()`, `Response.Clear()`, `Response.Flush()`, `Response.End()` |
| [BWFC014](#bwfc014-request-object-usage) | ⚠️ Warning | Request Object Usage | `Request.Form[]`, `Request.Cookies[]`, `Request.Headers[]`, `Request.Files`, `Request.QueryString[]`, `Request.ServerVariables[]` |

---

## BWFC001: Missing `[Parameter]` Attribute

**Severity:** ⚠️ Warning  
**Category:** Usage

### What It Detects

Public properties on classes that derive from `WebControl` or `CompositeControl` that don't have a `[Parameter]` attribute. In Blazor, any property that should accept values from markup **must** be decorated with `[Parameter]` — otherwise the value is silently ignored.

The analyzer skips properties that are inherited from BWFC base classes (like `ID`, `CssClass`, `Visible`, `Enabled`, etc.) and static properties.

### Why It Matters

In Web Forms, public properties on server controls are automatically available in markup. In Blazor, the `[Parameter]` attribute is required to wire properties to component markup. Missing this attribute is one of the most common issues after migrating custom controls — the component renders but silently ignores values passed from parent components.

### Example

=== "Before (triggers BWFC001)"
    ```csharp
    public class ProductCard : WebControl
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }
    ```

=== "After (code fix applied)"
    ```csharp
    using Microsoft.AspNetCore.Components;

    public class ProductCard : WebControl
    {
        [Parameter]
        public string ProductName { get; set; }

        [Parameter]
        public decimal Price { get; set; }
    }
    ```

### Code Fix

Adds `[Parameter]` to the property and inserts `using Microsoft.AspNetCore.Components;` if not already present.

---

## BWFC002: ViewState Usage

**Severity:** ⚠️ Warning  
**Category:** Usage

### What It Detects

`ViewState["key"]` and `this.ViewState["key"]` element-access patterns in code-behind. Blazor has no ViewState mechanism — it uses in-memory component state instead.

### Why It Matters

ViewState was the persistence mechanism for Web Forms page lifecycle. In Blazor, components live in memory on the server (Blazor Server) or in the browser (Blazor WebAssembly). State is naturally preserved across re-renders as regular C# fields and properties — no serialization needed. Any leftover `ViewState` access will either fail at compile time or use the BWFC compatibility shim, which is meant as a stepping stone, not a long-term solution.

### Example

=== "Before (triggers BWFC002)"
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ViewState["SortDirection"] = "ASC";
        }

        var direction = (string)ViewState["SortDirection"];
    }
    ```

=== "After (code fix applied)"
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // TODO: Replace ViewState["SortDirection"] with component state
            // ViewState["SortDirection"] = "ASC";
        }

        // TODO: Replace ViewState["SortDirection"] with component state
        // var direction = (string)ViewState["SortDirection"];
    }
    ```

### Code Fix

Comments out the statement and adds a `// TODO` prompting you to replace it with a component field or property:

```csharp
private string SortDirection { get; set; } = "ASC";
```

!!! note "See Also"
    The [ViewState utility documentation](../UtilityFeatures/ViewState.md) explains the BWFC compatibility shim and the recommended path to component state.

---

## BWFC003: IsPostBack Usage

**Severity:** ⚠️ Warning  
**Category:** Usage

### What It Detects

References to `IsPostBack` and `Page.IsPostBack` in code-behind. Blazor does not use the postback model — it uses component lifecycle methods instead.

### Why It Matters

In Web Forms, `IsPostBack` distinguished between the initial page load and subsequent postbacks. In Blazor, this distinction maps to lifecycle methods:

| Web Forms Pattern | Blazor Equivalent |
|-------------------|-------------------|
| `if (!IsPostBack)` (first load) | `OnInitialized` / `OnInitializedAsync` |
| `if (IsPostBack)` (postback) | Event handlers (`@onclick`, `@onchange`, etc.) |
| `Page_Load` (every request) | `OnParametersSet` / `OnParametersSetAsync` |

### Example

=== "Before (triggers BWFC003)"
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindGrid();
        }
    }
    ```

=== "After (code fix applied)"
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        // TODO: Replace IsPostBack check with Blazor lifecycle (OnInitialized/OnParametersSet)
        // if (!IsPostBack)
        // {
        //     BindGrid();
        // }
    }
    ```

### Code Fix

Comments out the `IsPostBack` block and adds a `// TODO` suggesting the appropriate lifecycle method. The recommended migration:

```csharp
protected override void OnInitialized()
{
    BindGrid();
}
```

---

## BWFC004: Response.Redirect Usage

**Severity:** ⚠️ Warning  
**Category:** Usage

### What It Detects

Calls to `Response.Redirect()`, `this.Response.Redirect()`, and `HttpContext.Current.Response.Redirect()`. Blazor does not have an `HttpResponse` object available in components — navigation uses `NavigationManager` instead.

### Why It Matters

`Response.Redirect()` works by sending an HTTP 302 response to the browser. Blazor Server communicates over a WebSocket (SignalR) connection, so there's no HTTP response to write to. Blazor WebAssembly runs entirely client-side. In both cases, `NavigationManager.NavigateTo()` is the correct way to perform navigation.

### Example

=== "Before (triggers BWFC004)"
    ```csharp
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        if (ValidateCredentials())
        {
            Response.Redirect("~/Dashboard.aspx");
        }
    }
    ```

=== "After (code fix applied)"
    ```csharp
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        if (ValidateCredentials())
        {
            // TODO: Replace Response.Redirect("~/Dashboard.aspx") with NavigationManager.NavigateTo("url")
            ;
        }
    }
    ```

### Code Fix

Replaces the statement with a `// TODO` comment pointing to `NavigationManager.NavigateTo()`. The recommended migration:

```csharp
@inject NavigationManager Navigation

// In your method:
Navigation.NavigateTo("/dashboard");
```

!!! note "See Also"
    The [Response.Redirect utility documentation](../UtilityFeatures/ResponseRedirect.md) explains the BWFC compatibility shim for `Response.Redirect`.

---

## BWFC005: Session State Usage

**Severity:** ⚠️ Warning  
**Category:** Usage

### What It Detects

Two patterns:

1. **Session element access** — `Session["key"]` and `this.Session["key"]`
2. **HttpContext.Current** — Any access to `HttpContext.Current`, which is not available in Blazor

### Why It Matters

Web Forms relies heavily on `HttpContext.Current` and `Session` for per-user state. In Blazor Server, there is no `HttpContext` during SignalR hub invocations (which is most of the component lifecycle). Session state should be replaced with:

- **Scoped services** — registered with `builder.Services.AddScoped<TService>()`, these are per-circuit in Blazor Server
- **`ProtectedSessionStorage`** — for browser session-scoped data that survives page reloads
- **`ProtectedLocalStorage`** — for data that persists across browser sessions

### Example

=== "Before (triggers BWFC005)"
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        var userId = Session["UserId"];
        var context = HttpContext.Current;
        var userName = context.User.Identity.Name;
    }
    ```

=== "After (code fix applied)"
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        // TODO: Replace Session["UserId"] with scoped service or ProtectedSessionStorage
        ;
        // TODO: Replace HttpContext.Current with scoped service or ProtectedSessionStorage
        ;
        var userName = context.User.Identity.Name;
    }
    ```

### Code Fix

Replaces the statement with a `// TODO` comment pointing to scoped services or `ProtectedSessionStorage`.

---

## BWFC010: Required Attribute Missing

**Severity:** ℹ️ Info  
**Category:** Usage

### What It Detects

When well-known BWFC component types are instantiated in code-behind without setting critical properties. Currently tracks:

| Component | Required Property |
|-----------|-------------------|
| `GridView` | `DataSource` |
| `HyperLink` | `NavigateUrl` |
| `Image` | `ImageUrl` |

The analyzer checks both object initializer syntax and subsequent property assignments on the same variable.

### Why It Matters

These components won't render correctly without their required properties. When you create a `GridView` in code-behind without setting `DataSource`, it renders an empty table. This analyzer is a safety net for migrated code where data binding was previously done declaratively in ASPX markup and might have been missed during migration.

### Example

=== "Before (triggers BWFC010)"
    ```csharp
    var grid = new GridView();
    grid.AutoGenerateColumns = true;
    // Missing: grid.DataSource = products;
    ```

=== "After (code fix applied)"
    ```csharp
    // TODO: Set GridView.DataSource — required for correct rendering
    var grid = new GridView();
    grid.AutoGenerateColumns = true;
    // Missing: grid.DataSource = products;
    ```

### Code Fix

Adds a `// TODO` comment above the creation statement reminding you to set the required property.

---

## BWFC011: Event Handler Signature

**Severity:** ℹ️ Info  
**Category:** Usage

### What It Detects

Methods in Blazor component classes (those deriving from `ComponentBase` or BWFC base classes) that have the classic Web Forms event handler signature: `(object sender, EventArgs e)`.

### Why It Matters

In Web Forms, event handlers always followed the `(object sender, EventArgs e)` pattern because the framework wired them up via delegates. In Blazor, events use `EventCallback` and `EventCallback<T>`, which don't pass a `sender` object. Methods with the old signature won't bind correctly to Blazor's `@onclick`, `@onchange`, and other event attributes.

### Example

=== "Before (triggers BWFC011)"
    ```csharp
    public class ProductPage : WebControl
    {
        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveProduct();
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterProducts();
        }
    }
    ```

=== "After (code fix applied)"
    ```csharp
    public class ProductPage : WebControl
    {
        // TODO: Convert to EventCallback pattern — remove sender parameter, change return type if needed
        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveProduct();
        }

        // TODO: Convert to EventCallback pattern — remove sender parameter, change return type if needed
        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterProducts();
        }
    }
    ```

### Code Fix

Adds a `// TODO` comment above the method. The recommended migration:

```csharp
// Simple click handler — no parameters needed
private void SaveProduct()
{
    // Your existing logic
}

// If you need event data, use the specific args type
private void FilterProducts(ChangeEventArgs e)
{
    var selectedValue = e.Value?.ToString();
    // Your existing logic
}
```

---

## BWFC012: runat="server" Leftover

**Severity:** ⚠️ Warning  
**Category:** Usage

### What It Detects

String literals in C# code that contain `runat="server"` or `runat='server'` (case-insensitive). This typically occurs in code that dynamically builds HTML markup or in test fixtures that reference ASPX patterns.

### Why It Matters

`runat="server"` was required on every Web Forms server control to distinguish it from plain HTML. Blazor has no concept of this attribute — all components are inherently server-processed. Leftover `runat="server"` in string literals is dead code at best and confusing at worst.

### Example

=== "Before (triggers BWFC012)"
    ```csharp
    private string GetControlMarkup()
    {
        return "<asp:TextBox ID=\"txtName\" runat=\"server\" />";
    }
    ```

=== "After (code fix applied)"
    ```csharp
    private string GetControlMarkup()
    {
        return "<asp:TextBox ID=\"txtName\" />";
    }
    ```

### Code Fix

Removes the `runat="server"` substring (and any leading whitespace before it) from the string literal.

---

## BWFC013: Response Object Usage

**Severity:** ⚠️ Warning  
**Category:** Usage

### What It Detects

Calls to `Response.Write()`, `Response.WriteFile()`, `Response.Clear()`, `Response.Flush()`, and `Response.End()`. Blazor has no `HttpResponse` object available in components — it uses markup rendering and result objects instead.

### Why It Matters

The Web Forms `Response` object is tightly coupled to the HTTP request-response cycle. In Blazor Server (which uses SignalR over WebSocket) and Blazor WebAssembly (which runs client-side), there is no `HttpResponse` to write to. These methods either fail at runtime or require the BWFC compatibility shim, which is meant as a stepping stone, not a long-term solution.

Each method maps to a different Blazor pattern:

| Web Forms Method | Blazor Equivalent |
|-----------------|-------------------|
| `Response.Write()` | Markup rendering (component state + template) |
| `Response.WriteFile()` | `FileResult` (from minimal API) or `HttpClient` for file downloads |
| `Response.Clear()` | Not needed — use `@if` conditionals in markup |
| `Response.Flush()` | Not needed — streaming is automatic in Blazor Server |
| `Response.End()` | Early return from event handler + state update |

### Example

=== "Before (triggers BWFC013)"
    ```csharp
    protected void ExportButton_Click(object sender, EventArgs e)
    {
        Response.Clear();
        Response.Write("<html><body>");
        Response.Write(GenerateReport());
        Response.Write("</body></html>");
        Response.End();
    }
    ```

=== "After (code fix applied)"
    ```csharp
    // TODO: Replace Response.Write/Clear/End with markup rendering or FileResult
    protected void ExportButton_Click(object sender, EventArgs e)
    {
        // TODO: Replace Response.Write/Clear/End with markup rendering or FileResult
        // TODO: Replace Response.Write/Clear/End with markup rendering or FileResult
        // TODO: Replace Response.Write/Clear/End with markup rendering or FileResult
        // TODO: Replace Response.Write/Clear/End with markup rendering or FileResult
    }
    ```

### Code Fix

Comments out each `Response` method call and adds a `// TODO` comment pointing to the appropriate Blazor pattern.

### Recommended Patterns

**For writing HTML content to the page:**

Instead of `Response.Write()`, use component state and markup:

```csharp
@page "/report"
@implements IAsyncDisposable

<div>
    @if (!string.IsNullOrEmpty(ReportHtml))
    {
        @((MarkupString)ReportHtml)
    }
</div>

@code {
    private string ReportHtml { get; set; }

    private void ExportButton_Click()
    {
        ReportHtml = GenerateReport();
    }
}
```

**For file downloads:**

Use a minimal API endpoint that returns `FileResult`:

```csharp
// In Program.cs or a service:
app.MapGet("/api/export-report", async () =>
{
    var fileBytes = GenerateReportFile();
    return Results.File(fileBytes, "application/pdf", "report.pdf");
});

// In your component:
@inject NavigationManager Navigation

private void ExportButton_Click()
{
    Navigation.NavigateTo("/api/export-report", forceLoad: true);
}
```

---

## BWFC014: Request Object Usage

**Severity:** ⚠️ Warning  
**Category:** Usage

### What It Detects

Access to `Request.Form[]`, `Request.Cookies[]`, `Request.Headers[]`, `Request.Files`, `Request.QueryString[]`, and `Request.ServerVariables[]`. Blazor does not expose an `HttpRequest` object in components — data flows through components properties, form binding, and `HttpContextAccessor` (Blazor Server only).

### Why It Matters

The Web Forms `Request` object provided a centralized way to access browser-sent data. In Blazor:

- **Query strings** — Passed as route parameters or via `NavigationManager.Uri`
- **Form data** — Bound to component properties using `@bind`
- **Cookies** — Accessed via `HttpContextAccessor` (Blazor Server only) or JavaScript interop
- **Headers** — Accessed via `HttpContextAccessor` (Blazor Server only)
- **Uploaded files** — Handled by `InputFile` component, not raw file access

Leaving `Request` calls in place will fail at runtime or require compatibility shims.

### Example

=== "Before (triggers BWFC014)"
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        var userId = Request.QueryString["id"];
        var email = Request.Form["email"];
        var sessionId = Request.Cookies["session"];
        var apiKey = Request.Headers["X-API-Key"];
        
        if (Request.Files.Count > 0)
        {
            var file = Request.Files[0];
        }
    }
    ```

=== "After (code fix applied)"
    ```csharp
    // TODO: Replace Request.QueryString[] with route parameters or NavigationManager.Uri
    protected void Page_Load(object sender, EventArgs e)
    {
        // TODO: Replace Request.QueryString[] with route parameters or NavigationManager.Uri
        // TODO: Replace Request.Form[] with @bind or parameter binding
        // TODO: Replace Request.Cookies[] with HttpContextAccessor (Blazor Server) or JS interop
        // TODO: Replace Request.Headers[] with HttpContextAccessor (Blazor Server)
        
        // TODO: Replace Request.Files with InputFile component
    }
    ```

### Code Fix

Comments out each `Request` access and adds a `// TODO` comment pointing to the appropriate Blazor pattern.

### Recommended Patterns

**For query string parameters:**

Use route parameters in your `@page` directive:

```razor
@page "/products/{ProductId:int}"

<h1>Product @ProductId</h1>

@code {
    [Parameter]
    public int ProductId { get; set; }
}
```

**For form data:**

Use `@bind` two-way binding:

```razor
<input @bind="Email" />
<button @onclick="HandleSubmit">Submit</button>

@code {
    private string Email { get; set; }

    private void HandleSubmit()
    {
        // Email is already populated
    }
}
```

**For cookies (Blazor Server):**

Use `HttpContextAccessor`:

```csharp
@inject HttpContextAccessor HttpContextAccessor

@code {
    private string GetCookie(string name)
    {
        var context = HttpContextAccessor.HttpContext;
        if (context?.Request.Cookies.TryGetValue(name, out var value) ?? false)
        {
            return value;
        }
        return null;
    }
}
```

**For headers (Blazor Server):**

Also use `HttpContextAccessor`:

```csharp
private string GetHeader(string name)
{
    var context = HttpContextAccessor.HttpContext;
    if (context?.Request.Headers.TryGetValue(name, out var value) ?? false)
    {
        return value.ToString();
    }
    return null;
}
```

**For file uploads:**

Use the `InputFile` component:

```razor
<InputFile OnChange="HandleFileSelected" />

@code {
    private async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        using var stream = file.OpenReadStream(maxAllowedSize: 1_000_000);
        // Process file stream
    }
}
```

!!! note "See Also"
    The [InputFile documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/file-uploads) explains how to use the component for file handling in Blazor.

---

## Using Analyzers in CI/CD

The analyzers integrate seamlessly with `dotnet build` and CI/CD pipelines. You can configure severity levels per rule and fail the build on violations.

### Example: .editorconfig for CI Enforcement

```ini
# .editorconfig — enforce analyzer rules in CI

# Mandatory rules: fail the build
dotnet_diagnostic.BWFC001.severity = error
dotnet_diagnostic.BWFC003.severity = error
dotnet_diagnostic.BWFC004.severity = error

# Important patterns: treat as warnings
dotnet_diagnostic.BWFC002.severity = warning
dotnet_diagnostic.BWFC005.severity = warning
dotnet_diagnostic.BWFC011.severity = warning
dotnet_diagnostic.BWFC012.severity = warning
dotnet_diagnostic.BWFC013.severity = warning
dotnet_diagnostic.BWFC014.severity = warning

# Informational patterns: visible but don't block build
dotnet_diagnostic.BWFC010.severity = suggestion
```

### CI Workflow

```bash
# Build the project — BWFC violations will appear as compiler warnings/errors
dotnet build MyBlazorProject.csproj

# In CI scripts, check for BWFC violations:
dotnet build MyBlazorProject.csproj --no-restore 2>&1 | grep "BWFC"
if [ $? -eq 0 ]; then
    echo "Web Forms migration violations detected"
    exit 1
fi
```

### Notes

- All BWFC diagnostics use the format `BWFC{NNN}` and can be configured via standard Roslyn mechanisms
- Build-time diagnostics are consistent with editor diagnostics
- Use `.editorconfig` to customize severity per project, not per file
- For bulk suppression (migrating a large codebase), set all rules to `suggestion` initially, then upgrade as you clean up

---

## Prioritization Guide: Which Rules to Fix First

If you're migrating a large application, fix analyzers in this order:

### Phase 1: Blocking Patterns (Fix First)

These patterns prevent your components from working at all:

1. **BWFC001** — Missing `[Parameter]` on public properties
   - Components silently ignore bound values → visual regression
   - Usually quick fixes (add one attribute per property)

2. **BWFC003** — `IsPostBack` checks
   - Affects page initialization and event handling
   - Core logic may depend on this check
   - Need to refactor to `OnInitialized` / event handlers

3. **BWFC004** — `Response.Redirect()` calls
   - All navigation breaks → user can't move between pages
   - Quick 1:1 replacement with `NavigationManager.NavigateTo()`

4. **BWFC011** — Old event handler signatures
   - Event handlers won't fire → broken interactions
   - Prevents using Blazor's `@onclick`, `@onchange`, etc.

### Phase 2: Data & State Patterns (Fix Next)

These affect how data flows through your app:

5. **BWFC002** — `ViewState` usage
   - Replace with component fields/properties
   - May require refactoring persistence logic

6. **BWFC005** — `Session` and `HttpContext` access
   - Replace with scoped services, protected storage, etc.
   - Medium complexity depending on scope

7. **BWFC014** — `Request` object usage
   - Replace with route parameters, `@bind`, `InputFile`, etc.
   - Usually straightforward per-instance fixes

### Phase 3: Output & Response Patterns (Fix Last)

These are less common and more specialized:

8. **BWFC013** — `Response.Write()`, `Response.WriteFile()`, etc.
   - Less common in modern applications
   - Usually isolated to reporting/export features

9. **BWFC012** — `runat="server"` in strings
   - Pure string cleanup, no logic impact
   - Can be done last as a polish pass

10. **BWFC010** — Missing required attributes
    - Usually caught by testing; low risk
    - Fix as you discover missing data

---

## Configuring and Suppressing Rules

### Using `.editorconfig`

You can adjust severity or disable specific rules using an `.editorconfig` file in your project:

```ini
# Disable a specific rule
dotnet_diagnostic.BWFC001.severity = none

# Downgrade a warning to info
dotnet_diagnostic.BWFC004.severity = suggestion

# Upgrade an info to warning
dotnet_diagnostic.BWFC011.severity = warning
```

### Using Pragma Directives

Suppress a diagnostic on a specific line:

```csharp
#pragma warning disable BWFC002
    var value = ViewState["LegacyKey"];
#pragma warning restore BWFC002
```

### Using `[SuppressMessage]`

Suppress at method or class level:

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "BWFC005",
    Justification = "HttpContext.Current is available in this middleware context")]
public void ConfigureServices()
{
    // ...
}
```

### Bulk Suppression Strategy

If you have a large migrated codebase and want to address analyzers incrementally, consider setting all rules to `suggestion` initially, then upgrading them to `warning` as you clean up each category:

```ini
# Phase 1: See everything as suggestions
dotnet_diagnostic.BWFC001.severity = suggestion
dotnet_diagnostic.BWFC002.severity = suggestion
dotnet_diagnostic.BWFC003.severity = suggestion
dotnet_diagnostic.BWFC004.severity = suggestion
dotnet_diagnostic.BWFC005.severity = suggestion

# Phase 2: Enforce rules you've cleaned up
dotnet_diagnostic.BWFC001.severity = warning
dotnet_diagnostic.BWFC012.severity = warning
```

---

## Recommended Workflow

1. **Run the automated migration** — Use `bwfc-migrate.ps1` and the Copilot `webforms-migration` skill to convert your markup and code-behind
2. **Install the analyzer package** — Add `BlazorWebFormsComponents.Analyzers` to your migrated project
3. **Build and review** — Run `dotnet build` and review the diagnostics in Visual Studio's Error List
4. **Apply code fixes** — Use **Ctrl+.** on each diagnostic to apply the suggested code fix
5. **Address TODOs** — The code fixes insert `// TODO` comments that guide your manual refactoring
6. **Clean up incrementally** — Use `.editorconfig` to manage which rules are active as you progress through the migration
