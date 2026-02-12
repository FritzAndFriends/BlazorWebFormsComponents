# Decision: Chart Component Architecture (Design Review)

**By:** Forge
**Date:** 2026-02-12
**Ceremony:** Design Review — Milestone 4

---

### Base class: DataBoundStyledComponent<T>

**What:** Create new `DataBoundStyledComponent<T>` inheriting `DataBoundComponent<T>` and implementing `IStyle`. Chart inherits this new class. Web Forms `Chart` inherits `DataBoundControl` → `WebControl` — it needs both data binding AND style properties. Our `DataBoundComponent<T>` chain skips `BaseStyledComponent`, so styled data-bound controls have no proper base class. GridView worked around this by re-declaring `CssClass` as a standalone `[Parameter]` — a pattern smell.

**Why:** Neither `DataBoundComponent<T>` (no styles) nor `BaseStyledComponent` (no data binding) alone satisfies the Web Forms Chart contract. The new base class fills a structural gap. It does not affect existing components (additive only).

---

### Child registration: CascadingValue + explicit Register on init

**What:** ChartSeries, ChartArea, ChartTitle, ChartLegend register with parent Chart via `[CascadingParameter(Name="ParentChart")]` and call `ParentChart.RegisterXxx(this)` in `OnInitializedAsync`. Chart maintains `SeriesList`, `ChartAreaList`, `TitleList`, `LegendList` collections.

**Why:** Follows the MultiView/View pattern already established in the project. Explicit registration gives Chart deterministic knowledge of its children before `OnAfterRenderAsync` fires the JS interop call.

---

### JS interop contract: Three-function ES module

**What:** `chart-interop.js` exports `createChart(canvasId, config)`, `updateChart(canvasId, config)`, `destroyChart(canvasId)`. Config is a standard Chart.js configuration object (type + data + options). C# wrapper class `ChartJsInterop` uses lazy `IJSObjectReference` import pattern matching `BlazorWebFormsJsInterop`. Canvas referenced by `id` (from `ClientID`), not `ElementReference`.

**Why:** Minimal JS surface area. Passing a standard Chart.js config object means C# owns the config shape and JS is a thin pass-through — no JS-side logic to maintain. Follows existing lazy-module-import pattern.

---

### Chart.js version: Pin to v4.4.8

**What:** Bundle `chart.min.js` v4.4.8 as a static asset in `wwwroot/js/`. Imported by `chart-interop.js` via relative ES module import.

**Why:** v4.4.8 is widely deployed and well-tested. v4.5.x is newer (Oct 2025) with less production mileage. Pinning to a stable version reduces risk for the project's first JS interop component. Upgrading is a single file replacement.

---

### Phase 1 chart types: 8 types mapped to Chart.js

**What:** Column→bar, Bar→bar(indexAxis:'y'), Line→line, Pie→pie, Area→line(fill:true), Doughnut→doughnut, Scatter→scatter, StackedColumn→bar(stacked:true). Full `SeriesChartType` enum (all 35 Web Forms values) created for API fidelity; unsupported types throw `NotSupportedException`.

**Why:** API fidelity requires the full enum. Chart.js maps cleanly to 8 common chart types. Unsupported types fail clearly rather than silently producing wrong output.

---

### Testing strategy: Extract ChartConfigBuilder as pure function

**What:** `ChartConfigBuilder` is a static class that takes registered children/parameters and produces the Chart.js config dictionary. bUnit tests cover: markup structure (canvas attributes), child registration, config generation (via ChartConfigBuilder), JS interop mock verification, dispose cleanup, error handling. Visual rendering verified by Playwright.

**Why:** Canvas content is opaque to bUnit. Extracting the config builder as a pure function maximizes testable surface area without JS interop. This is the same principle as testing a ViewModel separately from a View.

---

### Enums: 4 new enum files

**What:** `SeriesChartType` (35 values), `ChartPalette` (13 values), `Docking` (4 values: Top/Bottom/Left/Right), `ChartDashStyle` (6 values). All placed in `Enums/` directory following project convention.

**Why:** Web Forms Chart uses these enums. Project convention requires every Web Forms enum to have a corresponding C# enum in `Enums/`.
