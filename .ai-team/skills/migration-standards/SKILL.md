---
name: "migration-standards"
description: "Canonical standards for migrating ASP.NET Web Forms applications to Blazor using BWFC"
domain: "migration"
confidence: "high"
source: "earned"
---

## Context

Updated 2026-03-06 with Run 7 learnings: WebFormsPageBase as canonical base class, LoginView native component, cookie auth pattern, form submission gotchas.

When migrating an ASP.NET Web Forms application to Blazor using BlazorWebFormsComponents, these standards define the canonical target architecture, tooling choices, and migration patterns. Established through seven WingtipToys migration benchmark runs and codified as a directive by Jeffrey T. Fritz.

Apply these standards to:
- Migration script (`bwfc-migrate.ps1`) enhancements
- Copilot-assisted Layer 2 work
- Migration documentation and checklists
- Any new migration test runs

---

## ⚠️ BWFC Control Preservation — MANDATORY

> **Jeff's directive:** "We need to ALWAYS preserve the default asp: controls by using the BWFC components."

**The entire purpose of BlazorWebFormsComponents is that these components exist — USE THEM.**

Every `asp:` control in the Web Forms source MUST become a BWFC component in the migration output. The migration strips the `asp:` prefix (`<asp:GridView>` → `<GridView>`) and converts Web Forms attributes to BWFC parameters. The resulting markup should look nearly identical to the original, minus the `asp:` prefix and `runat="server"`.

### Rules

1. **ALL asp: controls MUST be preserved as BWFC components.** The script strips the prefix; the component name stays.
2. **NEVER flatten data controls to raw HTML.** This means: GridView, ListView, Repeater, DataList, DataGrid, DetailsView, FormView. These MUST remain as their BWFC component equivalents with proper column/template definitions.
3. **NEVER flatten editor controls to raw HTML elements.** This means: TextBox → `<input>`, CheckBox → `<input type="checkbox">`, Button → `<button>`, Label → `<span>`, etc. are all WRONG. Keep them as `<TextBox>`, `<CheckBox>`, `<Button>`, `<Label>`.
4. **NEVER flatten navigation/structural controls.** HyperLink, ImageButton, LinkButton, Panel, PlaceHolder, etc. must all remain as BWFC components.
5. **The migration script `Test-BwfcControlPreservation` verification function runs post-transform** to catch any control loss. Warnings indicate a human or AI flattened a control during Layer 2 work.

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
| Render mode | Global Server Interactive |
| Base class | `WebFormsPageBase` (from the BWFC library) — extends `ComponentBase` with `Page.Title`, `Page.MetaDescription`, `IsPostBack` (always false), and `Page.GetRouteUrl()` compatibility. Layout files still use `LayoutComponentBase`. |
| Layout | `MainLayout.razor` with `@inherits LayoutComponentBase` and `@Body` |

> **`_Imports.razor` MUST include `@inherits WebFormsPageBase`** so all migrated pages automatically get `Page.Title`, `IsPostBack`, and `GetRouteUrl()` compatibility. This is a Layer 1 (script) scaffold requirement.

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

> **DIRECTIVE:** CSS `<link>` elements from the master page **MUST** be migrated to `App.razor`, not the layout file. This includes Bootstrap, site CSS, and any other stylesheets. The layout file handles structural markup; `App.razor` handles document-level `<head>` elements.
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
| `Page.Title` | `WebFormsPageBase.Title` (via `Page => this` property) | Migrated pages inheriting from `WebFormsPageBase` get `Page.Title` for free — no `<PageTitle>` wrapping needed. `Page.Title = "..."` in code-behinds compiles unchanged. |
| `Response.Redirect` | `NavigationManager.NavigateTo()` | Inject `NavigationManager` |

⚠️ **`OnInitializedAsync` vs `OnParametersSetAsync`:** When a page uses `[SupplyParameterFromQuery]` and the user navigates to the same page with different query params (e.g., `/ProductList?id=1` → `/ProductList?id=3`), `OnInitializedAsync` does NOT re-run. Use `OnParametersSetAsync` so data reloads when query params change. `Page_Load` fires on every postback in Web Forms; `OnParametersSetAsync` is the closer equivalent when parameters change.

> **DEFAULT RULE:** Always map Web Forms `Page_Load` to Blazor `OnInitializedAsync`. This is the canonical mapping for first-load data initialization. Only use `OnParametersSetAsync` when the page uses `[SupplyParameterFromQuery]` and needs to reload on param changes.

### Layer 1 (Script) vs Layer 2 (Manual) Boundary

**Script handles (Layer 1):**
- `asp:` prefix stripping (preserves BWFC tags)
- Data-binding expression conversion (5 variants)
- LoginView `asp:` prefix stripping (keep as `<LoginView>` — BWFC native component)
- Master page → MainLayout.razor
- Scaffold generation (csproj, Program.cs, etc.)
- SelectMethod/GetRouteUrl flagging
- Register directive cleanup

> **LoginView — BWFC native component:** The migration script should strip the `asp:` prefix from `<asp:LoginView>` and keep it as `<LoginView>`. The BWFC library now has a native LoginView component (`src/BlazorWebFormsComponents/LoginControls/LoginView.razor`) that wraps `<AuthorizeView>` internally. The script should **NOT** convert LoginView to AuthorizeView.

> **MasterPage migration:** The BWFC library has a MasterPage migration strategy. The script should preserve `<MasterPage>` semantics and NOT rewrite the layout approach.

**Always manual (Layer 2):**
- EF6 → EF Core (models, DbContext, seed)
- Identity/Auth subsystem
- Session → scoped services
- Business logic (checkout, payment, admin CRUD)
- Complex data-binding with arithmetic/method chains

---

## Runtime Gotchas (Discovered in Benchmarks)

| Issue | Impact | Resolution |
|-------|--------|------------|
| Blazor strips `onclick` from buttons | Form submissions silently fail | Use `<a role="button">` or minimal API POST endpoints |
| Singleton auth state providers | Auth state bleeds across sessions | Register as scoped + use cookie auth (see `blazor-auth-migration` skill) |
| Missing Bootstrap CSS in App.razor | Navbar renders unstyled, locators fail | Add explicit `<link>` tags to `App.razor` `<head>` |
| `_Imports.razor` missing `@inherits WebFormsPageBase` | Page.Title, IsPostBack, GetRouteUrl all fail | Add `@inherits WebFormsPageBase` to scaffolded _Imports.razor |

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

// RIGHT — WebFormsPageBase provides Page.Title, IsPostBack, GetRouteUrl compatibility
public partial class ProductList : WebFormsPageBase { }
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
