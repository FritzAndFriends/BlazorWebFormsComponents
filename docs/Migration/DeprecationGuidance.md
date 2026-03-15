# Deprecation Guidance: Web Forms Patterns Without Blazor Equivalents

This guide explains common ASP.NET Web Forms patterns that have no direct Blazor equivalent, and recommends Blazor-native approaches for accomplishing the same goals.

## Overview

ASP.NET Web Forms was built on a **stateful, server-side execution model** that fundamentally differs from Blazor's **component-based, request-response architecture**. Several Web Forms features were deeply tied to this model and have no direct counterpart in Blazor. Rather than forcing these patterns into Blazor, we recommend adopting Blazor-native approaches that are cleaner, more performant, and easier to maintain.

---

## `runat="server"` Attribute

### What Was It?

In Web Forms, the `runat="server"` attribute on HTML elements told the framework to manage that element as a server-side control, enabling server-side event handling, postback processing, and dynamic property modification.

```html
<!-- Web Forms -->
<input type="text" runat="server" id="txtName" />
<div runat="server" id="divContent">Static content</div>
```

### Why It's Gone

Blazor components are inherently server-side (in Server mode) or client-side (in WebAssembly). Every component is automatically "managed" by the framework. The `runat="server"` attribute is redundant and not part of Blazor syntax.

### Blazor Equivalent

Use Blazor components or HTML elements with `@bind` and event handlers:

=== "Web Forms"
    ```html
    <input type="text" runat="server" id="txtName" />
    <button runat="server" OnServerClick="btnSave_Click">Save</button>
    ```

=== "Blazor (Recommended)"
    ```razor
    <input type="text" @bind="Name" />
    <button @onclick="SaveAsync">Save</button>
    
    @code {
        private string Name { get; set; }
        
        private async Task SaveAsync()
        {
            // Handle save logic
        }
    }
    ```

### Key Takeaway

In Blazor, **components are server-managed by default**. Remove `runat="server"` and replace event handlers with Blazor event directives (`@onclick`, `@onchange`, etc.) and two-way binding (`@bind`).

---

## ViewState

### What Was It?

ViewState serialized control and page state into a hidden Base64-encoded input field, transmitted with every postback to restore state on the server.

```html
<!-- Web Forms renders something like this: -->
<input type="hidden" name="__VIEWSTATE" value="dDwtNT..." />
```

### Why It's Gone

- **Performance cost:** ViewState bloated HTTP payloads (sometimes megabytes), slowing pages significantly.
- **Blazor's model:** Server-side state is maintained in component fields and properties—no serialization needed.
- **WebAssembly apps:** State is already in browser memory.

### Blazor Equivalent

Use **component fields** for per-instance state, or **services** for shared state:

=== "Web Forms (ViewState)"
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        if (ViewState["Counter"] == null)
            ViewState["Counter"] = 0;
        
        int counter = (int)ViewState["Counter"];
        lblCounter.Text = counter.ToString();
    }
    
    protected void btnIncrement_Click(object sender, EventArgs e)
    {
        int counter = (int)ViewState["Counter"];
        counter++;
        ViewState["Counter"] = counter;
    }
    ```

=== "Blazor (Component State)"
    ```razor
    <p>Counter: @Counter</p>
    <button @onclick="IncrementAsync">Increment</button>
    
    @code {
        private int Counter { get; set; }
        
        private Task IncrementAsync()
        {
            Counter++;
            return Task.CompletedTask;
        }
    }
    ```

### For Shared State: Use Services

=== "Web Forms (Session State)"
    ```csharp
    // Store user preferences in Session
    Session["Theme"] = "dark";
    string theme = (string)Session["Theme"];
    ```

=== "Blazor (Scoped Service)"
    ```csharp
    // UserPreferencesService.cs
    public class UserPreferencesService
    {
        public string Theme { get; set; } = "light";
    }
    
    // Add to DI container
    services.AddScoped<UserPreferencesService>();
    
    // Use in component
    @inject UserPreferencesService PreferencesService
    
    @code {
        private void SetTheme(string theme)
        {
            PreferencesService.Theme = theme;
        }
    }
    ```

### Key Takeaway

Replace ViewState with:
- **Component fields** for per-instance state (replaces most ViewState usage)
- **Scoped services** for shared state across components (replaces Session)
- **Database or persistent cache** for long-term data persistence

---

## UpdatePanel

### What Was It?

UpdatePanel enabled **partial-page postbacks**, allowing a server-side event to update only a portion of the page via AJAX without a full page reload.

```html
<!-- Web Forms -->
<asp:UpdatePanel ID="pnlUserForm" runat="server">
    <ContentTemplate>
        <asp:TextBox ID="txtName" runat="server" />
        <asp:Button ID="btnSave" Text="Save" OnClick="btnSave_Click" runat="server" />
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>
```

### Why It's Gone

Blazor's **component rendering model is inherently incremental**. When a component's state changes (via an event handler), Blazor automatically re-renders only the affected parts. UpdatePanel is redundant.

### Blazor Equivalent

Use Blazor components directly:

=== "Web Forms (UpdatePanel)"
    ```html
    <asp:UpdatePanel ID="pnlUserForm" runat="server">
        <ContentTemplate>
            <asp:Label ID="lblMessage" runat="server" />
            <asp:Button ID="btnSave" Text="Save" OnClick="btnSave_Click" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <script runat="server">
        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "Saved!";
        }
    </script>
    ```

=== "Blazor (Recommended)"
    ```razor
    <p>@Message</p>
    <button @onclick="SaveAsync">Save</button>
    
    @code {
        private string Message { get; set; }
        
        private async Task SaveAsync()
        {
            Message = "Saved!";
        }
    }
    ```

### Key Takeaway

In Blazor, **partial updates are automatic**. When your component's `@code` block updates a field or property, Blazor re-renders the component and only sends the changed HTML to the browser. You don't need to think about "which part of the page to update"—just change the data, and Blazor handles the rest.

---

## ScriptManager & ScriptManagerProxy

### What Was It?

ScriptManager managed ASP.NET AJAX framework resources (libraries and web service proxies) and coordinated partial-page updates via UpdatePanel.

```html
<!-- Web Forms -->
<asp:ScriptManager ID="ScriptManager1" runat="server" />

<asp:UpdatePanel ID="pnl1" runat="server">
    <!-- UpdatePanel content -->
</asp:UpdatePanel>
```

### Why It's Gone

- **UpdatePanel is gone** → ScriptManager's primary job is obsolete.
- **Blazor manages JavaScript interop** via dedicated `IJSRuntime` and `.js` module imports.
- **No separate web service proxies needed**—just use HTTP client calls or SignalR.

### Blazor Equivalent

**For JavaScript Interop:**

=== "Web Forms (ScriptManager + ScriptReference)"
    ```html
    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/scripts/myScript.js" />
        </Scripts>
    </asp:ScriptManager>
    
    <script type="text/javascript">
        // Call managed function
        MyScript.DoSomething();
    </script>
    ```

=== "Blazor (IJSRuntime)"
    ```razor
    <button @onclick="CallJavaScriptAsync">Call JS</button>
    
    @inject IJSRuntime JSRuntime
    
    @code {
        private async Task CallJavaScriptAsync()
        {
            await JSRuntime.InvokeVoidAsync("MyModule.doSomething");
        }
    }
    ```

**For Web Service Access:**

=== "Web Forms (ScriptManager + ServiceReference)"
    ```html
    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/Services/UserService.asmx" />
        </Services>
    </asp:ScriptManager>
    
    <script>
        // Auto-generated proxy
        Sys.Net.JsonWebServiceProxy.invoke(
            "/Services/UserService.asmx/GetUsers",
            true,
            null,
            onSuccess
        );
    </script>
    ```

=== "Blazor (HttpClient)"
    ```razor
    <button @onclick="GetUsersAsync">Load Users</button>
    
    @inject HttpClient Http
    
    @code {
        private async Task GetUsersAsync()
        {
            var users = await Http.GetFromJsonAsync<List<User>>(
                "/api/users"
            );
        }
    }
    ```

### Key Takeaway

Replace ScriptManager with:
- **`IJSRuntime`** for calling JavaScript functions
- **`HttpClient`** or **Refit** for API calls
- **SignalR** for real-time server-to-client communication

---

## PostBack Events & Page Lifecycle

### What Was It?

Web Forms had a **complex Page lifecycle** tied to postbacks:
- **Page_Init** — Control initialization
- **Page_Load** — Data binding and initialization
- **Event handlers** — Respond to control events
- **Page_Render** — Prepare output

Every button click triggered a full postback, re-running the entire lifecycle.

```csharp
// Web Forms Page Lifecycle
public partial class MyPage : Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        // Initialize controls
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Load data only on first request
            LoadUsers();
        }
    }
    
    protected void btnSave_Click(object sender, EventArgs e)
    {
        // Handle button click
        SaveData();
    }
}
```

### Why It's Gone

Blazor's **component lifecycle is simpler and more predictable**:
- **OnInitializedAsync** — Run once when component initializes
- **OnParametersSetAsync** — Run when component parameters change
- **Render** — Generate component output
- **OnAfterRenderAsync** — Run after rendering completes

There's **no postback**—state changes trigger re-rendering of only the affected component.

### Blazor Equivalent

=== "Web Forms (Page Lifecycle)"
    ```csharp
    public partial class MyPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                gvUsers.DataSource = GetUsers();
                gvUsers.DataBind();
            }
        }
        
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            gvUsers.DataSource = GetUsers();
            gvUsers.DataBind();
        }
    }
    ```

=== "Blazor (Component Lifecycle)"
    ```razor
    @implements IAsyncDisposable
    
    <GridView Items="Users" />
    <button @onclick="RefreshAsync">Refresh</button>
    
    @code {
        private List<User> Users { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            Users = await LoadUsersAsync();
        }
        
        private async Task RefreshAsync()
        {
            Users = await LoadUsersAsync();
            // Component automatically re-renders
        }
    }
    ```

### Lifecycle Mapping

| Web Forms | Blazor | Purpose |
|-----------|--------|---------|
| `Page_Init` | `OnInitializedAsync` | One-time initialization |
| `Page_Load` (non-postback) | `OnInitializedAsync` | Load initial data |
| `Page_Load` (postback) | Event handler | Respond to user action |
| Control events | `@onclick`, `@onchange`, etc. | Handle user input |
| `Page_PreRender` | `OnAfterRenderAsync` | Final setup before render |

### Key Takeaway

In Blazor:
- **Initialize data in `OnInitializedAsync`**, not `Page_Load`
- **Handle events with `@onclick` and `@onchange`**, not server-side event handlers
- **State changes trigger re-rendering automatically**—no postback cycle
- Use `@implements IAsyncDisposable` to clean up resources in `DisposeAsync()`

---

## `IsPostBack` Check

### What Was It?

In Web Forms, `IsPostBack` distinguished between the initial page load and subsequent postbacks:

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        // Load data on initial request only
        LoadData();
    }
}
```

### Why It's Gone

Blazor components don't have "postbacks"—each component initialization is a fresh start. Use `OnInitializedAsync` or `OnParametersSetAsync` to run logic when data is needed.

### Blazor Equivalent

=== "Web Forms"
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlCountries.DataSource = GetCountries();
            ddlCountries.DataBind();
        }
    }
    ```

=== "Blazor"
    ```razor
    <select @bind="SelectedCountry">
        @foreach (var country in Countries)
        {
            <option value="@country.Id">@country.Name</option>
        }
    </select>
    
    @code {
        private List<Country> Countries { get; set; }
        private string SelectedCountry { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            Countries = await LoadCountriesAsync();
        }
    }
    ```

### Key Takeaway

Replace `if (!IsPostBack)` checks with **`OnInitializedAsync`** for initial setup. Blazor runs this once per component instance, giving you a clean equivalent to the "load-only-once" pattern.

---

## Server-Side Control Properties & Dynamic Updates

### What Was It?

In Web Forms, you could modify control properties server-side and they'd be reflected in the HTML:

```csharp
// Web Forms
protected void btnToggle_Click(object sender, EventArgs e)
{
    lblMessage.Text = "Updated text";
    lblMessage.Visible = !lblMessage.Visible;
    lblMessage.ForeColor = System.Drawing.Color.Red;
}
```

### Why It's Different

Blazor uses **declarative binding** rather than imperative property manipulation. You update component data, and Blazor handles the re-render.

### Blazor Equivalent

=== "Web Forms"
    ```csharp
    protected void btnToggle_Click(object sender, EventArgs e)
    {
        lblMessage.Text = "Updated text";
        lblMessage.ForeColor = System.Drawing.Color.Red;
        lblMessage.Visible = true;
    }
    ```

=== "Blazor"
    ```razor
    <label style="color: @(IsRed ? "red" : "black")" 
           style="display: @(IsVisible ? "block" : "none")">
        @Message
    </label>
    
    <button @onclick="ToggleAsync">Toggle</button>
    
    @code {
        private string Message = "Initial text";
        private bool IsRed { get; set; }
        private bool IsVisible { get; set; }
        
        private Task ToggleAsync()
        {
            Message = "Updated text";
            IsRed = true;
            IsVisible = true;
            return Task.CompletedTask;
        }
    }
    ```

### Key Takeaway

Instead of setting properties on controls imperatively, **update component data** and let Blazor's declarative rendering handle the HTML updates. This is more maintainable and closely mirrors modern front-end frameworks.

---

## `Page.Title` vs. `<PageTitle>`

### What Was It?

In Web Forms, you set the page title server-side:

```csharp
// Web Forms
protected void Page_Load(object sender, EventArgs e)
{
    Page.Title = "My Page Title";
}
```

### Blazor Approach

Blazor uses the `<PageTitle>` component:

=== "Web Forms"
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = "Users - My App";
    }
    ```

=== "Blazor"
    ```razor
    <PageTitle>Users - My App</PageTitle>
    
    <h1>Users</h1>
    
    @code {
        // Page title updates when component initializes
    }
    ```

### Key Takeaway

Use the **`<PageTitle>` component** at the top of your `.razor` pages or in layouts. It can contain dynamic content via `@` expressions.

---

## Server-Side Data Binding Events

### What Was It?

Web Forms data controls raised `ItemDataBound` events, allowing you to manipulate data during binding:

```csharp
// Web Forms
protected void gvUsers_ItemDataBound(object sender, GridViewRowEventArgs e)
{
    if (e.Row.RowType == DataControlRowType.DataRow)
    {
        // Modify cell content
        e.Row.Cells[0].Text = e.Row.Cells[0].Text.ToUpper();
    }
}
```

### Blazor Approach

Use component templates with Blazor's `@context` variable:

=== "Web Forms"
    ```html
    <asp:GridView ID="gvUsers" runat="server" OnRowDataBound="gvUsers_ItemDataBound">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Name" />
        </Columns>
    </asp:GridView>
    
    <script runat="server">
        protected void gvUsers_ItemDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].Text = e.Row.Cells[0].Text.ToUpper();
            }
        }
    </script>
    ```

=== "Blazor"
    ```razor
    <GridView Items="Users">
        <Columns>
            <GridViewColumn>
                @if (context is User user)
                {
                    <text>@user.Name.ToUpper()</text>
                }
            </GridViewColumn>
        </Columns>
    </GridView>
    
    @code {
        private List<User> Users { get; set; }
    }
    ```

### Key Takeaway

Replace `ItemDataBound` events with **inline Razor expressions** or **separate components** that accept data as parameters. Blazor's component model is more declarative.

---

## Application & Session State

### What Was It?

Web Forms used `Application` and `Session` collections to store global and per-user state:

```csharp
// Web Forms
// Global application state
Application["ActiveUsers"] = (int)Application["ActiveUsers"] + 1;

// Per-user session state
Session["UserPreferences"] = userPrefs;
```

### Blazor Approach

Use **services** with appropriate lifetimes:

=== "Web Forms (Global Application State)"
    ```csharp
    protected void Application_Start()
    {
        Application["ActiveUsers"] = 0;
    }
    
    void IncrementActiveUsers()
    {
        Application["ActiveUsers"] = (int)Application["ActiveUsers"] + 1;
    }
    ```

=== "Blazor (Singleton Service for Global State)"
    ```csharp
    // AppStateService.cs
    public class AppStateService
    {
        public int ActiveUsers { get; set; }
    }
    
    // Program.cs
    services.AddSingleton<AppStateService>();
    
    // In component
    @inject AppStateService AppState
    
    @code {
        private void IncrementUsers()
        {
            AppState.ActiveUsers++;
        }
    }
    ```

=== "Web Forms (Session State)"
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["UserPreferences"] = new UserPreferences();
    }
    ```

=== "Blazor (Scoped Service for User State)"
    ```csharp
    // UserPreferencesService.cs
    public class UserPreferencesService
    {
        public UserPreferences Preferences { get; set; }
    }
    
    // Program.cs
    services.AddScoped<UserPreferencesService>();
    
    // In component
    @inject UserPreferencesService Prefs
    
    @code {
        protected override async Task OnInitializedAsync()
        {
            Prefs.Preferences = new UserPreferences();
        }
    }
    ```

### Service Lifetimes

| Web Forms Pattern | Blazor Service Lifetime | Use Case |
|------------------|-------------------------|----------|
| `Application["key"]` | `Singleton` | Global app state (counters, config) |
| `Session["key"]` | `Scoped` | Per-user/per-request state |
| Local variables | Component fields | Per-component state |

### Key Takeaway

- **Global state** → `AddSingleton<T>()` service
- **Per-user state** → `AddScoped<T>()` service
- **Per-component state** → Component fields and properties

---

## Server-Side Event Timing

### What Was It?

Web Forms guaranteed specific event ordering and re-execution on postback:

```csharp
// Web Forms - events fire in this order every time:
// 1. Page_Init
// 2. Page_Load
// 3. Event handler (btnSave_Click)
// 4. Page_PreRender
// 5. Render
```

### Blazor Approach

Blazor's lifecycle is **simpler but different**:

```csharp
// Blazor - event firing order:
// 1. SetParametersAsync (parameters passed from parent)
// 2. OnInitializedAsync (initialization once per instance)
// 3. OnParametersSetAsync (triggered by parameter changes)
// 4. OnAfterRenderAsync (after HTML render)
// 5. Event handler (@onclick, @onchange, etc.)
// 6. StateHasChanged() triggers re-render
```

### Key Takeaway

Blazor's lifecycle is:
1. **Less frequent** — doesn't re-run from the top on every event
2. **More predictable** — events are async/await-friendly
3. **Parameter-driven** — `OnParametersSetAsync` replaces data-dependency logic

---

## Summary: Migration Checklist

When migrating a Web Forms page, watch for these patterns:

| Web Forms | Blazor Replacement | Notes |
|-----------|-------------------|-------|
| `runat="server"` | Remove it (Blazor components are native) | Use `@bind`, `@onclick`, etc. |
| `ViewState["key"]` | Component fields or services | Remove serialization overhead |
| `UpdatePanel` | Component re-rendering (automatic) | Blazor updates incrementally by default |
| `ScriptManager` | `IJSRuntime` + `HttpClient` | Simpler, more direct |
| `Page_Load` | `OnInitializedAsync` | Run once per component instance |
| `IsPostBack` | Remove (use `OnInitializedAsync`) | No postback concept in Blazor |
| `Page.Title` | `<PageTitle>` component | Per-page or per-route |
| `ItemDataBound` | Component templates with `@context` | More declarative |
| `Application["key"]` | `AddSingleton<Service>()` | Global application state |
| `Session["key"]` | `AddScoped<Service>()` | Per-user state |
| Server-side control properties | Data-bound component properties | Declarative, not imperative |

---

## Further Reading

- [Blazor Lifecycle Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle)
- [State Management in Blazor](https://docs.microsoft.com/en-us/aspnet/core/blazor/state-management)
- [BlazorWebFormsComponents ViewState Documentation](../UtilityFeatures/ViewState.md)
- [BlazorWebFormsComponents UpdatePanel Documentation](../EditorControls/UpdatePanel.md)
