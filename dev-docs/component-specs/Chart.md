>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# Chart — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.datavisualization.charting.chart?view=netframework-4.8.1
**Blazor Component:** `BlazorWebFormsComponents.Chart`
**Implementation Status:** ⚠️ Partial

> Note: The Web Forms Chart control lives in `System.Web.UI.DataVisualization.Charting`, not `System.Web.UI.WebControls`. The Blazor implementation uses Chart.js via JS interop instead of server-side image rendering. This is an intentional architectural deviation — renders `<canvas>` instead of `<img>`.

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | Inherited from BaseWebFormsComponent |
| Visible | bool | ✅ Match | Inherited from BaseWebFormsComponent |
| CssClass | string | ✅ Match | Inherited from BaseStyledComponent |
| BackColor | Color | ✅ Match | Inherited from BaseStyledComponent |
| BorderColor | Color | ✅ Match | Inherited from BaseStyledComponent |
| BorderStyle | BorderStyle | ✅ Match | Inherited from BaseStyledComponent |
| BorderWidth | Unit | ✅ Match | Inherited from BaseStyledComponent |
| ForeColor | Color | ✅ Match | Inherited from BaseStyledComponent |
| Height | Unit | ✅ Match | Inherited from BaseStyledComponent |
| Width | Unit | ✅ Match | Inherited from BaseStyledComponent |
| Font | FontInfo | ✅ Match | Inherited from BaseStyledComponent |
| Enabled | bool | ✅ Match | Inherited from BaseWebFormsComponent |
| TabIndex | short | ✅ Match | Inherited from BaseWebFormsComponent |
| ChartAreas | ChartAreaCollection | ⚠️ Needs Work | Child component `ChartArea` registered via `RegisterChartArea()` — not a direct property |
| Series | SeriesCollection | ⚠️ Needs Work | Child component `ChartSeries` registered via `RegisterSeries()` — not a direct property |
| Legends | LegendCollection | ⚠️ Needs Work | Child component `ChartLegend` registered via `RegisterLegend()` — not a direct property |
| Titles | TitleCollection | ⚠️ Needs Work | Child component `ChartTitle` registered via `RegisterTitle()` — not a direct property |
| Palette | ChartColorPalette | ✅ Match | `ChartPalette` enum, defaults to BrightPastel |
| ImageType | ChartImageType | ⚠️ Needs Work | Parameter exists but marked "API compatibility only; not functional" |
| ChildContent | — | ✅ Match | Blazor-specific composition model |
| ChartWidth | string | ⚠️ Needs Work | Blazor-specific; Web Forms uses Width property |
| ChartHeight | string | ⚠️ Needs Work | Blazor-specific; Web Forms uses Height property |
| AlternateText | string | 🔴 Missing | Alt text for `<img>` — N/A for `<canvas>` |
| Annotations | AnnotationCollection | 🔴 Missing | Complex annotation system |
| AntiAliasing | AntiAliasingStyles | 🔴 Missing | Handled by Chart.js internally |
| BackGradientStyle | GradientStyle | 🔴 Missing | |
| BackHatchStyle | ChartHatchStyle | 🔴 Missing | |
| BackImage | string | 🔴 Missing | |
| BackImageAlignment | ChartImageAlignmentStyle | 🔴 Missing | |
| BackImageTransparentColor | Color | 🔴 Missing | |
| BackImageWrapMode | ChartImageWrapMode | 🔴 Missing | |
| BackSecondaryColor | Color | 🔴 Missing | |
| BorderlineColor | Color | 🔴 Missing | Chart-specific border |
| BorderlineDashStyle | ChartDashStyle | 🔴 Missing | |
| BorderlineWidth | int | 🔴 Missing | |
| BorderSkin | BorderSkin | 🔴 Missing | |
| Compression | int | 🔴 Missing | Image compression — N/A |
| DataManipulator | DataManipulator | 🔴 Missing | Server-side data manipulation |
| DataMember | string | 🔴 Missing | Handled differently in Blazor |
| DataSource | object | 🔴 Missing | Not inherited from DataBoundComponent |
| ImageLocation | string | 🔴 Missing | N/A for canvas |
| ImageStorageMode | ImageStorageMode | 🔴 Missing | N/A for canvas |
| IsSoftShadows | bool | 🔴 Missing | |
| MapAreas | MapAreasCollection | 🔴 Missing | |
| PaletteCustomColors | Color[] | 🔴 Missing | |
| RenderType | RenderType | 🔴 Missing | Always renders as canvas |
| RightToLeft | RightToLeft | 🔴 Missing | |
| Serializer | ChartSerializer | 🔴 Missing | |
| SuppressExceptions | bool | 🔴 Missing | |
| TextAntiAliasingQuality | TextAntiAliasingQuality | 🔴 Missing | |
| ToolTip | string | 🔴 Missing | |
| DataSourceID | string | N/A | Server-only |
| EnableViewState | bool | N/A | Server-only |
| EnableTheming | bool | N/A | Server-only |
| SkinID | string | N/A | Server-only |
| ViewState | StateBag | N/A | Server-only |
| Style | CssStyleCollection | N/A | Computed from styled base |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| Click | ImageMapEventHandler | 🔴 Missing | Canvas click not implemented |
| Customize | EventHandler | 🔴 Missing | |
| CustomizeLegend | EventHandler<CustomizeLegendEventArgs> | 🔴 Missing | |
| CustomizeMapAreas | EventHandler | 🔴 Missing | |
| FormatNumber | EventHandler | 🔴 Missing | |
| PostPaint | EventHandler<ChartPaintEventArgs> | 🔴 Missing | |
| PrePaint | EventHandler<ChartPaintEventArgs> | 🔴 Missing | |
| DataBinding | EventHandler | ✅ Match | Inherited from BaseWebFormsComponent |
| DataBound | EventHandler | 🔴 Missing | Not a DataBoundComponent |
| Init | EventHandler | ✅ Match | Inherited (OnInit) |
| Load | EventHandler | ✅ Match | Inherited (OnLoad) |
| PreRender | EventHandler | ✅ Match | Inherited (OnPreRender) |
| Unload | EventHandler | ✅ Match | Inherited (OnUnload) |
| Disposed | EventHandler | ✅ Match | Inherited (OnDisposed) |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | void | N/A | Server-only pattern |
| SaveImage() | void | 🔴 Missing | Canvas export not implemented |
| LoadTheme() | void | 🔴 Missing | |
| AlignDataPointsByAxisLabel() | void | 🔴 Missing | |
| ApplyPaletteColors() | void | 🔴 Missing | Handled by Chart.js |
| Focus() | void | N/A | Server-only |
| Dispose() | void | ✅ Match | Implemented via IAsyncDisposable |

## HTML Output Comparison

| Aspect | Web Forms | Blazor |
|--------|-----------|--------|
| Root element | `<img>` (rendered chart image) | `<div>` containing `<canvas>` |
| Chart rendering | Server-side GDI+, delivered as image | Client-side Chart.js on canvas |
| Interactivity | PostBack via image map areas | Chart.js native hover/click |
| Styling | Inline on `<img>` | CSS class on wrapper `<div>` |

This is an intentional architectural deviation. Chart.js provides richer interactivity than the original server-rendered image approach.

## Summary

- **Matching:** 14 properties (base styled + Palette), 6 events (lifecycle)
- **Needs Work:** 5 properties (collections as child components, ImageType stub, ChartWidth/ChartHeight)
- **Missing:** ~30 properties, 7 events (Click, Customize, paint events, etc.)
- **N/A (server-only):** ~8 items (ViewState, Theming, DataSourceID, Focus, etc.)

The Chart component is a ground-up reimplementation using Chart.js. Core charting functionality (series, areas, legends, titles, palettes) is present but exposed through a Blazor child-component composition model rather than direct property collections. Many advanced Web Forms Chart properties (annotations, image storage, serializer, map areas) have no equivalent in the canvas-based approach.
