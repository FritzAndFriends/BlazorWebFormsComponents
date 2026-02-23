# Decisions

> Shared team decisions. All agents read this. Only Scribe writes here (by merging from inbox).

<!-- Decisions are appended below by the Scribe after merging from .ai-team/decisions/inbox/ -->

### 2026-02-10: Sample pages use Components/Pages path

**By:** Jubilee
**What:** All new sample pages should be created in `Components/Pages/ControlSamples/{ComponentName}/Index.razor`. The older `Pages/ControlSamples/` path should not be used for new components.
**Why:** The sample app has two page directories — the newer .NET 8+ `Components/Pages/` layout is the standard for new work.

### 2026-02-10: PR merge readiness ratings

**By:** Forge
**What:** PR review ratings established: #333 Calendar (Needs Work — SelectionMode enum), #335 FileUpload (Needs Work — broken data flow), #337 ImageMap (Needs Work — wrong base class), #327 PageService (Ready with minor fixes), #328 ASCX CLI (Risky — conflicts, no tests), #309 VS Snippets (Risky — conflicts).
**Why:** Systematic review of all open PRs to establish sprint priorities and identify blockers.

### 2026-02-10: CalendarSelectionMode must be an enum, not a string (consolidated)

**By:** Forge, Cyclops
**What:** Created `CalendarSelectionMode` enum in `Enums/CalendarSelectionMode.cs` with values None (0), Day (1), DayWeek (2), DayWeekMonth (3). Refactored `Calendar.SelectionMode` from string to enum. Also added `Caption`, `CaptionAlign`, `UseAccessibleHeader` properties. Fixed blocking `.GetAwaiter().GetResult()` call.
**Why:** Web Forms uses `CalendarSelectionMode` as an enum. Project convention requires every Web Forms enum to have a corresponding C# enum in `Enums/`. String-based modes are fragile. Blocking async calls risk deadlocks in Blazor's sync context.

### 2026-02-10: FileUpload must use Blazor InputFile internally (consolidated)

**By:** Forge, Cyclops
**What:** The `@onchange` binding on `<input type="file">` uses `ChangeEventArgs` which does not provide file data in Blazor. FileUpload MUST use Blazor's `<InputFile>` component internally instead of a raw `<input type="file">`. `InputFile` provides proper `InputFileChangeEventArgs` with `IBrowserFile` objects that enable all file operations. Without this, `HasFile` always returns false and `FileBytes`, `FileContent`, `PostedFile`, `SaveAs()` are all broken.
**Why:** Ship-blocking bug — the component cannot function without actual file data access. `InputFile` renders as `<input type="file">` in the DOM so existing tests still pass. Requires `@using Microsoft.AspNetCore.Components.Forms` in the `.razor` file. Any future component needing browser file access must use `InputFile`.

### 2026-02-10: ImageMap base class must be BaseStyledComponent

**By:** Forge
**What:** ImageMap should inherit `BaseStyledComponent`, not `BaseWebFormsComponent`. Web Forms `ImageMap` inherits from `Image` → `WebControl` which has style properties.
**Why:** `BaseWebFormsComponent` is insufficient for controls that need CssClass, Style, and other style properties.

### 2026-02-10: ImageMap categorized under Navigation Controls

**By:** Beast
**What:** ImageMap is categorized under Navigation Controls in the documentation nav, alongside HyperLink, Menu, SiteMapPath, and TreeView.
**Why:** ImageMap's primary purpose is clickable regions for navigation — it aligns with navigation-oriented controls rather than editor/display controls.

### 2026-02-10: Shelve ASCX CLI and VS Snippets indefinitely

**By:** Jeffrey T. Fritz (via Copilot)
**What:** PR #328 (ASCX CLI, issue #18) and PR #309 (VS Snippets, issue #11) removed from sprint plan and shelved indefinitely.
**Why:** Both PRs have merge conflicts and are considered risky. Not worth the effort right now.

### 2026-02-10: Docs and samples must ship with components

**By:** Jeffrey T. Fritz (via Copilot)
**What:** Documentation (Beast) and sample pages (Jubilee) must be delivered in the same sprint as the component they cover — never deferred to a later sprint.
**Why:** Components aren't complete without docs and samples.

### 2026-02-10: Sprint plan — 3-sprint roadmap

**By:** Forge
**What:** Sprint 1: Land & Stabilize current PRs (Calendar enum fix, FileUpload data flow, ImageMap base class, PageService merge). Sprint 2: Editor & Login Controls (MultiView, Localize, ChangePassword, CreateUserWizard). Sprint 3: Data Controls + Tooling + Polish (DetailsView, PasswordRecovery, migration guide, sample updates).
**Why:** Prioritizes getting current PRs mergeable first, then fills biggest control gaps, then invests in tooling and documentation.

### 2026-02-10: Sprint 1 gate review results

**By:** Forge
**What:** Gate review of Sprint 1 PRs: Calendar (#333) REJECTED — branch regressed, dev already has fixes, assigned to Rogue for triage. FileUpload (#335) REJECTED — `PostedFileWrapper.SaveAs()` missing path sanitization, assigned to Jubilee. ImageMap (#337) APPROVED — ready to merge. PageService (#327) APPROVED — ready to merge.
**Why:** Formal gate review to determine merge readiness. Lockout protocol enforced: Cyclops locked out of Calendar and FileUpload revisions.

### 2026-02-10: FileUpload SaveAs path sanitization required

**By:** Forge
**What:** `PostedFileWrapper.SaveAs()` must sanitize file paths to prevent path traversal attacks. `Path.Combine` silently drops earlier arguments if a later argument is rooted. Must use `Path.GetFileName()` and validate resolved paths.
**Why:** Security defect blocking merge of FileUpload (#335).

### 2026-02-10: Lockout protocol — Cyclops locked out of Calendar and FileUpload

**By:** Jeffrey T. Fritz
**What:** Cyclops is locked out of revising Calendar (#333) and FileUpload (#335). Calendar triage assigned to Rogue. FileUpload fix assigned to Jubilee.
**Why:** Lockout protocol enforcement after gate review rejection.

### 2026-02-10: Close PR #333 — Calendar work already on dev

**By:** Rogue
**What:** PR #333 (`copilot/create-calendar-component`) should be closed without merging. All Calendar work including enum fix, Caption/CaptionAlign/UseAccessibleHeader, and non-blocking OnDayRender is already on `dev` (commit `d33e156`). The PR branch has 0 unique commits — merging would be a no-op or actively harmful. Issue #332 is resolved on `dev`.
**Why:** Cyclops committed Calendar fixes directly to `dev` instead of the feature branch, leaving the PR branch behind with old broken code. Rebasing would produce an empty diff. Process note: future PR review fixes should go to the feature branch, not the target branch.

### 2026-02-10: Sprint 2 Design Review

**By:** Forge
**What:** Design specs for Sprint 2 components — MultiView + View, Localize, ChangePassword, CreateUserWizard — covering base classes, properties, events, templates, enums, HTML output, risks, and dependencies.
**Why:** Sprint 2 scope involves 4 new components touching shared systems (LoginControls, Enums, base classes). A design review before implementation prevents rework, ensures Web Forms fidelity, and establishes contracts between Cyclops (implementation), Rogue (tests), Beast (docs), and Jubilee (samples).

### 2026-02-10: Sprint 2 complete — 4 components shipped

**By:** Squad (Forge, Cyclops, Beast, Jubilee, Rogue)
**What:** Localize, MultiView+View, ChangePassword, and CreateUserWizard all shipped with full docs, sample pages, and tests. Build passes with 0 errors, 709 tests. status.md updated to 41/53 components (77%).
**Why:** Sprint 2 milestone — all planned components delivered with docs and samples per team policy.

### Integration test audit — full coverage achieved

**By:** Colossus
**What:** Audited all 74 sample page routes against existing smoke tests. Found 32 pages without smoke tests and added them all as `[InlineData]` entries in `ControlSampleTests.cs`. Added 4 new interaction tests in `InteractiveComponentTests.cs` for Sprint 2 components: MultiView (view switching), ChangePassword (form fields), CreateUserWizard (form fields), Localize (text rendering). Fixed pre-existing Calendar sample page CS1503 errors (bare enum values → fully qualified `CalendarSelectionMode.X`).
**Why:** Every sample page is a promise to developers. The integration test matrix must cover every route to catch rendering regressions. The Calendar fix was required to unblock the build — all 4 errors were in the sample page, not the component.

### 2026-02-10: Sprint 3 Scope and Plan

**By:** Forge
**What:** Sprint 3 scope finalized — DetailsView and PasswordRecovery are the two buildable components. Chart, Substitution, and Xml deferred indefinitely.
**Why:** With 48/53 components complete (91%), we have exactly 5 remaining. Three of them (Chart, Substitution, Xml) are poor candidates: Chart requires an entire charting library, Substitution is a cache-control mechanism that has no Blazor equivalent, and Xml/XSLT transforms are a dead-end technology with near-zero migration demand. DetailsView and PasswordRecovery are the only two that provide real migration value and are feasible to build.

### 2026-02-10: Colossus added — dedicated integration test engineer

**By:** Jeffrey T. Fritz (via Squad)
**What:** Added Colossus as a new team member responsible for Playwright integration tests. Colossus owns `samples/AfterBlazorServerSide.Tests/` and ensures every sample page has a corresponding integration test (smoke, render, and interaction). Rogue retains ownership of bUnit unit tests. Integration testing split from Rogue's QA role.
**Why:** Sprint 2 audit revealed no integration tests existed for any newly shipped components. Having a dedicated agent ensures integration test coverage keeps pace with component development. Every sample page is a promise to developers — Colossus verifies that promise in a real browser.

### 2026-02-11: Deferred controls documentation pattern established

**By:** Beast
**What:** Created `docs/Migration/DeferredControls.md` to document the three permanently deferred controls (Chart, Substitution, Xml). Each control gets its own section with: what it did in Web Forms, why it's not implemented, and the recommended Blazor alternative with before/after migration examples. Added to mkdocs.yml nav under Migration.
**Why:** Components without documentation don't exist for the developer trying to migrate. Even controls we *don't* implement need a clear "here's what to do instead" — otherwise developers hit a dead end with no guidance. This pattern can be reused if additional controls are deferred in the future.

### 2026-02-11: DetailsView and PasswordRecovery documentation shipped

**By:** Beast
**What:** Created `docs/DataControls/DetailsView.md` and `docs/LoginControls/PasswordRecovery.md`. Both added to `mkdocs.yml` nav in alphabetical order. README.md updated with documentation links for both components.
**Why:** Sprint 3 requires docs to ship with components per team policy. Both docs follow the established convention: title → MS docs link → Features Supported → NOT Supported → Web Forms syntax → Blazor syntax → HTML output → Migration Notes (Before/After) → Examples → See Also. PasswordRecovery follows the ChangePassword.md pattern for login controls; DetailsView follows the data control patterns from FormView/GridView.

### 2026-02-11: DetailsView inherits DataBoundComponent, not BaseStyledComponent

**By:** Cyclops
**What:** DetailsView inherits `DataBoundComponent<ItemType>` (same as GridView and FormView) rather than `BaseStyledComponent`. The `CssClass` property is declared directly on the component. Event args use separate DetailsView-specific types (`DetailsViewCommandEventArgs`, `DetailsViewModeEventArgs`, etc.) rather than reusing FormView's event args.
**Why:** Web Forms DetailsView inherits from `CompositeDataBoundControl`, which is a data-bound control. GridView and FormView already follow this pattern in the codebase via `DataBoundComponent<T>`. Using separate event args (not FormView's) matches Web Forms where `DetailsViewInsertEventArgs` and `FormViewInsertEventArgs` are distinct types. The `DetailsViewMode` enum is also separate from `FormViewMode` even though they have identical values — Web Forms treats them as distinct types.

### 2026-02-11: PasswordRecovery component — inherits BaseWebFormsComponent, 3-step flow

**By:** Cyclops
**What:** Built PasswordRecovery component in `LoginControls/` with 3-step password reset flow (UserName → Question → Success). Inherits `BaseWebFormsComponent` following existing login control patterns. Created `SuccessTextStyle` sub-component, `MailMessageEventArgs`, and `SendMailErrorEventArgs` event args. Each step has its own `EditForm`. Styles cascade via existing sub-components (`TitleTextStyle`, `LabelStyle`, `TextBoxStyle`, etc.) plus new `SuccessTextStyle`. The `SubmitButtonStyle` property maps to `LoginButtonStyle` cascading name to reuse existing sub-component. Templates (`UserNameTemplate`, `QuestionTemplate`, `SuccessTemplate`) allow full customization of each step.
**Why:** Sprint 3 deliverable. Followed ChangePassword/CreateUserWizard patterns for consistency. Used `BaseWebFormsComponent` instead of spec-suggested `BaseStyledComponent` because all existing login controls use this base class and manage styles through CascadingParameters. Per-step `EditForm` prevents validation interference between steps.

### 2026-02-11: Sprint 3 gate review — DetailsView and PasswordRecovery APPROVED

**By:** Forge
**What:** Both Sprint 3 components passed gate review. DetailsView: correct `DataBoundComponent<ItemType>` inheritance, all 10 events with proper EventArgs, table-based HTML matching Web Forms, `DetailsViewMode` enum. 3 minor non-blocking issues (CellPadding/CellSpacing logic, docs DataSource→Items). PasswordRecovery: correct `BaseWebFormsComponent` inheritance, 3-step wizard flow, all 6 events, table-based nested HTML. 3 minor non-blocking issues (RenderOuterTable unused, SubmitButtonType unused, sample uses Sender casting vs @ref). Build: 0 errors, 797 tests. Status: 50/53 (94%).
**Why:** Formal gate review — both components meet Web Forms fidelity standards. No blocking issues. Minor issues tracked but do not prevent shipping.

### 2026-02-11: DetailsView sample uses Items parameter with inline data

**By:** Jubilee
**What:** DetailsView sample page uses the `Items` parameter with an inline `List<Customer>` rather than `SelectMethod` for data binding. This matches the GridView RowSelection sample pattern and is the clearest way to demonstrate paging and mode-switching without requiring a static query method.
**Why:** The `SelectMethod` approach (used in GridView Default) requires a specific method signature with `out totalRowCount` that adds complexity. For a single-record-at-a-time control like DetailsView, the `Items` parameter is more natural and easier for migrating developers to understand. Both patterns work; `Items` is preferred for sample clarity.

### 2026-02-12: Sprint 3 bUnit tests shipped — DetailsView + PasswordRecovery

**By:** Rogue
**What:** 71 new bUnit tests added for Sprint 3 components: 42 for DetailsView (5 test files: Rendering, HeaderFooter, CommandRow, Events, Paging) and 29 for PasswordRecovery (2 test files: Step1UserName, BasicFlow). Total test count now 797, all passing.
**Why:** QA gate for Sprint 3 — both components needed comprehensive unit test coverage before merge. Tests verify rendering fidelity (table structure, property names/values, empty data, header/footer), interactive behavior (mode switching, paging, event firing), and edge cases (null items, single item paging, cancel flows, failure text display).

### 2026-02-12: Chart component JS library evaluation

**By:** Forge
**What:** Recommend Chart.js via JS interop as the rendering engine for a Chart component, targeting 8 core chart types. Do NOT build from scratch with D3. Accept the HTML output deviation (SVG/Canvas instead of `<img>`) as a documented, justified exception to the project's HTML fidelity rule.
**Why:** See full analysis below.

---

## 1. Web Forms Chart Control Analysis

#### What It Is
`System.Web.UI.DataVisualization.Charting.Chart` (namespace: `System.Web.UI.DataVisualization.Charting`, assembly: `System.Web.DataVisualization.dll`) is a server-side charting control that shipped with .NET Framework 3.5 SP1+. It inherits from `DataBoundControl` → `WebControl` → `Control`.

#### Chart Types (35 total via `SeriesChartType` enum)
| # | Type | # | Type | # | Type |
|---|------|---|------|---|------|
| 0 | Point | 12 | StackedColumn100 | 24 | RangeColumn |
| 1 | FastPoint | 13 | Area | 25 | Radar |
| 2 | Bubble | 14 | SplineArea | 26 | Polar |
| 3 | Line | 15 | StackedArea | 27 | ErrorBar |
| 4 | Spline | 16 | StackedArea100 | 28 | BoxPlot |
| 5 | StepLine | 17 | Pie | 29 | Renko |
| 6 | FastLine | 18 | Doughnut | 30 | ThreeLineBreak |
| 7 | Bar | 19 | Stock | 31 | Kagi |
| 8 | StackedBar | 20 | Candlestick | 32 | PointAndFigure |
| 9 | StackedBar100 | 21 | Range | 33 | Funnel |
| 10 | Column | 22 | SplineRange | 34 | Pyramid |
| 11 | StackedColumn | 23 | RangeBar | | |

#### Key Properties & Sub-Objects
- **Chart:** `Series` (collection), `ChartAreas` (collection), `Legends` (collection), `Titles` (collection), `Annotations` (collection), `DataSource`, `Width`, `Height`, `ImageType`, `RenderType`, `Palette`
- **Series:** `Name`, `ChartType`, `Points` (DataPointCollection), `XValueMember`, `YValueMembers`, `Color`, `BorderWidth`, `ChartArea`, `Legend`, `IsVisibleInLegend`, `MarkerStyle`, `ToolTip`
- **DataPoint:** `XValue`, `YValues[]`, `Label`, `Color`, `ToolTip`, `IsValueShownAsLabel`
- **ChartArea:** `Name`, `AxisX`, `AxisY`, `AxisX2`, `AxisY2`, `Area3DStyle`, `BackColor`
- **Axis:** `Title`, `Minimum`, `Maximum`, `Interval`, `LabelStyle`, `MajorGrid`, `MinorGrid`, `IsLogarithmic`
- **Legend:** `Name`, `Docking`, `Alignment`, `Title`
- **Title:** `Text`, `Font`, `Alignment`, `Docking`

#### HTML Output
The Web Forms Chart control renders as an **`<img>` tag** pointing to a server-side image handler:
```html
<img src="/ChartImg.axd?i=chart_ABC123.png&g=..." alt="Chart" style="height:300px;width:400px;" />
```
The actual chart is a **server-generated PNG/JPEG/BMP image** produced by GDI+ (`System.Drawing`). The `ChartImg.axd` HTTP handler streams the image bytes on demand. There is no client-side rendering whatsoever — it's purely a raster image.

---

## 2. JS Library Evaluation

#### Evaluation Criteria
| Criterion | Weight | Rationale |
|-----------|--------|-----------|
| License | Must-have | Project is MIT; library must be MIT-compatible |
| Bundle size | High | Library ships with the NuGet package |
| Chart type coverage | High | Must cover the core Web Forms chart types |
| Blazor wrapper exists | High | Reduces our JS interop surface area dramatically |
| Ease of integration | Medium | Less JS code = less maintenance burden |
| Rendering tech | Info | Canvas vs SVG affects testability |

#### D3.js
| Aspect | Assessment |
|--------|------------|
| License | BSD-3-Clause (MIT-compatible ✓) |
| Bundle size | ~150-250 KB gzipped (full); modular |
| Chart types built-in | **Zero.** D3 is a DOM manipulation toolkit, not a charting library. Every chart type must be built from scratch using SVG primitives. |
| Blazor wrapper | **None.** No maintained C# wrapper. All integration is raw `IJSRuntime` interop. |
| Effort to implement | **XL.** We'd be writing an entire charting library on top of D3. Each chart type is hundreds of lines of JS (axes, scales, transitions, tooltips, legends). |
| Verdict | **❌ REJECT.** D3 is the wrong abstraction level. It's like suggesting we use System.Drawing to implement the Chart control — that's literally what Microsoft did, and it took a team years. |

#### Chart.js
| Aspect | Assessment |
|--------|------------|
| License | MIT ✓ |
| Bundle size | ~60 KB gzipped |
| Chart types | Line, Bar, Pie, Doughnut, Radar, Polar Area, Bubble, Scatter (~10 built-in) |
| Blazor wrapper | **Yes.** Multiple: PSC.Blazor.Components.Chartjs (active, .NET 8), ChartJs.Blazor forks |
| API style | Declarative JSON config → maps well to C# object model |
| Rendering | Canvas (HTML5 `<canvas>`) |
| Verdict | **✅ RECOMMENDED.** Best balance of size, coverage, and Blazor ecosystem support. |

#### ApexCharts
| Aspect | Assessment |
|--------|------------|
| License | MIT ✓ |
| Bundle size | ~100-130 KB gzipped |
| Chart types | 20+ types including candlestick, heatmap, treemap, boxplot, radar |
| Blazor wrapper | **Yes.** Blazor-ApexCharts (official, actively maintained, .NET 8+, 6.1.0) |
| API style | Declarative, strongly typed Blazor components with `<ApexChart>` / `<ApexPointSeries>` |
| Rendering | SVG |
| Verdict | **✅ STRONG ALTERNATIVE.** Better chart type coverage than Chart.js. Larger bundle but still reasonable. The official Blazor wrapper is the most mature in the ecosystem. |

#### Plotly.js
| Aspect | Assessment |
|--------|------------|
| License | MIT ✓ |
| Bundle size | **3-4 MB gzipped** |
| Chart types | 40+ including 3D, contour, Sankey, geo maps |
| Blazor wrapper | Limited — Plotly.Blazor exists but less maintained |
| Rendering | SVG + WebGL |
| Verdict | **❌ REJECT.** Bundle size is disqualifying. A 3-4 MB JS dependency for a component library is unreasonable. The 3D/scientific features are overkill — Web Forms Chart was never used for that. |

#### Summary Matrix

| Library | License | Size (gz) | Chart Types | Blazor Wrapper | Effort | Verdict |
|---------|---------|-----------|-------------|----------------|--------|---------|
| D3.js | BSD-3 | ~150KB | 0 (DIY) | None | XL | ❌ |
| **Chart.js** | **MIT** | **~60KB** | **~10** | **Yes (multiple)** | **M** | **✅** |
| ApexCharts | MIT | ~120KB | ~20+ | Yes (official) | M | ✅ Alt |
| Plotly.js | MIT | ~3.5MB | 40+ | Limited | L | ❌ |

---

## 3. Architecture Decision

#### The HTML Output Problem

This is the elephant in the room. Our project's #1 rule is **identical HTML output**. Web Forms Chart renders:
```html
<img src="/ChartImg.axd?..." style="height:300px;width:400px;" />
```

A JS library renders:
```html
<canvas width="400" height="300"></canvas>  <!-- Chart.js -->
<!-- or -->
<div class="apexcharts-canvas"><svg>...</svg></div>  <!-- ApexCharts -->
```

**These are fundamentally different.** There is no way to make a JS charting library produce an `<img>` tag. This is a **justified exception** to the HTML fidelity rule for these reasons:

1. **The `<img>` output was an implementation detail, not a semantic contract.** Developers never targeted the `<img>` tag with CSS or JS — they styled the chart through the Chart control's own API (colors, fonts, borders). The surrounding `<div>` with `width`/`height` provides the same layout behavior.

2. **Server-side image generation is architecturally impossible in Blazor.** Blazor has no equivalent to `ChartImg.axd` handlers, no `System.Drawing` (without SkiaSharp), and no server-side image streaming pipeline. Even if we wanted to replicate the `<img>` approach, we'd need a separate HTTP endpoint, image generation library, and caching layer — which is outside the scope of a component library.

3. **The migration value is in the API, not the HTML.** Developers migrating Chart usage care about preserving their `Series`, `ChartAreas`, `DataPoints` configuration — not the output format. A Chart that accepts the same property names and renders a visually equivalent chart is a successful migration.

4. **Precedent exists in the project.** The deferred controls documentation already acknowledges that Chart requires a fundamentally different approach. We're formalizing that acknowledgment.

**Recommendation:** Document this as an explicit HTML output exception in the Chart component's docs. Add a "Migration Notes" section explaining that the output changes from `<img>` (raster) to `<canvas>`/`<svg>` (vector), and that this is actually an **upgrade** (better resolution, interactivity, accessibility).

#### API Surface Design

The Web Forms Chart has hundreds of properties across its object hierarchy. We should emulate the **structure** but not the **completeness**:

```razor
@* Proposed Blazor API — mirrors Web Forms declarative markup *@
<Chart Width="400px" Height="300px" Palette="ChartPalette.Berry">
    <ChartAreas>
        <ChartArea Name="MainArea">
            <AxisX Title="Month" />
            <AxisY Title="Revenue ($)" />
        </ChartArea>
    </ChartAreas>
    <Series>
        <ChartSeries Name="Sales" ChartType="SeriesChartType.Column"
                     XValueMember="Month" YValueMembers="Amount"
                     Items="@salesData" />
        <ChartSeries Name="Costs" ChartType="SeriesChartType.Line"
                     XValueMember="Month" YValueMembers="Amount"
                     Items="@costData" />
    </Series>
    <Legends>
        <ChartLegend Name="Default" Docking="Docking.Bottom" />
    </Legends>
    <Titles>
        <ChartTitle Text="Monthly Performance" />
    </Titles>
</Chart>
```

Compare to Web Forms:
```aspx
<asp:Chart ID="Chart1" runat="server" Width="400px" Height="300px" Palette="Berry">
    <ChartAreas>
        <asp:ChartArea Name="MainArea">
            <AxisX Title="Month" />
            <AxisY Title="Revenue ($)" />
        </asp:ChartArea>
    </ChartAreas>
    <Series>
        <asp:Series Name="Sales" ChartType="Column"
                    XValueMember="Month" YValueMembers="Amount" />
    </Series>
    <Legends>
        <asp:Legend Name="Default" Docking="Bottom" />
    </Legends>
    <Titles>
        <asp:Title Text="Monthly Performance" />
    </Titles>
</asp:Chart>
```

The migration path: remove `asp:` prefix, remove `runat="server"`, change data binding from `DataSource`/`DataBind()` to `Items` parameter (consistent with GridView/DetailsView pattern).

#### Scope: Initial Chart Type Subset

Phase 1 — **8 chart types** covering 90%+ of real-world Web Forms Chart usage:

| Priority | SeriesChartType | Chart.js Type | ApexCharts Type |
|----------|----------------|---------------|-----------------|
| P0 | Column | `bar` (vertical) | `bar` |
| P0 | Bar | `bar` (horizontal) | `bar` (horizontal) |
| P0 | Line | `line` | `line` |
| P0 | Pie | `pie` | `pie` |
| P0 | Area | `line` (fill) | `area` |
| P1 | Doughnut | `doughnut` | `donut` |
| P1 | Scatter/Point | `scatter` | `scatter` |
| P1 | StackedColumn | `bar` (stacked) | `bar` (stacked) |

Phase 2 (future, if demand exists): Radar, Polar, Bubble, Spline, Stock/Candlestick.

Phase 3 (probably never): Renko, Kagi, ThreeLineBreak, PointAndFigure, Funnel, Pyramid — these are exotic financial/statistical types with near-zero migration demand.

#### Which JS Library?

**Primary recommendation: Chart.js**

Rationale:
1. **Smallest bundle** (~60KB gz) — critical for a library that NuGet consumers may not need
2. **Covers all 8 Phase 1 chart types** out of the box
3. **Multiple Blazor wrappers exist** — we can either use one or write a thin interop layer
4. **Canvas rendering** — simpler DOM footprint, no SVG complexity in test assertions
5. **Declarative JSON config** — maps cleanly to a C# object model that mirrors Web Forms' property hierarchy

**Why not ApexCharts?**
ApexCharts is technically superior (more chart types, SVG rendering, official Blazor wrapper), but:
- Twice the bundle size for features we don't need in Phase 1
- The official Blazor wrapper (`Blazor-ApexCharts`) has its own opinionated API that doesn't match Web Forms naming — we'd be wrapping a wrapper
- If we ever expand to Phase 2 (Stock/Candlestick), ApexCharts becomes more compelling. We can pivot then.

**Architecture approach:**
- **Do NOT take a NuGet dependency on a Blazor Chart.js wrapper package.** These wrappers are community-maintained with uncertain longevity. Instead:
- Bundle `chart.min.js` (~60KB) in the project's static assets
- Write a thin JS interop module (`chart-interop.js`) that translates our C# config objects to Chart.js config
- The Blazor `Chart` component converts its Web Forms-style properties to a JSON config, passes it to JS interop, and Chart.js renders into a `<canvas>` element
- This gives us full control over the API surface and no external NuGet dependency risk

---

## 4. Final Recommendation

| Item | Decision |
|------|----------|
| **JS Library** | Chart.js (MIT, ~60KB gz, bundled as static asset) |
| **Approach** | Thin JS interop — no external Blazor wrapper dependency |
| **Phase 1 chart types** | Column, Bar, Line, Pie, Area, Doughnut, Scatter, StackedColumn |
| **API design** | Mirror Web Forms property names (`Series`, `ChartAreas`, `Legends`, `Titles`, `SeriesChartType` enum) |
| **HTML output exception** | Documented deviation — `<canvas>` instead of `<img>`. Justified by architectural impossibility and migration value being in the API, not the HTML. |
| **Effort estimate** | **L (Large)** — breakdown below |
| **Base class** | `DataBoundComponent<T>` (Web Forms Chart inherits `DataBoundControl`) |
| **Enums needed** | `SeriesChartType`, `ChartPalette`, `Docking`, `ChartDashStyle` |

#### Effort Breakdown (Large)
- **C# component hierarchy:** Chart, ChartArea, Series (ChartSeries), DataPoint, Legend, Title, Axis — ~8 component/class files. **M effort.**
- **JS interop layer:** `chart-interop.js` bridging C# config → Chart.js config. **M effort.**
- **SeriesChartType mapping:** Translating our enum values to Chart.js type strings + config. **S effort.**
- **Testing:** bUnit tests for component API + manual visual testing (canvas content can't be asserted in bUnit). **M effort.**
- **Docs, samples, migration guide:** Per team policy. **M effort.**
- **Total: L** — comparable to DetailsView in component count, but the JS interop layer adds a new dimension we haven't tackled before.

#### Risks & Gotchas

1. **JS interop is a new pattern for this project.** Every other component is pure Blazor/C#. Chart will be the first (and likely only) component requiring JavaScript. This adds complexity to the build, test, and packaging pipeline.

2. **Canvas can't be unit-tested with bUnit.** We can test the component renders a `<canvas>` element and passes correct config JSON, but we can't assert the visual output. Integration tests (Playwright screenshots) become the primary quality gate.

3. **Chart.js version management.** We're bundling a specific version of `chart.min.js`. Major version upgrades could break our interop layer. Pin the version and document it.

4. **Data binding model differs.** Web Forms Chart uses `DataSource` + `DataBind()` with `XValueMember`/`YValueMembers` string-based binding. Our Blazor version should use the project's established `Items` parameter pattern (typed collection), with `XValueMember`/`YValueMembers` as lambda expressions or property name strings for member access.

5. **No image export.** Web Forms Chart could `SaveImage()` to a file. Chart.js can export to base64 PNG via `chart.toBase64Image()`, but this requires JS interop. Consider exposing as a future feature, not Phase 1.

6. **SSR/prerendering.** Chart.js requires a DOM and `<canvas>` element. The component must suppress rendering during server-side prerendering (`OnAfterRenderAsync` pattern) or show a placeholder.

#### Decision: Proceed or Defer?

**Recommendation: Proceed with Chart.js, but as a separate sprint/milestone — not Sprint 3.**

The library is at 50/53 (94%) and effectively feature-complete for practical migration. Chart is the highest-value remaining component, and JS interop makes it feasible. However, it introduces a new architectural pattern (JS dependencies, interop layer, canvas testing) that warrants dedicated planning, not tacking onto an existing sprint.

Suggested timeline:
- **Sprint 4 planning:** Design review for Chart component architecture, JS interop patterns, build/packaging changes
- **Sprint 4 execution:** Implement Chart with Phase 1 types, docs, samples, integration tests
- **Post-Sprint 4:** Evaluate demand for Phase 2 chart types

### 2026-02-12: Milestone 4 — Chart component with Chart.js
**By:** Forge (architecture), Squad (planning)
**What:** Milestone 4 will implement the Chart component using Chart.js (~60KB, MIT) via Blazor JS interop. Phase 1: 8 chart types (Column, Bar, Line, Pie, Area, Doughnut, Scatter, StackedColumn). HTML output exception: `<canvas>` instead of `<img>` (justified — API fidelity is the migration value, not HTML fidelity). 8 work items across 5 waves. Target: 51/53 components (96%).
**Why:** Chart is the highest-value remaining deferred component. Chart.js provides the best balance of bundle size, chart type coverage, and Blazor ecosystem support. D3 rejected (wrong abstraction), Plotly rejected (3-4MB bundle). Design review required before implementation (auto-triggered ceremony).

### 2026-02-12: User directive — use "milestones" not "sprints" (consolidated)
**By:** Jeffrey T. Fritz (via Copilot)
**What:** Going forward, use "milestones" instead of "sprints" for naming work batches. All future planning uses "milestone" terminology.
**Why:** User preference — captured for team memory. Applies retroactively to planning references where practical.

### 2026-02-23: AccessKey and ToolTip must be added to BaseStyledComponent (consolidated)

**By:** Beast, Cyclops
**What:** `AccessKey` (string) and `ToolTip` (string) are missing from all styled controls. Both are standard `WebControl` properties present on every control inheriting `WebControl` in Web Forms. Beast's audit of 15 editor controls (L–X) and Cyclops's audit of 13 editor controls (A–I) independently confirmed the gap. 7 of 13 A–I controls add ToolTip individually; the remaining 6 lack it entirely.
**Recommendation:** Add `[Parameter] public string AccessKey { get; set; }` and `[Parameter] public string ToolTip { get; set; }` to `BaseStyledComponent`. This fixes the gap for all 20+ styled controls in one change.
**Why:** Universal gap confirmed by two independent audits across 28 controls. Base-class fix is the highest-leverage single change available.

### 2026-02-23: Label should inherit BaseStyledComponent

**By:** Beast
**What:** `Label` currently inherits `BaseWebFormsComponent` but Web Forms `Label` inherits from `WebControl`. This means Label is missing all 9 style properties (CssClass, BackColor, ForeColor, Font, etc.). Also consider `AssociatedControlID` to render `<label for="...">` instead of `<span>`.
**Why:** Label is one of the most commonly used controls. Missing style properties break migration for any page that styles its Labels.

### 2026-02-23: Substitution and Xml remain permanently deferred

**By:** Beast
**What:** Both controls are tightly coupled to server-side ASP.NET infrastructure: Substitution depends on the output caching pipeline (`HttpResponse.WriteSubstitution`); Xml depends on XSLT transformation. Neither maps to Blazor's component model.
**Recommendation:** Document migration alternatives in `DeferredControls.md`. Substitution → Blazor component lifecycle. Xml → convert XML to C# objects and use Blazor templates.
**Why:** Reinforces and formalizes the Sprint 3 deferral decision with specific migration guidance.

### 2026-02-23: Chart Type Gallery documentation convention

**By:** Beast
**What:** Added a "Chart Type Gallery" section to `docs/DataControls/Chart.md` showing screenshots of all 8 Phase 1 chart types. Each entry includes an H3 heading, MkDocs image (`![alt](../images/chart/chart-{type}.png)`), `SeriesChartType` enum value, and usage description. Pie and Doughnut include `!!! warning "Palette Limitation"` admonitions for the Phase 1 single-color-per-series issue.
**Why:** Visual documentation helps migrating developers choose the correct chart type. Image path convention: `docs/images/{component}/` with `chart-{type}.png` naming.

### 2026-02-23: Feature audit findings — Editor Controls A–I

**By:** Cyclops
**What:** Completed feature comparison audit for 13 editor controls (AdRotator through ImageMap). Key findings beyond AccessKey/ToolTip (consolidated separately):
1. **Image needs BaseStyledComponent.** Image inherits `BaseWebFormsComponent` but Web Forms `Image` inherits `WebControl`, leaving 10+ style properties missing. ImageMap was already fixed — Image should follow suit.
2. **HyperLink.NavigateUrl vs NavigationUrl.** Blazor uses `NavigationUrl` but Web Forms uses `NavigateUrl`. Migration-breaking name difference.
3. **List controls share common gaps.** BulletedList, CheckBoxList, and DropDownList all lack `DataTextFormatString`, `AppendDataBoundItems`, `CausesValidation`, and `ValidationGroup` from Web Forms `ListControl` base.
**Why:** Prioritized work items for closing API gaps. Image base class fix is a single targeted change with high impact.

### 2026-02-23: Chart component implementation architecture (consolidated)

**By:** Cyclops, Forge
**What:** Combined implementation decisions from Cyclops (WI-1/2/3 execution) and Forge (Milestone 4 design review):
1. **Base class: DataBoundStyledComponent<T>.** New class inheriting `DataBoundComponent<T>` + implementing `IStyle`. Fills the structural gap where neither `DataBoundComponent<T>` (no styles) nor `BaseStyledComponent` (no data binding) alone satisfies the Web Forms Chart contract. Additive — does not affect existing components.
2. **Child registration via CascadingParameter.** ChartSeries, ChartArea, ChartTitle, ChartLegend use `[CascadingParameter(Name="ParentChart")]` and register in `OnInitializedAsync`, following the MultiView/View pattern.
3. **JS interop: Three-function ES module.** `chart-interop.js` exports `createChart`, `updateChart`, `destroyChart`. `ChartJsInterop` is separate from `BlazorWebFormsJsInterop` — chart-specific JS stays isolated. Lazy `IJSObjectReference` import pattern. Canvas referenced by `id`, not `ElementReference`.
4. **Chart.js v4.4.8 bundled.** Pinned as static asset in `wwwroot/js/`. Originally a placeholder stub; now replaced with real Chart.js v4.4.8.
5. **Phase 1 chart types.** 8 mappings: Column→bar, Bar→bar(indexAxis:'y'), Line→line, Pie→pie, Area→line(fill), Doughnut→doughnut, Point→scatter, StackedColumn→bar(stacked). `SeriesChartType.Point` maps to Chart.js `"scatter"` (Web Forms has no explicit Scatter value). Full 35-value enum created; unsupported types throw `NotSupportedException`.
6. **ChartConfigBuilder uses snapshot classes.** Pure static class taking config snapshots (`ChartSeriesConfig`, `ChartAreaConfig`, etc.) via `.ToConfig()` methods. Decouples builder from component lifecycle, enables unit testing. Canvas content tested visually via Playwright.
7. **ChartWidth/ChartHeight as string parameters.** Avoids hiding base `Width`/`Height` (Unit type) on `BaseStyledComponent`.
8. **Docking parameter naming.** `ChartLegend.LegendDocking` and `ChartTitle.TitleDocking` use prefixed names to avoid conflicts. Nullable `Docking?` distinguishes "not set" from default.
9. **Task.Yield() before first chart creation.** Gives child components time to register before JS interop fires.
10. **Enums: 4 new files.** `SeriesChartType` (35), `ChartPalette` (13), `Docking` (4), `ChartDashStyle` (6) in `Enums/` per project convention.
**Why:** Consolidates architecture decisions from design review and implementation to provide a single reference for chart component patterns.

### 2026-02-23: DataBoundComponent style property gap (consolidated)

**By:** Forge
**What:** Controls inheriting `DataBoundComponent<T>` (DataGrid, GridView, FormView, DetailsView, ListView) lack all WebControl-level style properties because the chain `DataBoundComponent<T> → BaseDataBoundComponent → BaseWebFormsComponent` skips `BaseStyledComponent`. Only DataList works around this by implementing `IStyle` directly. GridView re-declares `CssClass` as a standalone `[Parameter]` — a pattern smell.
**Recommendation:** Create `DataBoundStyledComponent<T>` inheriting `DataBoundComponent<T>` and implementing `IStyle`. Chart already uses this approach. This would immediately give BackColor, BorderColor, CssClass, Font, ForeColor, Height, Width, etc. to all 5 affected data controls.
**Why:** Affects 5 of 9 data controls. Identified independently in both the data controls audit and chart design review. Single base-class fix with broad impact.

### 2026-02-23: GridView is highest-priority data control gap

**By:** Forge
**What:** GridView is the most commonly used data control in Web Forms and has the weakest coverage: no paging, sorting, editing, selection, no style properties, 14 of 22 events missing. Currently read-only table rendering only.
**Recommendation:** GridView enhancement should be a near-term priority, potentially as a Milestone 5 workstream.
**Why:** Developers migrating from Web Forms will expect GridView to be functional.

### 2026-02-23: DetailsView branch should be merged forward

**By:** Forge
**What:** DetailsView has strong implementation (27 props, 16 events with cancellation) but only exists on `sprint3/detailsview-passwordrecovery`. It is not on the current working branch.
**Why:** APPROVED in Milestone 3 gate review. Should be available on the main development branches.

### 2026-02-23: Themes and Skins — CascadingValue ThemeProvider recommended

**By:** Forge
**What:** Evaluated 5 approaches for Web Forms Themes/Skins migration. Recommended CascadingValue ThemeProvider — the only approach that faithfully models both `Theme` (override) and `StyleSheetTheme` (default) semantics. CSS-only approaches cannot set non-CSS properties. DI approach cannot scope to subtrees. SkinID must be corrected from `bool` to `string` on `BaseWebFormsComponent` first. Implementation is opt-in via `<ThemeProvider>` wrapper — zero breaking changes. Strategy is exploratory; README exclusion of themes/skins still stands.
**Why:** Jeff requested exploration. CascadingValue aligns with existing library patterns (TableItemStyle already cascades) and is incrementally adoptable.

### 2026-02-23: PasswordRecovery component missing from current branch

**By:** Rogue
**What:** `PasswordRecovery.razor` and `.razor.cs` do not exist in `src/BlazorWebFormsComponents/LoginControls/` despite history.md referencing Sprint 3 delivery with 29 bUnit tests. Component exists on `sprint3/detailsview-passwordrecovery` branch.
**Recommendation:** Merge the sprint3 branch forward or cherry-pick the PasswordRecovery files.
**Why:** Confirmed by audit. Related to DetailsView branch merge-forward decision.

### 2026-02-23: Validation Display property gap — migration-blocking

**By:** Rogue
**What:** ALL six validation controls are missing the `Display` property (`ValidatorDisplay` enum: None, Static, Dynamic). Current Blazor implementation always uses Dynamic behavior (hidden when valid). `Static` reserves space even when valid — pages relying on this for layout stability will break.
**Recommendation:** Add `Display` parameter to `BaseValidator<T>`.
**Why:** Migration-blocking for pages using `Display="Static"`.

### 2026-02-23: ValidationSummary functional gaps and comma-split bug

**By:** Rogue
**What:** `AspNetValidationSummary` is missing `HeaderText`, `ShowMessageBox`, `ShowSummary`, `ShowValidationErrors`, and `ValidationGroup`. Error message parsing uses `x.Split(',')[1]` which silently corrupts messages containing commas.
**Recommendation:** Fix comma-split bug immediately (data corruption risk). Prioritize `HeaderText` and `ValidationGroup` (common in multi-form pages).
**Why:** Comma-split is a latent data corruption bug. Missing properties affect multi-form page migration.

### 2026-02-23: Login controls missing outer WebControl style properties

**By:** Rogue
**What:** Login, ChangePassword, CreateUserWizard, and LoginView inherit `BaseWebFormsComponent` instead of `BaseStyledComponent`, so they lack outer-level style properties (BackColor, CssClass, ForeColor, Width, Height). Only LoginName and LoginStatus have full style support. Web Forms applies outer styles to the wrapping `<table>`.
**Recommendation:** Evaluate whether these composite controls should inherit `BaseStyledComponent` or if CascadingParameter sub-element styles are sufficient.
**Why:** Migrating pages that set `CssClass` on Login controls will break.

### 2026-02-12: ChangePassword and CreateUserWizard sample pages require LoginControls using directive
**By:** Colossus
**What:** Added `@using BlazorWebFormsComponents.LoginControls` to `ChangePassword/Index.razor` and `CreateUserWizard/Index.razor`. Without this, the components render as raw HTML custom elements instead of Blazor components — silently failing with no error.
**Why:** The root `@using BlazorWebFormsComponents` in `_Imports.razor` does not cover sub-namespaces like `LoginControls`. Any future sample page using Login Controls must include this directive. PasswordRecovery already had it; these two were missed during Sprint 2.

### 2026-02-12: External placeholder URLs replaced with local SVG images
**By:** Colossus
**What:** Replaced all `https://via.placeholder.com/...` URLs in Image and ImageMap sample pages with local SVG placeholder images in `wwwroot/img/`. Created 8 SVG files matching the sizes used in the samples.
**Why:** External URLs are unreachable in CI/test environments, causing integration test failures. Local SVGs are always available and test-safe. Future sample pages must never use external image URLs.

### 2026-02-12: ASP.NET Core structured log messages filtered in integration tests
**By:** Colossus
**What:** Added a regex filter in `VerifyPageLoadsWithoutErrors` to exclude browser console messages matching `^\[\d{4}-\d{2}-\d{2}T` (ISO 8601 timestamp prefix). These are ASP.NET Core ILogger messages forwarded to the browser console by Blazor Server, not actual page errors.
**Why:** Without this filter, any page that triggers framework-level logging (e.g., Calendar with many interactive elements) produces false positive test failures.

### 2026-02-12: Milestone exit criteria — samples and integration tests mandatory

**By:** Jeffrey T. Fritz
**What:** Every milestone/sprint must meet ALL of the following exit criteria before submission for review:
1. **Samples for every feature** — Every feature of every component created or modified in the sprint must have a corresponding sample page demonstrating it
2. **Integration tests for every sample** — Every sample page added must have at least one Playwright integration test verifying its interactive behavior
3. **All integration tests pass** — 100% of integration tests (both new and pre-existing) must pass before the sprint is submitted
**Why:** Sprint 3 exposed gaps where components shipped without full sample coverage and integration tests. This gate ensures no sprint is declared complete until the full chain — component → sample → integration test → green — is verified. This is a permanent policy for all future sprints.

### DetailsView auto-generated fields render inputs in Edit/Insert mode

**By:** Cyclops
**What:** Fixed `DetailsViewAutoField.GetValue()` to render `<input type="text">` elements when the DetailsView is in Edit or Insert mode, instead of always rendering plain text. Edit mode pre-fills the input with the current property value; Insert mode renders an empty input. ReadOnly mode continues to render plain text as before.
**Why:** The `mode` parameter was being ignored — the method always rendered plain text regardless of mode. This broke the Edit workflow: clicking "Edit" switched the command row buttons correctly but fields remained non-editable. This matches ASP.NET Web Forms behavior where auto-generated fields become textboxes in edit/insert mode.
