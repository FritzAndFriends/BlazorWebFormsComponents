# Deprecation Guidance: Web Forms Patterns Without Blazor Equivalents

This guide documents Web Forms patterns that do not have direct equivalents in Blazor. Understanding these differences is essential for successful migration from ASP.NET Web Forms to Blazor.

!!! info "Why This Guide Exists"
    BlazorWebFormsComponents helps you reuse markup and familiar component APIs during migration. However, some Web Forms features were fundamentally tied to Web Forms architecture and cannot be replicated in Blazor. This guide helps you understand *why* these patterns are deprecated and *what* to use instead.

---

## runat="server" Scope

### What It Was

In ASP.NET Web Forms, every server control required the `runat="server"` attribute to signal the framework that the control should be processed server-side:

```html
<asp:TextBox ID="txtName" runat="server" />
<asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
```

Without `runat="server"`, the markup would be treated as literal HTML and not processed by the Web Forms runtime. This attribute was the bridge between client markup and server-side event handling.

### Why It's Deprecated

**Blazor components are *always* server-side by default** (in Blazor Server) or rendered on the server (in Blazor WebAssembly with server-side prerendering). There is no concept of "client-side markup" that needs server-side processing — all components are inherently server-aware.

The `runat="server"` distinction existed because Web Forms served both static HTML and dynamic controls in the same file. Blazor components are **always** interactive by design.

### What To Do Instead

Simply **remove `runat="server"`** when migrating:

=== "Web Forms"
    ```html
    <asp:TextBox ID="txtName" runat="server" />
    <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
    ```

=== "Blazor"
    ```razor
    <TextBox @bind-Value="name" />
    <Button Text="Submit" OnClick="HandleSubmit" />
    
    @code {
        private string name = "";
        
        private void HandleSubmit()
        {
            // Handle the submission
        }
    }
    ```

!!! tip "Removal Strategy"
    Use Find and Replace in your editor to remove all `runat="server"` attributes. Example pattern: `runat="server"\s+` → `` (empty string). This is a safe global replacement with no side effects.

---

## ViewState

### What It Was

Web Forms' ViewState was a key-value store that persisted control and page state across postbacks:

```csharp
// Store data
ViewState["UserID"] = 42;

// Retrieve data
int userId = (int)ViewState["UserID"];
```

The state was serialized, Base64-encoded, and stored as a hidden `__VIEWSTATE` input field in the rendered HTML. On postback, the framework deserialized this field to restore the page state.

**Problems with ViewState:**
- Hidden field could grow to megabytes if not carefully managed
- Serialization/deserialization added overhead
- Boxing and unboxing of objects reduced performance
- Encouraged anti-patterns of storing complex objects

### Why It's Deprecated

**Blazor components maintain state natively in memory.** There is no postback cycle — components are persistent objects on the server (in Blazor Server) with their fields and properties intact throughout the user's session.

BlazorWebFormsComponents does provide a `ViewState` property for migration compatibility, but it's an anti-pattern in modern Blazor development.

### What To Do Instead

Replace ViewState usage with **strongly-typed component fields and properties**:

=== "Web Forms ViewState"
    ```csharp
    // In Web Forms Page
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ViewState["UserID"] = 42;
            ViewState["UserName"] = "John Doe";
        }
    }
    
    protected void btnLoad_Click(object sender, EventArgs e)
    {
        int userId = (int)ViewState["UserID"];
        string userName = (string)ViewState["UserName"];
        lblResult.Text = $"User: {userName} (ID: {userId})";
    }
    ```

=== "Blazor Component Fields"
    ```razor
    <Label Text="@result" />
    <Button Text="Load" OnClick="HandleLoad" />
    
    @code {
        private int userId = 42;
        private string userName = "John Doe";
        private string result = "";
        
        private void HandleLoad()
        {
            result = $"User: {userName} (ID: {userId})";
        }
    }
    ```

**Benefits of this approach:**
- ✅ Strongly typed — compile-time type checking
- ✅ No serialization overhead — just in-memory references
- ✅ Clearer code — field names are explicit, not string keys
- ✅ Better IDE support — IntelliSense works on fields

### Storing Data Across Multiple Users

If you need to persist data across users (e.g., application-wide or session state), use **dependency injection with scoped or singleton services**:

```csharp
// Program.cs
builder.Services.AddScoped<AppStateService>();

// AppStateService.cs
public class AppStateService
{
    private Dictionary<string, object> _state = new();
    
    public void Set(string key, object value) => _state[key] = value;
    public object? Get(string key) => _state.TryGetValue(key, out var value) ? value : null;
}

// Component.razor
@inject AppStateService AppState

<Button Text="Store" OnClick="HandleStore" />

@code {
    private void HandleStore()
    {
        AppState.Set("UserID", 42);
    }
}
```

---

## UpdatePanel / Partial Page Updates

### What It Was

UpdatePanel enabled partial-page AJAX updates in Web Forms:

```html
<asp:ScriptManager ID="ScriptManager1" runat="server" />

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Label ID="lblStatus" runat="server" Text="Ready" />
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" OnClick="btnRefresh_Click" />
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnRefresh" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>
```

When triggered, the UpdatePanel would:
1. Post to the server via AJAX (not a full page post)
2. Re-render the content inside the UpdatePanel
3. Send back only that rendered HTML
4. Update the page without a full refresh

Without UpdatePanel, *every* button click or form submission caused a full page postback and reload.

### Why It's Deprecated

**Blazor's component model handles all rendering incrementally by default.** There is no distinction between "partial" and "full" updates — every component re-render is a partial update via SignalR. The concept of UpdatePanel is unnecessary.

BlazorWebFormsComponents provides an `UpdatePanel` component *purely as a structural wrapper* for HTML compatibility (e.g., if your CSS targets a `.update-panel` class). **The UpdateMode, Triggers, and AsyncPostBackTrigger properties have no effect.**

### What To Do Instead

Simply **use Blazor component state and event handlers**:

=== "Web Forms UpdatePanel"
    ```html
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Label ID="lblStatus" runat="server" Text="Ready" />
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" OnClick="btnRefresh_Click" />
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnRefresh" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    ```
    
    ```csharp
    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        lblStatus.Text = $"Refreshed at {DateTime.Now:HH:mm:ss}";
    }
    ```

=== "Blazor Component"
    ```razor
    <Label Text="@status" />
    <Button Text="Refresh" OnClick="HandleRefresh" />
    
    @code {
        private string status = "Ready";
        
        private void HandleRefresh()
        {
            status = $"Refreshed at {DateTime.Now:HH:mm:ss}";
        }
    }
    ```

**Key differences:**
- ✅ No ScriptManager needed
- ✅ No Triggers collection — event handlers update state directly
- ✅ No ContentTemplate wrapper — just place content between tags
- ✅ All re-rendering is automatic and incremental

### If You're Using UpdatePanel for HTML Structure Only

If your CSS or JavaScript depends on the `<div>` or `<span>` wrapper that UpdatePanel provides, you can keep it:

```razor
<UpdatePanel>
    <Label Text="@status" />
    <Button Text="Refresh" OnClick="HandleRefresh" />
</UpdatePanel>

@code {
    private string status = "Ready";
    
    private void HandleRefresh()
    {
        status = $"Refreshed at {DateTime.Now:HH:mm:ss}";
    }
}
```

This renders a `<div>` (or `<span>` if you set `RenderMode="Inline"`) and works identically to a Blazor component. However, in new code, prefer a plain `<div>` if the UpdatePanel wrapper is the only reason you're including it.

---

## Page_Load / IsPostBack Lifecycle

### What It Was

Web Forms had a strict page lifecycle with well-known event hooks:

```csharp
protected override void OnInit(EventArgs e)
{
    base.OnInit(e);
    // Controls are initialized
}

protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        // First load only — load initial data
        LoadDropDownData();
    }
    else
    {
        // Postback — state already loaded from ViewState
    }
}

protected override void OnPreRender(EventArgs e)
{
    base.OnPreRender(e);
    // Last chance to update state before rendering
}
```

The `IsPostBack` boolean indicated whether the page was:
- `true` — A form submission (user posted the page back to itself)
- `false` — Initial page load (user navigated to the page)

### Why It's Deprecated

**Blazor components use a different lifecycle model** based on component initialization and parameter changes, not postbacks.

In Blazor Server:
- **No postback concept** — components stay alive in memory
- **No `IsPostBack`** — state is preserved naturally by the component instance
- **Lifecycle is event-driven** — triggered by initialization, parameter changes, and user interactions

### What To Do Instead

Map Web Forms lifecycle methods to Blazor equivalents:

| Web Forms | Blazor | When It Fires | Purpose |
|-----------|--------|---------------|---------|
| `Page_Init` | `OnInitializedAsync` | Once, when component first creates | Initialize static data, set defaults |
| `Page_Load` (first load) | `OnInitializedAsync` or `OnParametersSetAsync` | Initial component creation | Load data for first render |
| `Page_Load` (postback) | Event handlers | When user interacts | Handle form submissions, button clicks |
| `OnPreRender` | `OnAfterRenderAsync` | After each render cycle | Access rendered DOM elements |

### Migration Patterns

#### Pattern 1: Initialize Data on First Load

=== "Web Forms"
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadDropDownData();
            LoadInitialContent();
        }
    }
    
    private void LoadDropDownData()
    {
        // Load dropdown options
    }
    
    private void LoadInitialContent()
    {
        // Load initial page content
    }
    ```

=== "Blazor"
    ```razor
    @implements IAsyncDisposable
    @code {
        protected override async Task OnInitializedAsync()
        {
            await LoadDropDownData();
            await LoadInitialContent();
        }
        
        private async Task LoadDropDownData()
        {
            // Load dropdown options
        }
        
        private async Task LoadInitialContent()
        {
            // Load initial page content
        }
        
        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            // Optional: cleanup resources
        }
    }
    ```

#### Pattern 2: React to Parameter Changes

=== "Web Forms (via QueryString)"
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string id = Request.QueryString["id"];
            if (!string.IsNullOrEmpty(id))
            {
                LoadData(id);
            }
        }
    }
    ```

=== "Blazor (via Cascading Parameter)"
    ```razor
    @implements IAsyncDisposable
    
    <h3>Item: @itemName</h3>
    
    @code {
        [Parameter]
        public string? Id { get; set; }
        
        private string itemName = "";
        
        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(Id))
            {
                await LoadData(Id);
            }
        }
        
        private async Task LoadData(string id)
        {
            // Load data based on ID
        }
        
        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            // Optional: cleanup
        }
    }
    ```

#### Pattern 3: Handle Form Submissions

=== "Web Forms"
    ```html
    <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
    
    <asp:Label ID="lblResult" runat="server" />
    ```
    
    ```csharp
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        // Handle submission
        lblResult.Text = "Form submitted!";
    }
    ```

=== "Blazor"
    ```razor
    <Button Text="Submit" OnClick="HandleSubmit" />
    
    <Label Text="@result" />
    
    @code {
        private string result = "";
        
        private void HandleSubmit()
        {
            result = "Form submitted!";
        }
    }
    ```

!!! tip "Key Insight: No IsPostBack Needed"
    In Blazor, you almost never need an equivalent to `IsPostBack`. The component instance persists — state is preserved naturally. Initialize data in `OnInitializedAsync`, respond to parameter changes in `OnParametersSetAsync`, and handle user interactions via event handlers. That's it.

---

## ScriptManager

### What It Was

ScriptManager was a required page-level component that managed:
- **Partial rendering** coordination for UpdatePanel/AJAX
- **Script registration** for adding `<script>` blocks dynamically
- **Web service proxies** for calling server-side PageMethods
- **Script mode** selection (Debug vs. Release)

```html
<asp:ScriptManager ID="ScriptManager1" runat="server" 
                   EnablePartialRendering="true"
                   EnablePageMethods="true" />

<asp:UpdatePanel>
    <ContentTemplate>
        <asp:Label ID="lblResult" runat="server" />
        <asp:Button ID="btnCall" runat="server" Text="Call Server" OnClick="btnCall_Click" />
    </ContentTemplate>
</asp:UpdatePanel>
```

Without ScriptManager, AJAX features didn't work.

### Why It's Deprecated

**Blazor handles all of these concerns natively:**
- **Partial rendering** — Built into Blazor's component model (no UpdatePanel needed)
- **Script registration** — Use `IJSRuntime` for JavaScript interop
- **Web service calls** — Use `HttpClient` or dependency injection
- **Script delivery** — Handled by the ASP.NET Core runtime

ScriptManager was a "hub" that Web Forms used to coordinate features Blazor provides out of the box.

### What To Do Instead

BlazorWebFormsComponents provides `ScriptManager` as a **migration compatibility stub** — it renders nothing and accepts all parameters silently. Include it during migration if it helps your markup compile, but **remove it once migration is stable.**

#### Option 1: Remove ScriptManager (Preferred)

=== "Web Forms"
    ```html
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
    
    <asp:Button ID="btnCall" runat="server" Text="Call Server" OnClick="btnCall_Click" />
    <asp:Label ID="lblResult" runat="server" />
    ```
    
    ```csharp
    [WebMethod]
    public static string GetData()
    {
        return "Data from server";
    }
    
    protected void btnCall_Click(object sender, EventArgs e)
    {
        // In Web Forms, PageMethods enabled calling GetData() from JavaScript
    }
    ```

=== "Blazor (Without ScriptManager)"
    ```razor
    <Button Text="Call Server" OnClick="HandleCall" />
    <Label Text="@result" />
    
    @code {
        private string result = "";
        
        private async Task HandleCall()
        {
            result = await GetDataAsync();
        }
        
        private async Task<string> GetDataAsync()
        {
            return "Data from server";
        }
    }
    ```

#### Option 2: Keep ScriptManager During Migration

If removing `<ScriptManager />` causes compilation errors during migration, include it temporarily:

```razor
<ScriptManager />  @* Will be removed in cleanup phase *@

<Button Text="Call Server" OnClick="HandleCall" />
<Label Text="@result" />

@code {
    private string result = "";
    
    private async Task HandleCall()
    {
        result = await GetDataAsync();
    }
    
    private async Task<string> GetDataAsync()
    {
        return "Data from server";
    }
}
```

#### If You're Using JavaScript Interop

Replace ScriptManager's script registration with `IJSRuntime`:

=== "Web Forms Script Registration"
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        string script = @"alert('Hello from Web Forms!');";
        ScriptManager.RegisterStartupScript(this, GetType(), "startup", script, true);
    }
    ```

=== "Blazor JS Interop"
    ```razor
    @inject IJSRuntime JS
    
    @code {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("eval", "alert('Hello from Blazor!');");
            }
        }
    }
    ```

Or better yet, call a proper JavaScript module function:

```javascript
// mymodule.js
export function showGreeting() {
    alert('Hello from Blazor!');
}
```

```razor
@inject IJSRuntime JS
@implements IAsyncDisposable

<Button Text="Greet" OnClick="HandleGreet" />

@code {
    private IJSObjectReference? module;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            module = await JS.InvokeAsync<IJSObjectReference>("import", "./mymodule.js");
        }
    }
    
    private async Task HandleGreet()
    {
        if (module is not null)
        {
            await module.InvokeVoidAsync("showGreeting");
        }
    }
    
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (module is not null)
        {
            await module.DisposeAsync();
        }
    }
}
```

!!! tip "Best Practice"
    Treat ScriptManager as scaffolding. Include it early in migration to keep pages compiling, then **remove it completely** as part of your cleanup phase. A Blazor page with `<ScriptManager />` behaves *identically* to one without it — the component renders nothing and does nothing.

---

## Server-Side Control Property Manipulation

### What It Was

In Web Forms, you could manipulate control properties on the server in code-behind:

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    // Disable a textbox
    txtEmail.Enabled = false;
    
    // Hide a button
    btnDelete.Visible = false;
    
    // Set a label's text
    lblStatus.Text = "Loading...";
    
    // Add CSS classes dynamically
    btnSubmit.CssClass = "btn btn-primary";
}
```

This approach worked because Web Forms re-rendered the entire page on each postback, picking up the property changes.

### Why It's Deprecated

**Blazor uses reactive data binding.** Control properties are derived from component state, not set imperatively.

In Blazor:
- ✅ **Declarative** — Properties come from component fields/parameters, not imperative assignments
- ✅ **Reactive** — When state changes, the UI automatically updates
- ✅ **Debuggable** — It's clear where each value comes from by looking at the template

### What To Do Instead

Bind control properties to component fields:

=== "Web Forms (Imperative)"
    ```html
    <asp:TextBox ID="txtEmail" runat="server" />
    <asp:Button ID="btnDelete" runat="server" Text="Delete" />
    <asp:Label ID="lblStatus" runat="server" />
    <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-default" />
    ```
    
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        txtEmail.Enabled = false;
        btnDelete.Visible = false;
        lblStatus.Text = "Loading...";
        btnSubmit.CssClass = "btn btn-primary";
    }
    ```

=== "Blazor (Declarative)"
    ```razor
    <TextBox Disabled="true" />
    <Button Text="Delete" Visible="isDeleteVisible" />
    <Label Text="@status" />
    <Button Text="Submit" Class="@btnClass" />
    
    @code {
        private bool isDeleteVisible = false;
        private string status = "Loading...";
        private string btnClass = "btn btn-primary";
        
        protected override async Task OnInitializedAsync()
        {
            // Data is set declaratively in the template above
            // These values are read from component fields
        }
    }
    ```

**Benefits:**
- ✅ Template clearly shows what's displayed
- ✅ No "hidden" state changes in code-behind
- ✅ Easier to reason about the UI
- ✅ Better testability (state is explicit)

---

## Application and Session State

### What It Was

Web Forms provided `HttpContext.Current.Application` and `HttpContext.Current.Session` for sharing data:

```csharp
// Store in Application state (shared across all users)
HttpContext.Current.Application["UserCount"] = 42;

// Retrieve
int count = (int)HttpContext.Current.Application["UserCount"];

// Store in Session state (per-user)
Session["UserID"] = currentUser.Id;

// Retrieve
int userId = (int)Session["UserID"];
```

- **Application state** — Shared across all users, stored in server memory
- **Session state** — Per-user, stored in server memory or external store (SQL Server, Redis)

### Why It's Deprecated

**Blazor uses dependency injection for state management.** This is cleaner, more testable, and scales better:

- ✅ **Singleton services** — Equivalent to Application state (shared across all users)
- ✅ **Scoped services** — Equivalent to Session state (per-connection/user)
- ✅ **Transient services** — New instance per request
- ✅ **Type-safe** — No casting or string keys

### What To Do Instead

Use **dependency injection with services**:

#### Application State → Singleton Service

=== "Web Forms"
    ```csharp
    // Store
    HttpContext.Current.Application["UserCount"] = 42;
    
    // Retrieve
    int count = (int)HttpContext.Current.Application["UserCount"];
    ```

=== "Blazor"
    ```csharp
    // Program.cs
    builder.Services.AddSingleton<AppStateService>();
    
    // AppStateService.cs
    public class AppStateService
    {
        public int UserCount { get; set; }
    }
    
    // Component.razor
    @inject AppStateService AppState
    
    <Label Text="@AppState.UserCount.ToString()" />
    
    @code {
        protected override void OnInitialized()
        {
            AppState.UserCount = 42;
        }
    }
    ```

#### Session State → Scoped Service

=== "Web Forms"
    ```csharp
    // Store
    Session["UserID"] = currentUser.Id;
    
    // Retrieve
    int userId = (int)Session["UserID"];
    ```

=== "Blazor"
    ```csharp
    // Program.cs
    builder.Services.AddScoped<UserSessionService>();
    
    // UserSessionService.cs
    public class UserSessionService
    {
        public int? UserId { get; set; }
    }
    
    // Component.razor
    @inject UserSessionService UserSession
    
    <Label Text="@UserSession.UserId?.ToString()" />
    
    @code {
        protected override void OnInitialized()
        {
            UserSession.UserId = currentUser.Id;
        }
    }
    ```

**Advantages:**
- ✅ Type-safe — No casting required
- ✅ Dependency injection — Easy to mock in tests
- ✅ Scalable — Works with distributed caching (Redis, etc.)
- ✅ Clear ownership — Services are explicit dependencies

---

## Data Binding Events (ItemDataBound, SelectedIndexChanged)

### What It Was

Web Forms raised binding and change events that you could handle to customize rendered content:

```html
<asp:Repeater ID="rptItems" runat="server" OnItemDataBound="rptItems_ItemDataBound">
    <ItemTemplate>
        <div><%# Eval("Name") %></div>
        <asp:Label ID="lblPrice" runat="server" />
    </ItemTemplate>
</asp:Repeater>

<asp:DropDownList ID="ddlCategory" runat="server" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged" />
```

```csharp
protected void rptItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
{
    // Customize each item as it's bound
    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
    {
        decimal price = (decimal)DataBinder.Eval(e.Item.DataItem, "Price");
        Label lblPrice = (Label)e.Item.FindControl("lblPrice");
        lblPrice.Text = price.ToString("C");
    }
}

protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
{
    // Handle selection change
    int categoryId = int.Parse(ddlCategory.SelectedValue);
    LoadProductsForCategory(categoryId);
}
```

### Why It's Deprecated

**Blazor uses component templates with `@context` to handle data binding.** There's no separate event — you just use the data directly in the template.

### What To Do Instead

Use Blazor component templates:

#### Repeater-Style Binding

=== "Web Forms"
    ```html
    <asp:Repeater ID="rptItems" runat="server" OnItemDataBound="rptItems_ItemDataBound">
        <ItemTemplate>
            <div><%# Eval("Name") %></div>
            <asp:Label ID="lblPrice" runat="server" />
        </ItemTemplate>
    </asp:Repeater>
    ```
    
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            rptItems.DataSource = GetItems();
            rptItems.DataBind();
        }
    }
    
    protected void rptItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            decimal price = (decimal)DataBinder.Eval(e.Item.DataItem, "Price");
            Label lblPrice = (Label)e.Item.FindControl("lblPrice");
            lblPrice.Text = price.ToString("C");
        }
    }
    ```

=== "Blazor with Repeater"
    ```razor
    <Repeater Items="items">
        <ItemTemplate>
            <div>@context.Name</div>
            <Label Text="@context.Price.ToString("C")" />
        </ItemTemplate>
    </Repeater>
    
    @code {
        private List<Item>? items;
        
        protected override async Task OnInitializedAsync()
        {
            items = await GetItemsAsync();
        }
        
        private async Task<List<Item>> GetItemsAsync()
        {
            // Load items
            return new();
        }
    }
    
    class Item
    {
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
    }
    ```

#### DropDownList Selection Change

=== "Web Forms"
    ```html
    <asp:DropDownList ID="ddlCategory" runat="server" 
                      OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged"
                      AutoPostBack="true" />
    <asp:Repeater ID="rptProducts" runat="server">
        <ItemTemplate>
            <div><%# Eval("Name") %></div>
        </ItemTemplate>
    </asp:Repeater>
    ```
    
    ```csharp
    protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        int categoryId = int.Parse(ddlCategory.SelectedValue);
        LoadProductsForCategory(categoryId);
    }
    
    private void LoadProductsForCategory(int categoryId)
    {
        rptProducts.DataSource = GetProducts(categoryId);
        rptProducts.DataBind();
    }
    ```

=== "Blazor"
    ```razor
    <DropDownList Items="categories" 
                  @bind-Value="selectedCategoryId" 
                  OnChange="HandleCategoryChange" />
    
    <Repeater Items="products">
        <ItemTemplate>
            <div>@context.Name</div>
        </ItemTemplate>
    </Repeater>
    
    @code {
        private List<Category> categories = new();
        private List<Product> products = new();
        private string selectedCategoryId = "";
        
        protected override async Task OnInitializedAsync()
        {
            categories = await GetCategoriesAsync();
        }
        
        private async Task HandleCategoryChange()
        {
            if (int.TryParse(selectedCategoryId, out int categoryId))
            {
                products = await GetProductsAsync(categoryId);
            }
        }
        
        private async Task<List<Category>> GetCategoriesAsync() { /* ... */ return new(); }
        private async Task<List<Product>> GetProductsAsync(int categoryId) { /* ... */ return new(); }
    }
    ```

**Key differences:**
- ✅ No separate `ItemDataBound` event — use `@context` in the template
- ✅ No `FindControl` — data is directly accessible
- ✅ No casting — types are strongly checked
- ✅ Reactive — updating `products` or `selectedCategoryId` automatically re-renders

---

## Summary Migration Checklist

As you encounter Web Forms patterns during migration, refer to this checklist:

| Web Forms Pattern | Action | Blazor Alternative |
|------------------|--------|-------------------|
| `runat="server"` | Remove | All components are server-side in Blazor |
| `ViewState` | Replace | Component fields, scoped/singleton services |
| `UpdatePanel` | Remove or keep for CSS | Blazor's incremental rendering is automatic |
| `ScriptManager` | Remove (keep temporarily if needed) | `IJSRuntime`, `HttpClient` |
| `Page_Load` | Replace | `OnInitializedAsync`, event handlers |
| `IsPostBack` | Remove | Component instance persistence |
| `Page_Init` | Replace | `OnInitializedAsync` |
| Server control properties | Bind | Component fields, reactive data binding |
| `Application` state | Replace | Singleton services |
| `Session` state | Replace | Scoped services |
| `ItemDataBound` event | Replace | Blazor templates with `@context` |
| `SelectedIndexChanged` event | Replace | `OnChange`, event handlers |

---

## Next Steps

1. **Review your Web Forms codebase** for patterns in this guide
2. **Plan your migration** using the [Migration Strategies](Strategies.md) document
3. **Use the [Automated Migration Guide](AutomatedMigration.md)** for your initial conversion
4. **Test thoroughly** — visual regression is common after migration
5. **Gradually refactor** — don't try to modernize everything at once

For more guidance, see:
- [Migration Strategies](Strategies.md)
- [Automated Migration Guide](AutomatedMigration.md)
- [Master Pages Migration](MasterPages.md)
- [User Controls Migration](User-Controls.md)
- [Page Service Documentation](../UtilityFeatures/PageService.md)
