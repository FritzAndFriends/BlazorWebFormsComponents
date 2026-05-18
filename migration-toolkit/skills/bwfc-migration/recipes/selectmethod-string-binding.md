# Recipe: SelectMethod String Binding

## Error Signature

```
CS1503: Argument 'SelectMethod': cannot convert from 'string' to 'SelectHandler<T>'
```

Or: a data control has `SelectMethod="MethodName"` but the data never loads.

## When This Applies

BWFC data controls (`ListView`, `GridView`, `FormView`, `Repeater`, `DataList`, `DetailsView`) accept `SelectMethod` as a **string** — the method name on the hosting `WebFormsPageBase`. The control resolves it via reflection at runtime.

## Prerequisites

1. The page must inherit from `WebFormsPageBase` (which cascades itself automatically)
2. The method must be `public` on the page class
3. The method name must match exactly (case-sensitive)

## Usage — Same as Web Forms

```razor
@inherits WebFormsPageBase

<ListView ItemType="Product" SelectMethod="GetProducts">
    <ItemTemplate Context="item">
        <span>@item.ProductName — @item.UnitPrice.ToString("C")</span>
    </ItemTemplate>
</ListView>

@code {
    [Inject] IDbContextFactory<ProductContext> DbFactory { get; set; }

    public List<Product> GetProducts()
    {
        using var db = DbFactory.CreateDbContext();
        return db.Products.ToList();
    }
}
```

## Supported Method Signatures

| Signature | Use Case |
|-----------|----------|
| `public List<T> Method()` | Simple list loading |
| `public IQueryable<T> Method()` | Queryable (materialized internally) |
| `public IEnumerable<T> Method()` | Enumerable |
| `public T[] Method()` | Array |
| `public Task<List<T>> Method()` | Async loading |
| `public IQueryable<T> Method(int maxRows, int startRowIndex, string sortBy, out int totalRowCount)` | Paginated GridView |

## InsertMethod, UpdateMethod, DeleteMethod

These follow the same string pattern:

```razor
<FormView ItemType="Product"
          SelectMethod="GetProduct"
          InsertMethod="InsertProduct"
          UpdateMethod="UpdateProduct"
          DeleteMethod="DeleteProduct">
```

Action methods accept a single parameter of the item type:
```csharp
public void InsertProduct(Product item) { /* ... */ }
public void UpdateProduct(Product item) { /* ... */ }
public void DeleteProduct(Product item) { /* ... */ }
```

## When to Use String SelectMethod vs Items Binding

| Scenario | Use |
|----------|-----|
| Page inherits `WebFormsPageBase` and has the method | `SelectMethod="MethodName"` — zero changes needed |
| Component/partial without page context | `Items="@data"` with explicit data loading |
| Test code or standalone components | `Items="@list"` or `SelectMethodDelegate` |

## Error: "No WebFormsPageBase cascade found"

If you see this error, the data control couldn't find a hosting `WebFormsPageBase`. Ensure:
1. The page class inherits `WebFormsPageBase`, not `ComponentBase`
2. The `@inherits WebFormsPageBase` directive is in the `.razor` file
3. The control is inside the page's render tree (not in a disconnected component)

## Verification

After wiring `SelectMethod`, the data control should load data on page render. Check that:
- The method is `public` (not `private` or `protected`)
- The return type matches or is convertible to `IEnumerable<T>`
- `ItemType` is set on the data control when using string SelectMethod (Razor can't infer the generic type from a string)
