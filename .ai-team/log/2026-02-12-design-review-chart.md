# Ceremony: Design Review — Chart Component Architecture

**Date:** 2026-02-12
**Facilitator:** Forge (Lead / Web Forms Reviewer)
**Participants:** Cyclops (Component Dev), Rogue (QA), Jubilee (Samples)
**Context:** Milestone 4 — First JS interop component in the project

---

## 1. Key Decisions

### Decision 1: Base Class — New `DataBoundStyledComponent<T>`

**Problem:** Web Forms `Chart` inherits `DataBoundControl` → `WebControl`. It needs BOTH data binding (Series data, DataSource) AND style properties (Width, Height, BackColor, CssClass, Font). In our codebase:
- `DataBoundComponent<T>` inherits `BaseDataBoundComponent` → `BaseWebFormsComponent` — **no style properties**
- `BaseStyledComponent` inherits `BaseWebFormsComponent` — **no data binding**
- GridView works around this by re-declaring `CssClass` as a standalone `[Parameter]` — a pattern smell

**Decision:** Create a new intermediate base class `DataBoundStyledComponent<T>` that combines both:

```
BaseWebFormsComponent
├── BaseStyledComponent (adds IStyle: BackColor, ForeColor, CssClass, Width, Height, Font, Border*)
│   └── [all styled controls]
├── BaseDataBoundComponent (adds DataSource, OnDataBound)
│   └── DataBoundComponent<T> (adds Items, DataMember, SelectMethod)
│       └── GridView, Repeater, ListView, etc.
└── NEW: DataBoundStyledComponent<T>
        inherits DataBoundComponent<T>, implements IStyle
        └── Chart<T>
```

`DataBoundStyledComponent<T>` inherits `DataBoundComponent<T>` and implements `IStyle` by duplicating the style property declarations from `BaseStyledComponent`. This is necessary because C# doesn't support multiple inheritance. The `IStyle` interface already exists and defines the contract.

**Rationale:** Chart Width/Height are not just CSS — in Web Forms they control the pixel dimensions of the rendered chart image. They map to `<canvas width="..." height="...">` attributes AND to the Chart.js config. `CssClass` maps to the wrapping `<div>` element's class. The chart MUST have proper data binding for Series data. Neither existing base class alone is sufficient.

**Note for Cyclops:** Place in `DataBinding/DataBoundStyledComponent.cs`. Copy the `IStyle` implementation from `BaseStyledComponent.cs` — same properties, same `Style` helper. Consider whether GridView should be refactored to inherit this new class in a follow-up (not in Milestone 4 scope).

---

### Decision 2: Component Hierarchy & Child Registration Pattern

**Component tree mirrors Web Forms:**
```
<Chart>                          → Chart.razor / Chart.razor.cs
    <ChartArea>                  → ChartArea.razor.cs (cascading param)
    <Series>                     → ChartSeries.razor.cs (cascading param)
        <DataPoint>              → optional child of Series
    <ChartTitle>                 → ChartTitle.razor.cs (cascading param)
    <Legend>                     → ChartLegend.razor.cs (cascading param)
</Chart>
```

**Registration pattern:** Follow MultiView/View (explicit registration on init), not Menu/MenuItem.

```csharp
// Chart.razor
<CascadingValue Name="ParentChart" Value="this">
    @ChildContent
</CascadingValue>
<canvas @ref="_canvasRef" id="@ClientID" width="@WidthPixels" height="@HeightPixels" />

// ChartSeries.razor.cs
[CascadingParameter(Name = "ParentChart")]
public Chart<TItemType> ParentChart { get; set; }

protected override async Task OnInitializedAsync()
{
    await base.OnInitializedAsync();
    ParentChart?.RegisterSeries(this);
}
```

**Chart maintains internal collections:**
```csharp
public List<ChartSeries> SeriesList { get; } = new();
public List<ChartArea> ChartAreaList { get; } = new();
public List<ChartTitle> TitleList { get; } = new();
public List<ChartLegend> LegendList { get; } = new();

internal void RegisterSeries(ChartSeries series) => SeriesList.Add(series);
internal void RegisterChartArea(ChartArea area) => ChartAreaList.Add(area);
internal void RegisterTitle(ChartTitle title) => TitleList.Add(title);
internal void RegisterLegend(ChartLegend legend) => LegendList.Add(legend);
```

Child content renders first (children register), then `OnAfterRenderAsync` builds the JSON config and calls JS interop.

---

### Decision 3: JS Interop Contract

**Module:** `wwwroot/js/chart-interop.js` (ES module, follows Basepage.module.js pattern)

**Three exported functions:**

```javascript
// chart-interop.js

/**
 * Creates or replaces a Chart.js instance on the given canvas.
 * @param {string} canvasId - The DOM ID of the <canvas> element
 * @param {object} config - Full Chart.js configuration object
 * @returns {void}
 */
export function createChart(canvasId, config) { ... }

/**
 * Updates an existing Chart.js instance with new data/options.
 * @param {string} canvasId - The DOM ID
 * @param {object} config - Updated Chart.js configuration
 * @returns {void}
 */
export function updateChart(canvasId, config) { ... }

/**
 * Destroys a Chart.js instance and releases resources.
 * @param {string} canvasId - The DOM ID
 * @returns {void}
 */
export function destroyChart(canvasId) { ... }
```

**JSON config shape** passed from C# to JS — a direct Chart.js configuration object:

```json
{
  "type": "bar",
  "data": {
    "labels": ["Jan", "Feb", "Mar"],
    "datasets": [
      {
        "label": "Sales",
        "data": [10, 20, 30],
        "backgroundColor": ["#FF6384", "#36A2EB", "#FFCE56"],
        "borderColor": ["#FF6384", "#36A2EB", "#FFCE56"],
        "borderWidth": 1,
        "borderDash": []
      }
    ]
  },
  "options": {
    "responsive": false,
    "plugins": {
      "title": { "display": true, "text": "Monthly Sales", "position": "top" },
      "legend": { "display": true, "position": "top" }
    },
    "scales": {
      "x": { "stacked": false },
      "y": { "stacked": false, "beginAtZero": true }
    }
  }
}
```

**C# interop service pattern:**

```csharp
// ChartJsInterop.cs — thin wrapper, follows BlazorWebFormsJsInterop pattern
public class ChartJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public ChartJsInterop(IJSRuntime jsRuntime)
    {
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/Fritz.BlazorWebFormsComponents/js/chart-interop.js"
        ).AsTask());
    }

    public async ValueTask CreateChartAsync(string canvasId, object config)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("createChart", canvasId, config);
    }

    public async ValueTask UpdateChartAsync(string canvasId, object config)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("updateChart", canvasId, config);
    }

    public async ValueTask DestroyChartAsync(string canvasId)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("destroyChart", canvasId);
    }

    public async ValueTask DisposeAsync() { ... }
}
```

**Canvas reference:** Use `ElementReference` in the `.razor` file (`@ref="_canvasRef"`) but pass the canvas `id` (from `ClientID`) to JS interop. Chart.js needs `document.getElementById()` anyway. The `ElementReference` is kept as a fallback if needed.

**Lifecycle:**
1. `OnInitializedAsync`: Children register via cascading params
2. `OnAfterRenderAsync(firstRender: true)`: Build config from registered children → call `createChart`
3. `OnParametersSetAsync` (subsequent): If data changed → call `updateChart`
4. `DisposeAsync`: Call `destroyChart` to prevent memory leaks

---

### Decision 4: SeriesChartType Enum Mapping

Web Forms `SeriesChartType` has 35 values. Phase 1 supports 8, mapping to Chart.js types:

| Web Forms SeriesChartType | Chart.js `type` | Notes |
|---------------------------|-----------------|-------|
| `Column`                  | `bar`           | Chart.js `bar` is vertical by default |
| `Bar`                     | `bar`           | With `indexAxis: 'y'` |
| `Line`                    | `line`          | Direct mapping |
| `Pie`                     | `pie`           | Direct mapping |
| `Area`                    | `line`          | With `fill: true` |
| `Doughnut`                | `doughnut`      | Direct mapping |
| `Scatter` (Point)         | `scatter`       | Direct mapping |
| `StackedColumn`           | `bar`           | With `stacked: true` on both axes |

**The full `SeriesChartType` enum MUST be created with ALL 35 Web Forms values** (for API fidelity), but unsupported types throw `NotSupportedException` at render time in Phase 1.

---

### Decision 5: Phase 1 Enums

Create in `src/BlazorWebFormsComponents/Enums/`:

| Enum | Values (Phase 1 subset) |
|------|------------------------|
| `SeriesChartType` | All 35 Web Forms values (Column, Bar, Line, Pie, Area, Doughnut, Point, StackedColumn, StackedBar, Spline, StepLine, Radar, Polar, Bubble, Funnel, Pyramid, etc.) |
| `ChartPalette` | None, Bright, Grayscale, Excel, Light, Pastel, EarthTones, SemiTransparent, Berry, Chocolate, Fire, SeaGreen, BrightPastel |
| `Docking` | Top, Bottom, Left, Right |
| `ChartDashStyle` | NotSet, Dash, DashDot, DashDotDot, Dot, Solid |

---

### Decision 6: Chart.js Version Pin

**Pin to Chart.js v4.4.8** (latest v4 LTS-stable widely used in production).

**Rationale:** v4.5.x is recent (Oct 2025). Pinning to 4.4.8 gives us a well-tested baseline. The bundled `chart.min.js` is a static asset — version updates are a single file replacement. We can bump later.

**Bundle location:** `wwwroot/js/chart.min.js` — imported by `chart-interop.js` via relative path.

**chart-interop.js imports Chart.js:**
```javascript
import { Chart, registerables } from './chart.min.js';
Chart.register(...registerables);
```

---

## 2. Testing Strategy (for Rogue)

**What bUnit CAN test** (canvas content is opaque — bUnit cannot inspect Chart.js rendering):

| Test Category | What to Assert | How |
|---------------|----------------|-----|
| **Markup structure** | `<canvas>` element exists with correct `id`, `width`, `height` attributes | `component.Find("canvas")`, assert attributes |
| **Child registration** | Series/ChartArea/Title/Legend register correctly with parent | Assert `chart.SeriesList.Count`, `chart.TitleList.Count`, etc. |
| **Config generation** | JSON config built from C# parameters matches expected shape | Unit test the config builder method directly (extract as pure function) |
| **Style rendering** | CssClass applied to wrapping div, inline styles present | `component.Find("div").GetAttribute("class")` |
| **Parameter binding** | Width/Height/Palette/AlternateText set correctly | Assert parameter values on component instance |
| **JS interop calls** | `createChart` called with correct args on first render | Mock `IJSRuntime`, verify `InvokeAsync` calls |
| **Dispose** | `destroyChart` called on dispose | Mock verification |
| **Error handling** | Unsupported chart type throws `NotSupportedException` | `Assert.Throws<NotSupportedException>()` |
| **Data binding** | Items/DataSource properly flow to config builder | Unit test config builder with various data shapes |
| **No data** | Empty/null data renders gracefully | Verify no JS errors, canvas still renders |

**What bUnit CANNOT test:**
- Visual chart rendering (colors, axes, labels drawn on canvas)
- Chart.js interactions (hover tooltips, click events)
- Animation behavior

**Recommendation:** Extract `ChartConfigBuilder` as a pure static class that takes the registered children and parameters and returns a `Dictionary<string, object>` config. This is 100% unit-testable without JS interop. The remaining integration testing (does Chart.js actually render?) belongs to Colossus's Playwright tests.

---

## 3. Sample Strategy (for Jubilee)

**Sample page:** `Components/Pages/ControlSamples/Chart/Index.razor`

**Demo charts to build (one per Phase 1 type):**

1. **Column Chart** — Monthly sales data (classic bar chart scenario)
2. **Bar Chart** — Horizontal product comparison
3. **Line Chart** — Temperature over time with multiple series
4. **Pie Chart** — Market share breakdown
5. **Area Chart** — Revenue trend with filled area
6. **Doughnut Chart** — Budget allocation
7. **Scatter Chart** — Height vs weight correlation
8. **Stacked Column** — Quarterly revenue by region

**Sample data pattern:** Use simple inline collections (not a database), similar to GridView samples. Each chart should demonstrate:
- At least 2 series (where applicable)
- Custom palette/colors
- Title and Legend positioning
- Data binding via `Items` parameter

---

## 4. Action Items

| # | Who | What | Priority |
|---|-----|------|----------|
| 1 | **Cyclops** | Create `DataBoundStyledComponent<T>` base class in `DataBinding/` | P0 — blocking |
| 2 | **Cyclops** | Create 4 enum files: `SeriesChartType`, `ChartPalette`, `Docking`, `ChartDashStyle` | P0 — blocking |
| 3 | **Cyclops** | Create `ChartJsInterop.cs` service (thin JS module wrapper) | P0 — blocking |
| 4 | **Cyclops** | Create `chart-interop.js` ES module in `wwwroot/js/` | P0 — blocking |
| 5 | **Cyclops** | Bundle `chart.min.js` v4.4.8 in `wwwroot/js/` | P0 — blocking |
| 6 | **Cyclops** | Create `ChartConfigBuilder` (pure static class, builds Chart.js JSON config) | P0 — blocking |
| 7 | **Cyclops** | Implement `Chart<T>.razor` / `Chart<T>.razor.cs` (parent component) | P1 |
| 8 | **Cyclops** | Implement child components: `ChartSeries`, `ChartArea`, `ChartTitle`, `ChartLegend` | P1 |
| 9 | **Rogue** | Write bUnit tests for markup structure, child registration, config generation, JS interop mock, dispose, error cases | P1 |
| 10 | **Rogue** | Write unit tests for `ChartConfigBuilder` (pure function, full coverage) | P1 |
| 11 | **Jubilee** | Create 8 sample charts in `Components/Pages/ControlSamples/Chart/Index.razor` | P2 |
| 12 | **Beast** | Write Chart component documentation page in `docs/` | P2 |
| 13 | **Colossus** | Write Playwright integration tests for visual rendering verification | P3 |

---

## 5. Risks Identified

| # | Risk | Severity | Mitigation |
|---|------|----------|------------|
| 1 | **First JS interop** — No precedent in project for bundling third-party JS. Module loading, static asset paths, CDN fallbacks are all new territory. | HIGH | Follow existing `BlazorWebFormsJsInterop` lazy-load pattern exactly. Test module loading early. |
| 2 | **SSR / prerendering** — Chart.js requires DOM. `OnAfterRenderAsync` is correct hook, but prerendered HTML will show an empty canvas until hydration. | MEDIUM | Add `@rendermode InteractiveServer` guidance in docs. Consider a placeholder/loading state in the canvas. |
| 3 | **Canvas not bUnit-testable** — We cannot verify visual output in unit tests. | MEDIUM | Mitigated by `ChartConfigBuilder` extraction (pure function tests) + Playwright integration tests. |
| 4 | **Chart.js bundle size (~60KB)** — Adds to download for all users, even those not using Chart. | LOW | Static asset is lazy-loaded via ES module import — only fetched when Chart component is used. Not in the critical path. |
| 5 | **DataBoundStyledComponent<T> is new infrastructure** — Introducing a new base class. Must not break existing components. | LOW | New class, no existing components change. GridView refactoring is future scope only. |
| 6 | **SeriesChartType mismatch** — Web Forms has 35 chart types, Chart.js has ~10. Unsupported types must fail clearly. | LOW | Throw `NotSupportedException` with clear message listing supported types. |
| 7 | **Dispose race condition** — If component disposes before `OnAfterRenderAsync` completes, `destroyChart` may fail. | LOW | Guard with `_chartCreated` flag. Swallow `ObjectDisposedException` in `DisposeAsync`. |

---

## 6. Open Questions (deferred)

- Should `DataBoundStyledComponent<T>` be backported to GridView/DetailsView/ListView? (Answer: not in Milestone 4 — future refactoring)
- Should Chart support `RenderType` property (ImageTag vs BinaryStreaming)? (Answer: No — Blazor always uses canvas. Property can exist for API compat but is ignored.)
- Should we support `DataBindCrossTable` method? (Answer: Phase 2 at earliest — complex pivot logic)

---

**End of ceremony. Design approved by Forge. Implementation may begin.**
