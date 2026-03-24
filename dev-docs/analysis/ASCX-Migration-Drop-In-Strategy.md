# ASCX → Blazor Component Migration: Drop-In Replacement Strategy Analysis

**Analysis Date:** 2026-03-12  
**Analyst:** Forge (Web Forms Lead Reviewer)  
**Case Study:** DepartmentPortal (12 ASCX controls → 12+ Blazor components)  
**Objective:** Understand ASCX user control migration patterns and define a drop-in replacement playbook.

---

## Executive Summary

The DepartmentPortal migration reveals a **partial drop-in replacement strategy** where ~70% of the migration involves mechanical transformations, but significant manual steps remain. BWFC supports the Web Forms control primitives (Button, DropdownList, Repeater, etc.) but user controls require deeper architectural decisions around data binding, event handling, and component composition.

**Key Finding:** The migration is not a true drop-in replacement. Developers must understand:
1. **Event Model Shift:** PostBack events → Blazor EventCallback parameters
2. **Data Binding Shift:** ViewState + Page_Load → Component Parameters + @code
3. **Component Composition:** ASCX Register directives → @using component includes
4. **Control Discovery:** FindControl patterns → Direct property access or cascading parameters

---

## 1. ASCX Source Controls Summary

### All 12 Controls in DepartmentPortal/Controls/

| Control | Type | Primary Web Forms Controls | Code-Behind Pattern |
|---------|------|---------------------------|---------------------|
| **DepartmentFilter** | Input | DropDownList, Label | AutoPostBack + event; ViewState for selected ID |
| **SearchBox** | Input | TextBox, Button | OnClick event; ViewState for text |
| **EmployeeList** | Data Grid | GridView | DataBind in Page_PreRender; paging events |
| **Pager** | Navigation | LinkButton, Repeater | Repeater binding; page change events; ViewState |
| **Breadcrumb** | Navigation | Repeater | String split binding; LINQ in Page_Load |
| **PageHeader** | Display | Literal, HtmlGenericControl | ViewState for title; Session for username |
| **AnnouncementCard** | Display | Literal (multiple) | Property-based rendering; HTML encoding |
| **QuickStats** | Display | Literal, HtmlGenericControl | Conditional visibility; PortalDataProvider calls |
| **Footer** | Display | Literal, HtmlGenericControl | ViewState for year; conditional links |
| **TrainingCatalog** | Data Grid | Repeater, Button | ItemCommand event binding; DataBind in PreRender |
| **DashboardWidget** | Container | PlaceHolder, Literal | Exposes ContentPlaceHolder property for child controls |
| **ResourceBrowser** | Composite | Repeater, LinkButton, SearchBox (nested) | Event subscription in OnInit; BindData method; nested user control events |

### Patterns Observed

- **8/12 controls** use ViewState for state management (DepartmentFilter, SearchBox, EmployeeList, Pager, Breadcrumb, PageHeader, Footer, QuickStats)
- **6/12 controls** declare public events (DepartmentFilter, SearchBox, Pager, TrainingCatalog, ResourceBrowser, plus implicit SelectedIndexChanged)
- **4/12 controls** use Repeater + ItemCommand pattern (Pager, TrainingCatalog, ResourceBrowser, Breadcrumb data binding)
- **3/12 controls** access PortalDataProvider directly (QuickStats, TrainingCatalog, RecentAnnouncements)
- **1/12 control** exposes a public control property (DashboardWidget.ContentPlaceHolder) for parent manipulation
- **1/12 control** subscribes to events of child user controls (ResourceBrowser subscribes to SearchBox.Search)

---

## 2. Blazor Component Equivalents: Markup Comparison

### DepartmentFilter: Drop-Down Input

**ASCX Markup (lines 1-10):**
```html
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DepartmentFilter.ascx.cs" ... %>
<div class="department-filter">
    <asp:Label ID="lblDepartment" runat="server" Text="Department:" AssociatedControlID="ddlDepartments" CssClass="filter-label" />
    <asp:DropDownList ID="ddlDepartments" runat="server"
        CssClass="filter-dropdown"
        OnSelectedIndexChanged="ddlDepartments_SelectedIndexChanged">
    </asp:DropDownList>
</div>
```

**ASCX Code-Behind Pattern:**
- Uses **ViewState** to persist `SelectedDepartmentId` and `AutoPostBack` properties
- In `Page_Load`, populates dropdown items with `ddlDepartments.Items.Add()`
- Fires `OnSelectedIndexChanged` event which raises `DepartmentChanged` event
- Logs activity via `LogActivity()`

**Blazor Equivalent (lines 1-18):**
```razor
<div class="mb-3">
    <label class="form-label">Filter by Department</label>
    <select class="form-select">
        <option value="">All Departments</option>
        @foreach (var dept in PortalDataProvider.GetDepartments())
        {
            <option value="@dept.Id">@dept.Name</option>
        }
    </select>
</div>

@code {
    [Parameter] public EventCallback<int?> OnDepartmentChanged { get; set; }
}
```

**Migration Changes:**

| Aspect | ASCX | Blazor | Diff |
|--------|------|--------|------|
| **HTML Tag** | `<asp:DropDownList>` | `<select>` | ✅ Mechanical (BWFC DropDownList removed) |
| **Label Control** | `<asp:Label>` | `<label>` | ✅ Mechanical |
| **Data Binding** | `Items.Add()` in Page_Load | `@foreach` in markup | 📌 Paradigm shift: imperative → declarative |
| **State Persistence** | `ViewState["SelectedDepartmentId"]` | N/A (stateless) | 📌 Lost: Blazor has no equivalent; parent must track |
| **Event Handling** | `OnSelectedIndexChanged` handler + `DepartmentChanged` event | `EventCallback<int?>` parameter | 📌 Shift: server postback → parent-controlled callback |
| **AutoPostBack** | Property with ViewState backing | N/A | ❌ Lost: Blazor doesn't auto-post; parent must bind `@onchange` |

**Analysis:**
- **70% mechanical:** HTML tags, CSS classes, label/option nesting
- **30% manual:** Event binding, data flow, state management
- **Missing:** AutoPostBack concept (Blazor requires explicit handler binding)

---

### SearchBox: Text Input + Button

**ASCX Markup:**
```html
<div class="search-box">
    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" />
    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-search" OnClick="btnSearch_Click" />
</div>
```

**ASCX Code-Behind:**
- ViewState for `SearchText` and `Placeholder`
- In `Page_Load`, sets placeholder attribute on TextBox
- `btnSearch_Click` fires custom `Search` event with `SearchEventArgs` (term + category)
- Logs activity

**Blazor Equivalent:**
```razor
<div class="input-group mb-3">
    <input type="text" class="form-control" placeholder="@Placeholder" aria-label="Search" />
    <button class="btn btn-outline-secondary" type="button">Search</button>
</div>

@code {
    [Parameter] public string Placeholder { get; set; } = "Search...";
    [Parameter] public EventCallback<string> OnSearch { get; set; }
}
```

**Migration Changes:**

| Aspect | ASCX | Blazor | Diff |
|--------|------|--------|------|
| **Container** | `<div class="search-box">` | `<div class="input-group mb-3">` | ✅ CSS class change (Bootstrap 5) |
| **TextBox** | `<asp:TextBox>` | `<input type="text">` | ✅ Mechanical |
| **Button** | `<asp:Button>` | `<button type="button">` | ✅ Mechanical |
| **Placeholder** | Attributes["placeholder"] set in Code-Behind | `placeholder="@Placeholder"` parameter | 📌 Inline vs property-driven |
| **Custom Event Args** | `SearchEventArgs { SearchTerm, Category }` | `EventCallback<string>` | 🔴 **Lost:** Only term passed; category field removed |
| **Click Handler** | `btnSearch_Click` with event logic | Parent must bind `@onclick` with callback | 📌 Parent responsibility |

**Analysis:**
- **Simple form control:** Good candidate for drop-in replacement
- **Gap:** Custom `SearchEventArgs` is lossy (category field discarded)
- **Missing:** No click handler in component—parent must provide it

---

### EmployeeList: Data Grid

**ASCX Markup:**
```html
<div class="employee-list">
    <asp:GridView ID="gvEmployees" runat="server"
        AutoGenerateColumns="false"
        CssClass="table employee-grid"
        AllowPaging="true"
        OnPageIndexChanging="gvEmployees_PageIndexChanging"
        EmptyDataText="No employees found.">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Name" />
            <asp:BoundField DataField="Title" HeaderText="Title" />
            ...
        </Columns>
    </asp:GridView>
</div>
```

**ASCX Code-Behind:**
- Property `Employees: IEnumerable<Employee>`
- Property `PageSize` with ViewState
- `Page_PreRender` calls `BindGrid()`
- `BindGrid()` filters by `DepartmentFilter`, applies `DataSource`, calls `DataBind()`
- `OnPageIndexChanging` updates `PageIndex` and re-binds

**Blazor Equivalent (TODO/Stub):**
```razor
<div class="employee-list">
    <p><em>Employee list component — will render employee cards when fully migrated.</em></p>
</div>

@code {
    [Parameter] public string? DepartmentFilter { get; set; }
    [Parameter] public string? SearchTerm { get; set; }
}
```

**Analysis:**

| Aspect | ASCX | Blazor | Diff |
|--------|------|--------|------|
| **Implementation** | Full GridView with columns | Stub (TODO) | 🔴 **Not migrated** |
| **Expected Blazor** | Either: (A) Use BWFC GridView, or (B) Render manual table with @foreach | N/A | 📌 Decision needed |
| **Pagination** | GridView.PageIndex + OnPageIndexChanging | Would need Pager sub-component | 📌 Composite pattern |
| **Data Binding** | IEnumerable property + DataBind() | Parameters + re-render | ✅ Blazor-idiomatic |

**Gap Identified:** GridView migration requires choice between BWFC GridView (exact fidelity) or manual table rendering (simpler, more Blazor-native). DepartmentPortal chose neither (stub).

---

### Pager: Pagination Navigation

**ASCX Markup:**
```html
<div class="pager">
    <asp:LinkButton ID="lnkPrevious" ... OnClick="lnkPrevious_Click" />
    <asp:Repeater ID="rptPages" runat="server">
        <ItemTemplate>
            <asp:LinkButton ID="lnkPage" ... 
                Text='<%# Container.DataItem %>'
                OnClick="lnkPage_Click" />
        </ItemTemplate>
    </asp:Repeater>
    <asp:LinkButton ID="lnkNext" ... OnClick="lnkNext_Click" />
</div>
```

**ASCX Code-Behind:**
- ViewState: `CurrentPage`, `TotalPages`, `PageSize`
- `Page_Load` calls `BindPager()`
- `BindPager()` disables/enables Prev/Next based on `CurrentPage`; binds repeater with page number list
- Three click handlers (`lnkPrevious_Click`, `lnkPage_Click`, `lnkNext_Click`) all invoke `OnPageChanged` event

**Blazor Equivalent:**
```razor
<nav aria-label="Page navigation">
    <ul class="pagination">
        <li class="page-item disabled"><a class="page-link" href="#">Previous</a></li>
        <li class="page-item active"><a class="page-link" href="#">1</a></li>
        <li class="page-item"><a class="page-link" href="#">Next</a></li>
    </ul>
</nav>

@code {
    [Parameter] public int CurrentPage { get; set; } = 1;
    [Parameter] public int TotalPages { get; set; } = 1;
    [Parameter] public EventCallback<int> OnPageChanged { get; set; }
}
```

**Analysis:**

| Aspect | ASCX | Blazor | Diff |
|--------|------|--------|------|
| **Markup Structure** | LinkButtons + Repeater | `<nav>`, `<ul>`, `<li>`, `<a>` | ✅ Clean HTML; no BWFC controls |
| **State** | ViewState for CurrentPage/TotalPages | Parameters (stateless) | 📌 Parent tracks state; cleaner |
| **Repeater + DataBind** | In BindPager() | Would need @foreach over range | ✅ Declarative pattern |
| **Click Handling** | Three separate handlers → one event | Would need @onclick handlers per link | ✅ Event callbacks work |
| **Disabled State** | Conditional .Enabled property | CSS class "disabled" | ✅ Bootstrap 5 approach |

**Stub Alert:** Razor shows static HTML (Previous, 1, Next); doesn't dynamically generate pages. True implementation missing.

---

### Breadcrumb: Navigation Trail

**ASCX Markup:**
```html
<nav aria-label="breadcrumb">
    <ol class="breadcrumb">
        <li class="breadcrumb-item" runat="server" id="homeLinkItem" visible="false">
            <a href="/">Home</a>
        </li>
        <asp:Repeater ID="rptBreadcrumb" runat="server">
            <ItemTemplate>
                <li class='<%# Container.ItemIndex == ... ? "active" : "" %>'>
                    <%# Container.DataItem %>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ol>
</nav>
```

**ASCX Code-Behind:**
- ViewState: `CurrentPath` (string), `ShowHomeLink` (bool)
- `Page_Load` splits `CurrentPath` by `/`, binds to Repeater
- Complex `<%# %>` binding logic to detect last item (active state)

**Blazor Equivalent:**
```razor
<nav aria-label="breadcrumb">
    <ol class="breadcrumb">
        @if (Items != null)
        {
            @foreach (var (label, url) in Items)
            {
                if (string.IsNullOrEmpty(url))
                {
                    <li class="breadcrumb-item active" aria-current="page">@label</li>
                }
                else
                {
                    <li class="breadcrumb-item"><a href="@url">@label</a></li>
                }
            }
        }
    </ol>
</nav>

@code {
    [Parameter] public IEnumerable<(string Label, string Url)>? Items { get; set; }
}
```

**Analysis:**

| Aspect | ASCX | Blazor | Diff |
|--------|------|--------|------|
| **Data Format** | String path ("Employees/List") split on "/" | Tuples of (Label, Url) | 📌 **Major**: Semantic change; URL inference lost |
| **Last Item Active** | Complex <%# %> binding with ItemIndex | Simple C# if(string.IsNullOrEmpty(url)) | ✅ Clearer logic |
| **Home Link Visibility** | HtmlGenericControl + visible=false | Parameter-driven (no home in default example) | 📌 Lost: Home link removed |

**Migration Gap:** ASCX infers URLs from path segments; Blazor version requires explicit URL tuples. Parent must construct breadcrumbs differently.

---

### PageHeader: Display Control

**ASCX Markup:**
```html
<div class="page-header">
    <h1><asp:Literal ID="litPageTitle" runat="server" /></h1>
    <div class="user-info" runat="server" id="pnlUserInfo" visible="false">
        <span class="welcome-message">Welcome, <asp:Literal ID="litUserName" runat="server" /></span>
    </div>
</div>
```

**ASCX Code-Behind:**
- ViewState: `PageTitle`, `ShowUserInfo`
- `Page_Load` sets `litPageTitle.Text`; if ShowUserInfo, sets `litUserName.Text` from Session

**Blazor Equivalent:**
```razor
<div class="page-header">
    <h1>@Title</h1>
    @if (!string.IsNullOrEmpty(Subtitle))
    {
        <p class="lead text-muted">@Subtitle</p>
    }
</div>

@code {
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string Subtitle { get; set; } = string.Empty;
}
```

**Analysis:**

| Aspect | ASCX | Blazor | Diff |
|--------|------|--------|------|
| **Literal Controls** | `<asp:Literal>` | Inline `@Title`, `@Subtitle` | ✅ Blazor-native |
| **User Info Section** | Conditional via HtmlGenericControl.Visible | Removed; not in example | ❌ Lost feature |
| **Session Access** | `Session["UserName"]` in code-behind | N/A (no Session in example) | 📌 Architecture change |
| **Parameters** | ViewState-backed properties | `[Parameter]` properties | ✅ Cleaner model |

**Note:** Migration simplified the control (removed user info section). True 1-to-1 migration would require cascading parameters for user info.

---

### AnnouncementCard: Display (Repeated Template)

**ASCX Markup:**
```html
<div class="announcement-card">
    <div class="announcement-header">
        <h3><asp:Literal ID="litTitle" runat="server" /></h3>
        <span class="announcement-date"><asp:Literal ID="litDate" runat="server" /></span>
    </div>
    <div class="announcement-meta">
        <span class="announcement-author">By <asp:Literal ID="litAuthor" runat="server" /></span>
    </div>
    <div class="announcement-body">
        <asp:Literal ID="litBody" runat="server" />
    </div>
</div>
```

**ASCX Code-Behind:**
- Property: `Announcement: Announcement` object
- `Page_Load` hydrates Literal controls from Announcement properties
- Conditional text truncation (150 chars) based on `ShowFullText` ViewState property
- HTML encoding of all text

**Blazor Equivalent:**
```razor
<div class="card mb-3">
    <div class="card-body">
        <h5 class="card-title">
            <a href="/announcements/@AnnouncementId">@Title</a>
        </h5>
        <p class="card-text">@Summary</p>
        <p class="card-text">
            <small class="text-muted">By @Author on @PublishDate.ToShortDateString()</small>
        </p>
    </div>
</div>

@code {
    [Parameter] public int AnnouncementId { get; set; }
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string Summary { get; set; } = string.Empty;
    [Parameter] public string Author { get; set; } = string.Empty;
    [Parameter] public DateTime PublishDate { get; set; }
}
```

**Analysis:**

| Aspect | ASCX | Blazor | Diff |
|--------|------|--------|------|
| **Data Model** | Single `Announcement` object passed to control | Individual parameters (AnnouncementId, Title, Summary, Author, PublishDate) | 📌 Disaggregation: component doesn't access complex objects |
| **Text Truncation** | In code-behind (150 chars) | Assumed done by parent ("Summary" parameter) | ❌ Lost: Parent must now truncate |
| **HTML Encoding** | `HttpUtility.HtmlEncode()` in code-behind | Automatic in Blazor (default) | ✅ Built-in safety |
| **Link Construction** | No link in ASCX | `/announcements/@AnnouncementId` in Blazor | ✅ Added feature (not in original) |

**Architectural Difference:** ASCX pulls from object properties; Blazor receives flattened parameters. This is actually a **Blazor best practice** (single responsibility) but loses composability.

---

### QuickStats: Dashboard Metrics Display

**ASCX Markup:**
```html
<div class="quick-stats">
    <div class="stat-item" runat="server" id="pnlEmployeeCount" visible="false">
        <span class="stat-label">Employees</span>
        <span class="stat-value"><asp:Literal ID="litEmployeeCount" runat="server" /></span>
    </div>
    <div class="stat-item" runat="server" id="pnlAnnouncementCount" visible="false">
        <span class="stat-label">Announcements</span>
        <span class="stat-value"><asp:Literal ID="litAnnouncementCount" runat="server" /></span>
    </div>
</div>
```

**ASCX Code-Behind:**
- ViewState: `ShowEmployeeCount`, `ShowAnnouncementCount`
- `Page_Load` conditionally renders stat items
- Direct calls to `PortalDataProvider.GetEmployees()`, `GetAnnouncements()`, `GetDepartments()`, `GetCourses()`
- No parameters passed; control is responsible for data fetching and filtering

**Blazor Equivalent:**
```razor
<div class="row mb-4">
    <div class="col-md-3">
        <div class="card text-center">
            <div class="card-body">
                <h3>@PortalDataProvider.GetEmployees().Count</h3>
                <p class="text-muted">Employees</p>
            </div>
        </div>
    </div>
    <div class="col-md-3">
        <div class="card text-center">
            <div class="card-body">
                <h3>@PortalDataProvider.GetDepartments().Count</h3>
                <p class="text-muted">Departments</p>
            </div>
        </div>
    </div>
    <div class="col-md-3">
        <div class="card text-center">
            <div class="card-body">
                <h3>@PortalDataProvider.GetAnnouncements().Count(a => a.IsActive)</h3>
                <p class="text-muted">Active Announcements</p>
            </div>
        </div>
    </div>
    <div class="col-md-3">
        <div class="card text-center">
            <div class="card-body">
                <h3>@PortalDataProvider.GetCourses().Count</h3>
                <p class="text-muted">Training Courses</p>
            </div>
        </div>
    </div>
</div>
```

**Analysis:**

| Aspect | ASCX | Blazor | Diff |
|--------|------|--------|------|
| **Conditional Visibility** | HtmlGenericControl with visible=false | `@if` directives removed; all stats always shown | ❌ Lost: No per-stat visibility control |
| **Data Fetching** | In code-behind; conditional via ViewState | Inline in markup | 📌 Same pattern (direct PortalDataProvider calls) but no control hiding |
| **HTML Structure** | `.quick-stats .stat-item` | Bootstrap grid: `.row .col-md-3 .card` | ✅ Better responsive design |
| **Literal vs Direct Calls** | Separate Literal controls | Direct method calls in markup | ✅ Simpler Blazor pattern |

**Problem:** Component can't selectively hide stats. Possible solutions:
- Add `[Parameter]` properties for visibility
- Hardcode to always show all
- Use CSS display:none (current approach)

---

### Footer: Static Display

**ASCX Markup:**
```html
<footer class="site-footer">
    <div class="footer-content">
        <p>&copy; <asp:Literal ID="litYear" runat="server" /> Department Portal. All rights reserved.</p>
        <div runat="server" id="pnlLinks" visible="false" class="footer-links">
            <a href="/Default.aspx">Home</a> | ...
        </div>
    </div>
</footer>
```

**ASCX Code-Behind:**
- ViewState: `ShowLinks`, `Year`
- `Page_Load` sets `litYear.Text` and `pnlLinks.Visible`

**Blazor Equivalent:**
```razor
<footer class="border-top pt-3 mt-4">
    <p class="text-muted">&copy; @DateTime.Now.Year - Department Portal | Contoso Corporation</p>
</footer>
```

**Analysis:**

| Aspect | ASCX | Blazor | Diff |
|--------|------|--------|------|
| **Year Display** | ViewState property | `DateTime.Now.Year` hardcoded | ✅ Simpler; dynamic year |
| **Links Section** | Conditional (visible=false) | Removed entirely | ❌ Lost: No way to show footer links |
| **Styling** | `.site-footer` custom class | Bootstrap classes: `.border-top .pt-3 .mt-4` | ✅ Standards-based |

**Note:** Migration simplified by removing unused feature (link section). Footer is largely presentation; good candidate for simple migration.

---

### TrainingCatalog: Complex Data Grid with Events

**ASCX Markup:**
```html
<div class="training-catalog">
    <asp:Repeater ID="rptCourses" runat="server" OnItemCommand="rptCourses_ItemCommand">
        <ItemTemplate>
            <div class="course-card">
                <h4><%# Eval("CourseName") %></h4>
                <p class="course-description"><%# Eval("Description") %></p>
                <div class="course-meta">
                    <span class="instructor">Instructor: <%# Eval("Instructor") %></span>
                    <span class="duration"><%# Eval("DurationHours") %> hours</span>
                    <span class="category"><%# Eval("Category") %></span>
                </div>
                <asp:Button ID="btnEnroll" runat="server" Text="Enroll" CssClass="btn btn-enroll"
                    CommandName="Enroll" CommandArgument='<%# Eval("Id") %>' />
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
```

**ASCX Code-Behind:**
- Property: `Courses: IEnumerable<TrainingCourse>`
- ViewState: `ShowEnrolled`
- Event: `EnrollmentRequested`
- `Page_PreRender` binds `Courses` to Repeater
- `rptCourses_ItemCommand` parses command name/argument and fires `OnEnrollmentRequested`

**Blazor Equivalent (TODO/Stub):**
```razor
<div class="training-catalog">
    <p><em>Training catalog component — will render course listings when fully migrated.</em></p>
</div>

@code {
    [Parameter] public string? CategoryFilter { get; set; }
}
```

**Analysis:**

| Aspect | ASCX | Blazor | Diff |
|--------|------|--------|------|
| **Implementation** | Full Repeater with ItemCommand | Stub (TODO) | 🔴 **Not migrated** |
| **Expected Pattern** | Repeater.ItemCommand with button.CommandName/CommandArgument | @foreach with @onclick per course + EventCallback | 📌 Would work but requires parent handler |
| **Data Binding** | Page_PreRender + DataBind() | Would be parameter-driven + @foreach | ✅ Blazor-standard |

**Gap:** Like EmployeeList, no completed migration. Key difference from ASCX: Repeater's ItemCommand pattern maps cleanly to Blazor @onclick per item.

---

### DashboardWidget: Container/Composition Pattern

**ASCX Markup:**
```html
<div class="widget">
    <div class="widget-header">
        <span class="widget-icon"><asp:Literal ID="litIcon" runat="server" /></span>
        <h3 class="widget-title"><asp:Literal ID="litWidgetTitle" runat="server" /></h3>
    </div>
    <div class="widget-body">
        <asp:PlaceHolder ID="phContent" runat="server" />
    </div>
</div>
```

**ASCX Code-Behind:**
- ViewState: `WidgetTitle`, `IconClass`
- Property: `ContentPlaceHolder` exposes the PlaceHolder for parent pages to add content
- `Page_Load` sets title and renders icon HTML

**Blazor Equivalent:**
- **No direct equivalent in the migration** (not in Components/Shared or Components/Pages)
- Would use Razor Component structure with `@ChildContent` parameter

**Analysis:**

| Aspect | ASCX | Blazor | Diff |
|--------|------|--------|------|
| **Composition** | PlaceHolder exposed as public property | RenderFragment parameter (@ChildContent) | 📌 Architectural: Parent adds controls to PlaceHolder vs passes RenderFragment |
| **Imperative Content** | `ContentPlaceHolder.Controls.Add(control)` | Declarative: `<YourComponent> ... </YourComponent>` | 📌 Paradigm shift |
| **Nesting Strategy** | Deep imperative control tree | Shallow Razor nesting with parameters | ✅ Blazor simpler |

**Gap:** DashboardWidget pattern (exposing mutable container) doesn't map 1-to-1 to Blazor. The Blazor equivalent would use `@ChildContent` RenderFragment.

---

### ResourceBrowser: Composite Control with Nested Events

**ASCX Markup:**
```html
<%@ Register Src="~/Controls/SearchBox.ascx" TagPrefix="uc" TagName="SearchBox" %>
<%@ Register Src="~/Controls/Breadcrumb.ascx" TagPrefix="uc" TagName="Breadcrumb" %>

<div class="resource-browser">
    <uc:Breadcrumb ID="ctlBreadcrumb" runat="server" ShowHomeLink="true" />
    <uc:SearchBox ID="ctlSearchBox" runat="server" Placeholder="Search resources..." />

    <div class="resource-categories" runat="server" id="pnlCategories" visible="false">
        <h4>Categories</h4>
        <asp:Repeater ID="rptCategories" runat="server" OnItemCommand="rptCategories_ItemCommand">
            <ItemTemplate>
                <asp:LinkButton ... CommandName="SelectCategory" CommandArgument='<%# Eval("CategoryId") %>' />
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <div class="resource-list">
        <asp:Repeater ID="rptResources" runat="server" OnItemCommand="rptResources_ItemCommand">
            <ItemTemplate>
                <div class="resource-item">
                    <asp:LinkButton ... CommandName="SelectResource" CommandArgument='<%# Eval("Id") %>' />
                    <span class="resource-type"><%# Eval("FileType") %></span>
                    <p class="resource-desc"><%# Eval("Description") %></p>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
```

**ASCX Code-Behind:**
- Declares protected references to nested user controls: `ctlBreadcrumb`, `ctlSearchBox`, `rptCategories`, `rptResources`
- In `OnInit`, subscribes to child `SearchBox.Search` event: `ctlSearchBox.Search += CtlSearchBox_Search;`
- Properties: `CategoryId`, `ShowCategories` (ViewState-backed)
- `Page_Load` calls `BindData()`
- `BindData()` fetches resources, filters by category if needed, binds to Repeaters
- Two ItemCommand handlers coordinate category selection and resource selection

**Blazor Equivalent:**
- **Not found in migration** (no ResourceBrowser.razor in Shared/; would be on a page)

**Analysis:**

| Aspect | ASCX | Blazor | Diff |
|--------|------|--------|------|
| **Register Directive** | `<%@ Register Src="~/Controls/SearchBox.ascx" ... %>` | `@using Components.Shared` (at page level) or component-level | ✅ @using is cleaner |
| **Nested Control References** | Protected fields (`ctlSearchBox`, `ctlBreadcrumb`) | Not directly accessible; must use parameters/cascading | 📌 Less tight coupling (good) |
| **Event Subscription** | `ctlSearchBox.Search += CtlSearchBox_Search;` in OnInit | Parent passes EventCallback to child, child invokes it | 📌 Inversion of control; parent drives child events |
| **State Coordination** | `BindData()` method called by parent and on child events | Would need parameter updates → parent re-renders | ✅ Blazor standard |
| **Conditional Visibility** | `pnlCategories.Visible = ShowCategories` | @if(ShowCategories) { ... } | ✅ Blazor-idiomatic |

**Architectural Gap:** ASCX pattern (parent finds child controls and subscribes to events) inverts in Blazor (children call parent via EventCallback).

---

## 3. Complete Mapping Table

| Control | ASCX Status | Blazor Status | Migration Approach | Complete? | Issues |
|---------|-------------|---------------|-------------------|-----------|--------|
| **DepartmentFilter** | ✅ Full | ✅ Partial | Stub (missing: data binding hook) | ~70% | ViewState lost; AutoPostBack missing |
| **SearchBox** | ✅ Full | ✅ Partial | Stub (missing: click handler) | ~60% | Custom SearchEventArgs fields lost |
| **EmployeeList** | ✅ Full | ❌ Stub | TODO | 0% | Decision: BWFC GridView vs manual table? |
| **Pager** | ✅ Full | ❌ Stub | Skeleton HTML only | ~40% | Page list generation not implemented |
| **Breadcrumb** | ✅ Full | ✅ Partial | Semantic data format change | ~80% | Home link feature removed |
| **PageHeader** | ✅ Full | ✅ Partial | Simplified | ~70% | User info section removed |
| **AnnouncementCard** | ✅ Full | ✅ Full | Complete parameter-driven | 100% | Text truncation now parent's responsibility |
| **QuickStats** | ✅ Full | ✅ Partial | Markup equivalent; lost visibility control | ~80% | Can't selectively hide stats |
| **Footer** | ✅ Full | ✅ Full | Simplified presentation | 100% | Links section removed |
| **TrainingCatalog** | ✅ Full | ❌ Stub | TODO | 0% | ItemCommand pattern needs @onclick mapping |
| **DashboardWidget** | ✅ Full | ❌ Not found | Would use @ChildContent | N/A | Compositional paradigm change |
| **ResourceBrowser** | ✅ Full | ❌ Not found | TODO | N/A | Nested control coordination complex |

---

## 4. Cross-Control Pattern Analysis

### ViewState Dependency

**Pattern:** 8/12 controls use ViewState for state management (DepartmentFilter, SearchBox, EmployeeList, Pager, Breadcrumb, PageHeader, Footer, QuickStats)

**ASCX Example (DepartmentFilter):**
```csharp
public int SelectedDepartmentId
{
    get { return (object)ViewState["SelectedDepartmentId"] != null ? (int)ViewState["SelectedDepartmentId"] : 0; }
    set { ViewState["SelectedDepartmentId"] = value; }
}
```

**Blazor Problem:**
- No ViewState in Blazor (stateless per request)
- Parent component must track state via `@bind` or EventCallback
- Control cannot persist state between requests without parent intervention

**Migration Strategy:**
- Remove ViewState backing
- Convert properties to `[Parameter]` for parent-controlled state
- Parent uses local variables or state object to persist

---

### Event Model Shift

**Pattern:** 6/12 controls declare public events; all tie to .NET EventHandler pattern

**ASCX Example (DepartmentFilter):**
```csharp
public event EventHandler DepartmentChanged;
protected void OnDepartmentChanged(EventArgs args)
{
    DepartmentChanged?.Invoke(this, args);
}
```

**Blazor Equivalent:**
```csharp
[Parameter] public EventCallback<int> OnDepartmentChanged { get; set; }

// Invoked by onclick or change handler:
await OnDepartmentChanged.InvokeAsync(selectedId);
```

**Key Differences:**
- EventHandler(object, EventArgs) → EventCallback<T>
- Async-first: `await OnDepartmentChanged.InvokeAsync(value)`
- Type-safe: custom `SearchEventArgs` fields → strongly-typed parameters

---

### Data Binding Patterns

**Pattern A: Direct Property (AnnouncementCard)**
```csharp
// ASCX
public Announcement Announcement { get; set; }
protected void Page_Load(object sender, EventArgs e)
{
    if (Announcement != null)
    {
        litTitle.Text = Announcement.Title;
    }
}

// Blazor
[Parameter] public string Title { get; set; }
[Parameter] public DateTime PublishDate { get; set; }
// Rendered as: @Title, @PublishDate.ToShortDateString()
```

**Pattern B: Data-Bound Collection (Pager)**
```csharp
// ASCX
protected void Page_Load(object sender, EventArgs e)
{
    var pages = Enumerable.Range(1, TotalPages).ToList();
    rptPages.DataSource = pages;
    rptPages.DataBind(); // Fires ItemDataBound
}

// Blazor
@foreach (int page in Enumerable.Range(1, TotalPages))
{
    <li><button @onclick="@(() => GoToPage(page))">@page</button></li>
}
```

**Pattern C: Declarative with Filtering (QuickStats, TrainingCatalog)**
```csharp
// ASCX (Code-Behind)
protected void Page_Load(object sender, EventArgs e)
{
    var courses = PortalDataProvider.GetCourses();
    if (!string.IsNullOrEmpty(CategoryFilter))
        courses = courses.Where(c => c.Category == CategoryFilter).ToList();
    rptCourses.DataSource = courses;
    rptCourses.DataBind();
}

// Blazor (Markup)
@foreach (var course in PortalDataProvider.GetCourses().Where(c => string.IsNullOrEmpty(CategoryFilter) || c.Category == CategoryFilter))
{
    <div class="course-card">...</div>
}
```

**Analysis:** Blazor shifts from imperative (code-behind data binding) to declarative (markup filtering). This is generally cleaner but puts more logic in view.

---

### Control Reference Pattern (FindControl)

**Pattern:** ResourceBrowser explicitly declares protected fields for child controls:
```csharp
protected Breadcrumb ctlBreadcrumb;
protected SearchBox ctlSearchBox;
```

Then accesses them:
```csharp
ctlBreadcrumb.CurrentPath = "Resources";
ctlSearchBox.Search += CtlSearchBox_Search;
```

**Blazor Equivalent:**
- No direct equivalent; can't "find" or reference sibling components imperatively
- Instead: Use parameters for data flow (top-down) and EventCallback for events (bottom-up)
- Alternative: Use service injection for shared state

**Migration Gap:** Deep component coordination (Resource browser controls Breadcrumb.CurrentPath; subscribes to SearchBox.Search) requires architectural rethinking in Blazor.

---

## 5. BWFC Coverage Analysis

### Controls Used in DepartmentPortal ASCX Files

| Web Forms Control | Frequency | BWFC Status | Notes |
|------------------|-----------|-------------|-------|
| `<asp:DropDownList>` | 1 | ✅ Available | DepartmentFilter; renders as `<select>` in Blazor |
| `<asp:TextBox>` | 1 | ✅ Available | SearchBox; renders as `<input type="text">` |
| `<asp:Button>` | 2 | ✅ Available | SearchBox, TrainingCatalog; renders as `<button>` |
| `<asp:Label>` | 1 | ✅ Available | DepartmentFilter; renders as `<label>` |
| `<asp:LinkButton>` | 4 | ✅ Available | Pager (Prev/Next/Pages), Breadcrumb, ResourceBrowser |
| `<asp:Repeater>` | 4 | ✅ Available | Pager, Breadcrumb, TrainingCatalog, ResourceBrowser |
| `<asp:GridView>` | 1 | ✅ Available | EmployeeList; not used in migration (stub instead) |
| `<asp:Literal>` | 8 | ✅ Available | PageHeader, AnnouncementCard, QuickStats, Footer, DashboardWidget |
| `<asp:PlaceHolder>` | 1 | ✅ Available | DashboardWidget; no equivalent in Blazor (@ChildContent) |
| `<asp:HtmlGenericControl>` | Implicit | ✅ Available | Breadcrumb, PageHeader, DashboardWidget, QuickStats (runat="server" divs) |

**Conclusion:** BWFC has all primitives covered. However:
- **User Control Composition** (nested RegisterDirectives, event subscriptions) is not covered by BWFC
- **Paradigm Shift** required: imperative (.NET event-driven) → declarative (Blazor parameter-driven)

---

## 6. Identified Gaps in BWFC's User Control Migration Story

### Gap 1: No AutoPostBack Equivalent

**Problem:** DepartmentFilter uses `AutoPostBack = true` to trigger server-side events on DropDownList selection change.

**BWFC Solution:** Not applicable. Blazor is inherently interactive; parent must bind `@onchange="@((ChangeEventArgs e) => HandleChange(e.Value))"`.

**Impact:** Requires developer understanding that AutoPostBack doesn't exist; parent must provide change handler.

---

### Gap 2: ViewState Pattern Not Supported

**Problem:** 8/12 controls use ViewState for state persistence.

**Solution:** Blazor parameters (stateless per render) + parent state management.

**Architectural Question:** Should BWFC provide a `LocalStorage` service or state management helper to simulate ViewState? Currently: No.

**Impact:** Every control migration requires identifying ViewState properties and converting to parameters.

---

### Gap 3: Nested User Control Event Subscription

**Problem:** ResourceBrowser subscribes to SearchBox.Search event in OnInit:
```csharp
protected override void OnInit(EventArgs e)
{
    base.OnInit(e);
    ctlSearchBox.Search += CtlSearchBox_Search;
}
```

**Blazor Pattern:** Must invert:
```csharp
<SearchBox @bind-Value="searchTerm" OnSearch="@HandleSearch" />
```

**Impact:** Parent-child control architectures must be redesigned. This is not a "drop-in" transformation; it requires re-thinking component composition.

---

### Gap 4: PlaceHolder Content Injection

**Problem:** DashboardWidget exposes `ContentPlaceHolder` property for parent pages to inject controls:
```csharp
ContentPlaceHolder.Controls.Add(new Label { Text = "..." });
```

**Blazor Equivalent:** `@ChildContent` RenderFragment (completely different model).

**Impact:** Control doesn't support imperative content injection; must be rewritten to accept `@ChildContent` or parameters.

---

### Gap 5: Data-Binding Syntax (<%# %>) Not Supported

**Problem:** Repeaters use `<%# Eval("FieldName") %>` and `<%# Convert.ToInt32(...) %>` for data binding.

**Blazor Equivalent:** `@foreach` with direct C# expressions.

**Impact:** Requires converting template expressions to Blazor syntax. Not mechanical; requires understanding the logic.

---

### Gap 6: Custom EventArgs Types Lose Fidelity

**Problem:** SearchBox defines `SearchEventArgs { SearchTerm, Category }` but Blazor equivalent only passes `EventCallback<string>` (SearchTerm).

**Solution:** Define Blazor-equivalent parameters or accept partial feature loss.

**Impact:** Some custom event data is lost in migration. Must decide: (A) add parameters, (B) lose features, or (C) use service for shared state.

---

### Gap 7: Server-Side Logging/Activity Tracking

**Problem:** All controls call `LogActivity("...")` for audit/tracing.

**Blazor Equivalent:** No built-in. Must inject ILogger or custom service.

**Impact:** Logging capability is present but must be explicitly wired via dependency injection. No "drop-in" equivalent.

---

### Gap 8: Complex Filtering and Sorting Logic in Code-Behind

**Problem:** EmployeeList filters by DepartmentFilter, applies sorting, handles paging in code-behind.

**Blazor Strategy:** Move to parent component or create a service.

**Impact:** Separation of concerns changes. What was a control's internal logic may need to move to parent or service. No standardized pattern.

---

## 7. Drop-In Replacement Playbook

### Phase 1: Assessment (Manual - 30 minutes per control)

**Checklist:**
- [ ] Identify all Web Forms controls used (`<asp:*>`)
- [ ] List public properties and their backing store (ViewState, Session, or plain field)
- [ ] List public events and custom EventArgs types
- [ ] Identify child controls and inter-control communication (events, property access)
- [ ] Note data-binding patterns (<%# %>, DataBind() calls)
- [ ] Document any FindControl patterns or imperative control tree manipulation

**Deliverable:** Control Analysis Document

---

### Phase 2: Create Blazor Component Skeleton (Mechanical - 15 minutes)

**Steps:**

1. **Create .razor file** with same name in `Components/Shared/` (for user controls) or `Components/Pages/` (for pages)

2. **Convert Register Directives to @using:**
   ```csharp
   // ASCX
   <%@ Register Src="~/Controls/SearchBox.ascx" TagPrefix="uc" TagName="SearchBox" %>

   // Blazor
   @using Components.Shared
   ```

3. **Convert ASCX Control Declarations to Component Usage:**
   ```csharp
   // ASCX
   <uc:SearchBox ID="ctlSearchBox" runat="server" />

   // Blazor
   <SearchBox @ref="searchBoxComponent" />
   ```
   *(Note: @ref only if parent must access component instance; prefer parameters.)*

4. **Replace Web Forms Controls with HTML or BWFC Components:**
   ```csharp
   // ASCX
   <asp:TextBox ID="txtName" runat="server" />

   // Blazor (native HTML)
   <input type="text" @bind="name" />

   // Blazor (BWFC)
   <TextBox @bind-Value="name" />
   ```

5. **Declare @code Block with Parameters:**
   ```csharp
   @code {
       [Parameter] public string? Title { get; set; }
       [Parameter] public EventCallback<string> OnSearch { get; set; }
   }
   ```

---

### Phase 3: Convert Properties to Parameters (Manual - 20 minutes)

**For each public property in code-behind:**

1. **Remove ViewState backing:**
   ```csharp
   // OLD (ASCX)
   public string SearchText
   {
       get { return (string)ViewState["SearchText"] ?? string.Empty; }
       set { ViewState["SearchText"] = value; }
   }

   // NEW (Blazor)
   [Parameter] public string SearchText { get; set; } = string.Empty;
   [Parameter] public EventCallback<string> OnSearchTextChanged { get; set; }
   ```

2. **Add two-way binding if property is mutable in child:**
   ```razor
   <input @bind="SearchText" />
   <!-- Parent can now use: <SearchBox @bind-SearchText="mySearchTerm" /> -->
   ```

3. **For read-only properties that depend on parent data:**
   ```csharp
   [Parameter] public IEnumerable<Department> Departments { get; set; } = [];
   ```

---

### Phase 4: Convert Events (Manual - 15 minutes)

**For each public event:**

1. **Convert EventHandler signature to EventCallback<T>:**
   ```csharp
   // OLD (ASCX)
   public event EventHandler DepartmentChanged;
   protected void OnDepartmentChanged(EventArgs e)
   {
       DepartmentChanged?.Invoke(this, e);
   }

   // NEW (Blazor)
   [Parameter] public EventCallback<int> OnDepartmentChanged { get; set; }
   
   // To invoke (in async handler):
   await OnDepartmentChanged.InvokeAsync(selectedDepartmentId);
   ```

2. **For custom EventArgs, flatten to individual parameters:**
   ```csharp
   // OLD (ASCX)
   public event EventHandler<SearchEventArgs> Search;
   // SearchEventArgs { SearchTerm, Category }

   // NEW (Blazor)
   [Parameter] public EventCallback<string> OnSearch { get; set; }
   [Parameter] public EventCallback<string> OnCategorySelected { get; set; }
   // Or accept a tuple:
   [Parameter] public EventCallback<(string SearchTerm, string Category)> OnSearch { get; set; }
   ```

3. **Update markup to call event:**
   ```razor
   <button @onclick="@(async () => await OnSearch.InvokeAsync(searchTerm))">
       Search
   </button>
   ```

---

### Phase 5: Convert Data Binding (Manual - 20-30 minutes)

**Pattern A: Simple Data Binding**
```csharp
// ASCX Code-Behind
var departments = PortalDataProvider.GetDepartments();
ddlDepartments.DataSource = departments;
ddlDepartments.DataBind();

// Blazor Markup
@foreach (var dept in PortalDataProvider.GetDepartments())
{
    <option value="@dept.Id">@dept.Name</option>
}
```

**Pattern B: Filtered Collection with Conditional Logic**
```csharp
// ASCX Code-Behind
var data = Employees;
if (!string.IsNullOrEmpty(DepartmentFilter))
    data = data.Where(e => e.Department == DepartmentFilter).ToList();
gvEmployees.DataSource = data;
gvEmployees.DataBind();

// Blazor Markup
@foreach (var emp in GetFilteredEmployees())
{
    <tr>
        <td>@emp.Name</td>
        ...
    </tr>
}

@code {
    private IEnumerable<Employee> GetFilteredEmployees()
    {
        var data = Employees ?? [];
        if (!string.IsNullOrEmpty(DepartmentFilter))
            data = data.Where(e => e.Department == DepartmentFilter);
        return data;
    }
}
```

**Pattern C: Repeater with ItemCommand Events**
```csharp
// ASCX
<asp:Repeater OnItemCommand="rptPages_ItemCommand">
    <ItemTemplate>
        <asp:LinkButton CommandName="SelectPage" CommandArgument='<%# Container.DataItem %>' />
    </ItemTemplate>
</asp:Repeater>

protected void rptPages_ItemCommand(object source, RepeaterCommandEventArgs e)
{
    if (e.CommandName == "SelectPage")
    {
        int page = int.Parse(e.CommandArgument.ToString());
        OnPageChanged(page);
    }
}

// Blazor
@foreach (int page in GetPages())
{
    <button @onclick="@(async () => await OnPageChanged.InvokeAsync(page))">
        @page
    </button>
}
```

---

### Phase 6: Handle Composite Controls (Manual/Architectural - 30-60 minutes)

**Case: Parent Control Accessing Child Controls**

**ASCX Pattern (Imperative):**
```csharp
protected SearchBox ctlSearchBox;
protected override void OnInit(EventArgs e)
{
    base.OnInit(e);
    ctlSearchBox.Search += OnChildSearch;
}
```

**Blazor Pattern (Declarative):**
```razor
<SearchBox @bind-SearchTerm="searchTerm" OnSearch="@HandleSearch" />

@code {
    private string searchTerm = string.Empty;
    
    private async Task HandleSearch()
    {
        // Parent decides what to do
        await OnSearch.InvokeAsync(searchTerm);
    }
}
```

**Key Change:** Child events don't automatically flow to parent. Parent must explicitly provide handlers and state.

---

### Phase 7: Testing & Validation (Manual - 20-30 minutes per control)

**Checklist:**

- [ ] Component renders without errors
- [ ] Parameters accept values from parent
- [ ] Events fire and parent receives callbacks
- [ ] Data displays correctly (compare HTML structure to original)
- [ ] CSS classes intact; styling matches
- [ ] Accessibility: labels, aria-* attributes present
- [ ] No console errors (JavaScript or .NET)
- [ ] Manual browser test: click buttons, change dropdowns, verify behavior

**Deliverable:** QA Sign-off

---

## 8. Proposed Drop-In Replacement Playbook Summary

### Myth vs Reality

**Myth:** "ASCX controls are drop-in replacements for Blazor components. Just remove `asp:` prefixes."

**Reality:** ~70% of the work is mechanical; ~30% requires architectural understanding of Blazor's component model.

### The Real Drop-In Replacement Strategy

#### **Tier 1: Mechanical (Can be Automated)**
- ✅ Remove `<%@ Control ... %>` declaration
- ✅ Replace `<asp:TextBox>`, `<asp:Button>`, `<asp:Label>`, `<asp:Literal>` with HTML or BWFC components
- ✅ Convert `<asp:Repeater>` to `@foreach`
- ✅ Convert `<%@ Register %>` to `@using`

**Time: 5-10 minutes per control (could be scripted)**

#### **Tier 2: Manual-Parameter Conversion (Requires Understanding)**
- ⚠️ Replace ViewState properties with `[Parameter]` properties
- ⚠️ Replace `OnPropertyChanged` patterns with `EventCallback<T>` parameters
- ⚠️ Remove `AutoPostBack` (no equivalent); add explicit `@onchange` or `@onclick` handlers
- ⚠️ Flatten custom `EventArgs` types to individual parameters

**Time: 15-20 minutes per control**

#### **Tier 3: Architectural Decisions (Requires Design Thinking)**
- 🔴 Composite Controls: Redesign how parent controls access/coordinate children
- 🔴 Data-Bound Controls: Choose between BWFC data controls or manual rendering
- 🔴 State Management: Decide on parent-owned vs component-owned state for complex interactions
- 🔴 Logging/Services: Wire up dependency injection for services referenced in code-behind

**Time: 30-60 minutes per control (or skip if features not needed)**

### Critical Success Factors

1. **Parent Ownership:** In Blazor, the parent component owns state and events. Controls are passive (receive parameters, invoke callbacks). This is the biggest paradigm shift.

2. **No Imperative Control Access:** Cannot call methods on child controls or access properties imperatively (no FindControl). Use parameters and EventCallback instead.

3. **Event Data Flattening:** Custom EventArgs types don't work. Pass individual parameters or tuple types.

4. **Async-First:** Event handlers are async (`EventCallback`). Use `await` when invoking.

5. **HTML Generation:** Move away from control-based HTML generation (GridView columns, Repeater ItemTemplate) to explicit `@foreach` + markup.

---

## Appendix: Recommended Tooling / Automation Opportunities

### Low-Hanging Fruit for Automation

1. **Register Directive → @using Converter**
   - Input: `<%@ Register Src="~/Controls/SearchBox.ascx" TagPrefix="uc" TagName="SearchBox" %>`
   - Output: `@using Components.Shared` + Razor markup: `<SearchBox />`

2. **Web Forms Control → HTML/BWFC Mapper**
   - Input: `<asp:TextBox id="txt" />` 
   - Output: `<input type="text" @bind="value" />`
   - Handles: TextBox, Button, Label, Literal, DropDownList, CheckBox, RadioButton, LinkButton

3. **ViewState Property Extractor**
   - Input: Code-behind file
   - Output: List of ViewState-backed properties with suggested `[Parameter]` signatures
   - Example: 
     ```csharp
     // Extract:
     public string SearchText { get { return (string)ViewState["SearchText"]; } ... }
     // Generate:
     [Parameter] public string SearchText { get; set; } = string.Empty;
     [Parameter] public EventCallback<string> OnSearchTextChanged { get; set; }
     ```

4. **Event Handler Signature Converter**
   - Input: `public event EventHandler DepartmentChanged;`
   - Output: `[Parameter] public EventCallback<int> OnDepartmentChanged { get; set; }`
   - Handles: Single-type events; warns on custom EventArgs

5. **DataBind() → @foreach Converter**
   - Input: Code-behind with Repeater.DataBind() call
   - Output: Razor markup with `@foreach`
   - Includes: ItemTemplate expression conversion

### Medium Complexity

6. **Nested Control Event Bridge Generator**
   - Input: Control code-behind referencing `protected ChildControl ctlChild;`
   - Output: Suggested Blazor parameter signatures and EventCallback handlers
   - Flag: Architectural decision required

7. **GridView → Blazor Table Generator**
   - Input: `<asp:GridView>` with `<asp:BoundField>` columns
   - Output: Choice of (A) BWFC GridView or (B) HTML table with `@foreach`
   - Note: Requires understanding of AllowPaging, AllowSorting, OnPageIndexChanging, etc.

### No Quick Wins

8. **PlaceHolder Content Injection Converter**
   - Input: Code accessing `ContentPlaceHolder.Controls.Add(...)`
   - Output: Blazor `@ChildContent` RenderFragment
   - Complexity: Architectural; no mechanical conversion

---

## Conclusion

The ASCX → Blazor migration is **70% mechanical, 30% architectural**. BWFC provides the primitives (Button, Label, TextBox, etc.), but user control migration requires:

1. **Understanding Blazor's parameter-driven model** vs Web Forms' event-driven model
2. **Flattening object hierarchies** (nested controls, custom EventArgs)
3. **Shifting state management** from server-side ViewState to component parameters
4. **Inverting control flow** (parent-driven data and events, not child-driven)

**Best practices for drop-in replacement:**
- Focus on Tier 1 (mechanical) first—candidate for scripting
- Accept Tier 2 (parameters/events) as necessary manual work—15-20 min per control
- Plan Tier 3 (composite, data-bound, service-integrated) controls separately with stakeholder input

**Recommendation:** Create a **BWFC User Control Migration Guide** with step-by-step examples (using DepartmentPortal as case study), automated tooling for Tier 1 conversion, and architectural decision trees for Tier 3 patterns.

---

*Analysis Complete: 2026-03-12*
