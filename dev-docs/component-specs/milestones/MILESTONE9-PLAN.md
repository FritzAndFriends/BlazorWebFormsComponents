# Milestone 9 — Migration Fidelity & Hardening

**Created:** 2026-02-25
**Author:** Forge (Lead / Web Forms Reviewer)
**Branch:** `milestone9/migration-fidelity`
**Baseline:** dev (post-v0.14.1, 51/53 components, 1200+ tests)

---

## Goals

Milestones 6–8 brought us from ~45% to 96% component completion (51/53) and shipped 1200+ tests. The remaining work is not about new components — it's about **migration fidelity**: making the existing 51 components behave exactly like their Web Forms originals so that migrating developers can paste markup and have it work.

This milestone targets the highest-leverage remaining gaps: a base class fix that sweeps across 28+ controls, a data-corruption bug, and stale branch cleanup. All changes are low-risk and high-impact.

### Theme: "Paste and It Works"

**Priority targets (by blast radius):**

| Change | Controls Affected | Risk | Impact |
|--------|-------------------|------|--------|
| ToolTip → BaseStyledComponent | 28+ controls gain ToolTip | Low | High — single most impactful remaining gap |
| ValidationSummary comma-split fix | All validation scenarios | Low | High — data corruption |
| SkinID type fix (bool→string) | All 51 components | Low | Medium — migration correctness |
| Stale branch cleanup | N/A | None | Housekeeping |
| Documentation gap closure | 8 controls | None | Medium — migration guides |

### Scoping Rules (carried forward)

- ✅ Substitution and Xml remain intentionally deferred
- ✅ Chart advanced properties remain deferred (canvas architecture)
- ✅ DataSourceID / model binding methods remain N/A
- ✅ Focus() method remains deferred (use `ElementReference.FocusAsync()`)
- ✅ Docs and samples must ship in the same milestone as the features they cover

---

## Pre-Milestone Verification

Before starting, the following gaps from prior audits were verified against the current `dev` branch:

### ✅ ALREADY FIXED (no action needed)

| Original Gap | Status | Fixed In |
|-------------|--------|----------|
| AccessKey on BaseWebFormsComponent | ✅ Done | M6 — on `BaseWebFormsComponent` line 118 |
| Image → BaseStyledComponent | ✅ Done | M6/M7 — `Image.razor.cs` inherits `BaseStyledComponent` |
| Label → BaseStyledComponent + AssociatedControlID | ✅ Done | M7 — `Label.razor.cs` inherits `BaseStyledComponent`, `AssociatedControlID` parameter present |
| Validation Display property | ✅ Done | M6 — `ValidatorDisplay` enum with None/Static/Dynamic on `BaseValidator` |
| HyperLink NavigateUrl vs NavigationUrl | ✅ Done | M6 — Property is `NavigateUrl`, obsolete `NavigationUrl` alias preserved |
| DataBoundComponent chain → BaseStyledComponent | ✅ Done | M7 — `BaseDataBoundComponent` inherits `BaseStyledComponent` |
| bUnit test migration (beta → 2.x) | ✅ Not needed | Tests already use modern bUnit API (no TestComponentBase/Fixture/SnapshotTest) |

### ❌ STILL OPEN (targeted in this milestone)

| Gap | Severity | Details |
|-----|----------|---------|
| ToolTip not on BaseStyledComponent | P0 | Scattered across 8 individual components; 28+ styled controls lack it entirely |
| ValidationSummary comma-split bug | P1 | `AspNetValidationSummary.razor.cs` line 27: `.Split(',')[1]` corrupts messages containing commas |
| SkinID is `bool` instead of `string` | P2 | `BaseWebFormsComponent.cs` line 101: `public bool SkinID` — Web Forms SkinID is `string` |

---

## Work Items

### P0 — ToolTip Base Class Fix (4 WIs, ~28 gap closures)

This is the single highest-leverage change remaining. Web Forms `WebControl.ToolTip` renders as `title="..."` on the HTML element. Currently 8 components implement ToolTip individually; 28+ styled controls are missing it. Moving ToolTip to `BaseStyledComponent` fixes all of them in one change and removes duplication from the 8 that have it.

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-01 | **ToolTip → BaseStyledComponent** | Add `[Parameter] public string ToolTip { get; set; }` to `BaseStyledComponent`. Remove the individual `ToolTip` declarations from the 8 components that define it locally (Button, Calendar, DataList, FileUpload, HyperLink, Image, ImageButton, ImageMap). Ensure templates that render `title="@ToolTip"` continue to work. For components that don't yet render ToolTip in their template, add `title="@ToolTip"` to the outermost HTML element. **Note:** ChartSeries.ToolTip, DataPoint.ToolTip, MenuItem.ToolTip, TreeNode.ToolTip, MenuItemBinding.ToolTipField, and TreeNodeBinding.ToolTipField are sub-component properties — do NOT remove these (they are semantically different from the control-level ToolTip). | Cyclops | — | M | P0 |
| WI-02 | **ToolTip base class tests** | Expand the existing `ToolTipTests.razor` test file. Add tests for controls that newly gain ToolTip: Label, TextBox, CheckBox, RadioButton, Panel, Table, DropDownList, ListBox, LinkButton. Verify `title="myTooltip"` renders in the HTML output. Verify that removing the individual ToolTip from Button/Image/HyperLink etc. doesn't regress (they should now inherit from base). | Rogue | WI-01 | M | P0 |
| WI-03 | **ToolTip rendering in templates** | Audit all 51 component `.razor` templates. For any component that inherits BaseStyledComponent (directly or via DataBoundComponent chain) but does NOT render `title="@ToolTip"` on its outermost element, add it. Components that render complex HTML (GridView renders `<table>`, Login renders `<table>`) should render ToolTip on the outermost container element, matching Web Forms behavior. | Cyclops | WI-01 | M | P0 |
| WI-04 | **ToolTip docs update** | Update component documentation to note ToolTip availability on all styled controls. Add a migration note to the main migration guide explaining that `ToolTip` is now universally supported. | Beast | WI-01 | S | P0 |

### P1 — Bug Fixes (3 WIs, 2 bug fixes)

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-05 | **Fix ValidationSummary comma-split bug** | In `AspNetValidationSummary.razor.cs` line 27, `FilteredMessages.Select(x => x.Split('\x1F')[0].Split(',')[1])` splits on comma to extract the error message. If the error message itself contains a comma (e.g., "Please enter first name, last name"), only the text after the first comma is shown. **Fix:** Use `IndexOf(',')` + `Substring()` instead of `Split(',')[1]` — take everything after the FIRST comma only. Also verify the corresponding storage in `BaseValidator` uses the same delimiter convention. | Cyclops | — | S | P1 |
| WI-06 | **ValidationSummary comma-split tests** | Add bUnit tests: (1) error message containing one comma renders full message, (2) error message containing multiple commas renders full message, (3) error message with no commas renders correctly, (4) empty error message renders correctly. | Rogue | WI-05 | S | P1 |
| WI-07 | **Fix SkinID type: bool → string** | In `BaseWebFormsComponent.cs` line 101, change `public bool SkinID { get; set; }` to `public string SkinID { get; set; }`. Web Forms `SkinID` is a string (the name of the skin to apply). Keep the `[Obsolete]` attribute. This is a breaking change for any code that sets `SkinID="true"` — but since the property does nothing, impact is minimal. Update any tests that reference SkinID. | Cyclops | — | XS | P1 |

### P2 — Housekeeping (5 WIs)

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-08 | **Clean up stale local branches** | Delete merged/stale local branches: `copilot/create-basepage-for-services`, `copilot/create-calendar-component`, `copilot/fix-334-fileupload-component`, `copilot/fix-336-imagemap-component`, `feature-phase2`, `fix-sample-container`, `fix/deployment-workflows`, `sprint-2/editor-login-controls`, `sprint3/detailsview-passwordrecovery`, `v0.13`. Keep `dev`, `main`, `milestone8/release-readiness` (tag reference). Coordinate with repo owner on remote branch cleanup. | Cyclops | — | XS | P2 |
| WI-09 | **Documentation gap audit** | Audit `docs/` directory against the features added in M6–M8. Identify any controls whose docs don't cover features added in M6–M8 (GridView selection/styles/display, TreeView selection/expand, Menu selection/events, FormView events/styles, DetailsView styles/caption, DataGrid styles/paging, validators ControlToValidate dual-path, PagerSettings). Produce a list of doc pages that need updates. | Beast | — | M | P2 |
| WI-10 | **Update stale planning-docs audit files** | The individual control audit files in `planning-docs/` (e.g., `GridView.md`, `Menu.md`, `TreeView.md`) are stale — they reflect pre-M6 state. Either update them to reflect the current state post-M8 or mark them as historical with a header noting they are pre-M6 snapshots. | Beast | — | M | P2 |
| WI-11 | **Integration test coverage for M7 features** | Review integration test coverage for features added in M7 (GridView selection, TreeView expand/collapse, Menu selection, FormView events, DetailsView styles). Identify any gaps and add missing Playwright smoke/interaction tests. | Colossus | — | M | P2 |
| WI-12 | **Sample site navigation audit** | Verify all sample pages are reachable from the AfterBlazorServerSide navigation. Check that new M7/M8 sample pages (GridView Selection, GridView DisplayProperties, TreeView Selection, TreeView ExpandCollapse, Menu Selection, FormView Events/Styles, DetailsView Styles/Caption, ListView CrudOperations) are linked in NavMenu.razor or the sample index. | Jubilee | — | S | P2 |

---

## Summary

| Priority | Work Items | Estimated Gap Closures | Theme |
|----------|-----------|----------------------|-------|
| P0 | 4 | ~28 | ToolTip base class fix |
| P1 | 3 | 2 bugs | ValidationSummary + SkinID fixes |
| P2 | 5 | 0 (housekeeping) | Cleanup + docs + test coverage |
| **Total** | **12** | **~30** | |

### Agent Assignments

| Agent | Work Items | Load |
|-------|-----------|------|
| Cyclops | WI-01, WI-03, WI-05, WI-07, WI-08 | 5 WIs (M + M + S + XS + XS) |
| Rogue | WI-02, WI-06 | 2 WIs (M + S) |
| Beast | WI-04, WI-09, WI-10 | 3 WIs (S + M + M) |
| Colossus | WI-11 | 1 WI (M) |
| Jubilee | WI-12 | 1 WI (S) |

### Dependencies

```
WI-01 ──→ WI-02 (tests need base class change)
WI-01 ──→ WI-03 (template audit after base class change)
WI-01 ──→ WI-04 (docs after implementation)
WI-05 ──→ WI-06 (tests need bug fix)
```

All other work items are independent and can proceed in parallel.

### Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|-----------|
| ToolTip base class change breaks existing component behavior | Low | Medium | 8 components already use ToolTip individually — removal is mechanical. Run full test suite before merge. |
| ValidationSummary fix introduces new parsing bugs | Low | High | Targeted fix (IndexOf vs Split). Test with edge cases. |
| SkinID type change breaks consumer code | Very Low | Low | Property is `[Obsolete]` and does nothing. |
| Stale branch deletion removes needed work | Low | Low | Only delete branches whose work is already on dev. |

### Exit Criteria

1. All 1200+ existing tests pass
2. ToolTip renders on all BaseStyledComponent-derived controls
3. ValidationSummary correctly handles commas in error messages
4. SkinID parameter accepts string values
5. No stale local branches remain
6. Docs and samples ship with any feature changes

---

## Post-M9 Outlook

After M9, the remaining meaningful gaps are:

1. **DataBoundStyledComponent<T>** — Chart currently inherits BaseStyledComponent directly; a proper intermediate class could benefit future data+style controls. Low priority since only Chart uses it.
2. **ListView CRUD events** — 16 missing events. Large effort, deferred since M7.
3. **Menu level styles** — Dynamic/Static menu level style sub-components. Medium effort.
4. **Panel.BackImageUrl** — Single missing property. XS effort.
5. **PasswordRecovery → BaseStyledComponent** — Login controls pattern alignment.
6. **LoginView → BaseStyledComponent** — Currently still on BaseWebFormsComponent.

These can be scoped into a Milestone 10 if the project continues toward 1.0.
