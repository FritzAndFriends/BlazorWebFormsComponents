>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# Image — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.image?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.Image`
**Implementation Status:** ✅ Implemented

## Properties

### Control-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ImageUrl | string | ✅ Match | Image source URL |
| AlternateText | string | ✅ Match | Alt text for accessibility |
| ImageAlign | ImageAlign | ✅ Match | Alignment enum; renders `align` attribute |
| DescriptionUrl | string | ✅ Match | Rendered as `longdesc` attribute |
| GenerateEmptyAlternateText | bool | ✅ Match | Generates `alt=""` when true |
| ToolTip | string | ✅ Match | Rendered as `title` attribute |

### WebControl Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | string | 🔴 Missing | Not in BaseWebFormsComponent |
| BackColor | Color | 🔴 Missing | Image inherits BaseWebFormsComponent, not BaseStyledComponent |
| BorderColor | Color | 🔴 Missing | Not available — wrong base class |
| BorderStyle | BorderStyle | 🔴 Missing | Not available — wrong base class |
| BorderWidth | Unit | 🔴 Missing | Not available — wrong base class |
| CssClass | string | 🔴 Missing | Not available — wrong base class |
| Font | FontInfo | 🔴 Missing | Not available — wrong base class |
| ForeColor | Color | 🔴 Missing | Not available — wrong base class |
| Height | Unit | 🔴 Missing | Not available — wrong base class |
| Width | Unit | 🔴 Missing | Not available — wrong base class |
| TabIndex | short | ✅ Match | From BaseWebFormsComponent |
| ToolTip | string | ✅ Match | Directly on component (not from base) |
| Style | CssStyleCollection | 🔴 Missing | No style computation — wrong base class |
| Enabled | bool | ✅ Match | From BaseWebFormsComponent |

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

**Note:** Image has no control-specific events in Web Forms.

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| Focus() | void | N/A | Server-only |
| DataBind() | void | N/A | Server-only |

## HTML Output Comparison

Web Forms renders `<img src="..." alt="..." id="..." style="..." class="..." align="..." longdesc="..." title="..." />`.

The Blazor component renders the same structure using `StringBuilder`:
```html
<img src="..." id="..." alt="..." longdesc="..." title="..." align="..." />
```

**Critical gap:** The Blazor `Image` component inherits `BaseWebFormsComponent` instead of `BaseStyledComponent`. In Web Forms, `Image` inherits from `WebControl` which provides CssClass, Style, BackColor, ForeColor, Font, Width, Height, etc. The Blazor component is missing ALL style properties. This is a significant base class mismatch.

Note: The sibling `ImageMap` component correctly inherits `BaseStyledComponent` (per team decision). `Image` should likely follow the same pattern.

## Summary

- **Matching:** 10 properties, 4 events
- **Needs Work:** 0 properties, 0 events
- **Missing:** 11 properties (AccessKey, BackColor, BorderColor, BorderStyle, BorderWidth, CssClass, Font, ForeColor, Height, Width, Style — all due to wrong base class), 0 events
- **N/A (server-only):** 7 items
