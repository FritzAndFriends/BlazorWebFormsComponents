# Page Lifecycle Migration

The migration script automatically transforms Web Forms page lifecycle methods (`Page_Init`, `Page_Load`, `Page_PreRender`) to their Blazor equivalents. Your page logic is preserved — only the method signatures change.

## Overview

**What it does:**
- Renames Web Forms lifecycle methods to Blazor component lifecycle overrides
- Adjusts method signatures (removes `sender` and `EventArgs` parameters)
- Preserves method body logic unchanged

**Why it matters:**
Web Forms has a [complex page lifecycle](https://learn.microsoft.com/en-us/previous-versions/aspnet/ms178472(v=vs.100)) with specific events fired in order: `Init → Load → PreRender → Render → Unload`. Blazor has a different but analogous component lifecycle. The migration script handles the mapping automatically, but understanding the correspondence helps you validate the results and handle edge cases.

## Lifecycle Mapping

The following table shows how Web Forms lifecycle methods map to Blazor:

| Web Forms | Blazor | Timing | Notes |
|-----------|--------|--------|-------|
| `Page_Init` | `OnInitialized()` | Sync, runs once | Called when the component is first initialized |
| `Page_Load` | `OnInitializedAsync()` | Async, runs once | Use for data loading; replaces the common `if (!IsPostBack)` pattern |
| `Page_PreRender` | `OnAfterRenderAsync(bool firstRender)` | Async, after render | Guard with `if (firstRender)` for one-time logic |
| `Page_Unload` | `Dispose()` via `IDisposable` | On component teardown | Implement `IDisposable` on the component |

!!! note
    In Web Forms, `Page_Load` runs on **every** postback. In Blazor, `OnInitializedAsync()` runs only **once** when the component initializes. If your `Page_Load` contained `if (!IsPostBack)` guards, the migrated code is correct — Blazor's `OnInitializedAsync()` is inherently "first load only."

## Before and After

=== "Web Forms (Original)"
    ```csharp
    // Products.aspx.cs
    public partial class Products : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            // Initialize controls
            CategoryDropDown.DataSource = GetCategories();
            CategoryDropDown.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // First load — fetch data
                ProductGrid.DataSource = GetProducts();
                ProductGrid.DataBind();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Update UI state before rendering
            ItemCountLabel.Text = $"Showing {ProductGrid.Rows.Count} items";
        }
    }
    ```

=== "Blazor (After Migration)"
    ```csharp
    // Products.razor.cs
    public partial class Products : ComponentBase
    {
        protected override void OnInitialized()
        {
            // Initialize controls (was Page_Init)
            CategoryDropDown.DataSource = GetCategories();
            CategoryDropDown.DataBind();
        }

        protected override async Task OnInitializedAsync()
        {
            // First load — fetch data (was Page_Load)
            // No IsPostBack check needed — runs once
            ProductGrid.DataSource = GetProducts();
            ProductGrid.DataBind();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Update UI state after first render (was Page_PreRender)
                ItemCountLabel.Text = $"Showing {ProductGrid.Rows.Count} items";
            }
        }
    }
    ```

## Automated Transformation

The migration script (`bwfc-migrate.ps1`) handles these transformations automatically:

### What the Script Does

1. **Renames methods** — `Page_Init` → `OnInitialized`, `Page_Load` → `OnInitializedAsync`, `Page_PreRender` → `OnAfterRenderAsync`
2. **Removes parameters** — Strips `(object sender, EventArgs e)` from the signature
3. **Adds override keyword** — Changes `protected void` to `protected override void` (or `async Task`)
4. **Wraps PreRender body** — Adds `if (firstRender) { ... }` guard to `OnAfterRenderAsync`
5. **Preserves method body** — All logic inside the method is left unchanged

### Example Transformation

```csharp
// INPUT: Web Forms
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        LoadProducts();
    }
}

// OUTPUT: Blazor (automated)
protected override async Task OnInitializedAsync()
{
    if (!IsPostBack)
    {
        LoadProducts();
    }
}
```

!!! tip
    The `IsPostBack` property is provided by the BWFC `WebFormsPage` base class and always returns `false` in Blazor, making the guard effectively a no-op. You can safely remove it in a later cleanup phase.

## Manual Review Checklist

After the automated migration, review the following:

### 1. Check for `sender` Usage in Method Body

If the original method body references the `sender` parameter, the automated transform will remove the parameter but leave the body reference, causing a compilation error:

```csharp
// Web Forms — uses sender
protected void Page_Init(object sender, EventArgs e)
{
    var page = (Page)sender;  // ← References sender
    page.Title = "Products";
}

// After migration — sender is gone, body breaks
protected override void OnInitialized()
{
    var page = (Page)sender;  // ❌ Compile error: 'sender' doesn't exist
    page.Title = "Products";
}
```

**Fix:** Replace `sender` references with direct property access or dependency injection:

```csharp
protected override void OnInitialized()
{
    // Use the component's own properties instead
    PageTitle = "Products";
}
```

### 2. Check for `EventArgs` Usage in Method Body

Similarly, if the method body uses the `e` parameter:

```csharp
// Web Forms — uses e
protected void Page_Load(object sender, EventArgs e)
{
    LogEvent(e.ToString());
}

// Fix: Remove or replace the EventArgs reference
protected override async Task OnInitializedAsync()
{
    LogEvent("Page initialized");  // Simplified
}
```

### 3. Multiple Page_Load Handlers

Web Forms allows wiring multiple handlers to the same event. If your page has this pattern, consolidate into a single Blazor lifecycle method:

```csharp
// Web Forms — multiple handlers (rare but possible)
this.Load += Page_Load;
this.Load += Page_LoadAdditional;

// Blazor — combine into one
protected override async Task OnInitializedAsync()
{
    // Logic from Page_Load
    LoadProducts();
    
    // Logic from Page_LoadAdditional
    LoadPromotions();
}
```

### 4. Async Data Loading

If `Page_Load` calls async methods synchronously (common in Web Forms), the Blazor migration is an opportunity to improve:

```csharp
// Web Forms — sync-over-async (anti-pattern)
protected void Page_Load(object sender, EventArgs e)
{
    var products = GetProductsAsync().Result;  // Blocking call
}

// Blazor — proper async (recommended cleanup)
protected override async Task OnInitializedAsync()
{
    var products = await GetProductsAsync();  // Non-blocking
}
```

## Lifecycle Execution Order

For reference, here's how the lifecycle methods execute in each framework:

=== "Web Forms Lifecycle"
    ```
    Page_PreInit
         ↓
    Page_Init          ← Component initialization
         ↓
    Page_InitComplete
         ↓
    Page_Load          ← Data loading (every request)
         ↓
    Page_LoadComplete
         ↓
    [Event Handlers]   ← Button clicks, grid commands, etc.
         ↓
    Page_PreRender     ← Final UI adjustments
         ↓
    Page_Render        ← HTML output
         ↓
    Page_Unload        ← Cleanup
    ```

=== "Blazor Component Lifecycle"
    ```
    OnInitialized()         ← Sync initialization (once)
         ↓
    OnInitializedAsync()    ← Async initialization (once)
         ↓
    OnParametersSet()       ← Parameters received
         ↓
    OnParametersSetAsync()  ← Async parameter processing
         ↓
    [Render]                ← HTML output
         ↓
    OnAfterRender(first)    ← Post-render logic
         ↓
    OnAfterRenderAsync(first) ← Async post-render
         ↓
    Dispose()               ← Cleanup (via IDisposable)
    ```

## Summary

- ✅ `Page_Init` → `OnInitialized()` — automatic
- ✅ `Page_Load` → `OnInitializedAsync()` — automatic
- ✅ `Page_PreRender` → `OnAfterRenderAsync(firstRender)` — automatic with guard
- ⚠️ Review method bodies for `sender` or `e` parameter references
- ⚠️ `IsPostBack` checks are safe to leave (always `false`) but can be removed later
- 🔄 Consider converting sync-over-async patterns to proper `await` calls

See [WebFormsPage](../UtilityFeatures/WebFormsPage.md) for details on the `IsPostBack` shim and other page-level compatibility features.
