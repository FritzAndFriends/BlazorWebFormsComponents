# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!--  Summarized 2026-02-27 by Scribe  covers M1M16 -->

### Core Context (2026-02-10 through 2026-02-27)

**M1M3 components:** Calendar (enum fix, async events), ImageMap (BaseStyledComponent, Guid IDs), FileUpload (InputFile integration, path sanitization), PasswordRecovery (3-step wizard), DetailsView (DataBoundComponent<T>, auto-field reflection, 10 events), Chart (BaseStyledComponent, CascadingValue "ParentChart", JS interop via ChartJsInterop, ChartConfigBuilder pure static).

**M6 base class fixes:** DataBoundComponent chain  BaseStyledComponent (14 data controls). BaseListControl<TItem> for 5 list controls (DataTextFormatString, AppendDataBoundItems). CausesValidation on CheckBox/RadioButton/TextBox. Label AssociatedControlID switches spanlabel. Login/ChangePassword/CreateUserWizard  BaseStyledComponent. Validator ControlToValidate dual-path: ForwardRef + string ID via reflection.

**M6 Menu overhaul:**  BaseStyledComponent. Selection tracking (SelectedItem/SelectedValue, MenuItemClick, MenuItemDataBound). MenuEventArgs, MaximumDynamicDisplayLevels, Orientation enum + CSS horizontal class. MenuLevelStyle lists. StaticMenuStyle sub-component + IMenuStyleContainer interface. RenderFragment parameters for all menu styles. RenderingMode=Table added (M14) with inline Razor for AngleSharp compatibility.

**M7 style sub-components:** GridView (8), DetailsView (10), FormView (7), DataGrid (7)  all CascadingParameter + UiTableItemStyle. Style priority: Edit > Selected > Alternating > Row. TreeView: TreeNodeStyle + 6 sub-components, selection, ExpandAll/CollapseAll, FindNode, ExpandDepth, NodeIndent. GridView: selection, 10 display props. FormView/DetailsView events + PagerTemplate + Caption. DataGrid paging/sorting. ListView 10 CRUD events + EditItemTemplate/InsertItemTemplate. Panel BackImageUrl. Login Orientation + TextLayout. Shared PagerSettings (12 props, IPagerSettingsContainer) for GridView/FormView/DetailsView.

**M8 bug fixes:** Menu JS null guard + Calendar conditional scope + Menu auto-ID (`menu_{GetHashCode():x}`).

**M9 migration-fidelity:** ToolTip  BaseStyledComponent (removed from 8, added title="@ToolTip" to 32 components). ValidationSummary comma-split fix (IndexOf + Substring). SkinID boolstring. TreeView NodeImage fallback restructured (ShowExpandCollapse + ExpandCollapseImage helper).

**M10 Theming:** ControlSkin (nullable props, StyleSheetTheme semantics). ThemeConfiguration (case-insensitive keys, empty-string default skin, GetSkin returns null). ThemeProvider as CascadingValue wrapper. SkinID="" default, EnableTheming=true, [Obsolete] removed. CascadingParameter in BaseStyledComponent, ApplySkin in OnParametersSet. LoginView/PasswordRecovery  BaseStyledComponent.

**M15 HTML fidelity fixes:** Button `<input>` rendering. BulletedList `<span>` removal + `<ol>` CSS-only (no HTML type attr, GetStartAttribute returns int?). LinkButton class + aspNetDisabled. Image longdesc conditional. Calendar structural (tbody, width:14%, day titles, abbr headers, align center, border-collapse, navigation sub-table). FileUpload clean ID. CheckBox span verified. GridView UseAccessibleHeader default falsetrue. 27 test files updated for Button `<input>`. 10 new tests. All 1283 pass.

**M16:** LoginView wrapper `<div>` for styles (#352). ClientIDMode enum (Inherit/AutoID/Static/Predictable) on BaseWebFormsComponent. ComponentIdGenerator refactored: GetEffectiveClientIDMode(), BuildAutoID(), BuildPredictableID(). UseCtl00Prefix only in AutoID mode. NamingContainer auto-sets AutoID when UseCtl00Prefix=true.

**Key patterns:** Orientation enum collides with parameter name  use `Enums.Orientation.Vertical`. `_ = callback.InvokeAsync()` for render-time events. `Path.GetFileName()` for file save security. CI secret-gating: env var indirection. Null-returning helpers for conditional HTML attributes. aspNetDisabled class for disabled controls. Always test default parameter values explicitly.

 Team update (2026-02-27): Branching workflow directive  feature PRs from personal fork to upstream dev, only devmain on upstream  decided by Jeffrey T. Fritz

 Team update (2026-02-27): Issues must be closed via PR references using 'Closes #N' syntax, no manual closures  decided by Jeffrey T. Fritz


 Team update (2026-02-27): AJAX Controls nav category created; migration stub doc pattern for no-op components; Substitution moved from deferred to implemented; UpdateProgress uses explicit state pattern  decided by Beast


 Team update (2026-02-27): M17 sample pages created for Timer, UpdatePanel, UpdateProgress, ScriptManager, Substitution. Default.razor filenames. ComponentCatalog already populated  decided by Jubilee

 Team update (2026-02-27): Forge approved M17 with 4 non-blocking follow-ups (ScriptManager defaults, UpdateProgress CSS/style gaps)  decided by Forge


 Team update (2026-02-27): Timer duplicate [Parameter] bug fixed; 47 M17 tests established with C# API pattern for Timer  decided by Rogue

### M17 Control Audit Fixes (2026-02-27)

- **Fix 1:** ScriptManager `EnablePartialRendering` default changed from `false` to `true` to match Web Forms default.
- **Fix 2:** ScriptManager now has `Scripts` collection (`List<ScriptReference>`) matching ScriptManagerProxy pattern. Added `using System.Collections.Generic;`.
- **Fix 3:** UpdateProgress `<div>` elements now render `class="@CssClass"` conditionally (only when CssClass is non-empty) to match Web Forms styled output.
- **Fix 4:** UpdateProgress non-dynamic mode div style changed from `visibility:hidden;` to `display:block;visibility:hidden;` matching Web Forms explicit rendering.
- **Fix 5:** ScriptReference gained `ScriptMode` (default Auto), `NotifyScriptLoaded` (default true), and `ResourceUICultures` properties from the Web Forms original. Added `using BlazorWebFormsComponents.Enums;`.
- **Lesson:** Always verify default values against the Web Forms originals — bool properties default to `false` in C# but Web Forms often defaults them to `true`.


 Team update (2026-02-27): No-op stub property coverage intentionally limited (41-50% acceptable)  deep AJAX infrastructure properties omitted  decided by Forge

 Team update (2026-02-27): UpdatePanel Triggers collection deliberately omitted  Blazor rendering model makes it unnecessary  decided by Forge

### M18 Bug Fix Verification (2026-02-27)

- **Bug #380 (BulletedList `<ol>` rendering):** Verified already correct from M15. `IsOrderedList` property correctly maps Numbered/LowerAlpha/UpperAlpha/LowerRoman/UpperRoman → `true`. Razor template branches `<ol>` vs `<ul>` based on `IsOrderedList`. `ListStyleType` correctly maps all 8 styles (disc, circle, square, decimal, lower-alpha, upper-alpha, lower-roman, upper-roman). All 13 BulletStyle tests pass.
- **Bug #382 (CheckBox span wrapper):** Verified already correct from M15. CheckBox.razor renders bare `<input>` + `<label>` with NO wrapping `<span>` for both TextAlign=Left and TextAlign=Right. Tests for both alignments pass (8 total).
- **Bug #383 (FileUpload GUID attribute leak):** Investigated. The only "stray" attribute is `blazor:elementReference=""` from Blazor's `InputFile` component internal `@ref` directive. Root cause: `InputFile` needs `@ref` for JS interop to handle file change events; this adds `blazor:elementReference` to rendered markup. In production, Blazor's JS runtime processes and removes it from the DOM. Cannot be removed without replacing `InputFile` entirely (which would break `IBrowserFile` file data access). Tests confirm no GUID leaks into `id` or other user-facing attributes; `blazor:elementReference` is accepted as a known framework artifact.
- **Lesson:** The M15 HTML fidelity fixes were thorough — all three "bugs" turned out to be already resolved in code. The `blazor:elementReference` on FileUpload is inherent to using Blazor's `InputFile` component and is not a bug in our code.

### M18 Wave 2 — Deterministic IDs & Menu Font Styles (2026-02-27)

- **Issue #386 (Stable deterministic IDs):** CheckBox and RadioButtonList already used `ClientID` (from `ComponentIdGenerator`) when developer provides `ID`, falling back to GUID when not. The only fix needed was adding `id="@_inputId"` to CheckBox's bare (no-text) `<input>` element (line 20) which was missing the `id` attribute. RadioButtonList was already correct: `{ClientID}_0`, `{ClientID}_1` pattern for item IDs, and `ClientID` as the `name` attribute for mutual exclusion. Tests in `CheckBox/IDRendering.razor` and `RadioButtonList/StableIds.razor` already cover all scenarios.
- **Issue #360 (Menu level styles):** The four style sub-components (`DynamicMenuStyle`, `StaticMenuStyle`, `DynamicMenuItemStyle`, `StaticMenuItemStyle`) were already implemented in `MenuItemStyle.razor.cs` as C# classes inheriting from `MenuItemStyle`. The `IMenuStyleContainer` interface was already wired. The actual fix was that `MenuItemStyle.SetPropertiesFromUnknownAttributes()` did not process `Font-` prefixed attributes (e.g. `Font-Bold`, `Font-Italic`). Added `this.SetFontsFromAttributes(OtherAttributes)` call in `OnInitialized()` after `SetPropertiesFromUnknownAttributes()` to use the existing `HasStyleExtensions.SetFontsFromAttributes` method. This fixed the failing `Menu_StaticMenuItemStyle_FontBold_RendersFontWeight` test.
- **Lesson:** When style sub-components use `CaptureUnmatchedValues`, font properties need explicit handling via `SetFontsFromAttributes()` because `Font-Bold` doesn't map to a simple property — it maps to `Font.Bold` on the `FontInfo` sub-object.
