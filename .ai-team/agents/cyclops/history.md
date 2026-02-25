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

**Key patterns:** `_ = callback.InvokeAsync()` for render-time events. `Path.GetFileName()` for file save security. Orientation enum collides with parameter name in Razor — use `Enums.Orientation.Vertical`. CI secret-gating: use env var indirection, not `secrets.*` in step-level `if:`.

📌 Team update (2026-02-25): M12 introduces Migration Analysis Tool PoC — decided by Forge

### ListView CRUD Events Completion (#356)

- **Sorting/Sorted events:** Added `ListViewSortEventArgs` (SortExpression, SortDirection, Cancel) and `Sorting`/`Sorted` EventCallback parameters. Sort command routed through `HandleCommand("Sort", expression, index)`. Toggles direction when sorting same expression (matches GridView pattern). `SortExpression` and `SortDirection` properties added to ListView.
- **SelectedIndexChanging/SelectedIndexChanged events:** Added `ListViewSelectEventArgs` (NewSelectedIndex, Cancel) and `SelectedIndexChanging`/`SelectedIndexChanged` EventCallback parameters. Select command routed through `HandleCommand("Select", null, index)`. Follows GridView `SelectRow` pattern with cancellation support.
- **PagePropertiesChanging/PagePropertiesChanged events:** Added `ListViewPagePropertiesChangingEventArgs` (StartRowIndex, MaximumRows) and `PagePropertiesChanging`/`PagePropertiesChanged` EventCallback parameters. Exposed via `SetPageProperties(startRowIndex, maximumRows)` public method. Added `StartRowIndex` and `MaximumRows` properties.
- **LayoutCreated event:** Converted from `EventHandler OnLayoutCreated` to `EventCallback<EventArgs> OnLayoutCreated`. Wired invocation via `RaiseLayoutCreated()` internal method called in ListView.razor after LayoutTemplate is resolved.
- **Key patterns followed:** EventArgs classes follow Web Forms signatures. Pre-operation events (`Sorting`, `SelectedIndexChanging`) support `Cancel` flag. Post-operation events (`Sorted`, `SelectedIndexChanged`, `PagePropertiesChanged`) fire after state updates. HandleCommand routes "sort" and "select" commands. `SortDirection` enum alias needed in test files to avoid `Shouldly.SortDirection` ambiguity.

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
