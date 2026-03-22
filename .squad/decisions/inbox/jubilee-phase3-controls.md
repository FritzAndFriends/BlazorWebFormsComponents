# Decision: DepartmentPortal Phase 3 — Custom Server Control Architecture

**Date:** 2026-03-21  
**Decider:** Jubilee (Sample Writer)  
**Status:** Implemented

## Context

Phase 3 of the DepartmentPortal sample required creating 7 custom server controls demonstrating various ASP.NET Web Forms control development patterns. These controls showcase different inheritance hierarchies and implementation techniques that migration developers need to understand.

## Decision

Created 7 custom server controls in `samples/DepartmentPortal/App_Code/Controls/`:

1. **StarRating.cs** (WebControl) — Demonstrates simple property rendering with ViewState
2. **EmployeeCard.cs** (CompositeControl) — Shows programmatic child control creation
3. **SectionPanel.cs** (Templated Control) — ITemplate pattern with multiple template regions
4. **PollQuestion.cs** (IPostBackEventHandler) — Interactive postback handling
5. **NotificationBell.cs** (Custom Events) — Event-driven UI with custom EventArgs
6. **EmployeeDataGrid.cs** (DataBoundControl) — Data binding with search/sort/paging
7. **DepartmentBreadcrumb.cs** (Bare Control) — Direct HTML rendering via HtmlTextWriter

### Implementation Patterns

- **ViewState properties:** Standard pattern: `get { return (type)ViewState["Key"] ?? default; } set { ViewState["Key"] = value; }`
- **HTML encoding:** Use `System.Web.HttpUtility.HtmlEncode()` directly (not `Server.HtmlEncode()` which is only available in Page/UserControl)
- **Custom HTML attributes:** Use string overload `writer.AddAttribute("attrname", value)` for non-enum attributes like "placeholder"
- **Templated controls:** Use `[TemplateContainer]` and `[PersistenceMode(PersistenceMode.InnerProperty)]` attributes, instantiate via `ITemplate.InstantiateIn(PlaceHolder)`
- **Postback handling:** Use `Page.ClientScript.GetPostBackEventReference(this, eventArg)` for client-side postback generation

### Web.config Registration

Added namespace registration to enable `<local:*>` prefix usage:
```xml
<add tagPrefix="local" namespace="DepartmentPortal.Controls" assembly="DepartmentPortal" />
```

### EventArgs Reuse

Reused existing `NotificationEventArgs` and `BreadcrumbEventArgs` from Models namespace. Created `PollVoteEventArgs` as inner class in PollQuestion.cs.

## Rationale

This set of controls provides comprehensive coverage of Web Forms server control development patterns:

- **Inheritance diversity:** WebControl, CompositeControl, DataBoundControl, bare Control
- **Rendering approaches:** RenderContents override, CreateChildControls, direct HtmlTextWriter
- **Interactivity:** ViewState, postback handling, custom events
- **Advanced features:** Templates, data binding, composite controls

These patterns are essential for migration developers to understand, as they represent the most common custom control scenarios in enterprise Web Forms applications.

## Consequences

### Positive
- Sample demonstrates 7 distinct Web Forms control development patterns
- Build succeeds with zero errors
- Controls follow authentic .NET Framework 4.8 conventions
- Reuses existing EventArgs types where appropriate

### Negative
- Controls are functional demonstrations, not production-ready components
- Some controls (like EmployeeDataGrid) render placeholder data rather than implementing full functionality
- No CSS provided (relies on class names for styling hooks)

### Neutral
- App_Code location follows Web Application Project conventions
- Namespace `DepartmentPortal.Controls` shared by both ASCX controls and custom server controls
- PollQuestion creates its own EventArgs inner class (could have been in Models, but inner class is also idiomatic)

## Notes

**Build process:**
1. Restore: `.\nuget.exe restore samples\DepartmentPortal\packages.config -PackagesDirectory packages -NonInteractive`
2. Build: `& "C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Current\Bin\MSBuild.exe" samples\DepartmentPortal\DepartmentPortal.csproj /p:Configuration=Debug /verbosity:minimal /nologo`

**Next phase:** Phase 4 will likely involve creating sample pages that use these custom controls alongside the ASCX controls to demonstrate a complete migration scenario.
