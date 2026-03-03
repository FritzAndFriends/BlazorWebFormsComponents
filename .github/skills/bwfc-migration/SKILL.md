---
name: bwfc-migration
description: "Migrate ASP.NET Web Forms applications to Blazor Server using BlazorWebFormsComponents (BWFC). Covers the full migration lifecycle: project setup, NuGet package installation, mechanical markup transforms, data binding conversion, code-behind migration, and architecture decisions. Use when converting .aspx/.ascx/.master files to Blazor."
---

# Web Forms → Blazor Migration with BlazorWebFormsComponents

This skill provides complete transformation rules for migrating ASP.NET Web Forms applications to Blazor Server using the **BlazorWebFormsComponents** (BWFC) NuGet package.

## What Is BWFC?

BlazorWebFormsComponents is an open-source library that provides **drop-in Blazor replacements** for ASP.NET Web Forms server controls. It enables migration from Web Forms to Blazor with minimal markup changes by preserving the same component names, attribute names, and rendered HTML output.

- **NuGet Package:** <https://www.nuget.org/packages/Fritz.BlazorWebFormsComponents>
- **GitHub Repository:** <https://github.com/FritzAndFriends/BlazorWebFormsComponents>
- **52 production-ready components** across 7 categories
- **Same HTML output** — existing CSS stylesheets and JavaScript continue to work

### Core Principle

> **Strip `asp:` and `runat="server"`, keep everything else, and it just works.**

BWFC components match Web Forms control names, property names, and rendered HTML. A well-structured Web Forms page can often be migrated by removing the `asp:` prefix, removing `runat="server"`, and making a small set of structural adjustments.

---

## Installation

### Step 1: Create Blazor Server Project

```bash
dotnet new blazor -n MyBlazorApp --interactivity Server
cd MyBlazorApp
dotnet add package Fritz.BlazorWebFormsComponents
```

### Step 2: Configure `_Imports.razor`

Add these to the project-level `_Imports.razor`:

```razor
@using BlazorWebFormsComponents
@using BlazorWebFormsComponents.Enums
```

### Step 3: Register BWFC Services

In `Program.cs`:

```csharp
builder.Services.AddBlazorWebFormsComponents();
```

### Step 4: Add BWFC JavaScript

In `App.razor` or the host page `<head>`:

```html
<script src="_content/Fritz.BlazorWebFormsComponents/js/Basepage.js"></script>
```

---

## Three-Layer Migration Methodology

The BWFC migration pipeline divides work into three layers based on how much human judgment is required. This structure ensures mechanical work is automated, structural transforms are Copilot-assisted, and only genuine architecture decisions require human thought.

| Layer | What It Handles | % of Work | Who Does It |
|-------|----------------|-----------|-------------|
| **Layer 1: Mechanical Transforms** | Tag prefix removal, `runat` removal, expression conversion, URL conversion, file renaming | ~40% | Automatable (find-and-replace, scripts, or Copilot) |
| **Layer 2: Structural Transforms** | Data binding conversion (`SelectMethod` → `Items`), code-behind lifecycle migration, template context variables, layout conversion | ~45% | Copilot-assisted (use the rules in this skill) |
| **Layer 3: Architecture Decisions** | State management, data access layer, authentication, third-party integrations, middleware | ~15% | Human + Copilot (requires project-specific judgment) |

### Layer 1 — Mechanical Transforms (Automatable)

These transforms are 100% mechanical and can be applied with find-and-replace or automated tooling:

- Remove all `asp:` tag prefixes
- Remove all `runat="server"` attributes
- Convert expressions: `<%: expr %>` → `@(expr)`, `<%# Item.X %>` → `@context.X`
- Convert URLs: `~/path` → `/path`
- Rename files: `.aspx` → `.razor`, `.ascx` → `.razor`, `.master` → `.razor`
- Remove `<asp:Content>` wrappers (page body IS the content in Blazor)
- Convert `<%@ Page %>` directives to `@page "/route"`
- Remove `<form runat="server">` wrapper

### Layer 2 — Structural Transforms (Copilot-Assisted)

Apply these transforms using the detailed rules in this skill:

- Convert `SelectMethod="GetX"` → `Items="x"` (load data in `OnInitializedAsync`)
- Convert `ItemType="Namespace.Type"` → `TItem="Type"`
- Add `Context="Item"` to `<ItemTemplate>` elements
- Migrate code-behind: `Page_Load` → `OnInitializedAsync`, event handler signatures
- Convert `Response.Redirect` → `NavigationManager.NavigateTo`
- Wire `EditForm` where form validation is needed
- Convert Master Page → Blazor Layout (`@inherits LayoutComponentBase`, `@Body`)

### Layer 3 — Architecture Decisions (Human + Copilot)

These require project-specific judgment. See the [Architecture Decision Templates](#architecture-decision-templates) section:

- Session state → scoped services / ProtectedSessionStorage
- Data access layer → EF Core + service injection
- Authentication → Blazor Identity
- Third-party integrations → HttpClient
- Global.asax → Program.cs middleware pipeline
- Web.config → appsettings.json

### Expected Results

After Layer 1 automation on a typical Web Forms app:

| Page Status | Typical % | Meaning |
|-------------|-----------|---------|
| ✅ Markup Complete | ~12% | Ready to compile and run |
| ⚠️ Needs Layer 2 | ~64% | Copilot can handle with this skill |
| ❌ Needs Layer 3 | ~24% | Requires architecture decisions |

Over **75% of pages** are either complete or handleable through Copilot-assisted transforms.

---

## Page Migration Rules

### File Conversion

| Web Forms | Blazor |
|-----------|--------|
| `MyPage.aspx` | `MyPage.razor` |
| `MyPage.aspx.cs` | `MyPage.razor.cs` (partial class) or `@code { }` block |
| `MyControl.ascx` | `MyControl.razor` |
| `MyControl.ascx.cs` | `MyControl.razor.cs` |
| `Site.Master` | `MainLayout.razor` |
| `Site.Master.cs` | `MainLayout.razor.cs` |

### Directive Conversion

| Web Forms Directive | Blazor Equivalent |
|--------------------|-------------------|
| `<%@ Page Title="X" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Y.aspx.cs" Inherits="NS.Y" %>` | `@page "/route"` |
| `<%@ Master Language="C#" ... %>` | (remove — layouts don't need directives) |
| `<%@ Control Language="C#" ... %>` | (remove — components don't need directives) |
| `<%@ Register TagPrefix="uc" TagName="X" Src="~/Controls/X.ascx" %>` | `@using MyApp.Components` (if needed) |
| `<%@ Import Namespace="X" %>` | `@using X` |

**Drop these attributes entirely** (no Blazor equivalent):

- `AutoEventWireup`
- `CodeBehind` / `CodeFile`
- `Inherits` (unless using `@inherits` for a base class)
- `EnableViewState` / `ViewStateMode`
- `MasterPageFile` (layouts are set differently — see Layout section)
- `ValidateRequest`
- `MaintainScrollPositionOnPostBack`

### Expression Conversion

| Web Forms Expression | Blazor Equivalent | Notes |
|---------------------|-------------------|-------|
| `<%: expression %>` | `@(expression)` | HTML-encoded output |
| `<%= expression %>` | `@(expression)` | Blazor always HTML-encodes |
| `<%# Item.Property %>` | `@context.Property` | Inside data-bound templates |
| `<%#: Item.Property %>` | `@context.Property` | Same — Blazor always encodes |
| `<%# Eval("Property") %>` | `@context.Property` | Direct property access |
| `<%# Bind("Property") %>` | `@bind-Value="context.Property"` | Two-way binding |
| `<%# string.Format("{0:C}", Item.Price) %>` | `@context.Price.ToString("C")` | Format in code |
| `<%$ RouteValue:id %>` | `@Id` (with `[Parameter]`) | Route parameters |
| `<%-- comment --%>` | `@* comment *@` | Razor comments |
| `<% if (condition) { %>` | `@if (condition) {` | Control flow |
| `<% foreach (var x in items) { %>` | `@foreach (var x in items) {` | Loops |

### Route URL Conversion

| Web Forms | Blazor |
|-----------|--------|
| `href="~/Products"` | `href="/Products"` |
| `NavigateUrl="~/Products/<%: Item.ID %>"` | `NavigateUrl="@($"/Products/{context.ID}")"` |
| `<%: GetRouteUrl("ProductRoute", new { id = Item.ID }) %>` | `@($"/Products/{context.ID}")` |
| `Response.Redirect("~/Products")` | `NavigationManager.NavigateTo("/Products")` |
| `Response.RedirectToRoute("ProductRoute", new { id })` | `NavigationManager.NavigateTo($"/Products/{id}")` |

### Content/Layout Conversion

| Web Forms | Blazor |
|-----------|--------|
| `<asp:Content ContentPlaceHolderID="MainContent" runat="server">` | (remove — page body IS the content) |
| `</asp:Content>` | (remove) |
| `<asp:Content ContentPlaceHolderID="HeadContent" runat="server">` | `<HeadContent>` ... `</HeadContent>` |
| `<asp:ContentPlaceHolder ID="MainContent" runat="server" />` | `@Body` (in layout) |

### Form Wrapper

Web Forms wraps everything in `<form runat="server">`. In Blazor:

- **Remove the `<form runat="server">` wrapper** entirely
- For forms that need validation, wrap with `<EditForm Model="@model">` instead
- Individual BWFC validation controls work inside `<EditForm>` the same way

---

## Control Translation Table

### Simple Controls (Trivial Migration)

These controls require only removing `asp:` and `runat="server"`:

| Web Forms | BWFC | Changes |
|-----------|------|---------|
| `<asp:Label ID="x" runat="server" Text="Hello" CssClass="title" />` | `<Label @ref="x" Text="Hello" CssClass="title" />` | Remove `asp:`, `runat`; `ID` → `@ref` (if referenced) |
| `<asp:Literal ID="x" runat="server" Text="Hello" />` | `<Literal Text="Hello" />` | Remove `asp:`, `runat` |
| `<asp:HyperLink NavigateUrl="~/About" Text="About" runat="server" />` | `<HyperLink NavigateUrl="/About" Text="About" />` | Remove `asp:`, `runat`; `~/` → `/` |
| `<asp:Image ImageUrl="~/images/logo.png" runat="server" />` | `<Image ImageUrl="/images/logo.png" />` | Remove `asp:`, `runat`; `~/` → `/` |
| `<asp:Panel CssClass="container" runat="server">` | `<Panel CssClass="container">` | Remove `asp:`, `runat` |
| `<asp:PlaceHolder runat="server">` | `<PlaceHolder>` | Remove `asp:`, `runat` |
| `<asp:HiddenField Value="x" runat="server" />` | `<HiddenField Value="x" />` | Remove `asp:`, `runat` |

### Form Controls (Easy Migration)

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<asp:TextBox ID="Name" runat="server" />` | `<TextBox @bind-Text="model.Name" />` | Add `@bind-Text` for data binding |
| `<asp:TextBox TextMode="Password" runat="server" />` | `<TextBox TextMode="Password" @bind-Text="model.Password" />` | TextMode preserved |
| `<asp:TextBox TextMode="MultiLine" Rows="5" runat="server" />` | `<TextBox TextMode="Multiline" Rows="5" @bind-Text="model.Notes" />` | Note: `Multiline` not `MultiLine` |
| `<asp:DropDownList ID="Category" runat="server" />` | `<DropDownList @bind-SelectedValue="model.Category" Items="categories" />` | Bind items + selected value |
| `<asp:CheckBox ID="Active" runat="server" Checked="true" />` | `<CheckBox @bind-Checked="model.Active" />` | `@bind-Checked` |
| `<asp:RadioButton GroupName="G" runat="server" />` | `<RadioButton GroupName="G" />` | Same attributes |
| `<asp:FileUpload ID="Upload" runat="server" />` | `<FileUpload />` | Uses `InputFile` internally |
| `<asp:Button Text="Submit" OnClick="Submit_Click" runat="server" />` | `<Button Text="Submit" OnClick="Submit_Click" />` | `OnClick` is now `EventCallback` |
| `<asp:LinkButton Text="Edit" CommandName="Edit" runat="server" />` | `<LinkButton Text="Edit" CommandName="Edit" />` | Same attributes |
| `<asp:ImageButton ImageUrl="~/btn.png" OnClick="Btn_Click" runat="server" />` | `<ImageButton ImageUrl="/btn.png" OnClick="Btn_Click" />` | `~/` → `/` |

### Validation Controls (Easy Migration)

Validation controls are nearly 1:1 — same names, same attributes:

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<asp:RequiredFieldValidator ControlToValidate="Name" ErrorMessage="Required" runat="server" />` | `<RequiredFieldValidator ControlToValidate="Name" ErrorMessage="Required" />` | Remove `asp:`, `runat` |
| `<asp:CompareValidator ControlToCompare="Password" ControlToValidate="Confirm" runat="server" />` | `<CompareValidator ControlToCompare="Password" ControlToValidate="Confirm" />` | Same |
| `<asp:RangeValidator MinimumValue="1" MaximumValue="100" Type="Integer" runat="server" />` | `<RangeValidator MinimumValue="1" MaximumValue="100" Type="Integer" />` | Same |
| `<asp:RegularExpressionValidator ValidationExpression="\d+" runat="server" />` | `<RegularExpressionValidator ValidationExpression="\d+" />` | Same |
| `<asp:CustomValidator OnServerValidate="Validate" runat="server" />` | `<CustomValidator OnServerValidate="Validate" />` | Same |
| `<asp:ValidationSummary DisplayMode="BulletList" runat="server" />` | `<ValidationSummary DisplayMode="BulletList" />` | Same |
| `<asp:ModelErrorMessage ModelStateKey="key" runat="server" />` | `<ModelErrorMessage ModelStateKey="key" />` | Same |

### Data Controls (Medium Migration)

Data controls require additional changes for data binding.

#### GridView

```xml
<!-- Web Forms -->
<asp:GridView ID="ProductGrid" runat="server"
    ItemType="WingtipToys.Models.Product"
    SelectMethod="GetProducts"
    AutoGenerateColumns="false"
    AllowPaging="true" PageSize="10">
    <Columns>
        <asp:BoundField DataField="Name" HeaderText="Product" />
        <asp:TemplateField HeaderText="Price">
            <ItemTemplate><%#: Item.UnitPrice.ToString("C") %></ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
```

```razor
<!-- Blazor with BWFC -->
<GridView Items="products" TItem="Product"
    AutoGenerateColumns="false"
    AllowPaging="true" PageSize="10">
    <Columns>
        <BoundField DataField="Name" HeaderText="Product" />
        <TemplateField HeaderText="Price">
            <ItemTemplate Context="Item">@Item.UnitPrice.ToString("C")</ItemTemplate>
        </TemplateField>
    </Columns>
</GridView>
```

**Key changes:**

- `ItemType` → `TItem` (generic type parameter)
- `SelectMethod="GetProducts"` → `Items="products"` (bind to a property loaded in `OnInitializedAsync`)
- `<%#: Item.X %>` → `@Item.X` inside templates
- Add `Context="Item"` to `<ItemTemplate>` for naming the loop variable

#### ListView

```xml
<!-- Web Forms -->
<asp:ListView ID="ProductList" runat="server"
    ItemType="WingtipToys.Models.Product"
    SelectMethod="GetProducts">
    <ItemTemplate>
        <div class="product">
            <h3><%#: Item.ProductName %></h3>
            <asp:Image ImageUrl="<%#: Item.ImagePath %>" runat="server" />
            <p><%#: Item.UnitPrice.ToString("C") %></p>
        </div>
    </ItemTemplate>
</asp:ListView>
```

```razor
<!-- Blazor with BWFC -->
<ListView Items="products" TItem="Product">
    <ItemTemplate Context="Item">
        <div class="product">
            <h3>@Item.ProductName</h3>
            <Image ImageUrl="@Item.ImagePath" />
            <p>@Item.UnitPrice.ToString("C")</p>
        </div>
    </ItemTemplate>
</ListView>
```

#### FormView

```xml
<!-- Web Forms -->
<asp:FormView ID="ProductDetail" runat="server"
    ItemType="WingtipToys.Models.Product"
    SelectMethod="GetProduct"
    RenderOuterTable="false">
    <ItemTemplate>
        <h2><%#: Item.ProductName %></h2>
        <p><%#: Item.Description %></p>
        <p>Price: <%#: Item.UnitPrice.ToString("C") %></p>
    </ItemTemplate>
</asp:FormView>
```

```razor
<!-- Blazor with BWFC -->
<FormView DataItem="product" TItem="Product" RenderOuterTable="false">
    <ItemTemplate Context="Item">
        <h2>@Item.ProductName</h2>
        <p>@Item.Description</p>
        <p>Price: @Item.UnitPrice.ToString("C")</p>
    </ItemTemplate>
</FormView>
```

**Key changes:**

- `SelectMethod` → `DataItem` (single object, loaded in `OnInitializedAsync`)
- `Items` for collection-bound controls, `DataItem` for single-record controls

#### Repeater

```razor
<!-- Blazor with BWFC -->
<Repeater Items="items" TItem="MyItem">
    <ItemTemplate Context="Item">
        <div>@Item.Name — @Item.Value</div>
    </ItemTemplate>
    <SeparatorTemplate><hr /></SeparatorTemplate>
</Repeater>
```

### Navigation Controls

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<asp:Menu>` | `<Menu>` | MenuItem structure preserved |
| `<asp:SiteMapPath>` | `<SiteMapPath>` | Provide `SiteMapNode` data |
| `<asp:TreeView>` | `<TreeView>` | TreeNode structure preserved |
| `<asp:ScriptManager runat="server" />` | `<ScriptManager />` | Migration stub — renders nothing (correct for Blazor) |

### Login Controls

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<asp:Login>` | `<Login>` | Wire auth provider via service |
| `<asp:LoginView>` | `<LoginView>` | Uses `AuthenticationState` |
| `<asp:LoginStatus>` | `<LoginStatus>` | Uses `AuthenticationState` |
| `<asp:LoginName>` | `<LoginName>` | Uses `AuthenticationState` |
| `<asp:ChangePassword>` | `<ChangePassword>` | Requires Identity wiring |
| `<asp:CreateUserWizard>` | `<CreateUserWizard>` | Requires Identity wiring |
| `<asp:PasswordRecovery>` | `<PasswordRecovery>` | Requires Identity wiring |

### AJAX Controls

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<asp:ScriptManager>` | `<ScriptManager>` | No-op migration stub — renders nothing |
| `<asp:ScriptManagerProxy>` | `<ScriptManagerProxy>` | No-op migration stub — renders nothing |
| `<asp:Timer Interval="1000">` | `<Timer Interval="1000">` | Uses `System.Threading.Timer` internally |
| `<asp:UpdatePanel>` | `<UpdatePanel>` | Renders `<div>` or `<span>` — Blazor already does partial rendering |
| `<asp:UpdateProgress>` | `<UpdateProgress>` | Loading indicator with `ProgressTemplate` |

---

## Code-Behind Migration

### Lifecycle Methods

| Web Forms | Blazor | Notes |
|-----------|--------|-------|
| `Page_Load(object sender, EventArgs e)` | `protected override async Task OnInitializedAsync()` | First load |
| `Page_PreRender(...)` | `protected override async Task OnParametersSetAsync()` | Before each render |
| `Page_Init(...)` | `protected override void OnInitialized()` | Sync initialization |
| `IsPostBack` check | (not needed) | Blazor doesn't have postback |

**Pattern:**

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

// Blazor
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
    // handle click
    Response.Redirect("~/Confirmation");
}

// Blazor
private void SubmitBtn_Click()
{
    // handle click
    NavigationManager.NavigateTo("/Confirmation");
}
```

### Navigation

| Web Forms | Blazor |
|-----------|--------|
| `Response.Redirect("~/path")` | `NavigationManager.NavigateTo("/path")` |
| `Response.RedirectToRoute(...)` | `NavigationManager.NavigateTo($"/path/{param}")` |
| `Server.Transfer("~/page.aspx")` | `NavigationManager.NavigateTo("/page")` |

### Session State

| Web Forms | Blazor |
|-----------|--------|
| `Session["key"] = value;` | Inject a scoped service; use `ProtectedSessionStorage` |
| `Application["key"]` | Use a singleton service |
| `Cache["key"]` | Use `IMemoryCache` or `IDistributedCache` |
| `ViewState["key"]` | Use component fields (state is per-component) |

### Query String / Route Parameters

```csharp
// Web Forms (Model Binding)
public IQueryable<Product> GetProducts([QueryString] int? categoryId) { ... }

// Blazor
[SupplyParameterFromQuery] public int? CategoryId { get; set; }

protected override async Task OnInitializedAsync()
{
    products = await ProductService.GetProductsAsync(CategoryId);
}
```

```csharp
// Web Forms (RouteData)
public void GetProduct([RouteData] int productId) { ... }

// Blazor
@page "/Products/{ProductId:int}"
[Parameter] public int ProductId { get; set; }
```

### Data Access

```csharp
// Web Forms (SelectMethod pattern)
public IQueryable<Product> GetProducts()
{
    var db = new ProductContext();
    return db.Products;
}

// Blazor (Service injection)
@inject IProductService ProductService

private List<Product> products = new();

protected override async Task OnInitializedAsync()
{
    products = await ProductService.GetProductsAsync();
}
```

**Key change:** Replace inline `DbContext` usage with injected services. Register in `Program.cs`:

```csharp
builder.Services.AddDbContext<ProductContext>();
builder.Services.AddScoped<IProductService, ProductService>();
```

---

## Data Binding Migration

### Collection-Bound Controls

For controls that bind to collections (GridView, ListView, Repeater, DataList, DataGrid):

| Web Forms Pattern | BWFC Pattern |
|-------------------|-------------|
| `SelectMethod="GetProducts"` | `Items="products"` (load in `OnInitializedAsync`) |
| `ItemType="Namespace.Product"` | `TItem="Product"` |
| `DataSource=<%# GetItems() %>` + `DataBind()` | `Items="items"` |
| `DataKeyNames="ProductID"` | `DataKeyNames="ProductID"` (preserved) |

### Single-Record Controls

For controls that bind to a single object (FormView, DetailsView):

| Web Forms Pattern | BWFC Pattern |
|-------------------|-------------|
| `SelectMethod="GetProduct"` | `DataItem="product"` (load in `OnInitializedAsync`) |
| `ItemType="Namespace.Product"` | `TItem="Product"` |

### Template Binding

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<%# Item.Name %>` | `@Item.Name` | Add `Context="Item"` to template element |
| `<%# Eval("Name") %>` | `@Item.Name` | Direct property access replaces reflection |
| `<%# Bind("Name") %>` | `@bind-Value="Item.Name"` | Two-way in edit templates |
| `<%# Item.Price.ToString("C") %>` | `@Item.Price.ToString("C")` | Same formatting |

### DropDownList Data Binding

```xml
<!-- Web Forms -->
<asp:DropDownList ID="CategoryList" runat="server"
    SelectMethod="GetCategories"
    DataTextField="CategoryName"
    DataValueField="CategoryID"
    SelectedValue="<%# Item.CategoryID %>">
</asp:DropDownList>
```

```razor
<!-- Blazor with BWFC -->
<DropDownList Items="categories"
    DataTextField="CategoryName"
    DataValueField="CategoryID"
    @bind-SelectedValue="model.CategoryID" />
```

---

## Master Page → Layout Migration

### Web Forms Master Page Structure

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
            <nav>
                <asp:Menu ID="MainMenu" runat="server" ... />
            </nav>
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

<PageTitle>@PageTitle</PageTitle>
<HeadContent>
    @HeadContent
</HeadContent>

<header>
    <nav>
        <Menu ... />
    </nav>
</header>
<main>
    @Body
</main>
<footer>© @DateTime.Now.Year</footer>
```

**Key changes:**

- `<%@ Master %>` directive → `@inherits LayoutComponentBase`
- `<form runat="server">` → removed entirely
- `<asp:ContentPlaceHolder ID="MainContent">` → `@Body`
- `<asp:ContentPlaceHolder ID="HeadContent">` → Handled by `<HeadContent>` in child pages
- `<asp:ScriptManager>` → `<ScriptManager />` (renders nothing — correct)
- `<%: expression %>` → `@expression`

### Nested Master Pages

Web Forms supports nested Master Pages. In Blazor, use nested layouts:

```razor
@* ChildLayout.razor *@
@inherits LayoutComponentBase
@layout MainLayout

<div class="child-wrapper">
    @Body
</div>
```

---

## Architecture Decision Templates

These are the top 10 architecture decisions every Web Forms migration must address. Each includes the Web Forms pattern, the Blazor replacement, and a code example.

### 1. Master Page → Blazor Layout

**Web Forms:** `Site.Master` with `ContentPlaceHolder` controls.
**Blazor:** `MainLayout.razor` inheriting `LayoutComponentBase` with `@Body`.

See the [Master Page → Layout Migration](#master-page--layout-migration) section above for full details.

### 2. Session State → Scoped Services

**Web Forms:** `Session["cart"]` dictionary accessed anywhere in the request pipeline.
**Blazor:** Scoped services injected via DI. For persistence across circuits, use `ProtectedSessionStorage`.

```csharp
// Web Forms
Session["ShoppingCart"] = cart;
var cart = (ShoppingCart)Session["ShoppingCart"];

// Blazor — Define a scoped service
public class CartService
{
    public ShoppingCart Cart { get; set; } = new();
}

// Register in Program.cs
builder.Services.AddScoped<CartService>();

// Inject in component
@inject CartService CartService

// Use in code
CartService.Cart.AddItem(product);
```

### 3. ASP.NET Identity → Blazor Identity

**Web Forms:** ASP.NET Identity with OWIN middleware, `AccountController`, `.aspx` login pages.
**Blazor:** ASP.NET Core Identity with Blazor UI scaffolding.

```csharp
// Program.cs — Identity registration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add authentication state provider
builder.Services.AddCascadingAuthenticationState();
```

```razor
@* Use AuthorizeView in components *@
<AuthorizeView>
    <Authorized>Welcome, @context.User.Identity?.Name</Authorized>
    <NotAuthorized><a href="/Account/Login">Log in</a></NotAuthorized>
</AuthorizeView>
```

### 4. Entity Framework 6 → EF Core

**Web Forms:** EF6 with `DbContext` instantiated directly in code-behind or `SelectMethod`.
**Blazor:** EF Core with `IDbContextFactory` registered in DI.

```csharp
// Web Forms
public IQueryable<Product> GetProducts()
{
    var db = new ProductContext();
    return db.Products;
}

// Blazor — Program.cs
builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Blazor — Service
public class ProductService(IDbContextFactory<ProductContext> factory)
{
    public async Task<List<Product>> GetProductsAsync()
    {
        using var db = factory.CreateDbContext();
        return await db.Products.ToListAsync();
    }
}
```

> **Important:** Use `IDbContextFactory`, not `AddDbContext`, for Blazor Server. Blazor circuits are long-lived, and a single `DbContext` will accumulate stale data and tracking issues.

### 5. Global.asax → Program.cs / Middleware

**Web Forms:** `Application_Start`, `Application_Error`, `Session_Start` in `Global.asax`.
**Blazor:** Middleware pipeline in `Program.cs`.

```csharp
// Web Forms — Global.asax
protected void Application_Start(object sender, EventArgs e)
{
    RouteConfig.RegisterRoutes(RouteTable.Routes);
    BundleConfig.RegisterBundles(BundleTable.Bundles);
}

protected void Application_Error(object sender, EventArgs e)
{
    var ex = Server.GetLastError();
    Logger.LogError(ex);
}

// Blazor — Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddBlazorWebFormsComponents();

var app = builder.Build();
app.UseExceptionHandler("/Error");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
```

### 6. Web.config → appsettings.json

**Web Forms:** XML-based `Web.config` with `<connectionStrings>`, `<appSettings>`, custom sections.
**Blazor:** JSON-based `appsettings.json` with `IConfiguration` injection.

```xml
<!-- Web Forms — Web.config -->
<connectionStrings>
  <add name="DefaultConnection"
       connectionString="Data Source=(LocalDb)\MSSQLLocalDB;..."
       providerName="System.Data.SqlClient" />
</connectionStrings>
<appSettings>
  <add key="PayPalMode" value="sandbox" />
</appSettings>
```

```json
// Blazor — appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(LocalDb)\\MSSQLLocalDB;..."
  },
  "PayPal": {
    "Mode": "sandbox"
  }
}
```

```csharp
// Access in Blazor
@inject IConfiguration Configuration

var mode = Configuration["PayPal:Mode"];
```

### 7. Data Source Controls → Service Injection

**Web Forms:** `<asp:SqlDataSource>`, `<asp:ObjectDataSource>`, `<asp:EntityDataSource>` bound declaratively.
**Blazor:** No equivalent. Replace with injected services that load data in `OnInitializedAsync`.

```xml
<!-- Web Forms -->
<asp:SqlDataSource ID="ProductsDS" runat="server"
    ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"
    SelectCommand="SELECT * FROM Products" />
<asp:GridView DataSourceID="ProductsDS" runat="server" />
```

```razor
@* Blazor *@
@inject IProductService ProductService

<GridView Items="products" TItem="Product" AutoGenerateColumns="true" />

@code {
    private List<Product> products = new();

    protected override async Task OnInitializedAsync()
    {
        products = await ProductService.GetProductsAsync();
    }
}
```

### 8. Route Table → @page Directives

**Web Forms:** `RouteConfig.cs` with `routes.MapPageRoute(...)` or URL rewriting in `Global.asax`.
**Blazor:** `@page` directives at the top of each `.razor` file.

```csharp
// Web Forms — RouteConfig.cs
routes.MapPageRoute("ProductRoute", "Product/{productId}", "~/ProductDetail.aspx");
routes.MapPageRoute("CategoryRoute", "Category/{categoryId}", "~/ProductList.aspx");
```

```razor
@* Blazor — ProductDetail.razor *@
@page "/Product/{ProductId:int}"

@code {
    [Parameter] public int ProductId { get; set; }
}
```

```razor
@* Blazor — ProductList.razor *@
@page "/Category/{CategoryId:int}"

@code {
    [Parameter] public int CategoryId { get; set; }
}
```

### 9. HTTP Handlers/Modules → Middleware

**Web Forms:** `IHttpHandler` for request processing, `IHttpModule` for pipeline events.
**Blazor:** ASP.NET Core middleware.

```csharp
// Web Forms — IHttpHandler
public class ImageHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var id = context.Request.QueryString["id"];
        // serve image
    }
    public bool IsReusable => true;
}

// Blazor — Middleware
app.MapGet("/api/images/{id}", async (int id, ImageService svc) =>
{
    var image = await svc.GetImageAsync(id);
    return Results.File(image.Data, image.ContentType);
});
```

### 10. Third-Party Integrations → HttpClient

**Web Forms:** Direct HTTP calls or SDK usage in code-behind with `WebRequest`/`WebClient`.
**Blazor:** `HttpClient` via DI with `IHttpClientFactory`.

```csharp
// Web Forms
var request = WebRequest.Create("https://api.paypal.com/v1/payments");
// ... configure and send

// Blazor — Program.cs
builder.Services.AddHttpClient("PayPal", client =>
{
    client.BaseAddress = new Uri("https://api.paypal.com/v1/");
});

// Blazor — Service
public class PayPalService(IHttpClientFactory factory)
{
    public async Task<PaymentResult> CreatePaymentAsync(Order order)
    {
        var client = factory.CreateClient("PayPal");
        var response = await client.PostAsJsonAsync("payments", order);
        return await response.Content.ReadFromJsonAsync<PaymentResult>();
    }
}
```

---

## Common Gotchas

### No ViewState

Blazor components maintain their own state in fields/properties. There is no `ViewState` dictionary. If code reads/writes `ViewState["key"]`, replace with a component field.

### No PostBack

There is no `IsPostBack`. Code in `Page_Load` that checks `if (!IsPostBack)` should move to `OnInitializedAsync()` (which runs once on first load).

### No DataSource Controls

`<asp:SqlDataSource>`, `<asp:ObjectDataSource>`, `<asp:EntityDataSource>` have no BWFC equivalents. Replace with injected services that load data in `OnInitializedAsync()`.

### ID Rendering

Web Forms generates client IDs like `ctl00_MainContent_GridView1`. Blazor doesn't render component IDs into HTML. If CSS or JavaScript targets these IDs, use `CssClass` instead or add explicit `id` attributes.

### Template Context Variable

In Web Forms, `Item` is implicitly available in templates. In BWFC, use `Context="Item"` on template elements:

```razor
<ItemTemplate Context="Item">
    @Item.PropertyName
</ItemTemplate>
```

### `runat="server"` on HTML Elements

Some Web Forms pages add `runat="server"` to plain HTML elements (e.g., `<div runat="server">`). Remove `runat="server"` — use `@ref` if the element needs programmatic access.

### String Format in Binding

```xml
<!-- Web Forms -->
<%#: string.Format("{0:C}", Item.Price) %>

<!-- Blazor -->
@Item.Price.ToString("C")
```

### Visibility Pattern

```xml
<!-- Web Forms -->
<asp:Panel Visible="false" runat="server">...</asp:Panel>

<!-- Blazor option 1: BWFC Visible parameter -->
<Panel Visible="false">...</Panel>

<!-- Blazor option 2: Razor conditional (preferred for dynamic) -->
@if (showPanel)
{
    <Panel>...</Panel>
}
```

### Event Handler Signatures

Web Forms event handlers use `(object sender, EventArgs e)`. BWFC uses `EventCallback`, so handlers take no parameters or the specific args type:

```csharp
// Web Forms
protected void Btn_Click(object sender, EventArgs e) { }

// Blazor — no parameters
private void Btn_Click() { }

// Blazor — with mouse event args (if needed)
private void Btn_Click(MouseEventArgs e) { }
```

### `TextMode="MultiLine"` Casing

BWFC uses `Multiline` (lowercase 'l'), not `MultiLine` (uppercase 'L'). This is a common typo that produces a silent failure.

### ScriptManager and ScriptManagerProxy Are No-Ops

These components exist purely for migration compatibility. They render nothing. Include them during migration to prevent markup errors, then remove them when the migration is stable.

---

## Attributes Removed During Migration

These Web Forms attributes have no Blazor equivalent and should be silently removed:

- `runat="server"` — always remove
- `AutoEventWireup="true"` — no equivalent
- `CodeBehind="X.aspx.cs"` — no equivalent (use `.razor.cs` convention)
- `CodeFile="X.aspx.cs"` — same as CodeBehind
- `Inherits="Namespace.Class"` — use `@inherits` only if needed
- `EnableViewState="false"` — no ViewState in Blazor
- `ViewStateMode="Disabled"` — no ViewState in Blazor
- `ValidateRequest="false"` — no request validation in Blazor
- `MaintainScrollPositionOnPostBack="true"` — no postback in Blazor
- `ClientIDMode="Static"` — no client ID munging in Blazor
- `EnableTheming="false"` — not applicable
- `SkinID="X"` — use BWFC theming system if needed

---

## Per-Page Migration Checklist

Copy this checklist for each page being migrated. Use it as a GitHub issue body or tracking document.

```markdown
## Page: [PageName.aspx] → [PageName.razor]

### Layer 1 — Mechanical (Automated)
- [ ] File renamed (.aspx → .razor, .aspx.cs → .razor.cs)
- [ ] <%@ Page %> directive replaced with @page "/route"
- [ ] asp: prefixes removed from all controls
- [ ] runat="server" removed from all elements
- [ ] Expressions converted (<%: %> → @(), <%# %> → @context.)
- [ ] URL references converted (~/ → /)
- [ ] <asp:Content> wrappers removed
- [ ] <form runat="server"> wrapper removed

### Layer 2 — Structural (Copilot-Assisted)
- [ ] SelectMethod → Items/DataItem binding wired
- [ ] ItemType → TItem converted
- [ ] Data loading moved to OnInitializedAsync
- [ ] Event handlers converted (removed sender/EventArgs params)
- [ ] Template Context="Item" variables added
- [ ] Navigation calls converted (Response.Redirect → NavigateTo)
- [ ] Form validation wired (EditForm if needed)
- [ ] IsPostBack checks removed/restructured

### Layer 3 — Architecture Decisions
- [ ] Data access pattern decided (service injection)
- [ ] State management approach decided
- [ ] Authentication/authorization wired (if applicable)
- [ ] Third-party integrations ported (if applicable)
- [ ] Route registered (@page directive with correct URL)

### Verification
- [ ] Page builds without errors
- [ ] Page renders correctly in browser
- [ ] Visual comparison against original Web Forms page
- [ ] Interactive features work (forms, buttons, navigation)
- [ ] No browser console errors
```

---

## Component Coverage Summary

**52 components** across 7 categories:

| Category | Count | Components |
|----------|-------|-----------|
| **Editor Controls** | 25 | AdRotator, BulletedList, Button, Calendar, CheckBox, CheckBoxList, DropDownList, FileUpload, HiddenField, HyperLink, Image, ImageButton, Label, LinkButton, ListBox, Literal, Localize, MultiView, Panel, PlaceHolder, RadioButton, RadioButtonList, Table, TextBox, View |
| **Data Controls** | 8 | DataGrid, DataList, DataPager, DetailsView, FormView, GridView, ListView, Repeater |
| **Validation Controls** | 7 | CompareValidator, CustomValidator, ModelErrorMessage, RangeValidator, RegularExpressionValidator, RequiredFieldValidator, ValidationSummary |
| **Navigation Controls** | 3 | Menu, SiteMapPath, TreeView |
| **Login Controls** | 7 | ChangePassword, CreateUserWizard, Login, LoginName, LoginStatus, LoginView, PasswordRecovery |
| **AJAX Controls** | 5 | ScriptManager, ScriptManagerProxy, Timer, UpdatePanel, UpdateProgress |
| **Migration Helpers** | 1 | Substitution |

### What BWFC Does NOT Cover

These Web Forms controls have **no BWFC equivalent**. Plan alternative approaches:

| Control | Recommended Alternative |
|---------|------------------------|
| `SqlDataSource` | Replace with injected service + EF Core |
| `ObjectDataSource` | Replace with injected service |
| `EntityDataSource` | Replace with injected service + EF Core |
| `LinqDataSource` | Replace with injected service + LINQ |
| `XmlDataSource` | Replace with injected service + `XDocument` |
| `SiteMapDataSource` | Build navigation data in a service |
| `Wizard` | Build multi-step form with component state |
| `Web Parts` | No equivalent — redesign as Blazor components |
| `AJAX Toolkit Extenders` | No equivalent — use Blazor interactivity or JS interop |

---

## Files to Create During Migration

For a typical Web Forms → Blazor migration, create these files:

1. **`Program.cs`** — Service registration, middleware pipeline
2. **`App.razor`** — Root component with Router
3. **`_Imports.razor`** — Global usings including BWFC namespaces
4. **`Components/Layout/MainLayout.razor`** — From Master Page
5. **`Components/Pages/*.razor`** — One per `.aspx` page
6. **`Services/*.cs`** — Replace DataSource controls and code-behind data methods
7. **`Models/*.cs`** — Copy/migrate from Web Forms project (often .NET Standard already)
