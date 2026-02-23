# Audit Summary — BlazorWebFormsComponents

**Generated:** 2026-02-12
**Auditor:** Forge (Lead / Web Forms Reviewer)
**Scope:** All 53 Web Forms controls audited against .NET Framework 4.8 API surface
**Source:** Individual audit files in `planning-docs/{Control}.md`

---

## 1. Executive Summary

| Metric | Properties | Events | **Total** |
|--------|-----------|--------|-----------|
| ✅ **Matching** | 931 | 341 | **1,272** |
| ⚠️ **Needs Work** | 50 | 1 | **51** |
| 🔴 **Missing** | 505 | 92 | **597** |
| ➖ **N/A (server-only)** | — | — | **264** |
| **Total Applicable** | **1,486** | **434** | **1,920** |

### Overall Health: **66.3%**

Of 1,920 applicable features (properties + events), 1,272 fully match Web Forms behavior. An additional 51 exist but need API corrections. 597 features are missing entirely.

**6 controls are 100% feature-complete.** 3 controls are not started (Substitution, Xml, PasswordRecovery on current branch). GridView is the weakest implemented control at 20.7% coverage.

---

## 2. Critical Findings

### 🚨 UNMERGED COMPONENTS — DetailsView & PasswordRecovery

**Branch `sprint3/detailsview-passwordrecovery` was NEVER merged to `dev`.**

- `status.md` incorrectly lists both as ✅ Complete
- **DetailsView** exists on the branch with 27 matching properties, 16 matching events, and strong CRUD support — but is inaccessible from the current working branch
- **PasswordRecovery** exists on the branch (29 bUnit tests referenced in history) but the component files are NOT found on the current branch — the audit shows **0 matching features** from the current branch perspective
- This means the actual shipped component count is **48/53 (91%)**, not 50/53 (94%)

**Action Required:** Merge `sprint3/detailsview-passwordrecovery` into `dev` immediately.

### 🔴 Base Class Inheritance Gap — DataBoundComponent\<T\>

`DataBoundComponent<T>` inherits `BaseWebFormsComponent`, **not** `BaseStyledComponent`. This means every data control that inherits `DataBoundComponent<T>` is missing all WebControl style properties:

- **Affected controls (7):** DataGrid, DetailsView, FormView, GridView, ListView, Chart*, Menu*
- **Missing properties per control:** BackColor, BorderColor, BorderStyle, BorderWidth, CssClass, Font, ForeColor, Height, Width, Style (~10 properties each)
- **Exception:** DataList implements `IStyle` directly — it has all style properties despite inheriting `DataBoundComponent<T>`

This single inheritance fix would close **~70 missing property gaps** across 7 controls.

*Chart inherits BaseStyledComponent but not DataBoundComponent. Menu inherits BaseWebFormsComponent directly.

### 🔴 AccessKey Universally Missing from BaseStyledComponent

`AccessKey` is marked 🔴 Missing on **every control that inherits BaseStyledComponent** (and BaseWebFormsComponent). Web Forms `WebControl.AccessKey` maps to the HTML `accesskey` attribute. Adding this to `BaseWebFormsComponent` would close **~40 gaps** at once.

### 🔴 ToolTip Missing from BaseStyledComponent

`ToolTip` (renders as HTML `title` attribute) is missing from the base class. A handful of controls implement it directly (Button, Calendar, DataList, FileUpload, HyperLink, Image, ImageButton, ImageMap), but **~35 controls** are missing it. Adding ToolTip to `BaseWebFormsComponent` would be a sweeping fix.

### 🔴 GridView Is the Weakest Data Control

GridView has only **20.7% feature coverage** — the lowest of any implemented control:
- No paging (AllowPaging, PageSize, PageIndex, PagerSettings, PagerStyle)
- No sorting (AllowSorting, SortDirection, SortExpression)
- No inline editing (EditIndex, EditRowStyle, RowEditing/RowUpdating/RowDeleting events)
- No selection (SelectedIndex, SelectedRow, SelectedRowStyle)
- No style properties (no BaseStyledComponent inheritance)
- Only 1 event implemented (RowCommand) out of 15 Web Forms events

GridView is the most commonly used data control in Web Forms applications. This is the single biggest migration blocker.

### 🔴 Validator Display Property Missing

All 6 validator controls are missing the `Display` property (`ValidatorDisplay` enum: None, Static, Dynamic). This controls whether the validator reserves space in the layout (Static), collapses when valid (Dynamic), or is invisible (None — used with ValidationSummary). Without this, migrated pages will have layout differences.

Also missing across all validators: `SetFocusOnError` (focus management on validation failure).

### 🔴 Login Controls Missing WebControl Style Properties

`ChangePassword`, `CreateUserWizard`, `Login`, and `PasswordRecovery` all inherit `BaseWebFormsComponent` instead of `BaseStyledComponent`. They're missing BackColor, BorderColor, CssClass, Font, ForeColor, Height, Width, and Style (~8-10 properties each). Sub-element styles work via CascadingParameters, but the outer container cannot be styled.

### ⚠️ Image and Label — Wrong Base Class

Both `Image` and `Label` inherit `BaseWebFormsComponent` instead of `BaseStyledComponent`, despite Web Forms having them inherit `WebControl`. This makes them missing **all 11 style properties** (BackColor through Width). ImageMap was already fixed per team decision — Image and Label should follow.

### ⚠️ Calendar Style Sub-Properties Use Strings Instead of TableItemStyle

Calendar has 9 style sub-properties (DayStyle, TitleStyle, etc.) implemented as CSS string parameters instead of `TableItemStyle` objects. This prevents cascading style inheritance and doesn't match the Web Forms API shape.

---

## 3. Per-Category Rollup Table

### Editor Controls (23 controls)

| Control | Match | Needs Work | Missing | Health |
|---------|-------|------------|---------|--------|
| HiddenField | 9 | 0 | 0 | **100%** |
| Literal | 11 | 0 | 0 | **100%** |
| Localize | 11 | 0 | 0 | **100%** |
| MultiView | 13 | 0 | 0 | **100%** |
| PlaceHolder | 11 | 0 | 0 | **100%** |
| View | 12 | 0 | 0 | **100%** |
| Button | 28 | 0 | 1 | 96.6% |
| FileUpload | 27 | 0 | 1 | 96.4% |
| ImageButton | 30 | 0 | 2 | 93.8% |
| ImageMap | 28 | 0 | 1 | 96.6% |
| LinkButton | 27 | 1 | 2 | 90.0% |
| HyperLink | 21 | 1 | 2 | 87.5% |
| Table | 27 | 2 | 2 | 87.1% |
| Panel | 26 | 1 | 3 | 86.7% |
| TextBox | 27 | 1 | 5 | 81.8% |
| CheckBoxList | 31 | 0 | 7 | 81.6% |
| RadioButtonList | 30 | 1 | 7 | 78.9% |
| CheckBox | 22 | 0 | 6 | 78.6% |
| ListBox | 29 | 1 | 7 | 78.4% |
| RadioButton | 24 | 1 | 6 | 77.4% |
| DropDownList | 27 | 0 | 8 | 77.1% |
| Image | 14 | 0 | 11 | 56.0% |
| Label | 13 | 0 | 11 | 54.2% |
| **TOTALS** | **498/140** | **9/0** | **79/3** | **84.6%** |

*Format: Props/Events for totals row*

### Data Controls (12 controls)

| Control | Match | Needs Work | Missing | Health |
|---------|-------|------------|---------|--------|
| DataPager | 34 | 0 | 3 | 91.9% |
| AdRotator | 27 | 0 | 3 | 90.0% |
| Repeater | 19 | 0 | 3 | 86.4% |
| Calendar | 35 | 11 | 1 | 74.5% |
| DataList | 46 | 0 | 17 | 73.0% |
| BulletedList | 25 | 0 | 10 | 71.4% |
| DetailsView ⚠️ | 43 | 0 | 25 | 63.2% |
| DataGrid | 25 | 1 | 30 | 44.6% |
| FormView | 22 | 1 | 40 | 34.9% |
| ListView | 23 | 3 | 41 | 34.3% |
| Chart | 20 | 5 | 37 | 32.3% |
| GridView | 17 | 1 | 64 | 20.7% |
| **TOTALS** | **232/104** | **22/0** | **216/58** | **53.2%** |

⚠️ DetailsView exists only on unmerged branch `sprint3/detailsview-passwordrecovery`

### Validation Controls (6 controls)

| Control | Match | Needs Work | Missing | Health |
|---------|-------|------------|---------|--------|
| RangeValidator | 25 | 2 | 4 | 80.6% |
| RegularExpressionValidator | 24 | 2 | 4 | 80.0% |
| CustomValidator | 22 | 3 | 4 | 75.9% |
| RequiredFieldValidator | 22 | 2 | 5 | 75.9% |
| CompareValidator | 24 | 3 | 5 | 75.0% |
| ValidationSummary | 20 | 1 | 7 | 71.4% |
| **TOTALS** | **101/36** | **12/1** | **29/0** | **76.5%** |

### Navigation Controls (3 controls)

| Control | Match | Needs Work | Missing | Health |
|---------|-------|------------|---------|--------|
| SiteMapPath | 32 | 1 | 4 | 86.5% |
| TreeView | 32 | 0 | 24 | 57.1% |
| Menu | 23 | 1 | 37 | 37.7% |
| **TOTALS** | **64/23** | **2/0** | **59/6** | **56.5%** |

### Login Controls (7 controls)

| Control | Match | Needs Work | Missing | Health |
|---------|-------|------------|---------|--------|
| LoginStatus | 27 | 0 | 2 | 93.1% |
| LoginName | 21 | 0 | 2 | 91.3% |
| LoginView | 13 | 0 | 2 | 86.7% |
| ChangePassword | 49 | 2 | 16 | 73.1% |
| Login | 40 | 0 | 16 | 71.4% |
| CreateUserWizard | 64 | 3 | 27 | 68.1% |
| PasswordRecovery ⚠️ | 0 | 0 | 58 | 0% |
| **TOTALS** | **176/38** | **5/0** | **110/13** | **62.6%** |

⚠️ PasswordRecovery exists only on unmerged branch `sprint3/detailsview-passwordrecovery`

### Not Started / Deferred (2 controls)

| Control | Match | Needs Work | Missing | Health |
|---------|-------|------------|---------|--------|
| Substitution | 0 | 0 | 9 | 0% |
| Xml | 0 | 0 | 15 | 0% |
| **TOTALS** | **0/0** | **0/0** | **12/12** | **0%** |

Both are intentionally deferred — Substitution is tied to ASP.NET output caching (no Blazor equivalent), Xml/XSLT is a legacy pattern.

---

## 4. Feature-Complete Controls (100%)

These 6 controls match 100% of applicable Web Forms features:

| Control | Properties | Events | Total | Notes |
|---------|-----------|--------|-------|-------|
| MultiView | 6 | 7 | 13 | + 4 command constants. Correct Control (not WebControl) inheritance. |
| View | 4 | 8 | 12 | Activate/Deactivate events, CascadingParameter integration with MultiView. |
| Literal | 5 | 6 | 11 | LiteralMode (Encode/PassThrough). Correct Control inheritance. |
| Localize | 5 | 6 | 11 | Inherits Literal — identical runtime behavior (design-time marker). |
| PlaceHolder | 5 | 6 | 11 | RenderFragment ChildContent. Correct Control inheritance. |
| HiddenField | 4 | 5 | 9 | Value + ValueChanged. Correct Control (not WebControl) inheritance. |

**Common theme:** All 6 inherit from `Control` (not `WebControl`) in Web Forms, meaning they have no style properties by design. The Blazor implementations correctly mirror this with `BaseWebFormsComponent` inheritance.

---

## 5. Priority Recommendations for Milestone 6

### P0 — Base Class Fixes (close gaps across 10+ controls at once)

| # | Fix | Impact | Controls Affected |
|---|-----|--------|-------------------|
| 1 | **Add `AccessKey` to `BaseWebFormsComponent`** | ~40 gaps closed | All 53 controls |
| 2 | **Add `ToolTip` to `BaseWebFormsComponent`** | ~35 gaps closed | All controls missing it (~35) |
| 3 | **Make `DataBoundComponent<T>` inherit `BaseStyledComponent`** (or implement IStyle) | ~70 gaps closed | DataGrid, DetailsView, FormView, GridView, ListView + any future data controls |
| 4 | **Add `Display` property to `BaseValidator`** (None/Static/Dynamic) | 6 gaps closed + layout fidelity for all validation scenarios | All 6 validators |
| 5 | **Add `SetFocusOnError` to `BaseValidator`** | 6 gaps closed | All 6 validators |
| 6 | **Change `Image` base class** to `BaseStyledComponent` | 11 gaps closed | Image (and fixes Image/ImageMap inconsistency) |
| 7 | **Change `Label` base class** to `BaseStyledComponent` | 11 gaps closed | Label |

**Estimated total: ~180 gaps closed with 7 base class changes.**

### P1 — Individual Control Gaps (high migration impact)

| # | Fix | Impact | Notes |
|---|-----|--------|-------|
| 8 | **Merge `sprint3/detailsview-passwordrecovery` to dev** | 2 controls restored | Unblocks DetailsView + PasswordRecovery |
| 9 | **GridView: Add paging** (AllowPaging, PageSize, PageIndex, PagerSettings) | Most-used data control feature | GridView is ~80% of data grid usage in Web Forms apps |
| 10 | **GridView: Add sorting** (AllowSorting, SortDirection, SortExpression, Sorted/Sorting events) | Critical for data display | |
| 11 | **GridView: Add row editing events** (RowEditing, RowUpdating, RowDeleting, RowCancelingEdit) | Inline editing support | |
| 12 | **Calendar: Convert style strings to TableItemStyle objects** | 9 properties corrected | DayStyle, TitleStyle, etc. |
| 13 | **Calendar: Convert DayNameFormat/TitleFormat to enums** | 2 properties corrected | String → enum for type safety |
| 14 | **FormView: Add CssClass, header/footer, empty data** | ~15 gaps closed | FormView is at 34.9% health |
| 15 | **Login controls: Add outer WebControl styles** | ~30 gaps closed | ChangePassword, CreateUserWizard, Login all missing container styles |
| 16 | **HyperLink: Rename `NavigationUrl` → `NavigateUrl`** | 1 gap closed | Breaking name mismatch blocks migration |
| 17 | **Validator `ControlToValidate`: Consider string ID support** | 5 controls improved | Current `ForwardRef` API doesn't match Web Forms migration pattern |
| 18 | **ValidationSummary: Add HeaderText, ValidationGroup** | Key migration features | Most ValidationSummary usage includes HeaderText |

### P2 — Nice-to-Have (edge cases, rarely used properties)

| # | Fix | Notes |
|---|-----|-------|
| 19 | ListControl `DataTextFormatString` | Format strings for BulletedList, CheckBoxList, DropDownList, ListBox, RadioButtonList |
| 20 | ListControl `AppendDataBoundItems` | Static + dynamic item merging |
| 21 | CausesValidation/ValidationGroup on CheckBox, RadioButton, TextBox | Validation trigger support |
| 22 | Menu `Orientation` property | Currently hardcoded to vertical |
| 23 | TreeView node-level styles (HoverNodeStyle, LeafNodeStyle, etc.) | Fine-grained visual control |
| 24 | Login/ChangePassword `Orientation` and `TextLayout` | Layout variants |
| 25 | DataList/DataGrid editing support (EditItemIndex, EditItemStyle) | Inline editing |
| 26 | ListView CRUD events (ItemDeleting, ItemInserting, etc.) | Full CRUD pipeline |
| 27 | Label `AssociatedControlID` | Renders `<label for="...">` |
| 28 | Image `BackImageUrl` on Panel | Background image support |

### Deferred — Intentionally Not Implementing

| Control/Feature | Reason |
|----------------|--------|
| **Substitution** | Tied to ASP.NET output caching pipeline. No Blazor equivalent. Recommend using Blazor component lifecycle instead. |
| **Xml (XSLT)** | Legacy pattern. Recommend converting XML data to C# objects and using Blazor templates. |
| **Chart advanced properties** (~25 props) | Chart.js canvas architecture intentionally deviates from GDI+ image rendering. Annotations, image storage, serializer have no canvas equivalent. |
| **DataSourceID** (all data controls) | Server-side DataSource controls don't exist in Blazor. Use `Items` parameter instead. |
| **MailDefinition** (ChangePassword, CreateUserWizard) | Email sending is a server concern, not a component concern in Blazor. |
| **Focus() method** | Server-initiated focus requires JS interop. Low priority — developers can use Blazor's `ElementReference.FocusAsync()`. |
| **ViewState/EnableViewState** | Blazor has no ViewState concept. All marked N/A. |
| **EnableTheming/SkinID** | Marked obsolete. Theme strategy documented separately (CascadingValue ThemeProvider approach). |

---

## 6. Unmerged Branch Alert

> ### ⚠️ CRITICAL: Branch `sprint3/detailsview-passwordrecovery` Must Be Merged
>
> **Two fully-reviewed, gate-approved components exist ONLY on this branch:**
>
> | Component | Properties | Events | Tests | Gate Status |
> |-----------|-----------|--------|-------|-------------|
> | DetailsView | 27 match, 23 missing | 16 match, 2 missing | Referenced in Sprint 3 | ✅ APPROVED by Forge |
> | PasswordRecovery | 0 on current branch | 0 on current branch | 29 bUnit tests referenced | ✅ APPROVED by Forge |
>
> **Impact of NOT merging:**
> - `status.md` claims 50/53 (94%) but actual shipped count is 48/53 (91%)
> - PasswordRecovery audit shows 0% because the files don't exist on the current branch
> - DetailsView features (strong CRUD events, auto-generated rows, edit mode) are inaccessible
> - Any work on `milestone4/chart-component` branch diverges further from these components
>
> **Recommended action:** Merge to `dev` immediately, then rebase current milestone branch.

---

## Appendix: Control Health Rankings (All 53)

| Rank | Control | Category | Health | Status |
|------|---------|----------|--------|--------|
| 1 | HiddenField | Editor | 100% | ✅ Complete |
| 2 | Literal | Editor | 100% | ✅ Complete |
| 3 | Localize | Editor | 100% | ✅ Complete |
| 4 | MultiView | Editor | 100% | ✅ Complete |
| 5 | PlaceHolder | Editor | 100% | ✅ Complete |
| 6 | View | Editor | 100% | ✅ Complete |
| 7 | Button | Editor | 96.6% | ✅ Near-complete |
| 8 | ImageMap | Editor | 96.6% | ✅ Near-complete |
| 9 | FileUpload | Editor | 96.4% | ✅ Near-complete |
| 10 | ImageButton | Editor | 93.8% | ✅ Near-complete |
| 11 | LoginStatus | Login | 93.1% | ✅ Near-complete |
| 12 | DataPager | Data | 91.9% | ✅ Near-complete |
| 13 | LoginName | Login | 91.3% | ✅ Near-complete |
| 14 | AdRotator | Data | 90.0% | ✅ Near-complete |
| 15 | LinkButton | Editor | 90.0% | ✅ Near-complete |
| 16 | HyperLink | Editor | 87.5% | Good |
| 17 | Table | Editor | 87.1% | Good |
| 18 | LoginView | Login | 86.7% | Good |
| 19 | Panel | Editor | 86.7% | Good |
| 20 | SiteMapPath | Navigation | 86.5% | Good |
| 21 | Repeater | Data | 86.4% | Good |
| 22 | TextBox | Editor | 81.8% | Good |
| 23 | CheckBoxList | Editor | 81.6% | Good |
| 24 | RangeValidator | Validation | 80.6% | Good |
| 25 | RegExValidator | Validation | 80.0% | Good |
| 26 | RadioButtonList | Editor | 78.9% | Adequate |
| 27 | CheckBox | Editor | 78.6% | Adequate |
| 28 | ListBox | Editor | 78.4% | Adequate |
| 29 | RadioButton | Editor | 77.4% | Adequate |
| 30 | DropDownList | Editor | 77.1% | Adequate |
| 31 | CustomValidator | Validation | 75.9% | Adequate |
| 32 | RequiredFieldValidator | Validation | 75.9% | Adequate |
| 33 | CompareValidator | Validation | 75.0% | Adequate |
| 34 | Calendar | Data | 74.5% | Adequate |
| 35 | ChangePassword | Login | 73.1% | Adequate |
| 36 | DataList | Data | 73.0% | Adequate |
| 37 | BulletedList | Data | 71.4% | Adequate |
| 38 | Login | Login | 71.4% | Adequate |
| 39 | ValidationSummary | Validation | 71.4% | Adequate |
| 40 | CreateUserWizard | Login | 68.1% | Needs Work |
| 41 | DetailsView | Data | 63.2% | ⚠️ Unmerged branch |
| 42 | TreeView | Navigation | 57.1% | Needs Work |
| 43 | Image | Editor | 56.0% | Needs Work |
| 44 | Label | Editor | 54.2% | Needs Work |
| 45 | DataGrid | Data | 44.6% | Needs Work |
| 46 | Menu | Navigation | 37.7% | Significant gaps |
| 47 | FormView | Data | 34.9% | Significant gaps |
| 48 | ListView | Data | 34.3% | Significant gaps |
| 49 | Chart | Data | 32.3% | Architectural deviation |
| 50 | GridView | Data | 20.7% | 🔴 Critical gaps |
| 51 | PasswordRecovery | Login | 0% | ⚠️ Unmerged branch |
| 52 | Substitution | Deferred | 0% | Intentionally deferred |
| 53 | Xml | Deferred | 0% | Intentionally deferred |
