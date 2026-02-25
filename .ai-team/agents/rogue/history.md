# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- ⚠ Summarized 2026-02-25 by Scribe — older entries condensed into Core Context -->

### Core Context (2026-02-10 through 2026-02-25)

M1-3 QA: Triaged PR #333 Calendar (closed, work on dev). 71 bUnit tests for Sprint 3 (42 DetailsView + 29 PasswordRecovery). DetailsView is generic DataBoundComponent<ItemType>, PasswordRecovery needs NavigationManager mock. M4: 152 bUnit tests for Chart (BunitContext with JSInterop.Mode=Loose). Feature audit: Display missing from all validators (migration-blocking), ValidationSummary comma-split bug, Login controls missing outer styles. M6 P0: 44 tests for base class changes (AccessKey, ToolTip, ImageStyle, LabelStyle). Fixed DataList duplicate AccessKey bug. WebColor "LightGray"→"LightGrey" via ColorTranslator.

**Key patterns:** Validator Display tests use EditForm + InputText + RequiredFieldValidator. BaseListControl.GetItems() applies DataTextFormatString to both static and data-bound items. Menu tests use `FindAll("li a")` to exclude skip-link. Login tests require AuthenticationStateProvider + NavigationManager mocks. `Orientation ori = Orientation.Horizontal;` variable pattern avoids Razor enum collision.

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

📌 Test pattern: Menu Orientation tests require JSInterop.Mode = JSRuntimeMode.Loose and @using BlazorWebFormsComponents.Enums. Login control tests require AuthenticationStateProvider and NavigationManager mock services. — Rogue

 Team update (2026-02-24): M8 scope excludes version bump to 1.0 and release  decided by Jeffrey T. Fritz
 Team update (2026-02-24): PagerSettings shared sub-component created  may need bUnit tests  decided by Cyclops

### Milestone 9:Migration Fidelity QA — WI-02 + WI-06

Wrote 24 bUnit tests across 2 files for migration fidelity work:

**ToolTipTests.razor (WI-02, 20 tests):** Extended existing file with 20 new tests. 9 controls gained ToolTip from BaseStyledComponent: Label (span title), TextBox (input title via CalculatedAttributes), CheckBox (span title), RadioButton (span title), Panel (div title), Table (table title), DropDownList (select title), ListBox (select title), LinkButton (a title). Each tested with ToolTip present and absent. HyperLink regression test added. All 3 regression controls (Button, Image, HyperLink) confirmed working after base class move.

**CommaSplitTests.razor (WI-06, 4 tests):** Validation message format is `Text,ErrorMessage\x1F ValidationGroup`. The comma-split fix uses `IndexOf(',')` + `Substring()` instead of `Split(',')` so commas in ErrorMessage are preserved. Tests: single comma in message, multiple commas, no commas, empty ErrorMessage. All pass.

📌 Test pattern: Validation messages are stored as `Text + "," + ErrorMessage + "\x1F" + ValidationGroup` by BaseValidator. AspNetValidationSummary extracts ErrorMessage using first-comma split (IndexOf + Substring). ErrorMessage with commas is preserved correctly. — Rogue

📌 Test pattern: ToolTip renders as `title` attribute on the outermost element. TextBox adds it via CalculatedAttributes dictionary (not direct markup). CheckBox/RadioButton render title on the wrapping `<span>` when Text is present, on `<input>` when no Text. Panel renders on `<div>` (no GroupingText) or `<fieldset>` (with GroupingText). — Rogue

 Team update (2026-02-25): ToolTip moved to BaseStyledComponent (28+ controls), ValidationSummary comma-split fixed, SkinID boolstring fixed  decided by Cyclops
 Team update (2026-02-25): M9 plan ratified  12 WIs, migration fidelity  decided by Forge

 Team update (2026-02-25): TreeView NodeImage now checks ShowExpandCollapse independently of ShowLines; ExpandCollapseImage() helper added (#361)  decided by Cyclops


 Team update (2026-02-25): M12 introduces Migration Analysis Tool PoC (`bwfc-migrate` CLI, regex-based ASPX parsing, 3-phase roadmap)  decided by Forge



 Team update (2026-02-25): Future milestone work should include a doc review pass to catch stale 'NOT Supported' entries  decided by Beast

 Team update (2026-02-25): Shared sub-components of sufficient complexity get their own doc page (e.g., PagerSettings)  decided by Beast

 Team update (2026-02-25): All login controls (Login, LoginView, ChangePassword, PasswordRecovery, CreateUserWizard) now inherit from BaseStyledComponent  decided by Cyclops

 Team update (2026-02-25): ComponentCatalog.cs now links all sample pages; new samples must be registered there  decided by Jubilee

 Team update (2026-02-25): ListView now has full CRUD event parity (Sorting/Sorted, SelectedIndexChanging/Changed, PagePropertiesChanging/Changed, LayoutCreated)  decided by Cyclops
 Team update (2026-02-25): Menu styles use MenuItemStyle pattern (not UiTableItemStyle); IMenuStyleContainer interface added  decided by Cyclops
 Team update (2026-02-25): 5 missing smoke tests added for ListView CrudOps, Label, Panel BackImageUrl, LoginControls Orientation, DataGrid Styles  decided by Colossus

 Team update (2026-02-25): All new work MUST use feature branches pushed to origin with PR to upstream/dev. Never commit directly to dev.  decided by Jeffrey T. Fritz


 Team update (2026-02-25): Theme core types (#364) use nullable properties for StyleSheetTheme semantics, case-insensitive keys, empty-string default skin key. ThemeProvider is infrastructure, not a WebForms control. GetSkin returns null for missing entries.  decided by Cyclops


 Team update (2026-02-25): SkinID defaults to empty string, EnableTheming defaults to true. [Obsolete] removed  these are now functional [Parameter] properties.  decided by Cyclops


 Team update (2026-02-25): ThemeConfiguration CascadingParameter wired into BaseStyledComponent (not BaseWebFormsComponent). ApplySkin runs in OnParametersSet with StyleSheetTheme semantics. Font properties checked individually.  decided by Cyclops



 Team update (2026-02-25): HTML audit strategy approved  decided by Forge
