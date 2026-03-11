# BWFC Capabilities vs Run 5 Manual Work — Analysis & Recommendations

**Author:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-03-05
**Based on:** WingtipToys Migration Run 5 (2026-03-04)

---

## 1. BWFC Event Handler Inventory

BWFC components expose EventCallback parameters that **already match Web Forms event names**. This means a migration script can preserve `OnClick="Button1_Click"` verbatim — the handler name stays the same, only the underlying type changes from a server-side postback delegate to a Blazor `EventCallback`.

### Base Component Events (inherited by ALL controls)

| EventCallback | Web Forms Equivalent | Source Class |
|---|---|---|
| `OnInit` | `OnInit` | BaseWebFormsComponent |
| `OnLoad` | `OnLoad` | BaseWebFormsComponent |
| `OnPreRender` | `OnPreRender` | BaseWebFormsComponent |
| `OnUnload` | `OnUnload` | BaseWebFormsComponent |
| `OnDisposed` | `Disposed` | BaseWebFormsComponent |
| `OnDataBinding` | `DataBinding` | BaseWebFormsComponent |

### Button Controls (Button, LinkButton, ImageButton)

| EventCallback | Web Forms Equivalent | Source Class |
|---|---|---|
| `OnClick` | `OnClick` / `Click` | ButtonBaseComponent |
| `OnCommand` | `OnCommand` / `Command` | ButtonBaseComponent |

### Data-Bound Controls (GridView, ListView, FormView, DetailsView, DataGrid, DataList, Repeater)

| EventCallback | Web Forms Equivalent | Controls |
|---|---|---|
| `OnDataBound` | `DataBound` | BaseDataBoundComponent (all) |
| `OnItemDataBound` | `ItemDataBound` | ListView, DataList |
| `OnRowCommand` | `RowCommand` | GridView |
| `RowEditing` | `RowEditing` | GridView |
| `RowUpdating` | `RowUpdating` | GridView |
| `RowDeleting` | `RowDeleting` | GridView |
| `RowCancelingEdit` | `RowCancelingEdit` | GridView |
| `SelectedIndexChanging` | `SelectedIndexChanging` | GridView, ListView |
| `SelectedIndexChanged` | `SelectedIndexChanged` | GridView, ListView, DataGrid, DropDownList, ListBox, CheckBoxList, RadioButtonList |
| `Sorting` / `Sorted` | `Sorting` / `Sorted` | GridView, ListView |
| `PageIndexChanging` / `PageIndexChanged` | `PageIndexChanging` / `PageIndexChanged` | GridView, FormView, DetailsView, DataPager |
| `ItemCommand` | `ItemCommand` | ListView, FormView, DetailsView |
| `ItemEditing` / `ItemCanceling` | `ItemEditing` / `ItemCanceling` | ListView |
| `ItemDeleting` / `ItemDeleted` | `ItemDeleting` / `ItemDeleted` | ListView, DetailsView |
| `ItemInserting` / `ItemInserted` | `ItemInserting` / `ItemInserted` | ListView, DetailsView |
| `ItemUpdating` / `ItemUpdated` | `ItemUpdating` / `ItemUpdated` | ListView, DetailsView |
| `ItemCreated` | `ItemCreated` | ListView, DataGrid |
| `ModeChanging` / `ModeChanged` | `ModeChanging` / `ModeChanged` | FormView, DetailsView |
| `OnItemDeleting` / `OnItemDeleted` | `ItemDeleting` / `ItemDeleted` | FormView |
| `OnItemInserting` / `OnItemInserted` | `ItemInserting` / `ItemInserted` | FormView |
| `OnItemUpdating` / `OnItemUpdated` | `ItemUpdating` / `ItemUpdated` | FormView |
| `PageIndexChanged` (DataGrid) | `PageIndexChanged` | DataGrid |
| `SortCommand` | `SortCommand` | DataGrid |
| `OnItemCommand` / `OnEditCommand` / `OnCancelCommand` / `OnUpdateCommand` / `OnDeleteCommand` | `ItemCommand` / `EditCommand` / `CancelCommand` / `UpdateCommand` / `DeleteCommand` | DataGrid |
| `OnPageIndexChanging` / `OnPageIndexChanged` | `PageIndexChanging` / `PageIndexChanged` | DataPager |

### Input Controls

| EventCallback | Web Forms Equivalent | Control |
|---|---|---|
| `OnTextChanged` | `TextChanged` | TextBox |
| `OnCheckedChanged` | `CheckedChanged` | CheckBox, RadioButton |
| `OnSelectedIndexChanged` | `SelectedIndexChanged` | DropDownList, ListBox, CheckBoxList, RadioButtonList |
| `OnFileSelected` | N/A (new) | FileUpload |
| `OnValueChanged` | `ValueChanged` | HiddenField |

### Calendar

| EventCallback | Web Forms Equivalent |
|---|---|
| `OnSelectionChanged` | `SelectionChanged` |
| `OnDayRender` | `DayRender` |
| `OnVisibleMonthChanged` | `VisibleMonthChanged` |

### Navigation / Other

| EventCallback | Web Forms Equivalent | Control |
|---|---|---|
| `OnClick` | `Click` | BulletedList, ImageMap |
| `MenuItemClick` | `MenuItemClick` | Menu |
| `MenuItemDataBound` | `MenuItemDataBound` | Menu |
| `SelectedNodeChanged` | `SelectedNodeChanged` | TreeView |
| `OnTreeNodeCheckChanged` | `TreeNodeCheckChanged` | TreeView |
| `OnTreeNodeCollapsed` / `OnTreeNodeExpanded` | `TreeNodeCollapsed` / `TreeNodeExpanded` | TreeView |
| `OnAdCreated` | `AdCreated` | AdRotator |
| `OnTick` | `Tick` | Timer |
| `OnActiveViewChanged` | `ActiveViewChanged` | MultiView |
| `OnActivate` / `OnDeactivate` | `Activate` / `Deactivate` | View |

### Login Controls

| EventCallback | Web Forms Equivalent | Control |
|---|---|---|
| `OnLoggingIn` / `OnLoggedIn` / `OnLoginError` / `OnAuthenticate` | `LoggingIn` / `LoggedIn` / `LoginError` / `Authenticate` | Login |
| `OnLoggingOut` / `OnLoggedOut` | `LoggingOut` / `LoggedOut` | LoginStatus |
| `OnCancelButtonClick` / `OnChangedPassword` / `OnChangePasswordError` / `OnChangingPassword` / `OnContinueButtonClick` | Same names | ChangePassword |
| `OnCreatingUser` / `OnCreatedUser` / `OnCreateUserError` / `OnCancelButtonClick` / `OnContinueButtonClick` / `OnActiveStepChanged` / `OnNextButtonClick` / `OnPreviousButtonClick` / `OnFinishButtonClick` | Same names | CreateUserWizard |
| `OnVerifyingUser` / `OnUserLookupError` / `OnVerifyingAnswer` / `OnAnswerLookupError` / `OnSendingMail` / `OnSendMailError` | Same names | PasswordRecovery |

**Total: ~95+ EventCallback parameters across 30+ components.**

### Key Finding: Event Handler Names Are Script-Convertible

The `On` prefix convention is consistent. A Web Forms attribute like `OnClick="Button1_Click"` maps directly to the BWFC `OnClick` EventCallback. The migration script can **preserve these attributes verbatim** — the handler method name stays the same, and BWFC already has the matching parameter. Only the method *signature* needs updating (from `void Handler(object sender, EventArgs e)` to `void Handler(MouseEventArgs e)` or `Task Handler(MouseEventArgs e)`).

---

## 2. BWFC Component Coverage vs Run 5 Manual Work

### What Run 5 Manually Rewrote — Could BWFC Have Handled It?

| Run 5 Manual Fix | BWFC Component Available? | Could It Have Been Used? | Assessment |
|---|---|---|---|
| **ProductList: ListView → `@foreach` + HTML table** | ✅ `ListView<T>` with `ItemTemplate`, `LayoutTemplate`, `AlternatingItemTemplate`, sorting, paging, 17 event callbacks | **YES** — BWFC ListView supports `ItemTemplate` and `LayoutTemplate`. The `@foreach` rewrite was unnecessary. | **High impact** — ListView is the #1 component that should be preserved |
| **ProductDetails: FormView → direct HTML** | ✅ `FormView<T>` with `ItemTemplate`, `EditItemTemplate`, `InsertItemTemplate`, paging, mode switching, 12 event callbacks | **YES** — BWFC FormView supports `ItemTemplate` for read-only display. The manual HTML rewrite was unnecessary. | **High impact** — FormView provides template-based rendering |
| **ShoppingCart: GridView → HTML table** | ✅ `GridView<T>` with `AutoGenerateColumns`, `BoundField`, `TemplateField`, editing, selection, sorting, paging, 11 event callbacks | **YES** — BWFC GridView handles table rendering with columns. The HTML table rewrite duplicated existing functionality. | **High impact** — GridView is the most common data control |
| **MainLayout: ListView (category menu) → `@foreach`** | ✅ `ListView<T>` or `Repeater<T>` | **YES** — A Repeater with `ItemTemplate` would render the category links identically. | **Medium impact** — layout-level data binding |
| **LoginView → AuthorizeView** | ✅ Script handles this (Run 5 enhancement) | Already automated in Layer 1 | N/A — solved |
| **Page.Title → static text** | ✅ BWFC has `PageService` + `Page` component | **YES** — BWFC provides `PageService.Title` as the direct equivalent of `Page.Title`, and a `Page` component that renders `<PageTitle>`. The script should emit `PageService.Title` mappings. | Script enhancement candidate |
| **Page base class → ComponentBase** | ✅ BWFC has `WebFormsPage` | **PARTIAL** — BWFC's `WebFormsPage` component provides Page lifecycle compatibility. Code-behinds should inherit `ComponentBase` but can use `WebFormsPage` in markup. | Low impact |
| **EF6 → EF Core** | N/A | Not a component concern | Architectural — always manual |
| **Identity/Auth rewrite** | N/A | Not a component concern | Architectural — always manual |
| **Session → services** | N/A | Not a component concern | Architectural — always manual |

### Key Finding: 3 of the Top 4 Manual Rewrites Were Unnecessary

ProductList (ListView), ProductDetails (FormView), and ShoppingCart (GridView) were all manually rewritten to raw HTML when BWFC already had fully functional equivalents. The migration should have **preserved the BWFC component tags** and only changed the data-binding approach (SelectMethod → Items parameter or @inject service).

**Estimated savings if BWFC components had been used:** ~180s of the ~440s Layer 2 time (page fixes category), or **~40% of manual page-fix effort**.

---

## 3. Script Enhancement Recommendations (Prioritized)

### Priority 1: Preserve BWFC Data Controls Instead of Rewriting to HTML

| Enhancement | Pattern | Output | Impact | Complexity |
|---|---|---|---|---|
| **Preserve ListView tags** | `<asp:ListView ... SelectMethod="X">` after asp: strip → `<ListView ... >` with `Items` parameter TODO | Keep `<ListView>` with `ItemTemplate` intact, add `@* TODO: Replace SelectMethod with Items=@yourData *@` | **Eliminates ~60s** of ProductList rewrite | Easy |
| **Preserve FormView tags** | `<asp:FormView ... SelectMethod="X">` | Keep `<FormView>` with templates intact, add Items TODO | **Eliminates ~30s** of ProductDetails rewrite | Easy |
| **Preserve GridView tags** | `<asp:GridView ... SelectMethod="X">` | Keep `<GridView>` with columns intact, add Items TODO | **Eliminates ~30s** of ShoppingCart rewrite | Easy |

**Total estimated savings: ~120s per project with data controls (3+ items eliminated)**

### Priority 2: Page.Title → PageService.Title Mapping

| Enhancement | Pattern | Output | Impact | Complexity |
|---|---|---|---|---|
| **Page.Title → PageService.Title** | `@(Title)` or `@(Page.Title)` or `Title = "..."` in code-behind | Map to `PageService.Title = "..."` (BWFC already provides `PageService` and `Page` component that renders `<PageTitle>`) | **4+ items per project** | Easy |

### Priority 3: Event Handler Annotation

| Enhancement | Pattern | Output | Impact | Complexity |
|---|---|---|---|---|
| **Preserve OnClick** | `OnClick="Handler"` on Button/LinkButton/ImageButton | Keep attribute as-is (BWFC has `OnClick` EventCallback), add `@* TODO: Update Handler signature from (object, EventArgs) to (MouseEventArgs) *@` | **Preserves handler wiring** | Easy |
| **Preserve OnSelectedIndexChanged** | `OnSelectedIndexChanged="Handler"` on list controls | Keep attribute, add signature TODO | **Preserves handler wiring** | Easy |
| **Preserve OnCommand** | `OnCommand="Handler"` on buttons | Keep attribute, add signature TODO | **Preserves handler wiring** | Easy |

**Total: all BWFC event handler attributes should be preserved, not stripped. The script should only annotate the signature change needed.**

### Priority 4: Page Base Class Swap

| Enhancement | Pattern | Output | Impact | Complexity |
|---|---|---|---|---|
| **Base class regex** | `: Page` or `: System.Web.UI.Page` in .razor.cs | Replace with `: ComponentBase` | **4+ items per project** | Easy |

### Priority 5: BundleConfig → Link Tags

| Enhancement | Pattern | Output | Impact | Complexity |
|---|---|---|---|---|
| **BundleConfig parser** | Read `BundleConfig.cs`, extract CSS/JS paths | Emit `<link>` and `<script>` tags in App.razor `<head>` | **2+ items per project** | Medium |

### Priority 6: dotnet new blazor Scaffolding

| Enhancement | Pattern | Output | Impact | Complexity |
|---|---|---|---|---|
| **Use dotnet new blazor** | Replace hand-crafted csproj/Program.cs scaffold | Run `dotnet new blazor --interactivity Server` then overlay BWFC references | **Better scaffolding** — gets latest templates | Medium |

---

## 4. Component Improvement Recommendations

### 4.1 ListView SelectMethod/Items Data Binding Documentation

**Gap:** Run 5 replaced ListView with `@foreach` because the migration developer didn't know `Items` parameter existed. The `SelectMethod` annotation says "replace with DI" but doesn't say "use `Items=@data` instead."

**Recommendation:** Update SelectMethod TODO comments to explicitly reference the BWFC `Items` parameter:
```
@* TODO: Replace SelectMethod="GetProducts" with Items="@_products" where _products is loaded in OnInitializedAsync *@
```

### 4.2 FormView ItemTemplate Rendering with Single Items

**Gap:** FormView was replaced with raw HTML for ProductDetails (a single-item display). This suggests the migration developer wasn't confident FormView works for single-item scenarios.

**Recommendation:** Add a FormView sample showing single-item display with `Items` containing one element. Verify `RenderOuterTable` works correctly for this case.

### 4.3 GridView Template Column Support for Complex Cells

**Gap:** ShoppingCart's GridView had cells with arithmetic (`Quantity * UnitPrice`) and Remove buttons. The replacement used raw HTML.

**Recommendation:** Ensure GridView `TemplateField` with `ItemTemplate` can handle computed values and button columns. Document a "ShoppingCart" migration example showing the pattern.

### 4.4 Missing Event Callbacks — Gap Analysis

After comparing with the full Web Forms event surface:

| Missing Event | Web Forms Control | Recommendation |
|---|---|---|
| `OnRowDataBound` | GridView | **Add** — commonly used for conditional formatting |
| `OnRowCreated` | GridView | **Add** — used for header/footer customization |
| `OnItemCommand` | Repeater | **Add** — Repeater currently has zero events |
| `OnItemCreated` / `OnItemDataBound` | Repeater | **Add** — standard data-bound events |
| `OnPageIndexChanging` | GridView | Already has `PageIndexChanged` — verify paging cancellation support |

### 4.5 Repeater Event Support

**Gap:** Repeater has zero EventCallback parameters. Web Forms Repeater has `ItemCommand`, `ItemCreated`, `ItemDataBound`. This limits its usefulness for interactive scenarios.

**Recommendation:** Add at minimum `OnItemDataBound` to Repeater for consistency with other data controls.

---

## 5. Migration Standards Summary

### Canonical Standards (per Jeff's directive)

| Standard | Value |
|---|---|
| **Target framework** | .NET 10 |
| **Project template** | `dotnet new blazor --interactivity Server` |
| **Database migration** | EF6 → EF Core (always) |
| **Identity migration** | ASP.NET Identity → ASP.NET Core Identity (when present) |
| **Event handler strategy** | Preserve BWFC event names; only update method signatures |
| **Data control strategy** | Prefer BWFC ListView/GridView/FormView over raw HTML `@foreach` |
| **Layout migration** | Site.Master → MainLayout.razor (script handles); LoginView → AuthorizeView (script handles) |
| **Session state** | Replace with scoped DI services |
| **Static assets** | Relocate to `wwwroot/`; BundleConfig → explicit `<link>`/`<script>` |

### What the Script Should Do (Layer 1)

1. Strip `asp:` prefixes → preserve BWFC component tags (already done)
2. Convert data-binding expressions (already done — 5 variants)
3. Convert LoginView → AuthorizeView (already done)
4. Flag SelectMethod → recommend `Items` parameter (done, improve guidance)
5. **NEW:** Preserve event handler attributes verbatim (OnClick, OnCommand, etc.)
6. **NEW:** Map Page.Title → `PageService.Title` (BWFC provides PageService + Page component)
7. **NEW:** Swap Page base class → ComponentBase
8. **NEW:** Annotate event handler signatures needing update

### What Stays Manual (Layer 2)

1. EF6 → EF Core DbContext and models
2. Identity/Auth subsystem
3. Session → scoped services
4. Complex business logic (checkout, payment)
5. SelectMethod → actual DI service injection code
6. Complex data-binding expressions with arithmetic/method chains

---

## Appendix: Full EventCallback Count by Component

| Component | EventCallback Count |
|---|---|
| BaseWebFormsComponent | 5 (OnInit, OnLoad, OnPreRender, OnUnload, OnDisposed) |
| BaseDataBoundComponent | 1 (OnDataBound) |
| Button/LinkButton/ImageButton | 2 (OnClick, OnCommand) |
| GridView | 11 (SelectedIndexChanging, SelectedIndexChanged, Sorting, Sorted, PageIndexChanged, OnRowCommand, RowEditing, RowUpdating, RowDeleting, RowCancelingEdit, + base) |
| ListView | 17 (OnLayoutCreated, OnItemDataBound, ItemCommand, ItemEditing, ItemCanceling, ItemDeleting, ItemDeleted, ItemInserting, ItemInserted, ItemUpdating, ItemUpdated, ItemCreated, Sorting, Sorted, PagePropertiesChanging, PagePropertiesChanged, SelectedIndexChanging, SelectedIndexChanged) |
| FormView | 12 (ModeChanging, ModeChanged, ItemCommand, ItemCreated, PageIndexChanging, PageIndexChanged, OnItemDeleting, OnItemDeleted, OnItemInserting, OnItemInserted, OnItemUpdating, OnItemUpdated) |
| DetailsView | 12 (ItemCommand, ItemDeleting, ItemDeleted, ItemInserting, ItemInserted, ItemUpdating, ItemUpdated, ModeChanging, ModeChanged, PageIndexChanging, PageIndexChanged) |
| DataGrid | 10 (PageIndexChanged, SortCommand, ItemCreated, ItemDataBound, SelectedIndexChanged, OnItemCommand, OnEditCommand, OnCancelCommand, OnUpdateCommand, OnDeleteCommand) |
| DataPager | 2 (OnPageIndexChanging, OnPageIndexChanged) |
| DropDownList | 2 (OnSelectedIndexChanged, SelectedIndexChanged) |
| CheckBoxList | 2 (OnSelectedIndexChanged, SelectedValuesChanged) |
| RadioButtonList | 2 (OnSelectedIndexChanged, SelectedValueChanged) |
| ListBox | 2 (OnSelectedIndexChanged, SelectedIndexChanged) |
| TextBox | 1 (OnTextChanged) |
| CheckBox | 1 (OnCheckedChanged) |
| RadioButton | 1 (OnCheckedChanged) |
| Calendar | 3 (OnSelectionChanged, OnDayRender, OnVisibleMonthChanged) |
| Menu | 2 (MenuItemClick, MenuItemDataBound) |
| TreeView | 4 (SelectedNodeChanged, OnTreeNodeCheckChanged, OnTreeNodeCollapsed, OnTreeNodeExpanded) |
| DataList | 1 (OnItemDataBound) |
| BulletedList | 1 (OnClick) |
| ImageMap | 1 (OnClick) |
| HiddenField | 1 (OnValueChanged) |
| FileUpload | 1 (OnFileSelected) |
| AdRotator | 1 (OnAdCreated) |
| Timer | 1 (OnTick) |
| MultiView | 1 (OnActiveViewChanged) |
| View | 2 (OnActivate, OnDeactivate) |
| Login | 4 (OnLoggingIn, OnLoggedIn, OnLoginError, OnAuthenticate) |
| LoginStatus | 2 (OnLoggingOut, OnLoggedOut) |
| ChangePassword | 5 (OnCancelButtonClick, OnChangedPassword, OnChangePasswordError, OnChangingPassword, OnContinueButtonClick) |
| CreateUserWizard | 9 (OnCreatingUser, OnCreatedUser, OnCreateUserError, OnCancelButtonClick, OnContinueButtonClick, OnActiveStepChanged, OnNextButtonClick, OnPreviousButtonClick, OnFinishButtonClick) |
| PasswordRecovery | 6 (OnVerifyingUser, OnUserLookupError, OnVerifyingAnswer, OnAnswerLookupError, OnSendingMail, OnSendMailError) |

**Grand total: ~95+ unique EventCallback parameters across the library.**
