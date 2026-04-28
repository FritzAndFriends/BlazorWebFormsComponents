# Transform Reference

This page documents the flat markup and code-behind transforms applied by the `webforms-to-blazor` CLI tool. They run in a fixed sequence before the separate **[Semantic Pattern Catalog](semantic-pattern-catalog.md)** pass.

## Transform Execution Order

| Order | Name | Type | Category | Purpose |
|-------|------|------|----------|---------|
| 10 | TodoHeader | Code-Behind | Meta | Inject TODO guidance header |
| 20 | PageDirective | Markup | Directive | Convert `<%@ Page %>` → `@page` |
| 30 | MasterDirective | Markup | Directive | Convert `<%@ Master %>` → BWFC master shell |
| 40 | ControlDirective | Markup | Directive | Convert `<%@ Control %>` → `@inherits` |
| 50 | RegisterDirective | Markup | Directive | Handle `<%@ Register %>` for custom controls |
| 60 | ImportDirective | Markup | Directive | Convert `<%@ Import %>` → `@using` |
| 250 | MasterPageTransform | Markup | Markup | Convert master markup to runnable `<MasterPage>` shell syntax |
| 300 | ContentWrapperTransform | Markup | Markup | Wrap loose content in `<div>` if needed |
| 310 | FormWrapperTransform | Markup | Markup | Convert `<form runat="server">` to Blazor form |
| 400 | ExpressionTransform | Markup | Markup | Convert `<%: %>`, `<%= %>` to `@()` |
| 510 | LoginViewTransform | Markup | Markup | Convert `<asp:LoginView>` → `<AuthorizeView>` |
| 520 | SelectMethodTransform | Markup | Markup | Flag SelectMethod/InsertMethod/etc. |
| 610 | AjaxToolkitPrefixTransform | Markup | Markup | Remove `ajaxToolkit:` prefixes |
| 620 | AspPrefixTransform | Markup | Markup | Remove `asp:` prefixes from controls |
| 700 | AttributeStripTransform | Markup | Markup | Remove `runat="server"`, `AutoEventWireup` |
| 750 | EventWiringTransform | Markup | Markup | Convert `OnClick="X"` → `OnClick="@X"` |
| 780 | UrlReferenceTransform | Markup | Markup | Convert `~/` paths to `/` |
| 800 | TemplatePlaceholderTransform | Markup | Markup | Convert `Item` → `context` in templates |
| 810 | AttributeNormalizeTransform | Markup | Markup | Normalize attribute values (booleans, enums) |
| 820 | DataSourceIdTransform | Markup | Markup | Replace DataSourceID with Items binding |
| 30 | GetRouteUrlTransform | Code-Behind | Code-Behind | Flag `Page.GetRouteUrl()` calls |
| 50 | GetRouteUrlTransform | Markup | Markup | Flag `<%: Page.GetRouteUrl() %>` expressions |
| 400 | SessionDetectTransform | Code-Behind | Code-Behind | Detect Session/Cache, inject shim references |
| 410 | ViewStateDetectTransform | Code-Behind | Code-Behind | Detect ViewState usage, flag migration |
| 500 | IsPostBackTransform | Code-Behind | Code-Behind | Unwrap `if (!IsPostBack)` guards |
| 510 | PageLifecycleTransform | Code-Behind | Code-Behind | Convert Page_Load, Page_Init → Blazor lifecycle |
| 520 | EventHandlerSignatureTransform | Code-Behind | Code-Behind | Adapt event handler signatures |
| 30 | BaseClassStripTransform | Code-Behind | Code-Behind | Remove `System.Web.UI.Page` base class |
| 20 | UsingStripTransform | Code-Behind | Code-Behind | Remove Web Forms and ASP.NET using statements |
| 25 | ResponseRedirectTransform | Code-Behind | Code-Behind | Convert `Response.Redirect()` → `NavigationManager.NavigateTo()` |
| 40 | DataBindTransform | Code-Behind | Code-Behind | Flag `DataBind()` calls |
| 50 | UrlCleanupTransform | Code-Behind | Code-Behind | Clean URL literals in code |

---

## Directive Transforms

### 1. PageDirective (Order: 20)

**Converts ASP.NET Page directives to Blazor routes.**

**Web Forms:**
```html
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Product.aspx.cs" Inherits="MyApp.Product" %>
<h1>Products</h1>
```

**Output:**
```razor
@page "/product"
@inherits MyApp.Product

<h1>Products</h1>
```

**What It Does:**
- Extracts `Inherits` attribute → `@inherits`
- Infers route from filename or `Url` attribute
- Removes boilerplate attributes (Language, AutoEventWireup, CodeBehind)
- Adds TODO if custom routing logic is detected

---

### 2. MasterDirective (Order: 30)

**Converts Master Page directives to BWFC shell components.**

**Web Forms:**
```html
<%@ Master Language="C#" CodeBehind="Site.master.cs" Inherits="MyApp.Site" %>
```

**Output:**
```razor
<MasterPage>
  <ChildContent>
    @ChildContent
  </ChildContent>
</MasterPage>
```

**What It Does:**
- Removes the Web Forms master directive
- Hands off to `MasterPageTransform` to build a runnable BWFC shell
- Preserves the master/content contract instead of forcing immediate `@Body` conversion
- Leaves later native-layout cleanup as a follow-up decision

---

### 3. ControlDirective (Order: 40)

**Converts User Control directives to Blazor component inheritance.**

**Web Forms:**
```html
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MenuBar.ascx.cs" Inherits="MyApp.MenuBar" %>
```

**Output:**
```razor
@inherits MyApp.MenuBar

<!-- component markup -->
```

**What It Does:**
- Extracts `Inherits` attribute → `@inherits`
- Removes boilerplate attributes
- Preserves component markup

---

### 4. RegisterDirective (Order: 50)

**Handles custom control registration.**

**Web Forms:**
```html
<%@ Register Namespace="MyCompany.Controls" Assembly="MyCompany.Web" TagPrefix="my" %>
<my:CustomControl ID="ctrl1" runat="server" />
```

**Output:**
```razor
@* TODO(bwfc-general): Custom control <my:CustomControl> — reference as Blazor component *@
<CustomControl ID="ctrl1" />
```

**What It Does:**
- Removes Register directive (Blazor uses `@using`)
- Flags custom controls with TODO
- Allows developer to map to appropriate Blazor component

---

### 5. ImportDirective (Order: 60)

**Converts Import directives to Blazor usings.**

**Web Forms:**
```html
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="MyApp.Services" %>
```

**Output:**
```razor
@using System.Collections.Generic
@using MyApp.Services
```

**What It Does:**
- Direct conversion to `@using`
- Preserves namespace imports
- Placed at top of component

---

## Markup Transforms

### 6. MasterPageTransform (Order: 250)

**Converts master page markup to the runnable BWFC shell contract.**

**Details:**
- Converts `<asp:ContentPlaceHolder>` blocks to named `<ContentPlaceHolder ID="...">` components
- Strips `runat="server"` from `<head>` and unwraps server-form shells
- Preserves `<head>` content inside BWFC `<Head>`
- Emits `<MasterPage>` with explicit `<ChildContent>` and nested `@ChildContent`
- Adds TODO guidance for shell-specific assets and auth/cart chrome

**Example:**
```html
<!-- Before -->
<head runat="server">
  <title>Site Master</title>
</head>
<form runat="server">
  <asp:ContentPlaceHolder ID="ContentPlaceHolder1" runat="server">
    Default content
  </asp:ContentPlaceHolder>
</form>

<!-- After -->
@* TODO(bwfc-master-page): Review shell scripts, bundle references, and auth/cart chrome for SSR-safe migration. *@

<MasterPage>
  <Head>
    <title>Site Master</title>
  </Head>
  <ChildContent>
    <ContentPlaceHolder ID="ContentPlaceHolder1">Default content</ContentPlaceHolder>
    @ChildContent
  </ChildContent>
</MasterPage>

@code {
  [Parameter]
  public RenderFragment? ChildContent { get; set; }
}
```

---

### 7. ContentWrapperTransform (Order: 300)

**Wraps loose content in a div if necessary.**

**Purpose:** Blazor requires a single root element. Wraps text nodes and mixed content.

---

### 8. FormWrapperTransform (Order: 310)

**Converts Web Forms form tags to Blazor EditForm or plain HTML form.**

**Web Forms:**
```html
<form runat="server">
  <asp:TextBox ID="txtName" runat="server" />
  <asp:Button Text="Submit" OnClick="Submit_Click" runat="server" />
</form>
```

**Output (with EditContext):**
```razor
<EditForm Model="@Model" OnValidSubmit="Submit_Click">
  <DataAnnotationsValidator />
  <TextBox @bind-Value="@Model.Name" />
  <Button Text="Submit" />
</EditForm>
```

---

### 9. ExpressionTransform (Order: 400)

**Converts Web Forms expression syntax to Blazor.**

| Pattern | Web Forms | Blazor |
|---------|-----------|--------|
| Output | `<%: expression %>` | `@(expression)` |
| Code | `<%= expression %>` | `@expression` |
| Data-Bind | `<%# Bind("Property") %>` | `@bind="Model.Property"` |
| Data-Eval | `<%# Eval("Property") %>` | `@Item.Property` |

**Example:**
```html
<!-- Before -->
<h1><%: Model.Title %></h1>
<p><%= GetDescription() %></p>
<input value="<%# Bind("Email") %>" />

<!-- After -->
<h1>@(Model.Title)</h1>
<p>@GetDescription()</p>
<input @bind="Model.Email" />
```

---

### 10. LoginViewTransform (Order: 510)

**Converts LoginView to Blazor AuthorizeView.**

**Web Forms:**
```html
<asp:LoginView runat="server">
  <AnonymousTemplate>
    <p><asp:LoginStatus runat="server" /></p>
  </AnonymousTemplate>
  <LoggedInTemplate>
    <p>Welcome, <asp:LoginName runat="server" />!</p>
  </LoggedInTemplate>
</asp:LoginView>
```

**Output:**
```razor
<AuthorizeView>
  <NotAuthorized>
    <p><LoginStatus /></p>
  </NotAuthorized>
  <Authorized>
    <p>Welcome, <LoginName />!</p>
  </Authorized>
</AuthorizeView>
```

**Complex RoleGroups are flagged with TODO:**
```html
<!-- Input -->
<asp:LoginView runat="server">
  <RoleGroups>
    <asp:RoleGroup Roles="Admin">
      <LoggedInTemplate>Admin panel</LoggedInTemplate>
    </asp:RoleGroup>
  </RoleGroups>
</asp:LoginView>

<!-- Output -->
@* TODO(bwfc-identity): Convert RoleGroups to policy-based AuthorizeView *@
```

---

### 11. SelectMethodTransform (Order: 520)

**Flags data source method attributes for conversion.**

**Purpose:** Detects `SelectMethod`, `InsertMethod`, `UpdateMethod`, `DeleteMethod` attributes and adds TODO comments.

**Example:**
```html
<!-- Input -->
<GridView DataSource='<%# Products %>' AllowPaging="true" runat="server">

<!-- Output -->
@* TODO(bwfc-datasource): Wire Items binding and implement IProductsDataService *@
<GridView Items="@Products" AllowPaging="true">
```

---

### 12. AjaxToolkitPrefixTransform (Order: 610)

**Removes AJAX Control Toolkit prefixes.**

**Web Forms:**
```html
<ajaxToolkit:TabContainer runat="server">
  <ajaxToolkit:TabPanel HeaderText="Tab 1" runat="server">
    Content
  </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
```

**Output:**
```razor
<TabContainer>
  <TabPanel HeaderText="Tab 1">
    Content
  </TabPanel>
</TabContainer>
```

---

### 13. AspPrefixTransform (Order: 620)

**Removes `asp:` prefix from all ASP.NET server controls.**

**Web Forms:**
```html
<div>
  <asp:Button ID="btnSubmit" Text="Submit" CssClass="btn-primary" runat="server" />
  <asp:TextBox ID="txtName" placeholder="Enter name" runat="server" />
  <asp:Label ID="lblStatus" runat="server" />
</div>
```

**Output:**
```razor
<div>
  <Button ID="btnSubmit" Text="Submit" CssClass="btn-primary" />
  <TextBox ID="txtName" placeholder="Enter name" />
  <Label ID="lblStatus" />
</div>
```

---

### 14. AttributeStripTransform (Order: 700)

**Removes Web Forms-specific attributes.**

Removes:
- `runat="server"`
- `AutoEventWireup="true|false"`
- `EnableEventValidation="true|false"`
- `ViewStateMode="Enabled|Disabled|Inherit"`

**Example:**
```html
<!-- Before -->
<asp:TextBox ID="txt1" runat="server" AutoEventWireup="true" ViewStateMode="Disabled" />

<!-- After -->
<TextBox ID="txt1" />
```

---

### 15. EventWiringTransform (Order: 750)

**Converts Web Forms event handler syntax to Blazor.**

**Web Forms:**
```html
<asp:Button ID="btnSubmit" Text="Submit" OnClick="Submit_Click" runat="server" />
<asp:TextBox ID="txtEmail" OnTextChanged="Email_Changed" AutoPostBack="true" runat="server" />
```

**Output:**
```razor
<Button ID="btnSubmit" Text="Submit" OnClick="@Submit_Click" />
<TextBox ID="txtEmail" OnInput="@Email_Changed" />
```

---

### 16. UrlReferenceTransform (Order: 780)

**Converts ASP.NET virtual paths to absolute URLs.**

Handles 8 URL attributes:

| Attribute | Example |
|-----------|---------|
| `NavigateUrl` | `~/products/list` → `/products/list` |
| `ImageUrl` | `~/images/logo.png` → `/images/logo.png` |
| `PostBackUrl` | `~/checkout` → `/checkout` |
| `ToolTip` | `~/help` in URL context → `/help` |
| `HRef` | `~/page` → `/page` |
| `Src` | `~/scripts/app.js` → `/scripts/app.js` |
| `DataSourceID` (partial) | Handled by DataSourceIdTransform |
| `onclick` (URLs) | JavaScript URLs updated |

**Example:**
```html
<!-- Before -->
<asp:HyperLink NavigateUrl="~/products/list" Text="Products" runat="server" />
<asp:Image ImageUrl="~/images/logo.png" runat="server" />
<script src="~/scripts/app.js"></script>

<!-- After -->
<HyperLink NavigateUrl="/products/list" Text="Products" />
<Image ImageUrl="/images/logo.png" />
<script src="/scripts/app.js"></script>
```

---

### 17. TemplatePlaceholderTransform (Order: 800)

**Converts `Item` placeholder to `context` in Blazor templates.**

Blazor uses `context` to reference the template context variable. Web Forms uses `Item`.

**Example:**
```html
<!-- Before (GridView template) -->
<ItemTemplate>
  <td><%# Item.Name %></td>
  <td><%# Item.Price %></td>
</ItemTemplate>

<!-- After -->
<ItemTemplate>
  <td>@context.Name</td>
  <td>@context.Price</td>
</ItemTemplate>
```

---

### 18. AttributeNormalizeTransform (Order: 810)

**Normalizes attribute values to Blazor syntax.**

Converts:
- `Visible="true"` → no attribute (show by default)
- `Visible="false"` → `style="display:none"`
- Boolean `true/false` → `true`/`false`
- Enum strings → proper C# enum syntax

**Example:**
```html
<!-- Before -->
<asp:Panel Visible="<%# ShowPanel %>" BackColor="Red" ForeColor="White" runat="server">

<!-- After -->
<Panel style="@(ShowPanel ? "" : "display:none")" style="background-color: red; color: white;">
```

---

### 19. DataSourceIdTransform (Order: 820)

**Replaces DataSourceID with Items binding and scaffolds data properties.**

**Web Forms:**
```html
<asp:GridView DataSourceID="SqlDataSource1" runat="server" />
<asp:SqlDataSource ID="SqlDataSource1" SelectCommand="SELECT * FROM Products" runat="server" />
```

**Output:**
```razor
@* TODO(bwfc-datasource): Implement IProductsDataService to replace SqlDataSource *@
<GridView Items="@ProductsData" />

@code {
    private List<Product> ProductsData { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // TODO(bwfc-datasource): Load ProductsData from service
    }
}
```

---

## Code-Behind Transforms

### 20. TodoHeaderTransform (Order: 10)

**Injects migration guidance header at the top of code-behind files.**

**Output:** (injected at file top)
```csharp
// =============================================================================
// TODO(bwfc-general): This code-behind was copied from Web Forms and needs manual migration.
//
// Common transforms needed (use the BWFC Copilot skill for assistance):
//   TODO(bwfc-lifecycle): Page_Load / Page_Init → OnInitializedAsync / OnParametersSetAsync
//   TODO(bwfc-lifecycle): Page_PreRender → OnAfterRenderAsync
//   TODO(bwfc-ispostback): IsPostBack checks → remove or convert to state logic
//   TODO(bwfc-viewstate): ViewState usage → component [Parameter] or private fields
//   TODO(bwfc-session-state): Session/Cache access → inject IHttpContextAccessor or use DI
//   TODO(bwfc-navigation): Response.Redirect → NavigationManager.NavigateTo
//   TODO(bwfc-general): Event handlers (Button_Click, etc.) → convert to Blazor event callbacks
//   TODO(bwfc-datasource): Data binding (DataBind, DataSource) → component parameters or OnInitialized
//   TODO(bwfc-general): ScriptManager code-behind references → remove (Blazor handles updates)
//   TODO(bwfc-general): UpdatePanel markup preserved by BWFC (ContentTemplate supported) — remove only code-behind API calls
//   TODO(bwfc-general): User controls → Blazor component references
// =============================================================================
```

---

### 21. UsingStripTransform (Order: 20)

**Removes Web Forms and ASP.NET using statements.**

**Removes:**
- `System.Web.*`
- `System.Web.UI.*`
- `Microsoft.AspNet.*`
- AJAX Control Toolkit namespaces

**Example:**
```csharp
// Before
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;

// After
using System;
// (Web Forms usings removed — use BWFC and ASP.NET Core namespaces)
```

---

### 22. BaseClassStripTransform (Order: 30)

**Removes Web Forms base classes and replaces with Blazor ComponentBase.**

**Example:**
```csharp
// Before
public partial class Product : System.Web.UI.Page
{
    // ...
}

// After
public partial class Product : ComponentBase
{
    // ...
}
```

---

### 23. ResponseRedirectTransform (Order: 25)

**Converts Response.Redirect() to NavigationManager.NavigateTo().**

**Web Forms:**
```csharp
private void CheckoutButton_Click(object sender, EventArgs e)
{
    Response.Redirect("~/checkout");
}
```

**Output:**
```csharp
private void CheckoutButton_Click(object sender, EventArgs e)
{
    NavigationManager.NavigateTo("/checkout");
}
```

---

### 24. GetRouteUrlTransform (Order: 30/50)

**Flags Page.GetRouteUrl() calls for conversion guidance.**

**Example:**
```csharp
// Before
string url = Page.GetRouteUrl("ProductRoute", new { id = 123 });

// After
@* TODO(bwfc-routing): Page.GetRouteUrl() → Use NavigationManager.GetUriByPage() or Router.TryResolveRoute() *@
string url = Page.GetRouteUrl("ProductRoute", new { id = 123 });
```

---

### 25. SessionDetectTransform (Order: 400)

**Detects Session/Cache usage and auto-wires SessionShim/CacheShim.**

**Web Forms:**
```csharp
public class CheckoutPage : Page
{
    private void LoadCart()
    {
        string cartId = (string)Session["CartId"];
        var items = (List<CartItem>)Cache["ProductList"];
    }
}
```

**Output:**
```csharp
// --- Session State Migration ---
// TODO(bwfc-session-state): SessionShim auto-wired via [Inject] — Session["CartId"] calls compile against the shim's indexer.
// Session keys found: CartId
// Options for long-term replacement:
//   (1) ProtectedSessionStorage (Blazor Server) — persists across circuits
//   (2) Scoped service via DI — lifetime matches user circuit
//   (3) Cascading parameter from a root-level state provider
// See: https://learn.microsoft.com/aspnet/core/blazor/state-management

// --- Cache Migration ---
// TODO(bwfc-session-state): CacheShim auto-wired via [Inject] — Cache["ProductList"] calls compile against the shim's indexer.
// Cache keys found: ProductList
// CacheShim wraps IMemoryCache — items are per-server, not distributed.
// For distributed caching, consider IDistributedCache.

public class CheckoutPage : ComponentBase
{
    [Inject] private SessionShim Session { get; set; }
    [Inject] private CacheShim Cache { get; set; }

    private void LoadCart()
    {
        string cartId = (string)Session["CartId"];
        var items = (List<CartItem>)Cache["ProductList"];
    }
}
```

---

### 26. ViewStateDetectTransform (Order: 410)

**Detects ViewState usage and flags migration strategy.**

**Example:**
```csharp
// Before
ViewState["CurrentPage"] = 1;
int page = (int)(ViewState["CurrentPage"] ?? 0);

// After
@* TODO(bwfc-viewstate): ViewState["CurrentPage"] — migrate to component private field or [Parameter] *@
// Options:
//   (1) Private field (simple state within component):
//       private int CurrentPage { get; set; } = 1;
//   (2) [Parameter] (state passed from parent):
//       [Parameter] public int CurrentPage { get; set; }
//   (3) Cascading parameter (shared across hierarchy):
//       [CascadingParameter] private int CurrentPage { get; set; }
ViewState["CurrentPage"] = 1;
int page = (int)(ViewState["CurrentPage"] ?? 0);
```

---

### 27. IsPostBackTransform (Order: 500)

**Unwraps IsPostBack guards and extracts postback logic.**

**Web Forms:**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        LoadInitialData();
    }
    else
    {
        HandlePostBack();
    }
}
```

**Output:**
```csharp
protected async Task OnInitializedAsync()
{
    LoadInitialData();  // Guard unwrapped — runs on first load only
}

private void HandlePostBack()
{
    // Postback logic extracted
}
```

---

### 28. PageLifecycleTransform (Order: 510)

**Converts Web Forms lifecycle methods to Blazor.**

| Web Forms | Blazor |
|-----------|--------|
| `Page_Load` | `OnInitializedAsync` |
| `Page_Init` | `OnInitializedAsync` / Constructor |
| `Page_PreRender` | `OnAfterRenderAsync` |
| `Page_Unload` | `Dispose` |

**Example:**
```csharp
// Before
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
        LoadProducts();
}

protected void Page_PreRender(object sender, EventArgs e)
{
    UpdateStatus();
}

// After
protected override async Task OnInitializedAsync()
{
    await LoadProducts();
}

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
        UpdateStatus();
}
```

---

### 29. EventHandlerSignatureTransform (Order: 520)

**Adapts Web Forms event handler signatures to Blazor.**

**Web Forms:**
```csharp
protected void SubmitButton_Click(object sender, EventArgs e)
{
    // handle click
}

protected void GridView_RowCommand(object sender, GridViewCommandEventArgs e)
{
    // handle row command
}
```

**Output:**
```csharp
// Event callback wired in markup: OnClick="@SubmitButton_Click"
private Task SubmitButton_Click()
{
    // handle click (no sender/e parameters)
    return Task.CompletedTask;
}

// For GridView row commands: OnRowCommand="@GridView_RowCommand"
private Task GridView_RowCommand(GridViewCommandEventArgs e)
{
    // e is preserved for complex data grid scenarios
    return Task.CompletedTask;
}
```

---

### 30. DataBindTransform (Order: 40)

**Flags DataBind() calls for conversion.**

**Example:**
```csharp
// Before
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        ProductGrid.DataBind();
    }
}

// After
@* TODO(bwfc-datasource): DataBind() → Remove (Blazor renders via @binding) or use StateHasChanged() after data load *@
protected override async Task OnInitializedAsync()
{
    // Load data and StateHasChanged() will re-render
    Products = await LoadProductsAsync();
}
```

---

### 31. UrlCleanupTransform (Order: 50)

**Cleans URL string literals in code.**

**Example:**
```csharp
// Before
string redirectUrl = "~/products/list";
string imageUrl = "~/images/logo.png";

// After
string redirectUrl = "/products/list";
string imageUrl = "/images/logo.png";
```

---

## How to Use This Reference

1. **During migration:** Open this page alongside your migrated code to understand what changed
2. **After migration:** Use "Transform Reference" links in TODO comments to jump to specific transform details
3. **For troubleshooting:** If output doesn't match expectations, check the transform order — earlier transforms affect later ones

## Next Steps

- **[TODO Categories](todo-conventions.md)** — Learn about TODO comment categories for L2 automation
- **[Migration Report](report.md)** — Understand how to read the migration report
- **[Back to CLI Overview](index.md)** — Return to main CLI documentation
