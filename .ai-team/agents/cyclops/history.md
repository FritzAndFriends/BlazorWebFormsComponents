# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
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
