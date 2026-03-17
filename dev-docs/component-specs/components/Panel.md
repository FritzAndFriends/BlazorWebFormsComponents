>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# Panel — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.panel?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.Panel`
**Implementation Status:** ✅ Implemented

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| DefaultButton | `string` | ✅ Match | `[Parameter]` — accepted but not wired to form submit behavior |
| Direction | `ContentDirection` | ✅ Match | `[Parameter]` using `ContentDirection` enum; renders `dir` attribute |
| GroupingText | `string` | ✅ Match | `[Parameter]` — renders `<fieldset>/<legend>` when set |
| HorizontalAlign | `HorizontalAlign` | ✅ Match | `[Parameter]` — renders `text-align` CSS |
| ScrollBars | `ScrollBars` | ✅ Match | `[Parameter]` — renders overflow CSS properties |
| Wrap | `bool` | ✅ Match | `[Parameter]` — default `true`; renders `white-space:nowrap` when false |
| BackImageUrl | `string` | 🔴 Missing | Not implemented in Blazor Panel |
| ChildContent | — | ✅ Match | Blazor-specific `RenderFragment` |
| ID | `string` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| ClientID | `string` (read-only) | ✅ Match | Rendered on the element |
| Visible | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| Enabled | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| TabIndex | `short` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| CssClass | `string` | ✅ Match | Inherited from `BaseStyledComponent` |
| BackColor | `Color` | ✅ Match | Inherited from `BaseStyledComponent` |
| ForeColor | `Color` | ✅ Match | Inherited from `BaseStyledComponent` |
| BorderColor | `Color` | ✅ Match | Inherited from `BaseStyledComponent` |
| BorderStyle | `BorderStyle` | ✅ Match | Inherited from `BaseStyledComponent` |
| BorderWidth | `Unit` | ✅ Match | Inherited from `BaseStyledComponent` |
| Font | `FontInfo` | ✅ Match | Inherited from `BaseStyledComponent` |
| Height | `Unit` | ✅ Match | Inherited from `BaseStyledComponent` |
| Width | `Unit` | ✅ Match | Inherited from `BaseStyledComponent` |
| AccessKey | `string` | 🔴 Missing | Not in any base class |
| ToolTip | `string` | 🔴 Missing | Not in any base class |
| Style | `CssStyleCollection` | ⚠️ Needs Work | Computed internally via `BuildStyle()` combining base + overflow + alignment |
| EnableViewState | `bool` | N/A | Obsolete, accepted but ignored |
| EnableTheming | `bool` | N/A | Obsolete, accepted but ignored |
| SkinID | `string` | N/A | Obsolete, accepted but ignored |
| ViewState | `StateBag` | N/A | Server-side only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| Init | `EventHandler` | ✅ Match | `OnInit` on base |
| Load | `EventHandler` | ✅ Match | `OnLoad` on base |
| PreRender | `EventHandler` | ✅ Match | `OnPreRender` on base |
| Unload | `EventHandler` | ✅ Match | `OnUnload` on base |
| Disposed | `EventHandler` | ✅ Match | `OnDisposed` on base |
| DataBinding | `EventHandler` | ✅ Match | `OnDataBinding` on base |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | `void DataBind()` | N/A | No-op in Blazor |
| Focus() | `void Focus()` | 🔴 Missing | Would require JS interop |
| FindControl() | `Control FindControl(string)` | ✅ Match | On `BaseWebFormsComponent` |

## HTML Output Comparison

**Web Forms** renders a `<div>` (or `<fieldset>` with GroupingText):
```html
<div id="Panel1">Content here</div>
```
With GroupingText:
```html
<fieldset id="Panel1"><legend>Group</legend>Content here</fieldset>
```

**Blazor** renders the same:
```html
<div id="Panel1" class="" style="">Content here</div>
```
With GroupingText:
```html
<fieldset id="Panel1" class="" style="" dir=""><legend>Group</legend>Content here</fieldset>
```

✅ HTML output matches. The `<fieldset>/<legend>` pattern for `GroupingText` is correctly implemented.

## Summary

- **Matching:** 20 properties, 6 events
- **Needs Work:** 1 property (Style)
- **Missing:** 3 properties (BackImageUrl, AccessKey, ToolTip), 0 events
- **N/A (server-only):** 4 items
