# Event Handler Fidelity & SelectMethod Audit

**Author:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-03-06
**Requested by:** Jeffrey T. Fritz
**Triggered by:** Run 8 Benchmark Report — "Event handler signatures (sender, EventArgs) — 15 instances — Required Blazor EventCallback conversion"
**Status:** Proposal — awaiting review

---

## Table of Contents

1. [Part 1: Event Handler Fidelity Audit](#part-1-event-handler-fidelity-audit)
   - [1.1 Data-Bound Controls](#11-data-bound-controls)
   - [1.2 Input/Form Controls](#12-inputform-controls)
   - [1.3 Navigation Controls](#13-navigation-controls)
2. [Part 2: SelectMethod & Data-Binding Methods](#part-2-selectmethod--data-binding-methods)
3. [Part 3: Event Handler Signature Migration](#part-3-event-handler-signature-migration)
4. [Part 4: Prioritized Recommendations](#part-4-prioritized-recommendations)

---

## Part 1: Event Handler Fidelity Audit

### Legend

| Symbol | Meaning |
|--------|---------|
| ✅ | Present with correct type and On-prefix alias |
| ✅⚠️ | Present but On-prefix alias missing |
| ⚠️ | Present but wrong type parameter |
| ❌ | Missing entirely |

### Common Base Events (all components via BaseWebFormsComponent)

| Web Forms Event | WF Delegate | BWFC Parameter | On-Prefix | Status |
|----------------|-------------|----------------|-----------|--------|
| `DataBinding` | `EventHandler` | `EventCallback<EventArgs> OnDataBinding` | ✅ (On only) | ✅ |
| `Init` | `EventHandler` | `EventCallback<EventArgs> OnInit` | ✅ (On only) | ✅ |
| `Load` | `EventHandler` | `EventCallback<EventArgs> OnLoad` | ✅ (On only) | ✅ |
| `PreRender` | `EventHandler` | `EventCallback<EventArgs> OnPreRender` | ✅ (On only) | ✅ |
| `Unload` | `EventHandler` | `EventCallback<EventArgs> OnUnload` | ✅ (On only) | ✅ |
| `Disposed` | `EventHandler` | `EventCallback<EventArgs> OnDisposed` | ✅ (On only) | ✅ |

### Common Data Base Events (via BaseDataBoundComponent)

| Web Forms Event | WF Delegate | BWFC Parameter | On-Prefix | Status |
|----------------|-------------|----------------|-----------|--------|
| `DataBound` | `EventHandler` | `EventCallback<EventArgs> OnDataBound` | ✅ (On only) | ✅ |

---

### 1.1 Data-Bound Controls

#### GridView

| Web Forms Event | WF Delegate Type | BWFC EventCallback Type | Bare Name | On-Prefix | Status |
|----------------|-----------------|------------------------|-----------|-----------|--------|
| `Sorting` | `GridViewSortEventHandler` (`GridViewSortEventArgs`) | `EventCallback<GridViewSortEventArgs>` | `Sorting` | `OnSorting` | ✅ |
| `Sorted` | `EventHandler` | `EventCallback<GridViewSortEventArgs>` | `Sorted` | `OnSorted` | ✅ |
| `PageIndexChanging` | `GridViewPageEventHandler` (`GridViewPageEventArgs`) | — | — | — | ❌ |
| `PageIndexChanged` | `EventHandler` | `EventCallback<PageChangedEventArgs>` | `PageIndexChanged` | `OnPageIndexChanged` | ✅ |
| `RowCommand` | `GridViewCommandEventHandler` | `EventCallback<GridViewCommandEventArgs>` | — | `OnRowCommand` | ✅⚠️ (On only, no bare) |
| `RowEditing` | `GridViewEditEventHandler` (`GridViewEditEventArgs`) | `EventCallback<GridViewEditEventArgs>` | `RowEditing` | `OnRowEditing` | ✅ |
| `RowUpdating` | `GridViewUpdateEventHandler` (`GridViewUpdateEventArgs`) | `EventCallback<GridViewUpdateEventArgs>` | `RowUpdating` | `OnRowUpdating` | ✅ |
| `RowUpdated` | `GridViewUpdatedEventHandler` | — | — | — | ❌ |
| `RowDeleting` | `GridViewDeleteEventHandler` (`GridViewDeleteEventArgs`) | `EventCallback<GridViewDeleteEventArgs>` | `RowDeleting` | `OnRowDeleting` | ✅ |
| `RowDeleted` | `GridViewDeletedEventHandler` | — | — | — | ❌ |
| `RowCancelingEdit` | `GridViewCancelEditEventHandler` | `EventCallback<GridViewCancelEditEventArgs>` | `RowCancelingEdit` | `OnRowCancelingEdit` | ✅ |
| `RowCreated` | `GridViewRowEventHandler` | — | — | — | ❌ |
| `RowDataBound` | `GridViewRowEventHandler` | — | — | — | ❌ |
| `SelectedIndexChanging` | `GridViewSelectEventHandler` | `EventCallback<GridViewSelectEventArgs>` | `SelectedIndexChanging` | `OnSelectedIndexChanging` | ✅ |
| `SelectedIndexChanged` | `EventHandler` | `EventCallback<int>` | `SelectedIndexChanged` | `OnSelectedIndexChanged` | ⚠️ type=`int` not `EventArgs` |

**GridView Summary:** 11 of 15 events present. **4 missing** (PageIndexChanging, RowUpdated, RowDeleted, RowCreated, RowDataBound — 5 total). 1 type mismatch (SelectedIndexChanged uses `int` instead of `EventArgs`). RowCommand has On-prefix only (no bare name).

---

#### ListView

| Web Forms Event | WF Delegate Type | BWFC EventCallback Type | Bare Name | On-Prefix | Status |
|----------------|-----------------|------------------------|-----------|-----------|--------|
| `ItemCommand` | `EventHandler<ListViewCommandEventArgs>` | `EventCallback<ListViewCommandEventArgs>` | `ItemCommand` | `OnItemCommand` | ✅ |
| `ItemCreated` | `EventHandler<ListViewItemEventArgs>` | `EventCallback<ListViewItemEventArgs>` | `ItemCreated` | `OnItemCreated` | ✅ |
| `ItemDataBound` | `EventHandler<ListViewItemEventArgs>` | `EventCallback<ListViewItemEventArgs>` | — | `OnItemDataBound` | ✅⚠️ (On only) |
| `ItemEditing` | `EventHandler<ListViewEditEventArgs>` | `EventCallback<ListViewEditEventArgs>` | `ItemEditing` | `OnItemEditing` | ✅ |
| `ItemCanceling` | `EventHandler<ListViewCancelEventArgs>` | `EventCallback<ListViewCancelEventArgs>` | `ItemCanceling` | `OnItemCanceling` | ✅ |
| `ItemDeleting` | `EventHandler<ListViewDeleteEventArgs>` | `EventCallback<ListViewDeleteEventArgs>` | `ItemDeleting` | `OnItemDeleting` | ✅ |
| `ItemDeleted` | `EventHandler<ListViewDeletedEventArgs>` | `EventCallback<ListViewDeletedEventArgs>` | `ItemDeleted` | `OnItemDeleted` | ✅ |
| `ItemInserting` | `EventHandler<ListViewInsertEventArgs>` | `EventCallback<ListViewInsertEventArgs>` | `ItemInserting` | `OnItemInserting` | ✅ |
| `ItemInserted` | `EventHandler<ListViewInsertedEventArgs>` | `EventCallback<ListViewInsertedEventArgs>` | `ItemInserted` | `OnItemInserted` | ✅ |
| `ItemUpdating` | `EventHandler<ListViewUpdateEventArgs>` | `EventCallback<ListViewUpdateEventArgs>` | `ItemUpdating` | `OnItemUpdating` | ✅ |
| `ItemUpdated` | `EventHandler<ListViewUpdatedEventArgs>` | `EventCallback<ListViewUpdatedEventArgs>` | `ItemUpdated` | `OnItemUpdated` | ✅ |
| `Sorting` | `EventHandler<ListViewSortEventArgs>` | `EventCallback<ListViewSortEventArgs>` | `Sorting` | `OnSorting` | ✅ |
| `Sorted` | `EventHandler<EventArgs>` | `EventCallback<ListViewSortEventArgs>` | `Sorted` | `OnSorted` | ✅ |
| `LayoutCreated` | `EventHandler` | `EventCallback<EventArgs>` | — | `OnLayoutCreated` | ✅⚠️ (On only) |
| `PagePropertiesChanging` | `EventHandler<PagePropertiesChangingEventArgs>` | `EventCallback<ListViewPagePropertiesChangingEventArgs>` | `PagePropertiesChanging` | `OnPagePropertiesChanging` | ✅ |
| `PagePropertiesChanged` | `EventHandler` | `EventCallback<EventArgs>` | `PagePropertiesChanged` | `OnPagePropertiesChanged` | ✅ |
| `SelectedIndexChanging` | `EventHandler<ListViewSelectEventArgs>` | `EventCallback<ListViewSelectEventArgs>` | `SelectedIndexChanging` | `OnSelectedIndexChanging` | ✅ |
| `SelectedIndexChanged` | `EventHandler` | `EventCallback<EventArgs>` | `SelectedIndexChanged` | `OnSelectedIndexChanged` | ✅ |

**ListView Summary:** 18 of 18 events present. **Best coverage of any control.** 2 have On-prefix only (ItemDataBound, LayoutCreated).

---

#### FormView

| Web Forms Event | WF Delegate Type | BWFC EventCallback Type | Bare Name | On-Prefix | Status |
|----------------|-----------------|------------------------|-----------|-----------|--------|
| `ItemCommand` | `FormViewCommandEventHandler` | `EventCallback<FormViewCommandEventArgs>` | `ItemCommand` | `OnItemCommand` | ✅ |
| `ItemCreated` | `EventHandler` | `EventCallback` (no type param) | `ItemCreated` | `OnItemCreated` | ✅ |
| `ItemDeleting` | `FormViewDeleteEventHandler` | `EventCallback<FormViewDeleteEventArgs>` | — | `OnItemDeleting` | ✅⚠️ (On only) |
| `ItemDeleted` | `FormViewDeletedEventHandler` | `EventCallback<FormViewDeletedEventArgs>` | — | `OnItemDeleted` | ✅⚠️ (On only) |
| `ItemInserting` | `FormViewInsertEventHandler` | `EventCallback<FormViewInsertEventArgs>` | — | `OnItemInserting` | ✅⚠️ (On only) |
| `ItemInserted` | `FormViewInsertedEventHandler` | `EventCallback<FormViewInsertEventArgs>` | — | `OnItemInserted` | ⚠️ wrong type (Insert not Inserted) |
| `ItemUpdating` | `FormViewUpdateEventHandler` | `EventCallback<FormViewUpdateEventArgs>` | — | `OnItemUpdating` | ✅⚠️ (On only) |
| `ItemUpdated` | `FormViewUpdatedEventHandler` | `EventCallback<FormViewUpdatedEventArgs>` | — | `OnItemUpdated` | ✅⚠️ (On only) |
| `ModeChanging` | `FormViewModeEventHandler` | `EventCallback<FormViewModeEventArgs>` | `ModeChanging` | `OnModeChanging` | ✅ |
| `ModeChanged` | `FormViewModeEventHandler` | `EventCallback<FormViewModeEventArgs>` | `ModeChanged` | `OnModeChanged` | ✅ |
| `PageIndexChanging` | `FormViewPageEventHandler` | `EventCallback<PageChangedEventArgs>` | `PageIndexChanging` | `OnPageIndexChanging` | ✅ |
| `PageIndexChanged` | `EventHandler` | `EventCallback<PageChangedEventArgs>` | `PageIndexChanged` | `OnPageIndexChanged` | ✅ |

**FormView Summary:** 12 of 12 events present. However, **6 CRUD events have On-prefix only** (no bare name). `OnItemInserted` has wrong type (`FormViewInsertEventArgs` instead of `FormViewInsertedEventArgs` — this is a bug).

---

#### DetailsView

| Web Forms Event | WF Delegate Type | BWFC EventCallback Type | Bare Name | On-Prefix | Status |
|----------------|-----------------|------------------------|-----------|-----------|--------|
| `ItemCommand` | `DetailsViewCommandEventHandler` | `EventCallback<DetailsViewCommandEventArgs>` | `ItemCommand` | `OnItemCommand` | ✅ |
| `ItemCreated` | `EventHandler` | — | — | — | ❌ |
| `ItemDeleting` | `DetailsViewDeleteEventHandler` | `EventCallback<DetailsViewDeleteEventArgs>` | `ItemDeleting` | `OnItemDeleting` | ✅ |
| `ItemDeleted` | `DetailsViewDeletedEventHandler` | `EventCallback<DetailsViewDeletedEventArgs>` | `ItemDeleted` | `OnItemDeleted` | ✅ |
| `ItemInserting` | `DetailsViewInsertEventHandler` | `EventCallback<DetailsViewInsertEventArgs>` | `ItemInserting` | `OnItemInserting` | ✅ |
| `ItemInserted` | `DetailsViewInsertedEventHandler` | `EventCallback<DetailsViewInsertedEventArgs>` | `ItemInserted` | `OnItemInserted` | ✅ |
| `ItemUpdating` | `DetailsViewUpdateEventHandler` | `EventCallback<DetailsViewUpdateEventArgs>` | `ItemUpdating` | `OnItemUpdating` | ✅ |
| `ItemUpdated` | `DetailsViewUpdatedEventHandler` | `EventCallback<DetailsViewUpdatedEventArgs>` | `ItemUpdated` | `OnItemUpdated` | ✅ |
| `ModeChanging` | `DetailsViewModeEventHandler` | `EventCallback<DetailsViewModeEventArgs>` | `ModeChanging` | `OnModeChanging` | ✅ |
| `ModeChanged` | `DetailsViewModeEventHandler` | `EventCallback<DetailsViewModeEventArgs>` | `ModeChanged` | `OnModeChanged` | ✅ |
| `PageIndexChanging` | `DetailsViewPageEventHandler` | `EventCallback<PageChangedEventArgs>` | `PageIndexChanging` | `OnPageIndexChanging` | ✅ |
| `PageIndexChanged` | `EventHandler` | `EventCallback<PageChangedEventArgs>` | `PageIndexChanged` | `OnPageIndexChanged` | ✅ |

**DetailsView Summary:** 11 of 12 events present. **1 missing** (ItemCreated). Excellent coverage otherwise.

---

#### Repeater

| Web Forms Event | WF Delegate Type | BWFC EventCallback Type | Status |
|----------------|-----------------|------------------------|--------|
| `ItemCommand` | `RepeaterCommandEventHandler` | — | ❌ |
| `ItemCreated` | `RepeaterItemEventHandler` | — | ❌ |
| `ItemDataBound` | `RepeaterItemEventHandler` | — | ❌ |

**Repeater Summary:** **0 of 3 events. ZERO EventCallbacks. CRITICAL GAP.** The Repeater component is a bare template renderer with no event support whatsoever.

---

#### DataList

| Web Forms Event | WF Delegate Type | BWFC EventCallback Type | Status |
|----------------|-----------------|------------------------|--------|
| `ItemCommand` | `DataListCommandEventHandler` | — | ❌ |
| `ItemCreated` | `DataListItemEventHandler` | — | ❌ |
| `ItemDataBound` | `DataListItemEventHandler` | `EventCallback<DataListItemEventArgs> OnItemDataBound` | ✅⚠️ (On only) |
| `SelectedIndexChanged` | `EventHandler` | — | ❌ |
| `EditCommand` | `DataListCommandEventHandler` | — | ❌ |
| `UpdateCommand` | `DataListCommandEventHandler` | — | ❌ |
| `DeleteCommand` | `DataListCommandEventHandler` | — | ❌ |
| `CancelCommand` | `DataListCommandEventHandler` | — | ❌ |

**DataList Summary:** **1 of 8 events.** Only `OnItemDataBound` exists. All command and selection events are missing.

---

#### DataGrid

| Web Forms Event | WF Delegate Type | BWFC EventCallback Type | Bare Name | On-Prefix | Status |
|----------------|-----------------|------------------------|-----------|-----------|--------|
| `ItemCommand` | `DataGridCommandEventHandler` | `EventCallback<DataGridCommandEventArgs>` | — | `OnItemCommand` | ✅⚠️ (On only) |
| `EditCommand` | `DataGridCommandEventHandler` | `EventCallback<DataGridCommandEventArgs>` | — | `OnEditCommand` | ✅⚠️ (On only) |
| `CancelCommand` | `DataGridCommandEventHandler` | `EventCallback<DataGridCommandEventArgs>` | — | `OnCancelCommand` | ✅⚠️ (On only) |
| `UpdateCommand` | `DataGridCommandEventHandler` | `EventCallback<DataGridCommandEventArgs>` | — | `OnUpdateCommand` | ✅⚠️ (On only) |
| `DeleteCommand` | `DataGridCommandEventHandler` | `EventCallback<DataGridCommandEventArgs>` | — | `OnDeleteCommand` | ✅⚠️ (On only) |
| `ItemCreated` | `DataGridItemEventHandler` | `EventCallback<DataGridItemEventArgs>` | `ItemCreated` | `OnItemCreated` | ✅ |
| `ItemDataBound` | `DataGridItemEventHandler` | `EventCallback<DataGridItemEventArgs>` | `ItemDataBound` | `OnItemDataBound` | ✅ |
| `PageIndexChanged` | `DataGridPageChangedEventHandler` | `EventCallback<DataGridPageChangedEventArgs>` | `PageIndexChanged` | `OnPageIndexChanged` | ✅ |
| `SortCommand` | `DataGridSortCommandEventHandler` | `EventCallback<DataGridSortCommandEventArgs>` | `SortCommand` | `OnSortCommand` | ✅ |
| `SelectedIndexChanged` | `EventHandler` | `EventCallback` (no type param) | `SelectedIndexChanged` | `OnSelectedIndexChanged` | ✅ |

**DataGrid Summary:** 10 of 10 events present. **100% coverage.** However, 5 command events have On-prefix only (no bare name).

---

### 1.2 Input/Form Controls

#### Button / LinkButton / ImageButton (all share ButtonBaseComponent)

| Web Forms Event | WF Delegate | BWFC EventCallback Type | Status | Notes |
|----------------|-------------|------------------------|--------|-------|
| `Click` | `EventHandler` (`EventArgs`) | `EventCallback<MouseEventArgs> OnClick` | ⚠️ | **Wrong type**: WF uses `EventArgs`, BWFC uses `MouseEventArgs` |
| `Command` | `CommandEventHandler` (`CommandEventArgs`) | `EventCallback<CommandEventArgs> OnCommand` | ✅ | Correct |

**Button Summary:** `OnClick` uses `MouseEventArgs` instead of `EventArgs`. This is a known migration friction point — `(object sender, EventArgs e)` signatures won't compile. The `MouseEventArgs` is from `Microsoft.AspNetCore.Components.Web` and contains mouse coordinates not present in Web Forms.

---

#### TextBox

| Web Forms Event | WF Delegate | BWFC EventCallback Type | Status |
|----------------|-------------|------------------------|--------|
| `TextChanged` | `EventHandler` | `EventCallback<string> TextChanged` + `EventCallback<ChangeEventArgs> OnTextChanged` | ✅ |

**TextBox Summary:** Has both a Blazor-idiomatic `TextChanged` (returns `string`) and a migration-friendly `OnTextChanged` (returns `ChangeEventArgs`). Good pattern.

---

#### CheckBox

| Web Forms Event | WF Delegate | BWFC EventCallback Type | Status |
|----------------|-------------|------------------------|--------|
| `CheckedChanged` | `EventHandler` | `EventCallback<bool> CheckedChanged` + `EventCallback<ChangeEventArgs> OnCheckedChanged` | ✅ |

**CheckBox Summary:** Same dual-callback pattern as TextBox. Correct.

---

#### RadioButton

| Web Forms Event | WF Delegate | BWFC EventCallback Type | Status |
|----------------|-------------|------------------------|--------|
| `CheckedChanged` | `EventHandler` | `EventCallback<bool> CheckedChanged` + `EventCallback<ChangeEventArgs> OnCheckedChanged` | ✅ |

**RadioButton Summary:** Same dual-callback pattern. Correct.

---

#### DropDownList

| Web Forms Event | WF Delegate | BWFC EventCallback Type | Status |
|----------------|-------------|------------------------|--------|
| `SelectedIndexChanged` | `EventHandler` | `EventCallback<int> SelectedIndexChanged` + `EventCallback<ChangeEventArgs> OnSelectedIndexChanged` + `EventCallback<string> SelectedValueChanged` | ✅ |

**DropDownList Summary:** Rich callback surface. Correct.

---

#### CheckBoxList

| Web Forms Event | WF Delegate | BWFC EventCallback Type | Status |
|----------------|-------------|------------------------|--------|
| `SelectedIndexChanged` | `EventHandler` | `EventCallback<ChangeEventArgs> OnSelectedIndexChanged` + `EventCallback<List<string>> SelectedValuesChanged` | ✅ |

---

#### RadioButtonList

| Web Forms Event | WF Delegate | BWFC EventCallback Type | Status |
|----------------|-------------|------------------------|--------|
| `SelectedIndexChanged` | `EventHandler` | `EventCallback<int> SelectedIndexChanged` + `EventCallback<ChangeEventArgs> OnSelectedIndexChanged` + `EventCallback<string> SelectedValueChanged` | ✅ |

---

#### ListBox

| Web Forms Event | WF Delegate | BWFC EventCallback Type | Status |
|----------------|-------------|------------------------|--------|
| `SelectedIndexChanged` | `EventHandler` | `EventCallback<int> SelectedIndexChanged` + `EventCallback<ChangeEventArgs> OnSelectedIndexChanged` + `EventCallback<string> SelectedValueChanged` + `EventCallback<List<string>> SelectedValuesChanged` | ✅ |

---

### 1.3 Navigation Controls

#### Menu

| Web Forms Event | WF Delegate | BWFC EventCallback Type | Bare Name | On-Prefix | Status |
|----------------|-------------|------------------------|-----------|-----------|--------|
| `MenuItemClick` | `MenuEventHandler` (`MenuEventArgs`) | `EventCallback<MenuEventArgs>` | `MenuItemClick` | `OnMenuItemClick` | ✅ |
| `MenuItemDataBound` | `MenuEventHandler` (`MenuEventArgs`) | `EventCallback<MenuEventArgs>` | `MenuItemDataBound` | `OnMenuItemDataBound` | ✅ |

**Menu Summary:** 2 of 2 events. Complete.

---

#### TreeView

| Web Forms Event | WF Delegate | BWFC EventCallback Type | Bare Name | On-Prefix | Status |
|----------------|-------------|------------------------|-----------|-----------|--------|
| `SelectedNodeChanged` | `EventHandler` | `EventCallback<TreeNodeEventArgs>` | `SelectedNodeChanged` | `OnSelectedNodeChanged` | ✅ |
| `TreeNodeDataBound` | `TreeNodeEventHandler` | `EventCallback<TreeNodeEventArgs>` | — | `OnTreeNodeDataBound` | ✅⚠️ (On only) |
| `TreeNodeCheckChanged` | `TreeNodeEventHandler` | `EventCallback<TreeNodeEventArgs>` | — | `OnTreeNodeCheckChanged` | ✅⚠️ (On only) |
| `TreeNodeCollapsed` | `TreeNodeEventHandler` | `EventCallback<TreeNodeEventArgs>` | — | `OnTreeNodeCollapsed` | ✅⚠️ (On only) |
| `TreeNodeExpanded` | `TreeNodeEventHandler` | `EventCallback<TreeNodeEventArgs>` | — | `OnTreeNodeExpanded` | ✅⚠️ (On only) |
| `TreeNodePopulate` | `TreeNodeEventHandler` | — | — | — | ❌ |

**TreeView Summary:** 5 of 6 events. 4 have On-prefix only. Missing `TreeNodePopulate` (used for on-demand node population).

---

### 1.1 Summary: Data-Bound Controls Event Coverage

| Control | WF Events | BWFC Present | Missing | Coverage |
|---------|-----------|-------------|---------|----------|
| **GridView** | 15 | 11 | 4 (PageIndexChanging, RowUpdated, RowDeleted, RowCreated, RowDataBound) | 73% |
| **ListView** | 18 | 18 | 0 | **100%** |
| **FormView** | 12 | 12 | 0 (but 1 wrong type) | **100%** |
| **DetailsView** | 12 | 11 | 1 (ItemCreated) | 92% |
| **Repeater** | 3 | 0 | 3 | **0%** |
| **DataList** | 8 | 1 | 7 | **13%** |
| **DataGrid** | 10 | 10 | 0 | **100%** |

---

## Part 2: SelectMethod & Data-Binding Methods

### 2.1 Current SelectMethod Implementation

**Location:** `DataBinding/SelectHandler.cs`, `DataBinding/DataBoundComponent.cs`

**Current delegate signature:**
```csharp
public delegate IQueryable<TItemType> SelectHandler<TItemType>(
    int maxRows, 
    int startRowIndex, 
    string sortByExpression, 
    out int totalRowCount
);
```

**How it works today:**

1. The `SelectMethod` parameter on `DataBoundComponent<T>` is typed as `SelectHandler<T>`
2. It is invoked in `OnAfterRender(firstRender)` — **only once, on first render**
3. Call site: `Items = SelectMethod(int.MaxValue, 0, "", out var totalRowCount);`
4. Returns `IQueryable<T>`, immediately materialized to `List<T>` via `.ToList()` in the `Items` setter

**Problems with current implementation:**

| Issue | Severity | Detail |
|-------|----------|--------|
| **Fires only once** | HIGH | `firstRender` guard means data never refreshes. Sorting/paging don't re-invoke it. |
| **`out` parameter** | HIGH | Cannot be used with lambdas (`out` params aren't supported in lambda expressions). Forces users to write a named method. |
| **Returns IQueryable** | MEDIUM | Most Blazor services return `IEnumerable<T>` or `List<T>`. Forces unnecessary IQueryable wrapping. |
| **Synchronous only** | MEDIUM | No `async` variant. EF Core and most data access is async in modern .NET. |
| **Hard-coded parameters** | LOW | Always passes `int.MaxValue` for maxRows and `0` for startRowIndex. Paging params are ignored. |
| **No re-execution trigger** | HIGH | No mechanism to re-invoke when sort/page/filter parameters change. |

### 2.2 Original Web Forms Model Binding

In Web Forms 4.5+, model binding was introduced as an alternative to `DataSource`/`DataSourceID`:

```aspx
<asp:GridView runat="server" ItemType="Product"
    SelectMethod="GetProducts"
    InsertMethod="InsertProduct"
    UpdateMethod="UpdateProduct"
    DeleteMethod="DeleteProduct" />
```

**How Web Forms handled it:**

| Method | When Invoked | Signature Pattern |
|--------|-------------|-------------------|
| `SelectMethod` | On every data bind (initial + after sort/page/edit/delete) | `IQueryable<T> GetProducts()` or `IQueryable<T> GetProducts(int maxRows, int startRowIndex, out int totalRowCount, string sortByExpression)` |
| `InsertMethod` | When Insert command fires | `void InsertProduct(Product item)` or `void InsertProduct()` (with TryUpdateModel) |
| `UpdateMethod` | When Update command fires | `void UpdateProduct(int id)` (key from DataKeyNames) or `void UpdateProduct(Product item)` |
| `DeleteMethod` | When Delete command fires | `void DeleteProduct(int id)` (key from DataKeyNames) |

Key behaviors:
- **SelectMethod** was re-invoked after every CRUD operation to refresh the display
- **Value providers** resolved method parameters from query string, route data, control values, etc.
- **TryUpdateModel** could populate the model from form values inside the method
- **ModelState** errors surfaced automatically via `ModelErrorMessage`

### 2.3 Current BWFC Status: CRUD Methods

| Method | BWFC Status | Notes |
|--------|-------------|-------|
| `SelectMethod` | ✅ Exists | Broken (fires once, `out` param, sync only) |
| `InsertMethod` | ❌ Not implemented | — |
| `UpdateMethod` | ❌ Not implemented | — |
| `DeleteMethod` | ❌ Not implemented | — |

### 2.4 Proposed Design: SelectMethod Improvements

**Replace the current delegate with two overloads — sync and async:**

```csharp
// Simple signature (no paging params) — lambda-friendly
public delegate IEnumerable<TItemType> SelectHandler<TItemType>();

// Full signature (with paging/sorting) — for server-side paging
public delegate IEnumerable<TItemType> SelectHandlerFull<TItemType>(
    int maxRows, int startRowIndex, string sortExpression, out int totalRowCount);

// Async variant — for EF Core / HTTP services
public delegate Task<IEnumerable<TItemType>> SelectHandlerAsync<TItemType>();
```

**Component parameters:**
```csharp
[Parameter] public SelectHandler<TItemType> SelectMethod { get; set; }
[Parameter] public SelectHandlerFull<TItemType> SelectMethodPaged { get; set; }
[Parameter] public SelectHandlerAsync<TItemType> SelectMethodAsync { get; set; }
```

**Execution changes:**
- Move from `OnAfterRender` to `OnParametersSetAsync`
- Re-invoke after sort, page, edit, delete operations
- Add a public `DataBind()` method for manual refresh

### 2.5 Proposed Design: InsertMethod / UpdateMethod / DeleteMethod

```csharp
// In DataBoundComponent<TItemType>:
[Parameter] public Action<TItemType> InsertMethod { get; set; }
[Parameter] public Func<TItemType, Task> InsertMethodAsync { get; set; }

[Parameter] public Action<TItemType> UpdateMethod { get; set; }
[Parameter] public Func<TItemType, Task> UpdateMethodAsync { get; set; }

[Parameter] public Action<TItemType> DeleteMethod { get; set; }
[Parameter] public Func<TItemType, Task> DeleteMethodAsync { get; set; }
```

**Behavior:**
- When an Insert/Update/Delete command fires, if the corresponding Method parameter is set, invoke it with the current item
- After the CRUD method completes, re-invoke SelectMethod to refresh the display
- If the method throws, set `ExceptionHandled` on the corresponding EventArgs
- The Method parameters and EventCallback parameters are **not mutually exclusive** — events fire first, then the method is called

**Usage example (migration target):**

```razor
<GridView ItemType="Product" Items="@products"
    SelectMethodAsync="@GetProducts"
    UpdateMethodAsync="@UpdateProduct"
    DeleteMethodAsync="@DeleteProduct">
    ...
</GridView>

@code {
    private async Task<IEnumerable<Product>> GetProducts() 
        => await _db.Products.ToListAsync();

    private async Task UpdateProduct(Product p) 
        => await _db.SaveChangesAsync();

    private async Task DeleteProduct(Product p) { 
        _db.Products.Remove(p); 
        await _db.SaveChangesAsync(); 
    }
}
```

---

## Part 3: Event Handler Signature Migration

### 3.1 The Structural Difference

**Web Forms pattern:**
```csharp
protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
{
    var grid = (GridView)sender;  // sender is the control
    e.Cancel = true;              // cancel the operation
}
```

**Blazor BWFC pattern:**
```csharp
private void GridView1_RowDeleting(GridViewDeleteEventArgs e)
{
    // No sender parameter — EventCallback<T> passes only the args
    e.Cancel = true;
}
```

**Key differences:**

| Aspect | Web Forms | Blazor EventCallback |
|--------|-----------|---------------------|
| Signature | `(object sender, TEventArgs e)` | `(TEventArgs e)` — single param |
| Sender access | `sender` parameter (cast to control type) | Must use `e.Sender` if available, or `@ref` |
| Return type | `void` | `void` or `Task` (both work) |
| Access modifier | `protected` | `private` (typical) |
| Registration | Markup attribute `OnRowDeleting="handler"` | Same: `OnRowDeleting="handler"` |

### 3.2 EventArgs Classes with Sender Property

The following BWFC EventArgs classes already have a `Sender` property for sender access:

| EventArgs Class | Sender Property | Used By |
|----------------|----------------|---------|
| `CommandEventArgs` | `object Sender` | Button, LinkButton, ImageButton, FormView, DataGrid |
| `FormViewDeleteEventArgs` | `object Sender` | FormView |
| `FormViewDeletedEventArgs` | `object Sender` | FormView |
| `FormViewInsertEventArgs` | `object Sender` | FormView |
| `FormViewUpdateEventArgs` | `object Sender` | FormView |
| `FormViewUpdatedEventArgs` | `object Sender` | FormView |
| `FormViewModeEventArgs` | `object Sender` | FormView |
| `ListViewItemEventArgs` | `object Sender` | ListView |
| `DataListItemEventArgs` | `object Sender` | DataList |
| `AdCreatedEventArgs` | `object Sender` | AdRotator |
| `AuthenticateEventArgs` | `object Sender` | Login |
| `CreateUserErrorEventArgs` | `object Sender` | CreateUserWizard |
| `LoginCancelEventArgs` | `object Sender` | LoginStatus |
| `MailMessageEventArgs` | `object Sender` | PasswordRecovery |
| `SendMailErrorEventArgs` | `object Sender` | PasswordRecovery |

**EventArgs classes MISSING Sender property (should be added):**

| EventArgs Class | Used By | Priority |
|----------------|---------|----------|
| `GridViewSortEventArgs` | GridView | P1 |
| `GridViewEditEventArgs` | GridView | P1 |
| `GridViewUpdateEventArgs` | GridView | P1 |
| `GridViewDeleteEventArgs` | GridView | P1 |
| `GridViewCancelEditEventArgs` | GridView | P1 |
| `GridViewSelectEventArgs` | GridView | P1 |
| `GridViewCommandEventArgs` | GridView | P1 |
| `PageChangedEventArgs` | GridView, FormView, DetailsView | P1 |
| `DetailsViewCommandEventArgs` | DetailsView | P1 |
| `DetailsViewDeleteEventArgs` | DetailsView | P1 |
| `DetailsViewDeletedEventArgs` | DetailsView | P1 |
| `DetailsViewInsertEventArgs` | DetailsView | P1 |
| `DetailsViewInsertedEventArgs` | DetailsView | P1 |
| `DetailsViewUpdateEventArgs` | DetailsView | P1 |
| `DetailsViewUpdatedEventArgs` | DetailsView | P1 |
| `DetailsViewModeEventArgs` | DetailsView | P1 |
| `ListViewCommandEventArgs` | ListView | P1 |
| `ListViewEditEventArgs` | ListView | P1 |
| `ListViewCancelEventArgs` | ListView | P1 |
| `ListViewDeleteEventArgs` | ListView | P1 |
| `ListViewDeletedEventArgs` | ListView | P1 |
| `ListViewInsertEventArgs` | ListView | P1 |
| `ListViewInsertedEventArgs` | ListView | P1 |
| `ListViewUpdateEventArgs` | ListView | P1 |
| `ListViewUpdatedEventArgs` | ListView | P1 |
| `ListViewSelectEventArgs` | ListView | P1 |
| `ListViewSortEventArgs` | ListView | P1 |
| `ListViewPagePropertiesChangingEventArgs` | ListView | P2 |
| `DataGridCommandEventArgs` | DataGrid | P1 |
| `DataGridItemEventArgs` | DataGrid | P2 |
| `DataGridPageChangedEventArgs` | DataGrid | P1 |
| `DataGridSortCommandEventArgs` | DataGrid | P1 |
| `MenuEventArgs` | Menu | P2 |
| `TreeNodeEventArgs` | TreeView | P2 |

### 3.3 Migration Script Auto-Transformation

The migration script should transform event handler signatures as follows:

**Step 1: Detect Web Forms event handler signatures**
```
Pattern: protected void (\w+)\(object sender, (\w+) e\)
```

**Step 2: Transform to Blazor EventCallback signature**
```csharp
// FROM:
protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)

// TO:
private void GridView1_RowDeleting(GridViewDeleteEventArgs e)
```

**Step 3: Replace sender references**
```csharp
// FROM:
var grid = (GridView)sender;

// TO:
// If the EventArgs has a Sender property:
var grid = (GridView)e.Sender;
// Otherwise, use @ref on the component and reference it directly
```

**Step 4: Handle EventHandler (no custom EventArgs)**
```csharp
// FROM (Web Forms):
protected void Button1_Click(object sender, EventArgs e)

// TO (Blazor):
private void Button1_Click()  // or: private void Button1_Click(EventArgs e)
```

**Implementation note:** The script should emit a `// TODO: sender access removed` comment when it detects `sender` usage in the method body and the EventArgs type doesn't have a `Sender` property.

---

## Part 4: Prioritized Recommendations

### P0 — Must Fix for Migration (Blocking)

| # | Issue | Component | Action | Effort |
|---|-------|-----------|--------|--------|
| P0-1 | **Repeater has ZERO EventCallbacks** | Repeater | Add `ItemCommand`, `ItemCreated`, `ItemDataBound` events with proper EventArgs types | S |
| P0-2 | **DataList missing 7 of 8 events** | DataList | Add `ItemCommand`, `SelectedIndexChanged`, `EditCommand`, `UpdateCommand`, `DeleteCommand`, `CancelCommand`, `ItemCreated` | M |
| P0-3 | **GridView missing RowDataBound** | GridView | Add `RowDataBound`/`OnRowDataBound` with `GridViewRowEventArgs` | S |
| P0-4 | **GridView missing RowCreated** | GridView | Add `RowCreated`/`OnRowCreated` with `GridViewRowEventArgs` | S |
| P0-5 | **DetailsView missing ItemCreated** | DetailsView | Add `ItemCreated`/`OnItemCreated` with `EventArgs` | XS |
| P0-6 | **FormView OnItemInserted wrong type** | FormView | Change from `FormViewInsertEventArgs` to `FormViewInsertedEventArgs` (create new class) | XS |
| P0-7 | **SelectMethod fires only once** | DataBoundComponent | Move to `OnParametersSetAsync`, re-invoke after CRUD operations | M |

### P1 — Should Fix (Migration Quality)

| # | Issue | Component | Action | Effort |
|---|-------|-----------|--------|--------|
| P1-1 | **Button OnClick uses MouseEventArgs** | ButtonBaseComponent | Change to `EventCallback<EventArgs>` (breaking change — needs discussion) | S |
| P1-2 | **Add Sender property to all EventArgs** | All EventArgs without it | Add `public object Sender { get; set; }` to ~30 EventArgs classes | M |
| P1-3 | **GridView missing PageIndexChanging** | GridView | Add `PageIndexChanging`/`OnPageIndexChanging` with cancellation | S |
| P1-4 | **GridView missing RowUpdated/RowDeleted** | GridView | Add post-operation events with AffectedRows/Exception | S |
| P1-5 | **GridView SelectedIndexChanged uses int** | GridView | Should use `EventArgs` for Web Forms compat, keep `int` overload for Blazor | S |
| P1-6 | **FormView CRUD events On-prefix only** | FormView | Add bare-name aliases: `ItemDeleting`, `ItemDeleted`, `ItemInserting`, `ItemInserted`, `ItemUpdating`, `ItemUpdated` | S |
| P1-7 | **SelectMethod `out` param blocks lambdas** | DataBoundComponent | Add lambda-friendly overload without `out` param | S |
| P1-8 | **SelectMethodAsync variant** | DataBoundComponent | Add `Func<Task<IEnumerable<T>>>` parameter | M |
| P1-9 | **TreeView missing TreeNodePopulate** | TreeView | Add event for on-demand node loading | S |
| P1-10 | **TreeView 4 events On-prefix only** | TreeView | Add bare names: `TreeNodeDataBound`, `TreeNodeCheckChanged`, `TreeNodeCollapsed`, `TreeNodeExpanded` | XS |
| P1-11 | **DataGrid 5 events On-prefix only** | DataGrid | Add bare names: `ItemCommand`, `EditCommand`, `CancelCommand`, `UpdateCommand`, `DeleteCommand` | XS |

### P2 — Nice to Have (Polish)

| # | Issue | Component | Action | Effort |
|---|-------|-----------|--------|--------|
| P2-1 | **InsertMethod/UpdateMethod/DeleteMethod** | DataBoundComponent | Implement model-binding CRUD methods as described in §2.5 | L |
| P2-2 | **RowCommand On-prefix only** | GridView | Add bare `RowCommand` alias | XS |
| P2-3 | **ListView ItemDataBound On-prefix only** | ListView | Add bare `ItemDataBound` alias | XS |
| P2-4 | **ListView LayoutCreated On-prefix only** | ListView | Add bare `LayoutCreated` alias | XS |
| P2-5 | **DataList OnItemDataBound On-prefix only** | DataList | Add bare `ItemDataBound` alias | XS |
| P2-6 | **Migration script event handler transform** | bwfc-migrate.ps1 | Add regex transform for `(object sender, TArgs e)` → `(TArgs e)` | M |
| P2-7 | **Public DataBind() method** | DataBoundComponent | Add public `DataBind()` for manual refresh (matches Web Forms API) | XS |

### Effort Legend

| Size | Estimate |
|------|----------|
| XS | < 30 min |
| S | 30 min – 2 hours |
| M | 2 – 4 hours |
| L | 4 – 8 hours |

---

## Appendix A: Complete On-Prefix Consistency Audit

### Controls with BOTH bare name and On-prefix (correct pattern):

- GridView: Sorting/OnSorting, Sorted/OnSorted, RowEditing/OnRowEditing, RowUpdating/OnRowUpdating, RowDeleting/OnRowDeleting, RowCancelingEdit/OnRowCancelingEdit, SelectedIndexChanging/OnSelectedIndexChanging, SelectedIndexChanged/OnSelectedIndexChanged, PageIndexChanged/OnPageIndexChanged
- ListView: ItemCommand/OnItemCommand, ItemEditing/OnItemEditing, ItemCanceling/OnItemCanceling, ItemDeleting/OnItemDeleting, ItemDeleted/OnItemDeleted, ItemInserting/OnItemInserting, ItemInserted/OnItemInserted, ItemUpdating/OnItemUpdating, ItemUpdated/OnItemUpdated, ItemCreated/OnItemCreated, Sorting/OnSorting, Sorted/OnSorted, PagePropertiesChanging/OnPagePropertiesChanging, PagePropertiesChanged/OnPagePropertiesChanged, SelectedIndexChanging/OnSelectedIndexChanging, SelectedIndexChanged/OnSelectedIndexChanged
- DetailsView: ItemCommand/OnItemCommand, ItemDeleting/OnItemDeleting, ItemDeleted/OnItemDeleted, ItemInserting/OnItemInserting, ItemInserted/OnItemInserted, ItemUpdating/OnItemUpdating, ItemUpdated/OnItemUpdated, ModeChanging/OnModeChanging, ModeChanged/OnModeChanged, PageIndexChanging/OnPageIndexChanging, PageIndexChanged/OnPageIndexChanged
- FormView: ItemCommand/OnItemCommand, ModeChanging/OnModeChanging, ModeChanged/OnModeChanged, PageIndexChanging/OnPageIndexChanging, PageIndexChanged/OnPageIndexChanged, ItemCreated/OnItemCreated
- DataGrid: ItemCreated/OnItemCreated, ItemDataBound/OnItemDataBound, PageIndexChanged/OnPageIndexChanged, SortCommand/OnSortCommand, SelectedIndexChanged/OnSelectedIndexChanged
- Menu: MenuItemClick/OnMenuItemClick, MenuItemDataBound/OnMenuItemDataBound
- TreeView: SelectedNodeChanged/OnSelectedNodeChanged

### Controls with On-prefix ONLY (need bare-name alias):

- GridView: `OnRowCommand` (no `RowCommand`)
- FormView: `OnItemDeleting`, `OnItemDeleted`, `OnItemInserting`, `OnItemInserted`, `OnItemUpdating`, `OnItemUpdated` (6 events)
- DataGrid: `OnItemCommand`, `OnEditCommand`, `OnCancelCommand`, `OnUpdateCommand`, `OnDeleteCommand` (5 events)
- ListView: `OnItemDataBound`, `OnLayoutCreated` (2 events)
- DataList: `OnItemDataBound` (1 event)
- TreeView: `OnTreeNodeDataBound`, `OnTreeNodeCheckChanged`, `OnTreeNodeCollapsed`, `OnTreeNodeExpanded` (4 events)

**Total On-prefix-only events needing bare aliases:** 19

---

## Appendix B: EventArgs Type Inventory

All custom EventArgs classes in BWFC with their inheritance and key properties:

| Class | Base | Key Properties | Has Sender | Has Cancel |
|-------|------|---------------|------------|------------|
| `CommandEventArgs` | `EventArgs` | CommandName, CommandArgument | ✅ | — |
| `GridViewSortEventArgs` | `EventArgs` | SortExpression, SortDirection | ❌ | ✅ |
| `GridViewEditEventArgs` | `EventArgs` | NewEditIndex | ❌ | ✅ |
| `GridViewUpdateEventArgs` | `EventArgs` | RowIndex | ❌ | ✅ |
| `GridViewDeleteEventArgs` | `EventArgs` | RowIndex | ❌ | ✅ |
| `GridViewCancelEditEventArgs` | `EventArgs` | RowIndex | ❌ | ✅ |
| `GridViewSelectEventArgs` | `EventArgs` | NewSelectedIndex | ❌ | ✅ |
| `GridViewCommandEventArgs` | `CommandEventArgs` | — | ✅ (inherited) | — |
| `PageChangedEventArgs` | `EventArgs` | NewPageIndex, OldPageIndex, TotalPages, StartRowIndex | ❌ | ✅ |
| `ListViewCommandEventArgs` | `EventArgs` | CommandName, CommandArgument | ❌ | — |
| `ListViewEditEventArgs` | `EventArgs` | NewEditIndex | ❌ | ✅ |
| `ListViewCancelEventArgs` | `EventArgs` | ItemIndex, CancelMode | ❌ | ✅ |
| `ListViewDeleteEventArgs` | `EventArgs` | ItemIndex | ❌ | ✅ |
| `ListViewDeletedEventArgs` | `EventArgs` | AffectedRows, Exception | ❌ | — |
| `ListViewInsertEventArgs` | `EventArgs` | — | ❌ | ✅ |
| `ListViewInsertedEventArgs` | `EventArgs` | AffectedRows, Exception | ❌ | — |
| `ListViewUpdateEventArgs` | `EventArgs` | ItemIndex | ❌ | ✅ |
| `ListViewUpdatedEventArgs` | `EventArgs` | AffectedRows, Exception, KeepInEditMode | ❌ | — |
| `ListViewSelectEventArgs` | `EventArgs` | NewSelectedIndex | ❌ | ✅ |
| `ListViewSortEventArgs` | `EventArgs` | SortExpression, SortDirection | ❌ | ✅ |
| `ListViewItemEventArgs` | `EventArgs` | Item | ✅ | — |
| `ListViewPagePropertiesChangingEventArgs` | `EventArgs` | StartRowIndex, MaximumRows | ❌ | — |
| `FormViewCommandEventArgs` | `CommandEventArgs` | — | ✅ (inherited) | — |
| `FormViewDeleteEventArgs` | `EventArgs` | RowIndex | ✅ | ✅ |
| `FormViewDeletedEventArgs` | `EventArgs` | AffectedRows, Exception | ✅ | — |
| `FormViewInsertEventArgs` | `EventArgs` | CommandArgument | ✅ | ✅ |
| `FormViewUpdateEventArgs` | `EventArgs` | CommandArgument | ✅ | ✅ |
| `FormViewUpdatedEventArgs` | `EventArgs` | AffectedRows, Exception | ✅ | — |
| `FormViewModeEventArgs` | `EventArgs` | NewMode | ✅ | ✅ |
| `DetailsViewCommandEventArgs` | `CommandEventArgs` | CommandSource, Handled | ✅ (inherited) | — |
| `DetailsViewDeleteEventArgs` | `EventArgs` | RowIndex | ❌ | ✅ |
| `DetailsViewDeletedEventArgs` | `EventArgs` | AffectedRows, Exception | ❌ | — |
| `DetailsViewInsertEventArgs` | `EventArgs` | CommandArgument | ❌ | ✅ |
| `DetailsViewInsertedEventArgs` | `EventArgs` | AffectedRows, Exception | ❌ | — |
| `DetailsViewUpdateEventArgs` | `EventArgs` | CommandArgument | ❌ | ✅ |
| `DetailsViewUpdatedEventArgs` | `EventArgs` | AffectedRows, Exception | ❌ | — |
| `DetailsViewModeEventArgs` | `EventArgs` | NewMode, CancelingEdit | ❌ | ✅ |
| `DataGridCommandEventArgs` | `CommandEventArgs` | — | ✅ (inherited) | — |
| `DataGridItemEventArgs` | `EventArgs` | Item | ❌ | — |
| `DataGridPageChangedEventArgs` | `EventArgs` | NewPageIndex | ❌ | — |
| `DataGridSortCommandEventArgs` | `EventArgs` | SortExpression, CommandSource | ❌ | — |
| `DataListItemEventArgs` | `EventArgs` | Item | ✅ | — |
| `MenuEventArgs` | `EventArgs` | Item | ❌ | — |
| `TreeNodeEventArgs` | `EventArgs` | Node | ❌ | — |
| `AdCreatedEventArgs` | `EventArgs` | AdProperties | ✅ | — |
| `BulletedListEventArgs` | `EventArgs` | Index | ❌ | — |
| `ImageMapEventArgs` | `EventArgs` | PostBackValue | ❌ | — |
| `AuthenticateEventArgs` | `EventArgs` | Authenticated | ✅ | — |
| `CreateUserErrorEventArgs` | `EventArgs` | — | ✅ | — |
| `LoginCancelEventArgs` | `EventArgs` | Cancel | ✅ | ✅ |
| `MailMessageEventArgs` | `EventArgs` | — | ✅ | — |
| `SendMailErrorEventArgs` | `EventArgs` | Exception, Handled | ✅ | — |

**Summary:** 15 of 52 EventArgs classes have `Sender`. 37 need it added.

---

*End of audit. — Forge*
