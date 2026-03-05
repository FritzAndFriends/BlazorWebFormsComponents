---
name: "migration-standards"
description: "Canonical standards for migrating ASP.NET Web Forms applications to Blazor using BWFC"
domain: "migration"
confidence: "high"
source: "earned"
---

## Context

When migrating an ASP.NET Web Forms application to Blazor using BlazorWebFormsComponents, these standards define the canonical target architecture, tooling choices, and migration patterns. Established through five WingtipToys migration benchmark runs and codified as a directive by Jeffrey T. Fritz.

Apply these standards to:
- Migration script (`bwfc-migrate.ps1`) enhancements
- Copilot-assisted Layer 2 work
- Migration documentation and checklists
- Any new migration test runs

---

## 🚫 BWFC CONTROL PRESERVATION — MANDATORY (ZERO EXCEPTIONS)

> **Jeff's directive:** "We need to ALWAYS preserve the default asp: controls by using the BWFC components."
>
> **This is the #1 rule of every migration. No exceptions. No shortcuts.**

**The entire purpose of BlazorWebFormsComponents is that these components exist — USE THEM.**

Every `asp:` control in the Web Forms source MUST become a BWFC component in the migration output. The migration strips the `asp:` prefix (`<asp:GridView>` → `<GridView>`) and converts Web Forms attributes to BWFC parameters. The resulting markup should look nearly identical to the original, minus the `asp:` prefix and `runat="server"`.

### The 5 Mandatory Rules

1. **ALL `asp:` controls MUST be preserved as BWFC components.** The script strips the prefix; the component name stays.
2. **NEVER flatten data controls to raw HTML.** GridView, ListView, Repeater, DataList, DataGrid, DetailsView, FormView → MUST remain as BWFC components.
3. **NEVER flatten editor controls to raw HTML elements.** TextBox → `<input>`, CheckBox → `<input type="checkbox">`, Button → `<button>`, Label → `<span>` are all FORBIDDEN.
4. **NEVER flatten navigation/login/structural controls.** HyperLink, ImageButton, LinkButton, Panel, PlaceHolder, LoginView, LoginStatus, LoginName → MUST remain as BWFC components.
5. **The migration script `Test-BwfcControlPreservation` verification function runs post-transform** to catch any control loss.

### Complete BWFC Component Inventory (110+ Components)

The BWFC library provides drop-in replacements for ALL of these controls. Every one MUST be used:

| Category | Components |
|----------|-----------|
| **Editor Controls** (33) | AdRotator, Button, BulletedList, Calendar, Chart, CheckBox, CheckBoxList, DropDownList, FileUpload, HiddenField, HyperLink, Image, ImageButton, ImageMap, Label, LinkButton, ListBox, Literal, MultiView, Panel, PlaceHolder, RadioButton, RadioButtonList, Substitution, Table, TableCell, TableRow, TableHeaderRow, TableFooterRow, TableHeaderCell, TextBox, Timer, View |
| **Data Controls** (22+) | BoundField, ButtonField, DataGrid, DataList, DataPager, DetailsView, FormView, GridView, GridViewRow, HyperLinkField, ListView, Repeater, TemplateField + style sub-components: AlternatingItemStyle, EditRowStyle, FooterStyle, HeaderStyle, ItemStyle, PagerStyle, PagerSettings, RowStyle, SelectedRowStyle |
| **Navigation** (6+) | Menu, MenuItem, MenuItemStyle, SiteMapPath, TreeView, TreeNode + node style sub-components |
| **Login** (18+) | ChangePassword, CreateUserWizard, Login, **LoginName**, **LoginStatus**, **LoginView**, PasswordRecovery, RoleGroup + style sub-components |
| **Validation** (7) | CompareValidator, CustomValidator, ModelErrorMessage, RangeValidator, RegularExpressionValidator, RequiredFieldValidator, ValidationSummary |
| **Layout/AJAX** (14) | Content, ContentPlaceHolder, MasterPage, EmptyLayout, NamingContainer, ScriptManager, ScriptManagerProxy, Timer, UpdatePanel, UpdateProgress |
| **Utility** | WebFormsPageBase, Page (render component), AddBlazorWebFormsComponents(), BaseWebFormsComponent, BaseStyledComponent, ThemeProvider, FontInfo |

### BWFC Utility Features — MANDATORY for Every Migration

1. **`AddBlazorWebFormsComponents()`** in `Program.cs` — registers IPageService, IHttpContextAccessor, JS interop
2. **`@inherits WebFormsPageBase`** in `_Imports.razor` — provides Page.Title, IsPostBack, GetRouteUrl
3. **`<BlazorWebFormsComponents.Page />`** in `MainLayout.razor` — renders PageTitle and meta tags
4. **`BaseStyledComponent`** — provides CssClass, BackColor, ForeColor, Font, Border*, Height, Width, ToolTip

### ⚠️ Commonly Missed Controls: LoginView and LoginStatus

`<asp:LoginView>` and `<asp:LoginStatus>` are BWFC components. They are the **most frequently broken** controls during migration — agents consistently replace them with raw `@if` blocks or `<a>` tags.

```razor
@* ❌ WRONG — LoginView replaced with @if block *@
@if (isAuthenticated) { <span>Welcome</span> } else { <a href="/login">Log in</a> }

@* ✅ CORRECT — BWFC LoginView preserved *@
<LoginView>
    <AnonymousTemplate><a href="/Account/Login">Log in</a></AnonymousTemplate>
    <LoggedInTemplate>Welcome, <LoginName />! <LoginStatus LogoutAction="Redirect" LogoutPageUrl="/" /></LoggedInTemplate>
</LoginView>
```

### What "Flattening" Means (Quick Reference)

| BWFC Component (✅ CORRECT) | Flattened HTML (❌ FORBIDDEN) |
|---|---|
| `<GridView Items="@data">` | `<table>` + `@foreach` loop |
| `<ListView Items="@data">` | `@foreach` + HTML divs |
| `<TextBox @bind-Text="val" />` | `<input @bind="val" />` |
| `<Label Text="Hello" />` | `<span>Hello</span>` |
| `<HyperLink NavigateUrl="/x" />` | `<a href="/x">` |
| `<Button Text="Go" OnClick="X" />` | `<button @onclick="X">` |
| `<LoginView>` | `@if (isAuth) { ... }` |
| `<LoginStatus />` | `<a href="/logout">` |
| `<Panel CssClass="x">` | `<div class="x">` |

### Concrete Example: ShoppingCart GridView (WingtipToys)

The AfterWingtipToys `ShoppingCart.razor` was migrated BEFORE the migration scripts existed. Someone (AI or human) decomposed the `<asp:GridView>` into a plain HTML `<table>` with `@foreach`. This destroyed:
- Editable TextBox for quantity (became read-only)
- CheckBox for item removal (gone)
- Update/Checkout buttons (gone)
- CssClass stripes, GridLines, CellPadding (degraded)
- ShowFooter with totals (gone)

The cart became **read-only** — users could not edit quantities or check out. Meanwhile, the BWFC GridView supports ALL of these features. Zero component gaps.

### Anti-Pattern: Flattened GridView (BAD)

```razor
@* ❌ BAD — Someone decomposed the GridView into raw HTML *@
<table class="table">
    <thead>
        <tr>
            <th>Product</th>
            <th>Price</th>
            <th>Quantity</th>
            <th>Total</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in _cartItems)
        {
            <tr>
                <td>@item.Product.ProductName</td>
                <td>@item.Product.UnitPrice?.ToString("c")</td>
                <td>@item.Quantity</td>
                <td>@((item.Quantity * item.Product.UnitPrice)?.ToString("c"))</td>
            </tr>
        }
    </tbody>
</table>
```

### Correct Pattern: Preserved GridView (GOOD)

```razor
@* ✅ GOOD — GridView preserved as BWFC component with full functionality *@
<GridView TItem="CartItem" Items="@_cartItems"
    AutoGenerateColumns="false" CssClass="table table-striped"
    ShowFooter="true" GridLines="GridLines.Both" CellPadding="5">
    <Columns>
        <BoundField DataField="Product.ProductName" HeaderText="Product" />
        <BoundField DataField="Product.UnitPrice" HeaderText="Price"
            DataFormatString="{0:c}" />
        <TemplateField HeaderText="Quantity">
            <ItemTemplate>
                <TextBox Text="@context.Quantity.ToString()" />
            </ItemTemplate>
        </TemplateField>
        <TemplateField HeaderText="Remove">
            <ItemTemplate>
                <CheckBox />
            </ItemTemplate>
        </TemplateField>
        <TemplateField HeaderText="Total">
            <ItemTemplate>
                @((context.Quantity * context.Product.UnitPrice)?.ToString("c"))
            </ItemTemplate>
        </TemplateField>
    </Columns>
</GridView>
```

### Why This Matters

- **CSS preservation:** BWFC components render the same HTML as Web Forms controls. Raw HTML tables don't.
- **Feature parity:** GridView has sorting, paging, editing, footer totals, GridLines, CellPadding built-in. A `@foreach` loop has none of these.
- **Migration velocity:** Preserving the component means the markup is 90% done after `asp:` prefix stripping. Flattening requires rewriting the entire page.
- **Fidelity guarantee:** The `Test-BwfcControlPreservation` function in the migration script catches control loss automatically.

---

## Patterns

### Target Architecture

| Setting | Standard |
|---|---|
| Framework | **.NET 10** (or latest LTS/.NET preview) |
| Project template | `dotnet new blazor --interactivity Server` |
| Render mode | Global Server Interactive (see [Render Mode Placement](#render-mode-placement) below) |
| Base class | `WebFormsPageBase` for pages (`@inherits` in `_Imports.razor`); `ComponentBase` for non-page components |
| Layout | `MainLayout.razor` with `@inherits LayoutComponentBase` and `@Body` |
| **BWFC service registration** | **`builder.Services.AddBlazorWebFormsComponents()`** in `Program.cs` — MANDATORY |
| **Page render component** | **`<BlazorWebFormsComponents.Page />`** in `MainLayout.razor` — MANDATORY |

### Standard Blazor Server-Side Patterns (for infrastructure)

These patterns use standard Blazor/ASP.NET Core features (NOT BWFC components) for application infrastructure:

| Concern | Pattern | Where |
|---------|---------|-------|
| **Static files** | `app.UseStaticFiles()` + `app.MapStaticAssets()` | `Program.cs` — `UseStaticFiles()` MUST come first |
| **CSS links** | `<link rel="stylesheet" href="/Content/Site.css" />` | `App.razor` `<head>` section |
| **JS references** | `<script src="/Scripts/app.js"></script>` | `App.razor` after `<Routes>` |
| **BWFC JS** | `<script src="_content/Fritz.BlazorWebFormsComponents/js/Basepage.js"></script>` | `App.razor` `<head>` |
| **Render mode** | `@rendermode="InteractiveServer"` on `HeadOutlet` and `Routes` | `App.razor` |
| **CSS isolation** | `{Component}.razor.css` for component-scoped styles | Per-component |
| **CSS bundles** | `BundleConfig.cs` → explicit `<link>` tags in `App.razor` | `App.razor` `<head>` |
| **JS bundles** | `Scripts.Render(...)` → explicit `<script>` tags in `App.razor` | `App.razor` |

### Render Mode Placement

> **`@rendermode` is a directive *attribute*, not a standalone directive.** It goes on component instances in markup, not in `_Imports.razor`.

**`_Imports.razor`** — add the static using so you can write `InteractiveServer` instead of `RenderMode.InteractiveServer`:

```razor
@using static Microsoft.AspNetCore.Components.Web.RenderMode
```

**`App.razor`** — apply render mode to the top-level routable components:

```razor
<HeadOutlet @rendermode="InteractiveServer" />
...
<Routes @rendermode="InteractiveServer" />
```

This gives every page global server interactivity. Do **not** place `@rendermode InteractiveServer` as a line in `_Imports.razor` — it is not a valid Razor directive and will cause build errors (RZ10003, CS0103, RZ10024).

> **Reference:** [ASP.NET Core Blazor render modes](https://learn.microsoft.com/aspnet/core/blazor/components/render-modes)

### Page Base Class

`WebFormsPageBase` eliminates per-page boilerplate when migrating Web Forms code-behind. Instead of injecting `IPageService` into every page, a single `@inherits` directive in `_Imports.razor` gives all pages access to familiar Web Forms properties.

**One-time setup:**

1. **`_Imports.razor`** — add the base class directive:

```razor
@inherits BlazorWebFormsComponents.WebFormsPageBase
```

2. **Layout (`MainLayout.razor`)** — add the Page render component (renders `<PageTitle>` and `<meta>` tags):

```razor
<BlazorWebFormsComponents.Page />
```

**Properties available on every page:**

| Property | Behavior |
|---|---|
| `Title` | Delegates to `IPageService.Title` — `Page.Title = "X"` works unchanged |
| `MetaDescription` | Delegates to `IPageService.MetaDescription` |
| `MetaKeywords` | Delegates to `IPageService.MetaKeywords` |
| `IsPostBack` | Always returns `false` — `if (!IsPostBack)` always enters block |
| `Page` | Returns `this` — enables `Page.Title = "X"` dot syntax |

**What is NOT provided (forces proper Blazor migration):**

- `Page.Request` — use `IHttpContextAccessor` or `NavigationManager`
- `Page.Response` — use `NavigationManager` for redirects
- `Page.Session` — use scoped DI services

**When to still use `@inject IPageService`:** Non-page components (e.g., a shared header or sidebar) that need access to page metadata should inject `IPageService` directly. `WebFormsPageBase` only applies to routable pages.

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
- **AuthorizeView requirement:** If `LoginView` → `AuthorizeView` conversion is used, `Program.cs` MUST have `AddCascadingAuthenticationState()` and `AddAuthorization()` — even without full Identity. Without these, any page containing `<AuthorizeView>` crashes at runtime.

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

### Data Control Strategy — ALWAYS Use BWFC (Never Raw HTML)

> **This is the #1 failure mode in migration runs.** Layer 2 agents consistently replace BWFC data controls with `@foreach` loops and HTML tables. This is ALWAYS wrong.

| Web Forms Control | BWFC Component (✅ USE THIS) | Raw HTML (❌ NEVER USE) |
|---|---|---|
| `<asp:ListView>` | `<ListView Items="@data">` with `ItemTemplate` | `@foreach` + HTML table |
| `<asp:GridView>` | `<GridView Items="@data">` with columns | `@foreach` + `<table>` |
| `<asp:FormView>` | `<FormView DataItem="@item">` with `ItemTemplate` | Direct HTML rendering |
| `<asp:Repeater>` | `<Repeater Items="@data">` with `ItemTemplate` | `@foreach` loops |
| `<asp:DetailsView>` | `<DetailsView Items="@data">` with fields | Manual field rendering |
| `<asp:DataList>` | `<DataList Items="@data">` with `ItemTemplate` | `@foreach` + grid HTML |
| `<asp:DataGrid>` | `<DataGrid Items="@data">` with columns | `@foreach` + `<table>` |

**SelectMethod → Items:** Replace `SelectMethod="GetProducts"` with `Items="@_products"` where `_products` is populated in `OnInitializedAsync` via an injected service or DbContext.

### Session State → Scoped Services

- Replace `Session["key"]` with a scoped DI service
- Use `IHttpContextAccessor` for cookie-based persistence when needed
- Register in `Program.cs` with `builder.Services.AddScoped<TService>()`
- Example: `Session["CartId"]` → `CartStateService` with cookie-based cart ID

### Static Asset Relocation

- All static files → `wwwroot/` (preserving original directory structure)
- `UseStaticFiles()` MUST be in the middleware pipeline BEFORE `MapStaticAssets()` — without it, files in `wwwroot/` subdirectories (e.g., `wwwroot/Content/`, `wwwroot/Catalog/Images/`) may return 404
- CSS bundles (`BundleConfig.cs`) → explicit `<link>` tags in `App.razor`
- CSS/JS `<link>` and `<script>` tags from master pages (`Site.Master`) must be extracted into `App.razor` `<head>` — the migration script does this automatically via `New-AppRazorScaffold -SourceRoot`
- JS bundles → explicit `<script>` tags in `App.razor`
- Image paths update: `~/Images/` → `/Images/`
- Font paths: same pattern
- **Image path preservation rule:** Paths in templates (e.g., `<Image ImageUrl="...">`) must match WHERE files actually land in `wwwroot/`. Do not invent paths — check the source project's actual directory structure

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
- LoginView → AuthorizeView
- Master page → MainLayout.razor
- CSS/JS link extraction from master pages → `App.razor` (via `New-AppRazorScaffold -SourceRoot`)
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

### ❌ Using System.Web.UI.Page as Base Class

```csharp
// WRONG — Web Forms base class
public partial class ProductList : Page { }

// RIGHT — BWFC page base class (provides Page.Title, IsPostBack, etc.)
// Set via @inherits WebFormsPageBase in _Imports.razor
public partial class ProductList : WebFormsPageBase { }

// ALSO RIGHT — for non-page components
public partial class MyComponent : ComponentBase { }
```
