# Chart Component Implementation Decisions

**By:** Cyclops
**Date:** 2026-02-12
**Scope:** WI-1, WI-2, WI-3 (Chart component, JS interop, chart type mapping)

## Decisions Made

### 1. SeriesChartType.Point maps to Chart.js "scatter"
Web Forms does not have a `Scatter` enum value — `Point = 0` is the equivalent. The design spec listed "Scatter" as a Phase 1 type, but the actual enum uses `Point`. `ChartConfigBuilder` maps `Point` → `"scatter"` in Chart.js.

### 2. ChartWidth/ChartHeight as string parameters (not overriding base Width/Height)
`BaseStyledComponent` already defines `Width` and `Height` as `Unit` type parameters. Rather than hiding these, Chart adds separate `ChartWidth`/`ChartHeight` string parameters (e.g., "400px", "300px") that render as inline CSS on the wrapper `<div>`. The base `Width`/`Height` remain available for CSS style generation via `this.ToStyle()`.

### 3. ChartJsInterop is separate from BlazorWebFormsJsInterop
Chart.js interop uses its own `ChartJsInterop` class, not the shared `BlazorWebFormsJsInterop` service. This keeps chart-specific JS isolated and avoids polluting the page-level interop service.

### 4. Chart.js placeholder file
Since no internet access is available, `wwwroot/js/chart.min.js` is a placeholder stub that exports a `Chart` constructor. It logs a console warning. Must be replaced with real Chart.js v4.4.8 before production use.

### 5. Child component registration via CascadingParameter
All child components (ChartSeries, ChartArea, ChartLegend, ChartTitle) use `[CascadingParameter(Name = "ParentChart")]` and register in `OnInitializedAsync`, following the MultiView/View pattern.

### 6. ChartConfigBuilder uses snapshot classes
Instead of passing the `Chart` component directly to `ChartConfigBuilder.BuildConfig()`, we pass config snapshot classes (`ChartSeriesConfig`, `ChartAreaConfig`, etc.) extracted via `.ToConfig()` methods. This decouples the builder from component lifecycle and enables pure unit testing.

### 7. Docking parameter naming avoids conflicts
`ChartLegend.LegendDocking` and `ChartTitle.TitleDocking` use prefixed names to avoid potential parameter name conflicts with the base class or future properties. They're nullable `Docking?` to distinguish "not set" from a default value.

### 8. Task.Yield() before first chart creation
`OnAfterRenderAsync(firstRender)` calls `Task.Yield()` before creating the chart, giving child components time to register via their own `OnInitializedAsync`. Without this, the chart would render before series/areas/titles/legends are registered.

## Files Created
- `Enums/SeriesChartType.cs`, `Enums/ChartPalette.cs`, `Enums/Docking.cs`, `Enums/ChartDashStyle.cs`
- `Axis.cs`, `DataPoint.cs`
- `wwwroot/js/chart.min.js`, `wwwroot/js/chart-interop.js`
- `ChartJsInterop.cs`, `ChartConfigBuilder.cs`
- `Chart.razor`, `Chart.razor.cs`
- `ChartSeries.razor`, `ChartSeries.razor.cs`
- `ChartArea.razor`, `ChartArea.razor.cs`
- `ChartLegend.razor`, `ChartLegend.razor.cs`
- `ChartTitle.razor`, `ChartTitle.razor.cs`
