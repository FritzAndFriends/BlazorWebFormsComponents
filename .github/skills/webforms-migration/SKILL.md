---
name: webforms-migration
description: "Migrate ASP.NET Web Forms applications (.aspx/.ascx/.master) to Blazor Server using BlazorWebFormsComponents (BWFC). Use this skill when converting Web Forms markup, code-behind, Master Pages, User Controls, or data-binding patterns to Blazor equivalents."
---

# Web Forms → Blazor Migration Skill

This skill provides complete transformation rules for migrating ASP.NET Web Forms applications to Blazor Server using the **BlazorWebFormsComponents** (BWFC) NuGet package. The BWFC library provides Blazor components with identical names, attributes, and HTML output to ASP.NET Web Forms controls — enabling migration with minimal markup changes.

## Core Principle

> **Strip `asp:` and `runat="server"`, keep everything else, and it just works.**

BWFC components match Web Forms control names, property names, and rendered HTML. A well-structured Web Forms page can often be migrated by removing the `asp:` prefix, removing `runat="server"`, and making a small set of structural adjustments.

---

## Migration Recipe (Step-by-Step)

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

### Step 5: Migrate Master Page → Blazor Layout

See [Master Page Migration Rules](#master-page-migration).

### Step 6: Migrate Pages

For each `.aspx` page, apply the [Page Migration Rules](#page-migration-rules) and [Control Translation Table](#control-translation-table).

### Step 7: Migrate Code-Behind

See [Code-Behind Migration Rules](#code-behind-migration).

### Step 8: Wire Data Access

Replace `DataSource` controls and `DataBind()` calls with service injection. See [Data Binding Migration](#data-binding-migration).

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

Data controls require additional changes for data binding:

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
| `<asp:ScriptManager runat="server" />` | `<ScriptManager />` | Renders nothing — correct for Blazor |

### Login Controls

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<asp:Login>` | `<Login>` | Wire auth provider via service |
| `<asp:LoginView>` | `<LoginView>` | Uses `AuthenticationState` |
| `<asp:LoginStatus>` | `<LoginStatus>` | Uses `AuthenticationState` |
| `<asp:LoginName>` | `<LoginName>` | Uses `AuthenticationState` |

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

## Master Page Migration

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

## BWFC Component Coverage

**52 components** across 7 categories:

| Category | Components |
|----------|-----------|
| **Editor Controls** | AdRotator, BulletedList, Button, Calendar, CheckBox, CheckBoxList, DropDownList, FileUpload, HiddenField, HyperLink, Image, ImageButton, Label, LinkButton, ListBox, Literal, Localize, MultiView, Panel, PlaceHolder, RadioButton, RadioButtonList, Table, TextBox, View |
| **Data Controls** | DataGrid, DataList, DataPager, DetailsView, FormView, GridView, ListView, Repeater |
| **Validation Controls** | CompareValidator, CustomValidator, ModelErrorMessage, RangeValidator, RegularExpressionValidator, RequiredFieldValidator, ValidationSummary |
| **Navigation Controls** | Menu, SiteMapPath, TreeView |
| **Login Controls** | ChangePassword, CreateUserWizard, Login, LoginName, LoginStatus, LoginView, PasswordRecovery |
| **AJAX Controls** | ScriptManager, ScriptManagerProxy, Timer, UpdatePanel, UpdateProgress |

---

## WingtipToys-Specific Patterns

The WingtipToys canonical demo (2013) uses these specific patterns that BWFC fully supports:

| WingtipToys Pattern | BWFC Migration |
|---------------------|---------------|
| `ItemType="WingtipToys.Models.Product"` | `TItem="Product"` |
| `SelectMethod="GetProducts"` | `Items="products"` (load in `OnInitializedAsync`) |
| `<asp:ListView>` with `LayoutTemplate`/`GroupTemplate` | `<ListView>` — templates preserved |
| `<asp:FormView>` with `RenderOuterTable="false"` | `<FormView RenderOuterTable="false">` ✅ |
| `<asp:GridView>` with `BoundField` + `TemplateField` | Same column types in BWFC |
| ASP.NET Identity login/register | Migrate to Blazor Identity (separate from BWFC) |
| Shopping cart with `Session` state | Replace with scoped service + `ProtectedSessionStorage` |
| `~/` route prefix | Replace with `/` |
| `<asp:ModelErrorMessage>` | `<ModelErrorMessage>` ✅ |

---

## Files to Create During Migration

For a typical Web Forms → Blazor migration, create these files:

1. **`Program.cs`** — Service registration, middleware pipeline
2. **`App.razor`** — Root component with Router
3. **`_Imports.razor`** — Global usings including BWFC namespaces
4. **`Components/Layout/MainLayout.razor`** — From Master Page
5. **`Components/Pages/*.razor`** — One per .aspx page
6. **`Services/*.cs`** — Replace DataSource controls and code-behind data methods
7. **`Models/*.cs`** — Copy/migrate from Web Forms project (often .NET Standard already)
