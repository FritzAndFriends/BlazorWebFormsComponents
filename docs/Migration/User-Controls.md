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

---

## References

- [Blazor Component Parameters](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/cascading-values-and-parameters)
- [Blazor Event Handling](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/event-handling)
- [Blazor Component Lifecycle](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle)
