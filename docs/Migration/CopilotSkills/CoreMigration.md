# Core Migration Skill

The **Core Migration Skill** handles Web Forms  Blazor markup and code-behind transformations. This is the primary skill for **Layer 2** migration work  the structural transforms that require pattern recognition and semantic understanding.

---

## When to Use This Skill

Use the Core Migration skill when:

- Converting `.aspx` files to `.razor` files (after Layer 1 automated transforms)
- Updating code-behind lifecycle methods (`Page_Load`  `OnInitializedAsync`)
- Converting data binding patterns (`SelectMethod`, `ItemType`, template contexts)
- Transforming event handler signatures
- Converting Master Pages to Blazor Layouts
- Understanding which Web Forms patterns work via BWFC shims

---

## What This Skill Covers

### 1. Shim Infrastructure Understanding

The skill provides deep knowledge of the **BWFC shim infrastructure** that preserves Web Forms API patterns in Blazor:

| Web Forms API | Shim | Status |
|---------------|------|--------|
| `Response.Redirect("~/path")` | ResponseShim |  Works AS-IS |
| `Request.QueryString["key"]` | RequestShim |  Works AS-IS |
| `Request.Form["key"]` | FormShim |  Works with `<WebFormsForm>` wrapper |
| `Session["key"]` | SessionShim |  Works AS-IS (in-memory per circuit) |
| `Cache["key"]` | CacheShim |  Works AS-IS (IMemoryCache) |
| `Server.MapPath("~/path")` | ServerShim |  Works AS-IS |
| `Page.Title`, `Page.IsPostBack` | WebFormsPageBase |  Works AS-IS |
| `ClientScript.RegisterStartupScript()` | ClientScriptShim |  Works AS-IS |
| `ViewState["key"]` | ViewStateDictionary |  Works AS-IS (in-memory only) |

**Key principle:** If the original Web Forms code uses these patterns, **keep them unchanged**. The shims make them work correctly in Blazor.

### 2. Control Tag Transformations

The skill knows all BWFC component mappings and transformations:

```html
<!-- Web Forms -->
<asp:Button ID="btnSubmit" Text="Submit" OnClick="Submit_Click" runat="server" />

<!-- Blazor (after Layer 1 + Layer 2) -->
<Button Text="Submit" OnClick="Submit_Click" />
```

**Layer 1 (automated):**
- Removes `asp:` prefix
- Removes `ID` attribute
- Removes `runat="server"`

**Layer 2 (this skill handles):**
- Understands that `OnClick` event handler signature changes from `void Submit_Click(object sender, EventArgs e)` to `void Submit_Click()`
- Knows when to add `@bind-` directives for two-way binding

### 3. Data Binding Patterns

The skill guides conversion of Web Forms data binding to Blazor patterns:

**SelectMethod  Items/SelectMethod delegate:**

```html
<!-- Web Forms -->
<asp:GridView ItemType="Product" SelectMethod="GetProducts" runat="server">
    <Columns>
        <asp:BoundField DataField="Name" HeaderText="Product" />
    </Columns>
</asp:GridView>
```

```razor
<!-- Blazor  preserve SelectMethod as delegate (preferred) -->
<GridView TItem="Product" SelectMethod="@productService.GetProducts">
    <Columns>
        <BoundField DataField="Name" HeaderText="Product" />
    </Columns>
</GridView>
```

**Template Context Variables:**

```html
<!-- Web Forms -->
<asp:Repeater ItemType="Product" runat="server">
    <ItemTemplate>
        <div><%#: Item.Name %></div>
    </ItemTemplate>
</asp:Repeater>
```

```razor
<!-- Blazor -->
<Repeater TItem="Product" SelectMethod="@GetProducts">
    <ItemTemplate Context="Item">
        <div>@Item.Name</div>
    </ItemTemplate>
</Repeater>
```

### 4. Lifecycle Method Conversions

**Page_Load  OnInitializedAsync:**

```csharp
// Web Forms
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        LoadProducts();
    }
}
```

```csharp
// Blazor  signature changes, but IsPostBack works via shim
protected override async Task OnInitializedAsync()
{
    if (!IsPostBack)
    {
        await LoadProductsAsync();
    }
}
```

**Event Handler Signatures:**

```csharp
// Web Forms
protected void AddToCart_Click(object sender, EventArgs e)
{
    Session["CartId"] = Guid.NewGuid().ToString();
}
```

```csharp
// Blazor  remove sender/e parameters, Session still works
protected void AddToCart_Click()
{
    Session["CartId"] = Guid.NewGuid().ToString();  // Works via SessionShim
}
```

### 5. Master Page  Layout Conversion

**Web Forms Master Page:**

```html
<%@ Master Language="C#" %>
<html>
<head runat="server">
    <title><asp:ContentPlaceHolder ID="TitleContent" runat="server" /></title>
</head>
<body>
    <asp:ContentPlaceHolder ID="MainContent" runat="server" />
</body>
</html>
```

**Blazor Layout:**

```razor
@inherits LayoutComponentBase

<html>
<head>
    <PageTitle>@Title</PageTitle>
</head>
<body>
    @Body
</body>
</html>
```

**Content Page Conversion:**

```html
<!-- Web Forms -->
<%@ Page MasterPageFile="~/Site.Master" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h1>Welcome</h1>
</asp:Content>
```

```razor
<!-- Blazor -->
@page "/Welcome"
@layout MainLayout

<h1>Welcome</h1>
```

---

## Anti-Patterns This Skill Detects

###  Don't Inject Services the Shims Already Provide

```csharp
// WRONG  NavigationManager for redirects
@inject NavigationManager Nav
Nav.NavigateTo("/Products");

// CORRECT  Use ResponseShim
Response.Redirect("~/Products");
```

###  Don't Use IHttpContextAccessor for Request Data

```csharp
// WRONG  Manual HttpContext access
@inject IHttpContextAccessor HttpContext
var id = HttpContext.HttpContext.Request.Query["id"];

// CORRECT  Use RequestShim
var id = Request.QueryString["id"];
```

###  Don't Create Minimal APIs for Page Actions

```csharp
// WRONG  Fighting Blazor with HTTP endpoints
app.MapPost("/api/AddToCart", async (int productId) => { ... });

// CORRECT  Keep as Blazor component methods
private async Task AddToCart_Click() { ... }
```

###  Don't Treat Shims as Temporary Scaffolding

**Wrong mindset:** "I'll use shims to get it compiling, then replace them with 'real' Blazor code."

**Correct mindset:** "Shims ARE the migration strategy. They work correctly. Replacing them with native Blazor patterns is an optional Layer 3 optimization."

---

## How to Use This Skill

### Option 1: Reference in Copilot Chat

If you've copied the skills to your project:

```
@workspace Use the migration patterns in .github/skills/bwfc-migration/SKILL.md 
to complete the migration of ProductList.razor. Focus on data binding and event handlers.
```

### Option 2: File-Specific Guidance

When working on a specific file:

```
Apply the BWFC migration skill to convert this Master Page to a Blazor Layout. 
Preserve the content placeholders as @Body and handle the navigation menu.
```

### Option 3: Pattern-Specific Questions

For specific patterns:

```
Using BWFC migration patterns, how should I convert this SelectMethod binding?
The original uses SelectMethod="GetProducts" with ItemType="Product".
```

---

## Migration Decision Tree

The skill provides this decision framework:

```
Original code uses Response.Redirect()?
  → Keep Response.Redirect()  ResponseShim handles it 

Original code uses Session["key"]?
   Keep Session["key"]  SessionShim handles it 
  
Original code uses Request.QueryString["key"]?
   Keep Request.QueryString["key"]  RequestShim handles it 

Original code uses ViewState["key"]?
   Keep ViewState["key"]  WebFormsPageBase provides it 
   (Consider refactoring to component fields for clarity)

Original code uses SelectMethod="MethodName"?
   Convert to SelectMethod="@ServiceMethod" (delegate binding)
   OR convert to Items="@data" + load in OnInitializedAsync

Original code has Page_Load(sender, e)?
   Change signature to OnInitializedAsync()
   IsPostBack checks inside still work via shim 

Original code has event handlers with (sender, e)?
   Remove sender and e parameters
   Rest of the code works unchanged
```

---

## Common Migration Scenarios

### Scenario 1: Simple Display Page

**Before (Web Forms):**
```html
<%@ Page Title="Products" MasterPageFile="~/Site.Master" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h1>Products</h1>
    <asp:GridView ItemType="Product" SelectMethod="GetProducts" runat="server">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Name" />
            <asp:BoundField DataField="Price" HeaderText="Price" />
        </Columns>
    </asp:GridView>
</asp:Content>
```

**After (Blazor with BWFC):**
```razor
@page "/Products"
@layout MainLayout
@inject IProductService ProductService

<h1>Products</h1>
<GridView TItem="Product" SelectMethod="@ProductService.GetProducts">
    <Columns>
        <BoundField DataField="Name" HeaderText="Name" />
        <BoundField DataField="Price" HeaderText="Price" />
    </Columns>
</GridView>
```

### Scenario 2: Interactive Form

**Before (Web Forms):**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        LoadCategories();
    }
}

protected void AddProduct_Click(object sender, EventArgs e)
{
    var product = new Product
    {
        Name = txtName.Text,
        CategoryId = int.Parse(ddlCategory.SelectedValue)
    };
    _db.Products.Add(product);
    _db.SaveChanges();
    Response.Redirect("~/Products.aspx");
}
```

**After (Blazor with BWFC):**
```csharp
protected override async Task OnInitializedAsync()
{
    if (!IsPostBack)  // Works via WebFormsPageBase shim
    {
        await LoadCategoriesAsync();
    }
}

protected async Task AddProduct_Click()  // Signature changed, no sender/e
{
    var product = new Product
    {
        Name = txtName.Text,
        CategoryId = int.Parse(ddlCategory.SelectedValue)
    };
    _db.Products.Add(product);
    await _db.SaveChangesAsync();
    Response.Redirect("~/Products");  // Works via ResponseShim, auto-strips .aspx
}
```

---

## Related Skills

- **[Identity & Authentication Migration](IdentityMigration.md)**  For login pages and auth
- **[Data & Architecture Migration](DataMigration.md)**  For EF6EF Core, DataSources, architecture

---

## Skill File Location

The full skill file is located at:
```
migration-toolkit/skills/bwfc-migration/SKILL.md
```

For the complete technical specification and all transformation rules, see the skill file in the BWFC repository.

---

## Related Documentation

- [Quick Start Guide](../QuickStart.md)  Step-by-step migration walkthrough
- [Three-Layer Methodology](../Methodology.md)  Understanding Layer 1, 2, and 3
- [Control Coverage](../ControlCoverage.md)  Which Web Forms controls are supported
- [Migration Checklist](../ChecklistTemplate.md)  Per-page progress tracking
