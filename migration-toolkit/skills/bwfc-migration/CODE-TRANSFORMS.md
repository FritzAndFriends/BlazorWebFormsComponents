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
| `if (!IsPostBack)` | Works unchanged via `WebFormsPageBase` | Always enters the block — correct for first-render code |
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

// Blazor (with WebFormsPageBase — IsPostBack compiles unchanged)
protected override async Task OnInitializedAsync()
{
    if (!IsPostBack)  // ✅ compiles — always true, block always executes
    {
        products = await ProductService.GetProductsAsync();
    }
}

// Or, simplified (since IsPostBack is always false):
protected override async Task OnInitializedAsync()
{
    products = await ProductService.GetProductsAsync();
}
```

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
| `Response.Redirect("~/path")` | `NavigationManager.NavigateTo("/path")` |
| `Response.RedirectToRoute(...)` | `NavigationManager.NavigateTo($"/path/{param}")` |
| `Server.Transfer("~/page.aspx")` | `NavigationManager.NavigateTo("/page")` |

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
- `<form runat="server">` → removed
- `<asp:ContentPlaceHolder ID="MainContent">` → `@Body`
- `<asp:ScriptManager>` → `<ScriptManager />` (renders nothing)
- CSS `<link>` elements from master page `<head>` → `App.razor` `<head>` section
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
