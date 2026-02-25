# ImageButton — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.imagebutton?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.ImageButton`
**Implementation Status:** ✅ Implemented

## Properties

### Control-Specific Properties (from Image)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ImageUrl | string | ✅ Match | Image source URL; rendered as `src` |
| AlternateText | string | ✅ Match | Alt text; rendered as `alt` |
| ImageAlign | ImageAlign | ✅ Match | Alignment enum; rendered as `align` |
| DescriptionUrl | string | ✅ Match | Rendered as `longdesc` |
| ToolTip | string | ✅ Match | Rendered as `title` |
| GenerateEmptyAlternateText | bool | 🔴 Missing | Not on ImageButton (exists on Image and ImageMap) |

### Button Properties (from IButtonControl / ButtonBaseComponent)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| Text | string | ✅ Match | From ButtonBaseComponent (not rendered visually for image buttons) |
| CommandName | string | ✅ Match | From ButtonBaseComponent |
| CommandArgument | object | ✅ Match | From ButtonBaseComponent |
| CausesValidation | bool | ✅ Match | From ButtonBaseComponent |
| ValidationGroup | string | ✅ Match | From ButtonBaseComponent |
| PostBackUrl | string | ✅ Match | From ButtonBaseComponent |
| OnClientClick | string | ✅ Match | From ButtonBaseComponent |

### WebControl Inherited Properties (from BaseStyledComponent via ButtonBaseComponent)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | string | 🔴 Missing | Not in BaseStyledComponent |
| BackColor | Color | ✅ Match | From BaseStyledComponent |
| BorderColor | Color | ✅ Match | From BaseStyledComponent |
| BorderStyle | BorderStyle | ✅ Match | From BaseStyledComponent |
| BorderWidth | Unit | ✅ Match | From BaseStyledComponent |
| CssClass | string | ✅ Match | From BaseStyledComponent; rendered as `class` |
| Enabled | bool | ✅ Match | From BaseWebFormsComponent; renders `disabled` attribute |
| Font | FontInfo | ✅ Match | From BaseStyledComponent |
| ForeColor | Color | ✅ Match | From BaseStyledComponent |
| Height | Unit | ✅ Match | From BaseStyledComponent |
| Width | Unit | ✅ Match | From BaseStyledComponent |
| TabIndex | short | ✅ Match | From BaseWebFormsComponent |
| Style | CssStyleCollection | ✅ Match | Computed from BaseStyledComponent; rendered as `style` |

### Control Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | From BaseWebFormsComponent; rendered as `id` |
| ClientID | string | ✅ Match | From BaseWebFormsComponent |
| Visible | bool | ✅ Match | From BaseWebFormsComponent |
| EnableViewState | bool | N/A | Server-only |
| ViewState | StateBag | N/A | Server-only |
| EnableTheming | bool | N/A | Server-only |
| SkinID | string | N/A | Server-only |
| Page | Page | N/A | Server-only |
| NamingContainer | Control | N/A | Server-only |
| UniqueID | string | N/A | Server-only |
| ClientIDMode | ClientIDMode | N/A | Server-only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| Click | ImageClickEventHandler | ✅ Match | `EventCallback<MouseEventArgs> OnClick` (no X/Y coordinates) |
| Command | CommandEventHandler | ✅ Match | `EventCallback<CommandEventArgs> OnCommand` |
| Init | EventHandler | ✅ Match | Via base class |
| Load | EventHandler | ✅ Match | Via base class |
| PreRender | EventHandler | ✅ Match | Via base class |
| Unload | EventHandler | ✅ Match | Via base class |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| Focus() | void | N/A | Server-only |
| DataBind() | void | N/A | Server-only |

## HTML Output Comparison

Web Forms renders `<input type="image" src="..." alt="..." />`. The Blazor component matches this:
```html
<input type="image" src="..." alt="..." longdesc="..." style="..." class="..." />
```

The element is built programmatically via `RenderTreeBuilder` for dynamic attribute control. The `disabled`, `title`, and `align` attributes are conditionally rendered.

Note: Web Forms `ImageButton.Click` provides `ImageClickEventArgs` with X/Y coordinates of the click point. The Blazor version uses `MouseEventArgs` which also has coordinate data, so this is functionally equivalent.

## Summary

- **Matching:** 24 properties, 6 events
- **Needs Work:** 0 properties, 0 events
- **Missing:** 2 properties (AccessKey, GenerateEmptyAlternateText), 0 events
- **N/A (server-only):** 7 items
