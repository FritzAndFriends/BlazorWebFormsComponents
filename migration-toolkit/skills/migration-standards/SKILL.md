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
| Render mode | Global Server Interactive (see [Render Mode Placement](#render-mode-placement) below) |
| Base class | `ComponentBase` (not `System.Web.UI.Page`) |
| Layout | `MainLayout.razor` with `@inherits LayoutComponentBase` and `@Body` |

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

### Database Migration

- **Always** migrate EF6 → EF Core
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
- Image paths update: `~/Images/` → `/Images/`
- Font paths: same pattern

### Page Lifecycle Mapping

| Web Forms | Blazor | Notes |
|---|---|---|
| `Page_Load` | `OnInitializedAsync` | One-time init |
| `Page_PreInit` | `OnInitializedAsync` (early) | Theme setup |
| `Page_PreRender` | `OnAfterRenderAsync` | Post-render logic |
| `IsPostBack` check | First render check via `firstRender` param | `if (!firstRender) return;` |
| `Page.Title` | `PageService.Title` / `<Page>` component | BWFC provides `PageService` and a `Page` component that renders `<PageTitle>`. Use `PageService.Title` in code-behind as the equivalent of `Page.Title`. |
| `Response.Redirect` | `NavigationManager.NavigateTo()` | Inject `NavigationManager` |

### Layer 1 (Script) vs Layer 2 (Manual) Boundary

**Script handles (Layer 1):**
- `asp:` prefix stripping (preserves BWFC tags)
- Data-binding expression conversion (5 variants)
- LoginView → AuthorizeView
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
// WRONG
public partial class ProductList : Page { }

// RIGHT
public partial class ProductList : ComponentBase { }
```
