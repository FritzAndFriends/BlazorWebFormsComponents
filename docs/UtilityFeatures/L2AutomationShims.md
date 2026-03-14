# L2 Automation Shims

BlazorWebFormsComponents includes a set of **Layer 2 (L2) automation shims** — library-level features that eliminate the most common manual fixes required after automated migration. These shims exist because Blazor's Razor compiler is stricter than Web Forms' parser: Web Forms silently coerced strings to enums, resolved virtual paths, and provided ambient page properties that Blazor does not.

By absorbing these differences into the BWFC library itself, migrated markup compiles with fewer manual interventions.

## The Problem

Automated migration (Layer 1) converts Web Forms markup to Blazor syntax. But the converted code often needs **Layer 2 manual fixes** because:

| Web Forms Behavior | Blazor Behavior | Result |
|---|---|---|
| `GridLines="None"` — string parsed at runtime | Enum parameter requires `@(GridLines.None)` | ❌ Every enum attribute needs wrapping |
| `Width="125px"` — string parsed by `UnitConverter` | `Unit` type required explicit cast | ❌ Every width/height needs `Unit.Parse()` |
| `Response.Redirect("~/Products.aspx")` | No `Response` object exists | ❌ All redirects need rewriting |
| `ViewState["key"] = value` | No `ViewState` dictionary | ❌ All ViewState access needs replacement |
| `GetRouteUrl("Products", new { id })` | No `GetRouteUrl` method | ❌ Route URL generation needs rewriting |

Each of these generates **5-20+ manual fixes per page** across a typical application. The L2 shims eliminate them.

## Shim Overview

### 1. EnumParameter\<T\> — String-Accepting Enum Parameters

**Problem:** `GridLines="None"` won't compile because Blazor expects `GridLines="@(GridLines.None)"`

**Solution:** All 46 BWFC components with enum parameters now use `EnumParameter<T>`, which accepts both strings and enum values via implicit conversion.

```razor
@* Both work — no wrapping needed *@
<GridView GridLines="None" />
<GridView GridLines="@(GridLines.None)" />
```

📖 [Full documentation →](EnumParameter.md)

---

### 2. Implicit Unit Conversion — String-Accepting Width/Height

**Problem:** `Width="125px"` won't compile because `Unit` required explicit construction

**Solution:** The `Unit` struct now has an implicit conversion from `string`, delegating to `Unit.Parse()`.

```razor
@* String values work directly *@
<Panel Width="200px" Height="100%" />
<GridView Width="50em" />

@* Numeric values still work *@
<Panel Width="@(new Unit(200))" />
```

All CSS unit formats are supported: `px`, `pt`, `%`, `em`, `ex`, `in`, `cm`, `mm`.

---

### 3. Response.Redirect — Navigation Compatibility

**Problem:** `Response.Redirect("~/Products.aspx")` won't compile — there's no `Response` object in Blazor

**Solution:** `WebFormsPageBase` exposes a `Response` property returning a `ResponseShim` that wraps `NavigationManager`. The shim automatically strips `~/` prefixes and `.aspx` extensions.

```razor
@inherits WebFormsPageBase

@code {
    void GoToProducts() => Response.Redirect("~/Products.aspx");
    // Navigates to /Products
}
```

📖 [Full documentation →](ResponseRedirect.md)

---

### 4. Page-Level ViewState — In-Memory Dictionary

**Problem:** `ViewState["key"] = value` won't compile on pages that don't have a `ViewState` property

**Solution:** `WebFormsPageBase` provides a `Dictionary<string, object>` property named `ViewState`. Values persist for the lifetime of the component instance (equivalent to private fields).

```razor
@inherits WebFormsPageBase

@code {
    protected override void OnInitialized()
    {
        #pragma warning disable CS0618 // Suppress obsolete warning
        ViewState["ProductCount"] = 42;
        var count = (int)ViewState["ProductCount"];
        #pragma warning restore CS0618
    }
}
```

!!! note "Marked Obsolete"
    `ViewState` is marked `[Obsolete]` to generate compiler warnings encouraging developers to refactor to typed properties. The warnings guide migration without breaking compilation.

📖 [Existing documentation →](ViewState.md)

---

### 5. GetRouteUrl — Route URL Generation

**Problem:** `GetRouteUrl("Products", new { id = 5 })` won't compile — no such method in Blazor components

**Solution:** `WebFormsPageBase` provides a `GetRouteUrl()` method that delegates to ASP.NET Core's `LinkGenerator`. It strips `.aspx` from route names for compatibility.

```razor
@inherits WebFormsPageBase

@code {
    private string GetProductUrl(int id)
    {
        return GetRouteUrl("ProductDetails", new { id });
    }
}
```

## Using WebFormsPageBase

All page-level shims (Response, ViewState, GetRouteUrl, IsPostBack, Page.Title) are available through a single base class:

```razor
@inherits WebFormsPageBase
@page "/products"

<h1>@Page.Title</h1>

<GridView DataSource="@_products" GridLines="None" Width="100%" />

<Button Text="Dashboard" OnClick="GoToDashboard" />

@code {
    private List<Product> _products;

    protected override void OnInitialized()
    {
        Page.Title = "Product Catalog";

        if (!IsPostBack) // Always true in Blazor — guarded code always runs
        {
            _products = LoadProducts();
        }
    }

    private void GoToDashboard()
    {
        Response.Redirect("~/Admin/Dashboard.aspx");
    }
}
```

### Required Services

`WebFormsPageBase` requires several services to be registered. These are automatically configured when you call `AddBlazorWebFormsComponents()` in `Program.cs`:

```csharp
builder.Services.AddBlazorWebFormsComponents();
```

This registers:
- `IPageService` (scoped)
- `IHttpContextAccessor`
- `LinkGenerator` (provided by ASP.NET Core routing)

## Migration Impact

Based on migration testing with WingtipToys (19 pages) and ContosoUniversity (8 pages):

| Shim | Manual Fixes Eliminated Per Page | Total Across Typical App |
|---|---|---|
| EnumParameter\<T\> | 3-8 enum wrappings | 60-150+ fixes |
| Unit implicit conversion | 2-5 width/height fixes | 40-100+ fixes |
| Response.Redirect | 1-3 redirect rewrites | 20-60+ fixes |
| ViewState dictionary | 0-5 ViewState accesses | 0-100+ fixes |
| GetRouteUrl | 0-2 route URL calls | 0-40+ fixes |
| **Combined** | **6-23 fixes per page** | **120-450+ fixes eliminated** |

## See Also

- [EnumParameter\<T\>](EnumParameter.md) — Detailed documentation for string-accepting enums
- [Response.Redirect](ResponseRedirect.md) — Detailed documentation for navigation shim
- [ViewState](ViewState.md) — ViewState compatibility and migration path
- [WebFormsPage](WebFormsPage.md) — Page wrapper for naming containers and theming
- [Page System](PageService.md) — Title, MetaDescription, and other page services
- [Automated Migration Guide](../Migration/AutomatedMigration.md) — Using the migration scripts
