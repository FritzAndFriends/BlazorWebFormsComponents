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

## Patterns

### Target Architecture

| Setting | Standard |
|---|---|
| Framework | **.NET 10** (or latest LTS/.NET preview) |
| Project template | `dotnet new blazor --interactivity Server` |
| Render mode | Global Server Interactive |
| Base class | `ComponentBase` (not `System.Web.UI.Page`) |
| Layout | `MainLayout.razor` with `@inherits LayoutComponentBase` and `@Body` |

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

### ListView Template Gotchas — GroupTemplate & LayoutTemplate Require `@context`

In Web Forms, `GroupTemplate` and `LayoutTemplate` use placeholder IDs (`itemPlaceholder`, `groupPlaceholder`) that the runtime replaces with actual content. In BWFC, `GroupTemplate` and `LayoutTemplate` are `RenderFragment<RenderFragment>` — they receive inner content via the `@context` parameter.

**You MUST render `@context` where the placeholder element was.** Without it, items silently don't appear — no error, no empty template, just blank output.

```razor
@* WRONG — Web Forms placeholder pattern, content silently lost *@
<GroupTemplate>
    <tr id="itemPlaceholderContainer">
        <td id="itemPlaceholder"></td>
    </tr>
</GroupTemplate>
<LayoutTemplate>
    <table>
        <tr id="groupPlaceholder"></tr>
    </table>
</LayoutTemplate>

@* RIGHT — render @context where placeholders were *@
<GroupTemplate>
    <tr id="itemPlaceholderContainer">
        @context
    </tr>
</GroupTemplate>
<LayoutTemplate>
    <table>
        @context
    </table>
</LayoutTemplate>
```

**Migration rule:** When converting `GroupTemplate` or `LayoutTemplate`, delete any placeholder `<td>` / `<tr>` elements and insert `@context` in their place. Keep surrounding structural markup.

### Session State → Scoped Services

- Replace `Session["key"]` with a scoped DI service
- Use `IHttpContextAccessor` for cookie-based persistence when needed
- Register in `Program.cs` with `builder.Services.AddScoped<TService>()`
- Example: `Session["CartId"]` → `CartStateService` with cookie-based cart ID

### Service Registration — AddHttpContextAccessor Required

`BaseWebFormsComponent` has `[Inject] public IHttpContextAccessor HttpContextAccessor`. The BWFC library's `AddBlazorWebFormsComponents()` does **not** register this service (the library project lacks the ASP.NET Core shared framework reference). Consuming apps **must** call `builder.Services.AddHttpContextAccessor()` in `Program.cs` **before** `AddBlazorWebFormsComponents()`.

Without it, any BWFC component throws: `InvalidOperationException: Cannot provide a value for property 'HttpContextAccessor'`.

```csharp
// Program.cs — required registration order
builder.Services.AddHttpContextAccessor();       // ← REQUIRED for BWFC
builder.Services.AddBlazorWebFormsComponents();
```

**Migration rule:** Every `Program.cs` scaffold must include `AddHttpContextAccessor()` before `AddBlazorWebFormsComponents()`. This is a Layer 1 (script) requirement.

### Static Asset Relocation

- All static files → `wwwroot/`
- CSS bundles (`BundleConfig.cs`) → explicit `<link>` tags in `App.razor`
- JS bundles → explicit `<script>` tags in `App.razor`
- Image paths update: `~/Images/` → `/Images/`
- Font paths: same pattern

### Page Lifecycle Mapping

| Web Forms | Blazor | Notes |
|---|---|---|
| `Page_Load` | `OnInitializedAsync` | One-time init (does NOT re-run on param changes) |
| `Page_Load` (on postback) | `OnParametersSetAsync` | Re-runs when parameters change — use for `[SupplyParameterFromQuery]` |
| `Page_PreInit` | `OnInitializedAsync` (early) | Theme setup |
| `Page_PreRender` | `OnAfterRenderAsync` | Post-render logic |
| `IsPostBack` check | First render check via `firstRender` param | `if (!firstRender) return;` |
| `Page.Title` | `<PageTitle>` component | Built-in Blazor |
| `Response.Redirect` | `NavigationManager.NavigateTo()` | Inject `NavigationManager` |

⚠️ **`OnInitializedAsync` vs `OnParametersSetAsync`:** When a page uses `[SupplyParameterFromQuery]` and the user navigates to the same page with different query params (e.g., `/ProductList?id=1` → `/ProductList?id=3`), `OnInitializedAsync` does NOT re-run. Use `OnParametersSetAsync` so data reloads when query params change. `Page_Load` fires on every postback in Web Forms; `OnParametersSetAsync` is the closer equivalent when parameters change.

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

    // Use OnParametersSetAsync if query params drive the data load
    protected override async Task OnInitializedAsync()
    {
        _products = await Db.Products.ToListAsync();
    }
}

@* Program.cs must include: *@
@* builder.Services.AddHttpContextAccessor();       // REQUIRED for BWFC *@
@* builder.Services.AddBlazorWebFormsComponents(); *@
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

### ❌ Using Web Forms Placeholder IDs in ListView Templates

```razor
@* WRONG — placeholder elements are a Web Forms runtime concept; *@
@* content silently disappears with no error *@
<GroupTemplate>
    <tr><td id="itemPlaceholder"></td></tr>
</GroupTemplate>
<LayoutTemplate>
    <table><tr id="groupPlaceholder"></tr></table>
</LayoutTemplate>

@* RIGHT — render @context where placeholders were *@
<GroupTemplate>
    <tr>@context</tr>
</GroupTemplate>
<LayoutTemplate>
    <table>@context</table>
</LayoutTemplate>
```

### ❌ Using OnInitializedAsync for Query-Parameter-Driven Data

```csharp
// WRONG — data loads once, navigation with different query params does nothing
[SupplyParameterFromQuery(Name = "id")]
private int? CategoryId { get; set; }

protected override async Task OnInitializedAsync()
{
    _products = await LoadProducts(CategoryId);
}

// RIGHT — data reloads when query param changes
protected override async Task OnParametersSetAsync()
{
    _products = await LoadProducts(CategoryId);
}
```

### ❌ Missing AddHttpContextAccessor Registration

```csharp
// WRONG — throws InvalidOperationException at runtime
// "Cannot provide a value for property 'HttpContextAccessor'"
builder.Services.AddBlazorWebFormsComponents();

// RIGHT — register IHttpContextAccessor BEFORE BWFC
builder.Services.AddHttpContextAccessor();       // ← REQUIRED
builder.Services.AddBlazorWebFormsComponents();
```
