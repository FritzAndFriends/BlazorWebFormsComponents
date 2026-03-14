### 2026-03-14: Divergence & Feature Gap Issue Plan
**By:** Forge (Lead / Web Forms Reviewer)
**What:** Comprehensive issue plan for addressing DIVERGENCE-REGISTRY fixes and known feature gaps across all 53 Web Forms controls. Issues organized by milestone with priority and acceptance criteria.

---

## Milestone: Component Parity (M20)

### Issue 1: Add AccessKey Property to BaseWebFormsComponent
**Labels:** enhancement, base-class, accessibility
**Priority:** High
**Description:** `AccessKey` is missing from `BaseWebFormsComponent`, affecting ~40 controls. Web Forms `WebControl.AccessKey` maps to the HTML `accesskey` attribute. Adding this to the base class will close a sweeping gap across all control hierarchies.
**Acceptance criteria:**
- [ ] Add `[Parameter] public string AccessKey { get; set; }` to `BaseWebFormsComponent`
- [ ] Render `accesskey="@AccessKey"` in component template
- [ ] Update all 53 control audit scores — 40 should show "Fixed" for AccessKey
- [ ] Add unit test for AccessKey rendering on 3+ controls (Button, TextBox, Calendar)
- [ ] Audit report regenerated showing closure of ~40 gaps

---

### Issue 2: Add ToolTip Property to BaseWebFormsComponent
**Labels:** enhancement, base-class, accessibility
**Priority:** High
**Description:** `ToolTip` (renders as HTML `title` attribute) is missing from the base class. ~35 controls need this. Currently only Button, Calendar, DataList, FileUpload, HyperLink, Image, ImageButton, ImageMap implement it directly, causing inconsistency.
**Acceptance criteria:**
- [ ] Add `[Parameter] public string ToolTip { get; set; }` to `BaseWebFormsComponent`
- [ ] Render `title="@ToolTip"` in component template
- [ ] Remove duplicate `ToolTip` implementations from 8 controls that already have it
- [ ] Add unit tests for ToolTip on Button, Panel, Label (previously missing)
- [ ] Audit report shows closure of ~35 gaps

---

### Issue 3: Fix DataBoundComponent<T> Inheritance — Add BaseStyledComponent
**Labels:** enhancement, base-class, data-controls
**Priority:** High
**Description:** `DataBoundComponent<T>` inherits `BaseWebFormsComponent`, not `BaseStyledComponent`. This means 7 data controls (DataGrid, DetailsView, FormView, GridView, ListView, Chart, Menu) are missing all WebControl style properties (BackColor, BorderColor, BorderStyle, BorderWidth, CssClass, Font, ForeColor, Height, Width, Style). A single inheritance fix will close ~70 missing property gaps.
**Acceptance criteria:**
- [ ] Change `DataBoundComponent<T>` to inherit `BaseStyledComponent` (or implement `IStyle`)
- [ ] Verify no breaking changes to existing data control implementations
- [ ] Test all 7 affected controls render style properties correctly
- [ ] Update GridView, FormView, ListView, DataGrid samples to demonstrate BackColor/CssClass
- [ ] Audit scores for all 7 controls show +10-15 point improvement

---

### Issue 4: Fix Image and Label Base Classes — Change to BaseStyledComponent
**Labels:** enhancement, base-class, controls
**Priority:** High
**Description:** `Image` and `Label` inherit `BaseWebFormsComponent` instead of `BaseStyledComponent`, despite Web Forms having them inherit `WebControl`. This causes both to miss all 11 style properties (BackColor through Width). ImageMap was already fixed per team decision — Image and Label should follow.
**Acceptance criteria:**
- [ ] Change `Image.cs` to inherit `BaseStyledComponent`
- [ ] Change `Label.cs` to inherit `BaseStyledComponent`
- [ ] Verify no breaking changes in existing sample pages
- [ ] Add tests showing BackColor, CssClass, Height, Width render correctly
- [ ] Update Image and Label samples to demonstrate style properties
- [ ] Both controls' audit scores improve by ~11 points

---

### Issue 5: Implement Display Property for All Validators
**Labels:** enhancement, validators, layout-fidelity
**Priority:** High
**Description:** All 6 validator controls (RequiredFieldValidator, RangeValidator, RegularExpressionValidator, CustomValidator, CompareValidator, ValidationSummary) are missing the `Display` property (`ValidatorDisplay` enum: None, Static, Dynamic). This controls whether the validator reserves space in layout (Static), collapses when valid (Dynamic), or is invisible (None — used with ValidationSummary). Without this, migrated pages have layout differences.
**Acceptance criteria:**
- [ ] Create `Enums/ValidatorDisplay.cs` with None (0), Static (1), Dynamic (2)
- [ ] Add `[Parameter] public ValidatorDisplay Display { get; set; } = ValidatorDisplay.Static` to `BaseValidator`
- [ ] Render `style="display:none"` when Display=None; `style="visibility:hidden"` when Display=Static; normal flow when Display=Dynamic
- [ ] Add tests for all 3 Display modes on RequiredFieldValidator and ValidationSummary
- [ ] Update validator samples to show Display property usage
- [ ] All 6 validators' audit scores improve by 1 point each

---

### Issue 6: Add SetFocusOnError Property to All Validators
**Labels:** enhancement, validators, ux
**Priority:** Medium
**Description:** All 6 validator controls are missing `SetFocusOnError` property. This controls whether a failed validation will move keyboard focus to the validator's control. This is a commonly-used UX pattern in Web Forms.
**Acceptance criteria:**
- [ ] Add `[Parameter] public bool SetFocusOnError { get; set; }` to `BaseValidator`
- [ ] On validation failure, if SetFocusOnError=true, call `element.FocusAsync()` via ElementReference
- [ ] Add tests for SetFocusOnError behavior on RequiredFieldValidator and CustomValidator
- [ ] Add documentation note about JS interop for focus management
- [ ] All 6 validators' audit scores improve by 1 point each

---

### Issue 7: Fix D-11 — GUID-Based IDs → Developer-Provided ID with Suffixes
**Labels:** bug, divergence-registry, controls
**Priority:** High
**Description:** **D-11 (DIVERGENCE-REGISTRY):** Controls currently generate GUID-based IDs for hidden fields and sub-elements. This is a bug. Controls should use the developer-provided `ID` parameter and append `_0`, `_1` per Web Forms convention. Affects CheckBox, RadioButton, RadioButtonList, FileUpload, and others.
**Acceptance criteria:**
- [ ] Audit which controls currently use GUIDs (CheckBox, RadioButton, RadioButtonList, FileUpload suspected)
- [ ] Change all GUID generation to use `ID` parameter + numeric suffixes (`{ID}_0`, `{ID}_1`, etc.)
- [ ] For controls without explicit ID, generate a stable, deterministic ID (not random GUID)
- [ ] Add tests for ID suffix generation on CheckBox (2 IDs), RadioButtonList (N IDs)
- [ ] Compare rendered HTML with Web Forms reference output — should match ID structure
- [ ] Verify no breaking changes to existing pages using these controls

---

### Issue 8: Fix D-13 — Calendar Previous-Month Day Padding (Full 42-Cell Grid)
**Labels:** bug, divergence-registry, calendar, html-fidelity
**Priority:** High
**Description:** **D-13 (DIVERGENCE-REGISTRY):** Calendar currently does not render previous-month day padding. Web Forms Calendar renders a full 42-cell grid with adjacent-month day numbers and applies `OtherMonthDayStyle`. This is visible structural content, not infrastructure. Missing this impacts Calendar's 74.5% similarity score.
**Acceptance criteria:**
- [ ] Calculate previous-month days needed to fill first week (7 - day of week of month 1)
- [ ] Calculate next-month days needed to complete last week
- [ ] Render all 3 month sections: prev-month days, current month days, next-month days
- [ ] Apply `OtherMonthDayStyle` (gray text, different background) to prev/next month days
- [ ] Compare rendered table row count with Web Forms reference — should match (6 weeks)
- [ ] Add test: March 2024 (starts Thursday) should show 2 Feb days + 31 Mar + 2 Apr = 42 cells
- [ ] Update Calendar sample to show full grid padding

---

### Issue 9: Fix D-14 — Calendar Style Property Pass-Through (TitleStyle, DayStyle, TodayDayStyle)
**Labels:** enhancement, divergence-registry, calendar, styles
**Priority:** High
**Description:** **D-14 (DIVERGENCE-REGISTRY):** Calendar style application is incomplete. The `<table>` and cell-level styles from style sub-properties are not fully applied. Prioritize `TitleStyle`, `DayStyle`, and `TodayDayStyle` first as the most commonly used. Currently Calendar properties use CSS strings instead of `TableItemStyle` objects, preventing cascading style inheritance.
**Acceptance criteria:**
- [ ] Change Calendar style properties from CSS string to `TableItemStyle` objects (or create `TableItemStyle` wrapper)
- [ ] Implement `TitleStyle` rendering on `<thead>` row
- [ ] Implement `DayStyle` rendering on regular `<td>` elements
- [ ] Implement `TodayDayStyle` rendering on today's `<td>` (overrides DayStyle)
- [ ] Test: verify BackColor, ForeColor, Font properties render as inline styles on cells
- [ ] Compare rendered Calendar HTML with Web Forms reference — cell styles should match
- [ ] Update Calendar sample page to show all 4 style properties

---

### Issue 10: GridView — Implement Paging (AllowPaging, PageSize, PageIndex, PagerSettings)
**Labels:** enhancement, gridview, data-controls
**Priority:** High
**Description:** GridView is at 20.7% health — the weakest data control. Paging is the #1 missing feature. Add `AllowPaging`, `PageSize`, `PageIndex`, `PagerSettings`, and `PagerStyle` properties. GridView is the most commonly used data control in Web Forms applications, making this the single biggest migration blocker.
**Acceptance criteria:**
- [ ] Add `[Parameter] public bool AllowPaging { get; set; }` to GridView
- [ ] Add `[Parameter] public int PageSize { get; set; }` (default 10)
- [ ] Add `[Parameter] public int PageIndex { get; set; }` (0-based)
- [ ] Create `PagerSettings` class with Position (Top/Bottom/TopAndBottom) and PageButtonCount properties
- [ ] Render pager UI with Previous/Next/numbered page buttons
- [ ] Add `PageIndexChanged` event callback when paging
- [ ] Test: 25-item dataset, PageSize=10 → renders 3 pages with correct item subsets
- [ ] Add GridView paging sample page with before/after screenshots

---

### Issue 11: GridView — Implement Sorting (AllowSorting, SortDirection, SortExpression)
**Labels:** enhancement, gridview, data-controls
**Priority:** High
**Description:** GridView sorting is completely missing. Add `AllowSorting`, `SortDirection`, `SortExpression` properties and `Sorting`/`Sorted` events. Critical for data display in Web Forms applications.
**Acceptance criteria:**
- [ ] Add `[Parameter] public bool AllowSorting { get; set; }` to GridView
- [ ] Add `[Parameter] public string SortExpression { get; set; }`
- [ ] Create `SortDirection` enum (Ascending=0, Descending=1)
- [ ] Add `[Parameter] public SortDirection SortDirection { get; set; }`
- [ ] Add `Sorting` and `Sorted` event callbacks
- [ ] Render column headers as clickable when AllowSorting=true
- [ ] Track sort state (expression, direction) and emit event on header click
- [ ] Test: click header, verify Sorting/Sorted events fire with correct expression/direction
- [ ] Add GridView sorting sample page with click-to-sort demo

---

### Issue 12: GridView — Implement Row Editing Events (RowEditing, RowUpdating, RowDeleting)
**Labels:** enhancement, gridview, data-controls
**Priority:** High
**Description:** GridView inline editing events are missing. Add `EditIndex`, `EditRowStyle`, and events: `RowEditing`, `RowUpdating`, `RowDeleting`, `RowCancelingEdit`. This unlocks CRUD workflows in GridView.
**Acceptance criteria:**
- [ ] Add `[Parameter] public int EditIndex { get; set; }` to GridView
- [ ] Create `EditRowStyle` class (inherits TableItemStyle with Background/Font/etc.)
- [ ] Add event callbacks: `RowEditing`, `RowUpdating`, `RowDeleting`, `RowCancelingEdit`
- [ ] When EditIndex is set, render row N with input controls instead of display text
- [ ] Apply EditRowStyle background/font to edit row
- [ ] Provide Edit/Update/Cancel buttons in edit row
- [ ] Test: set EditIndex=1, verify row 1 shows inputs; update data, verify RowUpdating event fires
- [ ] Add GridView editing sample page with CRUD demo

---

### Issue 13: Change Login Control Base Classes — Add WebControl Style Properties
**Labels:** enhancement, login-controls, styles
**Priority:** Medium
**Description:** `ChangePassword`, `CreateUserWizard`, `Login`, and `PasswordRecovery` all inherit `BaseWebFormsComponent` instead of `BaseStyledComponent`. They're missing BackColor, BorderColor, CssClass, Font, ForeColor, Height, Width, and Style (~8-10 properties each). Sub-element styles work via CascadingParameters, but the outer container cannot be styled.
**Acceptance criteria:**
- [ ] Change all 4 login controls to inherit `BaseStyledComponent`
- [ ] Verify outer container `<div>` renders style properties
- [ ] Add tests for BackColor, CssClass, Height, Width on Login and ChangePassword
- [ ] Update Login and ChangePassword sample pages showing outer container styling
- [ ] Audit scores for all 4 login controls improve by ~8-10 points each

---

### Issue 14: Improve L1 Script Automation — Push Coverage from 40% to 60%
**Labels:** enhancement, l1-script, migration-toolkit
**Priority:** Medium
**Description:** The L1 migration script (bwfc-migrate.ps1) currently automates ~40% of typical migration tasks. Analysis shows 6 OPPs (Opportunities) to push coverage to ~60%: enum/bool/unit string normalization, Response.Redirect shimming, GetRouteUrl shimming, Session pattern detection, ViewState visibility, and DataSourceID warnings.
**Acceptance criteria:**
- [ ] Implement automatic normalization of enum parameters (e.g., `string "1"` → `EnumType.Value`)
- [ ] Add Response.Redirect detection → emit `NavigationManager.NavigateTo()` template
- [ ] Add GetRouteUrl detection → emit `GetRouteUrl()` shim call template
- [ ] Add Session detection → emit HttpContextAccessor + Session warning template
- [ ] Add ViewState reference detection → emit ViewState/EnableViewState guidance
- [ ] Add DataSourceID detection → emit Items parameter conversion guidance
- [ ] Run on ContosoUniversity migration → measure automation %, target 60%+
- [ ] Document each OPP with before/after code samples

---

### Issue 15: Create L1 Script Test Harness — Measure Script Quality Metrics
**Labels:** enhancement, l1-script, testing, migration-toolkit
**Priority:** Medium
**Description:** There is no test harness for measuring L1 script quality. Create a comprehensive test harness that measures: automation coverage %, time to complete, compilation success, test pass rate, and divergence from Web Forms HTML output.
**Acceptance criteria:**
- [ ] Design test harness metrics (automation %, runtime, build success, test pass %, HTML divergence)
- [ ] Create 3-5 small reference projects (mini-apps covering common patterns)
- [ ] Run L1 script on each reference project → measure and log metrics
- [ ] Compare pre/post-migration build output (should be 0 errors)
- [ ] Create HTML diff report comparing migrated pages against Web Forms originals
- [ ] Document baseline metrics and target improvements for future runs
- [ ] Add test harness to CI/CD pipeline (automated after each L1 script change)

---

### Issue 16: HyperLink — Rename NavigationUrl to NavigateUrl
**Labels:** bug, controls, breaking-change
**Priority:** Medium
**Description:** HyperLink uses `NavigationUrl` but Web Forms uses `NavigateUrl`. This name mismatch blocks migration — migrated markup will have property mismatches. This is a breaking change but necessary for fidelity.
**Acceptance criteria:**
- [ ] Rename `NavigationUrl` parameter to `NavigateUrl` in HyperLink.razor
- [ ] Update migration toolkit to map `NavigationUrl` → `NavigateUrl` (if applicable)
- [ ] Update HyperLink sample page
- [ ] Run full test suite — verify no breaking changes to tests (use Find/Replace)
- [ ] Update audit score for HyperLink — should close the "wrong property name" gap

---

### Issue 17: ValidationSummary — Add HeaderText and ValidationGroup Properties
**Labels:** enhancement, validators
**Priority:** Medium
**Description:** `ValidationSummary` is missing `HeaderText` (displays above error list) and `ValidationGroup` (groups validators). These are key features in Web Forms validation workflows — most ValidationSummary usage includes HeaderText.
**Acceptance criteria:**
- [ ] Add `[Parameter] public string HeaderText { get; set; }` to ValidationSummary
- [ ] Add `[Parameter] public string ValidationGroup { get; set; }` to ValidationSummary
- [ ] Render HeaderText as `<h3>@HeaderText</h3>` above error list
- [ ] Filter displayed errors to match ValidationGroup (or show all if ValidationGroup empty)
- [ ] Test: ValidationSummary with HeaderText="Please fix:" shows header; filter by group
- [ ] Update ValidationSummary sample showing HeaderText usage

---

### Issue 18: Merge DetailsView from sprint3/detailsview-passwordrecovery Branch
**Labels:** bug, devops, controls
**Priority:** Medium
**Description:** DetailsView exists on unmerged branch `sprint3/detailsview-passwordrecovery` with 27 matching properties and 16 matching events. PasswordRecovery was already merged; DetailsView is the only remaining unmerged component. This blocks access to DetailsView features (CRUD support, auto-generated rows, edit mode). `status.md` incorrectly lists DetailsView as ✅ Complete when actual shipped count is 49/53 (92%), not 50/53 (94%).
**Acceptance criteria:**
- [ ] Merge `sprint3/detailsview-passwordrecovery` into `dev`
- [ ] Resolve any merge conflicts
- [ ] Verify DetailsView tests pass post-merge
- [ ] Update `status.md` to reflect actual shipped count (50/53, 94%)
- [ ] Rebase any ongoing milestones to include DetailsView

---

### Issue 19: FormView — Add CssClass, Header/Footer, and Empty Data Support
**Labels:** enhancement, data-controls
**Priority:** Medium
**Description:** FormView is at 34.9% health. Add container styling (CssClass, BackColor), header/footer templates, empty data template, and improve paging/mode support. FormView is a critical CRUD control for single-record display.
**Acceptance criteria:**
- [ ] Add `[Parameter] public string CssClass { get; set; }` to FormView
- [ ] Add `[Parameter] public RenderFragment HeaderTemplate { get; set; }`
- [ ] Add `[Parameter] public RenderFragment FooterTemplate { get; set; }`
- [ ] Add `[Parameter] public RenderFragment EmptyDataTemplate { get; set; }`
- [ ] Render header/footer around ItemTemplate content
- [ ] When data is empty, show EmptyDataTemplate instead of ItemTemplate
- [ ] Test: FormView with header, footer, empty data template all render correctly
- [ ] Update FormView sample showing all 4 template types

---

### Issue 20: Calendar — Convert Style Strings to TableItemStyle Objects
**Labels:** enhancement, calendar, styles
**Priority:** Low
**Description:** Calendar has 9 style sub-properties (DayStyle, TitleStyle, etc.) currently implemented as CSS string parameters instead of `TableItemStyle` objects. This prevents cascading style inheritance and doesn't match the Web Forms API shape. This is a follow-up to D-14 style improvements.
**Acceptance criteria:**
- [ ] Create or reuse `TableItemStyle` class with BackColor, ForeColor, Font, etc.
- [ ] Change Calendar style properties to use `TableItemStyle` instead of strings
- [ ] Verify all 9 style properties (DayStyle, TitleStyle, NextPrevStyle, SelectorStyle, WeekEndStyle, OtherMonthDayStyle, TodayDayStyle, SelectedDayStyle, WeekendDayStyle) work with new shape
- [ ] Test CSS generation from TableItemStyle properties
- [ ] Update Calendar sample showing style object usage

---

## Milestone: Advanced Features (M21)

### Issue 21: GridView — Implement Row Selection (SelectedIndex, SelectedRow, SelectedRowStyle)
**Labels:** enhancement, gridview, data-controls
**Priority:** Medium
**Description:** GridView row selection allows users to highlight and interact with specific rows. Add `SelectedIndex`, `SelectedRow` property access, `SelectedRowStyle`, and `SelectedIndexChanged` event.
**Acceptance criteria:**
- [ ] Add `[Parameter] public int SelectedIndex { get; set; }` to GridView
- [ ] Add `[Parameter] public GridViewRow SelectedRow { get; }` computed property
- [ ] Create `SelectedRowStyle` class (inherits TableItemStyle)
- [ ] Add `SelectedIndexChanged` event callback
- [ ] Render selected row with SelectedRowStyle background/font
- [ ] Add click-to-select behavior on rows
- [ ] Test: click row 2, verify SelectedIndex=2, SelectedIndexChanged fires
- [ ] Add GridView selection sample

---

### Issue 22: ListControl — Add DataTextFormatString Property
**Labels:** enhancement, data-controls
**Priority:** Low
**Description:** ListControl-based controls (BulletedList, CheckBoxList, DropDownList, ListBox, RadioButtonList) are missing `DataTextFormatString` to format bound data. Example: `DataTextFormatString="{0:C}"` for currency display.
**Acceptance criteria:**
- [ ] Add `[Parameter] public string DataTextFormatString { get; set; }` to ListControl base
- [ ] Apply format string to text property during data binding
- [ ] Test: format currency and date values using DataTextFormatString
- [ ] Update samples showing formatted list items

---

### Issue 23: ListControl — Add AppendDataBoundItems Property
**Labels:** enhancement, data-controls
**Priority:** Low
**Description:** `AppendDataBoundItems` property allows combining static items with data-bound items. Currently list controls replace all items when data binding occurs.
**Acceptance criteria:**
- [ ] Add `[Parameter] public bool AppendDataBoundItems { get; set; }` to ListControl
- [ ] When AppendDataBoundItems=true, preserve existing items before adding bound items
- [ ] Test: static items + data-bound items both present in rendered list
- [ ] Update sample showing static + dynamic item combinations

---

### Issue 24: Label — Add AssociatedControlID Property
**Labels:** enhancement, controls, accessibility
**Priority:** Low
**Description:** `Label.AssociatedControlID` renders `<label for="...">` to associate the label with a form control. This improves accessibility and allows click-to-focus behavior.
**Acceptance criteria:**
- [ ] Add `[Parameter] public string AssociatedControlID { get; set; }` to Label
- [ ] Render `<label for="@AssociatedControlID">` when AssociatedControlID is provided
- [ ] Test: click label, focus moves to associated control (via browser behavior)
- [ ] Update Label sample showing AssociatedControlID usage

---

### Issue 25: Panel — Add BackImageUrl Property
**Labels:** enhancement, controls, styles
**Priority:** Low
**Description:** Panel's `BackImageUrl` property renders a background image URL. Add support for `style="background-image: url(...)"` rendering.
**Acceptance criteria:**
- [ ] Add `[Parameter] public string BackImageUrl { get; set; }` to Panel
- [ ] Render background-image CSS property when BackImageUrl is provided
- [ ] Test: background image displays on Panel `<div>`
- [ ] Update Panel sample showing background image

---

## Milestone: Complete Data Control Support (M22)

### Issue 26: ListView — Add CRUD Events (ItemDeleting, ItemInserting, ItemUpdating)
**Labels:** enhancement, listview, data-controls
**Priority:** Medium
**Description:** ListView is at 34.3% health. Add full CRUD pipeline: `ItemDeleting`, `ItemInserting`, `ItemUpdating`, and related templates (InsertItemTemplate, EditItemTemplate, DeleteConfirmTemplate).
**Acceptance criteria:**
- [ ] Add event callbacks: ItemDeleting, ItemInserting, ItemUpdating, ItemCancelingEdit
- [ ] Add template parameters: InsertItemTemplate, EditItemTemplate, DeleteConfirmTemplate
- [ ] Render Insert/Edit/Delete buttons in items
- [ ] Test: delete item triggers ItemDeleting, insert triggers ItemInserting
- [ ] Add ListView CRUD sample page

---

### Issue 27: DataList — Implement Editing Support (EditItemIndex, EditItemStyle)
**Labels:** enhancement, datalist, data-controls
**Priority:** Medium
**Description:** DataList is at 73.0% health. Add `EditItemIndex` and `EditItemStyle` to enable inline editing, plus `EditCommand` and `UpdateCommand` events.
**Acceptance criteria:**
- [ ] Add `[Parameter] public int EditItemIndex { get; set; }` to DataList
- [ ] Create `EditItemStyle` class (inherits TableItemStyle)
- [ ] Add `EditCommand` and `UpdateCommand` event callbacks
- [ ] When EditItemIndex is set, render item with EditItemTemplate instead of ItemTemplate
- [ ] Test: set EditItemIndex=1, verify item renders in edit mode
- [ ] Add DataList editing sample

---

### Issue 28: Menu — Implement Orientation Property and Static/Dynamic Rendering
**Labels:** enhancement, menu, navigation
**Priority:** Medium
**Description:** Menu is at 37.7% health. Menu currently hardcodes vertical orientation. Add `Orientation` property (Horizontal/Vertical), Static/Dynamic submenu rendering, and CSS-based layout.
**Acceptance criteria:**
- [ ] Create `Orientation` enum (Horizontal=0, Vertical=1)
- [ ] Add `[Parameter] public Orientation Orientation { get; set; }` to Menu
- [ ] Render horizontal menu with `display:inline-block` items
- [ ] Render vertical menu with `display:block` items
- [ ] Implement Static submenu mode (always visible) vs Dynamic (hover)
- [ ] Add CSS for submenu positioning (below for horizontal, right for vertical)
- [ ] Test: Horizontal menu renders items in a row; Vertical in a column
- [ ] Add Menu orientation samples (horizontal and vertical)

---

### Issue 29: TreeView — Add Node-Level Styles (HoverNodeStyle, LeafNodeStyle, etc.)
**Labels:** enhancement, treeview, navigation
**Priority:** Low
**Description:** TreeView is at 57.1% health. Add fine-grained node-level styles: `HoverNodeStyle`, `LeafNodeStyle`, `ParentNodeStyle`, `SelectedNodeStyle` for visual control over different node types.
**Acceptance criteria:**
- [ ] Create node style classes (NodeStyle base with BackColor, ForeColor, etc.)
- [ ] Add HoverNodeStyle, LeafNodeStyle, ParentNodeStyle, SelectedNodeStyle parameters
- [ ] Apply appropriate style when node is hovered, leaf, parent, or selected
- [ ] Test: hover node shows HoverNodeStyle, leaf nodes show LeafNodeStyle
- [ ] Add TreeView node styling sample

---

## Milestone: Fine-Tuning & Edge Cases (M23)

### Issue 30: Validator ControlToValidate — Support String ID for Backward Compatibility
**Labels:** enhancement, validators, migration-toolkit
**Priority:** Low
**Description:** Current `ControlToValidate` uses `ForwardRef` pattern, which requires a direct component reference. Web Forms uses string-based control ID lookup. Consider adding string ID support for migrated pages that reference controls by ID string instead of reference.
**Acceptance criteria:**
- [ ] Add `[Parameter] public string ControlToValidateId { get; set; }` (optional alternative to ForwardRef)
- [ ] When ControlToValidateId is provided, use JS interop to look up element by ID
- [ ] Maintain backward compatibility — ForwardRef still works
- [ ] Test: both ForwardRef and ControlToValidateId patterns work
- [ ] Update migration toolkit to emit ControlToValidateId for string-based references

---

### Issue 31: Add CausesValidation/ValidationGroup Support to CheckBox, RadioButton, TextBox
**Labels:** enhancement, validators
**Priority:** Low
**Description:** These controls should support `CausesValidation` (postback triggers validation) and `ValidationGroup` (which validators to trigger). Currently only command controls (Button, LinkButton, ImageButton) have these.
**Acceptance criteria:**
- [ ] Add `[Parameter] public bool CausesValidation { get; set; }` to CheckBox, RadioButton, TextBox
- [ ] Add `[Parameter] public string ValidationGroup { get; set; }` to all three
- [ ] Test: validation only runs for controls in the specified group
- [ ] Update samples showing grouped validation scenarios

---

### Issue 32: FormView/DetailsView — Add Orientation and TextLayout Properties
**Labels:** enhancement, login-controls, data-controls
**Priority:** Low
**Description:** Login controls and FormView have layout variants (Horizontal vs Vertical item layout). Add `Orientation` property to control label/input arrangement.
**Acceptance criteria:**
- [ ] Create `Orientation` enum if not already present
- [ ] Add `[Parameter] public Orientation Orientation { get; set; }` to FormView/DetailsView
- [ ] Render horizontal: labels and inputs side-by-side
- [ ] Render vertical: labels above inputs
- [ ] Test: both orientations render correctly
- [ ] Add sample showing both layouts

---

### Issue 33: DataGrid — Deprecation Notice and Migration Guidance
**Labels:** documentation, data-controls, migration-toolkit
**Priority:** Low
**Description:** DataGrid is at 44.6% health and is deprecated in Web Forms in favor of GridView. Create clear documentation and migration guidance to push users toward GridView equivalents. Update DataGrid samples to recommend GridView.
**Acceptance criteria:**
- [ ] Add deprecation notice to DataGrid component docs
- [ ] Create migration guide: DataGrid → GridView with feature mapping
- [ ] Update DataGrid sample page with "Consider using GridView" banner
- [ ] Add FAQ: when to use DataGrid vs GridView
- [ ] Update audit report with migration guidance

---

### Issue 34: Chart Component — Document Intentional Divergences and Limitations
**Labels:** documentation, chart
**Priority:** Low
**Description:** Chart is at 32.3% health due to architectural divergence (using Chart.js canvas instead of GDI+ server-side rendering). Document why certain properties are not implemented and provide workarounds/alternatives.
**Acceptance criteria:**
- [ ] Create CHART-DIVERGENCES.md explaining Canvas vs GDI+ architectural differences
- [ ] Document which properties are intentionally not implemented (Annotations, Image storage, Serializer)
- [ ] Provide Chart.js alternative patterns for common use cases
- [ ] Add to Chart sample page as developer guidance
- [ ] Link divergence doc from main audit report

---

## Summary

**Total: 34 issues across 4 milestones**

| Milestone | High Priority | Medium Priority | Low Priority | Total |
|-----------|---------------|-----------------|--------------|-------|
| M20: Parity | 13 | 2 | 0 | 15 |
| M21: Advanced | 0 | 3 | 4 | 7 |
| M22: CRUD | 0 | 5 | 1 | 6 |
| M23: Fine-tuning | 0 | 0 | 6 | 6 |
| **TOTAL** | **13** | **10** | **11** | **34** |

**Estimated impact:**
- **M20 (Component Parity):** 180+ gaps closed via base class fixes; 3 critical divergence bugs fixed (D-11, D-13, D-14)
- **M21 (Advanced):** GridView reach 60%+ health; complete selection/paging/sorting trio
- **M22 (CRUD):** Full CRUD support for ListView, DataList, FormView; Menu accessibility parity
- **M23 (Fine-tuning):** Edge cases, accessibility, edge-case validation patterns, deprecation guidance

**Expected outcome after all 34 issues:**
- Project health increases from 68.5% to ~78-80%
- GridView moves from 20.7% to 70%+
- Data controls category moves from 53.2% to 75%+
- All base class inheritance gaps closed
- Full component parity for top 35 controls

---

**Maintenance Notes:**
- Issues 1-6 are foundational — complete before M20 data control issues
- Issues 7-9 are divergence fixes — fix before rerunning audit
- Issues 10-12 form GridView paging/sorting/editing trio — can run in parallel
- Issues 21-29 depend on M20 base class fixes being complete
- Issues 30-34 are optional polish — low impact, low priority
