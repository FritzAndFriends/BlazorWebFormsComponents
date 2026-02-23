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

- **DataBoundComponent style inheritance (WI-07):** Changed `BaseDataBoundComponent` to inherit `BaseStyledComponent` instead of `BaseWebFormsComponent`. This gives ALL data controls (GridView, DetailsView, FormView, ListView, DataGrid, DataList, Repeater, TreeView, AdRotator, BulletedList, CheckBoxList, DropDownList, ListBox, RadioButtonList) the full IStyle property set (BackColor, CssClass, ForeColor, Font, etc.) from the base class. Removed duplicate IStyle implementations and CssClass properties from: GridView, DetailsView, DataGrid, DataList, TreeView, AdRotator, BulletedList, CheckBoxList, DropDownList, ListBox, RadioButtonList. DataList kept its `new string Style` parameter (user-supplied CSS) but removed its IStyle declaration and 9 duplicate style properties. ListView kept its obsolete `new string Style` parameter. FormView and Repeater needed no changes.
- **CausesValidation pattern for non-button controls (WI-49):** CheckBox, RadioButton, and TextBox now have `CausesValidation` (bool, default true), `ValidationGroup` (string), and a `[CascadingParameter(Name = "ValidationGroupCoordinator")] ValidationGroupCoordinator Coordinator` — identical to the pattern in `ButtonBaseComponent`. Validation is triggered in the existing `HandleChange` method for CheckBox/RadioButton. TextBox has the parameters but no handler wiring because the component has no `@onchange` binding in its template.
- **BaseListControl<TItem> base class (WI-47/48):** Created `DataBinding/BaseListControl.cs` inheriting `DataBoundComponent<TItem>`. Consolidates `StaticItems`, `DataTextField`, `DataValueField`, `GetItems()`, and `GetPropertyValue()` from all 5 list controls (BulletedList, CheckBoxList, DropDownList, ListBox, RadioButtonList). All 5 now inherit `BaseListControl<TItem>`. This mirrors Web Forms `ListControl` as the shared base.
- **DataTextFormatString (WI-47):** `[Parameter] public string DataTextFormatString` on `BaseListControl<TItem>`. Applied via `string.Format(DataTextFormatString, text)` at render time in `GetItems()`, not at bind time. Affects both static and data-bound items. Static items get a cloned `ListItem` to avoid mutating the source collection.
- **AppendDataBoundItems (WI-48):** `[Parameter] public bool AppendDataBoundItems` on `BaseListControl<TItem>`, default `false`. When `false` and `Items != null`, static items are skipped in `GetItems()`. When `true`, static items always render before data-bound items. Matches Web Forms semantics where `DataBind()` clears `Items` by default.
- **List control inheritance chain:** `BaseListControl<TItem>` → `DataBoundComponent<TItem>` → `BaseDataBoundComponent` → `BaseStyledComponent` → `BaseWebFormsComponent`. All list controls get full style property set via this chain.
- **Orientation enum and Menu orientation (WI-50):** Created `Enums/Orientation.cs` (Horizontal=0, Vertical=1) using file-scoped namespace. Menu.razor.cs gets `[Parameter] public Orientation Orientation { get; set; } = Orientation.Vertical;`. Horizontal layout achieved via CSS class `horizontal` on the top-level `<ul>`, with `display: inline-block` on direct `<li>` children. JS interop orientation string made dynamic from enum value.
- **Label AssociatedControlID (WI-51):** When `AssociatedControlID` is set (non-null/non-empty), Label renders `<label for="{AssociatedControlID}">` instead of `<span>`. All existing attributes (id, class, style, accesskey) apply to whichever element renders. This matches Web Forms behavior where `Label.AssociatedControlID` switches the output element. Important for accessibility — `<label for>` enables screen readers.
- **Login controls upgraded to BaseStyledComponent (WI-52):** Login, ChangePassword, and CreateUserWizard now inherit `BaseStyledComponent` instead of `BaseWebFormsComponent`. This adds BackColor, BorderColor, BorderStyle, BorderWidth, CssClass, Font, ForeColor, Height, Width to the outer `<table>` element. No conflicts with CascadingParameter sub-styles (TitleTextStyle, LabelStyle, etc.) because outer styles are `[Parameter]` and sub-styles are `[CascadingParameter]` — completely independent mechanisms. `SetFontsFromAttributes` called in `HandleUnknownAttributes` for Font-* attribute support. The `GetCssClassOrNull()` helper returns null when empty so the `class` attribute is omitted from HTML when no CssClass is set.

 Team update (2026-02-23): Menu Orientation Orientation parameter collides with enum type name in Razor  samples must use local variable with fully-qualified type  decided by Jubilee
 Team update (2026-02-23): P2 test observation  Login/ChangePassword/CreateUserWizard already inherit BaseStyledComponent, so WI-52 may have been a no-op or template-only change  decided by Rogue
 Team update (2026-02-23): Milestone 6 Work Plan ratified  54 WIs across P0/P1/P2 tiers targeting ~345 feature gaps  decided by Forge
 Team update (2026-02-23): UI overhaul requested  ComponentCatalog (UI-2) and search (UI-8) assigned to Cyclops  decided by Jeffrey T. Fritz
