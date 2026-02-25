# HiddenField — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.hiddenfield?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.HiddenField`
**Implementation Status:** ✅ Implemented

## Properties

### Control-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| Value | string | ✅ Match | Hidden field value |

### Control Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | From BaseWebFormsComponent; rendered as `id` |
| ClientID | string | ✅ Match | From BaseWebFormsComponent |
| Visible | bool | ✅ Match | From BaseWebFormsComponent |
| Enabled | bool | ✅ Match | From BaseWebFormsComponent (not applicable to hidden fields) |
| TabIndex | short | ✅ Match | From BaseWebFormsComponent (not applicable to hidden fields) |
| EnableViewState | bool | N/A | Server-only |
| ViewState | StateBag | N/A | Server-only |
| EnableTheming | bool | N/A | Server-only |
| SkinID | string | N/A | Server-only |
| Page | Page | N/A | Server-only |
| NamingContainer | Control | N/A | Server-only |
| UniqueID | string | N/A | Server-only |
| ClientIDMode | ClientIDMode | N/A | Server-only |

**Note:** Web Forms `HiddenField` inherits directly from `Control`, not `WebControl`, so it does NOT have style properties (BackColor, CssClass, Font, etc.). The Blazor implementation correctly inherits from `BaseWebFormsComponent` (not `BaseStyledComponent`), matching this hierarchy.

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| ValueChanged | EventHandler | ✅ Match | `EventCallback<EventArgs> OnValueChanged` |
| Init | EventHandler | ✅ Match | Via base class |
| Load | EventHandler | ✅ Match | Via base class |
| PreRender | EventHandler | ✅ Match | Via base class |
| Unload | EventHandler | ✅ Match | Via base class |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| Focus() | void | N/A | Server-only; throws NotSupportedException in Web Forms too |
| DataBind() | void | N/A | Server-only |
| LoadPostData() | bool | N/A | Server-only (IPostBackDataHandler) |
| RaisePostDataChangedEvent() | void | N/A | Server-only (IPostBackDataHandler) |

## HTML Output Comparison

Web Forms renders `<input type="hidden" id="..." name="..." value="..." />`. The Blazor component renders:
```html
<input id="..." type="hidden" value="..." />
```

The output is nearly identical. The Blazor version uses `@onchange` for value change detection rather than postback-based change tracking.

Note: Web Forms renders both `id` and `name` attributes; the Blazor component only renders `id`.

## Summary

- **Matching:** 4 properties, 5 events
- **Needs Work:** 0 properties, 0 events
- **Missing:** 0 properties, 0 events
- **N/A (server-only):** 9 items
