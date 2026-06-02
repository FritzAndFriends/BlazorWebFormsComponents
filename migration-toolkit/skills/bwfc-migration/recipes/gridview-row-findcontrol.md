# Recipe: GridView Row Traversal & FindControl

## Error Signature

```
CS1061: 'GridView<T>' does not contain a definition for 'Rows'
CS1061: 'GridViewRow' does not contain a definition for 'FindControl'
CS0246: The type or namespace name 'DataControlFieldCell' could not be found
```

## Detection

```powershell
Select-String -Path **/*.cs -Pattern "\.Rows\[|\.FindControl\(|DataControlFieldCell"
```

## Root Cause

Web Forms `GridView` exposes a `Rows` collection of `GridViewRow` objects. Code iterates rows and calls `FindControl("ControlName")` to locate controls like `TextBox` and `CheckBox` inside `TemplateField` columns. This pattern is common in edit/update scenarios (shopping carts, bulk editors, admin grids).

BWFC provides `GridViewRow` and `FindControl()` shims that return proxy objects populated from form data. The code-behind compiles and runs — but the shim relies on the controls having matching `id` attributes in the rendered markup.

## How the BWFC Shim Works

`GridViewRow.FindControl(id)` returns a proxy component (e.g., `TextBox`, `CheckBox`) whose properties (`Text`, `Checked`) are populated from `Request.Form` data keyed by the control's HTML `id`. This means:

1. The `TemplateField` markup **must** render controls with matching `id` attributes
2. The page **must** use `<WebFormsForm>` for interactive form submission (so `Request.Form` is populated)

## Fix Pattern

**The code-behind usually compiles as-is** if the emission planner emits it. The key fixes are in markup:

```razor
@* Ensure GridView TemplateFields have controls with id attributes *@
<TemplateField HeaderText="Quantity">
    <ItemTemplate Context="Item">
        <TextBox id="PurchaseQuantity" Width="40" Text="@Item.Quantity" />
    </ItemTemplate>
</TemplateField>

<TemplateField HeaderText="Remove">
    <ItemTemplate Context="Item">
        <CheckBox id="Remove" />
    </ItemTemplate>
</TemplateField>
```

**Wrap in interactive form if the page does row updates:**
```razor
<WebFormsForm OnSubmit="SetRequestFormData">
    <GridView Items="@CartItems" ItemType="CartItem">
        @* ... columns with controls ... *@
    </GridView>
    <Button Text="Update" OnClick="UpdateBtn_Click" />
</WebFormsForm>
```

## When the Shim Is Not Enough

If the code-behind uses `DataControlFieldCell.ContainingField.ExtractValuesFromCell()` or other deep grid internals, the shim won't cover it. In that case, replace the row-traversal logic with direct collection manipulation:

```csharp
// BEFORE (Web Forms row traversal):
for (int i = 0; i < CartList.Rows.Count; i++)
{
    var qty = (TextBox)CartList.Rows[i].FindControl("PurchaseQuantity");
    var remove = (CheckBox)CartList.Rows[i].FindControl("Remove");
    // ... update database ...
}

// AFTER (direct collection approach if shim is insufficient):
// Bind Items to a list, use form data or event callbacks per-row
```

## Verification

After ensuring the code-behind is emitted (not artifact-only), `GridViewRow` and `FindControl` references should compile against BWFC types. Verify with `dotnet build`.
