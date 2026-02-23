# LinkButton — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.linkbutton?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.LinkButton`
**Implementation Status:** ✅ Implemented

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| Text | `string` | ✅ Match | Inherited from `ButtonBaseComponent` |
| CommandName | `string` | ✅ Match | Inherited from `ButtonBaseComponent` |
| CommandArgument | `object` | ✅ Match | Inherited from `ButtonBaseComponent` |
| CausesValidation | `bool` | ✅ Match | Inherited from `ButtonBaseComponent` |
| ValidationGroup | `string` | ✅ Match | Inherited from `ButtonBaseComponent` |
| PostBackUrl | `string` | ✅ Match | Inherited from `ButtonBaseComponent`; renders `href` when set |
| OnClientClick | `string` | ✅ Match | Inherited from `ButtonBaseComponent` |
| ID | `string` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| ClientID | `string` (read-only) | ✅ Match | Computed via `ComponentIdGenerator` |
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
| Style | `CssStyleCollection` | ⚠️ Needs Work | Computed internally; not directly settable as parameter |
| EnableViewState | `bool` | N/A | Obsolete, accepted but ignored |
| EnableTheming | `bool` | N/A | Obsolete, accepted but ignored |
| SkinID | `string` | N/A | Obsolete, accepted but ignored |
| ViewState | `StateBag` | N/A | Server-side only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| Click | `EventHandler` | ✅ Match | `OnClick` EventCallback<MouseEventArgs> |
| Command | `CommandEventHandler` | ✅ Match | `OnCommand` EventCallback<CommandEventArgs> |
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

**Web Forms** renders a hyperlink-styled element with JavaScript postback:
```html
<a id="LinkButton1" href="javascript:__doPostBack('LinkButton1','')">Click Me</a>
```

**Blazor** renders a simplified anchor:
```html
<a id="LinkButton1">Click Me</a>
```

⚠️ The Web Forms version uses `javascript:__doPostBack` for postback; Blazor uses `@onclick` Blazor event binding instead. The visible HTML is similar but the click mechanism differs.

## Summary

- **Matching:** 19 properties, 8 events
- **Needs Work:** 1 property (Style)
- **Missing:** 2 properties (AccessKey, ToolTip), 0 events
- **N/A (server-only):** 4 items
