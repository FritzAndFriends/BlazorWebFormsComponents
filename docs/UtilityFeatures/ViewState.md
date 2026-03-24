# ViewState & PostBack Shim

## Overview

ViewState and PostBack detection are fundamental to ASP.NET Web Forms state management. Originally, ViewState serialized control state into a hidden `__VIEWSTATE` field that round-tripped with form submissions, while `IsPostBack` distinguished between initial page loads and form submissions.

BlazorWebFormsComponents now provides **mode-adaptive ViewState and PostBack shims** that work correctly in both SSR (Static Server-Side Rendering) and InteractiveServer rendering modes. This enables Web Forms developers' ViewState-backed property patterns and `if (!IsPostBack)` guards to work unchanged during migration—a critical goal for drop-in compatibility.

### Key Features

- **ViewStateDictionary API**: `IDictionary<string, object?>` with null-safe indexer and type-safe convenience methods
- **SSR Persistence**: Protected (encrypted + signed) hidden field round-trip via `IDataProtector`
- **Interactive Persistence**: In-memory storage for the component's lifetime
- **IsPostBack Detection**: SSR checks HTTP POST method; Interactive tracks component lifecycle
- **JSON Serialization**: Modern, debuggable format for viewstate payloads
- **Zero Migration Boilerplate**: Existing Web Forms ViewState code patterns compile and work unchanged

---

## ViewState in Web Forms vs Blazor

### Web Forms Semantics

In ASP.NET Web Forms, ViewState stored control and developer state:

```csharp
// Typical Web Forms pattern in code-behind
public partial class MyPage : Page
{
    public int SelectedDepartmentId
    {
        get => (int)(ViewState["dept"] ?? 0);
        set => ViewState["dept"] = value;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SelectedDepartmentId = 5; // Persists until next postback
        }
    }

    protected void DropDown_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Can read SelectedDepartmentId here ↑
        RefreshDataGrid();
    }
}
```

After `Page_PreRender`, ViewState was serialized, encrypted (with ViewState MAC), and emitted as a hidden form field. On the next POST request, the value was deserialized and available throughout the page lifecycle.

### BWFC Implementation

BlazorWebFormsComponents replicates this behavior in Blazor:

| Aspect | SSR Mode | Interactive Mode |
|--------|----------|------------------|
| **Storage** | Protected hidden form field (`__bwfc_viewstate_{ComponentId}`) | Component instance memory |
| **Persistence** | Survives HTTP round-trip via form POST | Lives as long as the SignalR circuit |
| **Security** | `IDataProtector` (encryption + HMAC) | Server-side memory (inherent security) |
| **Format** | JSON (protected payload) | In-memory dictionary |
| **Availability** | Restored during `OnInitializedAsync` | Available immediately on component instance |

---

## ViewStateDictionary API

The `ViewState` property is now a `ViewStateDictionary` instead of a plain `Dictionary`. It implements `IDictionary<string, object?>` for backward compatibility while adding type-safe convenience methods.

### Basic Indexer (Migration-Compatible)

Unchanged from Web Forms: get returns `null` for missing keys, set marks the dictionary dirty.

```csharp
@code {
    var selectedId = (int?)(ViewState["selected"] ?? 0) ?? 0;
    ViewState["selected"] = 42;
}
```

=== "Web Forms"

```csharp
public int SelectedId
{
    get => (int)(ViewState["SelectedId"] ?? 0);
    set => ViewState["SelectedId"] = value;
}
```

=== "Blazor"

```razor
@code {
    public int SelectedId
    {
        get => ViewState.GetValueOrDefault<int>("SelectedId", 0);
        set => ViewState.Set<int>("SelectedId", value);
    }
}
```

### Type-Safe Convenience Methods

For cleaner, more maintainable code:

#### GetValueOrDefault&lt;T&gt;

```csharp
var selectedDept = ViewState.GetValueOrDefault<int>("DepartmentId", defaultValue: 0);
var title = ViewState.GetValueOrDefault<string>("PageTitle", "Untitled");
```

**Behavior:**
- Returns `defaultValue` if the key is missing or the stored value is null
- Handles JSON deserialization type coercion (JSON numbers → `int`/`long`/`double`)
- Strongly typed—no casting required

#### Set&lt;T&gt;

```csharp
ViewState.Set("DepartmentId", 42);
ViewState.Set("SelectedItems", new[] { 1, 2, 3 });
ViewState.Set("Metadata", new { Timestamp = DateTime.UtcNow });
```

**Behavior:**
- Stores any serializable value
- Marks the dictionary dirty (serialized on render in SSR mode)
- No explicit casting or boxing needed

### Example: Migrating a ViewState-Backed Property

=== "Web Forms (ASCX Code-Behind)"

```csharp
public partial class DepartmentFilter : UserControl
{
    public int SelectedDepartmentId
    {
        get
        {
            object val = ViewState["SelectedDepartmentId"];
            return val != null ? (int)val : 0;
        }
        set
        {
            ViewState["SelectedDepartmentId"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlDepartments.SelectedValue = SelectedDepartmentId.ToString();
        }
    }

    protected void ddlDepartments_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectedDepartmentId = int.Parse(ddlDepartments.SelectedValue);
        OnDepartmentChanged?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler DepartmentChanged;
}
```

=== "Blazor (Razor Component)"

```razor
@code {
    [Parameter] public EventCallback<int> OnDepartmentChanged { get; set; }

    public int SelectedDepartmentId
    {
        get => ViewState.GetValueOrDefault<int>("SelectedDepartmentId", 0);
        set => ViewState.Set("SelectedDepartmentId", value);
    }

    protected override async Task OnInitializedAsync()
    {
        if (!IsPostBack)
        {
            SelectedDepartmentId = 0;
        }
    }

    private async Task OnSelectionChanged(ChangeEventArgs e)
    {
        SelectedDepartmentId = int.Parse((string)e.Value);
        await OnDepartmentChanged.InvokeAsync(SelectedDepartmentId);
    }
}
```

---

## Mode-Adaptive Behavior

### SSR Mode (Static Server-Side Rendering)

Each HTTP request creates a new component instance. ViewState **must round-trip** via a hidden form field, exactly as Web Forms did.

#### How It Works

1. **During Render:** ViewState is serialized to JSON and protected (encrypted + HMAC'd) using `IDataProtector`
2. **Emitted HTML:** A hidden `<input>` field is rendered: `<input type="hidden" name="__bwfc_viewstate_MyComponentId" value="<protected-payload>" />`
3. **On Form POST:** The request model binding restores the protected field
4. **During OnInitializedAsync:** Before your component code runs, the hidden field is deserialized and the dictionary is populated
5. **Your Code:** Reads the restored values as if they'd never been lost

#### Example SSR Flow

```csharp
// Initial GET request
protected override async Task OnInitializedAsync()
{
    if (!IsPostBack)  // ← true on initial GET
    {
        var depts = await DataProvider.GetDepartments();
        ViewState.Set("Departments", depts);
    }
}

// User submits form (POST)
// On next request, ViewState is automatically restored
// IsPostBack is now true, so initialization is skipped
protected override async Task OnInitializedAsync()
{
    if (!IsPostBack)  // ← false on POST; block is skipped
    {
        var depts = await DataProvider.GetDepartments();  // not called
        ViewState.Set("Departments", depts);  // not called
    }
    else  // Optional: run different code on postback
    {
        var depts = ViewState.GetValueOrDefault<List<Department>>("Departments");
        // depts is restored!
    }
}
```

#### Security

ViewState payloads are protected using ASP.NET Core's Data Protection API:

- **Encryption**: AES with random IVs
- **Integrity**: HMAC-SHA256 to detect tampering
- **Key Rotation**: Automatic, no developer configuration needed
- **Configuration**: Respects `IDataProtectionProvider` (can be customized in DI)

You should **never** read or trust unencrypted ViewState directly.

### InteractiveServer Mode

The Blazor component instance lives on the server for the duration of the SignalR circuit. ViewState is simple in-memory storage.

#### How It Works

1. **OnInitializedAsync:** ViewState is initialized (empty dictionary)
2. **User Interaction:** ViewState persists in component instance memory
3. **Re-Renders:** ViewState survives via the component's field lifetime
4. **Circuit Disconnect:** ViewState is discarded when the circuit closes

No hidden fields, no serialization—pure in-memory dictionary.

#### Example Interactive Flow

```csharp
protected override async Task OnInitializedAsync()
{
    // IsPostBack is false on initial render
    if (!IsPostBack)
    {
        await LoadInitialData();
    }
}

private async Task HandleClick()
{
    // ViewState persists across interactions
    var counter = ViewState.GetValueOrDefault<int>("ClickCount", 0);
    counter++;
    ViewState.Set("ClickCount", counter);

    // On next render, ViewState["ClickCount"] == counter
}
```

---

## IsPostBack Property

The `IsPostBack` property distinguishes initial loads from re-submissions, enabling guarded data-binding patterns.

### Web Forms Semantics

In Web Forms, `IsPostBack` indicated whether the page was rendered in response to a form submission:

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        // Run only on initial GET
        LoadDepartments();
        LoadEmployees();
    }
    // Postback-safe code runs on every request
    UpdateStatusBar();
}
```

This pattern avoided re-binding data on postback (expensive) but ensured initialization happened exactly once.

### BWFC Implementation

| Scenario | Web Forms | BWFC | How It Works |
|----------|-----------|------|--------------|
| **SSR ← GET** | `IsPostBack = false` | `IsPostBack = false` | Initial page load; no form submission |
| **SSR ← POST** | `IsPostBack = true` | `IsPostBack = true` | Form submission detected via HTTP method |
| **Interactive ← Init** | N/A | `IsPostBack = false` | First `OnInitializedAsync`; component just created |
| **Interactive ← Re-render** | N/A | `IsPostBack = true` | After first render; any subsequent re-render is user-triggered |

### SSR Detection

```csharp
public bool IsPostBack
{
    get
    {
        // SSR: check HTTP method
        if (HttpContextAccessor?.HttpContext is { } context)
            return HttpMethods.IsPost(context.Request.Method);

        // InteractiveServer: use lifecycle tracking
        return _hasInitialized;
    }
}
```

### Interactive Detection

```csharp
private bool _hasInitialized = false;

protected override void OnInitialized()
{
    // First call: IsPostBack is false (set above)
    _hasInitialized = true;
    // Subsequent calls: IsPostBack is true
}
```

### Common Pattern: Conditional Data Loading

=== "Web Forms"

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        LoadDepartments();
        LoadEmployees();
    }
}
```

=== "Blazor"

```razor
@code {
    protected override async Task OnInitializedAsync()
    {
        if (!IsPostBack)
        {
            await LoadDepartments();
            await LoadEmployees();
        }
    }
}
```

---

## WebFormsRenderMode Enum

Components can query their current rendering mode via the `CurrentRenderMode` property:

```csharp
public enum WebFormsRenderMode
{
    StaticSSR,
    InteractiveServer
}
```

### Usage

```csharp
@code {
    [Inject] private WebFormsPageBase Page { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Page.CurrentRenderMode == WebFormsRenderMode.StaticSSR)
        {
            // SSR-specific logic: restore from ViewState hidden field
            await RestoreSessionData();
        }
        else
        {
            // Interactive: use in-memory cache
            CacheManager.GetOrLoad("data");
        }
    }
}
```

### Checking at Render Time

```razor
@if (Page.CurrentRenderMode == WebFormsRenderMode.StaticSSR)
{
    <!-- Render SSR-specific form controls -->
    <input type="hidden" name="@ViewStateFieldName" value="@ViewStateValue" />
}
else
{
    <!-- Render interactive-specific controls -->
    <button @onclick="HandleClick">Click Me</button>
}
```

---

## Complete Example: Order Summary Component

This example shows ViewState, IsPostBack, and EventCallback working together.

=== "Web Forms ASCX"

```html
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderSummary.ascx.cs" Inherits="Orders.Controls.OrderSummary" %>

<div class="order-summary">
    <h2>Order #<asp:Literal ID="litOrderId" runat="server" /></h2>
    <asp:Panel ID="pnlItems" runat="server">
        <asp:Repeater ID="rptItems" runat="server">
            <ItemTemplate>
                <div class="order-item">
                    <span><%# Eval("ProductName") %></span>
                    <asp:TextBox ID="txtQty" runat="server" Text='<%# Eval("Quantity") %>' />
                    <asp:LinkButton ID="btnRemove" runat="server" CommandName="Remove" CommandArgument='<%# Eval("OrderItemId") %>' Text="Remove" />
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>
    <asp:Button ID="btnRecalculate" runat="server" Text="Recalculate" OnClick="btnRecalculate_Click" />
</div>
```

```csharp
public partial class OrderSummary : UserControl
{
    public int OrderId
    {
        get => (int)(ViewState["OrderId"] ?? 0);
        set => ViewState["OrderId"] = value;
    }

    public List<OrderItem> Items
    {
        get => (List<OrderItem>)(ViewState["Items"] ?? new List<OrderItem>());
        set => ViewState["Items"] = value;
    }

    public event EventHandler<OrderChangedEventArgs> OrderChanged;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            litOrderId.Text = OrderId.ToString();
            BindItems();
        }
    }

    private void BindItems()
    {
        rptItems.DataSource = Items;
        rptItems.DataBind();
    }

    protected void btnRecalculate_Click(object sender, EventArgs e)
    {
        var updatedItems = new List<OrderItem>();
        foreach (RepeaterItem item in rptItems.Items)
        {
            var txtQty = (TextBox)item.FindControl("txtQty");
            var itemId = int.Parse((string)item.GetOrdinal(2));
            updatedItems.Add(new OrderItem { OrderItemId = itemId, Quantity = int.Parse(txtQty.Text) });
        }
        Items = updatedItems;
        OrderChanged?.Invoke(this, new OrderChangedEventArgs { Items = updatedItems });
    }
}
```

=== "Blazor Razor Component"

```razor
<div class="order-summary">
    <h2>Order #@OrderId</h2>
    <div>
        @if (Items?.Any() == true)
        {
            @foreach (var item in Items)
            {
                <div class="order-item">
                    <span>@item.ProductName</span>
                    <input type="number" @bind="item.Quantity" min="1" />
                    <button @onclick="() => RemoveItem(item.OrderItemId)">Remove</button>
                </div>
            }
        }
        else
        {
            <p>No items in order.</p>
        }
    </div>
    <button @onclick="Recalculate">Recalculate</button>
</div>

@code {
    [Parameter]
    public int OrderId { get; set; }

    [Parameter]
    public List<OrderItem> Items { get; set; } = new();

    [Parameter]
    public EventCallback<List<OrderItem>> OnOrderChanged { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (!IsPostBack)
        {
            // Load items on initial page load
            Items = await LoadOrderItems(OrderId);
            ViewState.Set("Items", Items);
        }
        else
        {
            // Restore from ViewState (SSR) or keep in-memory (Interactive)
            Items = ViewState.GetValueOrDefault<List<OrderItem>>("Items", Items ?? new());
        }
    }

    private void RemoveItem(int orderItemId)
    {
        Items?.RemoveAll(i => i.OrderItemId == orderItemId);
        ViewState.Set("Items", Items);
    }

    private async Task Recalculate()
    {
        ViewState.Set("Items", Items);
        await OnOrderChanged.InvokeAsync(Items);
    }

    private async Task<List<OrderItem>> LoadOrderItems(int orderId)
    {
        // Fetch from database
        return await DataProvider.GetOrderItems(orderId);
    }
}
```

---

## Migration Guide

### Step 1: Identify ViewState Usage

Scan your ASCX code-behind for `ViewState["key"]` patterns:

```csharp
// Pattern 1: Dictionary access
var value = ViewState["MyKey"];
ViewState["MyKey"] = newValue;

// Pattern 2: Property wrapper (most common)
public string SelectedId
{
    get => (string)(ViewState["SelectedId"] ?? "");
    set => ViewState["SelectedId"] = value;
}
```

### Step 2: Identify IsPostBack Guards

Look for `if (!IsPostBack)` blocks that guard initialization:

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        LoadData();  // Run only once
    }
}
```

### Step 3: Convert to Blazor Component

Rewrite using `ViewState.Set<T>` and `ViewState.GetValueOrDefault<T>`:

**Before (Web Forms):**

```csharp
public int SelectedDepartmentId
{
    get => (int)(ViewState["SelectedDepartmentId"] ?? 0);
    set => ViewState["SelectedDepartmentId"] = value;
}
```

**After (Blazor):**

```csharp
public int SelectedDepartmentId
{
    get => ViewState.GetValueOrDefault<int>("SelectedDepartmentId", 0);
    set => ViewState.Set("SelectedDepartmentId", value);
}
```

### Step 4: Migrate IsPostBack Logic

**Before:**

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        LoadDropdownData();
    }
}
```

**After:**

```csharp
protected override async Task OnInitializedAsync()
{
    if (!IsPostBack)
    {
        await LoadDropdownData();
    }
}
```

### Step 5: Test Both Rendering Modes

- **SSR:** Verify ViewState survives form POST (inspect network requests for `__bwfc_viewstate_` hidden field)
- **Interactive:** Verify ViewState persists across user interactions and component re-renders

---

## What's Supported Now (Phase 1)

✅ ViewStateDictionary with null-safe indexer  
✅ Type-safe `GetValueOrDefault<T>` and `Set<T>` methods  
✅ IsPostBack detection in SSR (HTTP POST) and Interactive (lifecycle) modes  
✅ JSON serialization and `IDataProtector` encryption in SSR mode  
✅ WebFormsRenderMode enum and `CurrentRenderMode` property  
✅ Hidden field round-trip via request/response cycle  

## Coming Soon (Planned)

🔄 AutoPostBack support for controls like DropDownList  
🔄 ViewState usage analyzer (BWFC002) improvements  
🔄 ViewState size warnings and diagnostics  

---

## See Also

- [User Controls Migration](../Migration/User-Controls.md) — How ViewState enables ASCX ↔ Blazor component migration
- [IsPostBack Property](https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.page.ispostback) — Web Forms reference
- [Data Protection in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/introduction) — IDataProtector reference
- [Blazor Component Lifecycle](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle) — OnInitializedAsync and OnParametersSetAsync

