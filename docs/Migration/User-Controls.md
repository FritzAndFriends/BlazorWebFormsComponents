# Migrating User Controls to Blazor

User Controls (`.ascx` files) are a fundamental building block in ASP.NET Web Forms applications. They provide reusable, encapsulated UI components with both markup and code-behind logic. Migrating them to Blazor is straightforward: ASCX user controls become Razor components (`.razor` files) with minimal structural changes.

## Understanding User Controls

### Web Forms User Control Structure

In Web Forms, a user control consists of three parts:

1. **Register Directive** — Declares the control in ASPX pages:
```html
<%@ Register TagPrefix="uc" TagName="PageHeader" Src="~/Controls/PageHeader.ascx" %>
```

2. **Markup (`.ascx` file)** — The HTML and Web Forms controls:
```html
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageHeader.ascx.cs" Inherits="DepartmentPortal.Controls.PageHeader" %>

<div class="page-header">
    <h1><%: Title %></h1>
    <p><%: Subtitle %></p>
</div>
```

3. **Code-Behind (`.ascx.cs` file)** — Properties, events, and lifecycle:
```csharp
public partial class PageHeader : UserControl
{
    public string Title { get; set; }
    public string Subtitle { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialization logic
    }
}
```

### Using User Controls in ASPX Pages

```html
<uc:PageHeader ID="header" runat="server" Title="Welcome" Subtitle="To Our Site" />
```

The developer passed properties declaratively; ASP.NET's control tree engine bound them at runtime.

---

## Mapping to Blazor Razor Components

In Blazor, user controls become **Razor components**:

| Web Forms | Blazor | Notes |
|-----------|--------|-------|
| `.ascx` file | `.razor` file | Single file combines markup + code |
| `<%@ Register %>` | Import in `_Imports.razor` | No registration needed |
| Public properties | `[Parameter]` decorated properties | Declare parameters for parent→child communication |
| `Page_Load` event | `OnInitializedAsync` or `OnParametersSetAsync` | Lifecycle hooks differ; see below |
| `FindControl()` + casting | `@ref` or cascading parameters | Direct component references or parameter passing |
| Data binding `<%# Eval(...) %>` | Direct property access `@item.Property` | Simpler, more declarative syntax |
| Events (`Click`, `Changed`) | `EventCallback<T>` | Async event handling |

---

## Step-by-Step Migration Process

### Step 1: Create the `.razor` File

Create a new Razor component file with the same name as your ASCX user control. Place it in a `Components` or `Controls` directory (this is a convention; Blazor has no requirement).

**Example: `PageHeader.razor` (replaces `PageHeader.ascx` + `PageHeader.ascx.cs`)**

```razor
@* PageHeader.razor *@

<div class="page-header">
    <h1>@Title</h1>
    <p>@Subtitle</p>
</div>

@code {
    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string Subtitle { get; set; }
}
```

### Step 2: Remove Web Forms Syntax

Remove the `<%@ Control %>` directive and `runat="server"` attributes. Blazor doesn't use these.

**Before:**
```html
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageHeader.ascx.cs" Inherits="..." %>
<asp:TextBox ID="searchBox" runat="server" />
```

**After:**
```razor
<input type="text" @bind="searchQuery" />
```

### Step 3: Convert Properties to Parameters

Properties that are set declaratively in markup must be decorated with `[Parameter]`.

**Before (Web Forms `PageHeader.ascx.cs`):**
```csharp
public partial class PageHeader : UserControl
{
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string BackgroundColor { get; set; } = "white";
}
```

**After (Blazor `PageHeader.razor`):**
```razor
@code {
    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string Subtitle { get; set; }

    [Parameter]
    public string BackgroundColor { get; set; } = "white";
}
```

### Step 4: Convert Events to EventCallback

Web Forms events (`Click`, `TextChanged`, etc.) become `EventCallback<T>` parameters in Blazor.

**Before (Web Forms):**
```csharp
public event EventHandler SearchClicked;

protected void SearchButton_Click(object sender, EventArgs e)
{
    SearchClicked?.Invoke(this, EventArgs.Empty);
}
```

```html
<asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" />
```

**After (Blazor):**
```razor
<button @onclick="OnSearchClick">Search</button>

@code {
    [Parameter]
    public EventCallback OnSearchClicked { get; set; }

    private async Task OnSearchClick()
    {
        await OnSearchClicked.InvokeAsync();
    }
}
```

### Step 5: Migrate Page Lifecycle

Web Forms user controls use `Page_Load`, `Page_PreRender`, and other lifecycle events. Blazor components use different hooks:

| Web Forms | Blazor | When It Runs |
|-----------|--------|--------------|
| `Page_Load` (if `!IsPostBack`) | `OnInitializedAsync` | Once, when component first loads |
| `Page_Load` (every postback) | `OnParametersSetAsync` | When parameters change or component re-initializes |
| `Page_PreRender` | `OnAfterRenderAsync` | After render tree is built, before DOM update |
| `Dispose` / `OnUnload` | `IAsyncDisposable` | When component is destroyed |

**Example: Initialization Logic**

**Before (Web Forms `Page_Load`):**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        LoadEmployeeData();
    }
}

private void LoadEmployeeData()
{
    // Fetch from database
}
```

**After (Blazor `OnInitializedAsync`):**
```razor
@code {
    protected override async Task OnInitializedAsync()
    {
        await LoadEmployeeData();
    }

    private async Task LoadEmployeeData()
    {
        // Fetch from database
    }
}
```

### Step 6: Replace Data Binding Syntax

Web Forms uses `<%# Eval("PropertyName") %>` for data binding. Blazor uses direct property access with `@`.

**Before (Web Forms `Repeater` inside ASCX):**
```html
<asp:Repeater ID="EmployeeRepeater" runat="server">
    <ItemTemplate>
        <div>
            <h3><%# Eval("FirstName") %> <%# Eval("LastName") %></h3>
            <p><%# Eval("Department") %></p>
        </div>
    </ItemTemplate>
</asp:Repeater>
```

**After (Blazor Razor component with `@foreach`):**
```razor
@if (Employees != null)
{
    @foreach (var emp in Employees)
    {
        <div>
            <h3>@emp.FirstName @emp.LastName</h3>
            <p>@emp.Department</p>
        </div>
    }
}

@code {
    [Parameter]
    public IEnumerable<Employee> Employees { get; set; }
}
```

### Step 7: Replace FindControl with @ref or Cascading Parameters

Web Forms allowed `FindControl("ID")` to locate child controls. Blazor uses `@ref` or cascading parameters instead.

**Before (Web Forms — cross-boundary FindControl):**
```csharp
public partial class MyUserControl : UserControl
{
    protected void SomeMethod()
    {
        var textBox = (TextBox)FindControl("MyTextBox");
        var value = textBox.Text;
    }
}
```

**After (Blazor — use @ref):**
```razor
<input @ref="myTextBox" type="text" />

@code {
    private ElementReference myTextBox;

    private async Task SomeMethod()
    {
        var value = await JS.InvokeAsync<string>("eval", "document.getElementById('" + myTextBox.Id + "').value");
        // Or better: use two-way binding with @bind
    }
}
```

For child **component** references (not HTML elements), use `@ref`:

```razor
<ChildComponent @ref="childComponentRef" />

@code {
    private ChildComponent childComponentRef;

    private void CallChildMethod()
    {
        childComponentRef.SomePublicMethod();
    }
}
```

---

## Complete Example: EmployeeList Control Migration

### Web Forms ASCX Control

**EmployeeList.ascx:**
```html
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmployeeList.ascx.cs" Inherits="DepartmentPortal.Controls.EmployeeList" %>

<div class="employee-list">
    <input type="text" id="searchBox" placeholder="Search..." onkeyup="<%# GetJavaScriptSearch() %>" />
    
    <asp:Repeater ID="EmployeeRepeater" runat="server" OnItemCommand="EmployeeRepeater_ItemCommand">
        <HeaderTemplate>
            <table class="table">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Department</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Eval("FirstName") %> <%# Eval("LastName") %></td>
                <td><%# Eval("Department") %></td>
                <td>
                    <asp:LinkButton ID="EditButton" runat="server" 
                        CommandName="Edit" CommandArgument='<%# Eval("ID") %>'>Edit</asp:LinkButton>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>
```

**EmployeeList.ascx.cs:**
```csharp
public partial class EmployeeList : UserControl
{
    public IEnumerable<Employee> Employees { get; set; }
    
    public event EventHandler<int> EditRequested;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack && Employees != null)
        {
            BindData();
        }
    }

    private void BindData()
    {
        EmployeeRepeater.DataSource = Employees;
        EmployeeRepeater.DataBind();
    }

    protected void EmployeeRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Edit")
        {
            EditRequested?.Invoke(this, int.Parse((string)e.CommandArgument));
        }
    }

    private string GetJavaScriptSearch()
    {
        return "alert('Search not implemented');";
    }
}
```

### Blazor Razor Component Equivalent

**EmployeeList.razor:**
```razor
<div class="employee-list">
    <input type="text" placeholder="Search..." @onkeyup="OnSearch" />
    
    @if (FilteredEmployees?.Any() == true)
    {
        <table class="table">
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Department</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var emp in FilteredEmployees)
                {
                    <tr>
                        <td>@emp.FirstName @emp.LastName</td>
                        <td>@emp.Department</td>
                        <td>
                            <button @onclick="() => OnEdit(emp.ID)">Edit</button>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    }
    else
    {
        <p>No employees found.</p>
    }
</div>

@code {
    [Parameter]
    public IEnumerable<Employee> Employees { get; set; }

    [Parameter]
    public EventCallback<int> OnEditRequested { get; set; }

    private IEnumerable<Employee> FilteredEmployees { get; set; }
    private string SearchTerm { get; set; } = "";

    protected override async Task OnParametersSetAsync()
    {
        await ApplyFilter();
    }

    private async Task OnSearch(KeyboardEventArgs e)
    {
        SearchTerm = await JS.InvokeAsync<string>("getInputValue", e.Target);
        await ApplyFilter();
    }

    private async Task ApplyFilter()
    {
        FilteredEmployees = string.IsNullOrEmpty(SearchTerm)
            ? Employees
            : Employees.Where(e => 
                e.FirstName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                e.LastName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

        // Optional: debounce or throttle the filter
        await Task.Delay(100);
    }

    private async Task OnEdit(int employeeId)
    {
        await OnEditRequested.InvokeAsync(employeeId);
    }
}
```

### Key Differences in the Migration

1. **No repeater control** — Use `@foreach` for list rendering
2. **No Register directive** — Import in `_Imports.razor` instead
3. **No FindControl** — Use event callbacks or `@ref`
4. **Data binding is automatic** — `Employees` property is directly accessible in the template
5. **Events are async** — Use `EventCallback<T>` and `await`
6. **Lifecycle is different** — `OnParametersSetAsync` replaces `Page_Load` with `IsPostBack` check

---

## Common Pitfalls and Solutions

### 1. Parameter Changes Not Triggering Re-Render

**Problem:** User control properties change, but the component doesn't update.

**Solution:** Use `OnParametersSetAsync()` to react to parameter changes, or use `@bind` for two-way binding.

```razor
@code {
    [Parameter]
    public string FilterCriteria { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        // This runs whenever FilterCriteria changes
        await RefreshData();
    }
}
```

### 2. Accessing HTML Element Values

**Problem:** Trying to use `FindControl` on HTML elements instead of components.

**Solution:** Use `@ref` for ElementReference, or use `@bind` for two-way data binding.

```razor
<input @bind="searchQuery" type="text" />

@code {
    private string searchQuery = "";
    
    // searchQuery is automatically updated as the user types
}
```

### 3. Child Component Not Responding to Parent Updates

**Problem:** Parent passes new data, but child component doesn't reflect the change.

**Solution:** Ensure the child component has `[Parameter]` properties and responds in `OnParametersSetAsync`.

```razor
@* Parent *@
<ChildList Items="filteredItems" />

@* Child *@
<div>
    @foreach (var item in Items)
    {
        <p>@item.Name</p>
    }
</div>

@code {
    [Parameter]
    public IEnumerable<Item> Items { get; set; }
}
```

### 4. Lost Context in Nested Components

**Problem:** Deep component hierarchies lose access to ancestor state.

**Solution:** Use cascading parameters to pass data down through multiple levels.

```razor
@* Ancestor *@
<CascadingValue Value="CurrentUser">
    <ChildComponent />
</CascadingValue>

@* Descendant (multiple levels deep) *@
@code {
    [CascadingParameter]
    public User CurrentUser { get; set; }
}
```

---

## State Management in User Controls

### Using ViewStateDictionary for Component State

When migrating user controls that rely on ViewState, use `ViewStateDictionary` (available on `BaseWebFormsComponent` and `WebFormsPageBase`) to store component-level state that persists across postbacks.

#### Basic ViewState Usage

**Before (Web Forms ASCX):**
```csharp
public partial class SearchResultsControl : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ViewState["CurrentPage"] = 1;
            ViewState["SearchTerm"] = "";
        }
    }

    protected void NextButton_Click(object sender, EventArgs e)
    {
        int page = (int)ViewState["CurrentPage"];
        ViewState["CurrentPage"] = page + 1;
        LoadResults();
    }

    private void LoadResults()
    {
        string term = (string)ViewState["SearchTerm"];
        // Load and bind data...
    }
}
```

**After (Blazor):**
```razor
@inherits BaseWebFormsComponent

<div>
    <input @bind="_searchTerm" placeholder="Search..." />
    <Button Text="Search" @onclick="OnSearch" />
    <Button Text="Next" @onclick="OnNext" />
</div>

@code {
    private string _searchTerm = "";

    protected override void OnInitialized()
    {
        if (!IsPostBack)
        {
            ViewState.Set("CurrentPage", 1);
            ViewState.Set("SearchTerm", "");
        }
        else
        {
            // Restore state on postback
            int page = ViewState.GetValueOrDefault<int>("CurrentPage", 1);
            _searchTerm = ViewState.GetValueOrDefault<string>("SearchTerm", "");
        }
    }

    private void OnSearch()
    {
        ViewState.Set("CurrentPage", 1);
        ViewState.Set("SearchTerm", _searchTerm);
    }

    private void OnNext()
    {
        int page = ViewState.GetValueOrDefault<int>("CurrentPage", 1);
        ViewState.Set("CurrentPage", page + 1);
    }
}
```

#### Type-Safe ViewState Access

For cleaner, more maintainable code, use the `GetValueOrDefault<T>()` and `Set<T>()` methods:

```razor
@code {
    private int GetCurrentPage()
    {
        return ViewState.GetValueOrDefault<int>("CurrentPage", 1);
    }

    private void SetCurrentPage(int page)
    {
        ViewState.Set("CurrentPage", page);
    }

    private void OnNext()
    {
        int page = GetCurrentPage();
        SetCurrentPage(page + 1);
    }
}
```

### Sharing State Between Components

When multiple child components need to share state, store it in the parent component's ViewState and pass it down via parameters:

```razor
@* Parent Component *@
<ChildFilter @ref="_filterControl" 
             OnFilterApplied="@OnFilterChanged" />
<ChildResults Items="@_results" />

@code {
    private ChildFilter _filterControl;
    private List<Item> _results;

    protected override void OnInitialized()
    {
        if (!IsPostBack)
        {
            _results = LoadDefaultResults();
            ViewState.Set("LastFilter", "");
        }
    }

    private void OnFilterChanged(string filter)
    {
        ViewState.Set("LastFilter", filter);
        _results = ApplyFilter(_results, filter);
    }
}

@* Child Filter Component *@
<input @bind="_filterText" />
<button @onclick="OnApplyFilter">Apply</button>

@code {
    [Parameter]
    public EventCallback<string> OnFilterApplied { get; set; }

    private string _filterText = "";

    private async Task OnApplyFilter()
    {
        await OnFilterApplied.InvokeAsync(_filterText);
    }
}
```

---

## Event Handling and Component Communication

### Converting Web Forms Events to EventCallback

Web Forms user controls expose events that parent pages listen to. In Blazor, use `EventCallback<T>` parameters for bidirectional communication.

#### Simple Event Callback (No Payload)

**Before (Web Forms):**
```csharp
public partial class ConfirmDialog : UserControl
{
    public event EventHandler OnOK;
    public event EventHandler OnCancel;

    protected void OKButton_Click(object sender, EventArgs e)
    {
        OnOK?.Invoke(this, EventArgs.Empty);
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        OnCancel?.Invoke(this, EventArgs.Empty);
    }
}
```

**After (Blazor):**
```razor
<div class="dialog">
    <p>@Message</p>
    <Button Text="OK" @onclick="OnOKClick" />
    <Button Text="Cancel" @onclick="OnCancelClick" />
</div>

@code {
    [Parameter]
    public string Message { get; set; }

    [Parameter]
    public EventCallback OnOK { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    private async Task OnOKClick()
    {
        await OnOK.InvokeAsync();
    }

    private async Task OnCancelClick()
    {
        await OnCancel.InvokeAsync();
    }
}
```

#### Event Callback With Data (Typed Payload)

**Before (Web Forms):**
```csharp
public partial class ItemSelector : UserControl
{
    public event EventHandler<int> OnItemSelected;

    protected void ItemRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Select")
        {
            int itemId = int.Parse((string)e.CommandArgument);
            OnItemSelected?.Invoke(this, itemId);
        }
    }
}
```

**After (Blazor):**
```razor
<ul>
    @foreach (var item in Items)
    {
        <li>
            @item.Name
            <button @onclick="() => OnSelectItem(item.ID)">Select</button>
        </li>
    }
</ul>

@code {
    [Parameter]
    public List<Item> Items { get; set; }

    [Parameter]
    public EventCallback<int> OnItemSelected { get; set; }

    private async Task OnSelectItem(int itemId)
    {
        await OnItemSelected.InvokeAsync(itemId);
    }

    public class Item
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
```

#### Parent Component Using Event Callbacks

```razor
@page "/demo"

<ConfirmDialog Message="Are you sure?" 
               OnOK="@OnConfirmed" 
               OnCancel="@OnCancelled" />

<ItemSelector Items="@_items" 
              OnItemSelected="@OnItemSelected" />

@code {
    private List<Item> _items = new();

    private void OnConfirmed()
    {
        Console.WriteLine("User confirmed");
    }

    private void OnCancelled()
    {
        Console.WriteLine("User cancelled");
    }

    private void OnItemSelected(int itemId)
    {
        Console.WriteLine($"Selected item: {itemId}");
    }
}
```

---

## PostBack Patterns: IsPostBack and ViewState Integration

### Understanding IsPostBack in Components

`IsPostBack` indicates whether the current render is a first-time initialization or a subsequent postback/re-render. Combined with `ViewState`, this enables Web Forms-style initialization patterns.

#### IsPostBack Detection Behavior

- **SSR (Server-Side Rendering)** — Returns `true` when the HTTP request method is POST (form submission)
- **ServerInteractive (Blazor WebSocket)** — Returns `true` after the component has initialized once

#### One-Time Initialization with IsPostBack

```razor
@inherits BaseWebFormsComponent

<div>
    <p>Items loaded: @_items.Count</p>
    <button @onclick="OnRefresh">Refresh Data</button>
</div>

@code {
    private List<string> _items = new();

    protected override async Task OnInitializedAsync()
    {
        if (!IsPostBack)
        {
            // First render: load data from database
            _items = await FetchItemsFromDatabase();
        }
        // On postback: _items retains its previous value
    }

    private async Task OnRefresh()
    {
        _items = await FetchItemsFromDatabase();
    }

    private async Task<List<string>> FetchItemsFromDatabase()
    {
        // Simulate database call
        await Task.Delay(100);
        return new() { "Item1", "Item2", "Item3" };
    }
}
```

### Combining IsPostBack with ViewState

For SSR scenarios where state must survive form POSTs, use both `IsPostBack` and `ViewState`:

```razor
@page "/cart"
@inherits WebFormsPageBase

<form method="post">
    <div>
        <h2>Shopping Cart</h2>
        <input @bind="_itemName" placeholder="Item name" />
        <Button Text="Add Item" @onclick="OnAddItem" />
    </div>

    @if (_cartItems?.Any() == true)
    {
        <ul>
            @foreach (var item in _cartItems)
            {
                <li>@item</li>
            }
        </ul>
    }

    <RenderViewStateField />
</form>

@code {
    private string _itemName = "";
    private List<string> _cartItems = new();

    protected override void OnInitialized()
    {
        if (!IsPostBack)
        {
            // First render: initialize cart from database
            _cartItems = new();
            ViewState.Set("CartItems", _cartItems);
        }
        else
        {
            // Postback: restore cart from ViewState
            _cartItems = ViewState.GetValueOrDefault<List<string>>("CartItems", new());
        }
    }

    private void OnAddItem()
    {
        if (!string.IsNullOrEmpty(_itemName))
        {
            _cartItems.Add(_itemName);
            ViewState.Set("CartItems", _cartItems);
            _itemName = "";
        }
    }
}
```

---

## Gradual Migration: Coexisting ASCX and Razor Components

When migrating a large application, you don't need to convert all user controls at once. ASCX controls and Razor components can coexist during the transition period.

### Migration Timeline Strategy

1. **Phase 1: Simple Controls** — Migrate stateless presentational controls first
2. **Phase 2: Event-Driven Controls** — Migrate controls with events and callbacks
3. **Phase 3: Stateful Controls** — Migrate complex controls with ViewState
4. **Phase 4: Integration** — Update parent pages to use new Razor versions

### Parallel Implementation Pattern

```razor
@* Interim state: Both old ASCX and new Razor exist *@

@* Page that uses OLD ASCX control *@
<%@ Register TagPrefix="uc" TagName="OldSearch" Src="~/Controls/OldSearchBox.ascx" %>
<uc:OldSearch ID="SearchControl" runat="server" />

@* Same page (during transition) using NEW Razor component *@
@* Import in _Imports.razor instead of Register directive *@
<NewSearchBox @ref="_newSearchControl" 
              OnSearch="@OnSearch" />
```

### Wrapper Pattern for Gradual Rollout

Create a wrapper component that conditionally uses old or new implementations:

```razor
@* SearchBoxAdapter.razor - wraps old and new implementations *@
@implements IAsyncDisposable

@if (UseNewImplementation)
{
    <NewSearchBox OnSearch="@OnSearch" />
}
else
{
    <div @ref="_oldControlContainer"></div>
}

@code {
    [Parameter]
    public EventCallback<string> OnSearch { get; set; }

    [Parameter]
    public bool UseNewImplementation { get; set; } = false;

    private ElementReference _oldControlContainer;

    private async Task OnSearch(string term)
    {
        await OnSearch.InvokeAsync(term);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        // Cleanup old control if needed
    }
}
```

Use a configuration flag to control which implementation is active:

```csharp
// Startup.cs or Program.cs
builder.Services.AddSingleton<MigrationConfig>(new MigrationConfig
{
    UseNewSearchBox = Environment.GetEnvironmentVariable("ENABLE_NEW_SEARCHBOX") == "true"
});
```

---

## Complete Working Examples

### Example 1: Product Catalog Control with ViewState and EventCallback

This example shows a realistic user control migration that includes state management, filtering, and events.

#### Web Forms ASCX (Before)

**ProductCatalog.ascx:**
```html
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductCatalog.ascx.cs" Inherits="eCommerce.Controls.ProductCatalog" %>

<div class="product-catalog">
    <div class="filters">
        <asp:DropDownList ID="CategoryDropdown" runat="server" 
            AutoPostBack="true" OnSelectedIndexChanged="CategoryDropdown_SelectedIndexChanged">
            <asp:ListItem Value="">All Categories</asp:ListItem>
            <asp:ListItem Value="Electronics">Electronics</asp:ListItem>
            <asp:ListItem Value="Clothing">Clothing</asp:ListItem>
        </asp:DropDownList>
    </div>

    <asp:Repeater ID="ProductRepeater" runat="server">
        <ItemTemplate>
            <div class="product-card">
                <h3><%# Eval("Name") %></h3>
                <p>Price: $<%# Eval("Price", "{0:0.00}") %></p>
                <asp:LinkButton ID="AddCartButton" runat="server" 
                    CommandName="AddCart" CommandArgument='<%# Eval("ID") %>'>
                    Add to Cart
                </asp:LinkButton>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
```

**ProductCatalog.ascx.cs:**
```csharp
public partial class ProductCatalog : UserControl
{
    public event EventHandler<int> OnProductAdded;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ViewState["CurrentCategory"] = "";
            LoadProducts("");
        }
    }

    protected void CategoryDropdown_SelectedIndexChanged(object sender, EventArgs e)
    {
        string category = CategoryDropdown.SelectedValue;
        ViewState["CurrentCategory"] = category;
        LoadProducts(category);
    }

    protected void ProductRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "AddCart")
        {
            int productId = int.Parse((string)e.CommandArgument);
            OnProductAdded?.Invoke(this, productId);
        }
    }

    private void LoadProducts(string category)
    {
        var products = GetProductsByCategory(category);
        ProductRepeater.DataSource = products;
        ProductRepeater.DataBind();
    }

    private List<Product> GetProductsByCategory(string category)
    {
        // Simulate database query
        return new()
        {
            new Product { ID = 1, Name = "Laptop", Category = "Electronics", Price = 999.99m },
            new Product { ID = 2, Name = "Shirt", Category = "Clothing", Price = 29.99m }
        };
    }
}

public class Product
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
}
```

#### Blazor Razor Component (After)

**ProductCatalog.razor:**
```razor
<div class="product-catalog">
    <div class="filters">
        <select @onchange="OnCategoryChanged">
            <option value="">All Categories</option>
            <option value="Electronics">Electronics</option>
            <option value="Clothing">Clothing</option>
        </select>
    </div>

    @if (_filteredProducts?.Any() == true)
    {
        <div class="product-grid">
            @foreach (var product in _filteredProducts)
            {
                <div class="product-card">
                    <h3>@product.Name</h3>
                    <p>Price: $@product.Price.ToString("0.00")</p>
                    <Button Text="Add to Cart" @onclick="() => OnAddToCart(product.ID)" />
                </div>
            }
        </div>
    }
    else
    {
        <p>No products found.</p>
    }
</div>

@code {
    [Parameter]
    public EventCallback<int> OnProductAdded { get; set; }

    private List<Product> _allProducts = new();
    private List<Product> _filteredProducts = new();
    private string _currentCategory = "";

    protected override void OnInitialized()
    {
        if (!IsPostBack)
        {
            // First render: load all products
            _allProducts = GetAllProducts();
            _filteredProducts = _allProducts;
            ViewState.Set("CurrentCategory", "");
        }
        else
        {
            // Postback: restore filter state
            _currentCategory = ViewState.GetValueOrDefault<string>("CurrentCategory", "");
            _allProducts = GetAllProducts();
            ApplyFilter(_currentCategory);
        }
    }

    private async Task OnCategoryChanged(ChangeEventArgs e)
    {
        _currentCategory = e.Value?.ToString() ?? "";
        ViewState.Set("CurrentCategory", _currentCategory);
        ApplyFilter(_currentCategory);
        await Task.CompletedTask;
    }

    private void ApplyFilter(string category)
    {
        _filteredProducts = string.IsNullOrEmpty(category)
            ? _allProducts
            : _allProducts.Where(p => p.Category == category).ToList();
    }

    private async Task OnAddToCart(int productId)
    {
        await OnProductAdded.InvokeAsync(productId);
    }

    private List<Product> GetAllProducts()
    {
        return new()
        {
            new Product { ID = 1, Name = "Laptop", Category = "Electronics", Price = 999.99m },
            new Product { ID = 2, Name = "Shirt", Category = "Clothing", Price = 29.99m },
            new Product { ID = 3, Name = "Tablet", Category = "Electronics", Price = 499.99m }
        };
    }

    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }
}
```

#### Usage in a Parent Page

**Web Forms (Before):**
```aspx
<%@ Page Language="C#" %>
<%@ Register TagPrefix="uc" TagName="ProductCatalog" Src="~/Controls/ProductCatalog.ascx" %>

<form runat="server">
    <h1>Shopping</h1>
    <uc:ProductCatalog ID="catalog" runat="server" OnProductAdded="Catalog_ProductAdded" />
    <asp:Label ID="StatusLabel" runat="server" />

    <script runat="server">
        protected void Catalog_ProductAdded(object sender, int productId)
        {
            StatusLabel.Text = $"Added product {productId} to cart";
        }
    </script>
</form>
```

**Blazor (After):**
```razor
@page "/shopping"

<h1>Shopping</h1>
<ProductCatalog OnProductAdded="@OnCatalogProductAdded" />
@if (!string.IsNullOrEmpty(_statusMessage))
{
    <p>@_statusMessage</p>
}

@code {
    private string _statusMessage = "";

    private void OnCatalogProductAdded(int productId)
    {
        _statusMessage = $"Added product {productId} to cart";
    }
}
```

### Example 2: Multi-Step Form Control with IsPostBack

This example demonstrates a form control that maintains state across steps using ViewState and IsPostBack.

#### Web Forms ASCX (Before)

**RegistrationWizard.ascx.cs:**
```csharp
public partial class RegistrationWizard : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ViewState["CurrentStep"] = 1;
            ShowStep(1);
        }
        else
        {
            int step = (int)ViewState["CurrentStep"];
            ShowStep(step);
        }
    }

    protected void NextButton_Click(object sender, EventArgs e)
    {
        int currentStep = (int)ViewState["CurrentStep"];
        
        if (ValidateStep(currentStep))
        {
            SaveStepData(currentStep);
            int nextStep = currentStep + 1;
            
            if (nextStep <= 3)
            {
                ViewState["CurrentStep"] = nextStep;
                ShowStep(nextStep);
            }
            else
            {
                SubmitRegistration();
            }
        }
    }

    private void SaveStepData(int step)
    {
        if (step == 1)
        {
            ViewState["FirstName"] = FirstNameTextBox.Text;
            ViewState["LastName"] = LastNameTextBox.Text;
        }
        // ... additional steps
    }

    private void ShowStep(int step)
    {
        Step1Panel.Visible = (step == 1);
        Step2Panel.Visible = (step == 2);
        Step3Panel.Visible = (step == 3);
    }
}
```

#### Blazor Razor Component (After)

**RegistrationWizard.razor:**
```razor
@inherits BaseWebFormsComponent

<div class="wizard">
    @if (CurrentStep == 1)
    {
        <div class="step">
            <h3>Step 1: Personal Information</h3>
            <input @bind="_firstName" placeholder="First Name" />
            <input @bind="_lastName" placeholder="Last Name" />
        </div>
    }
    else if (CurrentStep == 2)
    {
        <div class="step">
            <h3>Step 2: Contact Information</h3>
            <input @bind="_email" placeholder="Email" />
            <input @bind="_phone" placeholder="Phone" />
        </div>
    }
    else if (CurrentStep == 3)
    {
        <div class="step">
            <h3>Step 3: Confirmation</h3>
            <p>Name: @(ViewState.GetValueOrDefault<string>("FirstName", "")) @(ViewState.GetValueOrDefault<string>("LastName", ""))</p>
            <p>Email: @(ViewState.GetValueOrDefault<string>("Email", ""))</p>
        </div>
    }

    <div class="buttons">
        @if (CurrentStep > 1)
        {
            <Button Text="Back" @onclick="OnBack" />
        }
        @if (CurrentStep < 3)
        {
            <Button Text="Next" @onclick="OnNext" />
        }
        else
        {
            <Button Text="Submit" @onclick="OnSubmit" />
        }
    </div>
</div>

@code {
    [Parameter]
    public EventCallback<RegistrationData> OnRegistrationComplete { get; set; }

    private string _firstName = "";
    private string _lastName = "";
    private string _email = "";
    private string _phone = "";

    private int CurrentStep
    {
        get => ViewState.GetValueOrDefault<int>("CurrentStep", 1);
        set => ViewState.Set("CurrentStep", value);
    }

    protected override void OnInitialized()
    {
        if (!IsPostBack)
        {
            CurrentStep = 1;
            ViewState.Set("FirstName", "");
            ViewState.Set("LastName", "");
            ViewState.Set("Email", "");
            ViewState.Set("Phone", "");
        }
        else
        {
            // Restore form fields from ViewState
            _firstName = ViewState.GetValueOrDefault<string>("FirstName", "");
            _lastName = ViewState.GetValueOrDefault<string>("LastName", "");
            _email = ViewState.GetValueOrDefault<string>("Email", "");
            _phone = ViewState.GetValueOrDefault<string>("Phone", "");
        }
    }

    private async Task OnNext()
    {
        if (ValidateCurrentStep())
        {
            SaveCurrentStepData();
            CurrentStep++;
        }
    }

    private void OnBack()
    {
        SaveCurrentStepData();
        CurrentStep--;
    }

    private async Task OnSubmit()
    {
        SaveCurrentStepData();

        var data = new RegistrationData
        {
            FirstName = ViewState.GetValueOrDefault<string>("FirstName", ""),
            LastName = ViewState.GetValueOrDefault<string>("LastName", ""),
            Email = ViewState.GetValueOrDefault<string>("Email", ""),
            Phone = ViewState.GetValueOrDefault<string>("Phone", "")
        };

        await OnRegistrationComplete.InvokeAsync(data);
    }

    private void SaveCurrentStepData()
    {
        switch (CurrentStep)
        {
            case 1:
                ViewState.Set("FirstName", _firstName);
                ViewState.Set("LastName", _lastName);
                break;
            case 2:
                ViewState.Set("Email", _email);
                ViewState.Set("Phone", _phone);
                break;
        }
    }

    private bool ValidateCurrentStep()
    {
        return CurrentStep switch
        {
            1 => !string.IsNullOrEmpty(_firstName) && !string.IsNullOrEmpty(_lastName),
            2 => !string.IsNullOrEmpty(_email) && !string.IsNullOrEmpty(_phone),
            _ => true
        };
    }

    public class RegistrationData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
```

---

## Using BWFC Components to Ease Migration

If your user controls use BWFC compatibility components (e.g., `WebControl`, `Repeater`, `Button`), the migration is even smoother:

```razor
@* ASCX using BWFC Button *@
<uc:SearchBox>
    <BlazorWebFormsComponents:Button ID="SearchButton" runat="server" Text="Search" OnClick="..." />
</uc:SearchBox>

@* Blazor equivalent *@
<SearchBox>
    <Button Text="Search" @onclick="OnSearch" />
</SearchBox>
```

BWFC components handle attribute rendering and styling automatically, so the markup conversion is nearly 1:1.

---

## See Also

- [Custom Controls Migration Guide](Custom-Controls.md) — For controls inheriting from `WebControl` or `CompositeControl`
- [Master Pages Migration Guide](MasterPages.md) — For layouts using `ContentPlaceHolder` and `Content`
- [FindControl Migration Guide](FindControl-Migration.md) — Detailed solutions for control tree traversal patterns
- [Deferred Controls](DeferredControls.md) — Controls with no Blazor equivalent
- [ViewState and PostBack Shim](../UtilityFeatures/ViewStateAndPostBack.md) — Comprehensive guide for ViewStateDictionary and IsPostBack patterns

---

## References

- [Blazor Component Parameters](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/cascading-values-and-parameters)
- [Blazor Event Handling](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/event-handling)
- [Blazor Component Lifecycle](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle)
- [ViewState and PostBack Shim](../UtilityFeatures/ViewStateAndPostBack.md) — State management and postback detection
