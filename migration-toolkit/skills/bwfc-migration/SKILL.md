---
name: bwfc-migration
description: "Migrate ASP.NET Web Forms .aspx/.ascx/.master markup to Blazor Server using BlazorWebFormsComponents (BWFC). Covers project setup, NuGet installation, control translation, expression conversion, data binding, code-behind lifecycle, and Master Page to Layout conversion. For Identity/auth migration use /bwfc-identity-migration. For data access and architecture decisions use /bwfc-data-migration."
---

# Web Forms → Blazor Markup Migration with BWFC

This skill provides transformation rules for migrating ASP.NET Web Forms markup to Blazor Server using the **BlazorWebFormsComponents** (BWFC) NuGet package.

**Related skills:**
- `/bwfc-identity-migration` — ASP.NET Identity/Membership → Blazor Identity
- `/bwfc-data-migration` — EF6 → EF Core, DataSource → services, architecture decisions

## What Is BWFC?

BlazorWebFormsComponents is an open-source library that provides **drop-in Blazor replacements** for ASP.NET Web Forms server controls. It preserves the same component names, attribute names, and rendered HTML output — enabling migration with minimal markup changes.

- **NuGet Package:** <https://www.nuget.org/packages/Fritz.BlazorWebFormsComponents>
- **GitHub Repository:** <https://github.com/FritzAndFriends/BlazorWebFormsComponents>
- **58 components** across 6 categories
- **Same HTML output** — existing CSS and JavaScript continue to work

> **Core Principle:** Strip `asp:` and `runat="server"`, keep everything else, and it just works.

---

## Installation

### Step 1: Create Blazor Server Project

```bash
dotnet new blazor -n MyBlazorApp --interactivity Server
cd MyBlazorApp
dotnet add package Fritz.BlazorWebFormsComponents
```

### Step 2: Configure `_Imports.razor`

```razor
@using BlazorWebFormsComponents
@using BlazorWebFormsComponents.Enums
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@inherits BlazorWebFormsComponents.WebFormsPageBase
```

The `@inherits` line makes every page inherit from `WebFormsPageBase`, which provides `Page.Title`, `Page.MetaDescription`, `Page.MetaKeywords`, and `IsPostBack` — so Web Forms code-behind patterns compile unchanged. No per-page `@inject IPageService` is needed for page-level usage. Individual pages can override with their own `@inherits` if needed.

> **Note:** The `@using static` import lets you write `InteractiveServer` as shorthand in `App.razor`. Do **not** add `@rendermode InteractiveServer` as a line in `_Imports.razor` — `@rendermode` is a directive attribute that belongs on component instances, not a standalone directive.

> **Note:** `@inject IPageService` is still valid for non-page components (e.g., a shared header component) that need access to page metadata. `WebFormsPageBase` only applies to routable pages.

### Step 2b: Configure Render Mode in `App.razor`

The `dotnet new blazor --interactivity Server` template generates `App.razor` with render mode already set. Verify it contains:

```razor
<HeadOutlet @rendermode="InteractiveServer" />
...
<Routes @rendermode="InteractiveServer" />
```

This enables global server interactivity for all pages. See [ASP.NET Core Blazor render modes](https://learn.microsoft.com/aspnet/core/blazor/components/render-modes) for per-page alternatives.

### Step 3: Register BWFC Services and Add Page Component to Layout

In `Program.cs`:

```csharp
builder.Services.AddBlazorWebFormsComponents();
```

In your layout file (`MainLayout.razor`), add the `<Page />` render component once. This subscribes to `IPageService` and emits `<PageTitle>` and `<meta>` tags:

```razor
@inherits LayoutComponentBase

<BlazorWebFormsComponents.Page />

<header>
    <!-- ... -->
</header>
<main>
    @Body
</main>
```

> **Important:** `WebFormsPageBase` provides the code-behind API (`Page.Title`, `IsPostBack`). The `<BlazorWebFormsComponents.Page />` component does the rendering (`<PageTitle>`, `<meta>` tags). Both are required.

### Step 4: Add BWFC JavaScript

In `App.razor` or the host page `<head>`:

```html
<script src="_content/Fritz.BlazorWebFormsComponents/js/Basepage.js"></script>
```

---

## Migration Pipeline — MANDATORY

> ⚠️ **CRITICAL: The migration pipeline is a two-layer automated sequence. Both layers MUST run. Do NOT make any manual code fixes between Layer 1 and Layer 2.**
> The migration pipeline measures script quality. Manual fixes between layers corrupt the measurement. If Layer 1 output has issues, those issues should be fixed in the script, not patched by hand.

The migration pipeline has **two mandatory layers** that run in strict sequence:

| Step | Layer | Executor | Description |
|------|-------|----------|-------------|
| 1 | **Layer 1: Mechanical** | **Automated script** (`bwfc-migrate.ps1`) | Tag transforms, expression conversion, file renaming, scaffolding |
| 2 | **Layer 2: Structural** | **Copilot-assisted** (this skill) | Data binding, lifecycle, templates, layouts |
| 3 | Build & verify | Copilot | `dotnet build`, fix any remaining compile errors |
| 4 | Report | Copilot | Document results |

### Layer 1 — Run the Migration Script

**You MUST run Layer 1 as a PowerShell script. Do NOT apply Layer 1 transforms manually.**

```powershell
.\migration-toolkit\scripts\bwfc-migrate.ps1 -Path "<source-webforms-project>" -Output "<blazor-output-dir>"
```

- `-Path` — path to the source Web Forms project directory (containing `.aspx`, `.ascx`, `.master` files)
- `-Output` — path to the target Blazor project directory (will be created if it doesn't exist)
- Layer 1 typically completes in 1–2 seconds and processes 30+ files
- The script performs ALL mechanical transforms: `asp:` prefix removal, `runat="server"` removal, expression conversion, URL rewriting, file renaming, scaffold generation (`.csproj`, `Program.cs`, `_Imports.razor`, `App.razor`, etc.)

**What Layer 1 handles:**

- Remove all `asp:` tag prefixes
- Remove all `runat="server"` attributes
- Convert expressions: `<%: expr %>` → `@(expr)`, `<%# Item.X %>` → `@context.X`
- Convert URLs: `~/path` → `/path`
- Rename files: `.aspx` → `.razor`, `.ascx` → `.razor`, `.master` → `.razor`
- Remove `<asp:Content>` wrappers
- Convert `<%@ Page %>` directives to `@page "/route"`
- Remove `<form runat="server">` wrapper
- LoginView preservation (keeps BWFC LoginView, does NOT rewrite as AuthorizeView)
- Master page → MainLayout.razor conversion
- Scaffold generation (csproj, Program.cs, _Imports.razor, App.razor)

### Layer 2 — Copilot Transforms

**After Layer 1 completes, immediately proceed to Layer 2. Do NOT fix, edit, or clean up any Layer 1 output first.**

Layer 2 is where Copilot applies structural transforms to every generated `.razor` and `.razor.cs` file. Work through each file and apply ALL of the following:

- Convert `SelectMethod="GetX"` → `Items="@_x"` (load data in `OnInitializedAsync`)
- Preserve `ItemType` attribute — BWFC data controls use `ItemType` (matches Web Forms `DataBoundControl.ItemType`). Do NOT change to `TItem` or any other name.
- Add `Context="Item"` to `<ItemTemplate>` elements
- Migrate code-behind: `Page_Load` → `OnInitializedAsync`
- Convert `Response.Redirect` → `NavigationManager.NavigateTo`
- Wire `EditForm` where form validation is needed
- Convert Master Page → Blazor Layout
- Ensure null-safe collection access (e.g., `Items="@(_products ?? new())"`)
- Add `@inject` directives for required services (NavigationManager, DbContext, etc.)
- Convert `Session["key"]` → scoped DI service patterns

### Pipeline Rules

1. **Run Layer 1 first** — always via the script, never manually
2. **Run Layer 2 immediately after** — no fixes between layers
3. **Build** — run `dotnet build` and fix compile errors
4. **Report** — document what was migrated and any issues

## Migration Workflow

This skill covers **Layers 1 and 2** of the three-layer pipeline. Use the related skills for Layer 3.

| Layer | What It Handles | Skill |
|-------|----------------|-------|
| **Layer 1: Mechanical** | Tag prefixes, `runat`, expressions, URLs, file renaming | ✅ This skill (automated via `bwfc-migrate.ps1`) |
| **Layer 2: Structural** | Data binding, code-behind lifecycle, templates, layouts | ✅ This skill (Copilot-assisted) |
| **Layer 3: Architecture** | State management, data access, auth, middleware | `/bwfc-data-migration`, `/bwfc-identity-migration` |

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

**Drop entirely** (no Blazor equivalent): `AutoEventWireup`, `CodeBehind`/`CodeFile`, `Inherits`, `EnableViewState`/`ViewStateMode`, `MasterPageFile`, `ValidateRequest`, `MaintainScrollPositionOnPostBack`, `ClientIDMode`, `EnableTheming`, `SkinID`

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

### Content/Layout Conversion

| Web Forms | Blazor |
|-----------|--------|
| `<asp:Content ContentPlaceHolderID="MainContent" runat="server">` | (remove — page body IS the content) |
| `<asp:Content ContentPlaceHolderID="HeadContent" runat="server">` | `<HeadContent>` ... `</HeadContent>` |
| `<asp:ContentPlaceHolder ID="MainContent" runat="server" />` | `@Body` (in layout) |

### Form Wrapper

- **Remove the `<form runat="server">` wrapper** entirely
- For forms that need validation, use `<EditForm Model="@model">` instead

---

## Control Translation Table

### Simple Controls (Trivial Migration)

| Web Forms | BWFC | Changes |
|-----------|------|---------|
| `<asp:Label ID="x" runat="server" Text="Hello" CssClass="title" />` | `<Label @ref="x" Text="Hello" CssClass="title" />` | Remove `asp:`, `runat`; `ID` → `@ref` (if referenced) |
| `<asp:Literal ID="x" runat="server" Text="Hello" />` | `<Literal Text="Hello" />` | Remove `asp:`, `runat` |
| `<asp:HyperLink NavigateUrl="~/About" Text="About" runat="server" />` | `<HyperLink NavigateUrl="/About" Text="About" />` | `~/` → `/` |
| `<asp:Image ImageUrl="~/images/logo.png" runat="server" />` | `<Image ImageUrl="/images/logo.png" />` | `~/` → `/` |
| `<asp:Panel CssClass="container" runat="server">` | `<Panel CssClass="container">` | Remove `asp:`, `runat` |
| `<asp:PlaceHolder runat="server">` | `<PlaceHolder>` | Remove `asp:`, `runat` |
| `<asp:HiddenField Value="x" runat="server" />` | `<HiddenField Value="x" />` | Remove `asp:`, `runat` |

### Form Controls (Easy Migration)

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<asp:TextBox ID="Name" runat="server" />` | `<TextBox @bind-Text="model.Name" />` | Add `@bind-Text` |
| `<asp:TextBox TextMode="Password" runat="server" />` | `<TextBox TextMode="Password" @bind-Text="model.Password" />` | TextMode preserved |
| `<asp:TextBox TextMode="MultiLine" Rows="5" runat="server" />` | `<TextBox TextMode="Multiline" Rows="5" @bind-Text="model.Notes" />` | Note: `Multiline` not `MultiLine` |
| `<asp:DropDownList ID="Category" runat="server" />` | `<DropDownList @bind-SelectedValue="model.Category" Items="categories" />` | Bind items + selected value |
| `<asp:CheckBox ID="Active" runat="server" Checked="true" />` | `<CheckBox @bind-Checked="model.Active" />` | `@bind-Checked` |
| `<asp:RadioButton GroupName="G" runat="server" />` | `<RadioButton GroupName="G" />` | Same attributes |
| `<asp:FileUpload ID="Upload" runat="server" />` | `<FileUpload />` | Uses `InputFile` internally |
| `<asp:Button Text="Submit" OnClick="Submit_Click" runat="server" />` | `<Button Text="Submit" OnClick="Submit_Click" />` | `OnClick` is `EventCallback` |
| `<asp:LinkButton Text="Edit" CommandName="Edit" runat="server" />` | `<LinkButton Text="Edit" CommandName="Edit" />` | Same attributes |
| `<asp:ImageButton ImageUrl="~/btn.png" OnClick="Btn_Click" runat="server" />` | `<ImageButton ImageUrl="/btn.png" OnClick="Btn_Click" />` | `~/` → `/` |

### Validation Controls (Easy Migration)

| Web Forms | BWFC |
|-----------|------|
| `<asp:RequiredFieldValidator ControlToValidate="Name" ErrorMessage="Required" runat="server" />` | `<RequiredFieldValidator ControlToValidate="Name" ErrorMessage="Required" />` |
| `<asp:CompareValidator ControlToCompare="Password" ControlToValidate="Confirm" runat="server" />` | `<CompareValidator ControlToCompare="Password" ControlToValidate="Confirm" />` |
| `<asp:RangeValidator MinimumValue="1" MaximumValue="100" Type="Integer" runat="server" />` | `<RangeValidator MinimumValue="1" MaximumValue="100" Type="Integer" />` |
| `<asp:RegularExpressionValidator ValidationExpression="\d+" runat="server" />` | `<RegularExpressionValidator ValidationExpression="\d+" />` |
| `<asp:CustomValidator OnServerValidate="Validate" runat="server" />` | `<CustomValidator OnServerValidate="Validate" />` |
| `<asp:ValidationSummary DisplayMode="BulletList" runat="server" />` | `<ValidationSummary DisplayMode="BulletList" />` |
| `<asp:ModelErrorMessage ModelStateKey="key" runat="server" />` | `<ModelErrorMessage ModelStateKey="key" />` |

### Data Controls (Medium Migration)

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
<GridView Items="products" ItemType="Product"
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

**Key changes:** `ItemType` preserved as-is, `SelectMethod` → `Items`, add `Context="Item"` to templates.

#### ListView

```xml
<!-- Web Forms -->
<asp:ListView ID="ProductList" runat="server"
    ItemType="WingtipToys.Models.Product" SelectMethod="GetProducts">
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
<ListView Items="products" ItemType="Product">
    <ItemTemplate Context="Item">
        <div class="product">
            <h3>@Item.ProductName</h3>
            <Image ImageUrl="@Item.ImagePath" />
            <p>@Item.UnitPrice.ToString("C")</p>
        </div>
    </ItemTemplate>
</ListView>
```

#### ListView with GroupItemCount

Web Forms `ListView` supports `GroupItemCount` for grid-style layouts (e.g., 4 products per row). BWFC preserves this attribute and the `GroupTemplate`/`LayoutTemplate` structure.

```xml
<!-- Web Forms — ListView with GroupItemCount for 4-column grid -->
<asp:ListView ID="productList" runat="server"
    ItemType="WingtipToys.Models.Product" SelectMethod="GetProducts"
    GroupItemCount="4">
    <LayoutTemplate>
        <table><tr runat="server" id="groupPlaceholder" /></table>
    </LayoutTemplate>
    <GroupTemplate>
        <tr><td runat="server" id="itemPlaceholder" /></tr>
    </GroupTemplate>
    <ItemTemplate>
        <td>
            <a href="<%#: GetRouteUrl("ProductRoute", new { productId = Item.ProductID }) %>">
                <img src="<%#: Item.ImagePath %>" alt="<%#: Item.ProductName %>" />
            </a>
            <span><%#: Item.ProductName %></span>
            <span><%#: Item.UnitPrice.ToString("C") %></span>
        </td>
    </ItemTemplate>
</asp:ListView>
```

```razor
@* Blazor — BWFC ListView preserves GroupItemCount and templates *@
<ListView Items="products" ItemType="Product" GroupItemCount="4">
    <LayoutTemplate>@context</LayoutTemplate>
    <GroupTemplate>@context</GroupTemplate>
    <ItemTemplate>
        <td>
            <a href="@($"/Products/{context.ProductID}")">
                <img src="@context.ImagePath" alt="@context.ProductName" />
            </a>
            <span>@context.ProductName</span>
            <span>@context.UnitPrice.ToString("C")</span>
        </td>
    </ItemTemplate>
</ListView>
```

**Key changes:** `GroupItemCount` preserved as-is. `LayoutTemplate` and `GroupTemplate` use `@context` as the placeholder (BWFC renders the table/tr structure). `ItemTemplate` uses `@context.Property` instead of `<%#: Item.Property %>`.

#### FormView

```razor
<!-- Blazor with BWFC -->
<FormView DataItem="product" ItemType="Product" RenderOuterTable="false">
    <ItemTemplate Context="Item">
        <h2>@Item.ProductName</h2>
        <p>@Item.Description</p>
        <p>Price: @Item.UnitPrice.ToString("C")</p>
    </ItemTemplate>
</FormView>
```

**Key changes:** `SelectMethod` → `DataItem` for single records, `Items` for collections.

### Navigation Controls

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<asp:Menu>` | `<Menu>` | Same structure, same sub-elements |
| `<asp:TreeView>` | `<TreeView>` | Same node structure |
| `<asp:SiteMapPath>` | `<SiteMapPath>` | Same attributes |

### AJAX Controls (No-Op Migration)

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<asp:ScriptManager runat="server" />` | `<ScriptManager />` | Renders nothing — migration compatibility |
| `<asp:ScriptManagerProxy runat="server" />` | `<ScriptManagerProxy />` | Renders nothing |
| `<asp:UpdatePanel runat="server">` | `<UpdatePanel>` | Passes through children |
| `<asp:UpdateProgress runat="server">` | `<UpdateProgress>` | Passes through children |
| `<asp:Timer Interval="5000" runat="server" />` | `<Timer Interval="5000" />` | Fires OnTick events |

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
| `SelectMethod="GetProducts"` | `Items="products"` (load in `OnInitializedAsync`) |
| `ItemType="Namespace.Product"` | `ItemType="Product"` (strip namespace only) |
| `DataSource=<%# GetItems() %>` + `DataBind()` | `Items="items"` |
| `DataKeyNames="ProductID"` | `DataKeyNames="ProductID"` (preserved) |

### Single-Record Controls

For FormView, DetailsView:

| Web Forms Pattern | BWFC Pattern |
|-------------------|-------------|
| `SelectMethod="GetProduct"` | `DataItem="product"` (load in `OnInitializedAsync`) |
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

---

## Common Gotchas

### No ViewState
Replace `ViewState["key"]` with component fields.

### No PostBack
`if (!IsPostBack)` → works AS-IS with `WebFormsPageBase` (always enters the block). `if (IsPostBack)` (without `!`) → **dead code** in Blazor; flag for manual review and move logic to event handlers.

### No DataSource Controls
`SqlDataSource`, `ObjectDataSource`, `EntityDataSource` → injected services. See `/bwfc-data-migration`.

### ID Rendering
Blazor doesn't render component IDs. Use `CssClass` or explicit `id` attributes for CSS/JS targeting.

### Template Context Variable
Add `Context="Item"` on template elements:
```razor
<ItemTemplate Context="Item">
    @Item.PropertyName
</ItemTemplate>
```

### `runat="server"` on HTML Elements
Remove `runat="server"` from plain HTML elements. Use `@ref` if programmatic access needed.

### Event Handler Signatures
```csharp
// Web Forms: protected void Btn_Click(object sender, EventArgs e) { }
// Blazor:    private void Btn_Click() { }
```

### `TextMode="MultiLine"` Casing
BWFC uses `Multiline` (lowercase 'l'), not `MultiLine`. Silent failure if wrong.

### ScriptManager/ScriptManagerProxy Are No-Ops
Include during migration to prevent errors, remove when stable.

---

## Per-Page Migration Checklist

```markdown
## Page: [PageName.aspx] → [PageName.razor]

### Layer 1 — Mechanical
- [ ] File renamed (.aspx → .razor)
- [ ] <%@ Page %> → @page "/route"
- [ ] asp: prefixes removed
- [ ] runat="server" removed
- [ ] Expressions converted
- [ ] URLs converted (~/ → /)
- [ ] <asp:Content> wrappers removed
- [ ] <form runat="server"> removed

### Layer 2 — Structural
- [ ] SelectMethod → Items/DataItem
- [ ] ItemType preserved (strip namespace prefix only)
- [ ] Data loading in OnInitializedAsync
- [ ] Event handlers converted
- [ ] Template Context="Item" added
- [ ] Navigation calls converted

### Verification
- [ ] Builds without errors
- [ ] Renders correctly
- [ ] Interactive features work
- [ ] No browser console errors
```

---

## Component Coverage Summary

**58 components** across 6 categories:

| Category | Count | Components |
|----------|-------|-----------|
| **Editor Controls** | 28 | AdRotator, BulletedList, Button, Calendar, Chart, CheckBox, CheckBoxList, DropDownList, FileUpload, HiddenField, HyperLink, Image, ImageButton, ImageMap, Label, LinkButton, ListBox, Literal, Localize, MultiView, Panel, PlaceHolder, RadioButton, RadioButtonList, Substitution, Table, TextBox, View |
| **Data Controls** | 8 | DataGrid, DataList, DataPager, DetailsView, FormView, GridView, ListView, Repeater |
| **Validation** | 7 | CompareValidator, CustomValidator, ModelErrorMessage, RangeValidator, RegularExpressionValidator, RequiredFieldValidator, ValidationSummary |
| **Navigation** | 3 | Menu, SiteMapPath, TreeView |
| **Login** | 7 | ChangePassword, CreateUserWizard, Login, LoginName, LoginStatus, LoginView, PasswordRecovery |
| **AJAX** | 5 | ScriptManager, ScriptManagerProxy, Timer, UpdatePanel, UpdateProgress |

### Page Base Class

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `System.Web.UI.Page` (base class) | `WebFormsPageBase` | `@inherits WebFormsPageBase` in `_Imports.razor`; `Page.Title`, `Page.MetaDescription`, `Page.MetaKeywords`, `IsPostBack` work unchanged |

### Structural & Infrastructure Components

These BWFC components support the migration infrastructure — they aren't direct control replacements but provide essential bridging capabilities:

| Component | Purpose | Usage |
|-----------|---------|-------|
| `WebFormsPage` | Unified layout wrapper combining NamingContainer + ThemeProvider + head rendering | `<WebFormsPage>@Body</WebFormsPage>` in `MainLayout.razor` — single component replaces multiple wrappers |
| `Page` | Standalone head renderer (renders `<PageTitle>` + `<meta>` from `IPageService`) | `<BlazorWebFormsComponents.Page />` in layout — use if you don't need `WebFormsPage` |
| `NamingContainer` | Establishes naming scope for child component IDs (emulates `INamingContainer`) | `<NamingContainer ID="MainContent">` — children get prefixed IDs like `MainContent_ButtonName` |
| `MasterPage` | Emulates Web Forms MasterPage with ContentPlaceHolder support | Gradual migration path — allows preserving `<MasterPage>` / `<Content>` / `<ContentPlaceHolder>` markup |
| `Content` | Provides content for a `ContentPlaceHolder` in a `MasterPage` | `<Content ContentPlaceHolderID="MainContent">...</Content>` |
| `ContentPlaceHolder` | Defines a replaceable region in a `MasterPage` | `<ContentPlaceHolder ID="MainContent" />` |
| `EmptyLayout` | Minimal layout component (`@inherits LayoutComponentBase` + `@Body`) | Use for pages that need no layout chrome |

### DataBinder.Eval Compatibility Shim

BWFC provides a `DataBinder` static class for legacy code that uses `DataBinder.Eval()`. This is marked `[Obsolete]` — use direct property access (`@context.Property`) instead.

```csharp
// Web Forms — DataBinder.Eval
<%# DataBinder.Eval(Container.DataItem, "ProductName") %>

// BWFC shim (compiles, but marked obsolete)
@DataBinder.Eval(context, "ProductName")

// Recommended — direct property access
@context.ProductName
```

### Theming Infrastructure

BWFC includes a theming system for migrating Web Forms skin files:

| Component | Purpose |
|-----------|---------|
| `ThemeProvider` (in `Theming/`) | Cascades `ThemeConfiguration` to child components |
| `ThemeConfiguration` | Defines theme settings (skin mappings) |
| `ControlSkin` / `SkinBuilder` | Internal — apply skin properties to controls |

> **Note:** Theming is an advanced feature. Most migrations can ignore it initially and add theming later if needed.

### Custom Control Base Classes

For migrating custom Web Forms controls that extend `System.Web.UI.WebControls.WebControl`:

| BWFC Class | Purpose |
|-----------|---------|
| `WebControl` (in `CustomControls/`) | Base class shim for custom controls |
| `CompositeControl` | Base class for controls that contain child controls |
| `HtmlTextWriter` | Shim for `Render(HtmlTextWriter)` patterns — use for gradual migration |

### Not Covered by BWFC

| Control | Alternative |
|---------|------------|
| `SqlDataSource` | Injected service + EF Core (see `/bwfc-data-migration`) |
| `ObjectDataSource` | Injected service |
| `EntityDataSource` | Injected service + EF Core |
| `Wizard` | Multi-step form with component state |
| `Web Parts` | Redesign as Blazor components |
| `AJAX Toolkit Extenders` | Blazor interactivity or JS interop |
