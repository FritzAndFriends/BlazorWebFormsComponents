>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# ImageMap — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.imagemap?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.ImageMap`
**Implementation Status:** ✅ Implemented

## Properties

### Control-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| HotSpotMode | HotSpotMode | ✅ Match | Default behavior for hotspots (Navigate, PostBack, Inactive, NotSet) |
| HotSpots | HotSpotCollection | ✅ Match | `List<HotSpot>` — supports Circle, Rectangle, Polygon hotspots |
| Target | string | ✅ Match | Default target for navigation hotspots |

### Image Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ImageUrl | string | ✅ Match | Image source URL |
| AlternateText | string | ✅ Match | Alt text for accessibility |
| ImageAlign | ImageAlign | ✅ Match | Alignment enum; rendered as `align` |
| DescriptionUrl | string | ✅ Match | Rendered as `longdesc` |
| GenerateEmptyAlternateText | bool | ✅ Match | Generates `alt=""` when true |
| ToolTip | string | ✅ Match | Rendered as `title` |

### WebControl Inherited Properties (from BaseStyledComponent)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | string | 🔴 Missing | Not in BaseStyledComponent |
| BackColor | Color | ✅ Match | From BaseStyledComponent |
| BorderColor | Color | ✅ Match | From BaseStyledComponent |
| BorderStyle | BorderStyle | ✅ Match | From BaseStyledComponent |
| BorderWidth | Unit | ✅ Match | From BaseStyledComponent |
| CssClass | string | ✅ Match | From BaseStyledComponent |
| Enabled | bool | ✅ Match | From BaseWebFormsComponent; disables hotspots when false |
| Font | FontInfo | ✅ Match | From BaseStyledComponent |
| ForeColor | Color | ✅ Match | From BaseStyledComponent |
| Height | Unit | ✅ Match | From BaseStyledComponent |
| Width | Unit | ✅ Match | From BaseStyledComponent |
| TabIndex | short | ✅ Match | From BaseWebFormsComponent |
| Style | CssStyleCollection | ✅ Match | Computed from BaseStyledComponent |

### Control Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | From BaseWebFormsComponent; rendered as `id` on `<img>` |
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
| Click | ImageMapEventHandler | ✅ Match | `EventCallback<ImageMapEventArgs> OnClick` (for PostBack hotspots) |
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

Web Forms renders:
```html
<img src="..." usemap="#MapID" alt="..." id="..." />
<map name="MapID">
  <area shape="rect" coords="..." href="..." alt="..." target="..." />
  <area shape="circle" coords="..." href="..." alt="..." />
  <area shape="poly" coords="..." href="javascript:__doPostBack(...)" alt="..." />
</map>
```

The Blazor component matches this structure:
- `<img>` with `usemap` attribute pointing to `<map>`
- `<map>` with `<area>` children for each hotspot
- Navigation hotspots render `href` and `target`
- PostBack hotspots render `@onclick` handlers with `preventDefault`
- Inactive/disabled hotspots render `nohref`

The map ID uses `Guid.NewGuid()` for instance uniqueness (per team decision — no static counters).

Hotspot types supported: `CircleHotSpot`, `RectangleHotSpot`, `PolygonHotSpot` — all three Web Forms types.

Each hotspot supports: `AlternateText`, `NavigateUrl`, `Target`, `HotSpotMode`, `PostBackValue`, `TabIndex`, `AccessKey`.

## Summary

- **Matching:** 23 properties, 5 events
- **Needs Work:** 0 properties, 0 events
- **Missing:** 1 property (AccessKey), 0 events
- **N/A (server-only):** 7 items
