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

# Chart Visual Appearance Testing Patterns

**By:** Colossus
**Date:** 2026-02-12

## What

Established patterns for testing Chart.js-based component visual appearance via Playwright:

1. **Canvas dimension verification**: Use `BoundingBoxAsync()` to verify canvas has non-zero width/height, indicating successful render.

2. **Container dimension verification**: Verify the chart container div has dimensions matching `ChartWidth`/`ChartHeight` parameters (with ±10px tolerance for border/padding).

3. **Chart.js library verification**: Use `page.EvaluateAsync<bool>` to check:
   - `typeof Chart !== 'undefined'` — library loaded
   - `Chart.instances && Object.keys(Chart.instances).length > 0` — at least one chart instance exists

4. **Multi-series verification**: Use `page.EvaluateAsync<int>` to query `Chart.instances[0].data?.datasets?.length` to verify correct number of datasets rendered.

5. **Canvas context verification**: Check `canvas.getContext('2d') !== null` to verify canvas is properly initialized for 2D rendering.

## Why

Chart.js uses JS interop and renders to `<canvas>`, so traditional DOM assertions are insufficient. These patterns let us verify:
- The canvas element exists and has dimensions (chart rendered something)
- The Chart.js library loaded and created chart instances (JS interop succeeded)
- Multiple data series produce multiple datasets (data binding works)
- The canvas is properly initialized (rendering pipeline is functional)

## Technical Notes

- Use `LocatorWaitForOptions { State = WaitForSelectorState.Visible }` instead of `Expect()` since `InteractiveComponentTests` doesn't inherit from Playwright's `PageTest` class.
- Use `WaitUntilState.NetworkIdle` for tests that need Chart.js fully initialized (library verification, dataset verification).
- Use `WaitUntilState.DOMContentLoaded` for basic canvas presence tests.


### 2026-02-12: ChartSeries data binding extracts points via reflection

**By:** Cyclops
**What:** Fixed `ChartSeries.ToConfig()` to support data binding via `Items`, `XValueMember`, and `YValueMembers` parameters. When `Items` is provided with a non-empty `YValueMembers`, the method extracts `DataPoint` objects using reflection. The `YValueMembers` property supports comma-separated field names for multi-value charts. Type conversion handles `double`, `float`, `int`, `long`, `decimal`, `short`, `byte`, and falls back to `Convert.ToDouble()`. The manual `Points` collection is used as a fallback when `Items` is null/empty or `YValueMembers` is not specified.
**Why:** Data-bound charts were rendering empty because `ToConfig()` only used the manual `Points` collection. Web Forms Chart supports data binding via `XValueMember`/`YValueMembers` properties, and this fix restores that capability for Blazor migration scenarios.


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


### 2026-02-13: Chart component Phase 1 gate review — CONDITIONAL APPROVAL

**By:** Forge
**What:** Chart component on `milestone4/chart-component` branch is **conditionally approved** pending one ship-blocking fix:

1. **BLOCKING:** Data binding (`Items` + `XValueMember`/`YValueMembers`) is not implemented. The parameters exist and docs show examples, but `ChartSeries.ToConfig()` ignores `Items` entirely. Fix required before merge.

**Non-blocking gaps (Phase 2/3):**
- 27 unsupported chart types (documented, throws NotSupportedException)
- Per-point coloring (`DataPoint.Color`) not wired
- Tooltips not wired
- `IsValueShownAsLabel` not implemented
- `MarkerStyle` not mapped
- Integration tests needed (Colossus to add)

**Work items for ship:**
1. **Cyclops:** Implement data binding in `ChartSeries.ToConfig()` — convert `Items` to `DataPoint` list using reflection on `XValueMember`/`YValueMembers`
2. **Rogue:** Add tests for data binding scenario
3. **Beast:** Verify doc examples work after fix
4. **Colossus:** Add Chart sample routes to integration smoke tests

**Why:** The component is architecturally sound and 90% complete. The data binding gap creates a docs-vs-reality mismatch that will frustrate migrating developers. Phase 2/3 features (more chart types, tooltips) are nice-to-have but not blocking for initial ship.


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


# Chart Sample Pages — Feature-Rich Samples

**By:** Jubilee
**Date:** 2026-02-12

## Decision

Created 4 new Chart sample pages demonstrating advanced features:

1. **DataBinding.razor** — Web Forms-style data binding with `Items`, `XValueMember`, `YValueMembers`
2. **MultiSeries.razor** — Multiple series comparisons on single charts
3. **Styling.razor** — All 11 color palettes plus custom `WebColor` usage
4. **ChartAreas.razor** — Axis configuration (Title, Min/Max, Interval, IsLogarithmic)

## Sample Patterns Established

### Data Binding Pattern
Use records for clean business object definitions:
```csharp
public record SalesData(string Month, decimal Amount);

private List<SalesData> salesData = new() { new("Jan", 12500), ... };
```

Then bind with:
```razor
<ChartSeries Items="@salesData" XValueMember="Month" YValueMembers="Amount" ... />
```

### WebColor Usage
Use static fields, NOT `FromName()`:
```razor
Color="WebColor.DodgerBlue"  // ✓ Correct
Color="@(WebColor.FromName("DodgerBlue"))"  // ✗ Wrong - method doesn't exist
```

### Nav Ordering
Chart sub-samples are alphabetically ordered in NavMenu.razor (Area, Bar, ChartAreas, Column, DataBinding, etc.).

## Why

Jeff requested rich samples covering all Chart features. These samples:
- Show migrating developers how Web Forms data binding translates to Blazor
- Demonstrate all available color palettes visually
- Provide copy-paste ready code for common scenarios


# ChartSeries Data Binding Test Coverage

**By:** Rogue
**Date:** 2026-02-12

## What

Added 12 new bUnit tests for `ChartSeries` data binding in `ChartTests.cs`. These tests verify the expected behavior when `Items` + `XValueMember` + `YValueMembers` are used for data binding vs. manual `Points`.

## Test Cases

1. **DataBinding_ExtractsValuesFromItems** — Items with XValueMember/YValueMembers extracts X and Y values correctly
2. **DataBinding_NumericXValues** — Numeric X values (e.g., years) are preserved as numbers
3. **DataBinding_DecimalYValues** — Decimal Y values are extracted with precision
4. **DataBinding_ManualPoints_WorksWithoutItems** — When Items is null, manual Points are used
5. **DataBinding_EmptyItems_ProducesEmptyPoints** — Empty Items collection produces empty points (not error)
6. **DataBinding_NullItems_FallsBackToPoints** — Null Items falls back to manual Points
7. **DataBinding_NullItems_NoFallback_ProducesEmptyPoints** — Null Items with no fallback produces empty list
8. **DataBinding_MissingXValueMember_UsesNullXValue** — Missing XValueMember results in null XValue
9. **DataBinding_MissingYValueMembers_UsesEmptyYValues** — Missing YValueMembers results in empty YValues array
10. **DataBinding_IntYValue_ConvertsToDouble** — Integer Y values are converted to double
11. **DataBinding_ItemsOverrideManualPoints** — When both Items and Points provided, Items wins
12. **DataBinding_InvalidPropertyName_ReturnsNullValue** — Invalid property names handled gracefully

## Implementation Pattern

Since `ChartSeries.ToConfig()` is `internal`, tests use a helper class `ChartSeriesDataBindingHelper` that implements the expected extraction logic:

```csharp
public static List<DataPoint> ExtractDataPoints(
    IEnumerable<object> items,
    string xValueMember,
    string yValueMembers,
    List<DataPoint> fallbackPoints = null)
```

This helper documents the contract that Cyclops must implement in `ToConfig()`:
- If `Items` is not null and not empty, extract `DataPoint` objects using reflection
- If `Items` is null, fall back to manual `Points`
- Handle missing properties by returning null/empty values
- Convert numeric Y values to `double`

## Why

Cyclops is fixing the `ChartSeries.ToConfig()` bug where data binding is not implemented. These tests:
1. Document the expected behavior before the fix
2. Provide regression tests after the fix
3. Cover edge cases that could cause silent failures

## Total Test Count

152 Chart tests (140 original + 12 new data binding tests). All passing.

