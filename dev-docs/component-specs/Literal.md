>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# Literal — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.literal?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.Literal`
**Implementation Status:** ✅ Implemented

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| Text | `string` | ✅ Match | `[Parameter]` — default empty string |
| Mode | `LiteralMode` | ✅ Match | `[Parameter]` using `LiteralMode` enum; default `Encode` matches Web Forms |
| ID | `string` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| Visible | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| Enabled | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` (not relevant for Literal, but present) |
| EnableViewState | `bool` | N/A | Obsolete, accepted but ignored |
| EnableTheming | `bool` | N/A | Obsolete, accepted but ignored |
| SkinID | `string` | N/A | Obsolete, accepted but ignored |
| ViewState | `StateBag` | N/A | Server-side only |
| ClientID | `string` | N/A | Literal renders no wrapper element; ClientID has no use |

Note: Web Forms `Literal` inherits from `Control`, **not** `WebControl`. It has no style properties (CssClass, BackColor, etc.) by design. The Blazor implementation correctly inherits from `BaseWebFormsComponent` (no style support), matching the original.

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
| Focus() | `void Focus()` | N/A | Not applicable — Literal renders no focusable element |
| FindControl() | `Control FindControl(string)` | ✅ Match | On `BaseWebFormsComponent` |

## HTML Output Comparison

**Web Forms** renders raw text or HTML-encoded text with no wrapper element:
```html
Hello World
```

In `LiteralMode.Encode`:
```html
&lt;b&gt;Hello&lt;/b&gt;
```

**Blazor** renders the same:
- `Mode = LiteralMode.Encode` → HTML-encoded via `WebUtility.HtmlEncode`
- `Mode = LiteralMode.PassThrough` → raw markup via `(MarkupString)`

✅ HTML output matches. No wrapper element rendered in either case.

## Summary

- **Matching:** 5 properties, 6 events
- **Needs Work:** 0 properties, 0 events
- **Missing:** 0 properties, 0 events
- **N/A (server-only):** 5 items
