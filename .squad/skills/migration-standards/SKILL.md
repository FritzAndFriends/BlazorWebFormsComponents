---
name: "migration-standards"
description: "Canonical standards for migrating ASP.NET Web Forms applications to Blazor using BWFC"
domain: "migration"
confidence: "medium"
source: "earned"
---

## Context

When migrating an ASP.NET Web Forms application to Blazor using BlazorWebFormsComponents, these standards define the canonical target architecture, tooling choices, and migration patterns. Established through five WingtipToys migration benchmark runs and codified as a directive by Jeffrey T. Fritz.

Apply these standards to:
- Migration script (`bwfc-migrate.ps1`) enhancements
- Copilot-assisted Layer 2 work
- Migration documentation and checklists
- Any new migration test runs

## Patterns

### Target Architecture

| Setting | Standard |
|---|---|
| Framework | **.NET 10** (or latest LTS/.NET preview) |
| Project template | `dotnet new blazor --interactivity Server` |
| Render mode | Global Server Interactive |
| Base class | `WebFormsPageBase` for pages (`@inherits` in `_Imports.razor`); `ComponentBase` for non-page components |
| Service registration | `builder.Services.AddBlazorWebFormsComponents()` in `Program.cs` |
| Layout | `MainLayout.razor` with `@inherits LayoutComponentBase` and `@Body` |

### Render Mode Placement

> **`@rendermode` is a directive *attribute*, not a standalone directive.** It goes on component instances in markup, not in `_Imports.razor`.

**`_Imports.razor`** — add the static using so you can write `InteractiveServer` instead of `RenderMode.InteractiveServer`:

```razor
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@inherits BlazorWebFormsComponents.WebFormsPageBase
```

**`App.razor`** — apply render mode to the top-level routable components:

```razor
<HeadOutlet @rendermode="InteractiveServer" />
...
<Routes @rendermode="InteractiveServer" />
```

Do **not** place `@rendermode InteractiveServer` as a line in `_Imports.razor` — it will cause build errors (RZ10003, CS0103, RZ10024).

### Page Base Class

`WebFormsPageBase` eliminates per-page boilerplate when migrating Web Forms code-behind. A single `@inherits` directive in `_Imports.razor` gives all pages access to familiar Web Forms properties.

**One-time setup:**

1. **`_Imports.razor`** — add `@inherits BlazorWebFormsComponents.WebFormsPageBase`
2. **`Program.cs`** — add `builder.Services.AddBlazorWebFormsComponents()`
3. **Layout (`MainLayout.razor`)** — add `<BlazorWebFormsComponents.Page />` (renders `<PageTitle>` and `<meta>` tags)

**Properties available on every page:**

| Property | Behavior |
|---|---|
| `Title` | Delegates to `IPageService.Title` — `Page.Title = "X"` works unchanged |
| `MetaDescription` | Delegates to `IPageService.MetaDescription` |
| `MetaKeywords` | Delegates to `IPageService.MetaKeywords` |
| `IsPostBack` | Always returns `false` — `if (!IsPostBack)` always enters block |
| `Page` | Returns `this` — enables `Page.Title = "X"` dot syntax |

**When to still use `@inject IPageService`:** Non-page components (e.g., a shared header) that need access to page metadata should inject `IPageService` directly.

### Database Migration

- **Always** migrate EF6 → EF Core using the **latest .NET 10 packages** (currently **10.0.3**)
- Required packages: `Microsoft.EntityFrameworkCore` (10.0.3), `.SqlServer` / `.Sqlite`, `.Tools`, `.Design`
- Prefer SQLite for local dev / demos; SQL Server for production
- Replace `DropCreateDatabaseIfModelChanges` with `EnsureCreated` + idempotent seed
- Use `IDbContextFactory<T>` or scoped `DbContext` injection
- Models: nullable reference types, file-scoped namespaces, modern init patterns

### Identity Migration

- When ASP.NET Identity is present → prefer **ASP.NET Core Identity**
- OWIN middleware → ASP.NET Core middleware pipeline
- Postback-based auth → HTTP endpoints + cookie auth
- Use `dotnet aspnet-codegenerator identity` for scaffolding
- `SignInManager` / `UserManager` APIs change — full subsystem replacement

### Event Handler Strategy

BWFC components already expose EventCallback parameters with **matching Web Forms names**:

| Web Forms | BWFC | Action |
|---|---|---|
| `OnClick="Handler"` | `OnClick` (EventCallback<MouseEventArgs>) | **Preserve attribute verbatim** — only update handler signature |
| `OnCommand="Handler"` | `OnCommand` (EventCallback<CommandEventArgs>) | Preserve, update signature |
| `OnSelectedIndexChanged="Handler"` | `OnSelectedIndexChanged` (EventCallback<ChangeEventArgs>) | Preserve, update signature |
| `OnTextChanged="Handler"` | `OnTextChanged` (EventCallback<ChangeEventArgs>) | Preserve, update signature |
| `OnCheckedChanged="Handler"` | `OnCheckedChanged` (EventCallback<ChangeEventArgs>) | Preserve, update signature |

**Signature change pattern:**
```csharp
// Web Forms
protected void Button1_Click(object sender, EventArgs e) { ... }

// Blazor (BWFC)
private void Button1_Click(MouseEventArgs e) { ... }
// or
private async Task Button1_Click(MouseEventArgs e) { ... }
```

The script should preserve the attribute and annotate the signature change needed.

### Data Control Strategy — Prefer BWFC Over Raw HTML

| Web Forms Control | BWFC Component | Use Instead Of |
|---|---|---|
| `<asp:ListView>` | `<ListView Items="@data">` with `ItemTemplate` | `@foreach` + HTML table |
| `<asp:GridView>` | `<GridView Items="@data">` with columns | `@foreach` + `<table>` |
| `<asp:FormView>` | `<FormView Items="@data">` with `ItemTemplate` | Direct HTML rendering |
| `<asp:Repeater>` | `<Repeater Items="@data">` with `ItemTemplate` | `@foreach` loops |
| `<asp:DetailsView>` | `<DetailsView Items="@data">` with fields | Manual field rendering |
| `<asp:DataList>` | `<DataList Items="@data">` with `ItemTemplate` | `@foreach` + grid HTML |

**SelectMethod → Items:** Replace `SelectMethod="GetProducts"` with `Items="@_products"` where `_products` is populated in `OnInitializedAsync` via an injected service or DbContext.

### Session State → Scoped Services

- Replace `Session["key"]` with a scoped DI service
- Use `IHttpContextAccessor` for cookie-based persistence when needed
- Register in `Program.cs` with `builder.Services.AddScoped<TService>()`
- Example: `Session["CartId"]` → `CartStateService` with cookie-based cart ID

### Static Asset Relocation

- All static files → `wwwroot/`
- CSS bundles (`BundleConfig.cs`) → explicit `<link>` tags in `App.razor`
- JS bundles → explicit `<script>` tags in `App.razor`
- Image paths update: `~/Images/` → `/Images/` (only for tilde-prefixed paths; see **Static Asset Path Preservation** below)
- Font paths: same pattern

### Static Asset Path Preservation

**CRITICAL RULE:** When rewriting code-behind files during Layer 2, PRESERVE the source image/asset path structure. 

- The Layer 1 script copies static files preserving their original relative directory structure into `wwwroot/`.
- If the source uses `/Catalog/Images/{name}.png`, keep `/Catalog/Images/{name}.png` in Blazor templates.
- Do NOT change paths to match FreshWingtipToys or any other reference implementation convention unless you ALSO move the physical files.
- The physical files in `wwwroot/` are the source of truth. Check what paths exist BEFORE rewriting `src` attributes.

**Example of what NOT to do:**
- Source markup: `<img src="/Catalog/Images/Thumbs/car.png">`
- Physical files at: `wwwroot/Catalog/Images/Thumbs/car.png` ✅
- Layer 2 rewrites to: `<img src="/Images/Products/car.png">` ❌ (files don't exist there!)

**Correct approach:**
- Check `wwwroot/` for actual file locations
- Preserve source paths that match physical file locations
- Only change paths if you also create/move the files

### CSS Reference Verification

After Layer 2 completes, verify that App.razor's `<head>` section contains `<link>` tags for CSS files. The Layer 1 script should auto-detect CSS files in `wwwroot/Content/` and inject references, but verify this happened. If CSS links are missing, add them manually.

Bootstrap CSS is REQUIRED for proper navbar and layout styling. Missing CSS is a P0 failure.

### Page Lifecycle Mapping

| Web Forms | Blazor | Notes |
|---|---|---|
| `Page_Load` | `OnInitializedAsync` | One-time init |
| `Page_PreInit` | `OnInitializedAsync` (early) | Theme setup |
| `Page_PreRender` | `OnAfterRenderAsync` | Post-render logic |
| `IsPostBack` check | `if (!IsPostBack)` works AS-IS via `WebFormsPageBase` | Always enters block; `if (IsPostBack)` without `!` is dead code — flag for review |
| `Page.Title` | `Page.Title = "X"` works AS-IS via `WebFormsPageBase` | `WebFormsPageBase` delegates to `IPageService`. `<BlazorWebFormsComponents.Page />` in layout renders `<PageTitle>` and `<meta>` tags. |
| `Response.Redirect` | `NavigationManager.NavigateTo()` | Inject `NavigationManager` |

### Layer 1 (Script) vs Layer 2 (Manual) Boundary

**Script handles (Layer 1):**
- `asp:` prefix stripping (preserves BWFC tags)
- Data-binding expression conversion (5 variants)
- LoginView → **preserve as BWFC LoginView** (uses `AuthenticationStateProvider` natively)
- Master page → MainLayout.razor
- Scaffold generation (csproj, Program.cs, etc.)
- SelectMethod/GetRouteUrl flagging
- Register directive cleanup

**Always manual (Layer 2):**
- EF6 → EF Core (models, DbContext, seed)
- Identity/Auth subsystem
- Session → scoped services
- Business logic (checkout, payment, admin CRUD)
- Complex data-binding with arithmetic/method chains

## Examples

### Preserving a ListView (CORRECT)

```razor
@* Web Forms *@
<asp:ListView ID="productList" runat="server"
    DataKeyNames="ProductID" GroupItemCount="4"
    ItemType="WingtipToys.Models.Product"
    SelectMethod="GetProducts">
    <ItemTemplate>
        <td><%#: Item.ProductName %></td>
    </ItemTemplate>
</asp:ListView>

@* After migration (BWFC preserved) *@
<ListView Items="@_products" GroupItemCount="4">
    <ItemTemplate>
        <td>@context.ProductName</td>
    </ItemTemplate>
</ListView>

@code {
    [Inject] private ProductContext Db { get; set; }
    private List<Product> _products;

    protected override async Task OnInitializedAsync()
    {
        _products = await Db.Products.ToListAsync();
    }
}
```

### Preserving Event Handlers (CORRECT)

```razor
@* Web Forms *@
<asp:Button ID="btnRemove" runat="server" Text="Remove"
    OnClick="RemoveItem_Click" CommandArgument='<%# Item.ItemId %>' />

@* After migration (BWFC preserved) *@
<Button Text="Remove"
    OnClick="RemoveItem_Click" CommandArgument="@context.ItemId" />

@code {
    // Only signature changes — method name stays the same
    private async Task RemoveItem_Click(MouseEventArgs e) { ... }
}
```

## Anti-Patterns

### ❌ Replacing BWFC Data Controls with Raw HTML

```razor
@* WRONG — loses all BWFC functionality *@
@foreach (var product in _products)
{
    <tr>
        <td>@product.ProductName</td>
    </tr>
}

@* RIGHT — use BWFC ListView *@
<ListView Items="@_products">
    <ItemTemplate>
        <tr><td>@context.ProductName</td></tr>
    </ItemTemplate>
</ListView>
```

### ❌ Stripping Event Handler Attributes

```razor
@* WRONG — strips the handler, requires manual re-wiring *@
<Button Text="Submit" />
@* TODO: re-add click handler *@

@* RIGHT — preserve the attribute, only annotate signature change *@
<Button Text="Submit" OnClick="Submit_Click" />
@* TODO: Update Submit_Click signature: (object, EventArgs) → (MouseEventArgs) *@
```

### ❌ Using Page as Base Class

```csharp
// WRONG — Web Forms base class
public partial class ProductList : Page { }

// RIGHT — BWFC page base class (provides Page.Title, IsPostBack, etc.)
// Set via @inherits WebFormsPageBase in _Imports.razor
public partial class ProductList : WebFormsPageBase { }

// ALSO RIGHT — for non-page components
public partial class MyComponent : ComponentBase { }
```

## Static Asset Migration Checklist

Copy ALL of these from the Web Forms source to `wwwroot/` in the Blazor project:

| Source Folder | Destination | Contains |
|--------------|-------------|----------|
| `Content/` | `wwwroot/Content/` | CSS files (bootstrap, Site.css, etc.) |
| `Scripts/` | `wwwroot/Scripts/` | JavaScript (jQuery, Bootstrap JS, app scripts) |
| `Images/` | `wwwroot/Images/` | Site images, logos |
| `Catalog/` (or similar) | `wwwroot/Catalog/` | Product/content images |
| `fonts/` | `wwwroot/fonts/` | Web fonts |
| `favicon.ico` | `wwwroot/favicon.ico` | Favicon |

**Verification checklist:**
- [ ] All `<link>` CSS references in App.razor have matching physical files in wwwroot
- [ ] All `<script>` JS references in App.razor have matching physical files in wwwroot
- [ ] All `<img>` src paths in pages have matching physical files in wwwroot
- [ ] Logo image path works
- [ ] Product/catalog images load at correct paths
- [ ] Fonts load (check browser dev tools for 404s)

**Common miss:** The `Scripts/` folder is easy to forget because CSS is more visually
obvious when missing. Always verify JS files are present — jQuery and Bootstrap JS are
needed for interactive Bootstrap features (dropdowns, modals, collapse).

## ListView Template Placeholder Conversion

### The Problem

In ASP.NET Web Forms, LayoutTemplate and GroupTemplate use **placeholder elements** —
HTML elements with specific IDs that the runtime replaces with rendered content:

```aspx
<asp:ListView ...>
    <LayoutTemplate>
        <table>
            <tr id="groupPlaceholder"></tr>  ← Runtime replaces this with groups
        </table>
    </LayoutTemplate>
    <GroupTemplate>
        <tr>
            <td id="itemPlaceholder"></td>  ← Runtime replaces this with items
        </tr>
    </GroupTemplate>
</asp:ListView>
```

### The Blazor Equivalent

In BWFC Blazor, `LayoutTemplate` and `GroupTemplate` are `RenderFragment<RenderFragment>`
parameters. The child content is passed as `@context` — you must explicitly render it:

```razor
<ListView Items="@Products" GroupItemCount="4" TItem="Product">
    <LayoutTemplate>
        <table>
            @context   @* ← This IS the rendered groups *@
        </table>
    </LayoutTemplate>
    <GroupTemplate>
        <tr>
            @context   @* ← This IS the rendered items *@
        </tr>
    </GroupTemplate>
    <ItemTemplate>
        <td>@context.ProductName</td>
    </ItemTemplate>
</ListView>
```

### Migration Rule

**Replace any element with an ID containing "Placeholder" inside a *Template block
with `@context`.** The placeholder element is just a marker — `@context` is the
actual rendered content.

| Web Forms Pattern | Blazor Pattern |
|------------------|----------------|
| `<tr id="groupPlaceholder"></tr>` | `@context` |
| `<td id="itemPlaceholder"></td>` | `@context` |
| `<div id="groupPlaceholder" />` | `@context` |

### Without GroupItemCount

If the ListView doesn't use GroupItemCount, LayoutTemplate still needs `@context`:

```razor
<ListView Items="@Items" TItem="MyItem">
    <LayoutTemplate>
        <ul>
            @context   @* Items render here *@
        </ul>
    </LayoutTemplate>
    <ItemTemplate>
        <li>@context.Name</li>
    </ItemTemplate>
</ListView>
```

### Diagnostic

If a ListView renders its LayoutTemplate structure but shows no items, the most
likely cause is a missing `@context` in LayoutTemplate or GroupTemplate.

## Preserving Action Links in Detail Pages

When migrating detail pages (ProductDetails, FormView, etc.), ensure action links
are preserved. The script converts `<asp:HyperLink>` but may lose context about
the link's purpose.

**Common action links to verify after migration:**
- "Add to Cart" → `<a href="/AddToCart?productID=@context.ProductID">Add To Cart</a>`
- "Edit" → `<a href="/Edit?id=@context.ID">Edit</a>`
- "Delete" → button or link with confirmation

**Verify:** After Layer 1 (script) conversion, check that all action links from the
original page survive in the converted output. If any are missing, add them manually
in Layer 2 using the `@context.PropertyName` syntax for data-bound values.
