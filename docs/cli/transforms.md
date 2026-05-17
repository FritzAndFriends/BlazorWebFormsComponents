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
| 255 | ScriptManagerStripTransform | Markup | Markup | Remove `ScriptManager`, bundle references, and `Scripts.Render(...)` placeholders from master pages |
| 300 | ContentWrapperTransform | Markup | Markup | Wrap loose content in `<div>` if needed |
| 310 | FormWrapperTransform | Markup | Markup | Convert `<form runat="server">` to Blazor form |
| 490 | DisplayExpressionTransform | Markup | Markup | Normalize `<%#:` / `<%=:` display expressions and broken `@(: expr)` output to valid Razor |
| 500 | ExpressionTransform | Markup | Markup | Convert `<%: %>`, `<%= %>`, and inline data expressions to Razor |
| 510 | ServerCodeBlockTransform | Markup | Markup | Convert `<% ... %>` statement blocks to Razor control flow |
| 510 | LoginViewTransform | Markup | Markup | Convert `<asp:LoginView>` → `<AuthorizeView>` |
| 520 | SelectMethodTransform | Markup | Markup | Preserve SelectMethod / InsertMethod / UpdateMethod / DeleteMethod model-binding attributes |
| 550 | WebMethodAnnotationTransform | Code-Behind | Code-Behind | Flag legacy static WebMethod endpoints and normalize `Page_PreRenderComplete` for lifecycle conversion |
| 600 | AjaxToolkitPrefixTransform | Markup | Markup | Remove `ajaxToolkit:` prefixes |
| 610 | AspPrefixTransform | Markup | Markup | Remove `asp:` prefixes from controls |
| 615 | DataBindingAttributeTransform | Markup | Markup | Convert `<%# ... %>` and `<%= ... %>` attribute values to `@(...)` |
| 615 | ValidatorGenericTypeTransform | Markup | Markup | Add explicit `Type="string"` / `InputType="string"` defaults for generic BWFC validators |
| 620 | TemplateFieldChildComponentsTransform | Markup | Markup | Wrap TemplateField style children in `<ChildComponents>` |
| 700 | AttributeStripTransform | Markup | Markup | Remove `runat="server"`, preserve BWFC `ItemType`, add generic fallbacks |
| 705 | GridViewColumnItemTypeTransform | Markup | Markup | Propagate typed `GridView ItemType` values to `BoundField` / `TemplateField` / `ButtonField` / `CommandField` child columns |
| 710 | EventWiringTransform | Markup | Markup | Convert `OnClick="X"` → `OnClick="@X"` |
| 720 | UrlReferenceTransform | Markup | Markup | Convert `~/` paths to `/` |
| 750 | ComponentRefMarkupTransform | Markup | Markup | Convert control IDs to `@ref`-compatible component references |
| 800 | TemplatePlaceholderTransform | Markup | Markup | Replace template placeholder elements with the layout/group render fragment context |
| 805 | TemplateContextTransform | Markup | Markup | Add explicit item/group/layout template contexts for typed data-control fragments |
| 810 | AttributeNormalizeTransform | Markup | Markup | Normalize attribute values (booleans, enums, units) |
| 820 | DataSourceIdTransform | Markup | Markup | Replace DataSourceID with Items binding |
| 30 | GetRouteUrlTransform | Code-Behind | Code-Behind | Flag `Page.GetRouteUrl()` calls |
| 50 | GetRouteUrlTransform | Markup | Markup | Flag `<%: Page.GetRouteUrl() %>` expressions |
| 390 | CartSessionKeyTransform | Code-Behind | Code-Behind | Replace cart/basket `Session.Id` usage with a stable session-backed `cart-key` helper |
| 400 | SessionDetectTransform | Code-Behind | Code-Behind | Detect Session/Cache usage and emit shim guidance |
| 410 | ViewStateDetectTransform | Code-Behind | Code-Behind | Detect ViewState usage, flag migration |
| 500 | IsPostBackTransform | Code-Behind | Code-Behind | Unwrap `if (!IsPostBack)` guards |
| 510 | PageLifecycleTransform | Code-Behind | Code-Behind | Convert Page_Load, Page_Init → Blazor lifecycle |
| 520 | EventHandlerSignatureTransform | Code-Behind | Code-Behind | Adapt event handler signatures |
| 30 | BaseClassStripTransform | Code-Behind | Code-Behind | Remove `System.Web.UI.Page` base class |
| 20 | UsingStripTransform | Code-Behind | Code-Behind | Remove Web Forms and ASP.NET using statements |
| 25 | ResponseRedirectTransform | Code-Behind | Code-Behind | Convert `Response.Redirect()` → `NavigationManager.NavigateTo()` |
| 40 | DataBindTransform | Code-Behind | Code-Behind | Flag `DataBind()` calls |
| 50 | UrlCleanupTransform | Code-Behind | Code-Behind | Clean URL literals in code |
| 104 | HttpUtilityRewriteTransform | Code-Behind | Code-Behind | Rewrite `HttpUtility.*` calls to `WebUtility.*` and add `using System.Net;` |
| 106 | EfContextConstructorTransform | Code-Behind | Code-Behind | Rewrite EF6 `base("name")` DbContext constructors to EF Core `DbContextOptions<TContext>` constructors |
| 107 | DbContextInstantiationTransform | Code-Behind | Code-Behind | Replace inline `new XxxContext()` usage with injected DbContext references |
| 108 | SelectMethodMaterializeTransform | Code-Behind | Code-Behind | Materialize IQueryable SelectMethod results before methods exit after `CreateDbContext()` usage |
| 850 | CompileSurfaceStubTransform | Code-Behind | Code-Behind | Quarantine identity/payment/mobile/admin/compile-blocked pages with build-safe stubs while preserving transformed originals in `migration-artifacts\codebehind\` and tracking them in `migration-artifacts\quarantine-manifest.json` |
| 900 | MarkupReferencedMemberStubTransform | Code-Behind | Code-Behind | Add fallback fields, render-method stubs, and event handlers for markup references still missing from emitted partial classes |

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
- Infers the primary route from the source-relative `.aspx` path when the page lives under a subdirectory (for example `Account/Login.aspx` → `@page "/Account/Login"`)
- Adds a secondary filename-only alias route when that alias differs from the source-relative route (for example `@page "/Login"`)
- Preserves the existing `/` home-page handling for `Default.aspx` and `Index.aspx`
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

### 7. ScriptManagerStripTransform (Order: 255)

**Removes Web Forms-only script infrastructure from master pages.**

**Details:**
- Replaces `<asp:ScriptManager>...</asp:ScriptManager>` blocks with `@* Framework scripts are managed by Blazor — no ScriptManager needed. *@`
- Removes `<webopt:bundlereference ... />` tags
- Removes `<asp:PlaceHolder runat="server">` wrappers that only contain `Scripts.Render(...)`
- Collapses excess blank lines left behind by the cleanup

---

### 8. ContentWrapperTransform (Order: 300)

**Wraps loose content in a div if necessary.**

**Purpose:** Blazor requires a single root element. Wraps text nodes and mixed content.

---

### 9. FormWrapperTransform (Order: 310)

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

### 10. DisplayExpressionTransform (Order: 490)

**Normalizes Web Forms display-expression syntax before broader expression handling.**

**Details:**
- Converts `<%#: expr %>` to idiomatic Razor output
- Leaves `Bind(...)` and `Eval(...)` for later specialized handling
- Converts complex expressions such as `String.Format(...)` to parenthesized Razor `@( ... )`
- Repairs broken generated `@(: expr)` output

---

### 11. ExpressionTransform (Order: 500)

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

### 12. LoginViewTransform (Order: 510)

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

### 13. SelectMethodTransform (Order: 520)

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

### 13. AspPrefixTransform (Order: 610)

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

### 14. DataBindingAttributeTransform (Order: 615)

**Converts Web Forms data-binding expressions that appear inside attribute values.**

**Web Forms:**
```html
<asp:HyperLink NavigateUrl='<%# Item.GetUrl() %>' Text='<%# Item.Name %>' />
```

**Output:**
```razor
<HyperLink NavigateUrl='@(Item.GetUrl())' Text='@(Item.Name)' />
```

**What It Does:**
- Targets attribute values that still contain `<%# ... %>` or `<%= ... %>` after the core expression pass
- Preserves surrounding attributes while replacing the value with a Razor `@(...)` expression
- Switches quote style when necessary so embedded C# string literals remain valid Razor

---

### 15. AttributeStripTransform (Order: 700)

**Removes Web Forms-specific attributes and preserves BWFC generic typing.**

Removes:
- `runat="server"`
- `AutoEventWireup="true|false"`
- `EnableViewState="true|false"`
- `ViewStateMode="Enabled|Disabled|Inherit"`
- `ClientIDMode="..."`

Also:
- normalizes `ItemType="Namespace.Product"` → `ItemType="Product"`
- adds `ItemType="object"` to generic BWFC controls when no type was present
- converts `ID="..."` → `id="..."`

**Example:**
```html
<!-- Before -->
<asp:GridView ID="ProductsGrid" ItemType="Contoso.Models.Product" runat="server" />

<!-- After -->
<GridView id="ProductsGrid" ItemType="Product" />
```

---

### 15a. GridViewColumnItemTypeTransform (Order: 705)

**Propagates a typed `GridView` item type down into child BWFC column components.**

When a migrated `GridView` is strongly typed, `TemplateField`, `BoundField`, `HyperLinkField`, and `ButtonField` children must compile against that same row type. This transform rewrites `ItemType="object"` fallbacks on those column tags to the parent grid's concrete `ItemType`, while leaving object-typed grids alone.

**Example:**
```html
<!-- Before -->
<GridView ItemType="CartItem" AutoGenerateColumns="false">
  <Columns>
    <BoundField ItemType="object" DataField="ProductID" />
    <TemplateField ItemType="object" HeaderText="Quantity">
      <ItemTemplate Context="Item">
        <TextBox Text="@Item.Quantity" />
      </ItemTemplate>
    </TemplateField>
  </Columns>
</GridView>

<!-- After -->
<GridView ItemType="CartItem" AutoGenerateColumns="false">
  <Columns>
    <BoundField ItemType="CartItem" DataField="ProductID" />
    <TemplateField ItemType="CartItem" HeaderText="Quantity">
      <ItemTemplate Context="Item">
        <TextBox Text="@Item.Quantity" />
      </ItemTemplate>
    </TemplateField>
  </Columns>
</GridView>
```

---

### 16. EventWiringTransform (Order: 710)

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

### 17. UrlReferenceTransform (Order: 720)

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

### 18. TemplatePlaceholderTransform (Order: 800)

**Replaces Web Forms placeholder elements with the Blazor render-fragment placeholder.**

This transform targets placeholder tags such as `itemPlaceholder`, `groupPlaceholder`, and similar generated template markers so BWFC `LayoutTemplate` and `GroupTemplate` blocks render the nested fragment correctly.

**Example:**
```html
<!-- Before -->
<LayoutTemplate>
  <table>
    <tr id="itemPlaceholder" runat="server"></tr>
  </table>
</LayoutTemplate>

<!-- After -->
<LayoutTemplate>
  <table>
    @context
  </table>
</LayoutTemplate>
```

---

### 19. TemplateContextTransform (Order: 805)

**Adds explicit contexts for typed item templates plus named placeholder contexts for `ListView` group/layout fragments.**

This keeps BWFC data controls aligned with Web Forms naming so generated `<ListView>`, `<FormView>`, `<GridView>`, `<DataList>`, and `<Repeater>` templates stay on the component model instead of being flattened into manual HTML. It also upgrades placeholder-only `GroupTemplate` / `LayoutTemplate` blocks to named contexts such as `items` and `groups`, which makes emitted `ListView` markup structurally valid and easier to read.

**Example:**
```html
<!-- Before -->
<ItemTemplate>
  <TextBox Text="@context.Name" />
</ItemTemplate>
<GroupTemplate>
  <tr>@context</tr>
</GroupTemplate>

<!-- After -->
<ItemTemplate Context="Item">
  <TextBox Text="@Item.Name" />
</ItemTemplate>
<GroupTemplate Context="items">
  <tr>@items</tr>
</GroupTemplate>
```

---

### 20. AttributeNormalizeTransform (Order: 810)

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

### 21. DataSourceIdTransform (Order: 820)

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

### CartSessionKeyTransform (Order: 390)

**Rewrites cart and basket flows so they use a stable session-backed `cart-key` instead of the raw session identifier.**

**Before:**
```csharp
var cart = _cartService.GetCart(Session.Id);
```

**After:**
```csharp
private string GetOrCreateCartKey()
{
    var cartKey = Session["cart-key"]?.ToString();
    if (string.IsNullOrEmpty(cartKey))
    {
        cartKey = Guid.NewGuid().ToString();
        Session["cart-key"] = cartKey;
    }

    return cartKey;
}

var cart = _cartService.GetCart(GetOrCreateCartKey());
```

**What It Does:**
- Matches cart/basket-focused statements that still use `Session.Id` or `HttpContext.Session.Id`
- Injects a single `GetOrCreateCartKey()` helper into the generated partial class
- Keeps the migration on top of BWFC's existing `SessionShim` surface (`Session["cart-key"]`)
- Leaves unrelated `Session.Id` usage alone to avoid over-matching

---

### 25. SessionDetectTransform (Order: 400)

**Detects Session/Cache usage and emits shim guidance for `WebFormsPageBase`.**

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
// TODO(bwfc-session-state): Session["CartId"] calls work automatically via SessionShim on WebFormsPageBase.
// Session keys found: CartId
// Options for long-term replacement:
//   (1) ProtectedSessionStorage (Blazor Server) — persists across circuits
//   (2) Scoped service via DI — lifetime matches user circuit
//   (3) Cascading parameter from a root-level state provider
// See: https://learn.microsoft.com/aspnet/core/blazor/state-management

// --- Cache Migration ---
// TODO(bwfc-session-state): Cache["ProductList"] calls work automatically via CacheShim on WebFormsPageBase.
// Cache keys found: ProductList
// CacheShim wraps IMemoryCache — items are per-server, not distributed.
// For distributed caching, consider IDistributedCache.

public class CheckoutPage : WebFormsPageBase
{
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

### 32. SelectMethodMaterializeTransform (Order: 108)

**Materializes `IQueryable` SelectMethod results before the scoped DbContext goes out of scope.**

**Before:**
```csharp
public IQueryable<Product> GetProducts(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
{
    using var db = DbFactory.CreateDbContext();
    var query = db.Products.OrderBy(p => p.ProductName);
    totalRowCount = query.Count();
    return query;
}
```

**After:**
```csharp
public IQueryable<Product> GetProducts(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
{
    var db = DbFactory.CreateDbContext();
    var query = db.Products.OrderBy(p => p.ProductName);
    totalRowCount = query.Count();
    return query.ToList().AsQueryable();
}
```

**Details:**
- Only applies to methods whose signature returns `IQueryable<...>`
- Removes `using` from `using var db = ...CreateDbContext()` so the query can be materialized before the method exits
- Rewrites simple `return query;` patterns to `return query.ToList().AsQueryable();`
- Leaves non-`IQueryable` methods untouched

---

### 33. CompileSurfaceStubTransform (Order: 850)

**Quarantines non-migratable pages behind build-safe placeholders.**

**Details:**
- Detects pages that still depend on ASP.NET Identity or membership APIs, payment providers, mobile-only shells, complex admin CRUD with 3+ data-source bindings, or unresolved compile-surface blockers after transforms run
- Keeps essential product, catalog, cart, home, contact, and about page paths out of quarantine even when they pick up minor incidental signals
- Requires either multiple signals, a clearly non-essential path (such as `Account/`, `Admin/`, or `Checkout/`), or a strong blocker before quarantine is applied
- Replaces the generated markup with a visible "Page Not Yet Migrated" placeholder that still routes and compiles
- Emits a minimal `WebFormsPageBase` partial class stub so the generated app still builds
- Preserves the transformed original code-behind under `migration-artifacts\codebehind\`, records a `bwfc-compile-surface` manual item, and adds the page to `migration-artifacts\quarantine-manifest.json`

---

### 34. MarkupReferencedMemberStubTransform (Order: 900)

**Adds compile-safe fallback members for identifiers that remain referenced from markup.**

**Details:**
- Scans the transformed markup for missing `@Method()`, `@_field`, and event-handler references
- Appends minimal stubs only when the emitted partial class does not already declare the member
- Keeps the compile surface moving while leaving explicit TODO comments for the real migration work

---

## How to Use This Reference

1. **During migration:** Open this page alongside your migrated code to understand what changed
2. **After migration:** Use "Transform Reference" links in TODO comments to jump to specific transform details
3. **For troubleshooting:** If output doesn't match expectations, check the transform order — earlier transforms affect later ones

## Next Steps

- **[TODO Categories](todo-conventions.md)** — Learn about TODO comment categories for L2 automation
- **[Migration Report](report.md)** — Understand how to read the migration report
- **[Back to CLI Overview](index.md)** — Return to main CLI documentation
