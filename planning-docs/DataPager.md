>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# DataPager — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.datapager?view=netframework-4.8.1
**Blazor Component:** `BlazorWebFormsComponents.DataPager`
**Implementation Status:** ⚠️ Partial

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | Inherited from BaseWebFormsComponent |
| Visible | bool | ✅ Match | Inherited from BaseWebFormsComponent |
| Enabled | bool | ✅ Match | Inherited from BaseWebFormsComponent |
| TabIndex | short | ✅ Match | Inherited from BaseWebFormsComponent |
| CssClass | string | ✅ Match | Inherited from BaseStyledComponent |
| BackColor | WebColor | ✅ Match | Inherited from BaseStyledComponent |
| BorderColor | WebColor | ✅ Match | Inherited from BaseStyledComponent |
| BorderStyle | BorderStyle | ✅ Match | Inherited from BaseStyledComponent |
| BorderWidth | Unit | ✅ Match | Inherited from BaseStyledComponent |
| ForeColor | WebColor | ✅ Match | Inherited from BaseStyledComponent |
| Font | FontInfo | ✅ Match | Inherited from BaseStyledComponent |
| Height | Unit | ✅ Match | Inherited from BaseStyledComponent |
| Width | Unit | ✅ Match | Inherited from BaseStyledComponent |
| TotalRowCount | int | ✅ Match | Direct parameter |
| PageSize | int | ✅ Match | Defaults to 10 |
| PageIndex | int | ✅ Match | Zero-based, two-way bindable via PageIndexChanged |
| MaximumRows | int | ✅ Match | Computed property (= PageSize) |
| StartRowIndex | int | ✅ Match | Computed property (= PageIndex * PageSize) |
| ChildContent | RenderFragment | ✅ Match | For TemplatePagerField support |
| PageButtonCount | int | ✅ Match | Defaults to 5 |
| Mode | PagerButtons | ✅ Match | Enum: Numeric, NextPrevious, NextPreviousFirstLast, NumericFirstLast |
| FirstPageText | string | ✅ Match | Defaults to "First" |
| PreviousPageText | string | ✅ Match | Defaults to "Previous" |
| NextPageText | string | ✅ Match | Defaults to "Next" |
| LastPageText | string | ✅ Match | Defaults to "Last" |
| ShowFirstLastButtons | bool | ✅ Match | Defaults to true |
| ShowPreviousNextButtons | bool | ✅ Match | Defaults to true |
| ShowNumericButtons | bool | ✅ Match | Defaults to true |
| Fields | DataPagerFieldCollection | 🔴 Missing | Web Forms uses DataPagerField objects (NumericPagerField, NextPreviousPagerField, TemplatePagerField) |
| PagedControlID | string | 🔴 Missing | Web Forms links pager to a specific control by ID |
| QueryStringField | string | 🔴 Missing | URL-based paging |
| EnableViewState | bool | N/A | Server-only |
| EnableTheming | bool | N/A | Server-only |
| SkinID | string | N/A | Server-only |
| ViewState | StateBag | N/A | Server-only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| PageIndexChanging | EventCallback<PageChangedEventArgs> | ✅ Match | `OnPageIndexChanging` — supports cancellation |
| PageIndexChanged | EventCallback<PageChangedEventArgs> | ✅ Match | `OnPageIndexChanged` — fired after change |
| PageIndexChanged (two-way) | — | ✅ Match | `PageIndexChanged` EventCallback<int> for two-way binding |
| Init | EventHandler | ✅ Match | Inherited (OnInit) |
| Load | EventHandler | ✅ Match | Inherited (OnLoad) |
| PreRender | EventHandler | ✅ Match | Inherited (OnPreRender) |
| Unload | EventHandler | ✅ Match | Inherited (OnUnload) |
| Disposed | EventHandler | ✅ Match | Inherited (OnDisposed) |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| SetPageProperties() | void | 🔴 Missing | Web Forms method for programmatic paging |
| Focus() | void | N/A | Server-only |
| DataBind() | void | N/A | Server-only |

## HTML Output Comparison

| Aspect | Web Forms | Blazor |
|--------|-----------|--------|
| Root element | `<div>` or inline | `<div>` with id/class/style ✅ |
| Page numbers | `<a>` links | `<a href="javascript:void(0)">` ✅ |
| Current page | `<span>` (non-clickable) | `<span class="aspNetCurrentPage">` ✅ |
| Disabled buttons | CSS class | `class="aspNetDisabled"` + disabled ✅ |
| Navigation buttons | First/Prev/Next/Last text links | First/Prev/Next/Last text links ✅ |

HTML output matches well for the default pager rendering.

## Summary

- **Matching:** 27 properties, 7 events
- **Needs Work:** 0
- **Missing:** 3 properties (Fields collection, PagedControlID, QueryStringField), 0 events
- **N/A (server-only):** ~4 items

DataPager is well-implemented. The core paging logic (page buttons, first/last/prev/next, configurable button counts, pager modes) is complete. Main gap is the DataPagerField collection model — Web Forms allows composing multiple pager field types (NumericPagerField, NextPreviousPagerField, TemplatePagerField) into a Fields collection. The Blazor version uses direct properties and ChildContent for template support instead. QueryStringField (URL-based paging) is also missing.
