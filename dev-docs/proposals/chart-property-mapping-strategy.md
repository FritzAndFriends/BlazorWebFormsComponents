# Chart Property Mapping Strategy

**Author:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-07-25
**Status:** Proposal — awaiting Jeff's evaluation

---

## Executive Summary

The BWFC Chart component currently wraps Chart.js for client-side rendering, replacing the ASP.NET Web Forms `System.Web.UI.DataVisualization.Charting.Chart` which rendered server-side via GDI+. The reference baseline shows 10 expected properties, 2 events, and reports ~25% coverage. That number is misleading — it compares apples to jet engines. Chart.js has *different* knobs (animation, responsiveness, plugin architecture, interactivity) that have no Web Forms equivalent, while Web Forms has server-side rendering knobs (anti-aliasing, image storage, hatch patterns) that have no canvas equivalent.

This document maps every missing property, explores four implementation strategies, and recommends a practical blend that serves BWFC's mission: **make migrated markup compile and look close enough that developers can ship**.

---

## 1. The Fundamental Architecture Difference

### Web Forms Chart: Server-Side GDI+ Rendering

```
Browser Request → ASP.NET → Chart control → GDI+ → PNG/JPEG → <img src="..."> → Browser
```

The Web Forms `Chart` control is a **server-side image generator**. It uses `System.Drawing` (GDI+) to rasterize charts into PNG or JPEG images on the server, then serves them as `<img>` tags or stores them to disk. Every "missing" property maps to a GDI+ rendering concept:

| Property | GDI+ Concept |
|---|---|
| `AntiAliasing` | `Graphics.SmoothingMode` — controls sub-pixel edge smoothing |
| `TextAntiAliasingQuality` | `Graphics.TextRenderingHint` — ClearType, AntiAlias, etc. |
| `BackGradientStyle` | `LinearGradientBrush` direction (TopBottom, LeftRight, DiagonalLeft, etc.) |
| `BackHatchStyle` | `HatchBrush` pattern (Cross, DottedGrid, Sphere, Wave, etc.) — 56 patterns |
| `BackSecondaryColor` | Second color in gradient/hatch fills |
| `BorderlineDashStyle` | `Pen.DashStyle` — Dash, Dot, DashDot, DashDotDot |
| `ImageLocation` | File path where generated image is saved on server |
| `ImageStorageMode` | Memory vs. file system for generated image |

### BWFC Chart: Client-Side Canvas Rendering

```
Blazor → Parameters → C# Config Builder → JSON → JS Interop → Chart.js → <canvas> → Browser
```

Chart.js renders directly to an HTML `<canvas>` element using the Canvas 2D API. There is no image generation step. The chart is interactive, responsive, and animated by default. The rendering pipeline is:

1. `Chart.razor.cs` collects parameters and child component registrations
2. `ChartConfigBuilder.cs` builds a configuration dictionary
3. `ChartJsInterop.cs` sends the config to JavaScript via `IJSRuntime`
4. `chart-interop.js` creates/updates a `Chart` instance on the canvas

**Why 1:1 mapping is impossible:**

- **Anti-aliasing**: Canvas 2D always anti-aliases. There is no `SmoothingMode` toggle. The browser controls sub-pixel rendering.
- **Hatch patterns**: GDI+ has 56 built-in `HatchStyle` values. Canvas has zero. You'd need custom pattern rendering.
- **Image storage**: There is no server-side image. The chart lives in the DOM as a `<canvas>`. The concept of `ImageLocation` and `ImageStorageMode` is architecturally meaningless.
- **Gradients**: GDI+ gradients are applied to the chart background via `LinearGradientBrush`. Chart.js has no built-in background gradient — you need a plugin or CSS.

**This is not a gap in coverage. It's a gap in paradigm.** Developers migrating from Web Forms need to understand that their chart was a static image; their new chart is a live, interactive widget. Many of these properties controlled *how the image was generated*, which is no longer relevant.

---

## 2. Property Mapping Options

### Property-by-Property Analysis

#### 2.1 `AntiAliasing`

**Web Forms:** `AntiAliasing` enum (`None`, `Graphics`, `Text`, `All`). Controls whether GDI+ applies edge smoothing to graphics elements and/or text.

| Strategy | Approach | Effort | Fidelity |
|---|---|---|---|
| **A: Stub** | Accept `[Parameter] AntiAliasing`, always ignore. Canvas always anti-aliases. | Trivial | N/A — behavior is always "on" |
| **B: Adapter** | `ChartPropertyAdapter` maps `AntiAliasing.None` to Chart.js config with `devicePixelRatio: 1` (reduces quality, approximates no AA). | Small | Low |
| **C: CSS** | N/A — CSS cannot control canvas anti-aliasing. | — | — |
| **D: Hybrid** | Accept the parameter, document that canvas always anti-aliases, mark `[Obsolete]`. | Trivial | Honest |

**Recommendation:** **Option D (Hybrid/Honest Stub)**. Accept the parameter for markup compatibility. Log a one-time console message: "AntiAliasing is always enabled with Canvas rendering." Mark `[Obsolete("Canvas rendering always applies anti-aliasing. This property is accepted for migration compatibility.")]`.

---

#### 2.2 `BackGradientStyle`

**Web Forms:** `GradientStyle` enum (`None`, `TopBottom`, `BottomTop`, `LeftRight`, `RightLeft`, `Center`, `DiagonalLeft`, `DiagonalRight`, `HorizontalCenter`, `VerticalCenter`). Controls the direction of the background gradient fill.

| Strategy | Approach | Effort | Fidelity |
|---|---|---|---|
| **A: Stub** | Accept parameter, ignore. | Trivial | None |
| **B: Adapter** | Map to Chart.js `backgroundColor` gradient via canvas gradient API in JS interop. `TopBottom` → `createLinearGradient(0, 0, 0, height)`. Requires `BackColor` + `BackSecondaryColor`. | Medium | High |
| **C: CSS** | Apply `background: linear-gradient(...)` to the chart container `<div>`. Canvas is transparent by default, so the CSS gradient shows through. | Small | Medium |
| **D: Hybrid** | CSS gradient by default (easy), with opt-in Canvas gradient via a `UseCanvasGradient` flag. | Small-Medium | Medium-High |

**Recommendation:** **Option C (CSS Gradient)** for Phase 1, with a path to Option B for Phase 2. CSS gradients map naturally to the Web Forms gradient directions:

```csharp
private string GetGradientCss() => BackGradientStyle switch
{
    GradientStyle.TopBottom => $"linear-gradient(to bottom, {BackColor}, {BackSecondaryColor})",
    GradientStyle.LeftRight => $"linear-gradient(to right, {BackColor}, {BackSecondaryColor})",
    GradientStyle.DiagonalLeft => $"linear-gradient(to bottom right, {BackColor}, {BackSecondaryColor})",
    GradientStyle.Center => $"radial-gradient(circle, {BackColor}, {BackSecondaryColor})",
    // ... etc.
    _ => null
};
```

The chart container `<div>` already supports inline styles. The canvas is layered on top with a transparent background. This requires zero Chart.js changes.

---

#### 2.3 `BackHatchStyle`

**Web Forms:** `ChartHatchStyle` enum — 56 values including `Cross`, `DottedGrid`, `Sphere`, `Wave`, `Plaid`, `Weave`, etc. Creates pattern fills for the chart background.

| Strategy | Approach | Effort | Fidelity |
|---|---|---|---|
| **A: Stub** | Accept parameter, ignore. | Trivial | None |
| **B: Adapter** | Generate SVG patterns and use as `backgroundImage` on the container. Or use a Chart.js pattern plugin (e.g., `patternomaly`). | Large | Medium-High |
| **C: CSS** | CSS repeating patterns for the ~10 most common hatch styles (Cross, Horizontal, Vertical, Diagonal, DottedGrid). Use `background-image` with `repeating-linear-gradient` or inline SVG data URIs. | Medium | Medium for common patterns |
| **D: Hybrid** | Accept the enum, implement ~10 common patterns via CSS, log warnings for unsupported patterns. | Medium | Medium |

**Recommendation:** **Option A (Stub) for Phase 1**. Hatch patterns are overwhelmingly a print/report aesthetic from the early 2000s. In practice, very few production Web Forms apps use `BackHatchStyle`. Accept the parameter with `[Obsolete]` and document the migration path: "Use CSS `background-image` patterns on the chart container for hatch effects." If customer demand materializes, escalate to Option C with the 10 most common patterns.

---

#### 2.4 `BackSecondaryColor`

**Web Forms:** The second color in gradient and hatch fills. Only meaningful when `BackGradientStyle` or `BackHatchStyle` is set.

| Strategy | Approach | Effort | Fidelity |
|---|---|---|---|
| **A: Stub** | Accept parameter, hold value. | Trivial | N/A alone |
| **B: Adapter** | Pass to the gradient/hatch CSS generation in conjunction with `BackGradientStyle`. | Bundled | Bundled |
| **C: CSS** | Used as the second stop in `linear-gradient()` or `radial-gradient()`. | Bundled | Bundled |
| **D: Hybrid** | Same — this is a dependency of other properties. | Bundled | Bundled |

**Recommendation:** **Accept as `[Parameter]`**, store the value, and consume it in the `BackGradientStyle` CSS implementation. This property has no meaning in isolation — it's a data dependency, not a behavior. No `[Obsolete]` needed because it *will* be used when gradients are implemented.

---

#### 2.5 `BorderlineDashStyle`

**Web Forms:** `ChartDashStyle` enum (`NotSet`, `Dash`, `DashDot`, `DashDotDot`, `Dot`, `Solid`). Controls the chart's outer border line pattern.

| Strategy | Approach | Effort | Fidelity |
|---|---|---|---|
| **A: Stub** | Accept parameter, apply as CSS `border-style`. | Small | High |
| **B: Adapter** | Map to Chart.js `borderDash` array on canvas (for axis lines, not chart border). | Small | Medium (wrong target) |
| **C: CSS** | Map to CSS `border-style` on the container `<div>`: `Dash` → `dashed`, `Dot` → `dotted`, `DashDot`/`DashDotDot` → `dashed` (CSS can't distinguish). | Small | High for most values |
| **D: Hybrid** | CSS border on container + document that `DashDot`/`DashDotDot` collapse to `dashed`. | Small | Good |

**Recommendation:** **Option C (CSS Border)** — this maps almost perfectly. The existing `ChartDashStyle` enum is already defined in the codebase (`Enums/ChartDashStyle.cs`). Implementation:

```csharp
private string GetBorderStyle() => BorderlineDashStyle switch
{
    ChartDashStyle.Solid => "solid",
    ChartDashStyle.Dash => "dashed",
    ChartDashStyle.Dot => "dotted",
    ChartDashStyle.DashDot => "dashed",    // CSS limitation
    ChartDashStyle.DashDotDot => "dashed", // CSS limitation
    _ => "none"
};
```

We already emit the container `<div>` with inline styles. Adding `border-style` is one line of code.

> **Note:** We already have `ChartDashStyle.cs` in the `Enums/` directory. No new enum needed.

---

#### 2.6 `ImageLocation`

**Web Forms:** The URL or file path where the chart image is saved on the server. Used with `ImageStorageMode.UseHttpHandler` to serve chart images via a handler, or `UseImageLocation` to save to a specific path.

| Strategy | Approach | Effort | Fidelity |
|---|---|---|---|
| **A: Stub** | Accept parameter, ignore completely. | Trivial | None — concept doesn't apply |
| **B: Adapter** | Convert canvas to PNG via `canvas.toDataURL()` and expose via a Blazor method. | Medium | Different paradigm |
| **C: CSS** | N/A | — | — |
| **D: Hybrid** | Accept parameter with `[Obsolete]`. Offer `ExportToPngAsync()` method as the Blazor equivalent. | Medium | Good migration story |

**Recommendation:** **Option A (Stub with `[Obsolete]`)** for now. The concept of server-side image storage doesn't exist in client-rendered charts. The `[Obsolete]` message should say: "Client-side Chart.js rendering does not generate server images. Use canvas.toDataURL() in JavaScript interop to export chart images." A future `ExportToPngAsync()` method would be a nice addition but is out of scope for property parity.

---

#### 2.7 `ImageStorageMode`

**Web Forms:** `ImageStorageMode` enum (`UseHttpHandler`, `UseImageLocation`). Controls whether the generated image is served via an HTTP handler or saved to a file.

| Strategy | Approach | Effort | Fidelity |
|---|---|---|---|
| **A: Stub** | Accept parameter, ignore completely. | Trivial | None — concept doesn't apply |
| **B-D** | Same analysis as `ImageLocation` — server image storage is not a thing. | — | — |

**Recommendation:** **Option A (Stub with `[Obsolete]`)**. Same reasoning as `ImageLocation`. These two properties are architecturally extinct in the Canvas world. Mark `[Obsolete("Chart.js renders directly to canvas. Server-side image storage is not applicable.")]`.

---

#### 2.8 `TextAntiAliasingQuality`

**Web Forms:** `TextAntiAliasingQuality` enum (`Normal`, `High`, `SystemDefault`). Fine-tunes the GDI+ text rendering hint for chart labels and titles.

| Strategy | Approach | Effort | Fidelity |
|---|---|---|---|
| **A: Stub** | Accept parameter, ignore. Canvas text rendering is controlled by the browser/OS. | Trivial | N/A |
| **B: Adapter** | Map to Chart.js font configuration — `font.weight`, `font.size` adjustments. | Small | Low correlation |
| **C: CSS** | `text-rendering: optimizeLegibility` or `geometricPrecision` on the container. Only affects HTML text, not canvas text. | Trivial | None for canvas |
| **D: Hybrid** | Accept with `[Obsolete]`, document that browser text rendering is always high-quality. | Trivial | Honest |

**Recommendation:** **Option D (Honest Stub)**. Same pattern as `AntiAliasing`. Modern browsers render canvas text with sub-pixel anti-aliasing by default. The Web Forms distinction between "Normal" and "High" quality text rendering is a GDI+ artifact. Mark `[Obsolete("Browser canvas text rendering is always high-quality. This property is accepted for migration compatibility.")]`.

---

#### 2.9 Missing Events: `CustomizeLegend` and `CustomizeMapAreas`

**Web Forms:**
- `CustomizeLegend`: Fires before the legend is drawn, allowing code to modify legend items programmatically.
- `CustomizeMapAreas`: Fires to allow adding/modifying `<map>` areas for image-map clickable regions on the chart image.

| Strategy | Approach | Effort | Fidelity |
|---|---|---|---|
| **A: Stub** | Accept as `EventCallback`, never fire. | Small | Compiles but broken |
| **B: Adapter** | `CustomizeLegend` → Chart.js `legend.labels.generateLabels` callback via JS interop. `CustomizeMapAreas` → N/A (canvas doesn't use image maps; Chart.js has built-in click handlers). | Large | Medium |
| **C: CSS** | N/A for events. | — | — |
| **D: Hybrid** | Accept `CustomizeLegend` as `EventCallback`, fire it with a `LegendCustomizeEventArgs` before config build. Skip `CustomizeMapAreas` entirely (image maps don't exist in canvas). | Medium | Good for legend, honest for map areas |

**Recommendation:** **Option D (Hybrid)** — `CustomizeLegend` is implementable as a Blazor event that fires during `ChartConfigBuilder.BuildConfig()`, passing the legend config for mutation. `CustomizeMapAreas` should be accepted as a no-op `[Obsolete]` parameter because Chart.js uses click event handlers instead of HTML image maps.

---

## 3. Chart.js Plugin Opportunities

Chart.js has a rich plugin ecosystem. Several plugins address the gap between Web Forms rendering capabilities and Chart.js defaults:

### 3.1 Gradient Backgrounds — `chartjs-plugin-gradient`

- **What it does:** Allows datasets to use gradient fills (linear or radial) as `backgroundColor` and `borderColor`.
- **Gap it fills:** `BackGradientStyle` + `BackSecondaryColor` for data series and chart areas.
- **Status:** Active, compatible with Chart.js 4.x.
- **Assessment:** Useful for *dataset* gradients, but chart *background* gradients are better handled by CSS on the container. Could complement the CSS approach if per-dataset gradients are needed.

### 3.2 Pattern Fills — `patternomaly`

- **What it does:** Generates canvas pattern images (dots, lines, crosses, zigzags, etc.) that can be used as `backgroundColor` for datasets.
- **Gap it fills:** `BackHatchStyle` — not a 1:1 match (patternomaly has ~20 patterns vs. GDI+'s 56), but covers the common ones (dot, cross, line, dash, zigzag).
- **Status:** Maintained, widely used.
- **Assessment:** Best option if hatch patterns are ever needed. Bundle size concern: adds ~5KB. Only worth including if customer demand exists.

### 3.3 Canvas Background Color — `chartjs-plugin-canvas-background-color`

- **What it does:** Simple plugin to set a solid or gradient background color on the chart canvas.
- **Gap it fills:** `BackColor` + `BackGradientStyle` directly on the canvas (not CSS).
- **Status:** Minimal plugin, easy to inline.
- **Assessment:** Simpler than CSS for solid backgrounds. Could be inlined directly in `chart-interop.js` (< 20 lines of code).

### 3.4 Anti-aliasing Control

- **There is no Chart.js plugin for this.** Canvas 2D rendering always anti-aliases. The only way to disable AA is `imageSmoothingEnabled = false` on the context, which only affects `drawImage()` calls, not path rendering. This confirms that `AntiAliasing` and `TextAntiAliasingQuality` must be stubs.

### Plugin Recommendation

**Don't add plugins for Phase 1.** The CSS approach for gradients and the stub approach for hatch/anti-aliasing avoid new JavaScript dependencies. If demand warrants it later:

1. Inline a minimal canvas background plugin in `chart-interop.js` (~20 lines)
2. Consider `patternomaly` for hatch patterns only if real customers report it as a migration blocker

---

## 4. Migration-First vs Fidelity-First

### Migration-First Approach

> "Accept all old property names, even as no-ops, so migrated markup compiles without changes."

**Pros:**
- Zero markup changes required for Chart properties during migration
- `bwfc-migrate.ps1` doesn't need special handling for Chart properties
- Developers can migrate now, refine later

**Cons:**
- Silent no-ops mask real differences — developers may not realize their chart looks different
- `[Obsolete]` warnings create noise if developers aren't ready to address them
- Properties like `ImageLocation` are genuinely meaningless and accepting them implies they do something

### Fidelity-First Approach

> "Only implement properties where we can match the visual output."

**Pros:**
- Every implemented property actually does something
- No false promises
- Smaller API surface to maintain

**Cons:**
- Migrated markup may not compile if it uses `ImageLocation`, `BackHatchStyle`, etc.
- Developers must delete properties during migration — extra work
- Breaks the BWFC core promise: "remove `asp:` prefix and it works"

### Recommended Blend: Migration-First with Honest Diagnostics

This is the **Pattern B+ approach** we established for other controls — accept everything, but be transparent:

| Category | Properties | Strategy |
|---|---|---|
| **Real Mapping** | `BackGradientStyle`, `BackSecondaryColor`, `BorderlineDashStyle` | Implement with CSS — the visual output is close enough |
| **Honest Stub** | `AntiAliasing`, `TextAntiAliasingQuality` | Accept, `[Obsolete]` with clear message, behavior is always "on" |
| **Architecturally Extinct** | `ImageLocation`, `ImageStorageMode` | Accept, `[Obsolete]`, no possible implementation |
| **Deferred** | `BackHatchStyle` | Accept, `[Obsolete]`, implement if demand exists |
| **Event: Implementable** | `CustomizeLegend` | Accept as `EventCallback`, implement in Phase 2 |
| **Event: Extinct** | `CustomizeMapAreas` | Accept as `EventCallback`, `[Obsolete]`, never fires |

**Why this serves BWFC's mission:**

1. **Migrated markup compiles** — no properties need to be removed during migration
2. **Visual output is reasonable** — gradients and borders work via CSS
3. **Developers know the truth** — `[Obsolete]` messages explain what's different and why
4. **No false promises** — we don't pretend `ImageLocation` does something
5. **No unnecessary dependencies** — no Chart.js plugins in Phase 1

---

## 5. Recommended Plan

### Property Mapping Table

| Web Forms Property | Chart.js / CSS Equivalent | Strategy | Effort | Priority |
|---|---|---|---|---|
| `AntiAliasing` | Always on (Canvas 2D) | Honest stub + `[Obsolete]` | Trivial | P2 |
| `BackGradientStyle` | CSS `linear-gradient`/`radial-gradient` on container `<div>` | Real mapping via CSS | Small | P1 |
| `BackHatchStyle` | None (future: `patternomaly` plugin or CSS patterns) | Stub + `[Obsolete]` | Trivial | P3 |
| `BackSecondaryColor` | CSS gradient second color stop | Data parameter (consumed by gradient) | Trivial | P1 |
| `BorderlineDashStyle` | CSS `border-style` on container `<div>` | Real mapping via CSS | Small | P1 |
| `ImageLocation` | N/A (no server image) | Stub + `[Obsolete]` | Trivial | P3 |
| `ImageStorageMode` | N/A (no server image) | Stub + `[Obsolete]` | Trivial | P3 |
| `TextAntiAliasingQuality` | Always high (browser rendering) | Honest stub + `[Obsolete]` | Trivial | P2 |
| `CustomizeLegend` | Chart.js `legend.labels.generateLabels` | `EventCallback` — Phase 2 | Medium | P2 |
| `CustomizeMapAreas` | N/A (canvas, not image map) | Stub + `[Obsolete]` | Trivial | P3 |

### Implementation Plan

#### Phase 1: Migration Compatibility (Effort: ~1 day)

1. **Add 8 `[Parameter]` properties** to `Chart.razor.cs`:
   - `AntiAliasing` (enum, `[Obsolete]`)
   - `BackGradientStyle` (new `GradientStyle` enum)
   - `BackHatchStyle` (new `ChartHatchStyle` enum, `[Obsolete]`)
   - `BackSecondaryColor` (`WebColor`)
   - `BorderlineDashStyle` (`ChartDashStyle` — enum already exists)
   - `ImageLocation` (`string`, `[Obsolete]`)
   - `ImageStorageMode` (new `ImageStorageMode` enum, `[Obsolete]`)
   - `TextAntiAliasingQuality` (new `TextAntiAliasingQuality` enum, `[Obsolete]`)

2. **Create 4 new enums** in `Enums/`:
   - `GradientStyle.cs` (None, TopBottom, BottomTop, LeftRight, RightLeft, Center, DiagonalLeft, DiagonalRight, HorizontalCenter, VerticalCenter)
   - `ChartHatchStyle.cs` (None, BackwardDiagonal, Cross, DarkDownwardDiagonal, ... — 56 values from GDI+)
   - `ImageStorageMode.cs` (UseHttpHandler, UseImageLocation)
   - `TextAntiAliasingQuality.cs` (Normal, High, SystemDefault)
   - `AntiAliasingStyles.cs` (None, Graphics, Text, All — flags enum)

3. **Implement CSS rendering** in `Chart.razor`:
   - Modify the container `<div>` style to include gradient background and border style
   - `BackGradientStyle` + `BackColor` + `BackSecondaryColor` → CSS `linear-gradient` / `radial-gradient`
   - `BorderlineDashStyle` + `BorderColor` + `BorderWidth` → CSS `border`

4. **Add 2 `EventCallback` parameters**:
   - `CustomizeLegend` (`EventCallback`, no-op initially)
   - `CustomizeMapAreas` (`EventCallback`, `[Obsolete]`, permanent no-op)

5. **Update `ChartConfigBuilder`** — no changes needed (CSS is handled in the Razor template, not the config).

6. **Write tests** — verify parameters are accepted, CSS is emitted correctly, `[Obsolete]` warnings compile.

#### Phase 2: Enhanced Fidelity (Effort: ~2 days, future)

1. Implement `CustomizeLegend` event — fire before config build, pass mutable legend items
2. Inline a minimal canvas background plugin in `chart-interop.js` for canvas-level gradient rendering
3. Evaluate `patternomaly` for hatch patterns if customer demand exists

### Effort Summary

| Phase | Scope | Effort |
|---|---|---|
| Phase 1 | 8 parameters + 5 enums + CSS rendering + 2 events | ~1 day |
| Phase 2 | CustomizeLegend event + canvas gradient plugin | ~2 days |
| Phase 3 (if needed) | Hatch patterns via plugin | ~1 day |

### Post-Implementation Coverage

After Phase 1:

- **Properties:** 10/10 accepted as parameters (100% markup compatibility)
- **Real visual mappings:** 3/10 (`BackGradientStyle`, `BackSecondaryColor`, `BorderlineDashStyle`)
- **Honest stubs:** 5/10 (`AntiAliasing`, `BackHatchStyle`, `ImageLocation`, `ImageStorageMode`, `TextAntiAliasingQuality`)
- **Already implemented:** 2/10 (`ImageType`, `Palette`)
- **Events:** 2/2 accepted (0/2 functional in Phase 1, 1/2 in Phase 2)

The baseline coverage metric jumps from 25% to **100% markup acceptance** with **50% real visual fidelity** — an honest and defensible position.

---

## 6. Example Migration

### Before: ASP.NET Web Forms Chart

```aspx
<asp:Chart ID="SalesChart" runat="server"
    Width="600" Height="400"
    BackColor="#F0F0F0"
    BackGradientStyle="TopBottom"
    BackSecondaryColor="#FFFFFF"
    BorderlineDashStyle="Solid"
    BorderlineColor="Gray"
    BorderlineWidth="1"
    AntiAliasing="All"
    ImageLocation="~/TempImages/ChartPic_#SEQ(300,3)"
    ImageStorageMode="UseImageLocation"
    Palette="BrightPastel">

    <Titles>
        <asp:Title Text="Monthly Sales" Docking="Top" />
    </Titles>

    <Legends>
        <asp:Legend Name="Default" Docking="Bottom" />
    </Legends>

    <ChartAreas>
        <asp:ChartArea Name="MainArea">
            <AxisX Title="Month" />
            <AxisY Title="Revenue ($)" />
        </asp:ChartArea>
    </ChartAreas>

    <Series>
        <asp:Series Name="Revenue"
            ChartType="Column"
            ChartArea="MainArea"
            XValueMember="Month"
            YValueMembers="Revenue"
            Color="SteelBlue" />
    </Series>
</asp:Chart>
```

### After: BWFC Blazor Chart (Post Phase 1)

```razor
@* Remove asp: prefix, remove runat="server" *@
@* ImageLocation and ImageStorageMode accepted but obsolete — no server images in Blazor *@
@* AntiAliasing accepted but obsolete — canvas always anti-aliases *@
@* BackGradientStyle and BackSecondaryColor → CSS gradient on container *@
@* BorderlineDashStyle → CSS border-style on container *@

<Chart ID="SalesChart"
    ChartWidth="600px" ChartHeight="400px"
    BackColor="#F0F0F0"
    BackGradientStyle="TopBottom"
    BackSecondaryColor="#FFFFFF"
    BorderlineDashStyle="Solid"
    BorderColor="Gray"
    BorderWidth="1"
    AntiAliasing="All"
    ImageLocation="~/TempImages/ChartPic_#SEQ(300,3)"
    ImageStorageMode="UseImageLocation"
    Palette="BrightPastel">

    <ChartTitle Text="Monthly Sales" TitleDocking="Docking.Top" />

    <ChartLegend Name="Default" LegendDocking="Docking.Bottom" />

    <ChartArea Name="MainArea"
        AxisX="@(new Axis { Title = "Month" })"
        AxisY="@(new Axis { Title = "Revenue ($)" })" />

    <ChartSeries Name="Revenue"
        ChartType="SeriesChartType.Column"
        ChartArea="MainArea"
        XValueMember="Month"
        YValueMembers="Revenue"
        Color="SteelBlue"
        Items="@salesData" />
</Chart>
```

### What the developer sees at build time

```
warning CS0618: 'Chart.AntiAliasing' is obsolete:
  'Canvas rendering always applies anti-aliasing. This property is accepted for migration compatibility.'

warning CS0618: 'Chart.ImageLocation' is obsolete:
  'Chart.js renders directly to canvas. Server-side image storage is not applicable. Remove this property.'

warning CS0618: 'Chart.ImageStorageMode' is obsolete:
  'Chart.js renders directly to canvas. Server-side image storage is not applicable. Remove this property.'
```

### What the rendered HTML looks like

```html
<!-- Web Forms would have emitted: <img src="/TempImages/ChartPic_001.png" /> -->
<!-- BWFC emits a live, interactive canvas chart: -->
<div id="SalesChart" style="width:600px;height:400px;
    background:linear-gradient(to bottom, #F0F0F0, #FFFFFF);
    border:1px solid Gray;border-style:solid;">
  <canvas id="SalesChart_canvas_a1b2c3d4"></canvas>
</div>
```

The chart looks visually similar — same palette, same gradient direction, same border — but it's interactive, responsive, and animated. The `ImageLocation` property is silently ignored. The developer gets compile-time guidance about what changed and why.

---

## Appendix A: Web Forms Chart Property Coverage (Complete)

| # | Property | Status | Implementation |
|---|---|---|---|
| 1 | `AntiAliasing` | 🟡 Stub | `[Obsolete]`, always on |
| 2 | `BackGradientStyle` | 🟢 Real | CSS `linear-gradient` / `radial-gradient` |
| 3 | `BackHatchStyle` | 🟡 Stub | `[Obsolete]`, deferred |
| 4 | `BackSecondaryColor` | 🟢 Real | CSS gradient second color |
| 5 | `BorderlineDashStyle` | 🟢 Real | CSS `border-style` |
| 6 | `ImageLocation` | 🔴 Extinct | `[Obsolete]`, no implementation possible |
| 7 | `ImageStorageMode` | 🔴 Extinct | `[Obsolete]`, no implementation possible |
| 8 | `ImageType` | ✅ Done | Existing parameter (API compat) |
| 9 | `Palette` | ✅ Done | Existing — `ChartPalette` enum with 11 palettes |
| 10 | `TextAntiAliasingQuality` | 🟡 Stub | `[Obsolete]`, always high |

## Appendix B: Decision Log

- **2026-07-25:** Forge proposed Migration-First with Honest Diagnostics (Pattern B+) for Chart properties.
- Pending Jeff's review and approval before implementation.
