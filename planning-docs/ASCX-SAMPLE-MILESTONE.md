# ASCX Sample Application Milestone

## Executive Summary

**Purpose:** Create a .NET Framework 4.8 sample application that prominently features ASCX user controls, custom server controls (WebControl/CompositeControl subclasses), and custom base classes to test migration toolkit coverage and demonstrate real-world Web Forms patterns.

**Gap Analysis:** Current samples (BeforeWebForms, WingtipToys, ContosoUniversity) have minimal ASCX usage (only trivial ViewSwitcher controls) and no custom base classes or custom server controls. This leaves significant migration patterns untested, particularly programmatic control creation, templating, postback handling, and data binding at the custom control level.

**Target:** A complete, realistic Web Forms application with 8-12 reusable ASCX controls, 7 custom server controls covering diverse patterns (Control, WebControl, CompositeControl, Templated, ViewState/Postback, custom events, data-bound), custom Page/MasterPage/UserControl base classes, and ~15-20 pages that consume these controls.

---

## 1. Application Concept

**Name:** DepartmentPortal

**Description:** An internal HR/IT department portal for managing employee information, announcements, training, and resources. This type of application naturally requires many reusable UI components and shared page behaviors.

**Why this concept:**
- **Real-world relevance:** Enterprise IT departments actually build these
- **Component-rich:** Dashboard widgets, data grids, search boxes, filters, navigation
- **Base class justification:** Shared authentication, audit logging, theme management
- **Data-driven:** Uses EF6 with SQL Server, realistic CRUD operations
- **Session usage:** User preferences, shopping cart-like training enrollment

**Domain model:**
- Employees (directory listing)
- Announcements (news feed)
- Training courses (catalog with enrollment)
- Resources (document library with categories)
- Departments (organizational structure)

---

## 2. ASCX User Controls

### 2.1 Simple Display Controls

#### **Breadcrumb.ascx**
- **Purpose:** Navigation breadcrumb trail
- **Properties:** `CurrentPath` (string), `ShowHomeLink` (bool)
- **Output:** `<nav>` with breadcrumb links
- **Pattern:** Read-only display control with property configuration

#### **PageHeader.ascx**
- **Purpose:** Standard page header with title, user greeting, logo
- **Properties:** `PageTitle` (string), `ShowUserInfo` (bool)
- **Session access:** Reads `Session["UserName"]` for greeting
- **Output:** `<div class="page-header">` with structured markup
- **Pattern:** Control with Session access, conditional rendering

#### **Footer.ascx**
- **Purpose:** Standard footer with copyright, links, timestamp
- **Properties:** `ShowLinks` (bool), `Year` (int)
- **Output:** `<footer>` element
- **Pattern:** Simple property-driven control

### 2.2 Data-Bound Controls

#### **AnnouncementCard.ascx**
- **Purpose:** Displays a single announcement with title, date, author, body
- **Properties:** `Announcement` (object), `ShowFullText` (bool)
- **Output:** `<div class="announcement-card">` with structured content
- **Pattern:** Data-bound control with object property, uses ViewState to store ShowFullText state across postbacks

#### **EmployeeList.ascx**
- **Purpose:** Displays a list/grid of employees with filtering
- **Properties:** `Employees` (IEnumerable), `DepartmentFilter` (string), `PageSize` (int)
- **Internal controls:** GridView with paging
- **Output:** GridView HTML structure
- **Pattern:** Complex data-bound control with nested ASP.NET controls, ViewState for filter state

#### **TrainingCatalog.ascx**
- **Purpose:** Displays available training courses with enrollment button
- **Properties:** `Courses` (IEnumerable), `ShowEnrolled` (bool)
- **Events:** `EnrollmentRequested` (custom event with course ID)
- **Output:** Repeater-based course cards
- **Pattern:** Data-bound control with custom event, nested controls

### 2.3 Controls with Events

#### **SearchBox.ascx**
- **Purpose:** Reusable search input with button
- **Properties:** `Placeholder` (string), `SearchText` (string - two-way)
- **Events:** `Search` event with `SearchEventArgs` (SearchTerm, Category)
- **ViewState:** Preserves `SearchText` value
- **Output:** `<div class="search-box">` with input and button
- **Pattern:** User input control with custom event args, ViewState usage

#### **DepartmentFilter.ascx**
- **Purpose:** Dropdown filter for department selection
- **Properties:** `SelectedDepartmentId` (int), `AutoPostBack` (bool)
- **Events:** `DepartmentChanged` event
- **Data binding:** Loads departments from database in Page_Load
- **Output:** DropDownList HTML
- **Pattern:** Data-bound dropdown with event, database access in code-behind

### 2.4 Complex/Nested Controls

#### **DashboardWidget.ascx**
- **Purpose:** Configurable dashboard widget container
- **Properties:** `WidgetTitle` (string), `IconClass` (string), `RefreshInterval` (int)
- **Template:** `ContentTemplate` (ITemplate) for custom content
- **Nested:** Contains inner controls defined by page
- **Output:** `<div class="widget">` with templated content area
- **Pattern:** Template control, demonstrates ITemplate pattern

#### **ResourceBrowser.ascx**
- **Purpose:** Browse and filter document resources
- **Properties:** `CategoryId` (int), `ShowCategories` (bool)
- **Nested controls:** Contains `SearchBox.ascx` and `Breadcrumb.ascx` internally
- **Events:** `ResourceSelected` event
- **Output:** Complex nested structure with multiple user controls
- **Pattern:** Composite control containing other user controls, demonstrates ASCX nesting

### 2.5 Web.config Registration

#### **QuickStats.ascx**
- **Purpose:** Displays summary statistics (employee count, announcements, training)
- **Properties:** `ShowEmployeeCount` (bool), `ShowAnnouncementCount` (bool)
- **Registration:** Registered in web.config with tagPrefix `<uc:QuickStats>`
- **Output:** `<div class="quick-stats">` with stat tiles
- **Pattern:** Demonstrates web.config tagPrefix registration vs <%@ Register %>

### 2.6 Additional Controls

#### **Pager.ascx**
- **Purpose:** Custom pagination control
- **Properties:** `CurrentPage` (int), `TotalPages` (int), `PageSize` (int)
- **Events:** `PageChanged` event
- **Output:** `<div class="pager">` with page links
- **Pattern:** Stateful control with ViewState, custom event

---

## 3. Custom Server Controls

Custom server controls are C#-based controls that inherit from `WebControl`, `CompositeControl`, `DataBoundControl`, or other `Control` base classes. Unlike ASCX controls, they have no markup file—all rendering and behavior is programmatic. These patterns are critical for migration testing because they exercise different code paths than ASCX controls.

### 3.1 StarRating : WebControl

**File:** `App_Code/Controls/StarRating.cs`

**Base class:** `System.Web.UI.WebControls.WebControl`

**Purpose:** Display a 1-5 star rating in the DepartmentPortal employee profiles and training course reviews. Renders stars as individual span elements.

**Key Properties:**
- `Rating` (int, 1-5) — Current rating value
- `ReadOnly` (bool) — If true, renders as display-only; if false, renders clickable stars
- `StarColor` (string, CSS color) — Color of filled stars (default: gold)
- `EmptyStarColor` (string) — Color of empty stars (default: lightgray)

**Key Methods:**
- `RenderContents(HtmlTextWriter)` — Renders `<span class="star-rating">` with individual star spans for each rating

**HTML Output:**
```html
<span class="star-rating" style="color: gold;">
  <span class="star filled" data-rating="1">★</span>
  <span class="star filled" data-rating="2">★</span>
  <span class="star filled" data-rating="3">★</span>
  <span class="star empty" data-rating="4">☆</span>
  <span class="star empty" data-rating="5">☆</span>
</span>
```

**ViewState Usage:** Rating value stored in ViewState

**Migration Pattern:** Simple WebControl subclass demonstrating `RenderContents()` override, property rendering via `AddAttributesToRender()`, and HTML generation via `HtmlTextWriter`.

---

### 3.2 EmployeeCard : CompositeControl

**File:** `App_Code/Controls/EmployeeCard.cs`

**Base class:** `System.Web.UI.WebControls.CompositeControl`

**Purpose:** Display an employee information card with photo, name, title, department, and contact info. Demonstrates programmatic child control creation.

**Key Properties:**
- `EmployeeId` (int)
- `EmployeeName` (string)
- `Title` (string)
- `Department` (string)
- `PhotoUrl` (string)
- `ShowContactInfo` (bool) — Conditionally shows email/phone
- `EnableDetailsLink` (bool) — If true, wraps name in hyperlink to employee details

**Key Methods:**
- `CreateChildControls()` — Creates Image, Label, HyperLink, and other child controls
- `EnsureChildControls()` — Called before rendering to ensure children are created
- Overrides `OnDataBinding()` to bind employee data

**Rendered Child Controls:**
- `Image` for photo
- `Label` controls for name, title, department, email, phone
- `HyperLink` (conditional) for name link

**HTML Output:**
```html
<div class="employee-card">
  <div class="card-header">
    <img src="..." alt="Photo" class="employee-photo" />
  </div>
  <div class="card-body">
    <h3><a href="EmployeeDetails.aspx?id=123">John Doe</a></h3>
    <p class="title">Senior Developer</p>
    <p class="department">Engineering</p>
    <div class="contact-info" style="display: block;">
      <p>john.doe@company.com | (555) 123-4567</p>
    </div>
  </div>
</div>
```

**ViewState Usage:** Stores property values for child controls

**Migration Pattern:** CompositeControl demonstrating `CreateChildControls()` pattern, parent-child control hierarchy, and data binding to composite controls.

---

### 3.3 SectionPanel : Templated Control

**File:** `App_Code/Controls/SectionPanel.cs`

**Base class:** `System.Web.UI.Control` (implements `INamingContainer`)

**Purpose:** Container control for dashboard sections with customizable header and body content via `ITemplate`. Used to wrap section content throughout DepartmentPortal.

**Key Properties:**
- `Title` (string) — Section title (not templated, used as default header)
- `HeaderTemplate` (ITemplate) — Custom header template (default: renders Title in `<h2>`)
- `ContentTemplate` (ITemplate, required) — Consumer-defined body content
- `FooterTemplate` (ITemplate, optional) — Custom footer template
- `CssClass` (string) — Additional CSS classes

**Key Methods:**
- `CreateChildControls()` — Instantiates and adds templated controls
- `DataBind()` — Override to bind nested templates
- `OnInit()` — Calls `EnsureChildControls()`

**Instantiation Pattern (in ASPX):**
```aspx
<local:SectionPanel Title="Training Courses" runat="server">
  <ContentTemplate>
    <asp:Repeater DataSource="<%# Courses %>" runat="server">
      <ItemTemplate>
        <div><%# Eval("CourseName") %></div>
      </ItemTemplate>
    </asp:Repeater>
  </ContentTemplate>
  <HeaderTemplate>
    <div class="custom-header">
      <h2>Courses for <%# Department %></h2>
    </div>
  </HeaderTemplate>
</local:SectionPanel>
```

**HTML Output:**
```html
<div class="section-panel">
  <div class="section-header">
    <h2>Training Courses</h2>
  </div>
  <div class="section-content">
    <!-- ContentTemplate rendered here -->
  </div>
  <div class="section-footer" style="display: none;">
    <!-- FooterTemplate rendered here if specified -->
  </div>
</div>
```

**ViewState Usage:** Minimal — mostly used for template instantiation

**Migration Pattern:** Templated control demonstrating `ITemplate` property declarations, `InstantiateIn()` pattern, template instantiation in `CreateChildControls()`, and nested control binding.

---

### 3.4 PollQuestion : Control with ViewState and Postback

**File:** `App_Code/Controls/PollQuestion.cs`

**Base class:** `System.Web.UI.Control` (implements `IPostBackEventHandler`)

**Purpose:** Display a poll/survey question with radio button options and "Submit Vote" button. Stores selected answer in ViewState and handles postback voting events.

**Key Properties:**
- `QuestionText` (string)
- `OptionsData` (string[], comma-separated) — Poll answer options
- `SelectedOption` (int, 0-based index) — Currently selected option
- `VotesPerOption` (int[]) — Vote counts per option (cached, readonly)

**Key Events:**
- `VoteSubmitted` event (custom EventArgs: `selectedOption` int, `optionText` string)

**Key Methods:**
- `RenderContents(HtmlTextWriter)` — Renders question, radio buttons, and submit button
- `RaisePostBackEvent(string eventArgument)` — Implements `IPostBackEventHandler`, handles vote submission
- `LoadViewState()` / `SaveViewState()` — Custom ViewState management for SelectedOption

**HTML Output:**
```html
<div class="poll-question">
  <h4>What is your preferred training format?</h4>
  <div class="poll-options">
    <div>
      <input type="radio" name="poll_123" value="0" id="opt_0" />
      <label for="opt_0">In-Person</label>
    </div>
    <div>
      <input type="radio" name="poll_123" value="1" id="opt_1" />
      <label for="opt_1">Online</label>
    </div>
    <div>
      <input type="radio" name="poll_123" value="2" id="opt_2" />
      <label for="opt_2">Hybrid</label>
    </div>
  </div>
  <button type="submit" name="poll_123_submit" value="submit">Submit Vote</button>
</div>
```

**ViewState Usage:** Stores `SelectedOption` index across postbacks

**Postback Handling:** Implements `IPostBackEventHandler.RaisePostBackEvent()` to process vote submission, validate selection, raise `VoteSubmitted` event.

**Migration Pattern:** Control with postback event handling, custom ViewState, and `IPostBackEventHandler` implementation. Demonstrates Web Forms event-driven postback model.

---

### 3.5 NotificationBell : Control with Custom Events

**File:** `App_Code/Controls/NotificationBell.cs`

**Base class:** `System.Web.UI.WebControls.WebControl`

**Purpose:** Display a notification bell icon in the page header showing unread announcements count. Supports click events to open/close notification drawer.

**Key Properties:**
- `UnreadCount` (int) — Number of unread notifications
- `IconUrl` (string) — Bell icon image URL
- `NotificationDrawerVisible` (bool, ViewState) — Whether drawer is expanded
- `MaxNotificationsToDisplay` (int) — Limit notifications shown in drawer (default 5)

**Custom Events:**
- `NotificationClicked` event (custom `NotificationEventArgs`)
- `NotificationDismissed` event (custom `NotificationEventArgs` with `NotificationId` int)

**Custom EventArgs Class (`NotificationEventArgs`):**
```csharp
public class NotificationEventArgs : EventArgs
{
    public int NotificationId { get; set; }
    public string NotificationText { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**Key Methods:**
- `RenderContents(HtmlTextWriter)` — Renders bell icon with badge showing count, drawer HTML structure
- `OnNotificationClicked(NotificationEventArgs)` — Raises `NotificationClicked` event
- Custom event delegates and event declarations

**HTML Output:**
```html
<div class="notification-bell">
  <span class="bell-icon">🔔
    <span class="badge">3</span>
  </span>
  <div class="notification-drawer" style="display: none;">
    <div class="drawer-header">
      <h4>Notifications</h4>
      <button class="close-btn">×</button>
    </div>
    <div class="drawer-body">
      <ul>
        <li class="notification-item" data-id="1">
          New policy document: IT Security Update
          <span class="timestamp">2 hours ago</span>
        </li>
        <!-- more notifications -->
      </ul>
    </div>
  </div>
</div>
```

**ViewState Usage:** Stores `NotificationDrawerVisible` state

**Migration Pattern:** Control with custom events and custom `EventArgs` classes. Demonstrates event-based communication between control and container page, custom event delegate declarations.

---

### 3.6 EmployeeDataGrid : Data-Bound Control

**File:** `App_Code/Controls/EmployeeDataGrid.cs`

**Base class:** `System.Web.UI.WebControls.DataBoundControl` (or implements `IDataItemContainer`)

**Purpose:** Display a searchable, sortable, pageable grid of employees bound to a data source. Wraps GridView with custom filtering and sorting logic.

**Key Properties:**
- `DataSource` (IEnumerable, IDataReader, DataTable) — Employee data source
- `SearchText` (string) — Filter grid by employee name (ViewState)
- `SortColumn` (string) — Current sort column name (ViewState)
- `SortDirection` (SortDirection enum: Ascending/Descending) — Sort direction (ViewState)
- `PageSize` (int) — Rows per page (default 10)
- `AllowPaging` (bool)
- `AllowSorting` (bool)
- `AllowSearch` (bool)

**Key Events:**
- `RowCommand` event (BoundGrid-style: e.CommandName, e.CommandArgument for Edit/Delete/View)
- `SortChanged` event (custom `SortChangedEventArgs`: SortColumn, SortDirection)
- `PageChanged` event

**Key Methods:**
- `PerformDataBinding()` — Override to apply filtering and sorting before binding
- `CreateChildControls()` — Creates internal GridView, Repeater for pager
- `CreateDataSource()` — Apply search/sort filters to data source
- Implement `IPageableItemContainer` for paging support

**HTML Output:**
```html
<div class="employee-data-grid">
  <div class="grid-toolbar">
    <input type="text" placeholder="Search employees..." id="searchText" />
    <button type="submit">Search</button>
  </div>
  <table class="grid">
    <thead>
      <tr>
        <th><a href="?sort=Name">Name</a></th>
        <th><a href="?sort=Department">Department</a></th>
        <th><a href="?sort=Title">Title</a></th>
        <th>Actions</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>John Doe</td>
        <td>Engineering</td>
        <td>Senior Developer</td>
        <td>
          <a href="..." data-cmd="View" data-arg="123">View</a>
          <a href="..." data-cmd="Edit" data-arg="123">Edit</a>
        </td>
      </tr>
      <!-- more rows -->
    </tbody>
  </table>
  <div class="grid-pager">
    <span>Page 1 of 5</span>
    <button>Previous</button> <button>Next</button>
  </div>
</div>
```

**ViewState Usage:** Stores `SearchText`, `SortColumn`, `SortDirection`, `CurrentPageIndex` across postbacks

**Data Binding Lifecycle:** Demonstrates `PerformDataBinding()`, `CreateDataSource()` override pattern, integration with ASP.NET data source controls (ObjectDataSource), and child control binding.

**Migration Pattern:** DataBoundControl demonstrating advanced data binding patterns, paging/sorting state in ViewState, command event handling (RowCommand), and complex child control hierarchy with data-driven grid rendering.

---

### 3.7 DepartmentBreadcrumb : Bare Control

**File:** `App_Code/Controls/DepartmentBreadcrumb.cs`

**Base class:** `System.Web.UI.Control` (bare base class, no HTML wrapper)

**Purpose:** Render a breadcrumb navigation path showing current department hierarchy (Organization → Division → Department). Demonstrates rendering custom HTML from scratch without inheriting from WebControl or CompositeControl, no built-in style support.

**Key Properties:**
- `OrganizationName` (string) — Root organization name
- `DivisionName` (string) — Middle-level division name (optional)
- `DepartmentName` (string) — Current department name
- `DepartmentId` (int) — Current department identifier
- `Separator` (string) — HTML separator between breadcrumb items (default: " → ")
- `EnableLinks` (bool) — Whether breadcrumb items are clickable links (default: true)
- `LinkCssClass` (string) — CSS class applied to breadcrumb links

**Custom Events:**
- `BreadcrumbItemClicked` event (custom `BreadcrumbEventArgs` with `DepartmentId` int, `ItemName` string)

**Custom EventArgs Class (`BreadcrumbEventArgs`):**
```csharp
public class BreadcrumbEventArgs : EventArgs
{
    public int DepartmentId { get; set; }
    public string ItemName { get; set; }
    public string NavigationLevel { get; set; } // "organization", "division", "department"
}
```

**Key Methods:**
- `Render(HtmlTextWriter)` — Directly writes HTML string with breadcrumb structure (no CreateChildControls, no controls collection used)
- `OnBreadcrumbItemClicked(BreadcrumbEventArgs)` — Raises `BreadcrumbItemClicked` event on postback
- `RaisePostBackEvent(string eventArgument)` — Implements `IPostBackEventHandler` for breadcrumb link postback handling

**HTML Output (No ViewState):**
```html
<div class="breadcrumb-container">
  <a href="javascript:__doPostBack('BreadcrumbControl', 'org_0')" class="breadcrumb-link">Organization</a>
  <span class="breadcrumb-separator"> → </span>
  <a href="javascript:__doPostBack('BreadcrumbControl', 'div_1')" class="breadcrumb-link">Engineering Division</a>
  <span class="breadcrumb-separator"> → </span>
  <span class="breadcrumb-current">Software Development</span>
</div>
```

**No ViewState:** This bare Control uses no ViewState (no properties persisted across postback). State is managed by parent page or parent control.

**Postback Handling:** Implements `IPostBackEventHandler` interface to handle breadcrumb link clicks directly in `RaisePostBackEvent()` without child controls.

**Migration Pattern:** Bare `System.Web.UI.Control` inheritor demonstrating pure HTML rendering via `Render()` override, direct postback event handling without child control hierarchy, and event-driven breadcrumb navigation. Exercises the most primitive control base class with no wrapper HTML element or ViewState support.

---

### 3.8 Web.config Registration for Custom Controls

Custom server controls can be registered in Web.config for application-wide use:

```xml
<system.web>
  <httpHandlers>
    <!-- Custom control registration not needed in Web.config 
         but assembly must be accessible and control types resolvable -->
  </httpHandlers>
</system.web>
```

In ASPX pages, use `@Register` directive (similar to ASCX, but points to control class):

```aspx
<%@ Register Assembly="DepartmentPortal" Namespace="DepartmentPortal.Controls" TagPrefix="local" %>
<local:StarRating Rating="4" ReadOnly="true" runat="server" />
<local:EmployeeCard EmployeeId="123" ShowContactInfo="true" runat="server" />
<local:SectionPanel Title="Dashboard" runat="server">
  <ContentTemplate>
    <local:EmployeeDataGrid AllowPaging="true" AllowSorting="true" runat="server" />
  </ContentTemplate>
</local:SectionPanel>
```

---

## 4. Custom Base Classes

### 4.1 BasePage : System.Web.UI.Page

**File:** `App_Code/BasePage.cs`

**Features:**
- **Authentication check:** Override `OnInit` to verify `Session["UserId"]`, redirect to Login.aspx if missing
- **Audit logging:** Override `OnPreRender` to log page access to database (UserId, PageUrl, Timestamp)
- **Theme management:** Set theme based on `Session["Theme"]` (Light/Dark)
- **Common properties:** `CurrentUser` (Employee object), `IsAdmin` (bool)
- **Helper methods:** `ShowMessage(string)`, `LogError(Exception)`

**Usage:** All authenticated pages inherit from `BasePage`

**Migration challenge:** BasePage assumes Session access, theme system, OnInit/OnPreRender overrides

### 4.2 BaseMasterPage : System.Web.UI.MasterPage

**File:** `App_Code/BaseMasterPage.cs`

**Features:**
- **Menu population:** Override `Page_Load` to populate navigation menu from database
- **User info display:** Provide `UserDisplayName` property exposed to content pages
- **Common placeholders:** Define `FindContentPlaceHolder` helper method
- **Script injection:** Override `OnPreRender` to inject analytics script

**Usage:** Site.Master inherits from `BaseMasterPage`

**Migration challenge:** MasterPage-specific APIs, ContentPlaceHolder access pattern

### 4.3 BaseUserControl : System.Web.UI.UserControl

**File:** `App_Code/BaseUserControl.cs`

**Features:**
- **Logging:** Provide `LogActivity(string)` method for control usage tracking
- **Cache helper:** `CacheGet<T>(string key)` and `CacheSet<T>(string key, T value, int minutes)`
- **Common properties:** `ControlId` (string), `IsVisible` (bool with ViewState)
- **Initialization:** Override `OnInit` to set default CSS class

**Usage:** All ASCX controls inherit from `BaseUserControl` in code-behind

**Migration challenge:** UserControl lifecycle, Cache API, ViewState in base class

---

## 5. Page Inventory

### 5.1 Public Pages (No Auth Required)

1. **Default.aspx** (Home)
   - Controls: PageHeader, Footer, QuickStats (web.config registered)
   - Purpose: Landing page with welcome message and stats

2. **Login.aspx**
   - Controls: Footer
   - Purpose: Authentication page (sets Session["UserId"])

### 5.2 Authenticated Pages (Inherit from BasePage)

3. **Dashboard.aspx**
   - Base class: `BasePage`
   - Controls: PageHeader, Breadcrumb, DashboardWidget (x3), QuickStats, Footer
   - Purpose: User dashboard with configurable widgets
   - Pattern: Template controls, multiple instances of same control

4. **Employees.aspx**
   - Base class: `BasePage`
   - Controls: PageHeader, Breadcrumb, SearchBox, DepartmentFilter, EmployeeList, Pager, Footer
   - Purpose: Employee directory with search and filter
   - Events: SearchBox.Search, DepartmentFilter.DepartmentChanged, Pager.PageChanged
   - Pattern: Multiple event handlers, control composition

5. **EmployeeDetail.aspx**
   - Base class: `BasePage`
   - Controls: PageHeader, Breadcrumb, Footer
   - Purpose: Single employee detail view
   - QueryString: EmployeeId

6. **Announcements.aspx**
   - Base class: `BasePage`
   - Controls: PageHeader, Breadcrumb, SearchBox, AnnouncementCard (Repeater), Pager, Footer
   - Purpose: Announcement listing
   - Pattern: Repeater with ASCX as ItemTemplate

7. **AnnouncementDetail.aspx**
   - Base class: `BasePage`
   - Controls: PageHeader, Breadcrumb, AnnouncementCard, Footer
   - QueryString: AnnouncementId

8. **Training.aspx**
   - Base class: `BasePage`
   - Controls: PageHeader, Breadcrumb, SearchBox, TrainingCatalog, Footer
   - Purpose: Training course catalog
   - Events: TrainingCatalog.EnrollmentRequested
   - Session: Adds to `Session["EnrolledCourses"]`

9. **MyTraining.aspx**
   - Base class: `BasePage`
   - Controls: PageHeader, Breadcrumb, TrainingCatalog (enrolled only), Footer
   - Purpose: User's enrolled courses
   - Session: Reads `Session["EnrolledCourses"]`

10. **Resources.aspx**
    - Base class: `BasePage`
    - Controls: PageHeader, Breadcrumb, ResourceBrowser, Footer
    - Purpose: Document resource library
    - Pattern: Nested ASCX (ResourceBrowser contains SearchBox and Breadcrumb)

11. **ResourceDetail.aspx**
    - Base class: `BasePage`
    - Controls: PageHeader, Breadcrumb, Footer
    - QueryString: ResourceId

### 5.3 Admin Pages (BasePage + Admin Check)

12. **Admin/ManageAnnouncements.aspx**
    - Base class: `BasePage` (checks `IsAdmin`)
    - Controls: PageHeader, Breadcrumb, Footer
    - Purpose: CRUD for announcements

13. **Admin/ManageTraining.aspx**
    - Base class: `BasePage` (checks `IsAdmin`)
    - Controls: PageHeader, Breadcrumb, Footer
    - Purpose: CRUD for training courses

14. **Admin/ManageEmployees.aspx**
    - Base class: `BasePage` (checks `IsAdmin`)
    - Controls: PageHeader, Breadcrumb, SearchBox, EmployeeList, Footer
    - Purpose: Employee management

### 5.4 Master Pages

15. **Site.Master**
    - Base class: `BaseMasterPage`
    - Controls: None (master layout)
    - ContentPlaceHolders: MainContent, ScriptsSection

---

## 6. Work Breakdown

### Phase 1: Foundation (Jubilee)
**Deliverable:** Working .NET Framework 4.8 project with data model and base classes

- **Task 1.1:** Create Visual Studio 2022 Web Forms project (DepartmentPortal)
  - Target: .NET Framework 4.8
  - NuGet: Entity Framework 6.x, jQuery, Bootstrap 3.x
  - Structure: App_Code/, App_Data/, Content/, Scripts/, Models/

- **Task 1.2:** Build data model with EF6 Database First
  - Entities: Employee, Department, Announcement, TrainingCourse, Resource, Enrollment
  - DbContext: PortalDbContext
  - Seed data: 50 employees, 5 departments, 10 announcements, 15 courses, 20 resources

- **Task 1.3:** Create custom base classes
  - `App_Code/BasePage.cs` with auth, audit, theme features
  - `App_Code/BaseMasterPage.cs` with menu population
  - `App_Code/BaseUserControl.cs` with logging and cache helpers

- **Task 1.4:** Create Site.Master
  - Inherit from `BaseMasterPage`
  - Bootstrap 3 layout with navbar, footer
  - ContentPlaceHolders: MainContent, ScriptsSection

### Phase 2: ASCX Controls (Jubilee)
**Deliverable:** All 12 ASCX controls with code-behind

- **Task 2.1:** Simple display controls
  - Breadcrumb.ascx
  - PageHeader.ascx
  - Footer.ascx
  - QuickStats.ascx (web.config registration)

- **Task 2.2:** Data-bound controls
  - AnnouncementCard.ascx
  - EmployeeList.ascx
  - TrainingCatalog.ascx

- **Task 2.3:** Controls with events
  - SearchBox.ascx (custom SearchEventArgs)
  - DepartmentFilter.ascx
  - Pager.ascx

- **Task 2.4:** Complex controls
  - DashboardWidget.ascx (ITemplate pattern)
  - ResourceBrowser.ascx (nested ASCX)

- **Task 2.5:** Web.config tagPrefix registration
  - Add `<pages><controls>` section with `uc:` prefix for QuickStats

### Phase 3: Pages (Jubilee)
**Deliverable:** All 14 pages wired to controls

- **Task 3.1:** Public pages
  - Default.aspx
  - Login.aspx

- **Task 3.2:** Main authenticated pages
  - Dashboard.aspx (template controls, multiple widgets)
  - Employees.aspx (search, filter, paging)
  - EmployeeDetail.aspx

- **Task 3.3:** Announcement pages
  - Announcements.aspx (Repeater with ASCX ItemTemplate)
  - AnnouncementDetail.aspx

- **Task 3.4:** Training pages
  - Training.aspx (event handling, Session write)
  - MyTraining.aspx (Session read)

- **Task 3.5:** Resource pages
  - Resources.aspx (nested ASCX)
  - ResourceDetail.aspx

- **Task 3.6:** Admin pages
  - Admin/ManageAnnouncements.aspx
  - Admin/ManageTraining.aspx
  - Admin/ManageEmployees.aspx

### Phase 4: Testing & Documentation (Multi-agent)

**Task 4.1: Manual smoke test (Jubilee)**
- Verify all pages load
- Test authentication flow (Login → Dashboard → pages → Logout)
- Test Session persistence (training enrollment)
- Test ViewState (SearchBox retains text, filters retain state)
- Test events (Search, DepartmentChanged, PageChanged, EnrollmentRequested)
- Test nested controls (ResourceBrowser renders SearchBox and Breadcrumb)
- Test web.config registration (QuickStats renders correctly)

**Task 4.2: Migration toolkit coverage analysis (Bishop)**
- Run `bwfc-migrate.ps1` against DepartmentPortal
- Document which ASCX patterns are converted successfully
- Identify gaps:
  - Does toolkit convert ASCX → Blazor components?
  - Does toolkit handle custom base classes (BasePage, BaseMasterPage, BaseUserControl)?
  - Does toolkit convert ITemplate controls?
  - Does toolkit handle web.config tagPrefix registrations?
  - Does toolkit convert custom event args (SearchEventArgs)?
  - Does toolkit handle Session/ViewState/Cache in base classes?
- Create backlog items for missing patterns

**Task 4.3: Documentation (Beast)**
- Create `dev-docs/samples/DEPARTMENTPORTAL.md` with:
  - Application overview
  - ASCX control catalog with migration notes
  - Base class migration patterns
  - Known migration toolkit gaps
  - Manual migration steps for unsupported patterns

**Task 4.4: Acceptance tests (Colossus — DEFERRED)**
- NOTE: This requires the Blazor "After" version to exist first
- Write Playwright tests for migrated Blazor version
- Test parity: Web Forms output vs Blazor output
- Defer until migration toolkit supports ASCX conversion

**Task 4.5: Unit tests (Rogue — DEFERRED)**
- NOTE: Blazor components must exist before writing bUnit tests
- Write bUnit tests for converted Blazor components
- Defer until ASCX → Blazor conversion is complete

---

## 7. Dependencies and Sequencing

### Critical Path

```
Foundation (1.1-1.4) 
   ↓
ASCX Controls (2.1-2.5)
   ↓
Pages (3.1-3.6)
   ↓
Manual Smoke Test (4.1)
   ↓
Migration Coverage Analysis (4.2)
   ↓
Documentation (4.3)
```

### Parallel Work

- **After Foundation complete:** Tasks 2.1-2.5 can be done in parallel (independent controls)
- **After Controls complete:** Tasks 3.1-3.6 can be parallelized by page groups
- **After Smoke Test:** Tasks 4.2 and 4.3 can run in parallel

### Deferred Work

- **Acceptance tests (4.4):** Blocked until migration toolkit produces Blazor version
- **Unit tests (4.5):** Blocked until Blazor components exist

### External Dependencies

- **Migration toolkit enhancement:** Likely required to support ASCX → Blazor conversion
- **BWFC library updates:** May need new base components or shims for base class patterns

---

## 8. Success Criteria

### Minimum Viable Sample (Phase 1-3 Complete)

✅ .NET Framework 4.8 project builds and runs without errors  
✅ All 12 ASCX controls render correctly with sample data  
✅ All 3 custom base classes function (auth, audit, theme, logging, cache)  
✅ All 14 pages load and display controls  
✅ Authentication flow works (Session["UserId"])  
✅ Event handlers fire correctly (Search, DepartmentChanged, PageChanged, EnrollmentRequested)  
✅ ViewState preserves state across postbacks (SearchBox, filters)  
✅ Session persists data (training enrollment)  
✅ Nested controls render (ResourceBrowser contains SearchBox + Breadcrumb)  
✅ Web.config tagPrefix registration works (QuickStats renders with `<uc:QuickStats>`)  
✅ Template control works (DashboardWidget.ContentTemplate)  

### Migration Coverage (Phase 4.2 Complete)

✅ Migration toolkit executed against DepartmentPortal  
✅ Coverage analysis document created listing:
  - ✅ Patterns successfully converted
  - ⚠️ Patterns partially converted (manual fixes needed)
  - ❌ Patterns not converted (toolkit gaps)  
✅ Backlog items created for toolkit enhancements (if needed)  

### Documentation (Phase 4.3 Complete)

✅ `dev-docs/samples/DEPARTMENTPORTAL.md` created  
✅ Each ASCX control documented with migration notes  
✅ Base class migration patterns documented  
✅ Manual migration procedures documented (for unsupported patterns)  

### Full Migration Success (DEFERRED — Requires Toolkit Enhancements)

⏳ ASCX controls converted to Blazor components  
⏳ Custom base classes converted to Blazor equivalents  
⏳ All pages render in Blazor with parity to Web Forms HTML  
⏳ Playwright tests pass (visual + functional parity)  
⏳ bUnit tests pass (component behavior)  

---

## 9. Risk Assessment

### High Risk

**R1: Migration toolkit may not support ASCX → Blazor conversion**
- Impact: Cannot produce "After" Blazor version
- Mitigation: Document manual conversion patterns as stopgap
- Owner: Bishop (toolkit analysis)

**R2: Custom base classes (BasePage, BaseMasterPage, BaseUserControl) have no Blazor equivalent**
- Impact: Significant manual migration work required
- Mitigation: Design Blazor base component patterns, update BWFC library if needed
- Owner: Forge (architecture review)

**R3: ITemplate pattern not supported in Blazor**
- Impact: DashboardWidget.ContentTemplate cannot migrate directly
- Mitigation: Document RenderFragment approach for Blazor
- Owner: Beast (documentation)

### Medium Risk

**R4: Session/ViewState/Cache access in base classes**
- Impact: Requires Blazor state management patterns (scoped services, ProtectedSessionStorage)
- Mitigation: Create Blazor shim services for Session/Cache access
- Owner: Cyclops (if BWFC enhancements needed)

**R5: Web.config tagPrefix registration**
- Impact: No web.config in Blazor, requires _Imports.razor pattern
- Mitigation: Migration toolkit should auto-generate `@using` directives
- Owner: Bishop (toolkit enhancement)

### Low Risk

**R6: Custom event args (SearchEventArgs)**
- Impact: EventArgs classes need to migrate to Blazor event callbacks
- Mitigation: Standard C# classes migrate cleanly, EventCallback<T> is straightforward
- Owner: None (standard pattern)

**R7: EF6 → EF Core conversion**
- Impact: Already solved by existing toolkit
- Mitigation: Run 22 validates EF6 EDMX conversion
- Owner: None (existing functionality)

---

## 10. Future Enhancements

### Post-MVP Features (Not in Scope)

- **Localization:** Add GlobalResource.resx and demonstrate Localize control usage
- **AJAX:** Add UpdatePanel for partial page updates (AJAX controls milestone)
- **Custom validators:** Complex CustomValidator scenarios
- **Two-way data binding:** Demonstrate Bind expressions in ASCX
- **User control properties with TypeConverter:** Complex property type scenarios
- **Dynamic control loading:** LoadControl() pattern in code-behind
- **ASCX output caching:** VaryByControl, VaryByParam scenarios

### Stretch Goals (If Time Permits)

- **Mobile.Master:** Separate master page for mobile (like BeforeWebForms)
- **Web.sitemap:** SiteMapPath integration with ASCX breadcrumb
- **Roles-based security:** Beyond simple Session["UserId"] check
- **ASCX in App_Code:** Demonstrate programmatic control creation

---

## 11. Timeline Estimate

**Phase 1 (Foundation):** 1-2 days  
**Phase 2 (ASCX Controls):** 2-3 days  
**Phase 3 (Pages):** 2-3 days  
**Phase 4.1 (Smoke Test):** 0.5 days  
**Phase 4.2 (Migration Analysis):** 1-2 days  
**Phase 4.3 (Documentation):** 1 day  

**Total:** 7-11 days (1.5-2 weeks)

**Deferred work (requires toolkit enhancements):** TBD based on toolkit roadmap

---

## 12. Key Decision Points

### Decision 1: EF6 Database First vs Code First
**Recommendation:** Database First with EDMX  
**Rationale:** Tests toolkit's EDMX → EF Core conversion (already implemented in Run 22)  
**Alternative:** Code First (simpler but doesn't test EDMX migration)

### Decision 2: Bootstrap 3 vs Bootstrap 4/5
**Recommendation:** Bootstrap 3  
**Rationale:** Matches BeforeWebForms, WingtipToys era (enterprise apps from that timeframe)  
**Alternative:** Bootstrap 4 (newer but less representative of legacy apps)

### Decision 3: SQL Server LocalDB vs In-Memory Database
**Recommendation:** SQL Server LocalDB  
**Rationale:** Realistic, tests connection string migration, supports full EF6 features  
**Alternative:** In-memory (simpler but less realistic)

### Decision 4: Full CRUD vs Read-Only
**Recommendation:** Read-only for most pages, CRUD for Admin section  
**Rationale:** Reduces complexity while still demonstrating postbacks and ViewState  
**Alternative:** Full CRUD everywhere (more realistic but much more work)

### Decision 5: Authentication Implementation
**Recommendation:** Simple Session-based auth (no ASP.NET Identity)  
**Rationale:** Focuses on ASCX patterns, not auth complexity; BeforeWebForms doesn't use Identity either  
**Alternative:** ASP.NET Identity (more realistic but out of scope for ASCX testing)

---

## 13. File Structure

```
DepartmentPortal/
├── App_Code/
│   ├── BasePage.cs
│   ├── BaseMasterPage.cs
│   └── BaseUserControl.cs
├── App_Data/
│   └── PortalDatabase.mdf
├── Content/
│   ├── bootstrap.css
│   └── site.css
├── Controls/
│   ├── AnnouncementCard.ascx
│   ├── AnnouncementCard.ascx.cs
│   ├── Breadcrumb.ascx
│   ├── Breadcrumb.ascx.cs
│   ├── DashboardWidget.ascx
│   ├── DashboardWidget.ascx.cs
│   ├── DepartmentFilter.ascx
│   ├── DepartmentFilter.ascx.cs
│   ├── EmployeeList.ascx
│   ├── EmployeeList.ascx.cs
│   ├── Footer.ascx
│   ├── Footer.ascx.cs
│   ├── Pager.ascx
│   ├── Pager.ascx.cs
│   ├── PageHeader.ascx
│   ├── PageHeader.ascx.cs
│   ├── QuickStats.ascx
│   ├── QuickStats.ascx.cs
│   ├── ResourceBrowser.ascx
│   ├── ResourceBrowser.ascx.cs
│   ├── SearchBox.ascx
│   ├── SearchBox.ascx.cs
│   ├── TrainingCatalog.ascx
│   └── TrainingCatalog.ascx.cs
├── Models/
│   ├── Model1.edmx
│   ├── Model1.edmx.diagram
│   ├── Model1.Context.cs
│   ├── Employee.cs
│   ├── Department.cs
│   ├── Announcement.cs
│   ├── TrainingCourse.cs
│   ├── Resource.cs
│   └── Enrollment.cs
├── Scripts/
│   ├── jquery-3.x.min.js
│   └── bootstrap.min.js
├── Admin/
│   ├── ManageAnnouncements.aspx
│   ├── ManageEmployees.aspx
│   └── ManageTraining.aspx
├── Default.aspx
├── Login.aspx
├── Dashboard.aspx
├── Employees.aspx
├── EmployeeDetail.aspx
├── Announcements.aspx
├── AnnouncementDetail.aspx
├── Training.aspx
├── MyTraining.aspx
├── Resources.aspx
├── ResourceDetail.aspx
├── Site.Master
├── Web.config
├── Web.sitemap
└── Global.asax
```

---

## 14. Integration with Existing Milestones

### Relationship to M22 (Migration Toolkit)
- DepartmentPortal provides **new test surface** for ASCX patterns
- Validates **EDMX conversion** (already implemented)
- Identifies **gaps** in toolkit coverage (ASCX, base classes, ITemplate)
- Drives **backlog prioritization** for toolkit enhancements

### Relationship to Sample Apps
- **BeforeWebForms:** Control samples (62 pages) — ASCX-focused equivalent
- **WingtipToys:** E-commerce (28 pages) — Migration test target with basic ASCX
- **ContosoUniversity:** Education (5 pages) — Migration test target, minimal ASCX
- **DepartmentPortal:** ASCX showcase (14 pages) — **New:** Demonstrates custom bases, nested ASCX, templates

### Relationship to Component Library
- May identify **missing BWFC components** for base class patterns
- Could drive **new utilities** (Session shims, ViewState helpers for Blazor)
- Validates **existing components** with complex ASCX compositions

---

## Appendix A: ASCX Control Catalog

| Control | Pattern | ViewState | Session | Events | Nested | Template | Web.config |
|---------|---------|-----------|---------|--------|--------|----------|------------|
| Breadcrumb | Display | No | No | No | No | No | No |
| PageHeader | Display | No | Yes | No | No | No | No |
| Footer | Display | No | No | No | No | No | No |
| QuickStats | Display | No | No | No | No | No | **Yes** |
| AnnouncementCard | Data-bound | Yes | No | No | No | No | No |
| EmployeeList | Data-bound | Yes | No | No | No | No | No |
| TrainingCatalog | Data-bound | No | No | Yes | No | No | No |
| SearchBox | Input | Yes | No | Yes | No | No | No |
| DepartmentFilter | Input | No | No | Yes | No | No | No |
| Pager | Input | Yes | No | Yes | No | No | No |
| DashboardWidget | Container | No | No | No | No | **Yes** | No |
| ResourceBrowser | Composite | No | No | Yes | **Yes** | No | No |

---

## Appendix B: Custom Base Class Feature Matrix

| Feature | BasePage | BaseMasterPage | BaseUserControl |
|---------|----------|----------------|-----------------|
| Session access | ✅ Auth check, theme | ❌ | ❌ |
| Database access | ✅ Audit logging | ✅ Menu population | ❌ |
| Cache access | ❌ | ❌ | ✅ Cache helpers |
| Lifecycle overrides | ✅ OnInit, OnPreRender | ✅ Page_Load, OnPreRender | ✅ OnInit |
| Custom properties | ✅ CurrentUser, IsAdmin | ✅ UserDisplayName | ✅ ControlId, IsVisible |
| Helper methods | ✅ ShowMessage, LogError | ✅ FindContentPlaceHolder | ✅ LogActivity |

---

## Appendix C: Migration Toolkit Enhancement Backlog (Projected)

Based on expected gaps, the following toolkit enhancements may be needed:

1. **ASCX → Blazor Component Conversion**
   - Parse .ascx markup + .ascx.cs code-behind
   - Generate .razor + .razor.cs component
   - Convert ViewState to `@code` fields or scoped state
   - Convert custom events to EventCallback<T>

2. **Custom Base Class Migration**
   - Detect inheritance from BasePage/BaseMasterPage/BaseUserControl
   - Generate Blazor base component equivalents
   - Convert Session access to ProtectedSessionStorage or scoped services
   - Convert Cache access to IMemoryCache

3. **Web.config TagPrefix → _Imports.razor**
   - Parse web.config `<pages><controls>` section
   - Generate `@using` directives in _Imports.razor
   - Update ASCX references to use new namespace

4. **ITemplate → RenderFragment**
   - Detect ITemplate properties in ASCX
   - Convert to RenderFragment<T> in Blazor
   - Update usage sites

5. **Nested ASCX Resolution**
   - Parse <%@ Register %> directives in ASCX files
   - Resolve nested control references
   - Generate correct @using statements

---

**END OF MILESTONE PLAN**
