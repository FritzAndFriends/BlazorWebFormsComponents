>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# AdRotator — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.adrotator?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.AdRotator`
**Implementation Status:** ✅ Implemented

## Properties

### Control-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AdvertisementFile | string | ✅ Match | Loads ads from XML file |
| AlternateTextField | string | ✅ Match | Default "AlternateText" |
| ImageUrlField | string | ✅ Match | Default "ImageUrl" |
| NavigateUrlField | string | ✅ Match | Default "NavigateUrl" |
| KeywordFilter | string | ✅ Match | Filters ads by keyword |
| Target | string | ✅ Match | Link target window/frame |

### Data Binding Properties (from DataBoundComponent)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| DataSource | object | ✅ Match | Via `DataBoundComponent<Advertisment>` |
| DataMember | string | ✅ Match | Inherited from DataBoundComponent |
| DataSourceID | string | 🔴 Missing | No server-side DataSource controls in Blazor |
| Items | IEnumerable | ✅ Match | Via `DataBoundComponent<Advertisment>` |
| SelectMethod | delegate | ✅ Match | Model binding support |

### WebControl Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | string | 🔴 Missing | Not in base class or IStyle |
| BackColor | Color | ✅ Match | Via IStyle implementation |
| BorderColor | Color | ✅ Match | Via IStyle implementation |
| BorderStyle | BorderStyle | ✅ Match | Via IStyle implementation |
| BorderWidth | Unit | ✅ Match | Via IStyle implementation |
| CssClass | string | ✅ Match | Via IStyle implementation |
| Enabled | bool | ✅ Match | From BaseWebFormsComponent |
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
| ID | string | ✅ Match | From BaseWebFormsComponent |
| ClientID | string | ✅ Match | From BaseWebFormsComponent |
| Visible | bool | ✅ Match | From BaseWebFormsComponent |
| EnableViewState | bool | N/A | Server-only; marked obsolete |
| ViewState | StateBag | N/A | Server-only |
| EnableTheming | bool | N/A | Server-only; marked obsolete |
| SkinID | string | N/A | Server-only; marked obsolete |
| Page | Page | N/A | Server-only |
| Parent | Control | N/A | Server-only (Blazor has Parent cascade) |
| NamingContainer | Control | N/A | Server-only |
| UniqueID | string | N/A | Server-only |
| ClientIDMode | ClientIDMode | N/A | Server-only |
| ViewStateMode | ViewStateMode | N/A | Server-only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| AdCreated | AdCreatedEventHandler | ✅ Match | `EventCallback<AdCreatedEventArgs> OnAdCreated` |
| DataBinding | EventHandler | ✅ Match | `EventCallback<EventArgs> OnDataBinding` (base) |
| Init | EventHandler | ✅ Match | `EventCallback<EventArgs> OnInit` (base) |
| Load | EventHandler | ✅ Match | `EventCallback<EventArgs> OnLoad` (base) |
| PreRender | EventHandler | ✅ Match | `EventCallback<EventArgs> OnPreRender` (base) |
| Unload | EventHandler | ✅ Match | `EventCallback<EventArgs> OnUnload` (base) |
| Disposed | EventHandler | ✅ Match | `EventCallback<EventArgs> OnDisposed` (base) |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | void | N/A | Server-only; marked obsolete in base |
| Focus() | void | N/A | Server-only |
| FindControl(string) | Control | ✅ Match | `FindControl` in BaseWebFormsComponent |

## HTML Output Comparison

Web Forms renders `<a href="..."><img src="..." /></a>` — the Blazor component matches this output. The `<a>` tag includes `target`, `style`, and `class` attributes. The `<img>` tag includes `src`, `width`, `height`, and `alt` attributes from the advertisement data.

Note: Web Forms also renders an `id` attribute on the outer element. The Blazor component does not currently render `id` on the `<a>` tag.

## Summary

- **Matching:** 20 properties, 7 events
- **Needs Work:** 0 properties, 0 events
- **Missing:** 3 properties (AccessKey, ToolTip, DataSourceID), 0 events
- **N/A (server-only):** 9 items
