# Milestone 6 — Feature Gap Closure Work Plan

**Created:** 2026-02-14
**Author:** Forge (Lead / Web Forms Reviewer)
**Branch:** `milestone6/feature-implementation`
**Baseline:** dev (post-PR #341 M4/M5 merge + PR #340 sprint3 merge)

---

## Goals

Close the highest-impact feature gaps identified in the 53-control audit (SUMMARY.md). Milestone 6 focuses on **base class fixes that sweep across many controls** (P0) and **individual control improvements for the weakest data controls** (P1). Substitution and Xml remain deferred.

### Resolved Pre-Conditions

- ✅ `sprint3/detailsview-passwordrecovery` merged to dev via PR #340 — DetailsView and PasswordRecovery are available
- ✅ Chart component + audit merged via PR #341
- ⚠️ PasswordRecovery audit doc (`planning-docs/PasswordRecovery.md`) shows 0% because audit ran before merge — needs re-audit (WI-43)

---

## Work Items

### P0 — Base Class Fixes (~180 gaps closed across 7 changes)

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-01 | Add `AccessKey` to `BaseWebFormsComponent` | Add `[Parameter] public string AccessKey { get; set; }` to `src/BlazorWebFormsComponents/BaseWebFormsComponent.cs`. Render as `accesskey` HTML attribute. This property is inherited by all 53 controls. ~40 gaps closed. | Cyclops | — | S | P0 |
| WI-02 | Tests for `AccessKey` on base class | Add bUnit tests in a new `BaseWebFormsComponentAccessKeyTests` file verifying `accesskey` attribute renders on representative controls (Button, Label, GridView). Test empty string = no attribute. | Rogue | WI-01 | S | P0 |
| WI-03 | Verify `AccessKey` in samples | Spot-check 3 existing sample pages (Button, Calendar, GridView) to confirm no rendering regressions from the new attribute. Add one `AccessKey` usage example to Button sample. | Jubilee | WI-01 | S | P0 |
| WI-04 | Add `ToolTip` to `BaseWebFormsComponent` | Add `[Parameter] public string ToolTip { get; set; }` to `src/BlazorWebFormsComponents/BaseWebFormsComponent.cs`. Render as `title` HTML attribute. Controls that already implement ToolTip directly (Button, Calendar, DataList, FileUpload, HyperLink, Image, ImageButton, ImageMap) must be checked for conflicts — remove duplicates, let base class handle it. ~35 gaps closed. | Cyclops | — | M | P0 |
| WI-05 | Tests for `ToolTip` on base class | bUnit tests verifying `title` attribute on representative controls. Test that controls with existing ToolTip still work (no double-render). Test empty = no attribute. | Rogue | WI-04 | S | P0 |
| WI-06 | Verify `ToolTip` in samples | Confirm no regressions on controls that already had ToolTip (Calendar, Button, Image). Add ToolTip example to one sample page. | Jubilee | WI-04 | S | P0 |
| WI-07 | Make `DataBoundComponent<T>` inherit `BaseStyledComponent` | Change `src/BlazorWebFormsComponents/DataBinding/DataBoundComponent.cs` (or its parent `BaseDataBoundComponent`) to inherit `BaseStyledComponent` instead of `BaseWebFormsComponent`. Verify compilation of all data controls: DataGrid, DetailsView, FormView, GridView, ListView. DataList already has IStyle — verify no conflict. Remove any duplicate CssClass declarations on controls that added it directly (DetailsView, GridView). ~70 gaps closed across 5+ controls. | Cyclops | — | M | P0 |
| WI-08 | Tests for `DataBoundComponent` style properties | bUnit tests on GridView and FormView verifying BackColor, CssClass, ForeColor, Width, Height render correctly after inheritance change. Test that DataList's explicit IStyle still works. | Rogue | WI-07 | M | P0 |
| WI-09 | Verify data control samples with styles | Check GridView, FormView, DetailsView, ListView sample pages for rendering regressions. Add one style example (CssClass + BackColor) to GridView sample. | Jubilee | WI-07 | S | P0 |
| WI-10 | Add `Display` property to `BaseValidator` | Add `ValidatorDisplay` enum (`None=0, Static=1, Dynamic=2`) in `src/BlazorWebFormsComponents/Enums/`. Add `[Parameter] public ValidatorDisplay Display` to `src/BlazorWebFormsComponents/Validations/BaseValidator.razor.cs`. Default: `Static`. When `None`, render `style="display:none"`. When `Dynamic`, render `style="display:none"` when valid, visible when invalid. When `Static`, render `style="visibility:hidden"` when valid (reserves space). 6 gaps closed. | Cyclops | — | S | P0 |
| WI-11 | Tests for `Display` on validators | bUnit tests on RequiredFieldValidator for each Display mode. Verify `style` attribute output for valid/invalid states in Static vs Dynamic vs None modes. | Rogue | WI-10 | S | P0 |
| WI-12 | Verify validator samples with Display | Update one validator sample page to demonstrate `Display="Dynamic"` usage. | Jubilee | WI-10 | S | P0 |
| WI-13 | Add `SetFocusOnError` to `BaseValidator` | Add `[Parameter] public bool SetFocusOnError { get; set; }` to `BaseValidator.razor.cs`. When true and validation fails, call `JSRuntime.InvokeVoidAsync("eval", "document.getElementById('X').focus()")` targeting the validated control. 6 gaps closed. | Cyclops | — | S | P0 |
| WI-14 | Tests for `SetFocusOnError` | bUnit tests verifying the parameter exists and is false by default. JS interop mock test for focus call when validation fails with `SetFocusOnError=true`. | Rogue | WI-13 | S | P0 |
| WI-15 | Change `Image` base class to `BaseStyledComponent` | Change `src/BlazorWebFormsComponents/Image.razor.cs` to inherit `BaseStyledComponent` instead of `BaseWebFormsComponent`. Verify `IImageComponent` still works. Update `.razor` to use `CombinedStyle` for style rendering. 11 gaps closed. Aligns Image with ImageMap (already fixed). | Cyclops | — | S | P0 |
| WI-16 | Tests for `Image` style properties | bUnit tests for BackColor, CssClass, Font, ForeColor, Height, Width, BorderColor, BorderStyle, BorderWidth on Image component. | Rogue | WI-15 | S | P0 |
| WI-17 | Change `Label` base class to `BaseStyledComponent` | Change `src/BlazorWebFormsComponents/Label.razor.cs` to inherit `BaseStyledComponent` instead of `BaseWebFormsComponent`. Update `.razor` to use `CombinedStyle`. 11 gaps closed. | Cyclops | — | S | P0 |
| WI-18 | Tests for `Label` style properties | bUnit tests for style properties on Label (same pattern as WI-16). | Rogue | WI-17 | S | P0 |

### P1 — Individual Control Improvements (high migration impact)

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-19 | GridView: Add paging | Add `AllowPaging`, `PageSize` (default 10), `PageIndex` (0-based) parameters to `src/BlazorWebFormsComponents/GridView.razor.cs`. Add `PageIndexChanging`/`PageIndexChanged` events with `GridViewPageEventArgs`. Render pager row with numeric links matching DetailsView's existing pager pattern. Add `PagerStyle` (TableItemStyle). | Cyclops | WI-07 | L | P1 |
| WI-20 | GridView paging tests | bUnit tests: paging renders correct page of items, PageIndexChanged fires, PageSize respected, boundary conditions (page 0, last page, empty data). | Rogue | WI-19 | M | P1 |
| WI-21 | GridView paging sample | New sample page `Components/Pages/ControlSamples/GridView/Paging.razor` demonstrating paged GridView with 50+ items. | Jubilee | WI-19 | S | P1 |
| WI-22 | GridView: Add sorting | Add `AllowSorting` (bool), `SortDirection` (`SortDirection` enum: Ascending=0, Descending=1), `SortExpression` (string) parameters. Add `Sorting`/`Sorted` events with `GridViewSortEventArgs`. Render header cells as clickable `<a>` links when sorting enabled. | Cyclops | WI-07 | M | P1 |
| WI-23 | GridView sorting tests | bUnit tests: sort header renders as links, Sorting event fires with correct expression, toggle ascending/descending. | Rogue | WI-22 | M | P1 |
| WI-24 | GridView sorting sample | New sample page `Components/Pages/ControlSamples/GridView/Sorting.razor` with sortable columns. | Jubilee | WI-22 | S | P1 |
| WI-25 | GridView: Add row editing | Add `EditIndex` (int, default -1), `EditRowStyle` (TableItemStyle). Add events: `RowEditing` (`GridViewEditEventArgs`), `RowUpdating` (`GridViewUpdateEventArgs`), `RowDeleting` (`GridViewDeleteEventArgs`), `RowCancelingEdit` (`GridViewCancelEditEventArgs`). Render Edit/Update/Cancel/Delete command links in edit mode. | Cyclops | WI-07 | M | P1 |
| WI-26 | GridView row editing tests | bUnit tests: EditIndex renders edit template, RowEditing/RowUpdating/RowDeleting events fire, CancelEdit resets EditIndex. | Rogue | WI-25 | M | P1 |
| WI-27 | GridView row editing sample | New sample page `Components/Pages/ControlSamples/GridView/InlineEditing.razor`. | Jubilee | WI-25 | S | P1 |
| WI-28 | Calendar: style strings → TableItemStyle | Convert 9 style properties (DayStyle, TitleStyle, DayHeaderStyle, TodayDayStyle, SelectedDayStyle, OtherMonthDayStyle, WeekendDayStyle, NextPrevStyle, SelectorStyle) from `string` parameters (e.g. `DayStyleCss`) to cascading `TableItemStyle` sub-components matching existing pattern in DetailsView/Login controls. Keep backward compat: old string props → `[Obsolete]`. | Cyclops | — | M | P1 |
| WI-29 | Calendar TableItemStyle tests | bUnit tests verifying each of the 9 style sub-components cascades correctly and renders appropriate CSS. | Rogue | WI-28 | S | P1 |
| WI-30 | Calendar style sample update | Update existing Calendar sample to use `<DayStyle>` sub-component syntax instead of string CSS. | Jubilee | WI-28 | S | P1 |
| WI-31 | Calendar: DayNameFormat/TitleFormat → enums | Create `DayNameFormat` enum (Full, Short, FirstLetter, FirstTwoLetters, Shortest) and `TitleFormat` enum (Month, MonthYear) in `src/BlazorWebFormsComponents/Enums/`. Change Calendar parameters from `string` to these enums. | Cyclops | — | S | P1 |
| WI-32 | Calendar enum tests | bUnit tests for each DayNameFormat and TitleFormat value rendering correctly. | Rogue | WI-31 | S | P1 |
| WI-33 | FormView: CssClass, headers, empty data | Add `CssClass` (if not inherited from WI-07), `HeaderText`, `HeaderTemplate`, `FooterText`, `FooterTemplate`, `EmptyDataText`, `EmptyDataTemplate`, `AllowPaging` (explicit bool, default true) to `FormView.razor.cs`. ~15 gaps closed. | Cyclops | WI-07 | M | P1 |
| WI-34 | FormView improvement tests | bUnit tests for header/footer rendering, EmptyDataText/EmptyDataTemplate, CssClass attribute. | Rogue | WI-33 | M | P1 |
| WI-35 | FormView sample update | Update existing FormView sample to demonstrate HeaderText, EmptyDataText, and CssClass. | Jubilee | WI-33 | S | P1 |
| WI-36 | HyperLink: Rename `NavigationUrl` → `NavigateUrl` | Rename the parameter in `src/BlazorWebFormsComponents/HyperLink.razor.cs` from `NavigationUrl` to `NavigateUrl`. Add `[Obsolete] NavigationUrl` property that forwards to `NavigateUrl` for backward compat. Update `.razor` template. 1 gap closed — but a blocking migration name mismatch. | Cyclops | — | S | P1 |
| WI-37 | HyperLink rename tests + sample | Update existing bUnit tests for `NavigateUrl`. Update sample page. Verify `[Obsolete]` forwarding works. | Rogue | WI-36 | S | P1 |
| WI-38 | ValidationSummary: Add HeaderText, ShowSummary, ValidationGroup | Add `HeaderText` (string), `ShowSummary` (bool, default true), `ValidationGroup` (string) to `src/BlazorWebFormsComponents/Validations/AspNetValidationSummary.razor.cs`. HeaderText renders as first `<li>` or `<p>` above the error list. ValidationGroup filters displayed errors. | Cyclops | — | S | P1 |
| WI-39 | ValidationSummary tests | bUnit tests for HeaderText rendering, ShowSummary=false hides summary, ValidationGroup filtering. | Rogue | WI-38 | S | P1 |
| WI-40 | ValidationSummary sample update | Update validator sample page to demonstrate HeaderText and ValidationGroup on AspNetValidationSummary. | Jubilee | WI-38 | S | P1 |

### P1 — Documentation & Audit Updates

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-41 | Re-audit PasswordRecovery | Update `planning-docs/PasswordRecovery.md` — the audit shows 0% because files weren't on branch during audit. PasswordRecovery now exists via PR #340. Re-run feature comparison against actual component. Update SUMMARY.md Login Controls totals. | Forge | — | S | P1 |
| WI-42 | GridView docs update | Update `docs/DataControls/GridView.md` to document new paging, sorting, and editing features. Add migration examples for each. | Beast | WI-19, WI-22, WI-25 | M | P1 |
| WI-43 | Calendar docs update | Update `docs/DataControls/Calendar.md` to reflect TableItemStyle sub-components and enum changes. | Beast | WI-28, WI-31 | S | P1 |
| WI-44 | FormView docs update | Update `docs/DataControls/FormView.md` to document new header/footer/empty data features. | Beast | WI-33 | S | P1 |

### P1 — Integration Tests

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-45 | Integration tests for GridView features | Add Playwright smoke + interaction tests for new GridView sample pages (Paging, Sorting, InlineEditing). Add to `ControlSampleTests.cs` and `InteractiveComponentTests.cs`. | Colossus | WI-21, WI-24, WI-27 | M | P1 |
| WI-46 | Integration tests for updated samples | Add/update Playwright tests for any updated Calendar, FormView, and ValidationSummary sample pages. | Colossus | WI-30, WI-35, WI-40 | S | P1 |

### P2 — Nice-to-Have

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-47 | ListControl `DataTextFormatString` | Add `DataTextFormatString` parameter to `BaseListControl` (or each ListControl). Applies `string.Format` to item text during rendering. Affects BulletedList, CheckBoxList, DropDownList, ListBox, RadioButtonList. | Cyclops | — | S | P2 |
| WI-48 | ListControl `AppendDataBoundItems` | Add `AppendDataBoundItems` (bool) to `BaseListControl`. When true, data-bound items are appended to statically defined items instead of replacing them. | Cyclops | — | S | P2 |
| WI-49 | CausesValidation on CheckBox, RadioButton, TextBox | Add `CausesValidation` (bool) and `ValidationGroup` (string) parameters to CheckBox, RadioButton, TextBox. Wire to form submission validation. | Cyclops | — | S | P2 |
| WI-50 | Menu `Orientation` property | Add `Orientation` enum (Horizontal, Vertical) to `src/BlazorWebFormsComponents/Menu.razor.cs`. Currently hardcoded to vertical rendering. | Cyclops | — | S | P2 |
| WI-51 | Label `AssociatedControlID` | Add `AssociatedControlID` (string) to Label. Renders `<label for="...">` instead of `<span>` when set. | Cyclops | WI-17 | S | P2 |
| WI-52 | Login controls outer WebControl styles | Add style properties (BackColor, BorderColor, CssClass, Font, ForeColor, Height, Width) to outer container of ChangePassword, CreateUserWizard, Login. These inherit BaseWebFormsComponent, not BaseStyledComponent — style must be applied to the rendered `<table>` wrapper. ~30 gaps closed. | Cyclops | — | M | P2 |
| WI-53 | P2 tests batch | bUnit tests for WI-47 through WI-52 features. One test file per feature. | Rogue | WI-47 to WI-52 | M | P2 |
| WI-54 | P2 sample updates | Update sample pages for ListControl format string, Menu orientation, Label for-attribute. | Jubilee | WI-47 to WI-52 | S | P2 |

---

## Execution Order

### Phase 1: Base Class Sweep (P0, parallel tracks)

```
Track A: WI-01 (AccessKey) → WI-02 (tests) + WI-03 (samples)
Track B: WI-04 (ToolTip)  → WI-05 (tests) + WI-06 (samples)
Track C: WI-07 (DataBound style) → WI-08 (tests) + WI-09 (samples)
Track D: WI-10 (Validator Display) → WI-11 (tests) + WI-12 (samples)
Track E: WI-13 (SetFocusOnError) → WI-14 (tests)
Track F: WI-15 (Image base) → WI-16 (tests)
Track G: WI-17 (Label base) → WI-18 (tests)
```

Tracks A–G can run in parallel. WI-07 must complete before any P1 data control work.

### Phase 2: GridView Overhaul (P1, sequential within GridView)

```
WI-19 (paging) → WI-20 + WI-21 → WI-22 (sorting) → WI-23 + WI-24 → WI-25 (editing) → WI-26 + WI-27
WI-42 (GridView docs) after WI-19 + WI-22 + WI-25
WI-45 (integration tests) after WI-21 + WI-24 + WI-27
```

### Phase 3: Other P1 Controls (parallel)

```
Track H: WI-28 (Calendar styles) → WI-29 + WI-30 → WI-43 (docs)
Track I: WI-31 (Calendar enums) → WI-32
Track J: WI-33 (FormView) → WI-34 + WI-35 → WI-44 (docs)
Track K: WI-36 (HyperLink rename) → WI-37
Track L: WI-38 (ValidationSummary) → WI-39 + WI-40
Track M: WI-41 (PasswordRecovery re-audit) — no dependencies
WI-46 (integration tests) after H + J + L
```

### Phase 4: P2 Nice-to-Have (if time permits)

```
WI-47 through WI-54 — parallel implementation, batch testing, batch samples
```

---

## Summary Stats

| Priority | Work Items | Est. Gaps Closed | Agents Involved |
|----------|-----------|-----------------|-----------------|
| P0 | 18 (WI-01 to WI-18) | ~180 | Cyclops, Rogue, Jubilee |
| P1 | 28 (WI-19 to WI-46) | ~120 | All agents |
| P2 | 8 (WI-47 to WI-54) | ~45 | Cyclops, Rogue, Jubilee |
| **Total** | **54** | **~345** | **6 agents** |

### Expected Health After Milestone 6

| Metric | Before (M5) | After (M6 P0+P1) |
|--------|-------------|-------------------|
| Overall Health | 66.3% | ~85% (est.) |
| Matching features | 1,272 | ~1,572 |
| GridView Health | 20.7% | ~55% |
| Data Controls avg | 53.2% | ~70% |
| Validation Controls avg | 76.5% | ~82% |
| 100% controls | 6 | 6 (no new 100% — gains are breadth) |

---

## Key Decisions

1. **P0 base class changes are the highest-ROI work** — 7 changes close ~180 gaps. This is the most efficient use of engineering time.
2. **GridView is the #1 P1 priority** — most-used Web Forms data control at 20.7% health. Paging → sorting → editing is the correct sequence.
3. **PasswordRecovery audit is stale, not the component** — the component ships; only the audit doc needs updating (WI-41).
4. **Login controls outer styles moved to P2** — these controls use CascadingParameter-based sub-styles by convention. Adding WebControl-level styles to the outer `<table>` wrapper is useful but lower priority than GridView/Calendar/FormView improvements.
5. **Skip Substitution and Xml** — per existing team decision, both remain deferred.
6. **HyperLink rename is small but blocking** — `NavigationUrl` vs `NavigateUrl` is a migration-breaking name mismatch that affects every HyperLink in every migrated app.
7. **Calendar style conversion is technically breaking** — old `DayStyleCss` string params become `[Obsolete]` with `<DayStyle>` sub-component replacements. Backward compat via obsolete forwarding.
