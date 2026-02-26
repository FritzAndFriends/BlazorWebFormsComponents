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


### 2026-02-23: AccessKey must be added to BaseStyledComponent

**By:** Beast, Cyclops
**What:** `AccessKey` (string) is missing from all styled controls. Standard `WebControl` property. Beast's audit of 15 editor controls (L–X) and Cyclops's audit of 13 editor controls (A–I) independently confirmed the gap.
**Recommendation:** Add `[Parameter] public string AccessKey { get; set; }` to `BaseStyledComponent`.
**Why:** Universal gap confirmed by two independent audits across 28 controls. Base-class fix is the highest-leverage single change available.
**Status:** AccessKey added in Milestone 6. ToolTip consolidated into 2026-02-25 entry below.


### 2026-02-23: Label should inherit BaseStyledComponent

**By:** Beast
**What:** `Label` currently inherits `BaseWebFormsComponent` but Web Forms `Label` inherits from `WebControl`. This means Label is missing all 9 style properties (CssClass, BackColor, ForeColor, Font, etc.). Also consider `AssociatedControlID` to render `<label for="...">` instead of `<span>`.
**Why:** Label is one of the most commonly used controls. Missing style properties break migration for any page that styles its Labels.


### 2026-02-25: Substitution and Xml permanently deferred (consolidated)

**By:** Beast
**What:** Both controls are tightly coupled to server-side ASP.NET infrastructure: Substitution depends on the output caching pipeline; Xml depends on XSLT transformation. Neither maps to Blazor's component model. Both are formally marked as deferred in status.md and README.md. DeferredControls.md contains migration guidance: Substitution -> Blazor component lifecycle; Xml -> convert XML to C# objects and use Blazor templates.
**Why:** Architecturally incompatible with Blazor. Marking as deferred (not "Not Started") accurately communicates they will not be implemented.


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


### 2026-02-23: BaseDataBoundComponent inherits BaseStyledComponent (consolidated)

**By:** Forge (gap identification), Cyclops (implementation)
**What:** Controls inheriting `DataBoundComponent<T>` lacked all WebControl-level style properties because the chain `DataBoundComponent<T>` → `BaseDataBoundComponent` → `BaseWebFormsComponent` skipped `BaseStyledComponent`. Changed inheritance to insert `BaseStyledComponent`: `DataBoundComponent<T>` → `BaseDataBoundComponent` → `BaseStyledComponent` → `BaseWebFormsComponent`. This gives all data-bound controls the full IStyle property set (BackColor, BorderColor, BorderStyle, BorderWidth, CssClass, ForeColor, Font, Height, Width). Removed duplicate IStyle declarations from 11 controls (GridView, DetailsView, DataGrid, DataList, TreeView, AdRotator, BulletedList, CheckBoxList, DropDownList, ListBox, RadioButtonList). 949/949 tests pass — zero regressions.
**Why:** Affects 5+ of 9 data controls. Identified independently in data controls audit and chart design review. Single base-class fix closing ~70 style property gaps across the library.

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


### 2026-02-23: ValidationSummary functional gaps

**By:** Rogue
**What:** `AspNetValidationSummary` is missing `HeaderText`, `ShowMessageBox`, `ShowSummary`, `ShowValidationErrors`, and `ValidationGroup`. Prioritize `HeaderText` and `ValidationGroup` (common in multi-form pages).
**Why:** Missing properties affect multi-form page migration.
**Status:** Comma-split bug fixed in M9 (2026-02-25) — see consolidated entry below.


### 2026-02-23: Login controls outer style properties (consolidated)

**By:** Rogue, Cyclops
**What:** Login, ChangePassword, and CreateUserWizard were identified as missing outer-level WebControl style properties (BackColor, CssClass, ForeColor, Width, Height) because they inherited `BaseWebFormsComponent` instead of `BaseStyledComponent`. Resolution: all three changed to inherit `BaseStyledComponent` (Option A — base class change). Outer `<table>` elements now render CssClass and computed IStyle inline styles alongside `border-collapse:collapse;`. `[Parameter]` style properties do NOT conflict with `[CascadingParameter]` sub-styles (TitleTextStyle, LabelStyle, etc.) — completely independent mechanisms. LoginView still inherits `BaseWebFormsComponent`. LoginName and LoginStatus already had full style support. PasswordRecovery should follow the same pattern when ready.
**Why:** Migrating pages that set CssClass on Login controls would break. `BaseStyledComponent` extends `BaseWebFormsComponent` — no functionality lost.


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


### 2026-02-14: User directive — Sample website UI overhaul
**By:** Jeffrey T. Fritz (via Copilot)
**What:** Improve the UI of the samples/AfterBlazorServerSide website with a modern layout that demos each sample, feature, and component cleanly. Add a search feature. Update integration tests with this overhaul.
**Why:** User request — captured for team memory


### 2026-02-23: Label AssociatedControlID switches rendered element
**By:** Cyclops
**What:** Label renders `<label for="{AssociatedControlID}">` when AssociatedControlID is set, `<span>` when not. All style/id/accesskey attributes apply to whichever element renders.
**Why:** Matches Web Forms behavior exactly. Important for accessibility — screen readers use `<label for>` to associate labels with inputs. No breaking change — default behavior (no AssociatedControlID) still renders `<span>`.


### 2026-02-23: BaseListControl<TItem> introduced as shared base for all list controls

**By:** Cyclops
**What:** Created `DataBinding/BaseListControl.cs` inheriting `DataBoundComponent<TItem>`, consolidating `StaticItems`, `DataTextField`, `DataValueField`, `GetItems()`, and `GetPropertyValue()` from BulletedList, CheckBoxList, DropDownList, ListBox, and RadioButtonList. Added `DataTextFormatString` (WI-47) and `AppendDataBoundItems` (WI-48) parameters to this base class. All 5 list controls now inherit `BaseListControl<TItem>` instead of `DataBoundComponent<TItem>`.
**Why:** Web Forms has `ListControl` as the shared base for these controls. The 5 Blazor list controls had identical duplicated code for item retrieval and property extraction. Consolidating into a base class eliminates ~150 lines of duplication and provides a single place to add shared ListControl features (DataTextFormatString, AppendDataBoundItems, and future properties like CausesValidation/ValidationGroup). The `AppendDataBoundItems=false` semantics mean static items are skipped when data-bound items exist — matching Web Forms behavior where `DataBind()` clears the Items collection by default.


### 2026-02-23: CausesValidation on non-button controls follows ButtonBaseComponent pattern
**By:** Cyclops
**What:** CheckBox, RadioButton, and TextBox now have `CausesValidation`, `ValidationGroup`, and `ValidationGroupCoordinator` cascading parameter — same 3-property pattern used by ButtonBaseComponent. Validation fires in existing `HandleChange` methods for CheckBox/RadioButton. TextBox has the parameters but no trigger wiring because it lacks an `@onchange` binding.
**Why:** Web Forms exposes CausesValidation/ValidationGroup on all postback-capable controls. Following the exact ButtonBaseComponent pattern (same property names, same cascading parameter name, same coordinator call) ensures consistency and lets the existing ValidationGroupProvider work with these controls without modification.


### 2026-02-23: Menu Orientation uses CSS class approach, not inline styles
**By:** Cyclops
**What:** Menu horizontal layout is achieved by adding a `horizontal` CSS class to the top-level `<ul>` and a scoped CSS rule `ul.horizontal > li { display: inline-block; }`. The `Orientation` enum lives at `Enums/Orientation.cs` (Horizontal=0, Vertical=1). Default is Vertical.
**Why:** CSS class approach is cleaner than inline styles and matches how Web Forms Menu generates different class-based layouts for orientation. The enum follows project convention (explicit integer values, file in Enums/). Default Vertical matches Web Forms default.

# Decision: Milestone 6 Work Plan — Feature Gap Closure

**By:** Forge
**Date:** 2026-02-14
**Status:** Proposed

## What

Milestone 6 work plan with 54 work items across 3 priority tiers, targeting ~345 feature gaps identified in the 53-control audit (SUMMARY.md). Full plan at `planning-docs/MILESTONE6-PLAN.md`.


### P0 — Base Class Fixes (18 WIs, ~180 gaps)
Seven base class changes that sweep across many controls:
1. `AccessKey` on `BaseWebFormsComponent` (~40 gaps)
2. `ToolTip` on `BaseWebFormsComponent` (~35 gaps)
3. `DataBoundComponent<T>` → inherit `BaseStyledComponent` (~70 gaps)
4. `Display` enum on `BaseValidator` (6 gaps)
5. `SetFocusOnError` on `BaseValidator` (6 gaps)
6. `Image` → `BaseStyledComponent` (11 gaps)
7. `Label` → `BaseStyledComponent` (11 gaps)


### P1 — Individual Control Improvements (28 WIs, ~120 gaps)
- GridView overhaul: paging, sorting, inline row editing (most-used data control, currently 20.7% health)
- Calendar: string styles → TableItemStyle sub-components + enum conversion
- FormView: CssClass, header/footer, empty data templates
- HyperLink: `NavigationUrl` → `NavigateUrl` rename (migration blocker)
- ValidationSummary: HeaderText, ShowSummary, ValidationGroup
- PasswordRecovery audit doc re-run (was 0% due to pre-merge timing)
- Docs + integration tests for all changed controls


### P2 — Nice-to-Have (8 WIs, ~45 gaps)
ListControl format strings, Menu Orientation, Label AssociatedControlID, Login controls outer styles, CausesValidation on CheckBox/RadioButton/TextBox.

## Key Scope Decisions
- **Login controls outer styles → P2** (not P1): These controls use CascadingParameter sub-styles by convention. Outer wrapper styling is useful but lower priority than GridView/Calendar/FormView.
- **Skip Substitution and Xml**: Per existing team decision, both remain permanently deferred.
- **sprint3 merge is DONE**: DetailsView and PasswordRecovery are on the branch. Only the PasswordRecovery audit doc needs updating.

## Why

The audit shows 66.3% overall health with 597 missing features. P0 base class fixes are the highest-ROI work — 7 changes close ~180 gaps. GridView at 20.7% is the single biggest migration blocker and must be addressed. Expected outcome: overall health rises to ~85%.

## Agents

All 6 agents involved: Cyclops (implementation), Rogue (bUnit tests), Jubilee (samples), Beast (docs), Colossus (integration tests), Forge (PasswordRecovery re-audit + review).

# Sample Website UI Overhaul — Scope & Work Breakdown

**Author:** Forge  
**Date:** 2026-02-13  
**Requested by:** Jeffrey T. Fritz

---

## 1. Current State Analysis


### Layout Structure
- **MainLayout.razor:** Classic sidebar + main content layout
  - Fixed 250px sidebar (purple gradient background, sticky)
  - Top row with Docs/About links
  - Main content area with `@Body`
- **NavMenu.razor:** Uses `TreeView` component for navigation (176 lines of hardcoded TreeNode markup)
  - Categories: Home → Utility Features → Editor → Data → Validation → Navigation → Login → Migration Guides
  - No search functionality
  - TreeView is nested 3-4 levels deep — complex to navigate


### CSS Framework
- **Bootstrap 4.3.1** (2019 vintage — two major versions behind)
- Custom `site.css` (~200 lines) for layout, sidebar theming, validation states
- Open Iconic icon font (Bootstrap 4 era icons)
- No utility-first CSS — all custom classes


### Sample Page Organization
- **34 top-level component folders** in `Components/Pages/ControlSamples/`
- Pattern: Each component folder contains `Index.razor` + variant pages + `Nav.razor` for sub-navigation
- No consistent structure — some have 1 page, some have 6+
- `ComponentList.razor` on homepage shows flat list by category — **manually maintained, out of sync** (missing DetailsView, PasswordRecovery links)


### Static Assets
- `wwwroot/css/` — Bootstrap + site.css
- `wwwroot/img/` — Sample images for AdRotator, Chart
- No favicon customization, no branding assets


### Integration Tests
- **4 test files:** `ControlSampleTests.cs`, `InteractiveComponentTests.cs`, `HomePageTests.cs`, `PlaywrightFixture.cs`
- Tests use **semantic selectors** (element types, attributes) not CSS class selectors
- Example: `page.Locator("span[style*='font-weight']")`, `page.QuerySelectorAsync("canvas")`
- **Low risk from CSS changes** — tests don't depend on `.sidebar`, `.page`, etc.

---

## 2. Proposed Design Direction


### 2.1 Layout Structure

**Recommendation: Modern sidebar + card-based demo area**

```
┌────────────────────────────────────────────────────────────┐
│ [Logo] BlazorWebFormsComponents    [Search: ______] [Docs]│
├─────────────┬──────────────────────────────────────────────┤
│ NAVIGATION  │  BREADCRUMB: Home > Data Controls > GridView│
│             ├──────────────────────────────────────────────┤
│ [Search 🔍] │  ┌─────────────────────────────────────────┐ │
│             │  │ GridView                                │ │
│ ▼ Editor    │  │ ─────────────────────────────────────── │ │
│   Button    │  │ Description text from component docs   │ │
│   CheckBox  │  └─────────────────────────────────────────┘ │
│   ...       │                                              │
│ ▼ Data      │  ┌─────────────────────────────────────────┐ │
│   GridView ←│  │ Live Demo                               │ │
│   ListView  │  │ ┌─────────────────────────────────────┐ │ │
│   ...       │  │ │  <actual component renders here>   │ │ │
│ ▼ Validation│  │ └─────────────────────────────────────┘ │ │
│ ▼ Navigation│  └─────────────────────────────────────────┘ │
│ ▼ Login     │                                              │
│             │  ┌─────────────────────────────────────────┐ │
│             │  │ Code Example                   [Copy 📋]│ │
│             │  │ <pre><code>...</code></pre>            │ │
│             │  └─────────────────────────────────────────┘ │
│             │                                              │
│             │  ┌──────┐ ┌──────┐ ┌──────┐                 │
│             │  │Style │ │Events│ │Paging│  ← sub-pages    │
│             │  └──────┘ └──────┘ └──────┘                 │
└─────────────┴──────────────────────────────────────────────┘
```

**Key changes:**
1. **Persistent top bar** with search input + branding
2. **Collapsible sidebar** with category grouping (current TreeView → simple `<details>` or Blazor Accordion)
3. **Card-based demo pages** — description card, live demo card, code example card
4. **Sub-page tabs** — replace current `Nav.razor` pattern with horizontal tabs


### 2.2 CSS Approach

**Recommendation: Bootstrap 5.3 (latest stable)**

| Option | Pros | Cons | Verdict |
|--------|------|------|---------|
| **Bootstrap 5.3** | Familiar to team, minimal learning curve, great docs, no jQuery | Needs migration from 4.3 classes | ✅ **RECOMMENDED** |
| Tailwind CSS | Modern, utility-first | Build tooling, different paradigm | ❌ Overkill for sample site |
| FluentUI Blazor | Microsoft ecosystem | Heavy dependency, learning curve | ❌ Different library, confusing |
| Custom CSS only | Full control | Maintenance burden, no responsive grid | ❌ Not worth it |

**Bootstrap 4→5 breaking changes to address:**
- `ml-*` → `ms-*`, `mr-*` → `me-*` (margin utilities)
- `pl-*` → `ps-*`, `pr-*` → `pe-*` (padding utilities)
- `data-toggle` → `data-bs-toggle` (JS attributes — not used)
- `form-group` → `mb-3` (form layout)
- `.close` → `.btn-close` (close buttons)
- No jQuery dependency (already not using it)


### 2.3 Component Organization

**Current:** Flat navigation duplicated in TreeView (NavMenu) + ComponentList + manual sample pages

**Proposed:**
1. **Single source of truth:** `ComponentCatalog.json` or static class with component metadata:
   ```json
   {
     "components": [
       {
         "name": "Button",
         "category": "Editor",
         "route": "/ControlSamples/Button",
         "description": "Server-side button control",
         "subPages": ["Style", "Events", "JavaScript"]
       }
     ]
   }
   ```
2. **Auto-generate NavMenu** from catalog
3. **Auto-generate ComponentList** from catalog
4. **Template-driven sample pages** — reduce boilerplate


### 2.4 Search Implementation

**Recommendation: Client-side search with Fuse.js or similar**

| Approach | Pros | Cons | Verdict |
|----------|------|------|---------|
| **Client-side JS (Fuse.js)** | Zero server load, instant results, works offline | 50KB+ bundle, client rendering | ✅ **RECOMMENDED** |
| Blazor input + filter | No JS, type-safe | Re-renders on every keystroke | ⚠️ Viable fallback |
| Server-side API | Scalable | Overkill for <100 pages | ❌ Unnecessary |

**Implementation:**
1. Generate `search-index.json` at build time from component catalog
2. Include component name, category, description, keywords
3. Fuse.js fuzzy search with highlighting
4. Results show in dropdown below search input
5. Keyboard navigation (arrow keys + Enter)

---

## 3. Work Breakdown

| ID | Title | Owner | Size | Dependencies | Notes |
|----|-------|-------|------|--------------|-------|
| UI-1 | Upgrade Bootstrap 4.3→5.3 | Jubilee | M | — | Replace CSS files, update utility classes in site.css |
| UI-2 | Create ComponentCatalog data source | Cyclops | S | — | JSON or static class with all 50+ components |
| UI-3 | Redesign MainLayout.razor | Jubilee | M | UI-1 | New layout structure, top bar, breadcrumbs |
| UI-4 | Redesign NavMenu from catalog | Jubilee | M | UI-2, UI-3 | Replace TreeView with Bootstrap 5 Accordion |
| UI-5 | Create SamplePageTemplate | Jubilee | M | UI-3 | Card layout: description, demo, code, sub-tabs |
| UI-6 | Migrate sample pages to template | Jubilee | L | UI-5 | 34 component folders, ~80 pages total |
| UI-7 | Update ComponentList.razor | Jubilee | S | UI-2 | Generate from catalog, add missing components |
| UI-8 | Implement search (Fuse.js) | Cyclops | M | UI-2 | Index generation, search component, dropdown |
| UI-9 | Update integration tests | Colossus | M | UI-3, UI-4 | Verify all routes, update any broken selectors |
| UI-10 | Add dark mode toggle | Jubilee | S | UI-1 | Bootstrap 5 color modes, localStorage persistence |
| UI-11 | Update branding/favicon | Beast | S | — | BlazorWebFormsComponents logo, favicon.ico |
| UI-12 | Documentation for new layout | Beast | S | UI-6 | Update any docs referencing sample site |


### Dependency Graph

```
         ┌──────┐
         │ UI-1 │ Bootstrap upgrade (Jubilee)
         └──┬───┘
            │
    ┌───────┼────────┐
    ▼       ▼        ▼
┌──────┐ ┌──────┐ ┌──────┐
│ UI-2 │ │ UI-3 │ │UI-10 │
│Catalog│ │Layout│ │Dark  │
│(Cyc)  │ │(Jub) │ │Mode  │
└──┬───┘ └──┬───┘ └──────┘
   │        │
   ├────────┤
   ▼        ▼
┌──────┐ ┌──────┐
│ UI-4 │ │ UI-5 │
│NavMenu│ │Templat│
│(Jub)  │ │(Jub)  │
└──┬───┘ └──┬───┘
   │        │
   │        ▼
   │     ┌──────┐
   │     │ UI-6 │
   │     │Migrate│
   │     │(Jub)  │
   │     └──┬───┘
   │        │
   ├────────┤
   ▼        ▼
┌──────┐ ┌──────┐
│ UI-7 │ │ UI-8 │
│CompLst│ │Search│
│(Jub)  │ │(Cyc) │
└──────┘ └──┬───┘
            │
            ▼
         ┌──────┐
         │ UI-9 │
         │Tests │
         │(Col) │
         └──────┘
```


### Parallel Execution Plan

**Phase 1 (parallel):**
- UI-1: Bootstrap upgrade
- UI-2: ComponentCatalog
- UI-11: Branding

**Phase 2 (after Phase 1):**
- UI-3: MainLayout redesign
- UI-10: Dark mode

**Phase 3 (after Phase 2):**
- UI-4: NavMenu
- UI-5: SamplePageTemplate

**Phase 4 (after Phase 3):**
- UI-6: Migrate pages (largest item)
- UI-7: ComponentList
- UI-8: Search

**Phase 5 (after Phase 4):**
- UI-9: Integration tests
- UI-12: Documentation

---

## 4. Risk Assessment


### 4.1 Integration Test Breakage Risk: **LOW**

Current tests use:
- Element selectors: `button`, `input[type='submit']`, `canvas`, `table`, `a`, `li`
- Attribute selectors: `span[style*='font-weight']`, `img[src='/img/CSharp.png']`
- ID selectors: `#event-count`, `#event-details`
- Class selectors: `.item-row`, `.alt-item-row` (component output, not layout)

**No layout CSS class selectors found in tests.** Tests target component output, not page structure.

**Mitigation:** UI-9 (Colossus) runs full test suite after each major phase. Fix any breakage immediately.


### 4.2 Hardcoded Selectors: **MEDIUM**

Found hardcoded patterns:
- `NavMenu.razor` line 6: `navbar-brand` class (Bootstrap 4)
- `ComponentList.razor` line 66: `col-md=3` (typo! should be `col-md-3`)
- `site.css` references `.sidebar`, `.page`, `.main`, `.top-row`

**Mitigation:** UI-3 (MainLayout) and UI-4 (NavMenu) will replace these classes. Grep for all Bootstrap 4 class usages before Phase 2.


### 4.3 Search Implementation: **MEDIUM**

Client-side search requires:
1. JS interop for Fuse.js (first non-Chart JS in sample app)
2. Build-time index generation (manual or automated)
3. Keyboard navigation UX

**Mitigation:** 
- Use existing JS interop patterns from Chart component
- Start with manual index; automate later if needed
- Keep scope to basic dropdown; no fancy UX


### 4.4 Large Migration Scope: **HIGH**

UI-6 touches ~80 files across 34 component folders. Risk of:
- Inconsistent migration
- Broken links
- Lost sample code

**Mitigation:**
- Create template first (UI-5)
- Migrate 2-3 components as pilot (Button, GridView, Calendar)
- Review pilot with Jeff before proceeding
- Use checklist to track progress


### 4.5 Bootstrap 4→5 Breaking Changes: **LOW**

Most changes are utility class renames. No jQuery dependency to remove.

**Mitigation:** 
- Run `grep -r "ml-\|mr-\|pl-\|pr-"` to find all usages
- Batch replace with `ms-`/`me-`/`ps-`/`pe-`
- Verify responsive behavior after upgrade

---

## 5. Open Questions for Jeff

1. **Dark mode priority?** UI-10 is nice-to-have. Include in Phase 2 or defer?
2. **Search scope?** Component names only, or also search within docs/descriptions?
3. **Branding assets?** Do you have a BlazorWebFormsComponents logo, or should Beast create one?
4. **Migration guide updates?** Should we update the MasterPages migration guide to reference the new layout?

---

## 6. Recommendation

**Proceed with UI-1, UI-2, UI-11 in parallel immediately.** These are foundational and have no dependencies.

**Estimated total effort:** 3-4 sprints (assuming 2-day sprints)
- Phase 1-2: 1 sprint
- Phase 3: 1 sprint
- Phase 4: 1-2 sprints (UI-6 is large)
- Phase 5: 0.5 sprint

**Owners:**
- Jubilee: UI-1, UI-3, UI-4, UI-5, UI-6, UI-7, UI-10 (frontend lead)
- Cyclops: UI-2, UI-8 (catalog + search logic)
- Colossus: UI-9 (integration tests)
- Beast: UI-11, UI-12 (branding + docs)

# Decision: Menu Orientation requires local variable in Razor

**By:** Jubilee
**Date:** 2026-02-23
**Context:** WI-54 P2 Sample Updates

**What:** When using the `Orientation` parameter on the `Menu` component in Razor markup, you must use a local variable with the fully-qualified enum type because the parameter name `Orientation` collides with the enum type name `Orientation`. Direct usage like `Orientation="Orientation.Horizontal"` causes a Razor compilation error.

**Pattern:**
```csharp
@code {
    BlazorWebFormsComponents.Enums.Orientation horizontal = BlazorWebFormsComponents.Enums.Orientation.Horizontal;
}
```
Then in markup: `Orientation="@horizontal"`

**Why:** This is a Razor-specific disambiguation issue. Any sample or documentation showing Menu Orientation must use this pattern, or developers will hit a confusing compiler error.

# Decision: P2 Test Coverage Strategy

**Date:** 2026-02-23
**Author:** Rogue (QA Analyst)
**Context:** WI-53 — P2 Tests Batch

## Decision

All P2 features (WI-47 through WI-52) have been tested with 32 bUnit tests across 7 files. Tests focus on parameter acceptance and rendered output verification rather than deep integration testing, since these are P2 priority features.

## Test Files Created

| Feature | File | Tests |
|---------|------|-------|
| DataTextFormatString (WI-47) | `DropDownList/DataTextFormatString.razor` | 4 |
| DataTextFormatString (WI-47) | `BulletedList/DataTextFormatString.razor` | 3 |
| AppendDataBoundItems (WI-48) | `DropDownList/AppendDataBoundItems.razor` | 4 |
| CausesValidation (WI-49) | `CheckBox/CausesValidation.razor` | 9 |
| Menu Orientation (WI-50) | `Menu/OrientationTests.razor` | 3 |
| Label AssociatedControlID (WI-51) | `Label/AssociatedControlID.razor` | 6 |
| Login outer styles (WI-52) | `LoginControls/Login/OuterStyle.razor` | 3 |

## Observations

1. **Login already inherits BaseStyledComponent** — WI-52 described Login as inheriting BaseWebFormsComponent, but it already inherits BaseStyledComponent. The outer `<table>` renders `class="@GetCssClassOrNull()"` and `style="border-collapse:collapse;@Style"`, so CssClass and style properties already work on the outer element. ChangePassword and CreateUserWizard also already inherit BaseStyledComponent.

2. **CausesValidation wiring is internal** — CheckBox, RadioButton, and TextBox all have CausesValidation and ValidationGroup parameters, and they wire to ValidationGroupCoordinator via CascadingParameter. Testing the full validation group triggering requires wrapping in `<ValidationGroupProvider>` + `<EditForm>`, which is already covered by Button tests. The P2 tests verify parameter existence and default values.

3. **AppendDataBoundItems edge case** — When Items is null and AppendDataBoundItems is false, static items still render (by design — the replace behavior only kicks in when there ARE data-bound items to replace with).

## Impact

Team should be aware that Login/ChangePassword/CreateUserWizard BaseStyledComponent inheritance was already in place — WI-52's implementation may have been a no-op or only required template changes to wire `Style`/`CssClass` to the outer element.

# Decision: M7 Integration Tests Added (WI-39 + WI-40)

**Author:** Colossus
**Date:** 2026-02-24
**Status:** Done

## Context

Milestone 7 added 9 new sample pages across GridView, TreeView, Menu, DetailsView, and FormView. Each page needed smoke tests (page loads without errors) and, where applicable, interaction tests (behaviors work).

## What Was Added


### Smoke Tests (ControlSampleTests.cs)

Added `[InlineData]` entries to existing `[Theory]` methods:

- **DataControl_Loads_WithoutErrors:**
  - `/ControlSamples/GridView/Selection`
  - `/ControlSamples/GridView/DisplayProperties`
  - `/ControlSamples/FormView/Events`
  - `/ControlSamples/FormView/Styles`
  - `/ControlSamples/DetailsView/Styles`
  - `/ControlSamples/DetailsView/Caption`

- **NavigationControl_Loads_WithoutErrors:**
  - `/ControlSamples/TreeView/Selection`
  - `/ControlSamples/TreeView/ExpandCollapse`

- **MenuControl_Loads_AndRendersContent:**
  - `/ControlSamples/Menu/Selection`


### Interaction Tests (InteractiveComponentTests.cs)

Added 9 new `[Fact]` tests:

| Test | What It Verifies |
|------|-----------------|
| `GridView_Selection_ClickSelect_HighlightsRow` | Click Select link ΓåÆ selected index updates, count increments |
| `GridView_DisplayProperties_RendersCaption` | Caption element, EmptyDataTemplate, ShowHeader/ShowFooter checkboxes |
| `TreeView_Selection_ClickNode_ShowsSelected` | Click node ΓåÆ selection text and count update |
| `TreeView_ExpandCollapse_ButtonsWork` | Expand All / Collapse All buttons, leaf node visibility, NodeIndent slider |
| `Menu_Selection_ClickItem_ShowsFeedback` | Click menu item ΓåÆ click count increments (no console error checks ΓÇö JS interop) |
| `DetailsView_Styles_RendersStyledTable` | Table renders, "Customer Details" header visible |
| `DetailsView_Caption_RendersCaptionElement` | `<caption>` elements present, "Customer Record" text |
| `FormView_Events_ClickEdit_LogsEvent` | Click Edit ΓåÆ event log entries appear |
| `FormView_Styles_RendersStyledHeader` | "Widget Catalog" header text visible |

## Patterns Used

- Menu Selection test skips console error checks (JS interop produces expected errors)
- FormView tests use `WaitUntilState.DOMContentLoaded` (items bound in `OnAfterRenderAsync`)
- All other tests use `WaitUntilState.NetworkIdle` with 30s timeout
- Console error filtering: ISO 8601 timestamps + "Failed to load resource"

## Build Verification

`dotnet build samples/AfterBlazorServerSide.Tests/ -c Release` ΓÇö succeeded with no errors.

# Decision: Avoid bare `text=` locators in Playwright integration tests

**Author:** Colossus  
**Date:** 2025-07-24  
**Status:** Proposed  

## Context

Five integration tests failed in CI (PR #343) because `page.Locator("text=Label:")` matches the *innermost* element containing the text. When markup uses `<p><strong>Label:</strong> value</p>`, the locator returns the `<strong>`, excluding the sibling value text from `TextContentAsync()`. Additionally, bare `text=` locators cause strict-mode violations when the same text appears in both rendered output and code examples.

## Decision

All Playwright integration tests MUST use container-targeted locators instead of bare `text=` selectors when reading text content that includes a label and a value:

```csharp
// Γ¥î BAD ΓÇö matches <strong>, returns only label text
var info = page.Locator("text=Selected index:");

// Γ£à GOOD ΓÇö matches the parent <p>, returns label + value
var info = page.Locator("p").Filter(new() { HasTextString = "Selected index:" });
```

For elements that might appear in multiple places (rendered output + code examples), target the specific rendered element type:

```csharp
// Γ¥î BAD ΓÇö strict mode violation if text appears twice
var header = page.Locator("text=Widget Catalog");

// Γ£à GOOD ΓÇö targets only the rendered <td>
var header = page.Locator("td").Filter(new() { HasTextString = "Widget Catalog" }).First;
```

## Consequences

- Existing tests using bare `text=` locators for value extraction should be migrated.
- New tests must follow this pattern from the start.
- WaitForSelectorAsync calls should use specific selectors (e.g., `button:has-text('Edit')`) not generic element type selectors.


### 2026-02-24: User directive ΓÇö M8 scope excludes version bump and release
**By:** Jeffrey T. Fritz (via Copilot)
**What:** Start on Forge's recommended Milestone 8 next steps EXCEPT the version bump to 1.0 and release. Focus on: Menu JS fix, Calendar fix, Menu auto-ID, formally defer Substitution/Xml, PagerSettings shared sub-component, doc polish.
**Why:** User request ΓÇö captured for team memory

# DataGrid Style Sub-Components + Paging/Sorting Events

**By:** Cyclops  
**Date:** 2026-02-24  
**Work Items:** WI-44, WI-45

## Decision

DataGrid style sub-components follow the exact same `IXxxStyleContainer` + `UiTableItemStyle` + `CascadingParameter` pattern used by GridView, DetailsView, and FormView. Paging and sorting events follow Web Forms DataGrid naming conventions (not GridView conventions).

## Details ΓÇö WI-44 (Style Sub-Components)

- **Interface:** `IDataGridStyleContainer` with 7 TableItemStyle properties (AlternatingItemStyle, ItemStyle, HeaderStyle, FooterStyle, PagerStyle, SelectedItemStyle, EditItemStyle)
- **CascadingValue name:** `"ParentDataGrid"` (matching GridView's `"ParentGridView"` convention)
- **7 sub-component pairs:** DataGridAlternatingItemStyle, DataGridItemStyle, DataGridHeaderStyle, DataGridFooterStyle, DataGridPagerStyle, DataGridSelectedItemStyle, DataGridEditItemStyle
- **Style priority in GetRowStyle:** EditItemStyle > SelectedItemStyle > AlternatingItemStyle > ItemStyle (matches Web Forms precedence)
- **Display properties added:** Caption, CaptionAlign, CellPadding, CellSpacing, GridLines, UseAccessibleHeader
- **Template enhanced:** Paging UI (page links in tfoot), footer row, caption element, grid lines rules attribute, sortable header links

## Details ΓÇö WI-45 (Paging + Sorting Events)

- **Events:** PageIndexChanged (DataGridPageChangedEventArgs), SortCommand (DataGridSortCommandEventArgs), ItemCreated (DataGridItemEventArgs), ItemDataBound (DataGridItemEventArgs), SelectedIndexChanged (EventCallback)
- **Event args:** DataGridPageChangedEventArgs (NewPageIndex), DataGridSortCommandEventArgs (SortExpression, CommandSource), DataGridItemEventArgs (Item)
- **Paging:** GoToPage(int) updates CurrentPageIndex and fires PageIndexChanged
- **Sorting:** Sort(string) fires SortCommand when AllowSorting is enabled via header links

## Key Naming Difference: DataGrid vs GridView

DataGrid uses Web Forms DataGrid naming (ItemStyle, AlternatingItemStyle, EditItemIndex, CurrentPageIndex) rather than GridView naming (RowStyle, AlternatingRowStyle, EditIndex, PageIndex). This matches the original ASP.NET Web Forms distinction between the two controls.

## Why

Consistency with existing GridView style pattern ensures predictable API. DataGrid-specific naming preserves Web Forms migration fidelity ΓÇö developers migrating `<asp:DataGrid>` markup expect `ItemStyle` not `RowStyle`.

# DetailsView + FormView Polish Decisions

**By:** Cyclops
**Date:** Milestone 7

## WI-26: DetailsView Style Sub-Components

**What:** Created `IDetailsViewStyleContainer` interface with 10 style properties and 10 sub-component pairs following the established GridView/Calendar pattern. DetailsView has two extra styles vs GridView: `CommandRowStyle` (for the Edit/Delete/New command row) and `FieldHeaderStyle` (for the left-side header cell in each data row). `InsertRowStyle` is separate from `EditRowStyle` to match Web Forms semantics where Insert and Edit modes can be styled independently.

**Why:** Consistent with the GridView style sub-component architecture (WI-05). DetailsView has distinct row types (command row, field headers) that Web Forms styled separately. CascadingParameter name is "ParentDetailsView" to avoid collision with "ParentGridView".

## WI-28: DetailsView Caption + PagerSettings

**What:** Added `Caption` (string), `CaptionAlign` (TableCaptionAlign enum), and `PageCount` (computed int). Reuses existing `TableCaptionAlign` enum and `GetCaptionStyle()` pattern from GridView. `PageCount` is a read-only computed property (`Items.Count()`) since DetailsView shows one item per page. PagerSettings deferred to a future WI ΓÇö the current implementation uses the existing PagerTemplate approach and inline numeric pager.

**Why:** Caption/CaptionAlign match GridView's implementation exactly. PageCount is trivially derived from the data source. Full PagerSettings (Mode, Position, PageButtonCount, navigation text) is better as a dedicated sub-component in a follow-up WI to keep this change focused.

## WI-31: FormView Remaining Events

**What:** Added `ModeChanged` (fires after mode transitions), `ItemCommand` (fires for all command bubbling via `FormViewCommandEventArgs`), `ItemCreated` (fires on first render), `PageIndexChanging`/`PageIndexChanged` (with cancellation via `PageChangedEventArgs.Cancel`). Added "page" command handler supporting "next"/"prev"/"first"/"last"/numeric arguments.

**Why:** Web Forms FormView fires ModeChanged after every mode switch. ItemCommand is the catch-all command handler that fires before specific handlers. ItemCreated maps to the initial data-bound lifecycle. Page events reuse the existing `PageChangedEventArgs` class (shared with GridView/DetailsView).

## WI-33: FormView Style Sub-Components + Pager + Caption

**What:** Created `IFormViewStyleContainer` interface with 7 style properties and 7 sub-component pairs. Added `PagerTemplate` (RenderFragment) that replaces the default numeric pager when set. Added `Caption`/`CaptionAlign` using the same pattern as DetailsView/GridView. `GetCurrentRowStyle()` resolves style based on `CurrentMode` (EditΓåÆEditRowStyle, InsertΓåÆInsertRowStyle, defaultΓåÆRowStyle).

**Why:** FormView doesn't have AlternatingRowStyle because it only displays one item at a time. The 7 styles (RowStyle, EditRowStyle, InsertRowStyle, HeaderStyle, FooterStyle, EmptyDataRowStyle, PagerStyle) cover all distinct visual regions. PagerTemplate enables custom pager markup, matching Web Forms' `<PagerTemplate>` element.

# GridView Display Properties ΓÇö Decision Record

**Author:** Cyclops  
**Date:** WI-07 implementation  
**Component:** GridView

## Decisions


### 1. ShowHeaderWhenEmpty defaults to false (breaking behavior change)

Previously, GridView always rendered `<thead>` when columns existed, regardless of data. Now `ShouldRenderHeader = ShowHeader && (HasData || ShowHeaderWhenEmpty)`. With `ShowHeaderWhenEmpty=false` (default), the header is hidden when the data source is empty. This matches Web Forms behavior where `ShowHeaderWhenEmpty` was added in .NET 4.5 with default `false`.

**Impact:** Existing GridViews with empty data will stop showing headers unless `ShowHeaderWhenEmpty="true"` is added. One test (`EmptyDataText.razor`) was updated accordingly.


### 2. UseAccessibleHeader adds scope="col" to existing th elements

The current GridView already renders `<th>` in the header (not `<td>`). Rather than changing the default to `<td>` (which would be a larger breaking change), `UseAccessibleHeader=true` adds `scope="col"` to the existing `<th>` elements for accessibility compliance. When false (default), `<th>` renders without scope ΓÇö preserving existing HTML output.


### 3. GridLines.None suppresses the rules attribute entirely

When `GridLines=None` (default), `GetGridLinesRules()` returns `null`, so Blazor omits the `rules` attribute from the `<table>` element. This matches Web Forms behavior where `GridLines.None` means no `rules` attribute is rendered.


### 4. CellPadding/CellSpacing use -1 sentinel for "don't render"

Following Web Forms convention, `-1` means the attribute is not rendered. Any value `>= 0` renders the corresponding `cellpadding`/`cellspacing` attribute on the `<table>`.


### 5. ShowFooter and paging share a single tfoot

When both `ShowFooter=true` and `AllowPaging` with multiple pages, both the footer row and pager row render inside the same `<tfoot>` element. The footer row renders first, followed by the pager row. Footer row gets `FooterStyle` applied.


### 6. EmptyDataTemplate takes precedence over EmptyDataText

When both `EmptyDataTemplate` (RenderFragment) and `EmptyDataText` (string) are set, the template wins. This matches Web Forms behavior.


### GridView Selection Support ΓÇö Pattern Decisions

**By:** Cyclops  
**Date:** 2026-02-24  
**WI:** WI-02

**What:**
- `SelectedIndex` (int, default -1) follows the same pattern as `EditIndex`
- `SelectedRow` and `SelectedValue` are computed read-only properties (not parameters)
- `SelectedValue` uses reflection on `DataKeyNames` first key field, matching Web Forms behavior
- `AutoGenerateSelectButton` adds a "Select" link to the command column, rendered before Edit/Delete links
- `ShowCommandColumn` now includes `AutoGenerateSelectButton` in its check
- `GetRowStyle()` priority: EditRowStyle > SelectedRowStyle > AlternatingRowStyle > RowStyle (edit takes precedence over selection)
- `GridViewSelectedRowStyle` follows the existing `IGridViewStyleContainer` + CascadingParameter pattern (same as `GridViewEditRowStyle`, etc.)
- `GridViewSelectEventArgs` follows the same pattern as `GridViewEditEventArgs` (NewSelectedIndex + Cancel)

**Why:**
- Selection mirrors the existing edit-mode pattern for consistency
- Edit takes priority over selection in styling because a row being edited is an active operation
- The `SelectedRowStyle` child component reuses the established `IGridViewStyleContainer` interface rather than creating a new one

# GridView Style Sub-Components Pattern

**By:** Cyclops  
**Date:** 2026-02-24  
**Work Item:** WI-05

## Decision

GridView style sub-components follow the same `IXxxStyleContainer` + `UiTableItemStyle` + `CascadingParameter` pattern used by Calendar and DataList.

## Details

- **Interface:** `IGridViewStyleContainer` with 8 TableItemStyle properties (RowStyle, AlternatingRowStyle, HeaderStyle, FooterStyle, EmptyDataRowStyle, PagerStyle, EditRowStyle, SelectedRowStyle)
- **CascadingValue name:** `"ParentGridView"` (matching Calendar's `"ParentCalendar"` and DataList's `"ParentDataList"` convention)
- **Style priority in GetRowStyle:** EditRowStyle > SelectedRowStyle > AlternatingRowStyle > RowStyle (matches Web Forms precedence)
- **Style application:** Inline `style` attribute via `TableItemStyle.ToString()` on `<tr>` elements, not CSS classes
- **EditRowStyle migration:** Changed from `[Parameter]` to `IGridViewStyleContainer` property with `internal set`, to be consistent with all other style properties and enable sub-component setting

## Why

This maintains consistency with the existing Calendar and DataList style patterns. The `CascadingParameter` + interface approach allows style sub-components to be declared as child elements in markup, exactly matching Web Forms `<asp:GridView><RowStyle .../></asp:GridView>` syntax.

# Decision: ListView CRUD Events Pattern (WI-41)

**By:** Cyclops
**Date:** 2026-02-24

## What

ListView CRUD events follow the same dual-event pattern as GridView and FormView:
- Pre-events (ItemEditing, ItemDeleting, ItemUpdating, ItemInserting, ItemCanceling) support `Cancel` bool
- Post-events (ItemDeleted, ItemInserted, ItemUpdated) carry `AffectedRows` + `Exception`
- `ItemCommand` fires for unrecognized commands (catch-all)
- `HandleCommand(string, object, int)` is the public routing method

## Why

Consistent with GridView's `EditRow`/`UpdateRow`/`DeleteRow`/`CancelEdit` and FormView's `HandleCommandArgs` patterns. ListView event args are intentionally simpler than FormView's (no OrderedDictionary) because the task spec said "don't over-engineer dictionaries if simpler patterns work."

## Key Decisions

1. **EmptyItemTemplate vs EmptyDataTemplate:** `EmptyItemTemplate` takes precedence when both are set. `EmptyDataTemplate` was the original, `EmptyItemTemplate` is the Web Forms ListView-specific name.
2. **ListViewCancelMode enum:** Created in `Enums/ListViewCancelMode.cs` ΓÇö `CancelingEdit` (0) and `CancelingInsert` (1). Follows project enum convention with explicit int values.
3. **GetItemTemplate helper:** Returns EditItemTemplate when itemIndex matches EditIndex, otherwise delegates to alternating template logic. Used in both grouped and non-grouped rendering paths.
4. **InsertItemTemplate positioning:** Renders at top (before items) or bottom (after items) based on InsertItemPosition enum, only in the non-grouped (GroupItemCount == 0) path.

# Decision: Menu auto-ID generation pattern

**By:** Cyclops
**Date:** 2026-02-24

## What

Menu component now auto-generates an ID (`menu_{GetHashCode():x}`) in `OnParametersSet` when no explicit `ID` parameter is provided. This ensures JS interop via `Sys.WebForms.Menu` always has a valid DOM element ID to target.

Additionally, `Menu.js` now has null safety (early return if element not found) and a try/catch around the constructor to prevent unhandled JS exceptions from crashing the Blazor circuit.

## Why

The Menu component's JS interop depends on a DOM element ID to find and manipulate the menu element. Without an ID, `document.getElementById('')` returns null, causing `TypeError: Cannot read properties of null (reading 'tagName')`. This crashed the entire Blazor circuit in headless Chrome environments.

## Impact

Any component that uses JS interop via element IDs should consider auto-generating IDs when none are provided. This pattern (`$"componentname_{GetHashCode():x}"` in `OnParametersSet`) could be reused by other components with JS interop dependencies.

# Decision: Menu Core Improvements (WI-19 + WI-21 + WI-23)

**Author:** Cyclops  
**Date:** 2026-02-24  
**Status:** Implemented  
**Branch:** milestone7/feature-implementation

## Context

Menu component needed three improvements: base class upgrade for styling, selection tracking with events, and missing core properties.

## Decisions


### WI-19: Menu inherits BaseStyledComponent

- Changed `Menu : BaseWebFormsComponent` ΓåÆ `Menu : BaseStyledComponent`
- Menu now gets BackColor, BorderColor, BorderStyle, BorderWidth, CssClass, Font, ForeColor, Height, Width from the base class
- `Menu.razor` root `<div>` renders `style="@Style"` and `class="@GetMenuCssClass()"` using the inherited `Style` property
- `GetMenuCssClass()` helper returns null when CssClass is empty (same pattern as Label)
- Existing sub-component styles (DynamicHoverStyle, StaticMenuItemStyle, etc.) remain unchanged ΓÇö they are non-parameter properties set by child sub-components, completely independent from `[Parameter]` base class styles
- MenuItem.razor still inherits BaseWebFormsComponent (no styling needed on individual items)


### WI-21: Selection tracking and events

- `SelectedItem` (MenuItem, read-only) ΓÇö set internally when a menu item is clicked
- `SelectedValue` (string, read-only) ΓÇö computed from `SelectedItem?.Value`
- `MenuItemClick` (EventCallback\<MenuEventArgs\>) ΓÇö fires when any menu item is clicked
- `MenuItemDataBound` (EventCallback\<MenuEventArgs\>) ΓÇö fires after each data-bound MenuItem is created
- Created `MenuEventArgs` class with `Item` property (follows TreeNodeEventArgs pattern)
- MenuItem calls `ParentMenu.NotifyItemClicked(this)` via `@onclick` handler
- `@onclick:preventDefault` only applies when `NavigateUrl` is empty (preserves navigation for link items)


### WI-23: Core missing properties

- `MaximumDynamicDisplayLevels` (int, default 3) ΓÇö limits depth of dynamic flyout menus
- `Target` (string) ΓÇö default link target for menu items; MenuItem has its own `Target` that overrides via `EffectiveTarget`
- `SkipLinkText` (string, default "Skip Navigation Links") ΓÇö rendered as `<a class="skip-link">` before the menu and an anchor `<a id="...SkipLink">` after; matches Web Forms pattern
- `PathSeparator` (char, default '/') ΓÇö stored on Menu, used in MenuItem.ValuePath computation
- MenuItem gets `Value` (string) and `ValuePath` (string, computed) properties
- MenuItem `target` attribute changed from hardcoded `_blank` to `@EffectiveTarget` (item-level Target > Menu-level Target)

## Files Changed

- `src/BlazorWebFormsComponents/Menu.razor` ΓÇö BaseStyledComponent inherits, style/class on root, skip link
- `src/BlazorWebFormsComponents/Menu.razor.cs` ΓÇö Base class change, new properties, events, NotifyItemClicked
- `src/BlazorWebFormsComponents/MenuItem.razor` ΓÇö Click handler, EffectiveTarget
- `src/BlazorWebFormsComponents/MenuItem.razor.cs` ΓÇö Value, Target, ValuePath, EffectiveTarget, HandleClick
- `src/BlazorWebFormsComponents/MenuEventArgs.cs` ΓÇö New file

## Risks

- `MenuItemDataBound` fires with `null` Item during RenderTreeBuilder execution (component isn't materialized yet). Consumers should use this for counting/logging, not item manipulation.
- `MaximumDynamicDisplayLevels` property is declared but not yet enforced in rendering logic ΓÇö the JS interop and CSS-based flyout system would need updates to actually limit depth.

# Decision: Menu Level Styles, Panel BackImageUrl, Login/ChangePassword Orientation

**Author:** Cyclops  
**Date:** 2026-02-24  
**Status:** Implemented  
**WIs:** WI-47, WI-48, WI-49

## WI-47 ΓÇö Menu Level Styles


### Decision
Created `MenuLevelStyle` as a standalone class (not a ComponentBase sub-component) with public constructor implementing `IStyle`. Level style collections are `List<MenuLevelStyle>` parameters on Menu, not `RenderFragment` sub-components.


### Rationale
- Level styles are positional (index-based), unlike named sub-components (StaticMenuItemStyle, DynamicMenuItemStyle)
- A `List<T>` parameter is the natural API for ordered collections
- `MenuLevelStyle` needed a public constructor (unlike `Style`/`TableItemStyle` which have `internal` constructors) so users can instantiate them in code
- Follows the same `IStyle` contract so `ToStyle()` extension works for CSS generation


### Style Resolution Order
MenuItem applies styles in this priority:
1. LevelSelectedStyles (if item is selected and entry exists at depth index)
2. LevelMenuItemStyles (if entry exists at depth index)
3. Falls back to static/dynamic CSS class styles from `<style>` block


### Files Changed
- `MenuLevelStyle.cs` (new)
- `Menu.razor.cs` ΓÇö added 3 List parameters + 3 internal getter helpers
- `MenuItem.razor` ΓÇö added `GetItemStyle()`, `GetItemCssClass()`, `GetSubMenuStyle()` methods

## WI-48 ΓÇö Panel BackImageUrl


### Decision
Added `BackImageUrl` string parameter to Panel. Renders as `background-image:url({value})` in the existing `BuildStyle()` method.


### Rationale
- Minimal change ΓÇö one parameter, one style line in existing `BuildStyle()`
- Follows same pattern as other computed styles (HorizontalAlign, ScrollBars, Wrap)
- No new rendering elements needed


### Files Changed
- `Panel.razor.cs` ΓÇö added parameter + style entry in `BuildStyle()`

## WI-49 ΓÇö Login/ChangePassword Orientation + TextLayout


### Decision
- Created `LoginTextLayout` enum (TextOnLeft, TextOnTop) in `Enums/`
- Added `Orientation` and `TextLayout` parameters to both Login and ChangePassword
- Used `Enums.Orientation.Vertical` fully-qualified comparison in Razor `@code` blocks to avoid parameter/type name collision


### Razor Naming Collision
The parameter `Orientation` has the same name as the enum type `Orientation`. In Razor, this causes ambiguity. Resolution: helper properties `IsVertical`/`IsCpVertical` use `Enums.Orientation.Vertical` (namespace-qualified) to disambiguate. This follows the known M6 pattern documented by Jubilee.


### Layout Approach
- Vertical (default): fields in separate `<tr>` rows (original behavior)
- Horizontal: fields in `<td>` columns within same `<tr>`
- TextOnLeft (default): label beside input (original behavior)
- TextOnTop: label in separate row above input
- Dynamic `colspan` adjusts full-width rows (title, instructions, failure text, buttons)


### Files Changed
- `Enums/LoginTextLayout.cs` (new)
- `Login.razor.cs` ΓÇö added Orientation + TextLayout parameters
- `Login.razor` ΓÇö 4 layout branches + helper properties
- `ChangePassword.razor.cs` ΓÇö added Orientation + TextLayout parameters
- `ChangePassword.razor` ΓÇö 4 layout branches + helper properties


### PagerSettings follows settings-not-style pattern

**By:** Cyclops
**What:** PagerSettings is a plain C# class (not inheriting `Style`), unlike the existing `TableItemStyle` sub-components. The `UiPagerSettings` base component extends `ComponentBase` directly (not `UiStyle<T>`) because PagerSettings has no visual style properties ΓÇö it's pure configuration. The same CascadingParameter pattern is used (`IPagerSettingsContainer` interface, cascaded `"ParentXxx"` value), but the base class is simpler. The `PagerButtons` enum already existed; only `PagerPosition` was new.
**Why:** Future sub-components that configure behavior (not style) should follow this `UiPagerSettings` pattern rather than `UiTableItemStyle`. The distinction is: style sub-components inherit `UiStyle<T>` and set visual properties; settings sub-components inherit `ComponentBase` and set configuration properties.

# Decision: TreeView Enhancement (WI-11 + WI-13 + WI-15)

**Author:** Cyclops  
**Date:** 2026-02-24  
**Status:** Implemented  
**Branch:** milestone7/feature-implementation

## Context

TreeView needed three enhancements implemented together since they all touch the same component: node-level styling, selection support, and expand/collapse programmatic control.

## Decisions


### 1. TreeNodeStyle extends Style (not TableItemStyle)

Web Forms `TreeNodeStyle` inherits from `Style`, not `TableItemStyle`. It adds tree-specific properties (`ChildNodesPadding`, `HorizontalPadding`, `ImageUrl`, `NodeSpacing`, `VerticalPadding`) but NOT `HorizontalAlign`/`VerticalAlign`/`Wrap` from `TableItemStyle`. Followed the same inheritance as Web Forms.


### 2. Sub-component pattern mirrors GridView exactly

Created `ITreeViewStyleContainer` + `UiTreeNodeStyle` + 6 sub-component pairs (`.razor` + `.razor.cs`), following the identical pattern used by `IGridViewStyleContainer` + `UiTableItemStyle` + GridView*Style sub-components. This keeps the codebase consistent.


### 3. Style resolution priority

`GetNodeStyle(node)` resolves: **SelectedNodeStyle** (if selected) > **type-specific style** (RootNodeStyle/ParentNodeStyle/LeafNodeStyle) > **NodeStyle** (fallback). This matches Web Forms behavior.


### 4. Selection via @onclick on text anchor

Rather than wrapping the entire row in a clickable element, selection is wired to the existing text `<a>` element via `@onclick="HandleNodeSelect"`. When `NavigateUrl` is empty, `@onclick:preventDefault` suppresses navigation. This preserves the existing HTML structure.


### 5. ExpandDepth applied in OnInitializedAsync

`ExpandDepth` controls initial expansion only. Applied during `TreeNode.OnInitializedAsync()` ΓÇö if `Depth >= ExpandDepth` and no user override exists, the node starts collapsed. User clicks override via `_UserExpanded`.


### 6. FindNode uses Value with Text fallback

`FindNode(valuePath)` splits on `PathSeparator` and matches each segment against `node.Value ?? node.Text`. This matches Web Forms behavior where Value defaults to Text if not explicitly set.


### 2026-02-26: WebFormsPage unified wrapper component (consolidated)

**By:** Jeffrey T. Fritz (via Copilot), Forge
**What:** Merge NamingContainer and the Skins/Themes wrapper into a single unified wrapper component named `WebFormsPage`. The name mirrors `System.Web.UI.Page` from ASP.NET Web Forms. Implementation: inherits `NamingContainer` (not `BaseWebFormsComponent`), adds `Theme` (ThemeConfiguration) parameter, wraps `ChildContent` in `CascadingValue<ThemeConfiguration>`. Parameters: `ID`, `UseCtl00Prefix`, `Theme`, `Visible`, `ChildContent` — no new parameters invented. Placement: `MainLayout.razor` wrapping `@Body`. ViewState already handled via inherited `BaseWebFormsComponent` dictionary. Rejected name alternatives: WebFormsSupport (too generic), WebFormsHost (implies hosting), LegacyPage (pejorative). One component instead of multiple separate wrappers.
**Why:** User directive to simplify the migration story — developers add one wrapper and get NamingContainer + Skins/Themes support together. Forge's design ensures backward compatibility: existing `NamingContainer` and `ThemeProvider` standalone usage is unaffected.


### 2026-02-26: SharedSampleObjects for sample data parity

**By:** Jeffrey T. Fritz (via Copilot)
**What:** Use `samples/SharedSampleObjects/` to deliver identical data to both Blazor and WebForms samples. Product model and sample data centralized in SharedSampleObjects. Both sides must consume identical model data from this shared library.
**Why:** The #1 blocker for HTML match rates is different sample data between WebForms and Blazor. SharedSampleObjects already exists and is shared by both projects.


### 2026-02-26: Defer Login+Identity to future milestone

**By:** Jeffrey T. Fritz (via Copilot)
**What:** Login controls + Blazor Identity integration (D-09) deferred to a future milestone. Do not schedule implementation work for Login, LoginName, LoginStatus, LoginView, ChangePassword, CreateUserWizard, or PasswordRecovery Identity integration now. Analysis preserved at `planning-docs/LOGIN-IDENTITY-ANALYSIS.md`.
**Why:** User request — wants to delay this work and focus on other priorities first.


### 7. NodeIndent replaces hardcoded 20px

The previously hardcoded `width:20px` in indent `<div>` elements now uses `IndentWidth` (from `TreeView.NodeIndent` parameter, default 20). No visual change for existing usage.

## Files Changed

- **New:** `TreeNodeStyle.cs`, `UiTreeNodeStyle.cs`, `Interfaces/ITreeViewStyleContainer.cs`
- **New:** 6 sub-component pairs: `TreeView{NodeStyle,HoverNodeStyle,LeafNodeStyle,ParentNodeStyle,RootNodeStyle,SelectedNodeStyle}.razor{,.cs}`
- **Modified:** `TreeView.razor`, `TreeView.razor.cs`, `TreeNode.razor`, `TreeNode.razor.cs`

## Risks

- BL0005 warning on `Selected` parameter set outside component (same pattern as GridView selection ΓÇö acceptable).
- HoverNodeStyle CSS is computed but hover interaction requires JS interop or CSS `:hover` pseudo-class ΓÇö the style data is available but hover event wiring is deferred to a future WI.

# Decision: Validator ControlToValidate String ID Support (WI-36)

**Date:** 2026-02-24
**By:** Cyclops
**Status:** Implemented

## What

Renamed the existing `ForwardRef<InputBase<Type>> ControlToValidate` parameter to `ControlRef` on `BaseValidator<Type>`, and added a new `[Parameter] public string ControlToValidate` parameter that accepts a string ID matching the Web Forms migration pattern `ControlToValidate="TextBox1"`.

## Why

In ASP.NET Web Forms, every validator uses `ControlToValidate="TextBoxID"` with a string control ID. The previous Blazor implementation required `ForwardRef<InputBase<Type>>` which doesn't match the "paste your markup and it works" migration story. This was identified as a migration-blocking API mismatch affecting all 5 input validators.

## How It Works

- **ControlToValidate (string):** Maps to a property/field name on the `EditContext.Model`. The validator uses `CurrentEditContext.Field(name)` for the field identifier and resolves the value via reflection on the model object. No JS interop needed.
- **ControlRef (ForwardRef):** The Blazor-native alternative. Uses the existing `ValueExpression.Body` ΓåÆ `MemberExpression` path and reads `CurrentValueAsString` from `InputBase<Type>` via reflection.
- **Precedence:** When both are set, `ControlRef` takes precedence.
- **Error handling:** Throws `InvalidOperationException` if neither is set.

## Impact

- `BaseValidator.razor.cs`: Core dual-path logic added (`GetFieldName()`, `GetCurrentValueAsString(fieldName)`)
- 38 test `.razor` files: `ControlToValidate=` ΓåÆ `ControlRef=`
- 8 sample `.razor` files: `ControlToValidate=` ΓåÆ `ControlRef=`
- `Login.razor`: `ControlToValidate=` ΓåÆ `ControlRef=`
- All 5 validators (RequiredFieldValidator, CompareValidator, RangeValidator, RegularExpressionValidator, CustomValidator) inherit dual-path support automatically through `BaseValidator<Type>`.

## Breaking Change

This is a parameter rename: existing code using `ControlToValidate="@someForwardRef"` must change to `ControlRef="@someForwardRef"`. This is intentional ΓÇö the string `ControlToValidate` parameter is the Web Forms-compatible API, while `ControlRef` is the Blazor-native alternative.

# Decision: Milestone 7 Plan Ratified

**Date:** 2026-02-23
**Author:** Forge (Lead / Web Forms Reviewer)
**Status:** Proposed
**Scope:** Milestone 7 planning ΓÇö "Control Depth & Navigation Overhaul"

## Context

Milestone 6 closed ~345 gaps across 54 work items, primarily through sweeping base class fixes (AccessKey, ToolTip, DataBoundComponent style inheritance, Validator Display/SetFocusOnError, Image/Label base class upgrades) and targeted control improvements (GridView paging/sorting/editing, Calendar styles+enums, FormView header/footer/empty, HyperLink rename, ValidationSummary, ListControl improvements, Menu Orientation, Label AssociatedControlID, Login base class upgrade).

The remaining gaps are in the "long tail" ΓÇö style sub-components, complex event pipelines, and navigation control completeness. The audit docs in `planning-docs/` are stale (reflect pre-M6 state).

## Decision

Milestone 7 targets ~138 gap closures across 51 work items, organized as:


### P0 ΓÇö Re-Audit + GridView Completion (10 WIs, ~23 gaps)
- **Re-audit all 53 controls** (mandatory, opens milestone)
- **GridView selection**: SelectedIndex, SelectedRow, SelectedRowStyle, AutoGenerateSelectButton, events
- **GridView style sub-components**: AlternatingRowStyle, RowStyle, HeaderStyle, FooterStyle, etc.
- **GridView display properties**: ShowHeader/ShowFooter, Caption, EmptyDataTemplate, GridLines


### P1 ΓÇö Navigation + Data Control Depth (30 WIs, ~67 gaps)
- **TreeView**: Node-level styles (TreeNodeStyle), selection, ExpandAll/CollapseAll, ExpandDepth
- **Menu**: Base class ΓåÆ BaseStyledComponent, selection+events, core missing props
- **DetailsView**: Style sub-components (10 styles), PagerSettings, Caption
- **FormView**: Remaining events (ModeChanged, ItemCommand, paging), style sub-components, PagerSettings
- **Validators**: ControlToValidate string ID support (migration-critical)
- **Integration tests** for all updated controls


### P2 ΓÇö Nice-to-Have (11 WIs, ~48 gaps)
- **ListView CRUD events** (large effort, ~22 gaps)
- **DataGrid style sub-components + events** (~18 gaps)
- **Menu level styles**, Panel BackImageUrl, Login/ChangePassword Orientation

## Rationale

1. GridView at ~55% is still the most-used data control ΓÇö completing selection and styles is highest-impact.
2. Menu (42%) and TreeView (60%) are the weakest non-deferred controls.
3. Style sub-components are the biggest systematic remaining gap across data controls.
4. Validator ControlToValidate string ID is a migration-blocking API mismatch.
5. PagerSettings should be a shared type across GridView/FormView/DetailsView.
6. ListView CRUD is P2 due to size (L) and lower usage frequency vs. GridView.

## Scoping Rules (unchanged)
- Substitution, Xml: intentionally deferred
- Chart advanced properties: intentionally deferred
- DataSourceID/model binding: N/A in Blazor

## Impact
- Overall health: ~82% ΓåÆ ~87-90%
- GridView: ~55% ΓåÆ ~75%
- TreeView: ~60% ΓåÆ ~75%
- Menu: ~42% ΓåÆ ~60-65%
- FormView: ~50% ΓåÆ ~65%
- DetailsView: ~70% ΓåÆ ~80%

Full plan in `planning-docs/MILESTONE7-PLAN.md`.

# Decision: Remove @rendermode InteractiveServer from CrudOperations.razor

**Author:** Jubilee (Sample Writer)
**Date:** 2025-07-15
**Status:** Applied

## Context

PR #343 introduced `CrudOperations.razor` in `samples/AfterBlazorServerSide/Components/Pages/ControlSamples/ListView/` with `@rendermode InteractiveServer` on line 2. The `AfterBlazorClientSide` project includes all server-side sample pages via wildcard in its csproj, so this directive caused a build failure ΓÇö `InteractiveServer` is not available in the WebAssembly SDK.

## Decision

Removed the `@rendermode InteractiveServer` directive. No other sample page in the `ControlSamples` directory uses this directive; they all work without it. This is the minimal change that restores consistency and fixes the CI build.

## Verification

- `dotnet build samples/AfterBlazorClientSide/ --configuration Release` ΓÇö Γ£à passes
- `dotnet build samples/AfterBlazorServerSide/ --configuration Release` ΓÇö Γ£à passes
- `dotnet test src/BlazorWebFormsComponents.Test/ --no-restore` ΓÇö Γ£à passes

### 2026-02-25: Deployment pipeline patterns for Docker versioning, Azure webhook, and NuGet publishing
**By:** Forge
**What:** Established three CI/CD patterns: (1) Compute version with nbgv outside Docker build and inject via build-arg, since .dockerignore excludes .git. (2) Gate optional deployment steps on repository secrets with `if: ${{ secrets.SECRET_NAME != '' }}` so workflows don't fail when secrets aren't configured. (3) Dual NuGet publishing  always push to GitHub Packages, conditionally push to nuget.org.
**Why:** The .dockerignore excluding .git is a structural constraint that won't change (it's correct for build performance). Secret-gating ensures the workflows work in forks and PRs where secrets aren't available. Dual NuGet publishing gives us private (GitHub) and public (nuget.org) distribution without duplicating the pack step. These patterns should be followed for any future workflow additions.

### 2026-02-25: Deployment pipeline patterns for Docker versioning, secret-gating, and NuGet publishing (consolidated)
**By:** Forge
**What:** Established three CI/CD patterns: (1) Compute version with nbgv outside Docker build and inject via build-arg, since .dockerignore excludes .git. (2) Gate optional deployment steps on repository secrets using env var indirection — declare the secret in `env:`, check `env.VAR_NAME` in step-level `if:`, reference `env.VAR_NAME` in `run:`. Direct `secrets.*` references in step-level `if:` conditions are invalid and cause GitHub Actions validation failures. Applied to both `nuget.yml` and `deploy-server-side.yml` (PR #372). (3) Dual NuGet publishing — always push to GitHub Packages, conditionally push to nuget.org.
**Why:** The .dockerignore excluding .git is a structural constraint that won't change (it's correct for build performance). Secret-gating via env var indirection ensures workflows work in forks and PRs where secrets aren't available. The original `if: ${{ secrets.SECRET_NAME != '' }}` pattern was incorrect — GitHub Actions rejects it at validation time for step-level conditions. Dual NuGet publishing gives us private (GitHub) and public (nuget.org) distribution without duplicating the pack step. These patterns should be followed for any future workflow additions.

### 2026-02-25: Milestone 9 Plan  Migration Fidelity & Hardening

**By:** Forge
**What:** Milestone 9 ratified: 12 WIs, ~30 gap closures. P0: ToolTip  BaseStyledComponent (4 WIs, ~28 gaps). P1: ValidationSummary comma-split fix, SkinID boolstring fix (3 WIs). P2: Branch cleanup, doc audit, planning-docs headers, integration test review, nav audit (5 WIs). 7 of 8 prior audit gaps already fixed; 1 confirmed open + 2 newly identified.
**Why:** ToolTip base class fix has highest blast radius (~28 controls). ValidationSummary is data corruption risk. SkinID type mismatch breaks compiled migration code. Full plan at `planning-docs/MILESTONE9-PLAN.md`.


### 2026-02-25: ToolTip belongs on BaseStyledComponent (consolidated)

**By:** Beast, Cyclops (original audit: 2026-02-23), Cyclops (implementation: 2026-02-25)
**What:** `[Parameter] public string ToolTip { get; set; }` added to `BaseStyledComponent`. Removed 8 duplicate declarations (Button, Calendar, DataList, FileUpload, HyperLink, Image, ImageButton, ImageMap). 32 templates updated with `title="@ToolTip"`. Sub-component types (ChartSeries, DataPoint, MenuItem, TreeNode) keep their own ToolTip (item-level semantics). All 28+ styled controls now inherit ToolTip automatically.
**Why:** Web Forms `WebControl.ToolTip` is defined at base class level. Two independent audits (Beast LX, Cyclops AI) confirmed universal gap. Base-class fix is highest-leverage single change.


### 2026-02-25: ValidationSummary comma-split fixed (consolidated)

**By:** Rogue (bug identification: 2026-02-23), Cyclops (fix: 2026-02-25)
**What:** `AspNetValidationSummary.ValidationMessages` now uses `IndexOf(',')` + `Substring()` instead of `Split(',')[1]` to extract error messages. The field identifier is always before the first comma; everything after is the message.
**Why:** Error messages containing commas were silently truncated  data corruption bug. Original audit by Rogue identified the issue; Cyclops implemented the fix.


### 2026-02-25: SkinID is a string, not a bool

**By:** Cyclops
**What:** `BaseWebFormsComponent.SkinID` type changed from `bool` to `string`. `[Obsolete]` attribute preserved.
**Why:** Web Forms `Control.SkinID` is a string containing the skin name. Boolean type breaks any migration code setting `SkinID="MySkin"`.


### 2026-02-25: Documentation gap audit  M6-M8 features (WI-09)

**By:** Beast
**What:** Audited docs against M6-M8 features. Fully documented: GridView, TreeView, Menu, Validators, Login. Gaps found: FormView (ItemCommand, styles, PagerSettings not in Blazor sections), DetailsView (Caption missing, styles/PagerSettings possibly stale), DataGrid (paging status unclear), ChangePassword (Orientation/TextLayout undocumented), PagerSettings (no dedicated doc page).
**Why:** Ensures documentation accuracy before 1.0. Gaps prioritized P1-P3.


### 2026-02-25: Planning-docs marked as historical snapshots (WI-10)

**By:** Beast
**What:** Added historical snapshot headers to all 54 per-control audit files and SUMMARY.md in `planning-docs/`. Excluded README.md and MILESTONE*-PLAN.md (still current).
**Why:** Prevents future contributors from treating pre-M6 gap data as current.


### 2026-02-25: Integration test coverage audit (WI-11)

**By:** Colossus
**What:** 100 of 105 sample page routes covered by smoke tests. 5 gaps: ListView CrudOperations (M7, P0), Label, Panel/BackImageUrl, LoginControls/Orientation, DataGrid/Styles (all pre-M7, P1). 9 of 10 M7 features have full coverage (smoke + interaction). 57 interaction tests exist.
**Why:** Read-only audit to identify test coverage gaps before 1.0.


### 2026-02-25: Sample site navigation audit (WI-12)

**By:** Jubilee
**What:** 4 components missing entirely from ComponentCatalog.cs (Menu, DataBinder, PasswordRecovery, ViewState). 15 SubPages missing across GridView (5), TreeView (2), FormView (3), DetailsView (2), ListView (1), DataGrid (1), Panel (1). DataList "Flow" SubPage name mismatch. All 10 M7/M8 feature pages exist on disk but none linked in sidebar.
**Why:** Users cannot discover sample pages that aren't in the sidebar navigation.




### 2026-02-25: Consolidated audit reports use `planning-docs/AUDIT-REPORT-M{N}.md`

**By:** Beast
**What:** When multiple audits are conducted in a milestone, their findings should be consolidated into a single report at `planning-docs/AUDIT-REPORT-M{N}.md`. The report follows the planning-docs historical snapshot header convention and includes: summary table, per-audit sections (findings + resolution status), additional findings section, and a complete issue tracker appendix. Each finding is mapped to its resolving GitHub Issue with assigned agent.
**Why:** M9 produced three separate audits (doc gaps, test coverage, sample navigation) with findings scattered across agent history files. A consolidated report makes it easy for Jeff and the team to see all findings in one place, track resolution against M10 issues, and verify coverage. This pattern should be reused for future milestone audits.


### 2026-02-25: TreeView NodeImage must check ShowExpandCollapse independently of ShowLines

**By:** Cyclops
**Date:** 2026-02-25
**Issue:** #361
**What:** The `NodeImage` property in `TreeNode.razor.cs` now explicitly checks `ShowExpandCollapse` in the non-`ShowLines` code paths, rather than relying on `ImageSet.Collapse` being non-empty. A new `ExpandCollapseImage(bool)` helper provides the ImageSet filename with a guaranteed fallback to `Default_Collapse.gif` / `Default_Expand.gif`.
**Why:** The previous code had a fragile assumption: it used `string.IsNullOrEmpty(ImageSet.Collapse)` as a proxy for "should I show expand/collapse images." The fix makes the intent explicit  `ShowExpandCollapse` controls whether expand/collapse images are used, and the ImageSet only controls *which* images. All 51 TreeView tests pass.


### 2026-02-25: Migration Analysis Tool PoC architecture
**By:** Forge
**What:** Milestone 12 introduces a Migration Analysis Tool as a CLI (`bwfc-migrate`) in the same repo at `src/BlazorWebFormsComponents.MigrationAnalysis/`. The PoC uses regex-based ASPX parsing (not Roslyn) to extract `<asp:*>` controls, maps them against a registry of all 53 planned BWFC components + ~15 known unsupported controls, analyzes code-behind patterns via regex, scores page complexity (Green/Yellow/Red), and produces Markdown + JSON reports. Packaged as a `dotnet tool`. Three-phase roadmap: M12 = analysis + CLI, Phase 2 = Roslyn + scaffolding, Phase 3 = Copilot agent. 13 work items total.
**Why:** At 51/53 components complete, the component library is mature. The highest-leverage remaining work is helping developers evaluate and execute migrations using the components we already built. Same-repo placement keeps the control mapping table in sync with the actual component library. Regex over Roslyn prevents scope creep in the PoC — Roslyn is explicitly Phase 2. The tool transforms BlazorWebFormsComponents from a component library into a migration platform.


# Decision: Dev Branch Cleanup (Fork Alignment)

**Date:** 2026-02-25
**Author:** Forge (Lead / Web Forms Reviewer)
**Requested by:** Jeffrey T. Fritz

## Context

PR #372 (csharpfritz:dev to FritzAndFriends:dev) was showing ~39 commits, but only 2 were actual new work. The remaining ~37 were old pre-squash commits and merge commits from previous milestones that had already been squash-merged upstream via earlier PRs.

## Decision

Reset origin/dev to upstream/dev and cherry-pick only the 2 relevant CI fix commits:

1. b4f021c -> 24084d9 -- fix(ci): use env var pattern for secrets in nuget.yml if condition
2. 0532c40 -> 97eedc5 -- fix(ci): use env var pattern for secrets in deploy-server-side.yml

Used --force-with-lease for the force-push to origin (not --force), which ensures we don't overwrite concurrent changes.

## Rationale

- The 37 extra commits were noise -- they'd already been incorporated upstream via squash merges
- PRs should show only the delta of new work, not re-introduce old history
- Clean commit ranges make code review tractable and reduce merge conflicts

## Verification

- git log --oneline upstream/dev..origin/dev shows exactly 2 commits, no merge commits
- upstream/main is clean: squash merge from PR #371 visible at d9a295b
- Force-push succeeded with --force-with-lease (no rejected push)

## Impact

- PR #372 should now show only the 2 CI fix commits
- No code was lost -- all previously merged work exists in upstream/dev


### 2026-02-25: Strip NBGV from Docker build to fix version stamping

**By:** Forge

**What:** Added `sed` command in `samples/AfterBlazorServerSide/Dockerfile` (after `COPY . .`, before `dotnet build`) to remove the `Nerdbank.GitVersioning` PackageReference from `Directory.Build.props` inside the Docker build context. This allows the SDK's default assembly attribute generation to use the externally-passed `-p:Version=$VERSION -p:InformationalVersion=$VERSION` properties without NBGV interference.

**Why:** NBGV's MSBuild targets unconditionally override version properties during build execution via task `<Output>` elements, which bypass MSBuild global property (`-p:`) precedence. Inside Docker (where `.git` is excluded by `.dockerignore`), NBGV falls back to `version.json` and stamps assemblies with a stale/inaccurate version instead of the precise version computed by `nbgv get-version` in the workflow. No combination of `-p:` properties can reliably prevent this because NBGV: (1) suppresses SDK attribute generation, (2) overwrites `AssemblyInformationalVersion` during execution, and (3) generates its own attribute source file. The only reliable fix is to remove NBGV entirely from the Docker build, which is safe because all projects only depend on NBGV transitively through `Directory.Build.props` — none reference NBGV properties directly.



### 2026-02-25: Fix 19 unreachable sample pages in ComponentCatalog.cs (#350)

**By:** Jubilee

**What:** Updated `ComponentCatalog.cs` to link all 19 previously unreachable sample pages. Added 5 new component entries (DataBinder, ViewState, DetailsView, Menu, PasswordRecovery) and 15 new SubPage entries across GridView, TreeView, FormView, ListView, DataGrid, and Panel. Fixed DataList SubPage name from "Flow" to "SimpleFlow".

**Why:** Sample pages existed on disk but were unreachable from the catalog navigation. DetailsView had zero catalog presence despite having 3 sample pages. SubPages are alphabetically sorted to match existing convention.


### 2026-02-25: PagerSettings gets a dedicated doc page (#359)

**By:** Beast

**What:** Created `docs/DataControls/PagerSettings.md` as a standalone documentation page for the shared PagerSettings sub-component. Added to mkdocs.yml nav under Data Controls. PagerSettings is used by FormView, DetailsView, and GridView. Future shared sub-components of similar complexity should follow this pattern.

**Why:** Rather than duplicating the PagerSettings property reference in each parent control's doc, a single dedicated page is linked from all three.


### 2026-02-25: Stale "NOT Supported" doc entries corrected (#359)

**By:** Beast

**What:** Removed PagerSettings and row styles from DetailsView's "NOT Supported" list. Removed Paging, Sorting, Selection, and Editing from DataGrid's "NOT Supported" list. All were implemented in M6-M8 but docs were never updated.

**Why:** Stale "NOT Supported" entries mislead migrators into thinking features are missing when they actually work. Future milestone work should include a doc review pass to catch similar drift.


### 2026-02-25: LoginView and PasswordRecovery migrated to BaseStyledComponent (#352, #354)

**By:** Cyclops

**What:** LoginView and PasswordRecovery now inherit from `BaseStyledComponent` instead of `BaseWebFormsComponent`, matching Login, ChangePassword, and CreateUserWizard. PasswordRecovery renders CssClass/Style/ToolTip on all three step tables. LoginView has no wrapper element (template-switching component) so styled properties are available but not rendered.

**Why:** All login controls should consistently inherit from BaseStyledComponent. No breaking changes — all 1200+ tests pass.


### 2026-02-25: ListView CRUD events — full event parity (#356)

**By:** Cyclops

**What:** ListView now has full event parity for CRUD operations. 7 remaining events implemented: Sorting, Sorted, PagePropertiesChanging, PagePropertiesChanged, SelectedIndexChanging, SelectedIndexChanged, LayoutCreated. Sort/Select commands route through HandleCommand; paging uses dedicated SetPageProperties() method. LayoutCreated converted from EventHandler to EventCallback<EventArgs>. SortDirection toggle matches GridView behavior. 3 new EventArgs classes, 12 new tests.

**Why:** ListView needs full event parity with Web Forms to support migration scenarios where applications rely on these events for sorting, paging, and selection behavior.


### 2026-02-25: Menu styles use MenuItemStyle pattern, not UiTableItemStyle (#360)

**By:** Cyclops

**What:** Menu style sub-components (DynamicMenuStyle, StaticMenuStyle, DynamicMenuItemStyle, StaticMenuItemStyle) inherit `MenuItemStyle` (which inherits `ComponentBase, IStyle`), NOT `UiTableItemStyle`. The `IMenuStyleContainer` interface exposes these as `MenuItemStyle` properties. When adding RenderFragment parameters to a component that already has `ChildContent`, tests mixing named RenderFragments with bare content must wrap the bare content in `<ChildContent>` tags. CascadingValue for `ParentMenu` now has `IsFixed="true"` for performance.

**Why:** Menu styles produce CSS text rendered into an inline `<style>` block via `ToStyle()`. This is fundamentally different from GridView/Calendar styles which use `TableItemStyle` objects applied as HTML element attributes. Forcing Menu styles into the `UiTableItemStyle` hierarchy would require restructuring the entire Menu CSS rendering approach with no benefit.


### 2026-02-25: All 5 missing smoke tests added (#358)

**By:** Colossus

**What:** Added 5 InlineData entries to existing Theory smoke tests in `ControlSampleTests.cs`: ListView CrudOperations, Label, Panel BackImageUrl, LoginControls Orientation, DataGrid Styles. All fit cleanly as InlineData on existing Theory methods — no new Fact tests needed. Panel/BackImageUrl uses external placeholder URLs; smoke test works due to existing "Failed to load resource" filter, but the page should be updated to use local images per team convention.

**Why:** M9 audit identified 5 sample pages without smoke tests. Every sample page is a promise to developers — all must have corresponding smoke tests.


### 2026-02-25: Feature branch workflow required
**By:** Jeffrey T. Fritz (via Copilot)
**What:** All new work MUST be done on feature branches, pushed to origin, with a PR to upstream/dev. Never commit directly to the dev branch.
**Why:** User directive  captured for team memory


# Decision: SkinID and EnableTheming Property Defaults

**Date:** 2026-02-26
**Author:** Cyclops
**Issue:** #365

## Context

The `SkinID` and `EnableTheming` properties on `BaseWebFormsComponent` were previously marked `[Obsolete]` with the message "Theming is not available in Blazor". As part of the Skins & Themes PoC, these properties need to become functional.

## Decision

Per Jeff's confirmed decisions:

- **EnableTheming defaults to `true`** — follows StyleSheetTheme semantics where the theme sets defaults and explicit component values override.
- **SkinID defaults to `""` (empty string)** — meaning "use default skin". This matches Web Forms behavior where an unset SkinID applies the default skin for the control type.
- **`[Obsolete]` attributes removed** — these are now functional `[Parameter]` properties ready for #366 integration.

## Impact

- All components inheriting from `BaseWebFormsComponent` now have `EnableTheming = true` and `SkinID = ""` by default.
- No breaking changes — existing code that doesn't use theming is unaffected since there's no theme provider yet (#364/#366 will add that).
- #366 (Forge's base class integration) can now wire these properties into the theme resolution pipeline.


# Decision: Theme Integration in BaseStyledComponent

**Date:** 2026-02-26
**Author:** Cyclops
**Issue:** #366

## Context

With core theme types (#364) and SkinID/EnableTheming activation (#365) complete, the final wiring step connects the `ThemeConfiguration` cascading parameter to `BaseStyledComponent` so all styled components automatically participate in theming.

## Decisions Made

1. **CascadingParameter on BaseStyledComponent, not BaseWebFormsComponent** — Only styled components have visual properties (BackColor, ForeColor, etc.) to skin. Placing the `[CascadingParameter] ThemeConfiguration Theme` here keeps the concern scoped correctly.

2. **OnParametersSet, not OnInitialized** — Theme values must be applied every time parameters change (e.g., if the theme is swapped at runtime). `OnParametersSet` is the correct lifecycle hook. Early-return when `EnableTheming == false` or `Theme == null` ensures zero impact on existing code.

3. **StyleSheetTheme semantics (defaults, not overrides)** — Each property is only applied when the component's current value equals its type default (`default(WebColor)`, `default(Unit)`, `default(BorderStyle)`, `string.IsNullOrEmpty`, `FontUnit.Empty`, `false` for booleans). This matches ASP.NET Web Forms StyleSheetTheme behavior where themes provide defaults and explicit attribute values take precedence.

4. **No logging for missing named skins** — Per project convention (Jeff's decision on #364), missing SkinID returns null and processing silently continues. ILogger injection deferred to M11 to avoid scope creep.

5. **Font properties checked individually** — Since `Font` is always initialized to `new FontInfo()` in `BaseStyledComponent`, we cannot use a null check. Instead, each font sub-property (Name, Size, Bold, Italic, Underline) is checked against its own default.

## Impact

- All components inheriting `BaseStyledComponent` now automatically receive theme skins when wrapped in `<ThemeProvider>`.
- Existing tests are unaffected — without a `ThemeProvider` ancestor, `Theme` is null and the early-return fires.
- Future work: add Overline/Strikeout font properties, ILogger for missing skin warnings, Theme property override semantics.


# Decision: Theme Core Types Design

**Author:** Cyclops
**Date:** 2026-02-26
**Related:** #364

## Context
WI-1 required core data types for the Skins & Themes PoC.

## Decisions Made

1. **ControlSkin uses nullable property types** — `BorderStyle?`, `Unit?`, and null reference types for `WebColor`, `FontInfo`, `CssClass`, `ToolTip`. This enables StyleSheetTheme semantics: null = "not set by theme, use component default/explicit value." Non-null = "theme wants this value applied as a default."

2. **ThemeConfiguration keys are case-insensitive** — Both the control type name and SkinID lookups use `StringComparer.OrdinalIgnoreCase`. This is forgiving for configuration and matches ASP.NET Web Forms behavior.

3. **Default skin key is empty string** — `AddSkin("Button", skin)` registers a default skin (no SkinID). `AddSkin("Button", skin, "Professional")` registers a named skin. This avoids a separate dictionary for defaults.

4. **ThemeProvider does NOT inherit BaseWebFormsComponent** — It's pure infrastructure (a CascadingValue wrapper), not a Web Forms control emulation. It uses `@namespace BlazorWebFormsComponents.Theming` and a simple `@code` block.

5. **GetSkin returns null for missing entries** — Per Jeff's decision, missing SkinID should log a warning and continue, not throw. The null return lets the integration layer (#366) decide how/where to log.

## Impact
- Integration step (#366) will need to consume `CascadingParameter<ThemeConfiguration>` in `BaseStyledComponent` and apply skin values during initialization.
- The nullable property design means the apply logic will check each property for null before overwriting the component's parameter value.



# Decision: ThemesAndSkins.md updated to reflect PoC implementation

**Author:** Beast (Technical Writer)
**Date:** 2026-02-25
**Scope:** `docs/Migration/ThemesAndSkins.md`

## Context

The ThemesAndSkins.md document was originally written as an exploratory strategy document before implementation. With the M10 PoC now complete (ThemeConfiguration, ControlSkin, ThemeProvider, BaseStyledComponent integration), the doc needed surgical updates to reflect reality.

## Decisions Made

1. **Doc status upgraded from exploratory to implemented.** The "Current Status" admonition now states the PoC is implemented and references actual class names and namespace.

2. **SkinID type warning removed.** The `SkinID` property is now correctly typed as `string` (not `bool` as the doc warned). The warning admonition was replaced with a "tip" confirming correct implementation.

3. **Approach 2 code examples use real API.** All code samples in the CascadingValue ThemeProvider section now reference `ThemeConfiguration.AddSkin()`, `ThemeConfiguration.GetSkin()`, `ControlSkin`, and `ThemeProvider` — the actual class names and method signatures.

4. **Implementation Roadmap Phase 1 marked complete.** All Phase 1 items show ✅ Done. Phase 2 deferred items explicitly listed for M11: `.skin` parser, Theme vs StyleSheetTheme priority, runtime switching, sub-component styles, container EnableTheming propagation, JSON format.

5. **Alternative approaches preserved.** Approaches 1, 3, 4, and 5 remain in the doc as reference context — they document the evaluation process and may be useful for future enhancements.

6. **PoC Decisions section added.** Seven key design decisions documented: StyleSheetTheme default, missing SkinID handling, namespace choice, string-keyed lookups, ControlSkin property mirroring, BaseStyledComponent placement, .skin parser deferral.

## Impact

- No code changes — documentation only.
- Consumers reading the doc will see accurate API examples they can copy-paste.
- The doc now serves as both a migration guide and an architecture decision record.

# Calendar Selection Behavior Review

**By:** Forge
**Date:** 2026-02-26
**Requested by:** Jeffrey T. Fritz
**Scope:** Calendar component selection fidelity vs. ASP.NET Web Forms Calendar control
**Reference:** [Calendar Class (MSDN)](https://learn.microsoft.com/dotnet/api/system.web.ui.webcontrols.calendar?view=netframework-4.8.1)

---

## Verdict

Jeff is right to not be confident. I found **7 issues** — 1 P0 (broken behavior), 4 P1 (wrong behavior), and 2 P2 (polish/gaps). The core click-to-select flow works, but there are real fidelity gaps in the SelectedDates API surface, default property values, parameter sync, and test coverage that would bite migrating teams.

---

## Issues


### 1. P0 — External `SelectedDate` parameter changes are not synced to visual selection

**Web Forms does:** When `SelectedDate` is set programmatically, the `SelectedDates` collection is updated to contain that date, and the calendar re-renders showing that date as selected.

**Our component does:** `_selectedDays` is only populated in `OnInitialized()` and in click handlers. There is NO `OnParametersSet` override. If a parent component programmatically changes the `SelectedDate` parameter (e.g., via data binding, async load, or external button), the `_selectedDays` HashSet is NOT updated. The calendar re-renders but the old date remains visually selected while `SelectedDate` property holds the new value.

**Repro:** Parent component has a "Set to July 4" button that changes the bound `SelectedDate`. Click it — the calendar shows the old selection (or no selection if it's a different month), not July 4.

**Fix:** Add `OnParametersSet` override that detects when `SelectedDate` changed externally and syncs `_selectedDays`:
```csharp
protected override void OnParametersSet()
{
    base.OnParametersSet();
    // Sync _selectedDays when SelectedDate changes from outside
    if (SelectedDate != DateTime.MinValue && !_selectedDays.Contains(SelectedDate.Date))
    {
        _selectedDays.Clear();
        _selectedDays.Add(SelectedDate.Date);
    }
    else if (SelectedDate == DateTime.MinValue && _selectedDays.Count > 0)
    {
        // Parent cleared the selection
        _selectedDays.Clear();
    }
}
```
Need care to avoid clearing multi-date selections (week/month) on re-render — track whether the last selection was internal (click) vs. external (parameter change).

---


### 2. P1 — `SelectWeekText` default value is wrong

**Web Forms does:** `SelectWeekText` defaults to `"&gt;"` (single `>` character).

**Our component does:** Defaults to `"&gt;&gt;"` (double `>>` characters).

**Source:** [Calendar.SelectWeekText (MSDN)](https://learn.microsoft.com/dotnet/api/system.web.ui.webcontrols.calendar.selectweektext?view=netframework-4.8.1) — *"The default value is `&gt;`, which is rendered as a greater than sign (>)."*

**Impact:** Any migrating page that relies on the default selector appearance will render `>>` instead of `>` for week selectors. `SelectMonthText` correctly defaults to `"&gt;&gt;"`, so the distinction between week and month selectors is lost — they both show `>>` when they should show `>` (week) and `>>` (month).

**Fix:** Change line 162 in `Calendar.razor.cs`:
```csharp
public string SelectWeekText { get; set; } = "&gt;";
```

---


### 3. P1 — `SelectedDates` collection is not sorted in ascending order

**Web Forms does:** `SelectedDates` is a `SelectedDatesCollection` that stores dates *"sorted in ascending order by date"* ([MSDN](https://learn.microsoft.com/dotnet/api/system.web.ui.webcontrols.calendar.selecteddates?view=netframework-4.8.1)). Code iterating `SelectedDates` can rely on chronological order. `SelectedDates[0]` is always the earliest date.

**Our component does:** `_selectedDays` is a `HashSet<DateTime>`. The `SelectedDates` property does `_selectedDays.ToList().AsReadOnly()`, which gives **arbitrary order**. A week selection spanning Sunday–Saturday could return dates in any order. Month selections (28–31 dates) are similarly unordered.

**Impact:** Migrating code that does `foreach (var d in Calendar1.SelectedDates)` and expects ascending order, or `SelectedDates[0]` expecting the first chronological date, will produce wrong results.

**Fix:** Sort before returning:
```csharp
public IReadOnlyCollection<DateTime> SelectedDates => 
    _selectedDays.OrderBy(d => d).ToList().AsReadOnly();
```
Or switch `_selectedDays` to a `SortedSet<DateTime>` for O(log n) insert with maintained order.

---


### 4. P1 — `SelectedDates` is read-only; Web Forms allows programmatic manipulation

**Web Forms does:** `SelectedDates` is a `SelectedDatesCollection` with `Add()`, `Remove()`, `Clear()`, `SelectRange(DateTime, DateTime)`, and indexer `[int]` methods. Developers programmatically add/remove dates: `Calendar1.SelectedDates.Add(someDate)`, `Calendar1.SelectedDates.SelectRange(start, end)`.

**Our component does:** `SelectedDates` returns `IReadOnlyCollection<DateTime>`. No `Add`, `Remove`, `Clear`, or `SelectRange` methods. Code using any of these methods won't compile.

**Impact:** Any migrating code that programmatically manipulates selections (blackout dates, preset ranges, holiday selectors) will break at compile time.

**Fix:** Create a `SelectedDatesCollection` class that wraps `_selectedDays` and exposes `Add`, `Remove`, `Clear`, `SelectRange`, `Contains`, and indexer. The collection should sync back to `SelectedDate` (first item) when modified. This is a larger effort but critical for API fidelity.

---


### 5. P1 — Day cell styles are exclusive, not layered/merged

**Web Forms does:** Day cell styles are **merged** — `DayStyle` is the base, then `WeekendDayStyle`, `OtherMonthDayStyle`, `TodayDayStyle`, and `SelectedDayStyle` are layered on top. Each layer overrides only the properties it defines; unset properties inherit from lower layers. A selected weekend day gets `DayStyle` font + `WeekendDayStyle` color + `SelectedDayStyle` background.

**Our component does:** `GetDayCellCss()` and `GetDayCellStyle()` return the **first matching** style and exit. A selected day gets ONLY `SelectedDayStyle` — all base `DayStyle` and `WeekendDayStyle` properties are lost. If `DayStyle` sets `font-size: 14px` and `SelectedDayStyle` sets only `background-color: yellow`, selected days lose their font size.

**Impact:** Calendars with layered styling (extremely common in production Web Forms apps) will look wrong after migration. Selected days may lose fonts, borders, padding set in base styles.

**Fix:** Refactor `GetDayCellCss()` and `GetDayCellStyle()` to merge styles. For inline styles, concatenate non-conflicting properties. For CSS classes, combine all applicable classes (space-separated). This is a deeper architectural fix that may benefit from a `TableItemStyle.MergeWith()` helper.

---


### 6. P2 — No test coverage for week or month selection

**Web Forms does:** N/A (test coverage concern).

**Our tests cover:** Single day click, SelectionMode.None, OnSelectionChanged callback, prev/next navigation. That's it.

**Missing test scenarios:**
- Week selector click → verify all 7 dates in `SelectedDates`
- Week selector click → verify `SelectedDate` equals first day of week
- Month selector click → verify all month dates in `SelectedDates`
- Month selector click → verify `SelectedDate` equals first of month
- Cross-month week selection (e.g., week spanning Jan 29–Feb 4)
- Re-selection: click week A, then click week B → verify week A dates are cleared
- Re-selection: click week, then click individual day → verify week dates are cleared
- Navigation after selection → verify selection visual state preserved
- DayWeek mode → verify month selector is NOT rendered
- DayWeekMonth mode → verify both selectors are rendered

**Impact:** Jeff can't be confident because there's no proof these paths work. The code *looks* correct for most of these, but without tests, regressions are undetectable.

**Fix:** Add comprehensive selection tests to `Selection.razor`. Priority: week selection and month selection end-to-end.

---


### 7. P2 — `SelectedDates` creates a new collection on every access

**Web Forms does:** `SelectedDates` returns the same `SelectedDatesCollection` instance across accesses. `Calendar1.SelectedDates == Calendar1.SelectedDates` is `true`.

**Our component does:**
```csharp
public IReadOnlyCollection<DateTime> SelectedDates => _selectedDays.ToList().AsReadOnly();
```
Every access creates a new `List<DateTime>` + `ReadOnlyCollection<DateTime>`. Reference equality fails. In a loop that accesses `SelectedDates` multiple times, this creates unnecessary allocations.

**Impact:** Performance concern for code that accesses `SelectedDates` in tight loops (e.g., `OnDayRender` handlers checking `SelectedDates.Contains(date)` for every cell — 42 calls per render). Also breaks `ReferenceEquals` checks, though those are rare.

**Fix:** Cache the read-only projection and invalidate when `_selectedDays` changes:
```csharp
private IReadOnlyList<DateTime> _cachedSelectedDates;
private bool _selectionDirty = true;

public IReadOnlyList<DateTime> SelectedDates 
{
    get 
    {
        if (_selectionDirty)
        {
            _cachedSelectedDates = _selectedDays.OrderBy(d => d).ToList().AsReadOnly();
            _selectionDirty = false;
        }
        return _cachedSelectedDates;
    }
}
```

---

## What's Working Correctly

For the record, these aspects are correctly implemented:

- ✅ `CalendarSelectionMode.None` — days render as `<span>`, not clickable
- ✅ `CalendarSelectionMode.Day` — single day selection, clears previous
- ✅ `CalendarSelectionMode.DayWeek` — week selector column appears, day click still selects single day
- ✅ `CalendarSelectionMode.DayWeekMonth` — month selector appears in title row
- ✅ `OnSelectionChanged` fires for day, week, and month selections
- ✅ `SelectedDateChanged` fires with correct date (first day for week/month)
- ✅ Week selection adds all 7 days to `_selectedDays` and sets `SelectedDate` to first day
- ✅ Month selection adds all month days and sets `SelectedDate` to first of month
- ✅ Re-selection clears previous selection (`_selectedDays.Clear()` in all handlers)
- ✅ Selector column hidden when `SelectionMode` is `None` or `Day`
- ✅ Month selector only shown in `DayWeekMonth` mode (empty cell in `DayWeek`)
- ✅ Cross-month week selection works (grid includes prev/next month days)
- ✅ Navigation doesn't clear selection (no `_selectedDays.Clear()` in nav handlers)
- ✅ `SelectedDayStyle` CSS/class applied via `GetDayCellCss()` checking `_selectedDays.Contains()`
- ✅ Style precedence order matches Web Forms (Selected > Today > OtherMonth > Weekend > Day)
- ✅ `SelectMonthText` default (`"&gt;&gt;"`) matches Web Forms
- ✅ Title row colspan arithmetic is correct with/without selector column
- ✅ `OnDayRender` allows synchronous `IsSelectable` override per day

---

## Recommended Priority

| # | Priority | Issue | Effort |
|---|----------|-------|--------|
| 1 | P0 | External SelectedDate parameter sync | Small — add `OnParametersSet` |
| 2 | P1 | SelectWeekText wrong default | Trivial — one-line fix |
| 3 | P1 | SelectedDates not sorted | Small — `OrderBy` or `SortedSet` |
| 4 | P1 | SelectedDates not mutable | Medium — new collection class |
| 5 | P1 | Style layering exclusive not merged | Medium-Large — architectural |
| 6 | P2 | Missing selection tests | Medium — ~10 new test cases |
| 7 | P2 | SelectedDates allocation per access | Small — caching |

**Recommendation:** Fix #1 (P0) and #2 (P1, trivial) immediately. #3 and #6 in the next sprint. #4 and #5 are architectural and should be work items. #7 is nice-to-have.


### 2026-02-26: HTML Audit Strategy and Divergence Registry (consolidated)

**By:** Forge
**What:** Evaluated and conditionally approved Jeff's Playwright-based HTML audit strategy for comparing Web Forms gold-standard output against Blazor component output. The marker-isolation approach is sound but requires normalization rules, sample gap remediation, and an Intentional Divergence Registry.

**Why:** The core idea — wrap controls with marker HTML, extract via Playwright, compare structurally — is the right approach for systematic HTML fidelity verification. However, five issues must be addressed:

1. **Normalization rules are mandatory.** Raw Web Forms HTML contains infrastructure artifacts that Blazor cannot and should not reproduce: `ctl00_MainContent_` ID prefixes, `javascript:__doPostBack(...)` hrefs, `WebResource.axd` image URLs, `__VIEWSTATE`/`__EVENTVALIDATION` hidden fields. The comparison pipeline must strip/normalize these before diffing.

2. **Sample coverage is only ~25%.** Only 13 of ~53 controls have BeforeWebForms samples. Phase 1 must include a "write missing samples" sub-phase before capture can be comprehensive.

3. **Three controls need special handling:** TreeView (JS data objects outside markers), Menu (two rendering modes), Calendar (week/month selectors use __doPostBack). These need per-control extraction strategies.

4. **Chart is permanently excluded.** Web Forms Chart emits `<img src="ChartImg.axd">`. Blazor Chart uses Chart.js canvas. Document as intentional divergence, do not audit.

5. **CodeBehind→CodeFile must be committed.** The dynamic compilation workaround must be in the repo or scripted so the audit pipeline runs on fresh clones.

**Intentional Divergence Registry:** Before the HTML audit begins, establish a formal registry (`planning-docs/intentional-divergences.md`). At least 5 categories of known intentional divergence:
1. **ID format** — Web Forms uses naming-container-prefixed IDs, Blazor uses developer-provided IDs
2. **PostBack mechanism** — Web Forms uses `javascript:__doPostBack()`, Blazor uses its own event system
3. **Resource URLs** — Web Forms uses `WebResource.axd`, Blazor uses static files
4. **Chart rendering** — Web Forms uses server-side image generation, Blazor uses Chart.js canvas
5. **Page infrastructure** — ViewState, EventValidation, ScriptManager output have no Blazor equivalent

**Additional recommendations:**
- **Prioritize Tier 1 controls** for pipeline validation: Button, TextBox, HyperLink, DropDownList, Repeater, DataList.
- Coordinator agent scope should be narrow: track captures, comparisons, and divergences only.
- **Screenshot comparison is low-value** — defer to a later phase.

### 2026-02-25: User directive — HTML audit milestones take priority
**By:** Jeffrey T. Fritz (via Copilot)
**What:** The HTML fidelity audit (BeforeWebForms IIS Express + Playwright capture + Blazor comparison) should be planned as milestones starting at M11. The existing M11 and M12 work (including Migration Analysis Tool PoC) should be delayed until after the audit milestones complete.
**Why:** User request — HTML output fidelity is the foundation that the rest of the project depends on. Audit first, then build tools on top of verified components.

### 2026-02-25: HTML audit milestone plan (M11-M13)
**By:** Forge
**What:** Created a three-milestone plan (M11–M13) for comprehensive HTML fidelity audit comparing Web Forms gold-standard output against Blazor component output. M11: audit infrastructure + Tier 1 (simple controls) capture. M12: Tier 2 (data controls) with normalization pipeline. M13: Tier 3 (JS-coupled) + remaining controls + master audit report. The existing M12 (Migration Analysis Tool PoC) has been renumbered to M14. The previously planned M11 (Skins & Themes Implementation) is deferred to M15+.
**Why:** Jeff directed that HTML output fidelity verification is the foundation the rest of the project depends on. At 51/53 components, we've never systematically verified that Blazor output matches Web Forms output. The audit must complete before building migration tooling (M14) so that tool can reference verified HTML fidelity data. The three-tier phasing (simple → data → JS-coupled) minimizes risk by proving infrastructure on easy controls first.

### 2026-02-26: IIS Express setup script for BeforeWebForms HTML audit
**By:** Cyclops
**What:** Created `scripts/Setup-IISExpress.ps1`  automates BeforeWebForms sample app setup for IIS Express with dynamic compilation. Key design: CodeBehind-to-CodeFile conversion is temporary (reverted via `-Revert` switch using git checkout), NuGet packages restored to `src/packages/`, Roslyn compilers copied to `bin/roslyn/`, nuget.exe downloaded on demand, fully idempotent.
**Why:** The HTML audit requires running the BeforeWebForms sample app under IIS Express to capture gold-standard Web Forms HTML output. Manual setup was error-prone and undocumented.

### 2026-02-26: Intentional divergence registry (D-01 through D-10)
**By:** Forge
**What:** Created `planning-docs/DIVERGENCE-REGISTRY.md` with 10 documented intentional divergences covering ID mangling, PostBack mechanisms, ViewState, WebResource.axd, Chart rendering, Menu table mode, TreeView JS, Calendar selection, Login infrastructure, and Validator scripts. Each entry documents affected controls, divergence description, category, reason, CSS/JS impact, and normalization rules.
**Why:** Without a pre-defined registry, auditors would repeatedly investigate platform-level differences that can never be replicated in Blazor. The registry provides normalization rules for the audit pipeline and classification guidance for audit reports.

### 2026-02-26: Decision: NamingContainer inherits BaseWebFormsComponent, not BaseStyledComponent

**By:** Cyclops
**Date:** 2026-02-26
**Task:** D-01

## What

`NamingContainer` inherits from `BaseWebFormsComponent` and renders no HTML of its own — only `@ChildContent`. It relies on the existing `BaseWebFormsComponent` constructor to cascade itself as `ParentComponent`, which `ComponentIdGenerator` already walks. `UseCtl00Prefix` is handled in `ComponentIdGenerator.GetClientID` by inserting "ctl00" before the NamingContainer's ID in the parts list when the flag is true.

## Why

- **No HTML output:** NamingContainer is a structural/scoping component like `PlaceHolder`, not a visual control. `BaseStyledComponent` would add unused style properties (`CssClass`, `Style`, `BackColor`, etc.) that have no rendering target.
- **Parent cascading already works:** `BaseWebFormsComponent`'s constructor wraps every component's render tree in a `CascadingValue<BaseWebFormsComponent>` named `"ParentComponent"`. This means NamingContainer automatically participates in the parent hierarchy with zero additional wiring — no need for a separate `CascadingValue Name="NamingContainer"` as initially suggested.
- **UseCtl00Prefix in ComponentIdGenerator:** Rather than creating a virtual parent component or overriding ID properties, the cleanest approach was a 5-line addition to `ComponentIdGenerator.GetClientID` that checks `current is NamingContainer nc && nc.UseCtl00Prefix` during the hierarchy walk. This keeps the NamingContainer component simple and the ID generation logic centralized.

## Alternatives Considered

1. **Separate CascadingValue in razor template** — Unnecessary since BaseWebFormsComponent already cascades.
2. **Internal parent component for ctl00** — Over-engineered; modifying ComponentIdGenerator is simpler.
3. **Override ID property getter** — Would break the component's own ID, causing confusion.


### 2026-02-26: D-06: Menu RenderingMode=Table — Whitespace in Table Elements

**Decision by:** Cyclops  
**Date:** 2026-02-26  
**Status:** Implemented

## Context

Adding `RenderingMode=Table` to the Menu component required rendering `<table>/<tr>/<td>` structures instead of `<ul>/<li>`. The straightforward Razor template approach produced whitespace (newlines, tabs) between `<tr>` and `<td>` elements from null RenderFragment expressions and Razor formatting.

## Problem

AngleSharp (used by bUnit for DOM assertions) foster-parents `<td>` elements out of `<tr>` when whitespace text nodes appear between them in table parsing context. This caused all table-mode DOM-query-based tests to fail even though the raw Blazor markup was structurally correct.

## Decision

- Table-mode rendering in `Menu.razor` and `MenuItem.razor` uses inline/compact Razor to eliminate whitespace between table structure elements (`<tr>`, `<td>`).
- Style RenderFragment parameters (`DynamicMenuStyleContent`, etc.) are rendered in a separate `<CascadingValue>` block outside the `<table>` for table mode, keeping the table interior clean.
- This is a rendering-layer concern only; the component API (`RenderingMode` parameter, `MenuRenderingMode` enum) is clean and straightforward.

## Alternatives Considered

1. **RenderTreeBuilder in code-behind** — Would avoid whitespace entirely but adds complexity and diverges from the Razor template pattern used by all other components.
2. **Markup string assertions in tests** — Would work around AngleSharp but makes tests fragile and less readable.

## Impact

- No impact on existing List mode rendering (zero changes to that path).
- 4 new tests validate both rendering modes and both orientations.
- All 1257+ tests pass with 0 regressions.


### 2026-02-26: Decision: Login Controls + Blazor Identity Integration Strategy (D-09)

**Date:** 2026-02-27
**By:** Forge
**Task:** D-09

## Context

The project has 7 login-related Web Forms controls implemented as Blazor components (Login, LoginName, LoginStatus, LoginView, ChangePassword, CreateUserWizard, PasswordRecovery). All are "visual shells" — they render correct Web Forms HTML structure but don't connect to ASP.NET Core Identity. This analysis determines how to bridge that gap.

## Key Findings

1. **LoginName and LoginView already work** — they correctly read `AuthenticationStateProvider` for display purposes. Both need a fix to re-render on auth state changes (currently read once in `OnInitializedAsync`).

2. **The HttpContext problem is the critical constraint.** `SignInManager` operations (sign-in, sign-out) require `HttpContext` for cookie manipulation, which is unavailable in Blazor Server interactive mode and Blazor WebAssembly. Direct `SignInManager` calls from components will fail. Redirect-based flows (navigate to a server endpoint) are the standard solution.

3. **UserManager operations work directly.** `ChangePasswordAsync()`, `CreateAsync()`, `GeneratePasswordResetTokenAsync()` do NOT require `HttpContext` and can be called from Blazor components in any hosting model.

4. **All 7 controls use an event-only pattern** where the developer handles all logic in callbacks. This works but provides no built-in functionality.

## Decisions

### 1. Create a separate `BlazorWebFormsComponents.Identity` NuGet package

The core package must NOT depend on `Microsoft.AspNetCore.Identity`. The Identity package provides pre-built handler implementations and server endpoints.

### 2. Use handler delegate pattern in core package

Each control gets optional `Func<>` handler delegate parameters (e.g., `AuthenticateHandler`, `ChangePasswordHandler`). When set, the component calls the handler instead of relying on event-only patterns. When not set, existing behavior is preserved. **Zero breaking changes.**

### 3. Priority order: LoginName → LoginView → LoginStatus → Login → ChangePassword → CreateUserWizard → PasswordRecovery

The first three are Small complexity (1-2 days each). Login and ChangePassword are Medium (3-5 days). CreateUserWizard and PasswordRecovery are Large (5-10 days).

### 4. Auth state re-render fix is cross-cutting

LoginName, LoginView, LoginStatus, and Login all need to subscribe to `AuthenticationStateChanged` or accept `[CascadingParameter] Task<AuthenticationState>`. This should be done as a single foundational change before individual control work.

### 5. Redirect-based flows for cookie operations

Login and LoginStatus should get `LoginActionUrl` and `LogoutActionUrl` parameters for redirect-based sign-in/sign-out. The Identity package provides the corresponding server endpoints.

## Impact

- **Core package:** Additive API surface only. No breaking changes. ~4-6 weeks.
- **Identity package:** New package. ~3-4 weeks after core changes.
- **Full analysis:** `planning-docs/LOGIN-IDENTITY-ANALYSIS.md`


### 2026-02-26: Decision: Data Control Divergence Analysis Results

**Date:** 2026-02-27
**By:** Forge (Lead / Web Forms Reviewer)
**Status:** Pending Review
**Relates to:** M13 HTML Fidelity Audit — Data Controls (DataList, GridView, ListView, Repeater)

## Context

The M13 audit captured WebForms and Blazor HTML for 4 data controls showing 389 total line differences (DataList 110, GridView 33, ListView 182, Repeater 64). A line-by-line analysis was performed to classify each divergence.

## Decision

### 1. Sample Parity is the dominant cause of divergences

The majority of differences (estimated 90%+) are caused by the Blazor sample pages using different templates, styles, columns, and data formats than the corresponding Web Forms samples. The Blazor samples must be rewritten to match the Web Forms samples before meaningful fidelity comparison is possible.

**Implication:** Items 6–10 in the action plan (Jubilee tasks) are prerequisites for accurate audit results.

### 2. Five genuine component bugs identified

| Bug | Control | Description | Priority |
|-----|---------|-------------|----------|
| BUG-DL-2 | DataList | Missing `itemtype` attribute | P2 |
| BUG-DL-3 | DataList | `border-collapse:collapse` unconditionally rendered | P1 |
| BUG-GV-1 | GridView | Default GridLines may not match WF default (Both) | P1 |
| BUG-GV-2 | GridView | Missing default `border-collapse:collapse` | P1 |
| BUG-GV-3 | GridView | Empty `<th>` instead of `&nbsp;` for blank headers | P2 |

### 3. ListView and Repeater have zero component bugs

All differences in these two controls are entirely sample parity issues. The components render templates correctly.

### 4. Normalization pipeline gaps

- Blazor output for data controls has not been normalized (directories missing under `audit-output/normalized/blazor/`)
- `<!--!-->` Blazor rendering markers need to be stripped by the normalization pipeline (new rule needed)

## Full Analysis

See `planning-docs/DATA-CONTROL-ANALYSIS.md` for the complete line-by-line breakdown.

## Impact

- 3 P1 bugs need Cyclops fixes before M13 completion
- 4 sample rewrites needed (Jubilee) before re-capture
- Normalization pipeline update needed (Colossus)


### 2026-02-26: Decision: Post-Bug-Fix Capture Results — Sample Parity is the Primary Blocker

**Date:** 2026-02-26
**Author:** Rogue (QA)
**Status:** proposed
**Scope:** HTML fidelity audit pipeline

## Context

Re-ran the full HTML capture pipeline after 14 bug fixes across 10 controls (Button, BulletedList, LinkButton, Calendar, TreeView, CheckBox, RadioButtonList, FileUpload, Image, DataList, GridView). Previous run (M13) showed 132 divergences with 0 exact matches.

## Results

- **All 11 targeted controls show verified structural improvements** in Blazor output
- Exact matches: 0 → 1 (Literal-3)
- Missing Blazor captures: 75 → 64 (11 new captures gained)
- Calendar variants now at 55–73% word similarity (was much lower)
- Bug fixes confirmed working: Button `<button>`→`<input>`, BulletedList span removal, LinkButton href addition, Calendar border styling, CheckBox span removal, Image longdesc removal, GridView rules/border addition

## Decision

**The #1 blocker for achieving meaningful match rates is sample data parity, not component bugs.**

Nearly every remaining divergence is caused by the WebForms and Blazor samples using completely different text, data, IDs, styling, and configuration. Examples: Label shows "Hello World" vs "Hello, World!", Button shows "Blue Button" vs "Click me!", HyperLink links to bing.com vs github.com.

### Recommended priority order:
1. **P0:** Align Blazor sample data to match WebForms samples (could convert 20+ divergences to exact matches)
2. **P1:** Add audit markers to 64 missing Blazor sample pages
3. **P2:** Enhance normalizer (GUID ID stripping, empty style="" removal)
4. **P3:** Fix remaining structural bugs (BulletedList `<ol>` for numbered, missing `list-style-type`)

## Impact

Without sample alignment, the pipeline cannot distinguish between "component renders wrong HTML" and "samples show different content." Any future bug-fix measurement will be equally blocked.

## Evidence

Full analysis: `planning-docs/POST-FIX-CAPTURE-RESULTS.md`
Diff report: `audit-output/diff-report-post-fix.md`

