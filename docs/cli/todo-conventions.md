# TODO Categories & L2 Automation

The CLI tool inserts standardized TODO comments throughout your code. Each TODO has a category slug in the format `TODO(bwfc-category)`. Copilot L2 skills use these slugs to automatically locate and convert patterns.

## Category Reference

### 1. **bwfc-general**
**Scope:** Miscellaneous Web Forms patterns without a more specific category  
**Usage:** Catch-all for general migration guidance  
**Example:**
```csharp
// TODO(bwfc-general): Event handlers (Button_Click, etc.) → convert to Blazor event callbacks
protected void SubmitButton_Click(object sender, EventArgs e)
{
    // handle click
}
```

**L2 Automation:**
- Convert event handler signatures from `(object, EventArgs)` to parameterless or typed callbacks
- Add `EventCallback<T>` bindings where appropriate

---

### 2. **bwfc-lifecycle**
**Scope:** Page lifecycle methods and initialization patterns  
**Usage:** Marks `Page_Load`, `Page_Init`, `Page_PreRender`, etc.  
**Example:**
```csharp
// TODO(bwfc-lifecycle): Page_Load / Page_Init → OnInitializedAsync / OnParametersSetAsync
protected void Page_Load(object sender, EventArgs e)
{
    LoadProductList();
}

// TODO(bwfc-lifecycle): Page_PreRender → OnAfterRenderAsync
protected void Page_PreRender(object sender, EventArgs e)
{
    UpdateStatus();
}
```

**L2 Automation:**
- Convert `Page_Load` to `OnInitializedAsync`
- Convert `Page_Init` to component constructor or `OnInitializedAsync`
- Convert `Page_PreRender` to `OnAfterRenderAsync`
- Wrap async work in `try/catch` for error handling
- Add `StateHasChanged()` calls where needed

---

### 3. **bwfc-ispostback**
**Scope:** IsPostBack detection and guard patterns  
**Usage:** Marks `if (!IsPostBack)` or `if (IsPostBack == false)` guards  
**Example:**
```csharp
// TODO(bwfc-ispostback): IsPostBack guard — review for Blazor
if (!IsPostBack)
{
    LoadInitialData();
}
```

**L2 Automation:**
- Remove unnecessary `IsPostBack` checks (Blazor components initialize once)
- Extract postback-only logic into separate methods
- Convert event-driven patterns to component state management

---

### 4. **bwfc-viewstate**
**Scope:** ViewState dictionary access  
**Usage:** Marks `ViewState["key"]` patterns  
**Example:**
```csharp
// TODO(bwfc-viewstate): ViewState usage → component [Parameter] or private fields
ViewState["CurrentPage"] = 1;
int page = (int)(ViewState["CurrentPage"] ?? 0);
```

**L2 Automation:**
- Replace `ViewState["key"]` with private component fields
- Convert persisted state to component `[Parameter]` or cascading parameters
- For complex state, suggest Scoped services or ProtectedSessionStorage

---

### 5. **bwfc-session-state**
**Scope:** Session and Cache dictionary access  
**Usage:** Marks `Session["key"]` and `Cache["key"]` patterns  
**Example:**
```csharp
// --- Session State Migration ---
// TODO(bwfc-session-state): SessionShim auto-wired via [Inject] — Session["CartId"] calls compile against the shim's indexer.
// Session keys found: CartId
// Options:
//   (1) ProtectedSessionStorage
//   (2) Scoped service via DI
//   (3) Cascading parameter from root-level state provider
string cartId = Session["CartId"];
```

**L2 Automation:**
- Map identified Session keys to scoped services
- Create session state provider classes with typed properties
- Wire up ProtectedSessionStorage for critical session data
- Add guidance on distributed caching alternatives for Cache patterns

---

### 6. **bwfc-navigation**
**Scope:** Navigation and redirection  
**Usage:** Marks `Response.Redirect()`, `Server.Transfer()`, URL patterns  
**Example:**
```csharp
// TODO(bwfc-navigation): Response.Redirect → NavigationManager.NavigateTo
Response.Redirect("~/checkout");
```

**L2 Automation:**
- Convert `Response.Redirect()` to `NavigationManager.NavigateTo()`
- Convert `Server.Transfer()` to component navigation or redirect
- Clean up `~/` paths to absolute routes

---

### 7. **bwfc-datasource**
**Scope:** Data binding and data source controls  
**Usage:** Marks `DataBind()`, `DataSourceID`, `DataSource` properties, and data source controls  
**Example:**
```csharp
// TODO(bwfc-datasource): Data binding (DataBind, DataSource) → component parameters or OnInitialized
// SQL/LINQ data source patterns → implement IDataService and wire Items binding
ProductGrid.DataBind();

// TODO(bwfc-datasource): Implement IProductsDataService to replace SqlDataSource
<GridView Items="@ProductsData" />
```

**L2 Automation:**
- Remove `DataBind()` calls (Blazor re-renders automatically)
- Create `IProductsDataService` or similar DI service
- Replace `DataSourceID` with `Items="@ProductList"` binding
- Add `OnInitializedAsync` data loading logic

---

### 8. **bwfc-identity**
**Scope:** Identity, authentication, and role-based access  
**Usage:** Marks `LoginView`, `RoleGroups`, `Roles` attributes  
**Example:**
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
<AuthorizeView Policy="IsAdmin">
  <Authorized>Admin panel</Authorized>
</AuthorizeView>
```

**L2 Automation:**
- Map role names to authorization policies
- Create `AuthorizationHandler<T>` implementations for custom policies
- Generate policy registration code in `Program.cs`

---

### 9. **bwfc-master-page**
**Scope:** Master page structure and layout  
**Usage:** Marks master page conversions and head content extraction  
**Example:**
```razor
@inherits LayoutComponentBase
@* TODO(bwfc-master-page): Review head content extraction for App.razor *@
<head>
  <title>@PageTitle</title>
</head>
<div class="container">
  @Body
</div>
```

**L2 Automation:**
- Extract head content (meta tags, stylesheets) to `App.razor`
- Verify `@Body` placement in layout
- Check for complex script/style blocks requiring special handling

---

### 10. **bwfc-routing**
**Scope:** URL routing and page routes  
**Usage:** Marks `GetRouteUrl()`, route attribute patterns  
**Example:**
```csharp
// TODO(bwfc-routing): Page.GetRouteUrl() → Use NavigationManager.GetUriByPage() or Router.TryResolveRoute()
string url = Page.GetRouteUrl("ProductRoute", new { id = 123 });
```

**L2 Automation:**
- Replace `GetRouteUrl()` with `NavigationManager` routes
- Create strongly-typed route builders if needed
- Verify `@page` directives match Web Forms routing

---

### 11. **bwfc-validation**
**Scope:** ASP.NET validation controls and patterns  
**Usage:** Marks `RequiredFieldValidator`, `RegularExpressionValidator`, etc.  
**Example:**
```html
@* TODO(bwfc-validation): ASP.NET validators → DataAnnotations + <ValidationMessage> *@
<RequiredFieldValidator ControlToValidate="txtEmail" runat="server" />
```

**L2 Automation:**
- Convert validator declarations to `[Required]`, `[RegularExpression]`, etc. attributes
- Generate `<ValidationMessage For="@(() => Model.Email)" />` for display
- Add `<DataAnnotationsValidator />` to form

---

### 12. **bwfc-ajax**
**Scope:** ASP.NET AJAX UpdatePanel, ScriptManager, AJAX extenders  
**Usage:** Marks UpdatePanel/ScriptManager patterns  
**Example:**
```html
@* TODO(bwfc-ajax): UpdatePanel preserved as markup — remove code-behind UpdatePanel API calls; use StateHasChanged() instead *@
<UpdatePanel>
  <ContentTemplate>
    <GridView Items="@Products" />
  </ContentTemplate>
</UpdatePanel>
```

**L2 Automation:**
- Remove `UpdatePanel.Update()` calls (replace with `StateHasChanged()`)
- Remove `ScriptManager` references and script injection code
- Convert AJAX extenders to Blazor components (tooltips, modals, etc.)

---

### 13. **bwfc-custom-control**
**Scope:** Custom Web Forms controls and user control conversion  
**Usage:** Marks unrecognized or custom control conversions  
**Example:**
```html
@* TODO(bwfc-custom-control): Custom control <my:ProductCard> — map to Blazor component *@
<ProductCard ProductId="@SelectedProductId" />
```

**L2 Automation:**
- Scan for custom control declarations in Register directives
- Create Blazor component wrapper classes
- Migrate custom control markup and code-behind to Razor components

---

## How L2 Automation Uses Categories

**Copilot L2 skills scan your migrated code for TODO categories and:**

1. **Group by category** — Find all `bwfc-datasource` patterns, all `bwfc-lifecycle` patterns, etc.
2. **Analyze context** — Use line numbers and surrounding code to understand each pattern
3. **Generate transforms** — Create targeted code changes for each category
4. **Apply iteratively** — Run multiple L2 passes for complex migrations (lifecycle → datasource → validation)

**Example Workflow:**

```bash
# After CLI migration
webforms-to-blazor migrate --input MyApp.Web --output MyApp.Blazor

# Review TODO categories in generated code
grep -r "TODO(bwfc-" MyApp.Blazor

# Use Copilot L2 for automated follow-up
copilot /webforms-migration --focus bwfc-lifecycle  # Convert lifecycle methods
copilot /webforms-migration --focus bwfc-datasource  # Wire data services
copilot /webforms-migration --focus bwfc-validation  # Add DataAnnotations
```

## Best Practices

### During Migration

- **Don't remove TODO comments** — They guide L2 automation
- **Preserve category slugs** — If you move code, keep the TODO with it
- **Group related work** — TODO comments on consecutive lines can be batch-converted

### After Migration

1. **Review errors first** — Sort migration report by severity
2. **Fix blocking issues** — Master pages, routing, basic compilation
3. **Run L2 passes** — One category at a time (lifecycle, then datasource, etc.)
4. **Test between passes** — Ensure each automation step improves stability

## Next Steps

- **[Transform Reference](transforms.md)** — See details on each transform
- **[Migration Report](report.md)** — Learn to interpret the report
- **[Back to CLI Overview](index.md)** — Return to main documentation
