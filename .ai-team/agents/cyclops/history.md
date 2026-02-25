# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
<!-- ⚠ Summarized 2026-02-23 by Scribe — original entries covered 2026-02-10 through 2026-02-12 -->

### Summary: Milestones 1–3 Implementation (2026-02-10 through 2026-02-12)

Built Calendar (enum fix, async events), ImageMap (BaseStyledComponent, Guid IDs, Enabled propagation), FileUpload (InputFile integration, path sanitization), PasswordRecovery (3-step wizard, per-step EditForm, SubmitButtonStyle→LoginButtonStyle cascading), DetailsView (DataBoundComponent<T>, auto-field reflection, mode switching, 10 events, paging). Image and Label upgraded to BaseStyledComponent (WI-15/WI-17).

**Key patterns:** Enum files in `Enums/` with explicit int values. Instance-based Guid IDs (not static). `_ = callback.InvokeAsync()` for render-time events. `Path.GetFileName()` for file save security. Login controls inherit BaseWebFormsComponent with CascadingParameter styles.

### Summary: Milestone 4 Chart Component (2026-02-12)

Chart uses BaseStyledComponent, CascadingValue `"ParentChart"` for child registration. JS interop via separate `ChartJsInterop` (not shared service). `ChartConfigBuilder` is pure static class for testability. ChartWidth/ChartHeight as strings (avoid base Width/Height conflict). SeriesChartType.Point → Chart.js "scatter". 8 Phase 1 types; unsupported throw NotSupportedException. ChartSeries data binding via reflection on Items/XValueMember/YValueMembers.

### Summary: Feature Audit — Editor Controls A–I (2026-02-23)

Audited 13 controls. Found: AccessKey/ToolTip missing from base class (universal gap), Image needs BaseStyledComponent, HyperLink.NavigateUrl naming mismatch, list controls missing DataTextFormatString/AppendDataBoundItems/CausesValidation/ValidationGroup, Calendar styles use CSS strings instead of TableItemStyle objects.

📌 Team update (2026-02-12): DetailsView auto-generated fields must render <input type="text"> in Edit/Insert mode — decided by Cyclops

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

### Milestone 8 Release-Readiness Bug Fixes (2026-02-24)

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



📌 Team update (2026-02-25): CI secret-gating pattern corrected — secrets.* cannot be used in step-level if: conditions. Use env var indirection: declare secret in env:, check env.VAR_NAME in if:. Applied to nuget.yml and deploy-server-side.yml (PR #372). — decided by Forge

