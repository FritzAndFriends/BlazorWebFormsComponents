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
