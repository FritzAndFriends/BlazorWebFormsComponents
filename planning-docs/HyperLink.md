# HyperLink — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.hyperlink?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.HyperLink`
**Implementation Status:** ✅ Implemented

## Properties

### Control-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| NavigateUrl | string | ⚠️ Needs Work | Named `NavigationUrl` in Blazor — **name mismatch** vs Web Forms `NavigateUrl` |
| Target | string | ✅ Match | Target window/frame; defaults to empty string |
| Text | string | ✅ Match | Link text content |
| ImageUrl | string | 🔴 Missing | Web Forms supports showing an image instead of text |
| ToolTip | string | ✅ Match | Rendered as `title` attribute |

### WebControl Inherited Properties (from BaseStyledComponent)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | string | 🔴 Missing | Not in BaseStyledComponent |
| BackColor | Color | ✅ Match | From BaseStyledComponent |
| BorderColor | Color | ✅ Match | From BaseStyledComponent |
| BorderStyle | BorderStyle | ✅ Match | From BaseStyledComponent |
| BorderWidth | Unit | ✅ Match | From BaseStyledComponent |
| CssClass | string | ✅ Match | From BaseStyledComponent |
| Enabled | bool | ✅ Match | From BaseWebFormsComponent |
| Font | FontInfo | ✅ Match | From BaseStyledComponent |
| ForeColor | Color | ✅ Match | From BaseStyledComponent |
| Height | Unit | ✅ Match | From BaseStyledComponent |
| Width | Unit | ✅ Match | From BaseStyledComponent |
| TabIndex | short | ✅ Match | From BaseWebFormsComponent |
| Style | CssStyleCollection | ✅ Match | Computed from BaseStyledComponent |

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
| Init | EventHandler | ✅ Match | Via base class |
| Load | EventHandler | ✅ Match | Via base class |
| PreRender | EventHandler | ✅ Match | Via base class |
| Unload | EventHandler | ✅ Match | Via base class |

**Note:** HyperLink has no control-specific events in Web Forms — it's purely a navigation control.

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| Focus() | void | N/A | Server-only |
| DataBind() | void | N/A | Server-only |

## HTML Output Comparison

Web Forms renders `<a id="..." href="..." target="..." title="..." class="..." style="...">Text</a>`. The Blazor component matches this:
```html
<a id="..." href="..." target="..." title="..." class="..." style="...">Text</a>
```

When `NavigateUrl` is null, Web Forms renders the `<a>` tag without an `href` attribute. The Blazor component matches this behavior — rendering `<a>` without `href` when `NavigationUrl` is null.

Missing: Web Forms supports `ImageUrl` to render `<a><img src="..." /></a>` instead of text. The Blazor component only supports text content.

## Summary

- **Matching:** 17 properties, 4 events
- **Needs Work:** 1 property (NavigateUrl → NavigationUrl name mismatch)
- **Missing:** 2 properties (AccessKey, ImageUrl), 0 events
- **N/A (server-only):** 7 items
