---
name: bwfc-migration
description: "Migrate ASP.NET Web Forms .aspx/.ascx/.master markup to Blazor Server using BlazorWebFormsComponents (BWFC). Covers project setup, NuGet installation, control translation, expression conversion, data binding, code-behind lifecycle, and Master Page to Layout conversion. For Identity/auth migration use /bwfc-identity-migration. For data access and architecture decisions use /bwfc-data-migration."
---

# Web Forms → Blazor Markup Migration with BWFC

> ## 🚫 MANDATORY: USE BWFC COMPONENTS — NEVER REPLACE WITH RAW HTML
>
> Every `<asp:X>` control in Web Forms **MUST** become a `<X>` BWFC component in Blazor.
> **NEVER** replace BWFC components with plain HTML elements (`<table>`, `<input>`, `<span>`, `<a>`, `@foreach` loops).
> This is the #1 rule of this migration skill. Violations destroy CSS, break functionality, and defeat the purpose of the library.

This skill provides transformation rules for migrating ASP.NET Web Forms markup to Blazor Server using the **BlazorWebFormsComponents** (BWFC) NuGet package. **BWFC is the primary migration strategy** — it provides the fastest path with the highest fidelity.

**Related skills:**
- `/bwfc-identity-migration` — ASP.NET Identity/Membership → Blazor Identity
- `/bwfc-data-migration` — EF6 → EF Core, DataSource → services, architecture decisions

---

## 1. BWFC Library — USE IT FIRST

BlazorWebFormsComponents is an open-source library that provides **drop-in Blazor replacements** for ASP.NET Web Forms server controls. It preserves the same component names, attribute names, and rendered HTML output — enabling migration with minimal markup changes.

- **NuGet Package:** <https://www.nuget.org/packages/Fritz.BlazorWebFormsComponents>
- **GitHub Repository:** <https://github.com/FritzAndFriends/BlazorWebFormsComponents>
- **110+ components** across 8 categories (including sub-components and style objects)
- **Same HTML output** — existing CSS and JavaScript continue to work
- **Utility features** — `WebFormsPageBase`, `AddBlazorWebFormsComponents()`, `<Page />` component, theming

> **Core Principle:** Strip `asp:` and `runat="server"`, keep everything else, and it just works.

### Complete BWFC Component Inventory

| Category | Components |
|----------|-----------|
| **Editor Controls** (33) | AdRotator, Button, BulletedList, Calendar, Chart, CheckBox, CheckBoxList, DropDownList, FileUpload, HiddenField, HyperLink, Image, ImageButton, ImageMap, Label, LinkButton, ListBox, Literal, MultiView, Panel, PlaceHolder, RadioButton, RadioButtonList, Substitution, Table, TableCell, TableRow, TableHeaderRow, TableFooterRow, TableHeaderCell, TextBox, Timer, View |
| **Data Controls** (22+) | BoundField, ButtonField, DataGrid, DataList, DataPager, DetailsView, FormView, GridView, GridViewRow, HyperLinkField, ListView, Repeater, TemplateField + style sub-components: AlternatingItemStyle, EditRowStyle, FooterStyle, HeaderStyle, ItemStyle, PagerStyle, PagerSettings, RowStyle, SelectedRowStyle |
| **Navigation Controls** (6+) | Menu, MenuItem, MenuItemStyle, SiteMapPath, TreeView, TreeNode + node style sub-components |
| **Login Controls** (8+) | ChangePassword, CreateUserWizard, Login, LoginName, **LoginStatus**, **LoginView**, PasswordRecovery, RoleGroup + style sub-components: CheckBoxStyle, FailureTextStyle, HyperLinkStyle, InstructionTextStyle, LabelStyle, LoginButtonStyle, SuccessTextStyle, TextBoxStyle, TitleTextStyle, ValidatorTextStyle |
| **Validation Controls** (7) | CompareValidator, CustomValidator, ModelErrorMessage, RangeValidator, RegularExpressionValidator, RequiredFieldValidator, ValidationSummary (maps to `asp:ValidationSummary`) |
| **Layout Controls** (9) | Content, ContentPlaceHolder, MasterPage, EmptyLayout, NamingContainer, UpdatePanel, UpdateProgress, ScriptManager, ScriptManagerProxy |
| **Utility Features** | `WebFormsPageBase` (base class), `<Page />` (render component), `AddBlazorWebFormsComponents()` (service registration), `BaseWebFormsComponent`, `BaseStyledComponent`, `ThemeProvider`, `FontInfo` |

### BWFC Utility Features — MANDATORY for Every Migration

These utility features are **not optional**. Every migration MUST use them:

1. **`AddBlazorWebFormsComponents()`** — Call in `Program.cs`. Registers `IPageService`, `IHttpContextAccessor`, JS interop, and all required services.

2. **`WebFormsPageBase`** — Add `@inherits BlazorWebFormsComponents.WebFormsPageBase` in `_Imports.razor`. Provides:
   - `Page.Title` / `Title` — delegates to `IPageService`
   - `Page.MetaDescription` / `MetaDescription`
   - `Page.MetaKeywords` / `MetaKeywords`
   - `IsPostBack` — always returns `false` (migration compatibility)
   - `Page` — returns `this` (enables `Page.Title = "X"` syntax)
   - `GetRouteUrl(routeName, routeValues)` — URL generation

3. **`<BlazorWebFormsComponents.Page />`** — Place in `MainLayout.razor`. Renders `<PageTitle>` and `<meta>` tags from `IPageService`.

4. **`BaseStyledComponent`** — All styled controls inherit this. Provides `CssClass`, `BackColor`, `ForeColor`, `Font`, `BorderStyle`, `BorderWidth`, `BorderColor`, `Height`, `Width`, `ToolTip` — matching Web Forms styling API exactly.

5. **Theming** — `ThemeProvider`, `ThemeConfiguration`, `ControlSkin` with fluent API. CascadingValue-based.

### ⚠️ Commonly Missed: LoginView and LoginStatus

`<asp:LoginView>` and `<asp:LoginStatus>` are **BWFC components** — do NOT skip them or replace with raw HTML:

```razor
@* ✅ CORRECT — use BWFC LoginView/LoginStatus *@
<LoginView>
    <AnonymousTemplate>
        <a href="/Account/Login">Log in</a>
    </AnonymousTemplate>
    <LoggedInTemplate>
        Welcome, <LoginName />!
        <LoginStatus LogoutAction="Redirect" LogoutPageUrl="/" />
    </LoggedInTemplate>
</LoginView>

@* ❌ WRONG — replaced with raw HTML *@
@if (isAuthenticated) {
    <span>Welcome, @userName!</span>
    <a href="/Account/Logout">Log out</a>
} else {
    <a href="/Account/Login">Log in</a>
}
```

See `/bwfc-identity-migration` for full auth migration details.

---

## 🚫 Control Preservation — MANDATORY (Zero Exceptions)

> **NEVER flatten `asp:` controls to raw HTML.** Always preserve them as BWFC components.

Every `<asp:GridView>` becomes `<GridView>`, every `<asp:TextBox>` becomes `<TextBox>`, every `<asp:LoginView>` becomes `<LoginView>`, etc. The BWFC library exists so these components **keep working** — with the same HTML output, the same attributes, and the same features.

### What "Flattening" Means (and Why It's Forbidden)

| Web Forms Control | BWFC Component (✅ CORRECT) | Flattened HTML (❌ FORBIDDEN) |
|---|---|---|
| `<asp:GridView>` | `<GridView Items="@data">` | `<table>` + `@foreach` loop |
| `<asp:ListView>` | `<ListView Items="@data">` | `@foreach` + HTML divs |
| `<asp:TextBox>` | `<TextBox @bind-Text="val" />` | `<input @bind="val" />` |
| `<asp:Label>` | `<Label Text="Hello" />` | `<span>Hello</span>` |
| `<asp:HyperLink>` | `<HyperLink NavigateUrl="/x" />` | `<a href="/x">` |
| `<asp:Button>` | `<Button Text="Go" OnClick="X" />` | `<button @onclick="X">` |
| `<asp:CheckBox>` | `<CheckBox @bind-Checked="val" />` | `<input type="checkbox">` |
| `<asp:LoginView>` | `<LoginView>` | `@if (isAuth) { ... }` |
| `<asp:LoginStatus>` | `<LoginStatus />` | `<a href="/logout">` |
| `<asp:Panel>` | `<Panel CssClass="x">` | `<div class="x">` |

**Why this matters:**
- BWFC components render the **exact same HTML** as Web Forms controls → existing CSS continues to work
- BWFC components include built-in features (sorting, paging, editing, GridLines, CellPadding, etc.) → `@foreach` loops have none
- Preserving the component means the markup is 90% done → flattening requires rewriting the entire page

The migration script's `Test-BwfcControlPreservation` function catches control loss automatically. See the **migration-standards** skill for full rules.

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

### Step 3: Register BWFC Services and Configure Program.cs

In `Program.cs`:

```csharp
builder.Services.AddBlazorWebFormsComponents();
builder.Services.AddHttpContextAccessor();
```

Also ensure the middleware pipeline includes static file serving:

```csharp
app.UseStaticFiles();   // MUST come before MapStaticAssets
app.MapStaticAssets();
```

> **Important:** `MapStaticAssets()` alone does NOT reliably serve files from `wwwroot/` subdirectories (e.g., `wwwroot/Content/`, `wwwroot/Catalog/Images/`). You MUST also call `UseStaticFiles()` and it MUST come first.

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

## Migration Workflow

This skill covers **Layers 1 and 2** of the three-layer pipeline. Use the related skills for Layer 3.

| Layer | What It Handles | Skill |
|-------|----------------|-------|
| **Layer 1: Mechanical** | Tag prefixes, `runat`, expressions, URLs, file renaming | ✅ This skill |
| **Layer 2: Structural** | Data binding, code-behind lifecycle, templates, layouts | ✅ This skill |
| **Layer 3: Architecture** | State management, data access, auth, middleware | `/bwfc-data-migration`, `/bwfc-identity-migration` |

### Layer 1 — Mechanical Transforms

These are 100% mechanical — apply to every file:

- Remove all `asp:` tag prefixes
- Remove all `runat="server"` attributes
- Convert expressions: `<%: expr %>` → `@(expr)`, `<%# Item.X %>` → `@context.X`
- Convert URLs: `~/path` → `/path`
- Rename files: `.aspx` → `.razor`, `.ascx` → `.razor`, `.master` → `.razor`
- Remove `<asp:Content>` wrappers
- Convert `<%@ Page %>` directives to `@page "/route"`
- Remove `<form runat="server">` wrapper

### Layer 2 — Structural Transforms

- Convert `SelectMethod="GetX"` → `Items="x"` (load in `OnInitializedAsync`)
- Convert `ItemType="Namespace.Type"` → `TItem="Type"`
- Add `Context="Item"` to `<ItemTemplate>` elements
- Migrate code-behind: `Page_Load` → `OnInitializedAsync`
- Convert `Response.Redirect` → `NavigationManager.NavigateTo`
- Wire `EditForm` where form validation is needed
- Convert Master Page → Blazor Layout

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
| `<%: GetRouteUrl("ProductRoute", new { id = Item.ID }) %>` | `@GetRouteUrl("ProductRoute", new { id = context.ID })` (via WebFormsPageBase) |
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

**Key changes:** `ItemType` → `TItem`, `SelectMethod` → `Items`, add `Context="Item"` to templates.

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

**Key changes:** `SelectMethod` → `DataItem` for single records, `Items` for collections.

### Navigation Controls

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<asp:Menu>` | `<Menu>` | Same structure, same sub-elements |
| `<asp:TreeView>` | `<TreeView>` | Same node structure |
| `<asp:SiteMapPath>` | `<SiteMapPath>` | Same attributes |

### Login Controls (Commonly Missed — Use BWFC!)

> **These controls are frequently skipped or replaced with raw HTML during migration. They MUST be preserved as BWFC components.**

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<asp:Login runat="server" />` | `<Login />` | Login form with username/password |
| `<asp:LoginName runat="server" />` | `<LoginName />` | Displays authenticated username |
| `<asp:LoginStatus runat="server" />` | `<LoginStatus />` | Login/Logout toggle link — **do NOT replace with a raw `<a>` tag** |
| `<asp:LoginView runat="server">` | `<LoginView>` | Anon vs auth content — **do NOT replace with `@if` block** |
| `<asp:CreateUserWizard runat="server" />` | `<CreateUserWizard />` | Registration form |
| `<asp:ChangePassword runat="server" />` | `<ChangePassword />` | Password change form |
| `<asp:PasswordRecovery runat="server" />` | `<PasswordRecovery />` | Password reset flow |

#### LoginView Migration Example

```html
<!-- Web Forms -->
<asp:LoginView runat="server">
    <AnonymousTemplate>
        <a href="~/Account/Login">Log in</a>
    </AnonymousTemplate>
    <LoggedInTemplate>
        Welcome, <asp:LoginName runat="server" />!
        <asp:LoginStatus runat="server" LogoutAction="Redirect" LogoutPageUrl="~/" />
    </LoggedInTemplate>
    <RoleGroups>
        <asp:RoleGroup Roles="Administrator">
            <ContentTemplate><a href="~/Admin">Admin Panel</a></ContentTemplate>
        </asp:RoleGroup>
    </RoleGroups>
</asp:LoginView>
```

```razor
@* Blazor with BWFC — preserves all controls *@
<LoginView>
    <AnonymousTemplate>
        <a href="/Account/Login">Log in</a>
    </AnonymousTemplate>
    <LoggedInTemplate>
        Welcome, <LoginName />!
        <LoginStatus LogoutAction="Redirect" LogoutPageUrl="/" />
    </LoggedInTemplate>
    <RoleGroups>
        <RoleGroup Roles="Administrator">
            <ContentTemplate><a href="/Admin">Admin Panel</a></ContentTemplate>
        </RoleGroup>
    </RoleGroups>
</LoginView>
```

> **Note:** If you later want to convert `LoginView` → `AuthorizeView` as a long-term upgrade, see `/bwfc-identity-migration`. But the initial migration should use the BWFC `<LoginView>` component for maximum fidelity.

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
| `ItemType="Namespace.Product"` | `TItem="Product"` |
| `DataSource=<%# GetItems() %>` + `DataBind()` | `Items="items"` |
| `DataKeyNames="ProductID"` | `DataKeyNames="ProductID"` (preserved) |

### Single-Record Controls

For FormView, DetailsView:

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

### Nested Master Pages → Nested Layouts

```razor
@inherits LayoutComponentBase
@layout MainLayout

<div class="child-wrapper">
    @Body
</div>
```

---

## Static File & Asset Migration

When migrating a Web Forms application, static files (CSS, images, fonts, scripts) must be relocated to the Blazor `wwwroot/` directory and all references updated.

### UseStaticFiles() + MapStaticAssets() in Program.cs

Both middleware calls are required for static files to load correctly:

```csharp
app.UseStaticFiles();   // Serves files from wwwroot/ subdirectories
app.MapStaticAssets();   // Optimized static asset delivery
```

`UseStaticFiles()` **must** come before `MapStaticAssets()`. Without `UseStaticFiles()`, files in subdirectories like `wwwroot/Content/` or `wwwroot/Catalog/Images/` may return 404.

### CSS Link Extraction from Master Pages → App.razor

Web Forms master pages (`Site.Master`) contain `<link rel="stylesheet">` tags in the `<head>` section. These must be extracted and placed in `App.razor`'s `<head>`:

```razor
@* App.razor — extracted from Site.Master *@
<head>
    <link rel="stylesheet" href="/Content/Site.css" />
    <link rel="stylesheet" href="/Content/bootstrap.css" />
    @* ... other stylesheets from the master page *@
</head>
```

**Conversion rules:**
- `~/Content/Site.css` → `/Content/Site.css` (strip `~` prefix)
- Remove `runat="server"` from link tags
- `<script>` tags from the master page `<head>` should also be noted for `App.razor`

The migration script handles this automatically via `New-AppRazorScaffold -SourceRoot`, which extracts stylesheet links from the master page and writes them into the generated `App.razor`.

### Image/Asset Path Preservation

!!! warning "Paths must match actual file locations"
    Template markup that references images (e.g., `<img src="...">`, `<Image ImageUrl="...">`) must use paths that match WHERE the files actually land in `wwwroot/`. A common mistake is generating invented paths like `/Images/Products/foo.png` when the actual files are at `/Catalog/Images/foo.png`.

**Rule:** Check the source project's actual image directory structure and preserve it. The migration script copies all static files to `wwwroot/` preserving the original directory structure.

| Web Forms Path | wwwroot Path | Blazor Reference |
|---|---|---|
| `~/Catalog/Images/foo.png` | `wwwroot/Catalog/Images/foo.png` | `/Catalog/Images/foo.png` |
| `~/Content/Site.css` | `wwwroot/Content/Site.css` | `/Content/Site.css` |
| `~/Scripts/app.js` | `wwwroot/Scripts/app.js` | `/Scripts/app.js` |

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

### AuthorizeView Requires Auth Services

!!! danger "CRASH-LEVEL: App will throw at runtime"
    When `<asp:LoginView>` is converted to `<AuthorizeView>`, the app **will crash** with:
    `InvalidOperationException: Authorization requires a cascading parameter of type Task<AuthenticationState>`

Even without full ASP.NET Core Identity, if ANY page uses `<AuthorizeView>`, `Program.cs` MUST include:

```csharp
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();
```

The migration script flags this as a manual review item when `LoginView` is detected in the source. See `/bwfc-identity-migration` for full Identity setup.

### ScriptManager/ScriptManagerProxy Are No-Ops
Include during migration to prevent errors, remove when stable.

### 🚫 Never Flatten Controls (THE #1 Anti-Pattern)

> **This is the most common migration failure mode.** Agents and developers consistently replace BWFC data controls with `@foreach` loops and HTML tables. This destroys functionality.

```razor
@* ❌ BAD — flattened to raw HTML, loses ALL BWFC features *@
<table class="table">@foreach (var item in items) { <tr><td>@item.Name</td></tr> }</table>

@* ❌ BAD — HyperLink replaced with raw <a> tag *@
<a href="/Products/@item.ID">@item.Name</a>

@* ❌ BAD — LoginView replaced with @if block *@
@if (isAuthenticated) { <span>Welcome</span> }

@* ✅ RIGHT — preserved as BWFC component *@
<GridView TItem="Product" Items="@items" AutoGenerateColumns="false">
    <Columns><BoundField DataField="Name" HeaderText="Product" /></Columns>
</GridView>

@* ✅ RIGHT — HyperLink preserved *@
<HyperLink NavigateUrl="@($"/Products/{item.ID}")" Text="@item.Name" />

@* ✅ RIGHT — LoginView preserved *@
<LoginView>
    <LoggedInTemplate>Welcome, <LoginName />!</LoggedInTemplate>
</LoginView>
```
See `migration-toolkit/skills/migration-standards/SKILL.md` for complete rules and the ShoppingCart case study.

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
- [ ] ItemType → TItem
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

**110+ components** across 8 categories (including sub-components and style objects):

| Category | Count | Components |
|----------|-------|-----------|
| **Editor Controls** | 33 | AdRotator, BulletedList, Button, Calendar, Chart, CheckBox, CheckBoxList, DropDownList, FileUpload, HiddenField, HyperLink, Image, ImageButton, ImageMap, Label, LinkButton, ListBox, Literal, Localize, MultiView, Panel, PlaceHolder, RadioButton, RadioButtonList, Substitution, Table, TableCell, TableRow, TableHeaderRow, TableFooterRow, TableHeaderCell, TextBox, View |
| **Data Controls** | 22+ | DataGrid, DataList, DataPager, DetailsView, FormView, GridView, GridViewRow, ListView, Repeater + field types: BoundField, ButtonField, HyperLinkField, TemplateField + style sub-components: AlternatingItemStyle, EditRowStyle, FooterStyle, HeaderStyle, ItemStyle, PagerStyle, PagerSettings, RowStyle, SelectedRowStyle |
| **Validation** | 7 | CompareValidator, CustomValidator, ModelErrorMessage, RangeValidator, RegularExpressionValidator, RequiredFieldValidator, ValidationSummary |
| **Navigation** | 6+ | Menu, MenuItem, MenuItemStyle, SiteMapPath, TreeView, TreeNode + node style sub-components |
| **Login** | 18+ | ChangePassword, CreateUserWizard, Login, **LoginName**, **LoginStatus**, **LoginView**, PasswordRecovery, RoleGroup + style sub-components: CheckBoxStyle, FailureTextStyle, HyperLinkStyle, InstructionTextStyle, LabelStyle, LoginButtonStyle, SuccessTextStyle, TextBoxStyle, TitleTextStyle, ValidatorTextStyle |
| **AJAX/Layout** | 14 | ScriptManager, ScriptManagerProxy, Timer, UpdatePanel, UpdateProgress, Content, ContentPlaceHolder, MasterPage, EmptyLayout, NamingContainer + Timer |
| **Utility** | — | WebFormsPageBase, Page (render component), AddBlazorWebFormsComponents(), BaseWebFormsComponent, BaseStyledComponent, ThemeProvider, ThemeConfiguration, ControlSkin, FontInfo |

### Page Base Class

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `System.Web.UI.Page` (base class) | `WebFormsPageBase` | `@inherits WebFormsPageBase` in `_Imports.razor`; `Page.Title`, `Page.MetaDescription`, `Page.MetaKeywords`, `IsPostBack` work unchanged |

### Standard Blazor Server-Side Patterns (for infrastructure)

These are NOT BWFC components — they are standard Blazor/ASP.NET Core patterns for application infrastructure:

| Concern | Pattern | Where |
|---------|---------|-------|
| **Static files** | `app.UseStaticFiles()` + `app.MapStaticAssets()` | `Program.cs` — `UseStaticFiles()` MUST come first |
| **CSS links** | `<link rel="stylesheet" href="/Content/Site.css" />` | `App.razor` `<head>` section |
| **JS references** | `<script src="/Scripts/app.js"></script>` | `App.razor` after `<Routes>` |
| **BWFC JS** | `<script src="_content/Fritz.BlazorWebFormsComponents/js/Basepage.js"></script>` | `App.razor` `<head>` |
| **Render mode** | `@rendermode="InteractiveServer"` on `HeadOutlet` and `Routes` | `App.razor` |
| **CSS isolation** | `{Component}.razor.css` | Component-scoped styles |

### Not Covered by BWFC (Requires Manual Migration)

| Control | Alternative |
|---------|------------|
| `SqlDataSource` | Injected service + EF Core (see `/bwfc-data-migration`) |
| `ObjectDataSource` | Injected service |
| `EntityDataSource` | Injected service + EF Core |
| `Wizard` | Multi-step form with component state |
| `Web Parts` | Redesign as Blazor components |
| `AJAX Toolkit Extenders` | Blazor interactivity or JS interop |
