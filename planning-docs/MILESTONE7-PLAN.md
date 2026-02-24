# Milestone 7 — Control Depth & Navigation Overhaul

**Created:** 2026-02-23
**Author:** Forge (Lead / Web Forms Reviewer)
**Branch:** `milestone7/control-depth`
**Baseline:** dev (post-M6 feature gap closure)

---

## Goals

Milestone 6 closed ~345 gaps with sweeping base class fixes and brought GridView from 20.7% to ~55%. Milestone 7 shifts focus to **depth over breadth**: completing GridView to ~75%+, overhauling the two weakest navigation controls (Menu, TreeView), and adding style sub-components to the data controls that still lack them (DetailsView, FormView). A mandatory re-audit opens the milestone so we have accurate numbers.

### Theme: "Finish the Data Controls, Fix the Navigation Controls"

**Priority targets (by remaining gap severity):**

| Control | Post-M6 Est. | M7 Target | Key additions |
|---------|-------------|-----------|---------------|
| GridView | ~55% | ~75% | Selection, style sub-components, display props |
| Menu | ~42% | ~60% | Base class upgrade, selection, events, core props |
| TreeView | ~60% | ~75% | Node-level styles, selection, expand control |
| FormView | ~50% | ~65% | Style sub-components, paging events, remaining events |
| DetailsView | ~70% | ~80% | Style sub-components, PagerSettings, Caption |
| ListView | ~42% | ~50% | CRUD events (P2 — large effort) |
| DataGrid | ~55% | ~65% | Style sub-components, paging/sorting events (P2) |

### Scoping Rules (carried from M6)

- ✅ Substitution and Xml remain intentionally deferred
- ✅ Chart advanced properties remain deferred (canvas architecture)
- ✅ DataSourceID / model binding methods remain N/A
- ✅ Focus() method remains deferred (use `ElementReference.FocusAsync()`)
- ✅ Docs and samples must ship in the same milestone as the features they cover

---

## Work Items

### P0 — Re-Audit + GridView Completion

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-01 | **Re-audit all 53 controls post-M6** | Update every audit doc in `planning-docs/` against the current `dev` branch. M6 shipped ~345 gap closures (AccessKey, ToolTip, DataBound style inheritance, validators, Image/Label base class, GridView paging/sorting/editing, Calendar styles+enums, FormView header/footer/empty, HyperLink rename, ValidationSummary, ListControl improvements, CausesValidation, Menu Orientation, Label AssociatedControlID, Login base class upgrade). Regenerate SUMMARY.md with accurate numbers. This unblocks accurate priority decisions for the rest of M7. | Forge | — | L | P0 |
| WI-02 | **GridView: Selection support** | Add `SelectedIndex` (int, default -1), `SelectedRow` (GridViewRow, read-only), `SelectedRowStyle` (TableItemStyle sub-component), `SelectedValue` (object, read-only — value of DataKeyNames field for selected row). Add `SelectedIndexChanged` and `SelectedIndexChanging` events with `GridViewSelectEventArgs` (supports cancellation via `Cancel` bool). Add `AutoGenerateSelectButton` (bool) — renders Select command link per row. When SelectedIndex is set, apply SelectedRowStyle CSS to that `<tr>`. ~9 gaps closed. | Cyclops | WI-01 | M | P0 |
| WI-03 | **GridView selection tests** | bUnit tests: SelectedIndex renders highlighted row, SelectedIndexChanging fires with correct index, cancellation prevents selection, AutoGenerateSelectButton renders Select link, SelectedRowStyle applies CSS, SelectedValue returns correct key, reset SelectedIndex to -1 clears selection. | Rogue | WI-02 | M | P0 |
| WI-04 | **GridView selection sample** | New sample page `Components/Pages/ControlSamples/GridView/Selection.razor` demonstrating row selection with SelectedRowStyle highlighting and SelectedValue display. | Jubilee | WI-02 | S | P0 |
| WI-05 | **GridView: Style sub-components** | Add TableItemStyle sub-components: `AlternatingRowStyle`, `RowStyle`, `HeaderStyle`, `FooterStyle`, `EmptyDataRowStyle`, `PagerStyle`. Follow the existing pattern from Calendar/DetailsView (cascading `TableItemStyle` parameters). Apply as CSS classes/inline styles to the appropriate `<tr>` elements. ~6 gaps closed. | Cyclops | — | M | P0 |
| WI-06 | **GridView style sub-component tests** | bUnit tests for each of the 6 style sub-components: verify CSS renders on correct rows, alternating style on odd rows, header/footer styles on respective rows, pager style on pager row, empty data style when no items. | Rogue | WI-05 | M | P0 |
| WI-07 | **GridView: Display properties** | Add `ShowHeader` (bool, default true), `ShowFooter` (bool, default false), `ShowHeaderWhenEmpty` (bool, default false), `Caption` (string), `CaptionAlign` (`TableCaptionAlign` enum), `EmptyDataTemplate` (RenderFragment — currently only EmptyDataText exists), `GridLines` (`GridLines` enum — renders `rules` attribute on `<table>`), `UseAccessibleHeader` (bool — renders `<th scope="col">` instead of `<td>` in header). ~8 gaps closed. | Cyclops | — | M | P0 |
| WI-08 | **GridView display property tests** | bUnit tests: ShowHeader=false hides `<thead>`, ShowFooter=true shows footer row, Caption renders `<caption>`, EmptyDataTemplate renders custom content, GridLines renders `rules` attribute, UseAccessibleHeader renders `<th scope="col">`. | Rogue | WI-07 | M | P0 |
| WI-09 | **GridView samples update** | Update existing GridView sample pages to demonstrate new style sub-components and display properties. Add Caption, GridLines, and RowStyle/AlternatingRowStyle examples to Paging sample. | Jubilee | WI-02, WI-05, WI-07 | S | P0 |
| WI-10 | **GridView docs update** | Update `docs/DataControls/GridView.md` to document selection features, style sub-components, and display properties. Add migration examples showing equivalent Web Forms markup. | Beast | WI-02, WI-05, WI-07 | M | P0 |

### P1 — TreeView Enhancement

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-11 | **TreeView: Node-level style sub-components** | Create `TreeNodeStyle` class (extends `TableItemStyle` or similar — adds `ChildNodesPadding`, `HorizontalPadding`, `ImageUrl`, `NodeSpacing`, `VerticalPadding`). Add as sub-components: `HoverNodeStyle`, `LeafNodeStyle`, `ParentNodeStyle`, `RootNodeStyle`, `SelectedNodeStyle`, `NodeStyle` (default for all nodes). Add `LevelStyles` (collection of `TreeNodeStyle` applied per depth level). Apply styles based on node type during rendering. ~7 gaps closed. | Cyclops | — | L | P1 |
| WI-12 | **TreeView node-level style tests** | bUnit tests: NodeStyle applies to all nodes, LeafNodeStyle overrides for leaf nodes, ParentNodeStyle for parents, RootNodeStyle for root, HoverNodeStyle adds CSS class, SelectedNodeStyle applies to selected node, LevelStyles apply at correct depths. | Rogue | WI-11 | M | P1 |
| WI-13 | **TreeView: Selection support** | Wire up the existing `Selected` property on TreeNode to a TreeView-level `SelectedNode` (TreeNode, read-only) and `SelectedValue` (string, read-only — returns TreeNode.Value). Add `SelectedNodeChanged` event (EventCallback). When a node is clicked/selected, deselect the previously selected node, update SelectedNode, fire SelectedNodeChanged. ~3 gaps closed. | Cyclops | — | S | P1 |
| WI-14 | **TreeView selection tests** | bUnit tests: clicking node sets SelectedNode, SelectedNodeChanged fires, SelectedValue returns correct value, previous selection is cleared, SelectedNodeStyle applies (if WI-11 done). | Rogue | WI-13 | S | P1 |
| WI-15 | **TreeView: Expand/collapse methods + properties** | Add `ExpandAll()` and `CollapseAll()` public methods (iterate all TreeNodes, set Expanded). Add `FindNode(string valuePath)` method (navigate tree by PathSeparator-delimited value path). Add `ExpandDepth` (int, default -1 meaning fully expanded) — on initial render, expand nodes to this depth. Add `NodeIndent` (int, default 20) — pixel indent per depth level. Add `PathSeparator` (char, default '/'). ~5 gaps closed. | Cyclops | — | M | P1 |
| WI-16 | **TreeView expand/collapse tests** | bUnit tests: ExpandAll expands all nodes, CollapseAll collapses all, FindNode returns correct node, ExpandDepth limits initial expansion, NodeIndent affects padding, PathSeparator used in FindNode. | Rogue | WI-15 | S | P1 |
| WI-17 | **TreeView samples update** | Update TreeView sample page to demonstrate selection with SelectedNodeChanged callback, node-level styles, ExpandDepth, and ExpandAll/CollapseAll buttons. | Jubilee | WI-11, WI-13, WI-15 | S | P1 |
| WI-18 | **TreeView docs update** | Update `docs/NavigationControls/TreeView.md` to document node styles, selection, expand/collapse features with migration examples. | Beast | WI-11, WI-13, WI-15 | S | P1 |

### P1 — Menu Core Improvements

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-19 | **Menu: Base class → BaseStyledComponent** | Change Menu to inherit `BaseStyledComponent` instead of `BaseWebFormsComponent`. This gives Menu all WebControl style properties: BackColor, BorderColor, BorderStyle, BorderWidth, CssClass, Font, ForeColor, Height, Width. Update `.razor` template to render `CombinedStyle` on root `<div>`. Remove any duplicate style declarations. Verify JS interop still works. ~9 gaps closed. | Cyclops | — | S | P1 |
| WI-20 | **Menu base class style tests** | bUnit tests: CssClass renders on root element, BackColor/ForeColor render as inline style, Font renders correctly, existing DynamicHoverStyle/StaticMenuItemStyle sub-components still work alongside base styles. | Rogue | WI-19 | S | P1 |
| WI-21 | **Menu: Selection tracking + events** | Add `SelectedItem` (MenuItem, read-only) and `SelectedValue` (string, read-only — returns SelectedItem.Value). Add `MenuItemClick` event (`EventCallback<MenuEventArgs>`) — fires when a menu item is clicked. Add `MenuItemDataBound` event (`EventCallback<MenuEventArgs>`) — fires after each MenuItem is data-bound. Wire MenuItemClick to the existing click handling in MenuItem component. ~4 gaps closed. | Cyclops | — | M | P1 |
| WI-22 | **Menu selection + events tests** | bUnit tests: MenuItemClick fires on item click with correct MenuEventArgs, SelectedItem set after click, SelectedValue returns value, MenuItemDataBound fires during data binding. | Rogue | WI-21 | S | P1 |
| WI-23 | **Menu: Core missing properties** | Add `MaximumDynamicDisplayLevels` (int, default 3) — limits depth of dynamic flyout menus. Add `Target` (string) — default link target for menu items. Add `SkipLinkText` (string, default "Skip Navigation Links") — accessibility skip link. Add `PathSeparator` (char, default '/') — used in MenuItem.ValuePath. ~4 gaps closed. | Cyclops | — | S | P1 |
| WI-24 | **Menu core property tests** | bUnit tests: MaximumDynamicDisplayLevels limits rendered depth, Target renders on `<a>` elements, SkipLinkText renders skip link, PathSeparator used in value paths. | Rogue | WI-23 | S | P1 |
| WI-25 | **Menu samples + docs update** | Update Menu sample to demonstrate CssClass, MenuItemClick handler, selection display, SkipLinkText. Update `docs/NavigationControls/Menu.md` with migration examples. | Jubilee + Beast | WI-19, WI-21, WI-23 | S | P1 |

### P1 — DetailsView Polish

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-26 | **DetailsView: Style sub-components** | Add TableItemStyle sub-components: `RowStyle`, `AlternatingRowStyle`, `HeaderStyle`, `FooterStyle`, `CommandRowStyle`, `EditRowStyle`, `InsertRowStyle`, `FieldHeaderStyle`, `EmptyDataRowStyle`, `PagerStyle`. Follow existing Calendar/GridView pattern. Apply CSS to the appropriate `<tr>` elements based on mode and row type. ~10 gaps closed. | Cyclops | — | M | P1 |
| WI-27 | **DetailsView style sub-component tests** | bUnit tests: RowStyle applies to field rows, AlternatingRowStyle alternates, HeaderStyle on header row, FooterStyle on footer, CommandRowStyle on command row, EditRowStyle applies in edit mode, InsertRowStyle in insert mode, FieldHeaderStyle on field label cells. | Rogue | WI-26 | M | P1 |
| WI-28 | **DetailsView: PagerSettings + Caption** | Add `PagerSettings` configuration object (Mode: Numeric/NextPrev/NumericFirstLast/NextPrevFirstLast; Position: Bottom/Top/TopAndBottom; PageButtonCount; FirstPageText, LastPageText, NextPageText, PreviousPageText, NextPageImageUrl, PreviousPageImageUrl). Add `Caption` (string), `CaptionAlign` (`TableCaptionAlign` enum). Add `PageCount` (int, computed = Items.Count()). ~4+ gaps closed. | Cyclops | — | M | P1 |
| WI-29 | **DetailsView PagerSettings tests** | bUnit tests: PagerSettings.Mode=NextPrev renders prev/next links, Mode=Numeric renders page numbers, Position=Top renders pager above content, Caption renders `<caption>` element, PageCount returns correct value. | Rogue | WI-28 | S | P1 |
| WI-30 | **DetailsView samples + docs update** | Update DetailsView sample with style sub-component examples and PagerSettings. Update `docs/DataControls/DetailsView.md`. | Jubilee + Beast | WI-26, WI-28 | S | P1 |

### P1 — FormView Polish

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-31 | **FormView: Remaining events** | Add `ModeChanged` (EventCallback — fires after mode transition completes, complementing existing ModeChanging). Add `ItemCommand` (EventCallback<FormViewCommandEventArgs> — exposes command bubbling). Add `ItemCreated` (EventCallback). Add `PageIndexChanging` and `PageIndexChanged` events (EventCallback<FormViewPageEventArgs>) — fires when pager navigation occurs. ~5 gaps closed. | Cyclops | — | S | P1 |
| WI-32 | **FormView event tests** | bUnit tests: ModeChanged fires after ModeChanging, ItemCommand fires on command bubble, PageIndexChanging fires on page nav with cancellation support, PageIndexChanged fires after. | Rogue | WI-31 | S | P1 |
| WI-33 | **FormView: Style sub-components + pager config** | Add TableItemStyle sub-components: `EditRowStyle`, `InsertRowStyle`, `RowStyle`, `HeaderStyle`, `FooterStyle`, `EmptyDataRowStyle`, `PagerStyle`. Add `PagerSettings` (reuse same type from DetailsView/GridView). Add `PagerTemplate` (RenderFragment). Add `Caption` (string), `CaptionAlign` (enum). ~11 gaps closed. | Cyclops | — | M | P1 |
| WI-34 | **FormView style sub-component tests** | bUnit tests: EditRowStyle applies in edit mode, InsertRowStyle in insert, RowStyle in readonly, PagerSettings controls pager rendering, PagerTemplate overrides default pager. | Rogue | WI-33 | M | P1 |
| WI-35 | **FormView samples + docs update** | Update FormView sample with style examples, PagerSettings demo. Update `docs/DataControls/FormView.md`. | Jubilee + Beast | WI-31, WI-33 | S | P1 |

### P1 — Validator ControlToValidate

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-36 | **Validator: ControlToValidate string ID support** | Add `ControlToValidate` (string) parameter to `BaseValidator`. Web Forms uses `ControlToValidate="TextBox1"` with string IDs. Current Blazor implementation uses `ForwardRef` which doesn't match the migration pattern. Support both: when `ControlToValidate` string is set, locate the target control by ID in the form and validate it. Keep ForwardRef as the Blazor-native alternative. Affects all 5 input validators (RequiredFieldValidator, CompareValidator, RangeValidator, RegularExpressionValidator, CustomValidator). ~5 gaps closed. | Cyclops | — | M | P1 |
| WI-37 | **Validator ControlToValidate tests** | bUnit tests: string ControlToValidate finds target control, validation fires against correct control, error when ControlToValidate ID not found, ForwardRef still works as alternative. | Rogue | WI-36 | S | P1 |
| WI-38 | **Validator docs update** | Update validator docs to document ControlToValidate string ID pattern with migration examples from Web Forms. | Beast | WI-36 | S | P1 |

### P1 — Integration Tests

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-39 | **Integration tests: GridView selection + styles** | Playwright smoke + interaction tests for new GridView sample pages (Selection, updated Paging with styles). Verify row selection highlighting, style rendering, Caption display, GridLines on `<table>`. | Colossus | WI-04, WI-09 | M | P1 |
| WI-40 | **Integration tests: TreeView + Menu + DetailsView + FormView** | Playwright tests for updated TreeView (selection, expand/collapse), Menu (click event, selection), DetailsView (style rendering, pager), FormView (pager, mode change). | Colossus | WI-17, WI-25, WI-30, WI-35 | M | P1 |

### P2 — Nice-to-Have

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-41 | **ListView: CRUD events + edit/insert templates** | Add events: `ItemCommand`, `ItemCanceling`, `ItemEditing`, `ItemDeleting`, `ItemDeleted`, `ItemInserting`, `ItemInserted`, `ItemUpdating`, `ItemUpdated`, `ItemCreated`. Add `EditIndex` (int), `EditItemTemplate` (RenderFragment<T>), `InsertItemTemplate` (RenderFragment<T>), `EmptyItemTemplate` (RenderFragment). Wire InsertItemPosition (already exists as stub). ~22 gaps closed. ListView is a common GridView alternative in Web Forms apps. | Cyclops | — | L | P2 |
| WI-42 | **ListView CRUD tests** | bUnit tests for all CRUD events: editing flow (ItemEditing → rendering EditItemTemplate → ItemUpdating → ItemUpdated), delete flow, insert flow, ItemCommand bubbling, cancellation support. | Rogue | WI-41 | M | P2 |
| WI-43 | **ListView samples + docs** | New ListView CRUD sample page. Update `docs/DataControls/ListView.md`. | Jubilee + Beast | WI-41 | S | P2 |
| WI-44 | **DataGrid: Style sub-components** | Add TableItemStyle sub-components: `AlternatingItemStyle`, `ItemStyle`, `HeaderStyle`, `FooterStyle`, `PagerStyle`, `SelectedItemStyle`, `EditItemStyle`. Add `Caption` (string), `CaptionAlign` (enum), `CellPadding` (int), `CellSpacing` (int), `GridLines` (enum), `UseAccessibleHeader` (bool). ~13 gaps closed. | Cyclops | — | M | P2 |
| WI-45 | **DataGrid: Paging + sorting events** | Add `PageIndexChanged` (DataGridPageChangedEventHandler), `SortCommand` (DataGridSortCommandEventHandler), `ItemCreated` (DataGridItemEventHandler), `ItemDataBound` (DataGridItemEventHandler), `SelectedIndexChanged` (EventHandler). Wire PageIndexChanged to existing AllowPaging logic. Wire SortCommand to AllowSorting. ~5 gaps closed. | Cyclops | — | S | P2 |
| WI-46 | **DataGrid tests** | bUnit tests for style sub-components and new events. | Rogue | WI-44, WI-45 | M | P2 |
| WI-47 | **Menu: Level styles** | Add `LevelMenuItemStyles` (MenuItemStyleCollection), `LevelSelectedStyles` (MenuItemStyleCollection), `LevelSubMenuStyles` (SubMenuStyleCollection). Apply per-depth-level styles to menu items during rendering. ~3 gaps closed. | Cyclops | WI-19 | M | P2 |
| WI-48 | **Panel BackImageUrl** | Add `BackImageUrl` (string) to Panel component. Render as `background-image: url(...)` in inline style. ~1 gap closed. | Cyclops | — | S | P2 |
| WI-49 | **Login/ChangePassword Orientation + TextLayout** | Add `Orientation` (Orientation enum: Horizontal, Vertical) and `TextLayout` (LoginTextLayout enum: TextOnLeft, TextOnTop) to Login and ChangePassword components. Adjust table layout rendering based on these values. ~4 gaps closed. | Cyclops | — | S | P2 |
| WI-50 | **P2 tests batch** | bUnit tests for WI-47, WI-48, WI-49. | Rogue | WI-47, WI-48, WI-49 | S | P2 |
| WI-51 | **P2 samples + docs batch** | Update sample pages for Menu level styles, Panel BackImageUrl, Login/ChangePassword Orientation. Update corresponding docs. | Jubilee + Beast | WI-47, WI-48, WI-49 | S | P2 |

---

## Execution Order

### Phase 1: Re-Audit (blocks P0 implementation scoping)

```
WI-01 (Re-audit) — Forge solo
  └── feeds updated priority information into all subsequent WIs
```

WI-01 is the only true blocker. Implementation WIs have technical dependencies but can begin in parallel once the re-audit confirms no surprises.

### Phase 2: GridView Completion (P0, partially parallel)

```
Track A: WI-02 (selection) → WI-03 (tests) + WI-04 (sample)
Track B: WI-05 (styles)    → WI-06 (tests)
Track C: WI-07 (display)   → WI-08 (tests)
Merge:   WI-09 (samples, after A+B+C) + WI-10 (docs, after A+B+C)
         WI-39 (integration tests, after WI-04 + WI-09)
```

Tracks A, B, C can run in parallel. WI-09/WI-10 wait for all three.

### Phase 3: Navigation Controls (P1, parallel tracks)

```
Track D: WI-11 (TreeView styles) → WI-12 (tests)
Track E: WI-13 (TreeView selection) → WI-14 (tests)
Track F: WI-15 (TreeView expand) → WI-16 (tests)
Merge:   WI-17 (TreeView samples) + WI-18 (TreeView docs) after D+E+F

Track G: WI-19 (Menu base class) → WI-20 (tests)
Track H: WI-21 (Menu selection) → WI-22 (tests)
Track I: WI-23 (Menu props) → WI-24 (tests)
Merge:   WI-25 (Menu samples+docs) after G+H+I
```

### Phase 4: Data Control Polish (P1, parallel tracks)

```
Track J: WI-26 (DetailsView styles) → WI-27 (tests)
Track K: WI-28 (DetailsView pager) → WI-29 (tests)
Merge:   WI-30 (DetailsView samples+docs) after J+K

Track L: WI-31 (FormView events) → WI-32 (tests)
Track M: WI-33 (FormView styles) → WI-34 (tests)
Merge:   WI-35 (FormView samples+docs) after L+M

Track N: WI-36 (Validator ControlToValidate) → WI-37 (tests) → WI-38 (docs)

WI-40 (integration tests) after WI-17 + WI-25 + WI-30 + WI-35
```

### Phase 5: P2 Nice-to-Have (if time permits)

```
Track O: WI-41 (ListView CRUD) → WI-42 (tests) → WI-43 (samples+docs)
Track P: WI-44 (DataGrid styles) + WI-45 (DataGrid events) → WI-46 (tests)
Track Q: WI-47 (Menu levels) + WI-48 (Panel) + WI-49 (Login Orientation) → WI-50 (tests) → WI-51 (samples+docs)
```

---

## Summary Stats

| Priority | Work Items | Est. Gaps Closed | Agents Involved |
|----------|-----------|-----------------|-----------------|
| P0 | 10 (WI-01 to WI-10) | ~23 + audit refresh | Forge, Cyclops, Rogue, Jubilee, Beast |
| P1 | 30 (WI-11 to WI-40) | ~67 | All agents |
| P2 | 11 (WI-41 to WI-51) | ~48 | Cyclops, Rogue, Jubilee, Beast |
| **Total** | **51** | **~138** | **6 agents** |

### Expected Health After Milestone 7

| Metric | Post-M6 (est.) | After M7 P0+P1 | After M7 All |
|--------|----------------|----------------|--------------|
| Overall Health | ~82% | ~87% | ~90% |
| GridView | ~55% | ~75% | ~75% |
| TreeView | ~60% | ~75% | ~75% |
| Menu | ~42% | ~60% | ~65% |
| FormView | ~50% | ~65% | ~65% |
| DetailsView | ~70% | ~80% | ~80% |
| ListView | ~42% | ~42% | ~55% |
| DataGrid | ~55% | ~55% | ~68% |
| Data Controls avg | ~55% (est.) | ~65% | ~70% |
| Navigation Controls avg | ~65% (est.) | ~72% | ~75% |

### Gap Closure Trajectory

| Milestone | Gaps Closed | Cumulative | Strategy |
|-----------|-----------|------------|----------|
| M1–M5 | ~1,272 (baseline matching) | 1,272 | Component creation |
| M6 | ~345 | 1,617 | Sweeping base class fixes + GridView overhaul |
| **M7** | **~138** | **~1,755** | **Control depth + navigation overhaul** |

The diminishing numbers reflect the long tail: remaining gaps are style sub-components and complex event pipelines that require more work per gap than base class inheritance fixes.

---

## Key Decisions

1. **Re-audit is P0 and opens the milestone.** The audit docs are stale — many reflect pre-M6 state. Accurate numbers are essential for tracking and prioritization. Forge owns this and it must complete before implementation WIs are finalized.

2. **GridView is still the #1 priority.** At ~55% post-M6, it's the most-used data control in Web Forms apps and still has major gaps (selection, styles, display props). Pushing it to ~75% is the highest-impact single-control work.

3. **Navigation controls are the new #2.** Menu (42%) and TreeView (60%) are the weakest remaining controls that aren't intentionally deferred. Both are commonly used in Web Forms apps for site navigation. Menu's base class upgrade is a small fix with big impact.

4. **Style sub-components are the consistent theme.** DetailsView, FormView, GridView, DataGrid — all need TableItemStyle sub-components for visual fidelity. This is the biggest remaining systematic gap.

5. **PagerSettings should be a shared type.** GridView, FormView, and DetailsView all need PagerSettings with the same API shape. Implement once, reuse across all three controls.

6. **ListView CRUD is P2 despite large gap count.** ListView has ~22 missing event gaps but the implementation is L-sized and ListView is less commonly used than GridView. If time permits, it's high value, but it shouldn't block the milestone.

7. **Validator ControlToValidate is a migration-blocking API mismatch.** The current ForwardRef pattern doesn't match how Web Forms developers wrote `ControlToValidate="TextBox1"`. String ID support is essential for the "paste your markup and it works" migration story.

8. **Skip Chart enhancements again.** Chart at 32.3% has known architectural deviations (canvas vs. `<img>`) and the remaining gaps are intentionally deferred per M4 decisions.

---

## Risk Register

| Risk | Impact | Mitigation |
|------|--------|------------|
| Re-audit reveals unexpected regressions from M6 | High — could invalidate plan | Run full test suite first; audit is primarily documentation, not functional |
| PagerSettings complexity — shared type across 3 controls | Medium — could cause breaking changes if API shape differs | Design review before implementation; check Web Forms source for exact type shape |
| Menu base class change breaks JS interop | Medium — Menu uses custom JS | Test JS initialization with new base class; Menu.js may assume specific DOM structure |
| TreeView LevelStyles collection is architecturally complex | Low — can ship without it initially | LevelStyles can be deferred to M8 if needed; ship other node styles first |
| ListView CRUD is underestimated at L | Low — it's P2 | If time-boxed out, defer to M8 |
