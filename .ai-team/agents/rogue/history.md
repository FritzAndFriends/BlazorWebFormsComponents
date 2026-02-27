# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- ⚠ Summarized 2026-02-25 by Scribe — older entries condensed into Core Context -->

### Core Context (2026-02-10 through 2026-02-25)

M1-3 QA: Triaged PR #333 Calendar (closed, work on dev). 71 bUnit tests for Sprint 3 (42 DetailsView + 29 PasswordRecovery). DetailsView is generic DataBoundComponent<ItemType>, PasswordRecovery needs NavigationManager mock. M4: 152 bUnit tests for Chart (BunitContext with JSInterop.Mode=Loose). Feature audit: Display missing from all validators (migration-blocking), ValidationSummary comma-split bug, Login controls missing outer styles. M6 P0: 44 tests for base class changes (AccessKey, ToolTip, ImageStyle, LabelStyle). Fixed DataList duplicate AccessKey bug. WebColor "LightGray"→"LightGrey" via ColorTranslator.

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

 Team update (2026-02-25): HTML audit milestones M11-M13 defined, existing M12M14, Skins/ThemesM15+  decided by Forge per Jeff's directive

### Post-Bug-Fix Capture Pipeline (2026-02-26)

Re-ran full HTML capture pipeline after 14 bug fixes across 10 controls. Results: 132→131 divergences, 0→1 exact match (Literal-3). All 11 targeted controls show verified structural improvements: Button (`<button>`→`<input>`), BulletedList (span removal), LinkButton (href addition), Calendar (border styling + tbody), CheckBox (span removal), Image (longdesc removal), DataList (spurious style removal), GridView (rules/border addition), TreeView (compacted output). 11 new Blazor captures gained (75→64 missing). Primary blocker identified: sample data parity, not component bugs.

📌 Pipeline note: `normalize-html.mjs --compare` compares RAW files, not normalized. For accurate comparison, run `--input`/`--output` normalization on both sides first, then `--compare` the normalized directories. — Rogue

📌 Pipeline note: The Blazor and WebForms samples use completely different data/text/IDs. Until sample data is aligned, the pipeline cannot distinguish component bugs from data differences. Calendar is the closest complex control at 73% word similarity. — Rogue

 Team update (2026-02-26): NamingContainer inherits BaseWebFormsComponent, UseCtl00Prefix handled in ComponentIdGenerator  decided by Cyclops

 Team update (2026-02-26): Menu RenderingMode=Table uses inline Razor to avoid whitespace; AngleSharp foster-parenting workaround  decided by Cyclops

 Team update (2026-02-26): Login+Identity strategy: handler delegates in core, separate Identity NuGet package, redirect-based cookie flows  decided by Forge

 Team update (2026-02-26): Data control divergence: 90%+ sample parity, 5 genuine bugs (3 P1, 2 P2), normalization pipeline gaps  decided by Forge

 Team update (2026-02-26): Post-fix capture confirms sample data alignment is P0 blocker for match rates  decided by Rogue

 Team update (2026-02-26): WebFormsPage unified wrapper  inherits NamingContainer, adds Theme cascading, replaces separate wrappers  decided by Jeffrey T. Fritz, Forge
 Team update (2026-02-26): Login+Identity controls deferred to future milestone  do not schedule tests  decided by Jeffrey T. Fritz

### Milestone 16: LoginView/PasswordRecovery OuterStyle Tests (#352, #354)

Wrote 18 bUnit tests across 2 new test files for BaseStyledComponent style property rendering on login controls:

**OuterStyle.razor (LoginView, 9 tests):** CssClass renders on outer element, no class when unset, ToolTip as title attribute, no title when unset, BackColor renders background-color in style, ForeColor renders color in style, Font-Bold renders font-weight:bold, combined CssClass+BackColor+ToolTip, template content still renders inside styled wrapper. Tests find outer element by `#ID` selector — will pass after Cyclops adds styled wrapper in #352.

**OuterStyle.razor (PasswordRecovery, 9 tests):** CssClass renders on outer `<table>`, no class when unset, ToolTip as title attribute, no title when unset, BackColor renders background-color, ForeColor renders color, Font-Bold renders font-weight:bold, combined CssClass+BackColor+ToolTip, border-collapse:collapse always present in style. Tests use `table#ID` selector matching existing PasswordRecovery markup.

**Panel BackImageUrl:** Tests already existed from Milestone 6 P2 (3 tests in `Panel/BackImageUrl.razor`). No new tests needed.

📌 Test pattern: LoginView tests require AuthenticationStateProvider mock (anonymous user). PasswordRecovery tests require NavigationManager mock. Both use fully-qualified `BlazorWebFormsComponents.LoginControls.LoginView` in Razor markup to avoid ambiguity. Find outer element by `#ID` for LoginView (wrapper element unknown until Cyclops implements), `table#ID` for PasswordRecovery (outer table already renders ID/CssClass/Style/ToolTip). — Rogue

📌 Test pattern: PasswordRecovery outer table always includes `border-collapse:collapse` in style — assertions for BackColor/ForeColor should check `ShouldContain` not exact match. LoginView currently has no styled wrapper; #352 will add one. — Rogue

### ClientIDMode Feature Tests

Wrote 12 bUnit tests in `ClientIDMode/ClientIDModeTests.razor` covering all 4 ClientIDMode values (Static, Predictable, AutoID, Inherit) plus edge cases.

**Static Mode (3 tests):** Raw ID without parent prefix, inside NamingContainer ignores parent, nested NamingContainers still only raw ID.

**Predictable Mode (3 tests):** Parent_Child pattern inside NamingContainer, Outer_Inner_Component with nesting, does NOT include ctl00 even with UseCtl00Prefix=true.

**AutoID Mode (2 tests):** Includes ctl00 prefix from NamingContainer with UseCtl00Prefix=true, nested containers with selective ctl00 prefix.

**Inherit Mode (2 tests):** Default resolves to Predictable behavior, walks up to parent's mode (parent with Static → child inherits Static).

**Edge Cases (2 tests):** No ID set returns null ClientID regardless of mode, Static mode without NamingContainer returns raw ID.

📌 Test pattern: ClientIDMode tests use Button as the test component (renders `<input id="@ClientID">`). `@using BlazorWebFormsComponents.Enums` is required for the enum. ClientIDMode is set directly on the component via `ClientIDMode="ClientIDMode.Static"` etc. — Rogue

📌 Finding: The existing `NamingContainerTests.UseCtl00Prefix_PrependsCtl00ToClientID` test FAILS after the ClientIDMode feature because UseCtl00Prefix only takes effect in `BuildAutoID()` mode but the default Inherit→Predictable skips ctl00. The test needs `ClientIDMode="ClientIDMode.AutoID"` on the Button, OR the NamingContainer should auto-set AutoID mode when UseCtl00Prefix=true. — Rogue

 Team update (2026-02-26): ClientIDMode tests consolidated with Cyclops implementation  backward compat regression fixed via NamingContainer auto-AutoID  decided by Cyclops, Rogue
 Team update (2026-02-26): Data control divergence analysis consolidated  sample parity is #1 blocker, 5 genuine bugs identified  decided by Forge, Rogue

 Team update (2026-02-27): Branching workflow directive  feature PRs from personal fork to upstream dev, only devmain on upstream  decided by Jeffrey T. Fritz

 Team update (2026-02-27): Issues must be closed via PR references using 'Closes #N' syntax, no manual closures  decided by Jeffrey T. Fritz
