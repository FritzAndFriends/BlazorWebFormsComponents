# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
<!-- ⚠ Summarized 2026-02-23 by Scribe — original entries covered 2026-02-10 through 2026-02-12 -->

### Summary: Milestones 1–3 QA (2026-02-10 through 2026-02-12)

Triaged PR #333 (Calendar regression — closed, work on dev). Wrote 71 bUnit tests for Sprint 3 (42 DetailsView + 29 PasswordRecovery). Key patterns: DetailsView is generic `DataBoundComponent<ItemType>`, PasswordRecovery needs NavigationManager mock, both use .razor test files via BlazorWebFormsTestContext. Total: 797 tests.

### Summary: Milestone 4 Chart QA (2026-02-12)

Wrote 152 bUnit tests for Chart (140 original + 12 data binding). Chart tests use `BunitContext` directly with `JSInterop.Mode = JSRuntimeMode.Loose`. ChartConfigBuilder is most testable part (pure static). `GetPaletteColors` is internal — tested indirectly via dataset colors. ChartSeriesDataBindingHelper documents expected data binding contract.

### Summary: Feature Audit — Validation + Login Controls (2026-02-12)

Audited 13 controls. Key findings: Display property missing from all validators (migration-blocking). SetFocusOnError missing. ValidationSummary gaps (HeaderText, ShowMessageBox, ShowSummary, ValidationGroup) and comma-split bug. Login controls missing outer WebControl styles. ChangePassword/CreateUserWizard have template support; Login LayoutTemplate not supported; PasswordRecovery missing from current branch.

### Summary: Milestone 6 P0 QA (2026-02-23)

Wrote 44 bUnit tests for P0 base class changes: AccessKey (4), ToolTip (8), ImageStyle (7), LabelStyle (7), StyleInheritance for DataBound (8), ValidatorDisplay (7), SetFocusOnError (3). Fixed DataList duplicate AccessKey parameter bug. Found AccessKey not actually added to base class by Cyclops — added it to BaseWebFormsComponent as unblock. Test patterns: WebColor "LightGray"→"LightGrey" via ColorTranslator; use unambiguous colors. EditForm+InputText pattern for validator tests. JSInterop mock for SetFocusOnError. 986 tests pass.

📌 Test pattern: Validator Display tests use EditForm + InputText + RequiredFieldValidator. Display=Static → visibility:hidden, Display=Dynamic → display:none, Display=None → always display:none. SetFocusOnError uses JSInterop.SetupVoid/VerifyInvoke. — Rogue

📌 Test pattern: BaseListControl.GetItems() applies DataTextFormatString to both static and data-bound items. AppendDataBoundItems=false replaces static items. When Items is null, static items always show. — Rogue

📌 Test pattern: Menu Orientation tests require JSInterop.Mode = JSRuntimeMode.Loose and @using BlazorWebFormsComponents.Enums. Login control tests require AuthenticationStateProvider and NavigationManager mock services. — Rogue

 Team update (2026-02-23): BaseDataBoundComponent now inherits BaseStyledComponent  removed duplicate IStyle from 11 data controls  decided by Cyclops
 Team update (2026-02-23): BaseListControl<TItem> introduced as shared base for 5 list controls (DataTextFormatString, AppendDataBoundItems)  decided by Cyclops
 Team update (2026-02-23): CausesValidation/ValidationGroup added to CheckBox, RadioButton, TextBox  decided by Cyclops
 Team update (2026-02-23): Label AssociatedControlID switches rendered element (label vs span)  decided by Cyclops
 Team update (2026-02-23): Milestone 6 Work Plan ratified  54 WIs across P0/P1/P2 tiers  decided by Forge

### Milestone 7: GridView Feature Tests — WI-03 + WI-06 + WI-08

Wrote 24 bUnit tests across 3 new test files for GridView features:

**Selection.razor (WI-03, 7 tests):** SelectedIndex with SelectedRowStyle CSS, SelectedIndexChanging event with correct index, cancellation prevents selection, AutoGenerateSelectButton renders Select links, SelectedValue returns DataKeyNames key, SelectedIndex=-1 clears selection, SelectedRow returns correct data item. Used `FindComponent<GridView<T>>().Instance` to verify SelectedRow/SelectedValue properties.

**StyleSubComponents.razor (WI-06, 8 tests):** RowStyle applies to even-indexed data rows, AlternatingRowStyle applies to odd-indexed rows, HeaderStyle on thead tr, FooterStyle on tfoot tr, EmptyDataRowStyle on empty data, PagerStyle on pager row, EditRowStyle on row at EditIndex, style priority chain (Edit > Selected > Alternating > Row).

**DisplayProperties.razor (WI-08, 9 tests):** ShowHeader=false hides thead, ShowFooter=true shows tfoot, Caption renders caption element, CaptionAlign renders correct caption-side/text-align style, EmptyDataTemplate overrides EmptyDataText, GridLines renders rules attribute, UseAccessibleHeader renders th scope="col", CellPadding/CellSpacing render table attributes, ShowHeaderWhenEmpty shows header with no data.

📌 Test pattern: GridView style sub-components use named RenderFragments (`<RowStyleContent>`, etc.) containing `<GridViewRowStyle BackColor="color" />`. The sub-component configures the GridView's TableItemStyle via CascadingParameter "ParentGridView". Styles render as inline `style` attributes on `<tr>` elements. — Rogue

📌 Test pattern: GridView AlternatingRowStyle is always initialized (non-null `new TableItemStyle()`), so `GetRowStyle()` returns it for odd rows even when not configured. RowStyle only applies to even-indexed rows; to style ALL rows, set both RowStyle and AlternatingRowStyle. — Rogue

📌 Test pattern: GridView with `AutoGenerateColumns="false"` renders in two passes: first pass initializes style sub-components (empty table), second pass renders table after BoundField children register via `AddColumn`. bUnit waits for both renders. — Rogue

### Milestone 6: P2 Feature Tests — ListView CRUD, DataGrid Styles+Events, Menu Level Styles, Panel BackImageUrl, Login Orientation

Wrote 41 bUnit tests across 6 new test files for P2 features:

**CrudEvents.razor (ListView, 12 tests):** HandleCommand routes Edit→sets EditIndex+fires ItemEditing, Cancel→clears EditIndex+fires ItemCanceling, Delete→fires ItemDeleting+ItemDeleted, Insert→fires ItemInserting+ItemInserted, Update→fires ItemUpdating+ItemUpdated. Unknown command→fires ItemCommand. EditIndex=-1 shows ItemTemplate for all. EmptyItemTemplate renders when Items empty. InsertItemTemplate at FirstItem/LastItem positions. ItemEditing cancellation prevents EditIndex change. EditItemTemplate rendering verified via HandleCommand (EditIndex set confirmed; template re-evaluation is a known component gap).

**StyleSubComponents.razor (DataGrid, 11 tests):** DataGridItemStyle on data rows, AlternatingItemStyle on odd rows, HeaderStyle on thead, FooterStyle on tfoot, PagerStyle on pager row, SelectedItemStyle at SelectedIndex, EditItemStyle at EditItemIndex. Caption+CaptionAlign render correctly. GridLines→rules attribute. UseAccessibleHeader→th scope=col. CellPadding+CellSpacing on table.

**Events.razor (DataGrid, 3 tests):** PageIndexChanged fires when pager link clicked, SortCommand fires on sortable header click, SelectedIndex default is -1.

**LevelStyles.razor (Menu, 7 tests):** LevelMenuItemStyles applies per-depth CssClass, Level 1 items get index 0 style, Level 2 items get index 1 style, LevelSelectedStyles applies after click, LevelSubMenuStyles applies background-color to submenu ul, Level styles override static/dynamic styles.

**BackImageUrl.razor (Panel, 3 tests):** BackImageUrl renders as background-image in style, not set→no background-image, URL-encoded characters preserved.

**LoginOrientation.razor (Login, 5 tests):** Default Orientation is Vertical (fields in separate rows), Horizontal puts all fields in one row with 4 tds, TextOnLeft labels align right, TextOnTop labels above inputs with colspan, Horizontal+TextOnTop puts both labels in same row.

📌 Test pattern: ListView HandleCommand must be called via `cut.InvokeAsync()` because it calls StateHasChanged() which requires the Blazor Dispatcher context. Direct `await theListView.HandleCommand()` throws InvalidOperationException. — Rogue

📌 Test pattern: DataGrid style sub-components use named RenderFragments (`<ItemStyleContent>`, `<HeaderStyleContent>`, etc.) containing `<DataGridItemStyle BackColor="color" />`. WebColor values must be declared as variables (`WebColor red = "Red";`) then passed as `BackColor="red"` — not string literals. — Rogue

📌 Test pattern: Menu tests that use `FindAll("a")` must use `FindAll("li a")` to exclude the accessibility skip-link `<a>` rendered before the menu. JSInterop.Mode = JSRuntimeMode.Loose required for all Menu tests. — Rogue

📌 Test pattern: Login control tests require AuthenticationStateProvider + NavigationManager mock services. Use `Orientation ori = Orientation.Horizontal;` variable pattern to avoid Razor enum name collision. Login renders 4 layout combos based on Orientation × TextLayout (Vertical/Horizontal × TextOnLeft/TextOnTop). — Rogue
