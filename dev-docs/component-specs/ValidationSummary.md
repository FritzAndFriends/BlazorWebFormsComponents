>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# ValidationSummary — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.validationsummary?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.Validations.AspNetValidationSummary`
**Implementation Status:** ✅ Implemented

> **Note:** The Blazor component is named `AspNetValidationSummary` (not `ValidationSummary`) to avoid conflicts with Blazor's built-in `ValidationSummary` component.

## Properties

### ValidationSummary-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| DisplayMode | `ValidationSummaryDisplayMode` | ✅ Match | `[Parameter] public ValidationSummaryDisplayMode DisplayMode` — defaults to `BulletList` |
| EnableClientScript | `bool` | N/A | Server-side concept |
| ForeColor | `Color` | ✅ Match | Via `BaseStyledComponent.ForeColor` (Web Forms defaults to `Color.Red`) |
| HeaderText | `string` | 🔴 Missing | Header text displayed above the summary |
| ShowMessageBox | `bool` | 🔴 Missing | Display errors in a JavaScript alert box |
| ShowSummary | `bool` | 🔴 Missing | Controls whether summary is displayed inline (default `true`) |
| ShowValidationErrors | `bool` | 🔴 Missing | Controls whether validation errors from validators are displayed |
| ValidationGroup | `string` | 🔴 Missing | Filter errors to a specific validation group |

### Inherited from WebControl (via BaseStyledComponent)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | `string` | 🔴 Missing | Not implemented |
| BackColor | `Color` | ✅ Match | Via `BaseStyledComponent.BackColor` |
| BorderColor | `Color` | ✅ Match | Via `BaseStyledComponent.BorderColor` |
| BorderStyle | `BorderStyle` | ✅ Match | Via `BaseStyledComponent.BorderStyle` |
| BorderWidth | `Unit` | ✅ Match | Via `BaseStyledComponent.BorderWidth` |
| CssClass | `string` | ✅ Match | Via `BaseStyledComponent.CssClass` |
| Enabled | `bool` | ✅ Match | Via `BaseWebFormsComponent.Enabled` |
| Font | `FontInfo` | ✅ Match | Via `BaseStyledComponent.Font` |
| Height | `Unit` | ✅ Match | Via `BaseStyledComponent.Height` |
| Style | `CssStyleCollection` | ⚠️ Needs Work | Uses `Style` property from `BaseStyledComponent` |
| TabIndex | `short` | ✅ Match | Via `BaseWebFormsComponent.TabIndex` |
| ToolTip | `string` | 🔴 Missing | Not implemented |
| Width | `Unit` | ✅ Match | Via `BaseStyledComponent.Width` |

### Inherited from Control

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | `string` | ✅ Match | Via `BaseWebFormsComponent.ID` |
| Visible | `bool` | ✅ Match | Via `BaseWebFormsComponent.Visible` |
| ClientID | `string` | ✅ Match | Via `BaseWebFormsComponent.ClientID` |
| EnableViewState | `bool` | N/A | Server-side only |
| ViewState | `StateBag` | N/A | Server-side only |
| Parent | `Control` | ✅ Match | Via `BaseWebFormsComponent.Parent` |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| DataBinding | `EventHandler` | ✅ Match | Via `BaseWebFormsComponent.OnDataBinding` |
| Init | `EventHandler` | ✅ Match | Via `BaseWebFormsComponent.OnInit` |
| Load | `EventHandler` | ✅ Match | Via `BaseWebFormsComponent.OnLoad` |
| PreRender | `EventHandler` | ✅ Match | Via `BaseWebFormsComponent.OnPreRender` |
| Unload | `EventHandler` | ✅ Match | Via `BaseWebFormsComponent.OnUnload` |
| Disposed | `EventHandler` | ✅ Match | Via `BaseWebFormsComponent.OnDisposed` |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | `void DataBind()` | N/A | No-op stub |
| Focus() | `void Focus()` | 🔴 Missing | Client focus management |
| FindControl() | `Control FindControl(string)` | ✅ Match | Via `BaseWebFormsComponent.FindControl` |

## HTML Output Comparison

Web Forms renders a `<div>` containing either a `<ul>` (BulletList), `<br>`-separated list (List), or single paragraph (SingleParagraph). Blazor renders a `<div>` with the same three display modes. The rendering logic is functionally equivalent.

Key difference: Blazor extracts error messages by splitting on comma (`x.Split(',')[1]`), which is a fragile approach — if an error message contains a comma, it will be truncated.

The `IsValid` property logic appears inverted: `IsValid => CurrentEditContext.GetValidationMessages().Any()` returns `true` when there ARE errors, but in the `.razor` template it only shows content when `IsValid` is `true`. This seems to show messages when there are errors, which is correct behavior, but the naming is confusing.

## Summary

- **Matching:** 14 properties, 6 events
- **Needs Work:** 1 property (Style)
- **Missing:** 7 properties (HeaderText, ShowMessageBox, ShowSummary, ShowValidationErrors, ValidationGroup, ToolTip, AccessKey), 1 method (Focus)
- **N/A (server-only):** 4 items (EnableClientScript, EnableViewState, ViewState)
