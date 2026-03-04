>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# BulletedList — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.bulletedlist?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.BulletedList<TItem>`
**Implementation Status:** ✅ Implemented

## Properties

### Control-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| BulletStyle | BulletStyle | ✅ Match | Enum with all values (Disc, Circle, Square, Numbered, etc.) |
| BulletImageUrl | string | ✅ Match | Custom image for bullets |
| DisplayMode | BulletedListDisplayMode | ✅ Match | Text, HyperLink, LinkButton |
| FirstBulletNumber | int | ✅ Match | Starting number for ordered lists |
| Target | string | ✅ Match | Target for hyperlinks |

### ListControl Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| Items | ListItemCollection | ✅ Match | Via `StaticItems` parameter |
| DataTextField | string | ✅ Match | Maps data to text |
| DataValueField | string | ✅ Match | Maps data to value |
| DataTextFormatString | string | 🔴 Missing | No format string support |
| DataSource | object | ✅ Match | Via DataBoundComponent |
| DataMember | string | ✅ Match | Via DataBoundComponent |
| DataSourceID | string | 🔴 Missing | No server-side DataSource controls |
| AppendDataBoundItems | bool | 🔴 Missing | Always appends static + data items |
| SelectedIndex | int | 🔴 Missing | Not applicable for bulleted list (read-only in WF too) |
| SelectedItem | ListItem | 🔴 Missing | Not applicable for bulleted list |
| SelectedValue | string | 🔴 Missing | Not applicable for bulleted list |
| AutoPostBack | bool | 🔴 Missing | Not applicable (bulleted list has no selection) |
| CausesValidation | bool | 🔴 Missing | Not implemented |
| ValidationGroup | string | 🔴 Missing | Not implemented |

### WebControl Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | string | 🔴 Missing | Not in base class or IStyle |
| BackColor | Color | ✅ Match | Via IStyle implementation |
| BorderColor | Color | ✅ Match | Via IStyle implementation |
| BorderStyle | BorderStyle | ✅ Match | Via IStyle implementation |
| BorderWidth | Unit | ✅ Match | Via IStyle implementation |
| CssClass | string | ✅ Match | Via IStyle implementation |
| Enabled | bool | ✅ Match | From BaseWebFormsComponent; propagates to items |
| Font | FontInfo | ✅ Match | Via IStyle implementation |
| ForeColor | Color | ✅ Match | Via IStyle implementation |
| Height | Unit | ✅ Match | Via IStyle implementation |
| Width | Unit | ✅ Match | Via IStyle implementation |
| TabIndex | short | ✅ Match | From BaseWebFormsComponent |
| ToolTip | string | 🔴 Missing | Not implemented on this component |
| Style | CssStyleCollection | ✅ Match | Computed from IStyle properties |

### Control Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | From BaseWebFormsComponent; rendered on list element |
| ClientID | string | ✅ Match | Rendered as `id` on `<ul>`/`<ol>` |
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
| Click | BulletedListEventHandler | ✅ Match | `EventCallback<BulletedListEventArgs> OnClick` (LinkButton mode) |
| DataBinding | EventHandler | ✅ Match | Via base class |
| Init | EventHandler | ✅ Match | Via base class |
| Load | EventHandler | ✅ Match | Via base class |
| PreRender | EventHandler | ✅ Match | Via base class |
| Unload | EventHandler | ✅ Match | Via base class |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | void | N/A | Server-only |
| Focus() | void | N/A | Server-only |

## HTML Output Comparison

Web Forms renders `<ul>` for unordered styles and `<ol>` for ordered styles (Numbered, LowerAlpha, UpperAlpha, LowerRoman, UpperRoman). Each item is a `<li>` containing either plain text in a `<span>`, an `<a>` hyperlink, or a clickable `<a>` link button. The Blazor component matches this structure. The `type` attribute is rendered on `<ol>` for ordered lists.

Custom image bullets use `list-style-image` CSS — matches Web Forms behavior.

## Summary

- **Matching:** 19 properties, 6 events
- **Needs Work:** 0 properties, 0 events
- **Missing:** 10 properties (AccessKey, ToolTip, DataTextFormatString, DataSourceID, AppendDataBoundItems, SelectedIndex, SelectedItem, SelectedValue, AutoPostBack, CausesValidation, ValidationGroup), 0 events
- **N/A (server-only):** 7 items
