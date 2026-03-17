# Component Health Dashboard

## Overview

The **Component Health Dashboard** is a diagnostic tool that measures how completely each Blazor component in the BlazorWebFormsComponents library reproduces the behavior of its original ASP.NET Web Forms control.

It answers the question: *"For each Web Forms control we've re-created, how close is the Blazor version to the original?"*

The dashboard tracks **52 targeted Web Forms controls** across six dimensions:
- Properties implemented vs. expected
- Events implemented vs. expected  
- Test coverage (bUnit tests)
- Documentation completeness
- Sample page availability
- Implementation status (Complete/Stub/Deferred)

A perfect score (100%) means the Blazor component is a drop-in replacement for the Web Forms original.

## How to Access

The Component Health Dashboard lives at **/dashboard** in the sample app. When you run the sample application locally, you can navigate to the dashboard to view real-time health metrics for all tracked components.

The dashboard is registered in the sample app's component catalog under the "Diagnostics" category.

## Scoring Model

The overall health score is a **weighted average** of six dimensions:

| Dimension | Weight | What It Measures |
|-----------|--------|------------------|
| **Property Parity** | 30% | `implemented properties / expected properties`. Shows how many of the original Web Forms properties are implemented. |
| **Event Parity** | 15% | `implemented events / expected events`. Shows how many of the original Web Forms events are implemented. |
| **Has bUnit Tests** | 20% | Binary: 0% or 100%. Does the component have at least one bUnit test file? Untested components are unreliable. |
| **Has Documentation** | 15% | Binary: 0% or 100%. Does the component have an MkDocs page? Developers can't use what they can't find. |
| **Has Sample Page** | 10% | Binary: 0% or 100%. Is the component registered in the sample app's ComponentCatalog? |
| **Implementation Status** | 10% | Complete = 100%, Stub = 50%, Deferred = 0%. A sanity-check dimension. |

**Score Cap:** Scores are capped at 100% per dimension. If BWFC implements MORE properties than the original Web Forms control, the score remains 100%, not 120%.

## Reading the Dashboard

### Color Coding

The dashboard uses a three-tier color scheme for the health score:

- 🟢 **Green (≥90%)** — Component is highly complete and reliable for migration.
- 🟡 **Yellow (70-89%)** — Component has good coverage but may have some gaps.
- 🔴 **Red (<70%)** — Component needs more work before recommending it for production migration.

### Property and Event Display

Properties and events are displayed as **fractions**: `7/8`

- **Numerator** = number of properties/events actually implemented in the Blazor component
- **Denominator** = number of properties/events the original Web Forms control has

For example, `7/8` means 7 of 8 expected properties are implemented (87.5% parity).

### Not Yet Baselined (N/A)

Some components may show **N/A** for property or event parity. This means:

- A reference baseline for the Web Forms control hasn't been curated yet
- The expected property/event count hasn't been documented in `dev-docs/reference-baselines.json`

When a baseline is missing, that dimension is excluded from the weighted score calculation, and remaining dimensions are re-weighted proportionally.

### Binary Indicators

Tests, Docs, and Samples use simple checkmarks:

- ✅ — Feature exists and is available
- ❌ — Feature does not exist

There is no in-between; the metric is binary. A component either has tests or it doesn't.

## What Counts (and What Doesn't)

### Component-Specific Properties Only

Not all properties are created equal. The dashboard counts only **component-specific** properties, not base class properties that every control inherits.

For example:

- **Counted:** `Button.UseSubmitBehavior` (unique to Button), `GridView.AllowSorting` (unique to GridView)
- **Not counted:** `ID`, `Enabled`, `Visible`, `BackColor`, `ForeColor`, `CssClass` (inherited by ALL components)

**Inheritance rule:** Walk the component's type hierarchy and count parameters declared **after** (more specific than) these base classes:
- `BaseWebFormsComponent` — Base properties every control shares
- `BaseStyledComponent` — Style properties shared by styled controls
- `BaseDataBoundComponent` — Data-binding properties shared by data-bound controls

### Events vs. EventCallbacks

In the component code, events are declared as **[Parameter] public EventCallback<T> OnEventName** properties. The dashboard recognizes `EventCallback` parameters as events, not properties.

- **Counted as events:** `OnClick`, `OnCommand`, `OnDataBound` (EventCallback parameters)
- **Not counted as properties:** These same EventCallback parameters are excluded from the property count

This prevents double-counting and keeps the metrics honest.

### Excluded from All Counts

The following parameters are **always excluded** from both property and event counts:

| Parameter Type | Why Excluded |
|---|---|
| `RenderFragment` / `RenderFragment<T>` | Blazor template infrastructure; no direct Web Forms equivalent. Examples: `ItemTemplate`, `ChildContent`, `HeaderStyleContent` |
| `AdditionalAttributes` | Catch-all HTML attribute pass-through; Blazor-specific infrastructure |
| `[CascadingParameter]` | Cascading values (Theme, Parent, ValidationCoordinator); framework infrastructure |
| `[Inject]` services | Dependency injection; not a control property |
| `[Obsolete]` parameters | Compatibility shims (e.g., `runat`, `EnableViewState`); deprecated |

**Rationale:** These are Blazor infrastructure, not Web Forms properties. Counting them would inflate the "implemented" count and make comparisons to the Web Forms original meaningless.

## Maintaining Baselines

### When Adding a New Component

When you add a new Blazor component to the library:

1. Add the component name to `status.md` (existing process)
2. Add an entry to `dev-docs/reference-baselines.json` with the expected property and event counts

The baseline file format looks like this:

```json
{
  "Button": {
    "namespace": "System.Web.UI.WebControls",
    "expectedProperties": 8,
    "expectedEvents": 2,
    "propertyList": ["Text", "CausesValidation", "ValidationGroup", "CommandName", "CommandArgument", "PostBackUrl", "OnClientClick", "UseSubmitBehavior"],
    "eventList": ["Click", "Command"],
    "notes": "Properties from ButtonBase class included"
  }
}
```

**Reference baseline values MUST come from the .NET Framework 4.8 type metadata**, not guesses. The [Microsoft .NET Framework 4.8 API documentation](https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols) is the authoritative source.

Once you add a baseline entry, the dashboard will automatically discover your component via reflection and score it.

### When Counts Seem Wrong

If you believe a component's implemented or expected property/event count is incorrect:

1. **Check the counting rules** (§2 of the PRD) for the specific component
2. **Verify that base class properties are properly excluded**
3. **For expected counts**, check the baseline in `dev-docs/reference-baselines.json` against the [Microsoft API documentation](https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols)
4. For implemented counts, inspect the component's Razor/C# source code to ensure properties are marked with `[Parameter]` and follow the counting rules

If you find an actual bug in the counting logic, please report it as an issue or reach out to the team.

## Glossary

| Term | Definition |
|------|-----------|
| **Expected** | The number of properties/events the original .NET Framework 4.8 Web Forms control had |
| **Implemented** | The number of properties/events the BWFC Blazor component currently has |
| **Parity** | The ratio of implemented to expected (e.g., 7/8 = 87.5% parity) |
| **Baseline** | The curated reference data that defines "expected" counts for each component |
| **Component-specific** | Properties unique to a control, not inherited from a base class |
| **Binary indicator** | A yes/no metric (✅/❌); no partial credit |
| **Health score** | The weighted average of all six dimensions (capped at 100%) |

## Next Steps

- **To see the live dashboard:** Run the sample app and navigate to `/dashboard`
- **To improve a component's health:** Implement missing properties, add tests, or update documentation
- **To add a new component:** Update `status.md`, add a baseline to `reference-baselines.json`, and the dashboard will auto-discover it
- **To understand component-specific property rules:** See the [PRD §2](https://github.com/FritzAndFriends/BlazorWebFormsComponents/blob/dev/dev-docs/prd-component-health-dashboard.md#2-data-model--property--event-counting)
