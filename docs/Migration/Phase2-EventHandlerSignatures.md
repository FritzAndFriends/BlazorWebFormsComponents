# Event Handler Signature Migration

The migration script automatically transforms Web Forms event handler signatures to Blazor-compatible equivalents. The `(object sender, EventArgs e)` pattern is stripped or simplified based on the `EventArgs` type, preserving your handler logic unchanged.

## Overview

**What it does:**
- Strips `(object sender, EventArgs e)` parameters from standard event handlers
- Strips only the `sender` parameter when specialized `EventArgs` are used (e.g., `GridViewCommandEventArgs`)
- Preserves all method body logic unchanged

**Why it matters:**
Every Web Forms event handler follows the `(object sender, EventArgs e)` convention. Blazor event callbacks use different signatures — parameterless for simple actions, or with a single event args parameter for specialized events. The migration script automates this transformation for the common cases, but you should understand the rules for manual review.

## The Rules

The migration follows two simple rules based on the `EventArgs` type:

### Rule 1: Standard EventArgs → Strip Both Parameters

When the handler uses the base `EventArgs` class (or no args), **remove both parameters** — the handler becomes parameterless:

```csharp
// Web Forms
protected void Button_Click(object sender, EventArgs e)
{
    SaveData();
}

// Blazor (migrated)
protected void Button_Click()
{
    SaveData();
}
```

### Rule 2: Specialized EventArgs → Strip Sender Only

When the handler uses a **specialized** `EventArgs` subclass, **remove only `sender`** — keep the event args parameter:

```csharp
// Web Forms
protected void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
{
    if (e.CommandName == "Delete")
    {
        int rowIndex = Convert.ToInt32(e.CommandArgument);
        DeleteRow(rowIndex);
    }
}

// Blazor (migrated)
protected void Grid_RowCommand(GridViewCommandEventArgs e)
{
    if (e.CommandName == "Delete")
    {
        int rowIndex = Convert.ToInt32(e.CommandArgument);
        DeleteRow(rowIndex);
    }
}
```

## Common Transformations

The following table shows before/after signatures for the most common Web Forms event handlers:

| Control | Web Forms Signature | Blazor Signature | Rule |
|---------|-------------------|-----------------|------|
| Button Click | `Button_Click(object sender, EventArgs e)` | `Button_Click()` | 1 — Standard |
| LinkButton Click | `Link_Click(object sender, EventArgs e)` | `Link_Click()` | 1 — Standard |
| ImageButton Click | `Image_Click(object sender, ImageClickEventArgs e)` | `Image_Click(ImageClickEventArgs e)` | 2 — Specialized |
| DropDownList Changed | `DropDown_Changed(object sender, EventArgs e)` | `DropDown_Changed()` | 1 — Standard |
| CheckBox Changed | `Check_Changed(object sender, EventArgs e)` | `Check_Changed()` | 1 — Standard |
| GridView RowCommand | `Grid_RowCommand(object sender, GridViewCommandEventArgs e)` | `Grid_RowCommand(GridViewCommandEventArgs e)` | 2 — Specialized |
| GridView RowEditing | `Grid_RowEditing(object sender, GridViewEditEventArgs e)` | `Grid_RowEditing(GridViewEditEventArgs e)` | 2 — Specialized |
| GridView RowDeleting | `Grid_RowDeleting(object sender, GridViewDeleteEventArgs e)` | `Grid_RowDeleting(GridViewDeleteEventArgs e)` | 2 — Specialized |
| GridView PageIndexChanging | `Grid_PageChanging(object sender, GridViewPageEventArgs e)` | `Grid_PageChanging(GridViewPageEventArgs e)` | 2 — Specialized |
| GridView RowDataBound | `Grid_RowDataBound(object sender, GridViewRowEventArgs e)` | `Grid_RowDataBound(GridViewRowEventArgs e)` | 2 — Specialized |
| Repeater ItemCommand | `Repeater_ItemCommand(object sender, RepeaterCommandEventArgs e)` | `Repeater_ItemCommand(RepeaterCommandEventArgs e)` | 2 — Specialized |
| ListView ItemCommand | `List_ItemCommand(object sender, ListViewCommandEventArgs e)` | `List_ItemCommand(ListViewCommandEventArgs e)` | 2 — Specialized |
| TextBox TextChanged | `TextBox_Changed(object sender, EventArgs e)` | `TextBox_Changed()` | 1 — Standard |
| Calendar SelectionChanged | `Calendar_Changed(object sender, EventArgs e)` | `Calendar_Changed()` | 1 — Standard |
| Timer Tick | `Timer_Tick(object sender, EventArgs e)` | `Timer_Tick()` | 1 — Standard |

## Before and After: Full Page Example

=== "Web Forms (Original)"
    ```csharp
    // ProductAdmin.aspx.cs
    public partial class ProductAdmin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        protected void AddButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddProduct.aspx");
        }

        protected void ProductGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"EditProduct.aspx?id={index}");
            }
        }

        protected void ProductGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int productId = (int)ProductGrid.DataKeys[e.RowIndex].Value;
            DeleteProduct(productId);
            BindGrid();
        }

        protected void SearchBox_TextChanged(object sender, EventArgs e)
        {
            string query = SearchBox.Text;
            ProductGrid.DataSource = SearchProducts(query);
            ProductGrid.DataBind();
        }
    }
    ```

=== "Blazor (After Migration)"
    ```csharp
    // ProductAdmin.razor.cs
    public partial class ProductAdmin : ComponentBase
    {
        protected override async Task OnInitializedAsync()
        {
            // Was Page_Load — lifecycle also transformed
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        protected void AddButton_Click()
        {
            // Rule 1: Standard EventArgs → parameterless
            Response.Redirect("AddProduct.aspx");
        }

        protected void ProductGrid_RowCommand(GridViewCommandEventArgs e)
        {
            // Rule 2: Specialized EventArgs → sender removed, e kept
            if (e.CommandName == "Edit")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"EditProduct.aspx?id={index}");
            }
        }

        protected void ProductGrid_RowDeleting(GridViewDeleteEventArgs e)
        {
            // Rule 2: Specialized EventArgs → sender removed, e kept
            int productId = (int)ProductGrid.DataKeys[e.RowIndex].Value;
            DeleteProduct(productId);
            BindGrid();
        }

        protected void SearchBox_TextChanged()
        {
            // Rule 1: Standard EventArgs → parameterless
            string query = SearchBox.Text;
            ProductGrid.DataSource = SearchProducts(query);
            ProductGrid.DataBind();
        }
    }
    ```

## Automated Transformation

The migration script (`bwfc-migrate.ps1`) handles these transformations automatically:

### What the Script Does

1. **Detects EventArgs type** — Inspects the second parameter's type to determine which rule to apply
2. **Applies Rule 1 or Rule 2** — Strips parameters according to the rules above
3. **Preserves method body** — All logic inside the handler is left unchanged
4. **Updates method visibility** — Keeps `protected void` as-is (no override needed for event handlers)

### Recognized Specialized EventArgs

The script recognizes these as specialized `EventArgs` that trigger Rule 2 (keep the parameter):

- `GridViewCommandEventArgs`, `GridViewEditEventArgs`, `GridViewDeleteEventArgs`, `GridViewUpdateEventArgs`, `GridViewPageEventArgs`, `GridViewRowEventArgs`, `GridViewSortEventArgs`, `GridViewSelectEventArgs`
- `RepeaterCommandEventArgs`, `RepeaterItemEventArgs`
- `ListViewCommandEventArgs`, `ListViewEditEventArgs`, `ListViewDeleteEventArgs`, `ListViewInsertEventArgs`, `ListViewUpdateEventArgs`
- `FormViewInsertEventArgs`, `FormViewUpdateEventArgs`, `FormViewDeleteEventArgs`
- `DataListCommandEventArgs`, `DataListItemEventArgs`
- `ImageClickEventArgs`
- `CommandEventArgs`
- Any type name ending in `EventArgs` that is **not** the base `System.EventArgs`

## Manual Review Checklist

After the automated migration, review the following:

### 1. Check for `sender` Usage in Handler Body

If the handler body references `sender`, the automated transform removes the parameter but leaves the reference:

```csharp
// Web Forms — uses sender to identify which button was clicked
protected void Button_Click(object sender, EventArgs e)
{
    var btn = (Button)sender;
    StatusLabel.Text = $"You clicked {btn.Text}";
}

// After migration — sender reference breaks
protected void Button_Click()
{
    var btn = (Button)sender;  // ❌ Compile error
    StatusLabel.Text = $"You clicked {btn.Text}";
}
```

**Fix:** Replace `sender` with direct control references or component state:

```csharp
// Option 1: Pass identifying data via CommandArgument
protected void Button_Click()
{
    StatusLabel.Text = "Button clicked";
}

// Option 2: Use separate handlers per button
protected void SaveButton_Click()
{
    StatusLabel.Text = "You clicked Save";
}
```

### 2. Check for Dynamic Event Wiring

Web Forms allows dynamically wiring events in code:

```csharp
// Web Forms — dynamic wiring
protected void Page_Init(object sender, EventArgs e)
{
    var btn = new Button();
    btn.Click += DynamicButton_Click;
    PlaceHolder1.Controls.Add(btn);
}
```

This pattern doesn't have a direct Blazor equivalent. Consider using `RenderFragment` or conditional rendering instead.

### 3. Custom EventArgs Subclasses

If your application defines custom `EventArgs` subclasses, verify the migration script recognized them:

```csharp
// Custom EventArgs — should trigger Rule 2
public class ProductEventArgs : EventArgs
{
    public int ProductId { get; set; }
}

protected void Product_Selected(object sender, ProductEventArgs e)
{
    LoadProduct(e.ProductId);
}

// Should migrate to (Rule 2):
protected void Product_Selected(ProductEventArgs e)
{
    LoadProduct(e.ProductId);
}
```

## Summary

- ✅ Standard `EventArgs` → strip both `sender` and `e` (parameterless handler)
- ✅ Specialized `EventArgs` → strip `sender` only, keep `e`
- ✅ Automated by the migration script
- ⚠️ Review handler bodies that reference `sender` — may need refactoring
- ⚠️ Dynamic event wiring needs manual conversion to Blazor patterns
- ⚠️ Custom `EventArgs` subclasses should be auto-detected but verify

See [Automated Migration Guide](AutomatedMigration.md) for the full list of automated transformations performed by the migration script.
