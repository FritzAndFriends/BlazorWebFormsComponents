# Code Transformation Rules

Patterns for migrating Web Forms code-behind, data binding, and master pages to Blazor equivalents.

**Parent skill:** `SKILL.md` (bwfc-migration)

---

## Code-Behind Migration

### Lifecycle Methods

| Web Forms | Blazor | Notes |
|-----------|--------|-------|
| `Page_Load(object sender, EventArgs e)` | `protected override async Task OnInitializedAsync()` | First load |
| `Page_PreRender(...)` | `protected override async Task OnParametersSetAsync()` | Before each render |
| `Page_Init(...)` | `protected override void OnInitialized()` | Sync initialization |
| `if (!IsPostBack)` | **L1 CLI auto-unwraps** simple guards; works unchanged via `WebFormsPageBase` if left in | Always enters the block — correct for first-render code |
| `if (IsPostBack)` (without `!`) | **Dead code — flag for manual review** | Never enters the block in Blazor; move logic to event handlers |

```csharp
// Web Forms
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        products = GetProducts();
        GridView1.DataBind();
    }
}

// After L1 (simple guard auto-unwrapped):
protected void Page_Load(object sender, EventArgs e)
{
    // BWFC: IsPostBack guard unwrapped — Blazor re-renders on every state change
    products = GetProducts();
    GridView1.DataBind();
}

// After L2 (Blazor lifecycle):
protected override async Task OnInitializedAsync()
{
    products = await ProductService.GetProductsAsync();
}
```

#### IsPostBack Guard Handling (L1 Automated)

The L1 CLI tool (`webforms-to-blazor`) applies `IsPostBackTransform` to every code-behind file:

**Simple guards** (no `else` clause): The `if (!IsPostBack)` / `if (!Page.IsPostBack)` / `if (!this.IsPostBack)` / `if (IsPostBack == false)` wrapper is removed and the body is extracted and dedented. A comment replaces the guard:

```csharp
// BWFC: IsPostBack guard unwrapped — Blazor re-renders on every state change
LoadCategories();
BindGrid();
```

**Complex guards** (with `else` clause): A TODO comment is inserted above the guard for manual review:

```csharp
// TODO: BWFC — IsPostBack guard with else clause. In Blazor, OnInitializedAsync runs once (no postback).
// Review: move 'if' body to OnInitializedAsync and 'else' body to an event handler or remove.
if (!IsPostBack)
{
    LoadInitialData();
}
else
{
    ProcessPostBackData();
}
```

**Single-statement guards** (no braces): Flagged with a TODO comment — the tool does not attempt to parse braceless `if` statements.

**Layer 2 action:** After L1, convert `Page_Load` → `OnInitializedAsync` and move the unwrapped body into the async method. For complex guards, move the `if` body into `OnInitializedAsync` and the `else` body into event handlers or remove it.

### Event Handlers

```csharp
// Web Forms
protected void SubmitBtn_Click(object sender, EventArgs e)
{
    Response.Redirect("~/Confirmation");
}

// Blazor — no sender/EventArgs parameters
private void SubmitBtn_Click()
{
    NavigationManager.NavigateTo("/Confirmation");
}
```

### Navigation

| Web Forms | Blazor |
|-----------|--------|
| `Response.Redirect("~/path")` | Works AS-IS via `ResponseShim` (auto-strips `~/` and `.aspx`); or use `NavigationManager.NavigateTo("/path")` |
| `Response.RedirectToRoute(...)` | `NavigationManager.NavigateTo($"/path/{param}")` |
| `Server.Transfer("~/page.aspx")` | `NavigationManager.NavigateTo("/page")` |

### `.aspx` URL Cleanup (L1 Automated)

The L1 CLI tool (`webforms-to-blazor`) automatically rewrites `.aspx` URL string literals in code-behind files. This runs after `Response.Redirect` → `NavigationManager.NavigateTo` conversion.

| Pattern | Before | After |
|---------|--------|-------|
| Tilde + query string | `"~/Products.aspx?id=5"` | `"/Products?id=5"` |
| Tilde, no query | `"~/Products.aspx"` | `"/Products"` |
| Relative in NavigateTo + query | `NavigationManager.NavigateTo("Products.aspx?q=x")` | `NavigationManager.NavigateTo("/Products?q=x")` |
| Relative in NavigateTo | `NavigationManager.NavigateTo("Products.aspx")` | `NavigationManager.NavigateTo("/Products")` |

> **Note:** `.aspx` references in markup (href attributes) are preserved — `AspxRewriteMiddleware` can handle those at runtime. The L1 URL cleanup targets **code-behind string literals** only.

### Query String / Route Parameters

```csharp
// Web Forms (Model Binding)
public IQueryable<Product> GetProducts([QueryString] int? categoryId) { ... }

// Blazor
[SupplyParameterFromQuery] public int? CategoryId { get; set; }
```

```csharp
// Web Forms (RouteData)
public void GetProduct([RouteData] int productId) { ... }

// Blazor
@page "/Products/{ProductId:int}"
[Parameter] public int ProductId { get; set; }
```

---

## Data Binding Migration

### Collection-Bound Controls

For GridView, ListView, Repeater, DataList, DataGrid:

| Web Forms Pattern | BWFC Pattern |
|-------------------|-------------|
| `SelectMethod="GetProducts"` | `SelectMethod="@productService.GetProducts"` (convert string to `SelectHandler<ItemType>` delegate — BWFC auto-populates `Items`) |
| `ItemType="Namespace.Product"` | `ItemType="Product"` (strip namespace only) |
| `DataSource=<%# GetItems() %>` + `DataBind()` | `Items="items"` |
| `DataKeyNames="ProductID"` | `DataKeyNames="ProductID"` (preserved) |

> **How SelectMethod works in BWFC:** `DataBoundComponent<ItemType>` has a `SelectMethod` parameter of type `SelectHandler<ItemType>` — a delegate with signature `(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount) → IQueryable<ItemType>`. When set, `OnAfterRenderAsync` automatically calls the delegate to populate `Items`. This mirrors how Web Forms `SelectMethod` worked.

### Single-Record Controls

For FormView, DetailsView:

| Web Forms Pattern | BWFC Pattern |
|-------------------|-------------|
| `SelectMethod="GetProduct"` | `SelectMethod="@productService.GetProduct"` (convert string to delegate) or `DataItem="product"` (load in `OnInitializedAsync`) |
| `ItemType="Namespace.Product"` | `ItemType="Product"` (strip namespace only) |

### Template Binding

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<%# Item.Name %>` | `@Item.Name` | Add `Context="Item"` to template element |
| `<%# Eval("Name") %>` | `@Item.Name` | Direct property access replaces reflection |
| `<%# Bind("Name") %>` | `@bind-Value="Item.Name"` | Two-way in edit templates |

---

## Master Page → Layout Migration

### Web Forms Master Page

```html
<%@ Master Language="C#" CodeBehind="Site.master.cs" Inherits="MyApp.SiteMaster" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title><%: Page.Title %></title>
    <asp:ContentPlaceHolder ID="HeadContent" runat="server" />
</head>
<body>
    <form runat="server">
        <asp:ScriptManager runat="server" />
        <header>
            <nav><asp:Menu ID="MainMenu" runat="server" ... /></nav>
        </header>
        <main>
            <asp:ContentPlaceHolder ID="MainContent" runat="server" />
        </main>
        <footer>© <%: DateTime.Now.Year %></footer>
    </form>
</body>
</html>
```

### Blazor Layout Equivalent

```razor
@inherits LayoutComponentBase

<header>
    <nav><Menu ... /></nav>
</header>
<main>
    @Body
</main>
<footer>© @DateTime.Now.Year</footer>
```

**Key changes:**
- `<%@ Master %>` → `@inherits LayoutComponentBase`
- `<form runat="server">` → replaced with `<div>` (preserves `id` attribute and CSS block formatting context)
- `<asp:ContentPlaceHolder ID="MainContent">` → `@Body`
- `<asp:ScriptManager>` → `<ScriptManager />` (renders nothing)
- CSS `<link>` elements from master page `<head>` → `App.razor` `<head>` section (relative `href` paths must be rewritten to absolute, e.g., `CSS/style.css` → `/CSS/style.css`, because `<HeadContent>` resolves from the page URL)
- `<head runat="server">` content → `<HeadContent>` in layout or `App.razor`

> **Alternative:** For a more gradual migration, BWFC provides `<MasterPage>`, `<Content>`, and `<ContentPlaceHolder>` components that preserve Web Forms-style markup. Use these as a stepping stone, then refactor to native Blazor layouts when ready.

> **Tip:** Use `<WebFormsPage>@Body</WebFormsPage>` as the layout wrapper instead of plain `@Body` to get NamingContainer (ID scoping), theming, and head rendering in one component.

### Nested Master Pages → Nested Layouts

```razor
@inherits LayoutComponentBase
@layout MainLayout

<div class="child-wrapper">
    @Body
</div>
```

---

## Phase 2 Transforms

Phase 2 (Layer 2, Copilot-assisted) converts compile-compatible code into functionally correct Blazor code.

### Page Lifecycle Method Transforms

Convert Web Forms lifecycle methods to Blazor equivalents. Apply **after** Phase 1 has unwrapped IsPostBack guards.

**Page_Load → OnInitializedAsync:**

```csharp
// Before (Phase 1 output):
protected void Page_Load(object sender, EventArgs e)
{
    categories = GetCategories();
    BindGrid();
}

// After (Phase 2):
protected override async Task OnInitializedAsync()
{
    categories = await GetCategoriesAsync();
    // DataBound controls auto-bind via SelectMethod — remove explicit BindGrid()
}
```

**Page_Init → OnInitialized:**

```csharp
// Before:
protected void Page_Init(object sender, EventArgs e)
{
    theme = "Default";
    ViewState["Initialized"] = true;
}

// After:
protected override void OnInitialized()
{
    theme = "Default";
    // ViewState removed — use component fields instead
}
```

**Page_PreRender → OnAfterRenderAsync:**

```csharp
// Before:
protected void Page_PreRender(object sender, EventArgs e)
{
    lblTotal.Text = cart.GetTotal().ToString("C");
}

// After:
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        lblTotal.Text = cart.GetTotal().ToString("C");
        StateHasChanged();
    }
}
```

> **Key differences:** `OnAfterRenderAsync` runs *after* render (not before), so UI updates require `StateHasChanged()`. Guard with `if (firstRender)` to avoid infinite render loops.

### Event Handler Signature Transforms

Strip **standard** `EventArgs` (carries no data). Preserve **specialized** EventArgs (carries command data, selection data, etc.).

**Standard EventArgs — strip both parameters:**

```csharp
// Before:
protected void SaveBtn_Click(object sender, EventArgs e)
{
    SaveRecord();
}

// After:
private void SaveBtn_Click()
{
    SaveRecord();
}
```

**Specialized EventArgs — keep the BWFC equivalent:**

```csharp
// Before:
protected void ProductList_ItemCommand(object sender, ListViewCommandEventArgs e)
{
    if (e.CommandName == "AddToCart")
    {
        var productId = e.CommandArgument.ToString();
        AddToCart(productId);
    }
}

// After:
private void ProductList_ItemCommand(CommandEventArgs e)
{
    if (e.CommandName == "AddToCart")
    {
        var productId = e.CommandArgument.ToString();
        AddToCart(productId);
    }
}
```

**Decision table:**

| Original Parameter Type | Action | BWFC Equivalent |
|---|---|---|
| `EventArgs` | **Strip** both params | No parameter |
| `CommandEventArgs` / `GridViewCommandEventArgs` / `ListViewCommandEventArgs` | **Keep** — map to BWFC type | `CommandEventArgs` |
| `GridViewEditEventArgs` / `GridViewDeleteEventArgs` | **Keep** — map to BWFC type | Check BWFC component API |
| `RepeaterCommandEventArgs` | **Keep** — map to BWFC type | `CommandEventArgs` |
