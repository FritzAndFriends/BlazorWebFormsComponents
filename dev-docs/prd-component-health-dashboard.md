# PRD: Component Health Dashboard

**Author:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-07-25
**Status:** Approved — Ready for implementation
**Requested by:** Jeffrey T. Fritz
**GitHub Issue:** [#439](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/439)

---

## 1. Purpose & Goals

### 1.1 What Is This?

A diagnostic dashboard that measures how completely each BWFC Blazor component reproduces the behavior of its original ASP.NET Web Forms control. It answers: *"For each Web Forms control we've re-created, how close is the Blazor version to the original?"*

### 1.2 Who Uses It?

| Audience | What They Need |
|----------|---------------|
| **Jeff (project owner)** | At-a-glance progress across all ~55 components; spot gaps to prioritize |
| **Squad agents (Forge, Cyclops, etc.)** | Per-component detail to guide implementation work |
| **External contributors** | Understanding of what's done vs. what's incomplete |
| **Migrating developers** | Confidence that a component they depend on is fully implemented |

### 1.3 What Decisions Does It Inform?

- Which components need more properties/events implemented?
- Which components have no tests or documentation?
- Where are the remaining HTML fidelity gaps?
- What should the next sprint prioritize?

### 1.4 What Does "Component Health" Mean?

A component is "healthy" when:
1. It implements the same **properties** as the original Web Forms control
2. It fires the same **events** as the original
3. It has **bUnit test coverage**
4. It has **documentation** (MkDocs page)
5. It has a **sample page** in the sample app
6. Its rendered **HTML output matches** the original Web Forms control

A perfect score means: "This Blazor component is a drop-in replacement for the Web Forms original."

---

## 2. Data Model — Property & Event Counting

> **This section exists because the first implementation got counting wrong in 5 different ways.** Every rule below is a direct response to a real bug.

### 2.1 The BWFC Class Hierarchy

```
ComponentBase                          ← Blazor framework
  └── BaseWebFormsComponent            ← BWFC base: ID, Enabled, Visible, lifecycle events
        └── BaseStyledComponent        ← Style props: BackColor, CssClass, Font, etc. (9 props)
              ├── ButtonBaseComponent   ← Button family: Text, OnClick, OnCommand, etc.
              │     ├── Button
              │     ├── ImageButton
              │     └── LinkButton
              ├── BaseDataBoundComponent  ← DataSource, OnDataBound
              │     └── DataBoundComponent<T>  ← Items, DataMember, SelectMethod
              │           ├── GridView<T>
              │           ├── Repeater<T>
              │           ├── ListView<T>
              │           └── ... (all data-bound controls)
              ├── Calendar
              ├── TextBox
              ├── Panel
              └── ... (most other controls)
```

### 2.2 Which Properties Count as "Implemented"?

**The core problem:** In Web Forms, every control inherits from `System.Web.UI.WebControls.WebControl`, which provides `ID`, `CssClass`, `Enabled`, `Visible`, `BackColor`, `ForeColor`, etc. When we compare a BWFC component to its Web Forms original, we should compare **control-specific** properties only — not the base class properties that every control inherits.

**Rule: Count [Parameter] properties from the COMPONENT-SPECIFIC layer only.**

The "component-specific layer" is defined as:

| Inheritance Level | Example | Counted? | Rationale |
|-------------------|---------|----------|-----------|
| `BaseWebFormsComponent` | ID, Enabled, Visible, AccessKey, TabIndex | ❌ NO | Equivalent to `WebControl` / `Control` base. Every component has these. |
| `BaseStyledComponent` | BackColor, ForeColor, CssClass, Font, Width, Height, etc. | ❌ NO | Equivalent to `WebControl` style properties. Every styled component has these. |
| `BaseDataBoundComponent` | DataSource, DataSourceID | ❌ NO | Equivalent to `BaseDataBoundControl`. Every data-bound component has these. |
| `DataBoundComponent<T>` | Items, DataMember, SelectMethod | ⚠️ CASE-BY-CASE | These are BWFC's generic data-binding API. Count only if the original Web Forms control had an equivalent (e.g., `Items` maps to the original's `DataSource` items collection). See §2.6. |
| **Intermediate base** (e.g., `ButtonBaseComponent`) | Text, OnClick, OnCommand, CausesValidation | ✅ YES | These are specific to the button family, not shared across all controls. |
| **Leaf component** (e.g., `Button`) | UseSubmitBehavior | ✅ YES | Component-specific. |

**Implementation: Walk the inheritance chain upward from the leaf class. Stop counting when you hit `BaseWebFormsComponent`, `BaseStyledComponent`, `BaseDataBoundComponent`, or `DataBoundComponent<T>`.** Everything between the leaf and those stop-points is "component-specific."

### 2.3 What Counts as an "Implemented Event"?

**Rule: `EventCallback<T>` parameters are EVENTS, not properties. Count them in the events column ONLY.**

An "event" is any `[Parameter]` whose type is `EventCallback` or `EventCallback<T>`.

**Double-counting prevention:** When scanning [Parameter] properties for the "properties" count, **skip** any parameter whose type is `EventCallback` or `EventCallback<T>`. These appear ONLY in the "events" column.

**Exclusions from event count:**
- Lifecycle events on `BaseWebFormsComponent` (OnInit, OnLoad, OnPreRender, OnUnload, OnDisposed, OnDataBinding) — these are base class events, not component-specific.
- `OnDataBound` on `BaseDataBoundComponent` — base class event.

**Only count events declared in the component-specific layer** (same layer rules as §2.2).

### 2.4 How to Handle RenderFragment / Template Parameters

**Rule: RenderFragment and RenderFragment<T> parameters are EXCLUDED from both property and event counts.**

These are Blazor's template slot mechanism. They have no direct Web Forms property equivalent. In Web Forms, templates like `ItemTemplate` are nested XML elements, not properties.

Examples of excluded RenderFragment parameters:
- `ItemTemplate`, `HeaderTemplate`, `FooterTemplate`, `SeparatorTemplate` (Repeater)
- `RowStyleContent`, `HeaderStyleContent`, `EmptyDataTemplate`, `Columns` (GridView)
- `ChildContent`, `ChildComponents`

**Rationale:** Counting these inflates the BWFC "implemented" count beyond what the Web Forms original had as enumerable properties, making comparisons meaningless.

### 2.5 How to Handle Blazor Infrastructure Parameters

**Rule: The following are EXCLUDED from all counts:**

| Parameter | Why Excluded |
|-----------|-------------|
| `AdditionalAttributes` (`CaptureUnmatchedValues=true`) | Blazor HTML attribute pass-through; no Web Forms equivalent |
| `ChildContent` (RenderFragment) | Blazor content projection infrastructure |
| `ChildComponents` (RenderFragment) | BWFC child component slot infrastructure |
| Any `[CascadingParameter]` | Blazor cascading value infrastructure (Theme, Parent, ValidationCoordinator) |
| Any `[Inject]` service | Dependency injection, not a control property |
| Any `[Parameter]` marked `[Obsolete]` | Compatibility shims (runat, EnableViewState, DataKeys, etc.) |

### 2.6 How to Handle Generic Types in Reflection

**Rule: Strip the generic arity suffix from reflection type names.**

.NET reflection returns `GridView`1` for `GridView<T>`. When matching against reference data, strip everything from the backtick onward:

```csharp
string cleanName = type.Name.Contains('`')
    ? type.Name.Substring(0, type.Name.IndexOf('`'))
    : type.Name;
```

This applies to: GridView, Repeater, ListView, DataList, DataGrid, DetailsView, FormView, BulletedList, CheckBoxList, DropDownList, RadioButtonList, ListBox, DataBoundComponent, BoundField, ButtonField, HyperLinkField, TemplateField.

### 2.7 Counting Summary — Worked Examples

#### Example: Button

**Inheritance chain:** `Button → ButtonBaseComponent → BaseStyledComponent → BaseWebFormsComponent`

**Stop point:** `BaseStyledComponent`

**Component-specific layer:** `Button` + `ButtonBaseComponent`

| Source | Parameter | Type | Counted As |
|--------|-----------|------|------------|
| ButtonBaseComponent | Text | string | ✅ Property |
| ButtonBaseComponent | CausesValidation | bool | ✅ Property |
| ButtonBaseComponent | ValidationGroup | string | ✅ Property |
| ButtonBaseComponent | CommandName | string | ✅ Property |
| ButtonBaseComponent | CommandArgument | object | ✅ Property |
| ButtonBaseComponent | PostBackUrl | string | ✅ Property |
| ButtonBaseComponent | OnClientClick | string | ✅ Property |
| ButtonBaseComponent | OnClick | EventCallback | ✅ **Event** (not property) |
| ButtonBaseComponent | OnCommand | EventCallback | ✅ **Event** (not property) |
| Button | UseSubmitBehavior | bool | ✅ Property |

**Result: 7 properties, 2 events**

#### Example: GridView<T>

**Inheritance chain:** `GridView<T> → DataBoundComponent<T> → BaseDataBoundComponent → BaseStyledComponent → BaseWebFormsComponent`

**Stop point:** `DataBoundComponent<T>` (but see §2.2 — case-by-case for DataBoundComponent props)

**Component-specific layer:** `GridView<T>` only (DataBoundComponent<T> props excluded as base-class data binding)

| Category | Properties | Events |
|----------|-----------|--------|
| Column/display | AutoGenerateColumns, EmptyDataText, DataKeyNames, ShowHeader, ShowFooter, ShowHeaderWhenEmpty, Caption, CaptionAlign, GridLines, UseAccessibleHeader, CellPadding, CellSpacing | — |
| Selection | AutoGenerateSelectButton, EditIndex, SelectedIndex | SelectedIndexChanging, SelectedIndexChanged |
| Sorting | AllowSorting, SortDirection, SortExpression | Sorting, Sorted |
| Paging | AllowPaging, PageSize, PageIndex | PageIndexChanged |
| Row operations | — | OnRowCommand, RowEditing, RowUpdating, RowDeleting, RowCancelingEdit |
| **EXCLUDED (RenderFragment)** | ~~RowStyleContent, HeaderStyleContent, FooterStyleContent, AlternatingRowStyleContent, EmptyDataRowStyleContent, PagerStyleContent, EditRowStyleContent, SelectedRowStyleContent, PagerSettingsContent, EmptyDataTemplate, Columns, ChildContent~~ | — |

**Result: ~18 properties, ~10 events** (exact count depends on final baseline verification)

---

## 3. Reference Baselines — Expected Property & Event Counts

### 3.1 The Problem With Guessing

The first dashboard attempt used **estimated** expected counts. These guesses were wrong for most complex components, making the health percentages meaningless.

### 3.2 Methodology: How to Derive Expected Counts

Reference counts MUST come from the **actual .NET Framework 4.8 type metadata**, following rules symmetric to the BWFC counting rules:

1. **Source:** The public instance properties of the Web Forms control class in `System.Web.UI.WebControls` (or `System.Web.UI.HtmlControls`, `System.Web.UI.WebControls.WebParts`, etc.).

2. **Exclude base class properties:** Properties inherited from `WebControl`, `Control`, `BaseDataBoundControl`, `DataBoundControl`, and `System.Web.UI.HtmlControls.HtmlControl` are excluded. Only the properties **declared on the control class itself and its immediate family** (e.g., `BaseCompareValidator` props count for `CompareValidator`).

3. **Events vs. properties:** In Web Forms, events are declared as `public event EventHandler<T> EventName`. Count them in the "expected events" column, not "expected properties."

4. **How to obtain these counts:**
   - Manually curate from the [.NET Framework 4.8 API reference](https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols) documentation, documented in a versioned JSON file (`dev-docs/reference-baselines.json`). MSDN is the sole authoritative source for reference baselines.

5. **Baseline file format:** `dev-docs/reference-baselines.json`

```json
{
  "version": "1.0",
  "source": "MSDN .NET Framework 4.8 API documentation",
  "generated": "2026-07-25T00:00:00Z",
  "components": {
    "Button": {
      "namespace": "System.Web.UI.WebControls",
      "expectedProperties": 8,
      "expectedEvents": 2,
      "propertyList": ["Text", "CausesValidation", "ValidationGroup", "CommandName", "CommandArgument", "PostBackUrl", "OnClientClick", "UseSubmitBehavior"],
      "eventList": ["Click", "Command"],
      "notes": "Properties from ButtonBase class included"
    },
    "GridView": {
      "namespace": "System.Web.UI.WebControls",
      "expectedProperties": 35,
      "expectedEvents": 12,
      "notes": "Complex control with sorting, paging, editing, selection"
    }
  }
}
```

### 3.3 Components to Track

The dashboard tracks the **52 completed + 2 deferred = 54 targeted Web Forms controls** listed in `status.md`, plus utility features:

| Category | Count | Components |
|----------|-------|------------|
| **Editor Controls** | 27 | AdRotator, Button, BulletedList, Calendar, CheckBox, CheckBoxList, DropDownList, FileUpload, HiddenField, HyperLink, Image, ImageButton, ImageMap, Label, LinkButton, ListBox, Literal, Localize, MultiView, Panel, PlaceHolder, RadioButton, RadioButtonList, Table, TextBox, View, ~~Substitution~~, ~~Xml~~ |
| **Data Controls** | 9 | Chart, DataGrid, DataList, DataPager, DetailsView, FormView, GridView, ListView, Repeater |
| **Validation Controls** | 8 | CompareValidator, CustomValidator, RangeValidator, RegularExpressionValidator, RequiredFieldValidator, ValidationSummary, ~~BaseValidator~~, ~~BaseCompareValidator~~ |
| **Navigation Controls** | 3 | Menu, SiteMapPath, TreeView |
| **Login Controls** | 7 | ChangePassword, CreateUserWizard, Login, LoginName, LoginStatus, LoginView, PasswordRecovery |
| **Infrastructure** | 6 | Content, ContentPlaceHolder, MasterPage, NamingContainer, ScriptManager, UpdatePanel |

> **Note:** BaseValidator and BaseCompareValidator are abstract base classes, not standalone Web Forms controls. They should be listed but scored as "N/A" for property parity (their properties roll up into the concrete validators).

### 3.4 What NOT to Track

These types exist in the BWFC assembly but are NOT Web Forms control equivalents:

| Category | Count | Examples |
|----------|-------|---------|
| Style container components | 63 | CalendarDayStyle, GridViewHeaderStyle, LoginButtonStyle, etc. |
| Pager settings components | 3 | GridViewPagerSettings, FormViewPagerSettings, etc. |
| Field column components | 4 | BoundField<T>, ButtonField<T>, HyperLinkField<T>, TemplateField<T> |
| Base classes (abstract) | 6 | BaseWebFormsComponent, BaseStyledComponent, BaseDataBoundComponent, DataBoundComponent<T>, ButtonBaseComponent, BaseValidator |
| Enums | 54 | BorderStyle, GridLines, SortDirection, etc. |
| Event args classes | 20+ | GridViewCommandEventArgs, etc. |
| Utility/shim classes | ~15 | WebControl, HtmlTextWriter, CommandEventArgs, etc. |
| Chart sub-components | 4 | ChartArea, ChartLegend, ChartSeries, ChartTitle |

These are **infrastructure types that support the main components**. They should not appear in the health dashboard.

---

## 4. Health Scoring Model

### 4.1 Scoring Dimensions

| # | Dimension | Weight | Source | What It Measures |
|---|-----------|--------|--------|------------------|
| 1 | **Property Parity** | 30% | Reflection | `min(implementedProps / expectedProps, 1.0)` |
| 2 | **Event Parity** | 15% | Reflection | `min(implementedEvents / expectedEvents, 1.0)` |
| 3 | **Has bUnit Tests** | 20% | File scan | Does `**/ComponentName*.razor` or `**/ComponentNameTests.cs` exist in the test project? Binary: 0% or 100%. |
| 4 | **Has Documentation** | 15% | File scan | Does a MkDocs page exist at `docs/components/ComponentName.md` or equivalent? Binary: 0% or 100%. |
| 5 | **Has Sample Page** | 10% | ComponentCatalog.cs | Is the component registered in `ComponentCatalog.Components`? Binary: 0% or 100%. |
| 6 | **Implementation Status** | 10% | status.md / manual | Complete = 100%, Stub = 50%, Deferred = 0%, Not Started = 0%. |

**Total: 100%**

### 4.2 Weight Rationale

| Dimension | Weight | Why |
|-----------|--------|-----|
| Property Parity (30%) | Highest weight because property completeness is the #1 migration concern. If a property a developer uses isn't implemented, migration fails. |
| Has bUnit Tests (20%) | Tests prove the component actually works. Second-highest because untested components are unreliable. |
| Event Parity (15%) | Events are important but fewer in number than properties. Many simple controls have 0-1 events. |
| Has Documentation (15%) | Developers can't use what they can't find. Documentation is table-stakes for an open-source library. |
| Has Sample Page (10%) | Samples demonstrate usage but are less critical than tests or docs. |
| Implementation Status (10%) | A sanity-check dimension. If a component is "Deferred," its score reflects that. |

### 4.3 Score Capping

- Scores are **capped at 100%** per dimension. If BWFC implements MORE properties than the original Web Forms control (because Blazor adds capabilities), the property parity score is still 100%, not 120%.
- The overall health score is the **weighted sum**, capped at 100.
- Components with **no expected properties** (e.g., Literal, PlaceHolder) get 100% for Property Parity by definition (0/0 = complete).
- Components with **no expected events** get 100% for Event Parity by definition.

### 4.4 Handling Missing Data

| Situation | Behavior |
|-----------|----------|
| No reference baseline yet for a component | Property/Event Parity scores show "N/A" and are excluded from the weighted average. Remaining dimensions are re-weighted proportionally. |
| No tests exist | bUnit Tests score = 0%. This is honest — it surfaces testing gaps. |
| Component is deferred | Implementation Status = 0%, other dimensions scored normally (they'll be low). |
| Component is a stub (e.g., ScriptManager renders nothing) | Implementation Status = 50%. Property/Event scores based on what's declared. |

---

## 5. Component Discovery & Classification

### 5.1 Discovery Method: Assembly Reflection

**Approach:** Reflect over the compiled `BlazorWebFormsComponents.dll` assembly.

```csharp
// Pseudocode
var assembly = typeof(BaseWebFormsComponent).Assembly;
var allTypes = assembly.GetExportedTypes()
    .Where(t => t.IsClass && !t.IsAbstract)
    .Where(t => typeof(ComponentBase).IsAssignableFrom(t));
```

### 5.2 Classification Rules

For each discovered type, classify it:

| Classification | Rule | Dashboard? |
|---------------|------|-----------|
| **Web Forms Control** | Type inherits from `BaseWebFormsComponent` AND is registered in the **tracked components list** (§3.3) | ✅ Yes |
| **Style Container** | Type name ends with `Style` or `StyleContent` AND inherits from a style base | ❌ No |
| **Pager Settings** | Type name ends with `PagerSettings` | ❌ No |
| **Field Column** | Type inherits from a field base (BoundField, etc.) | ❌ No |
| **Base Class** | Type is abstract | ❌ No |
| **Infrastructure** | Everything else (event args, enums, utilities) | ❌ No |

**Why use a tracked components list instead of auto-classifying?** Because auto-classification is fragile. The first implementation tried to auto-detect "real" Web Forms controls and got it wrong for 14+ components. A curated list of ~55 component names is trivially maintainable and eliminates false positives/negatives.

### 5.3 The Tracked Components List

Maintain a `TrackedComponents` list (in code or JSON config) that maps BWFC class names to their expected Web Forms counterparts:

```json
{
  "Button": { "webFormsType": "System.Web.UI.WebControls.Button", "category": "Editor" },
  "GridView": { "webFormsType": "System.Web.UI.WebControls.GridView", "category": "Data" },
  "Repeater": { "webFormsType": "System.Web.UI.WebControls.Repeater", "category": "Data" }
}
```

This list is the **single source of truth** for what the dashboard tracks. Adding a new component = adding one line.

### 5.4 Handling the Class Hierarchy for Property Counting

Given a tracked component type `T`, the counting algorithm is:

```
1. Let stopTypes = { BaseWebFormsComponent, BaseStyledComponent, 
                     BaseDataBoundComponent, DataBoundComponent<> }
2. Let currentType = T
3. Let properties = []
4. Let events = []
5. While currentType is not null AND currentType is not in stopTypes:
     a. For each [Parameter] property declared on currentType (DeclaredOnly):
        - Skip if [Obsolete]
        - Skip if [CascadingParameter]
        - Skip if type is RenderFragment or RenderFragment<T>
        - Skip if name is "AdditionalAttributes" or "ChildContent" or "ChildComponents"
        - If type is EventCallback or EventCallback<T>: add to events
        - Else: add to properties
     b. currentType = currentType.BaseType
        (For generic bases, compare against the generic type definition)
6. Return (properties.Count, events.Count)
```

**Critical: When comparing against stopTypes for generic bases, use `GetGenericTypeDefinition()`:**

```csharp
bool IsStopType(Type t) {
    if (t.IsGenericType)
        t = t.GetGenericTypeDefinition();
    return stopTypes.Contains(t);
}
```

This prevents `DataBoundComponent<ItemType>` from being missed as a stop type because reflection sees `DataBoundComponent`1[ItemType]` instead of the open generic.

---

## 6. UX Requirements

### 6.1 Primary View: Category Summary Table

The default view shows all tracked components grouped by category, sorted by health score (ascending, so worst components surface first):

```
╔══════════════════════════════════════════════════════════════════════════════╗
║ Component Health Dashboard                                                  ║
║ 52 components tracked │ Average health: 87% │ Generated: 2026-07-25        ║
╠══════════════════════════════════════════════════════════════════════════════╣

Category: Editor Controls (25 components)                     Avg: 89%
┌──────────────┬───────┬─────────┬────────┬───────┬────────┬────────┬────────┐
│ Component    │ Score │ Props   │ Events │ Tests │ Docs   │ Sample │ Status │
├──────────────┼───────┼─────────┼────────┼───────┼────────┼────────┼────────┤
│ Calendar     │  72%  │ 12/18   │  3/5   │  ✅   │  ✅    │  ✅    │ Done   │
│ ImageMap     │  85%  │  6/8    │  1/1   │  ✅   │  ✅    │  ✅    │ Done   │
│ Button       │  95%  │  7/8    │  2/2   │  ✅   │  ✅    │  ✅    │ Done   │
│ TextBox      │ 100%  │  8/8    │  1/1   │  ✅   │  ✅    │  ✅    │ Done   │
└──────────────┴───────┴─────────┴────────┴───────┴────────┴────────┴────────┘
```

### 6.2 Key UX Principles

1. **Worst-first sorting** — Low-scoring components surface to the top. The dashboard exists to find gaps, not celebrate wins.
2. **Binary indicators for tests/docs/samples** — ✅ or ❌. Don't try to quantify "how much" test coverage — that's a rabbit hole. Existence is enough.
3. **Fraction display for props/events** — Show `implemented/expected` so users see both the numerator and denominator. "7/8" is meaningful; "88%" without context is not.
4. **Category grouping** — Match the categories from `status.md` (Editor, Data, Validation, Navigation, Login, Infrastructure).
5. **Color coding** — Green (≥90%), Yellow (70-89%), Red (<70%) for the Score column.

### 6.3 Filtering & Sorting

- Filter by category (dropdown)
- Filter by status (Complete / Stub / Deferred)
- Sort by: Score, Name, Category, Property Parity, Event Parity
- Show/hide deferred components (default: hidden)

### 6.4 Where It Lives

| Location | Purpose |
|----------|---------|
| **Sample app page** (`/dashboard`) | Primary interactive dashboard with live reflection data. Registered in ComponentCatalog under a "Diagnostics" category. |
| **MkDocs page** (`docs/dashboard.md`) | Static snapshot for documentation site. Generated from the same data model, committed as markdown. Updated via a script or CI step. |

The sample app page is the **authoritative** version. The MkDocs page is a convenience snapshot.

---

## 7. Technical Approach

### 7.1 Architecture

```
┌─────────────────────────────────┐
│  ComponentHealthService         │  ← Registered as singleton in DI
│  - Reflects over BWFC assembly  │
│  - Loads reference baselines    │
│  - Scans for tests/docs/samples │
│  - Computes health scores       │
└────────────┬────────────────────┘
             │
     ┌───────┴────────┐
     ▼                ▼
┌──────────┐   ┌────────────────┐
│ Dashboard │   │ Static Export  │
│ .razor    │   │ (markdown gen) │
│ page      │   │ for MkDocs     │
└──────────┘   └────────────────┘
```

### 7.2 Runtime Reflection (Live Dashboard)

The sample app uses **runtime reflection** over the loaded `BlazorWebFormsComponents` assembly. This is accurate and always up-to-date.

```csharp
public class ComponentHealthService
{
    private readonly ReferenceBaselines _baselines;  // loaded from JSON
    
    public IReadOnlyList<ComponentHealthReport> GetAllReports();
    public ComponentHealthReport GetReport(string componentName);
}

public record ComponentHealthReport(
    string Name,
    string Category,
    int ImplementedProperties,
    int ExpectedProperties,
    int ImplementedEvents,
    int ExpectedEvents,
    bool HasTests,
    bool HasDocumentation,
    bool HasSamplePage,
    ImplementationStatus Status,
    double HealthScore
);
```

### 7.3 Reference Baselines Storage

```
dev-docs/reference-baselines.json     ← Versioned baseline data (curated from MSDN)
```

The baselines JSON file is checked into the repo and loaded at runtime by `ComponentHealthService`. It is manually curated from [MSDN .NET Framework 4.8 API documentation](https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols). The JSON file has a `source` field that documents provenance.

### 7.4 Test/Docs/Sample Detection

| Dimension | Detection Method |
|-----------|-----------------|
| **bUnit Tests** | Scan the test project directory (`src/BlazorWebFormsComponents.Test/`) for `.razor` or `.cs` files whose name contains the component name. A component "has tests" if at least one matching file exists. |
| **Documentation** | Scan `docs/` directory for a `.md` file matching the component name (case-insensitive). Check both `docs/components/` and `docs/` root. |
| **Sample Page** | Check if the component is registered in `ComponentCatalog.Components` (the centralized registry in the sample app). |

### 7.5 Keeping Data Maintainable

When a new component is added to BWFC:

1. Add it to `status.md` (existing process)
2. Add it to `dev-docs/reference-baselines.json` with expected counts
3. The dashboard auto-discovers it via reflection and scores it

When a component gains new properties:
- The dashboard auto-detects them via reflection. No manual update needed for the "implemented" side.
- If the "expected" baseline needs updating, edit `reference-baselines.json`.

---

## 8. Known Pitfalls — Lessons from the Failed First Attempt

Every item below is a **real bug** from the reverted dashboard code. This section exists so the next implementer doesn't repeat these mistakes.

### Pitfall 1: Generic Type Name Mismatch

**What happened:** Reflection returns `GridView`1` for `GridView<T>`. The reference baselines used the key `"GridView"`. The lookup failed, so 14+ data-bound components showed "not implemented."

**Fix:** Always strip the generic arity suffix before looking up reference data (§2.6).

### Pitfall 2: Inherited Properties Inflated Counts

**What happened:** Using `GetProperties(BindingFlags.Public | BindingFlags.Instance)` without `DeclaredOnly` pulled ALL inherited [Parameter] properties from BaseWebFormsComponent (21), BaseStyledComponent (9), etc. Button showed "40 implemented" instead of ~7.

**Fix:** Walk the hierarchy manually with DeclaredOnly, stopping at the defined stop-types (§5.4).

### Pitfall 3: DeclaredOnly Went Too Far

**What happened:** After switching to `DeclaredOnly` on just the leaf type, properties from intermediate bases like `ButtonBaseComponent` were missed. Button showed "1 implemented" (only `UseSubmitBehavior`).

**Fix:** Walk the hierarchy from leaf to stop-type, collecting DeclaredOnly properties at each level (§5.4).

### Pitfall 4: Events Double-Counted

**What happened:** `EventCallback<T>` parameters were counted as both "properties" AND "events," inflating both columns.

**Fix:** EventCallback parameters go in the events column ONLY (§2.3).

### Pitfall 5: RenderFragment Templates Inflated Property Counts

**What happened:** GridView's 12 RenderFragment parameters (HeaderStyleContent, RowStyleContent, ItemTemplate, etc.) were counted as "properties," making GridView show 30+ implemented properties against a baseline of ~18.

**Fix:** RenderFragment and RenderFragment<T> parameters are excluded from all counts (§2.4).

### Pitfall 6: Reference Baselines Were Guesses

**What happened:** The "expected" property/event counts were manually estimated without consulting actual .NET Framework 4.8 metadata. Many were wrong, producing misleading percentages.

**Fix:** Derive baselines from actual .NET Fx 4.8 type reflection or verified MSDN documentation. Document the source in the baselines file (§3.2).

### Pitfall 7: Test & Integration Scores Hardcoded to Zero

**What happened:** The test/integration dimensions always returned 0 because no detection logic was implemented. This dragged the max achievable health score to ~80%.

**Fix:** Implement actual file-scan detection for tests, docs, and samples (§7.4). Binary scoring (has/doesn't have) is simple and correct.

### Pitfall 8: Arbitrary Score Weights

**What happened:** The weight split (30/15/15/5/15/10/10) had no documented rationale. When tests scored 0, the impact was invisible because test weight was low.

**Fix:** Document weight rationale explicitly (§4.2). Tests get 20% because untested components are unreliable.

### Pitfall 9: AdditionalAttributes and Obsolete Params Counted

**What happened:** `AdditionalAttributes` (the catch-all HTML attribute dictionary) and `[Obsolete]` compatibility shims like `runat` were counted as implemented properties, inflating counts.

**Fix:** Explicitly exclude infrastructure and obsolete parameters (§2.5).

### Pitfall 10: No Distinction Between "Not Implemented" and "No Baseline"

**What happened:** Components without reference baselines showed 0% property parity, the same as components that genuinely had no properties implemented. This made it impossible to distinguish "we haven't measured this yet" from "this is broken."

**Fix:** Missing baselines show "N/A" and are excluded from the weighted average. The dashboard clearly distinguishes "not yet baselined" from "failing" (§4.4).

---

## 9. Implementation Phases

### Phase 1: Reference Baselines (prerequisite)

- Build or manually create `dev-docs/reference-baselines.json` for all 54 tracked components
- Verify counts against .NET Framework 4.8 API documentation
- This MUST be done before any dashboard code is written

### Phase 2: Core Health Service

- Implement `ComponentHealthService` with reflection-based property/event counting
- Implement the hierarchy-walking algorithm from §5.4
- Implement test/docs/sample detection from §7.4
- Unit test the counting logic against known components (Button, GridView, Repeater as test fixtures)

### Phase 3: Dashboard UI

- Build the Razor page at `/dashboard` in the sample app
- Register it in ComponentCatalog under "Diagnostics"
- Implement category grouping, sorting, filtering
- Add color coding for health scores

### Phase 4: Static Export

- Build a markdown generator that produces the MkDocs-compatible dashboard page
- Integrate into the build/CI pipeline if desired

---

## 10. Acceptance Criteria

The dashboard is considered correct when:

1. **Button** shows ~7 properties, 2 events (not 40+ or 1)
2. **GridView** shows ~18 properties, ~10 events (not 30+ or 0)
3. **Repeater** shows 0 properties, 0 events (all its [Parameter]s are RenderFragment templates)
4. **No generic component** shows as "not found" due to backtick name mismatch
5. **No base class properties** (ID, CssClass, BackColor, etc.) inflate any component's count
6. **No EventCallback** appears in both the properties AND events columns
7. **No RenderFragment** appears in any count column
8. **Missing baselines** show "N/A," not 0%
9. **All completed components except Login controls** show tests=✅ (797+ bUnit tests exist). Login controls (ChangePassword, CreateUserWizard, Login, LoginName, LoginStatus, LoginView, PasswordRecovery) show tests=❌ until bUnit coverage is added for that category.
10. **Deferred components** (Substitution, Xml) show Status=Deferred with honest scores

---

## Appendix A: BWFC Base Class Parameter Inventory

For reference, here are ALL [Parameter] properties on each base class that must be EXCLUDED from component-specific counts:

### BaseWebFormsComponent (21 parameters)

| Parameter | Type | Category |
|-----------|------|----------|
| ID | string | Web Forms base |
| Enabled | bool | Web Forms base |
| Visible | bool | Web Forms base |
| TabIndex | short | Web Forms base |
| AccessKey | string | Web Forms base |
| ToolTip | string | Web Forms base |
| EnableTheming | bool | Web Forms base |
| SkinID | string | Web Forms base |
| ClientIDMode | EnumParameter<ClientIDMode> | Web Forms base |
| EnableViewState | bool | **Obsolete** |
| runat | string | **Obsolete** |
| DataKeys | string | **Obsolete** |
| ItemPlaceholderID | string | **Obsolete** |
| OnDataBinding | EventCallback<EventArgs> | Base event |
| OnInit | EventCallback<EventArgs> | Base lifecycle |
| OnLoad | EventCallback<EventArgs> | Base lifecycle |
| OnPreRender | EventCallback<EventArgs> | Base lifecycle |
| OnUnload | EventCallback<EventArgs> | Base lifecycle |
| OnDisposed | EventCallback<EventArgs> | Base lifecycle |
| AdditionalAttributes | Dictionary<string,object> | **Blazor infrastructure** |
| ChildComponents | RenderFragment | **Blazor infrastructure** |

### BaseStyledComponent (9 parameters)

| Parameter | Type |
|-----------|------|
| BackColor | WebColor |
| BorderColor | WebColor |
| BorderStyle | EnumParameter<BorderStyle> |
| BorderWidth | Unit |
| CssClass | string |
| ForeColor | WebColor |
| Height | Unit |
| Width | Unit |
| Font | FontInfo |

### BaseDataBoundComponent (3 parameters)

| Parameter | Type |
|-----------|------|
| DataSource | object |
| DataSourceID | string (**Obsolete**) |
| OnDataBound | EventCallback<EventArgs> |

### DataBoundComponent<T> (3 parameters)

| Parameter | Type |
|-----------|------|
| DataMember | string |
| SelectMethod | SelectHandler<T> |
| Items | IEnumerable<T> |

**Total base class parameters: 36** — All excluded from component-specific counts.

---

## Appendix B: Component Classification Quick Reference

| Classification Test | Result |
|--------------------|--------|
| Name ends with `Style` or `StyleContent`? | → Style container (exclude) |
| Name ends with `PagerSettings`? | → Pager settings (exclude) |
| Is abstract? | → Base class (exclude) |
| Name is in TrackedComponents list? | → Dashboard component (include) |
| Inherits BaseWebFormsComponent but not in list? | → Infrastructure component (exclude) |
| Is an enum, event args, or static class? | → Supporting type (exclude) |
