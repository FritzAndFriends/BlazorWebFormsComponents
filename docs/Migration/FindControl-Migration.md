# Migrating Away from FindControl

**FindControl** is a Web Forms method that allows you to locate a control in the page's control tree by its ID. While powerful, it's one of the most problematic patterns to migrate to Blazor because Blazor uses a component-based architecture with no "control tree" in the Web Forms sense.

This guide explains the FindControl problem, why it's difficult to migrate, and the idiomatic Blazor solutions.

---

## What FindControl Does in Web Forms

`FindControl(string id)` searches the control hierarchy for a control with the specified ID:

```csharp
// Web Forms Page code-behind
protected void Page_Load(object sender, EventArgs e)
{
    TextBox searchBox = (TextBox)FindControl("SearchBox");
    if (searchBox != null)
    {
        searchBox.Text = "Initial value";
    }
}
```

It returns null if the control is not found, which is why code often checks before using the result.

The search is **shallow by default** — it only searches direct children of the current container. To search deeper, you must either:

1. Recursively call FindControl on child containers, or
2. Understand **naming container boundaries** (explained below)

---

## The Naming Container Problem

A **naming container** is any control that implements `INamingContainer`. These include:

- `Page` — The top-level container
- `ContentPlaceHolder` — Master page content areas
- `Panel` with `GroupingText`
- Custom controls inheriting from `INamingContainer`

**The Problem:** `FindControl` does not cross naming container boundaries. If a control is inside a naming container that is not a direct ancestor, `FindControl` cannot find it.

### Example: The Master Page Boundary Problem

In DepartmentPortal, the master page contains a `MessageLiteral` control in the header:

**Site.Master:**
```html
<%@ Master Language="C#" %>
<html>
<head>
    <title>Department Portal</title>
</head>
<body>
    <form runat="server">
        <div class="header">
            <asp:Literal ID="MessageLiteral" runat="server" />
        </div>
        
        <asp:ContentPlaceHolder ID="MainContent" runat="server" />
    </form>
</body>
</html>
```

**MyPage.aspx (content page trying to access master's control):**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    // This fails! FindControl cannot cross the ContentPlaceHolder boundary
    var message = (Literal)FindControl("MessageLiteral");
    // Result: null
}
```

**Why does it fail?** The `ContentPlaceHolder` is a naming container. The page's `FindControl` searches within the page's container and the ContentPlaceHolder, but **not the MasterPage's content** (which is in a separate naming container managed by the master).

**The Web Forms Fix:** Access the master page directly:

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    // Cast Master to the specific master page type
    var siteMaster = (Site)Master;
    siteMaster.SetMessage("Welcome!");
}
```

And add a public method to the master page code-behind:

```csharp
// Site.Master.cs
public void SetMessage(string text)
{
    MessageLiteral.Text = text;
}
```

### Example: The Template Container Problem

In DepartmentPortal, the `SectionPanel` control is a composite that uses `ITemplate` for child content:

**SectionPanel.cs (Web Forms custom control):**
```csharp
public class SectionPanel : CompositeControl, INamingContainer
{
    protected override void CreateChildControls()
    {
        var container = new Control();
        Controls.Add(container);
        
        if (ContentTemplate != null)
        {
            ContentTemplate.InstantiateIn(container);
        }
    }
    
    [TemplateContainer(typeof(SectionPanel))]
    public ITemplate ContentTemplate { get; set; }
}
```

**PageContent.aspx (content page with SectionPanel):**
```html
<asp:SectionPanel ID="AnnouncementsSection" runat="server">
    <ContentTemplate>
        <asp:Repeater ID="AnnouncementsRepeater" runat="server" />
    </ContentTemplate>
</asp:SectionPanel>
```

**PageContent.aspx.cs (trying to access repeater):**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    // This fails! The Repeater is inside SectionPanel's template container
    var repeater = (Repeater)FindControl("AnnouncementsRepeater");
    // Result: null
    
    // Correct approach: go through the panel
    var panel = (SectionPanel)FindControl("AnnouncementsSection");
    var repeater = (Repeater)panel.FindControl("AnnouncementsRepeater");
}
```

**Why?** `SectionPanel` implements `INamingContainer`, creating a boundary. Controls inside the template are children of the panel's container, not the page.

---

## Why FindControl Doesn't Translate to Blazor

Blazor uses a **component-based architecture**, not a control tree:

1. **Components are not automatically indexed** — Blazor components don't have a global registry
2. **Component hierarchy is logical, not traversable** — There is no "control tree" API
3. **Parameters are explicit** — Communication happens through parameters and cascading values, not search

**In Blazor, the equivalent of "finding a control by ID" is:**
- Storing a direct reference via `@ref`
- Passing data through parameters
- Using cascading parameters for ancestor→descendant communication
- Using events for descendant→ancestor communication

---

## Blazor Equivalents

### Pattern 1: @ref for Direct References

Use `@ref` to store a reference to a component or HTML element:

**Before (Web Forms):**
```csharp
TextBox searchBox = (TextBox)FindControl("SearchBox");
searchBox.Text = "Search here";
```

**After (Blazor component):**
```razor
<SearchBox @ref="searchBoxRef" />

@code {
    private SearchBox searchBoxRef;

    private void SetSearchText()
    {
        searchBoxRef.Text = "Search here";  // Requires public property on SearchBox
    }
}
```

**Limitation:** The child component must expose the property publicly.

### Pattern 2: Parameters for Configuration

Instead of finding and modifying a control after creation, pass the desired state as a parameter:

**Before (Web Forms — find and configure):**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        TextBox nameBox = (TextBox)FindControl("NameTextBox");
        nameBox.Text = currentUser.Name;
    }
}
```

```html
<asp:TextBox ID="NameTextBox" runat="server" />
```

**After (Blazor — pass as parameter):**
```razor
<NameEditor InitialValue="currentUser.Name" />
```

```razor
@* NameEditor.razor *@
<input type="text" value="@InitialValue" />

@code {
    [Parameter]
    public string InitialValue { get; set; }
}
```

### Pattern 3: Cascading Parameters for Ancestor Communication

Use cascading parameters to allow deep component hierarchies to access ancestor state:

**Before (Web Forms — find in master page):**
```csharp
// Content page code-behind
protected void ShowAlert(string message)
{
    var master = (SiteMaster)Master;
    master.DisplayAlert(message);  // Requires public method on master
}
```

**After (Blazor — cascading parameter):**
```razor
@* App.razor or MainLayout.razor *@
<CascadingValue Value="this">
    @Body
</CascadingValue>

@code {
    public void DisplayAlert(string message) { /* ... */ }
}

@* Any descendant component *@
@code {
    [CascadingParameter]
    public MainLayout Layout { get; set; }

    private void ShowAlert(string message)
    {
        Layout?.DisplayAlert(message);
    }
}
```

### Pattern 4: EventCallback for Sibling/Child Communication

Use `EventCallback<T>` to communicate upward from child to parent:

**Before (Web Forms — repeater item command):**
```csharp
protected void EmployeeRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
{
    if (e.CommandName == "Delete")
    {
        int employeeId = (int)e.CommandArgument;
        DeleteEmployee(employeeId);
    }
}
```

**After (Blazor — event callback):**
```razor
@* Parent *@
<EmployeeList OnDeleteRequested="HandleDelete" />

@code {
    private async Task HandleDelete(int employeeId)
    {
        await DeleteEmployee(employeeId);
    }
}

@* Child (EmployeeList.razor) *@
<button @onclick="() => OnDeleteRequested.InvokeAsync(employeeId)">Delete</button>

@code {
    [Parameter]
    public EventCallback<int> OnDeleteRequested { get; set; }
}
```

### Pattern 5: Dependency Injection for Cross-Cutting Concerns

For global services (authentication, logging, settings), use DI instead of searching:

**Before (Web Forms — find global master control):**
```csharp
var userLabel = (Label)FindControl("UserLabel");  // Unreliable
userLabel.Text = GetCurrentUserName();
```

**After (Blazor — inject service):**
```razor
@inject AuthService Auth

<span>Welcome, @Auth.CurrentUser.Name</span>

@code {
    protected override async Task OnInitializedAsync()
    {
        await Auth.LoadUserAsync();
    }
}
```

---

## BWFC's FindControl — What It Does

The `BaseWebFormsComponent` class provides a `FindControl` method that matches the Web Forms API name:

```csharp
public class BaseWebFormsComponent : ComponentBase
{
    public BaseWebFormsComponent FindControl(string id)
    {
        // Recursively searches this component and all descendants for matching ID
    }
}
```

**What it does:** Searches the current component's child controls and all descendants recursively for one with the matching ID. This mirrors the deep-search behavior that migrated Web Forms code typically expects.

---

## Complete DepartmentPortal Migration Examples

### Example 1: Master Page Message Control

**Original Web Forms:**
```csharp
// Site.Master.cs
public void SetMessage(string message)
{
    MessageLiteral.Text = message;
}

// MyPage.aspx.cs
protected void Page_Load(object sender, EventArgs e)
{
    ((Site)Master).SetMessage("Welcome!");
}
```

**Blazor Equivalent:**
```razor
@* MainLayout.razor *@
<CascadingValue Value="this">
    @Body
</CascadingValue>

<div class="message">@Message</div>

@code {
    public string Message { get; set; }

    public void SetMessage(string message)
    {
        Message = message;
        StateHasChanged();  // Trigger re-render
    }
}

@* MyPage.razor *@
@page "/"
@inject MainLayout Layout

<h1>Welcome</h1>

@code {
    protected override async Task OnInitializedAsync()
    {
        Layout.SetMessage("Welcome!");
    }
}
```

### Example 2: SectionPanel with Repeater

**Original Web Forms:**
```html
<asp:SectionPanel ID="AnnouncementsSection" runat="server">
    <ContentTemplate>
        <asp:Repeater ID="AnnouncementsRepeater" runat="server" />
    </ContentTemplate>
</asp:SectionPanel>
```

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    var panel = (SectionPanel)FindControl("AnnouncementsSection");
    var repeater = (Repeater)panel.FindControl("AnnouncementsRepeater");
    repeater.DataSource = GetAnnouncements();
    repeater.DataBind();
}
```

**Blazor Equivalent:**
```razor
<SectionPanel @ref="announcementsPanelRef">
    <Repeater Items="announcements">
        <ItemTemplate>
            <div>@context.Title</div>
        </ItemTemplate>
    </Repeater>
</SectionPanel>

@code {
    private SectionPanel announcementsPanelRef;
    private List<Announcement> announcements = new();

    protected override async Task OnInitializedAsync()
    {
        announcements = await GetAnnouncements();
    }
}
```

**Key difference:** The repeater is bound declaratively via the `Items` parameter, not through imperative FindControl + DataBind.

### Example 3: Navigation Control with Active State

**Original Web Forms:**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    var navControl = (SidebarNav)FindControl("Navigation");
    if (navControl != null)
    {
        navControl.SetActiveItem(GetCurrentPageName());
    }
}
```

**Blazor Equivalent:**
```razor
<SidebarNav ActiveItem="currentPageName" />

@code {
    private string currentPageName;

    protected override void OnInitialized()
    {
        currentPageName = GetCurrentPageName();
    }
}
```

**Pattern:** Pass the active item as a parameter instead of finding and calling a method.

---

## Migration Patterns Table

| Web Forms Pattern | Problem | Blazor Solution |
|------------------|---------|-----------------|
| `FindControl("ID")` on direct child | Simple lookup | Use `@ref` reference |
| `FindControl()` for configuration | Late-binding state | Use parameters instead |
| Access control in master page | Naming container boundary | Expose public method on master; call from derived page class |
| Access control in content placeholder | Naming container boundary | Pass as cascading parameter from master |
| Search repeater items | Dynamic control creation | Use `@foreach` with direct references |
| Get control value to process | Imperative access | Use two-way binding `@bind` or parameters |
| Fire child control event from parent | Cross-component signaling | Use `@ref` to call public method, or use event callbacks |
| Access sibling controls | Lateral traversal | Use parent as intermediary; communicate via parameters/events |

---

## Common Pitfalls

### Pitfall 1: Assuming @ref Works Like FindControl

`@ref` only works for components and HTML elements in the current component's template. It does not recursively search child components.

```razor
@* Wrong — SearchBox is not a direct child *@
<Container>
    <SearchBox @ref="searchRef" />  @* Won't work *@
</Container>

@* Correct — hold reference to Container, not SearchBox *@
<Container @ref="containerRef" />

@code {
    private Container containerRef;
    
    private SearchBox GetSearchBox() => containerRef.SearchBoxRef;  @* Requires Container to expose it *@
}
```

### Pitfall 2: Forgetting to Check for Null

`FindControl` returns null if not found. Blazor's `@ref` is type-safe, but you must still null-check:

```razor
@code {
    private SearchBox searchRef;

    private void DoSomething()
    {
        if (searchRef != null)
        {
            searchRef.Focus();
        }
    }
}
```

### Pitfall 3: Modifying Control State After FindControl

FindControl returns a control you can modify, but in Blazor, parameters are one-way. Modifying a component via `@ref` bypasses the parameter binding and can cause inconsistency:

```razor
@* Problematic *@
<TextBox @ref="textRef" Value="initialValue" />

@code {
    private TextBox textRef;

    private void BadApproach()
    {
        textRef.Value = "new value";  @* Bypasses the Value parameter binding *@
    }

    private void GoodApproach()
    {
        // Instead, change state in parent and let it flow down
        initialValue = "new value";
        StateHasChanged();  @* Trigger re-render with new Value parameter *@
    }
}
```

---

## See Also

- [User Controls Migration Guide](User-Controls.md) — Full guide on migrating ASCX controls
- [Cascading Parameters and Values](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/cascading-values-and-parameters) — Microsoft docs on cascading parameters
- [Component References with @ref](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle#capture-references-to-components) — Official Blazor documentation
- [Custom Controls Migration Guide](Custom-Controls.md) — Information on BWFC's BaseWebFormsComponent

---

## References

- [Web Forms FindControl method](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.control.findcontrol)
- [INamingContainer interface](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.inamingcontainer)
- [Blazor Event Handling](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/event-handling)
