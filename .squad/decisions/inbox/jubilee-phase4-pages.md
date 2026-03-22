# Decision: Phase 4 ASPX Page Implementation Patterns

**Date:** 2026-03-21  
**Context:** Creating all remaining ASPX pages for DepartmentPortal sample

## Decisions Made

### 1. Page Architecture Pattern
- **All authenticated pages inherit from BasePage** (not directly from System.Web.UI.Page)
- BasePage provides: CurrentUser (Employee), IsAdmin (bool), ShowMessage(string)
- Login.aspx → sets Session["CurrentUser"] → BasePage reads it

### 2. Admin Page Security Pattern
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsAdmin)
    {
        ShowMessage("Access denied. Administrator privileges required.");
        Response.Redirect("~/Dashboard.aspx");
        return;
    }
    // ... rest of page logic
}
```
**Rationale:** Consistent guard pattern at top of Page_Load for all admin pages

### 3. Pager Control Integration Pattern
```csharp
// Pager event handler signature: EventHandler<int>
protected void PagerControl_PageChanged(object sender, int pageNumber)
{
    CurrentPageIndex = pageNumber - 1; // Convert to 0-indexed
    BindData();
}

// Pager setup
pager.TotalPages = (int)Math.Ceiling((double)totalItems / PageSize);
pager.CurrentPage = CurrentPageIndex + 1; // Convert to 1-indexed
```
**Rationale:** 
- Pager control uses 1-indexed CurrentPage for UI display
- Page logic uses 0-indexed CurrentPageIndex internally
- Event passes pageNumber directly (int), not EventArgs wrapper

### 4. SearchBox Event Pattern
```csharp
protected void SearchBoxControl_Search(object sender, SearchEventArgs e)
{
    SearchQuery = e.SearchTerm; // Property is SearchTerm, not SearchQuery
    CurrentPageIndex = 0; // Reset to first page
    BindData();
}
```
**Rationale:** SearchEventArgs has SearchTerm property, always reset pagination on search

### 5. Department Lookup Pattern (No DepartmentId in Employee)
```csharp
// Employee.Department is string (department name), not ID
// To filter by DepartmentId from DepartmentFilter:
if (SelectedDepartmentId > 0)
{
    var dept = PortalDataProvider.GetDepartments().FirstOrDefault(d => d.Id == SelectedDepartmentId);
    if (dept != null)
    {
        filteredEmployees = filteredEmployees.Where(e => e.Department == dept.Name);
    }
}
```
**Rationale:** Employee model stores department name directly, requires join to filter by ID

### 6. Session State for Enrollment
```csharp
private List<int> EnrolledCourses
{
    get
    {
        if (Session["EnrolledCourses"] == null)
        {
            Session["EnrolledCourses"] = new List<int>();
        }
        return (List<int>)Session["EnrolledCourses"];
    }
}
```
**Rationale:** Shared session state between Training.aspx and MyTraining.aspx, lazy initialization

### 7. PageHeader Title Setting Pattern
```csharp
var pageHeader = (DepartmentPortal.Controls.PageHeader)FindControl("PageHeaderControl");
if (pageHeader != null)
{
    pageHeader.PageTitle = "Title"; // Property is PageTitle, not Title
}
```
**Rationale:** PageHeader control exposes PageTitle property, not Title/Description pair

### 8. Resource Model Simplified Properties
```csharp
// Resource model has: CategoryName (string), not Category
// Resource model DOES NOT have: FileSize, LastUpdated
// Solution: Use CategoryName, set FileSize/LastUpdated to "N/A"
CategoryLabel.Text = resource.CategoryName;
FileSizeLabel.Text = "N/A";
LastUpdatedLabel.Text = "N/A";
```
**Rationale:** Keep code simple, avoid adding properties to model for sample app

### 9. StarRating Control Property
```csharp
ratingControl.Rating = 4; // Property is Rating, not CurrentRating
```
**Rationale:** StarRating control uses Rating property

### 10. PollQuestion Options Format
```csharp
poll.Options = "In-person classroom,Live virtual sessions,Self-paced online,Hybrid";
// NOT: new List<string> { ... }
```
**Rationale:** PollQuestion.Options is string (comma-delimited), not List<string>

### 11. ASPX Directive Pattern
```aspx
<%@ Page Title="..." Language="C#" AutoEventWireup="true" CodeFile="Page.aspx.cs" Inherits="DepartmentPortal.PageClass" %>
```
**Rationale:** Use CodeFile (not CodeBehind) for this Web Application Project structure

## Impact
- **Build Success:** All 11 pages (22 files) compile with 0 errors
- **Consistency:** All pages follow same patterns for auth, events, pagination
- **Demonstrates Controls:** Uses all ASCX controls and all custom server controls
- **Admin Security:** Consistent guard pattern protects admin pages

## Alternatives Considered
- **DepartmentId in Employee:** Could have added DepartmentId property, but keeping it simple with Department string
- **Resource extended properties:** Could have added FileSize/LastUpdated to Resource model, but unnecessary for sample
- **Pager 0-indexed:** Could have made Pager 0-indexed, but 1-indexed is more user-friendly
