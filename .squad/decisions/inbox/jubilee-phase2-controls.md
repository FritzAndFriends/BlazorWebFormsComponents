# Decision: Phase 2 ASCX Control Implementation Patterns

**Author:** Jubilee (Sample Writer)
**Date:** 2026-03-21
**Status:** Accepted

## Context
Creating 12 ASCX user controls for the DepartmentPortal sample application (Phase 2).

## Decisions

### 1. CodeFile over CodeBehind
Used `CodeFile` directive (not `CodeBehind`) to match the existing Phase 1 pattern established in .aspx and .master files. This avoids .designer.cs files and keeps field declarations explicit in code-behind.

### 2. Manual field declarations
All server control references (Literal, Repeater, GridView, etc.) are declared as `protected` fields in the code-behind class. HTML elements with `runat="server"` use `HtmlGenericControl`.

### 3. ViewState for all stateful properties
Properties that need to survive postback (CurrentPage, SearchText, DepartmentFilter, ShowFullText, etc.) use `ViewState["PropertyName"]` pattern consistently.

### 4. Event patterns
- Simple events: `EventHandler` (DepartmentChanged)
- Typed events: `EventHandler<int>` (EnrollmentRequested, PageChanged, ResourceSelected)
- Custom args: `EventHandler<SearchEventArgs>` (Search)

### 5. ResourceBrowser nesting
ResourceBrowser uses `<%@ Register Src %>` to nest SearchBox and Breadcrumb controls, demonstrating ASCX composition — a key migration pattern.

### 6. QuickStats web.config registration
Added `<add tagPrefix="uc" src="~/Controls/QuickStats.ascx" tagName="QuickStats" />` alongside the existing namespace-based registration.

## Consequences
All 12 controls build successfully. They cover: simple display, data-bound, event-driven, complex/nested, and web.config-registered patterns per the milestone spec.
