# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- ⚠ Summarized 2026-02-25 by Scribe — covers M1–M9 + early M10 -->

### Core Context (2026-02-10 through 2026-02-25)

**M1–M3 components:** Calendar (enum fix, async events), ImageMap (BaseStyledComponent, Guid IDs), FileUpload (InputFile integration, path sanitization), PasswordRecovery (3-step wizard), DetailsView (DataBoundComponent<T>, auto-field reflection, 10 events), Chart (BaseStyledComponent, CascadingValue "ParentChart", JS interop via ChartJsInterop, ChartConfigBuilder pure static).

**M6 base class fixes:** DataBoundComponent chain → BaseStyledComponent (14 data controls). BaseListControl<TItem> for 5 list controls. CausesValidation on CheckBox/RadioButton/TextBox. Label AssociatedControlID switches span→label. Login/ChangePassword/CreateUserWizard → BaseStyledComponent. Validator ControlToValidate dual-path: ForwardRef + string ID via reflection.

**M6 Menu overhaul:** → BaseStyledComponent. Selection tracking (SelectedItem/SelectedValue, MenuItemClick, MenuItemDataBound). MenuEventArgs, MaximumDynamicDisplayLevels, Orientation enum + CSS horizontal class, MenuLevelStyle lists.

**M7 style sub-components:** GridView (8), DetailsView (10), FormView (7), DataGrid (7) — all CascadingParameter + UiTableItemStyle. Style priority: Edit > Selected > Alternating > Row. TreeView: TreeNodeStyle + 6 sub-components, selection, ExpandAll/CollapseAll, FindNode, ExpandDepth, NodeIndent. GridView: selection, 10 display props. FormView/DetailsView events + PagerTemplate + Caption. DataGrid paging/sorting. ListView CRUD events + templates. Panel BackImageUrl. Login Orientation + TextLayout.

**M8 bug fixes:** Menu JS null guard, Calendar conditional scope, Menu auto-ID. Shared PagerSettings sub-component (12 props, IPagerSettingsContainer) wired into GridView/FormView/DetailsView.

**M9 migration-fidelity:** ToolTip → BaseStyledComponent (removed from 8 components, added `title="@ToolTip"` to 32 components). ValidationSummary comma-split fix. SkinID bool→string fix. TreeView NodeImage fallback restructured (ShowExpandCollapse check + ExpandCollapseImage helper).

**M10 batch 1:** Panel BackImageUrl already done. LoginView/PasswordRecovery → BaseStyledComponent. Login controls with `<table>` add `class="@CssClass" style="border-collapse:collapse;@Style" title="@ToolTip"`.

<!-- ⚠ Summarized 2026-02-25 by Scribe — original entries covered Milestone 6–7 implementation details -->

### Summary: Milestone 6 P0/P1 Implementation (2026-02-23)

**Base class fixes:** DataBoundComponent chain → BaseStyledComponent (WI-07, 14 data controls). BaseListControl<TItem> created for 5 list controls with DataTextFormatString + AppendDataBoundItems (WI-47/48). CausesValidation added to CheckBox/RadioButton/TextBox (WI-49). Label AssociatedControlID switches span→label (WI-51). Login/ChangePassword/CreateUserWizard → BaseStyledComponent (WI-52). Validator ControlToValidate dual-path: ForwardRef (ControlRef) + string ID via reflection (WI-36).

**Menu overhaul (WI-19/21/23/47/50):** → BaseStyledComponent. Selection tracking (SelectedItem/SelectedValue, MenuItemClick, MenuItemDataBound). MenuEventArgs, Value, Target, ValuePath, SkipLinkText. MaximumDynamicDisplayLevels. Orientation enum + CSS horizontal class. MenuLevelStyle (public IStyle class) with LevelMenuItemStyles/LevelSelectedStyles/LevelSubMenuStyles lists.

### Summary: Milestone 7 Data Control Depth (2026-02-24)

**Style sub-components:** GridView (8 styles, IGridViewStyleContainer), DetailsView (10 styles), FormView (7 styles), DataGrid (7 styles, IDataGridStyleContainer). All follow CascadingParameter pattern with UiTableItemStyle base. Style priority: Edit > Selected > Alternating > Row.

**TreeView enhancements (WI-11/13/15):** TreeNodeStyle (5 props) + 6 sub-components. Selection support (SelectedNode, SelectedNodeChanged, keyboard). ExpandAll/CollapseAll, FindNode, ExpandDepth, NodeIndent, PathSeparator.

**GridView (WI-02/05/07):** Selection (SelectedIndex, AutoGenerateSelectButton, SelectedRow/Value). 10 display props (ShowHeader/Footer, Caption, GridLines, CellPadding/Spacing, EmptyDataTemplate, UseAccessibleHeader).

**Other controls:** FormView events (ModeChanged, ItemCommand, paging with cancellation) + PagerTemplate + Caption (WI-31/33). DetailsView Caption + PageCount (WI-28). DataGrid paging/sorting events (WI-45). ListView 10 CRUD events + EditItemTemplate/InsertItemTemplate (WI-41). Panel BackImageUrl (WI-48). Login/ChangePassword Orientation + TextLayout (WI-49, LoginTextLayout enum).

**Key patterns:** Orientation enum collides with parameter name in Razor — use `Enums.Orientation.Vertical` fully-qualified.

📌 Team update (2026-02-23): Milestone 6 Work Plan ratified — 54 WIs across P0/P1/P2 tiers targeting ~345 feature gaps — decided by Forge
📌 Team update (2026-02-23): UI overhaul requested — ComponentCatalog (UI-2) and search (UI-8) assigned to Cyclops — decided by Jeffrey T. Fritz
**Key patterns:** `_ = callback.InvokeAsync()` for render-time events. `Path.GetFileName()` for file save security. Orientation enum collides with parameter name in Razor — use `Enums.Orientation.Vertical`. CI secret-gating: use env var indirection, not `secrets.*` in step-level `if:`.

📌 Team update (2026-02-25): M12 introduces Migration Analysis Tool PoC — decided by Forge

### ListView CRUD Events Completion (#356)

- **Sorting/Sorted events:** Added `ListViewSortEventArgs` (SortExpression, SortDirection, Cancel) and `Sorting`/`Sorted` EventCallback parameters. Sort command routed through `HandleCommand("Sort", expression, index)`. Toggles direction when sorting same expression (matches GridView pattern). `SortExpression` and `SortDirection` properties added to ListView.
- **SelectedIndexChanging/SelectedIndexChanged events:** Added `ListViewSelectEventArgs` (NewSelectedIndex, Cancel) and `SelectedIndexChanging`/`SelectedIndexChanged` EventCallback parameters. Select command routed through `HandleCommand("Select", null, index)`. Follows GridView `SelectRow` pattern with cancellation support.
- **PagePropertiesChanging/PagePropertiesChanged events:** Added `ListViewPagePropertiesChangingEventArgs` (StartRowIndex, MaximumRows) and `PagePropertiesChanging`/`PagePropertiesChanged` EventCallback parameters. Exposed via `SetPageProperties(startRowIndex, maximumRows)` public method. Added `StartRowIndex` and `MaximumRows` properties.
- **LayoutCreated event:** Converted from `EventHandler OnLayoutCreated` to `EventCallback<EventArgs> OnLayoutCreated`. Wired invocation via `RaiseLayoutCreated()` internal method called in ListView.razor after LayoutTemplate is resolved.
- **Key patterns followed:** EventArgs classes follow Web Forms signatures. Pre-operation events (`Sorting`, `SelectedIndexChanging`) support `Cancel` flag. Post-operation events (`Sorted`, `SelectedIndexChanged`, `PagePropertiesChanged`) fire after state updates. HandleCommand routes "sort" and "select" commands. `SortDirection` enum alias needed in test files to avoid `Shouldly.SortDirection` ambiguity.

- **Menu JS interop crash (Bug 1):** `Menu.js` `Sys.WebForms.Menu` constructor crashes when `getElement()` returns null (e.g., headless Chrome timing). Fixed by adding null guard after `getElement()` (early return if element missing) and wrapping entire constructor body in try/catch to prevent unhandled exceptions from killing the Blazor circuit. File: `src/BlazorWebFormsComponents/wwwroot/Menu/Menu.js`.
- **Calendar attribute rendering (Bug 2):** `Calendar.razor` line 64 used raw Razor expression injection to conditionally add `scope="col"` to `<th>` tags. This caused `@(UseAccessibleHeader` to appear literally in server logs due to Razor parsing issues. Fixed by replacing with proper conditional attribute: `scope="@(UseAccessibleHeader ? "col" : null)"` -- Blazor omits the attribute entirely when value is null. File: `src/BlazorWebFormsComponents/Calendar.razor`.
- **Menu auto-ID generation (Bug 3):** Menu JS interop requires a DOM element ID, but when no `ID` parameter is provided, it passes an empty string causing null element lookup. Fixed by adding `OnParametersSet` override in `Menu.razor.cs` that auto-generates `menu_{GetHashCode():x}` when ID is null/empty. File: `src/BlazorWebFormsComponents/Menu.razor.cs`.
- **Shared PagerSettings sub-component:** Created `PagerSettings` class (plain C# POCO, not a Blazor component) with all 12 Web Forms PagerSettings properties (Mode, PageButtonCount, First/Last/Next/PreviousPageText, image URLs, Position, Visible). Created `PagerPosition` enum in `Enums/` (PagerButtons already existed). Created `IPagerSettingsContainer` interface in `Interfaces/`. Created `UiPagerSettings` abstract base component following the `UiTableItemStyle` CascadingParameter pattern but for settings instead of styles. Created 3 concrete sub-component pairs: `GridViewPagerSettings`, `FormViewPagerSettings`, `DetailsViewPagerSettings` — each inherits `UiPagerSettings` and uses `[CascadingParameter(Name = "ParentXxx")]` to set properties on the parent's `PagerSettings` instance. Wired into GridView, FormView, DetailsView: added `IPagerSettingsContainer` to each control's interface list, added `PagerSettings` property + `PagerSettingsContent` RenderFragment parameter, rendered `@PagerSettingsContent` inside existing `<CascadingValue>` block. Key files: `Enums/PagerPosition.cs`, `PagerSettings.cs`, `Interfaces/IPagerSettingsContainer.cs`, `UiPagerSettings.cs`, `GridViewPagerSettings.razor(.cs)`, `FormViewPagerSettings.razor(.cs)`, `DetailsViewPagerSettings.razor(.cs)`.
 Team update (2026-02-24): Substitution/Xml formally deferred  no implementation needed  decided by Beast
 Team update (2026-02-24): M8 scope excludes version bump to 1.0 and release  decided by Jeffrey T. Fritz

 Team update (2026-02-25): Deployment pipeline patterns established  compute Docker version with nbgv before build, gate on secrets, dual NuGet publishing  decided by Forge

### Milestone 9 Migration-Fidelity Fixes (2026-02-25)

- **ToolTip → BaseStyledComponent (WI-01):** Added `[Parameter] public string ToolTip { get; set; }` to `BaseStyledComponent.cs`. Removed duplicate ToolTip declarations from 8 components: Button, Calendar, DataList, FileUpload, HyperLink, Image, ImageButton, ImageMap. All controls inheriting BaseStyledComponent (28+) now get ToolTip automatically. Intentionally preserved ToolTip on sub-component types (ChartSeries, DataPoint, MenuItem, TreeNode) and binding fields (MenuItemBinding.ToolTipField, TreeNodeBinding.ToolTipField) since those are semantically different item-level tooltips.
- **ValidationSummary comma-split bug fix (WI-05):** `AspNetValidationSummary.razor.cs` used `Split(',')[1]` to extract error messages, which truncated messages containing commas. Fixed to use `IndexOf(',')` + `Substring()` to take everything after the first comma. This is a data corruption bug — any validation message with a comma would silently lose content.
- **SkinID type fix (WI-07):** Changed `SkinID` property in `BaseWebFormsComponent.cs` from `bool` to `string`. Web Forms SkinID is the name of a skin to apply (a string), not a boolean flag. The `[Obsolete]` attribute was preserved since theming is not available in Blazor.
- **ToolTip rendering in templates (WI-03):** Audited all `.razor` files inheriting BaseStyledComponent (directly or via DataBoundComponent chain). Added `title="@ToolTip"` to outermost HTML elements on 32 components that were missing it. Components already rendering ToolTip (Button, Calendar, DataList, FileUpload, HyperLink, Image, ImageButton, ImageMap) were left alone. Skipped: ListView and Repeater (no wrapper element), all style sub-components (GridViewRowStyle, CalendarDayStyle, etc.), and GridViewRow/DataGridRow (row sub-components). For multi-layout components (CheckBoxList, RadioButtonList, Panel, CheckBox, RadioButton), added title to every branch's outermost element. For Login controls (Login, ChangePassword, CreateUserWizard), added title to inner `<table>` elements since outer `<EditForm>` is a Blazor component. TextBox uses `CalculatedAttributes` dictionary — added ToolTip there. All 1206 tests pass.

 Team update (2026-02-25): Doc audit found DetailsView/DataGrid features needing implementation verification  decided by Beast
 Team update (2026-02-25): Test audit found 5 missing smoke tests (P0: ListView CrudOperations)  decided by Colossus
 Team update (2026-02-25): M9 plan ratified  12 WIs, migration fidelity  decided by Forge

### Bug Fix: TreeView caret not rotating on expand/collapse (#361)

- **NodeImage fallback logic restructured (TreeNode.razor.cs lines 176–240):** The `NodeImage` property's three fallback paths (for root, parent, and leaf nodes when `ShowLines=false`) did not check `ShowExpandCollapse`. They relied on `ImageSet.Collapse` being non-empty to determine whether to show expand/collapse images vs `Default_NoExpand.gif`. For any ImageSet where `Collapse` returned empty, nodes would always show `Default_NoExpand.gif` regardless of expanded state. Fixed by adding explicit `if (ParentTreeView.ShowExpandCollapse)` checks in the non-ShowLines paths. Extracted `ExpandCollapseImage(bool expanded)` private helper to DRY the ImageSet→filename resolution with guaranteed fallbacks (`Default_Collapse.gif` / `Default_Expand.gif`). When `ShowExpandCollapse=false`, the method now explicitly returns `Default_NoExpand.gif`.
- **Key files:** `src/BlazorWebFormsComponents/TreeNode.razor.cs` (NodeImage property + ExpandCollapseImage helper). Template in `TreeNode.razor` was already correct — it only renders `NodeImage` when `ShowExpandCollapse=true`.
- **Pattern:** TreeView expand/collapse image resolution has three tiers: (1) ShowLines+ShowExpandCollapse → line-style images (Dash/T/L variants), (2) ShowExpandCollapse only → ImageSet images with Default fallback, (3) neither → NoExpand. The `TreeViewImageSet` base class's `Collapse`/`Expand` properties never return empty for built-in sets, but the code must not assume this.

 Team update (2026-02-25): M12 introduces Migration Analysis Tool PoC (`bwfc-migrate` CLI, regex-based ASPX parsing, 3-phase roadmap)  decided by Forge

### Menu Level Styles — StaticMenuStyle, IMenuStyleContainer (#360)

- **StaticMenuStyle sub-component:** Added `StaticMenuStyle` class to `MenuItemStyle.razor.cs` following the same pattern as `DynamicMenuStyle` — inherits `MenuItemStyle`, sets `ParentMenu.StaticMenuStyle = this` in `OnInitialized`. Renders CSS to `ul.level1` in `Menu.razor`.
- **IMenuStyleContainer interface:** Created `Interfaces/IMenuStyleContainer.cs` exposing `DynamicMenuStyle`, `StaticMenuStyle`, `DynamicMenuItemStyle`, `StaticMenuItemStyle` as `MenuItemStyle` properties. Menu implements `IMenuStyleContainer` via explicit interface implementation since the concrete Menu properties use derived types (`DynamicMenuStyle`, `StaticMenuStyle`, etc.).
- **RenderFragment parameters:** Added `DynamicMenuStyleContent`, `StaticMenuStyleContent`, `DynamicMenuItemStyleContent`, `StaticMenuItemStyleContent` RenderFragment parameters to Menu. Rendered inside `<CascadingValue Name="ParentMenu">` block before `@ChildContent`. Added `IsFixed="true"` to the CascadingValue.
- **CSS rendering:** `StaticMenuStyle` CSS applied to `#{ID} ul.level1` in Menu.razor's `<style>` block, analogous to how `DynamicMenuStyle` applies to `ul.dynamic`.
- **Key pattern:** Menu styles use `MenuItemStyle` (inherits `ComponentBase, IStyle`) NOT `UiTableItemStyle`. This is intentional — Menu styles produce CSS text via `ToStyle()` for inline `<style>` blocks, whereas GridView/Calendar styles use `TableItemStyle` objects for HTML attribute-level styling. When mixing named RenderFragment params with bare child content in tests, wrap bare content in `<ChildContent>` tags.
- **Files created:** `ListViewSortEventArgs.cs`, `ListViewPagePropertiesChangingEventArgs.cs`, `ListViewSelectEventArgs.cs`, test files: `SortingEvents.razor`, `SelectionEvents.razor`, `PagingEvents.razor`, `LayoutCreatedEvent.razor`.
- **All 1229 tests pass** including 12 new ListView event tests (27 total ListView tests).

### ThemeConfiguration Core Types (#364)

- **Namespace:** `BlazorWebFormsComponents.Theming` — clean separation from existing component code per team decision.
- **ControlSkin:** Mirrors `BaseStyledComponent` parameters (BackColor, ForeColor, BorderColor, BorderStyle, BorderWidth, CssClass, Height, Width, Font, ToolTip). Uses nullable types (`BorderStyle?`, `Unit?`, null reference types) so "not set" is distinguishable from "set to default" — essential for StyleSheetTheme semantics where component explicit values override theme defaults.
- **ThemeConfiguration:** Dictionary-of-dictionaries keyed by control type name (case-insensitive) → SkinID (empty string = default skin). `GetSkin` returns null for missing entries (no throw) per Jeff's decision; callers log warnings. `AddSkin` throws on null/empty controlTypeName for safety.
- **ThemeProvider.razor:** Minimal `CascadingValue<ThemeConfiguration>` wrapper. Does not inherit `BaseWebFormsComponent` — it's infrastructure, not a Web Forms control emulation.
- **Key files:** `src/BlazorWebFormsComponents/Theming/ControlSkin.cs`, `ThemeConfiguration.cs`, `ThemeProvider.razor`.

### SkinID + EnableTheming Activation (#365)

- **Removed `[Obsolete]`** from `EnableTheming` and `SkinID` properties on `BaseWebFormsComponent`. These were previously marked obsolete with "Theming is not available in Blazor" messages.
- **Set defaults:** `EnableTheming = true` (StyleSheetTheme semantics — theme sets defaults, explicit values override), `SkinID = ""` (empty string means "use default skin").
- **Updated XML doc comments** to describe the properties' purpose instead of the old 🚨 warning messages.
- **No other files reference these properties** — only `BaseWebFormsComponent.cs` needed changes.
- **All 1246 tests pass**, 0 regressions. Build succeeds with 0 errors.
- These properties are now ready for #366 (base class integration with the theming system being built in #364).

### Theme Base Class Integration (#366)

- **CascadingParameter:** Added `[CascadingParameter] public ThemeConfiguration Theme` to `BaseStyledComponent`. All styled components automatically receive the theme when wrapped in `<ThemeProvider>`.
- **OnParametersSet override:** Early-returns when `EnableTheming` is false or `Theme` is null (protects all existing tests). Calls `Theme.GetSkin(GetType().Name, SkinID)` and applies via `ApplySkin`.
- **StyleSheetTheme semantics:** `ApplySkin` only overwrites a property when the component value is still at its default and the skin provides a non-default value. Explicit component parameters always win.
- **Font handling:** Checks individual FontInfo properties (Name, Size, Bold, Italic, Underline) since Font is always initialized to `new FontInfo()`. Uses `FontUnit.Empty` comparison for Size.
- **Missing named skins:** If `SkinID` is set but no matching skin exists, silently continues (returns null from `GetSkin`). Logging deferred to M11 per project convention.
- **Only file modified:** `src/BlazorWebFormsComponents/BaseStyledComponent.cs`.
- **All 1246 tests pass**, 0 regressions. Build succeeds with 0 errors.

 Team update (2026-02-25): All new work MUST use feature branches pushed to origin with PR to upstream/dev. Never commit directly to dev.  decided by Jeffrey T. Fritz


 Team update (2026-02-25): Theme core types (#364) use nullable properties for StyleSheetTheme semantics, case-insensitive keys, empty-string default skin key. ThemeProvider is infrastructure, not a WebForms control. GetSkin returns null for missing entries.  decided by Cyclops


 Team update (2026-02-25): SkinID defaults to empty string, EnableTheming defaults to true. [Obsolete] removed  these are now functional [Parameter] properties.  decided by Cyclops


 Team update (2026-02-25): ThemeConfiguration CascadingParameter wired into BaseStyledComponent (not BaseWebFormsComponent). ApplySkin runs in OnParametersSet with StyleSheetTheme semantics. Font properties checked individually.  decided by Cyclops


 Team update (2026-02-25): ThemesAndSkins.md documentation updated to match PoC implementation  class names, API, roadmap status, PoC decisions table added  decided by Beast

 Team update (2026-02-25): Calendar selection behavior review found 7 issues (1 P0: external SelectedDate sync, 4 P1: SelectWeekText default, SelectedDates sorting/mutability, style layering, 2 P2: test gaps, allocation)  decided by Forge


 Team update (2026-02-25): HTML audit strategy approved  decided by Forge

 Team update (2026-02-25): HTML audit milestones M11-M13 defined, existing M12M14, Skins/ThemesM15+  decided by Forge per Jeff's directive

### P1 Bug Fixes — Button and BulletedList

- **Button `<input>` rendering:** Button.razor already rendered `<input type="submit" value="@Text" />` (fixed in prior commit `a5011ff`). `UseSubmitBehavior` (default true) controls `type="submit"` vs `type="button"`. `CausesValidation` no longer affects HTML type attribute — only validation behavior.
- **BulletedList `<span>` removal:** Removed `<span>` wrapping in all three `DisplayMode` branches (Text, HyperLink disabled fallback, LinkButton disabled fallback). Items now render as `<li>Item</li>` for text and `<li><a>Item</a></li>` for links, matching WebForms output.
- **BulletedList `<ol>` for ordered styles:** Already correct — `IsOrderedList` property correctly returns `true` for Numbered, LowerAlpha, UpperAlpha, LowerRoman, UpperRoman. `ListStyleType` correctly maps to CSS values.
- **Test updates:** Changed 27 test files — all `Find("button")` / `FindAll("button")` selectors updated to `Find("input")` or `Find("input[type=submit]")` for Button component, and `li span` assertions changed to `li` text content for BulletedList.
- **All 1253 tests pass**, 0 regressions.

### Calendar Structural Bug Fixes (HTML Audit)

- **`<tbody>` wrapper:** Added `<tbody>` around all `<tr>` elements inside the calendar `<table>`, matching WebForms structural output.
- **`width:14%` on day cells:** `GetDayCellStyle()` now prepends `width:14%;` to all day cell styles, ensuring equal-width day columns.
- **Day `title` attributes:** Added `GetDayTitle(DateTime)` method returning "MonthName Day" format (e.g., "January 25"). Applied as `title` on day `<a>` links for accessibility.
- **Full `abbr` day names:** Added `GetFullDayName(DayOfWeek)` method that always returns the full day name (e.g., "Sunday") for the `abbr` attribute on `<th>` headers, regardless of `DayNameFormat`. Display text still uses the configured format.
- **Day header `align="center"`:** Added `align="center"` to `<th>` day header elements matching WebForms output.
- **Root table default styles:** `GetTableStyle()` now always includes `border-width:1px;border-style:solid;border-collapse:collapse;` as default styles, matching WebForms default rendering.
- **Navigation sub-table:** Restructured the title/navigation row from flat `<td>` cells to a `<td colspan="7">` containing a nested `<table>` with prev/title/next cells (`width:15%/70%/15%`), matching WebForms sub-table pattern. Added `title="Go to the previous month"` and `title="Go to the next month"` on nav links.
- **Files modified:** `Calendar.razor`, `Calendar.razor.cs`.
- **All 1253 tests pass**, 0 regressions.

 Team update (2026-02-26): Login+Identity strategy: handler delegates in core, separate Identity NuGet package, redirect-based cookie flows  decided by Forge

 Team update (2026-02-26): Data control divergence: 3 P1 bugs in DataList/GridView need fixes before M13 completion  decided by Forge

 Team update (2026-02-26): Post-fix capture: sample data alignment is P0, structural bugs are P3  decided by Rogue

 Team update (2026-02-26): WebFormsPage unified wrapper  inherits NamingContainer, adds Theme cascading, replaces separate wrappers  decided by Jeffrey T. Fritz, Forge
 Team update (2026-02-26): SharedSampleObjects is the single source for sample data parity between Blazor and WebForms  decided by Jeffrey T. Fritz
 Team update (2026-02-26): Login+Identity controls deferred to future milestone  do not schedule implementation  decided by Jeffrey T. Fritz

### M15 HTML Fidelity Bug Fixes (#380, #379, #378)

- **BulletedList `<ol>` rendering (#380):** Removed HTML `type` attribute from `<ol>` element — WebForms uses CSS `list-style-type` only, not the HTML `type` attribute. Made `start` attribute conditional (only renders when `FirstBulletNumber != 1`). The `IsOrderedList` property and `ListStyleType` CSS mappings were already correct. Replaced `OrderedListType` property with `GetStartAttribute()` helper returning `int?` (null suppresses the attribute in Blazor).
- **LinkButton `class` pass-through (#379):** Updated `GetCssClassOrNull()` in LinkButton.razor to add `aspNetDisabled` class when `Enabled=false`, matching the Button component's `CalculatedCssClass` pattern. Base CssClass pass-through was already working via the existing `GetCssClassOrNull()` method. Added 5 tests covering CssClass rendering, disabled state, and PostBackUrl+CssClass combo.
- **Image `longdesc` conditional (#378):** The `GetLongDesc()` method in Image.razor already correctly returned null when `DescriptionUrl` was empty/unset, suppressing the `longdesc` attribute. Added 3 explicit tests verifying conditional rendering behavior. `DescriptionUrl` defaults to `string.Empty` in Image.razor.cs.
- **Patterns followed:** Checked WebForms audit HTML to match exact attribute output. Followed Button's `CalculatedCssClass` pattern for disabled-state class handling. Used Blazor's null-attribute-suppression for conditional rendering (return null → attribute not rendered). All existing tests pass; added 10 new tests total.
- **All 1277 tests pass**, 0 regressions.

### M15 HTML Fidelity Closure Fixes (#383, #382, GridView default)

- **FileUpload ID rendering (#383):** Verified that FileUpload renders the developer-set ID exactly (e.g., `id="myUpload"`) with no GUID suffix. In .NET 10, `InputFile` does not generate its own internal id — the `id` from `GetInputAttributes()` via `ClientID` is rendered as-is. When no ID is set, no `id` attribute is rendered. Added 2 regression tests in `FileUpload/IdRendering.razor`.
- **CheckBox span wrapper removal (#382):** Verified that CheckBox renders `<input>` and `<label>` directly without a `<span>` wrapper (fixed in prior commit `3d84b4a`). CheckBoxList renders its own inline `<input>`/`<label>` pairs and does not use the CheckBox component, so no span dependency exists. Added 2 regression tests in `CheckBox/NoSpanWrapper.razor`.
- **GridView UseAccessibleHeader default:** Changed `UseAccessibleHeader` default from `false` to `true` in `GridView.razor.cs`, matching WebForms behavior. WebForms defaults this to true, rendering `<th scope="col">` for header cells. The existing `UseAccessibleHeader_RendersThWithScope` test explicitly passed `UseAccessibleHeader="true"` and didn't catch the wrong default. Added 2 tests in `GridView/AccessibleHeaderDefault.razor` verifying default behavior and explicit false.
- **Key learning:** When verifying WebForms fidelity, always test the default parameter values — passing `true` explicitly masks wrong defaults. CheckBoxList uses its own inline rendering, not the CheckBox component — changes to CheckBox don't affect CheckBoxList.
- **All 1283 tests pass**, 0 regressions.

### LoginView Wrapper Element for BaseStyledComponent (#351, #352, #354)

- **Panel BackImageUrl (#351):** Already implemented — `BackImageUrl` parameter and `background-image:url(...)` rendering in `BuildStyle()` were present in `Panel.razor.cs`.
- **PasswordRecovery base class (#354):** Already migrated — inherits `BaseStyledComponent`, outer `<table>` elements already render `class="@CssClass" style="border-collapse:collapse;@Style" title="@ToolTip"`.
- **LoginView wrapper div (#352):** Added `<div class="@CssClass" style="@Style" title="@ToolTip">` wrapper around LoginView's content in `LoginView.razor`. The component already inherited `BaseStyledComponent` but had no outer HTML element rendering the style properties. Updated 8 LoginView test files to use `cut.Find("div").TextContent.Trim()` instead of `cut.Markup.Trim()` since the new wrapper `<div>` is now part of the rendered output.
- **Key files changed:** `LoginControls/LoginView.razor`, 8 test files in `LoginControls/LoginView/`.
- **Pattern:** For template-switching controls without a table layout (like LoginView), use a `<div>` wrapper with `class="@CssClass" style="@Style" title="@ToolTip"` — unlike Login/ChangePassword/CreateUserWizard which use `<table>` wrappers.
- **All 1283 tests pass**, 0 regressions.
