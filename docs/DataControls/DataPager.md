# DataPager

The DataPager component provides paging functionality for data-bound controls. It displays navigation UI (page numbers, Previous/Next buttons, First/Last buttons) to navigate through large data sets.

Original Microsoft implementation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.datapager?view=netframework-4.8

## Features Supported in Blazor

- **TotalRowCount** - Total number of items in the data source
- **PageSize** - Number of items per page (default: 10)
- **PageIndex** - Current page (zero-based)
- **PageButtonCount** - Number of numeric page buttons to show (default: 5)
- **Mode** - PagerButtons enum (Numeric, NextPrevious, NextPreviousFirstLast, NumericFirstLast)
- **Custom button text** - FirstPageText, PreviousPageText, NextPageText, LastPageText
- **Events** - OnPageIndexChanging (cancellable), OnPageIndexChanged
- **StartRowIndex** / **MaximumRows** - Properties for data slicing

### Blazor Notes

Unlike Web Forms, which links DataPager to ListView via `PagedControlID`, the Blazor implementation uses two-way binding. You manage the page index in your component and pass it to both the DataPager and your data query.

The DataPager provides `StartRowIndex` and `MaximumRows` properties that you can use to slice your data:

```csharp
var pagedData = allData.Skip(dataPager.StartRowIndex).Take(dataPager.MaximumRows);
```

## Web Forms Features NOT Supported

- **PagedControlID** - Not supported; use two-way binding instead
- **Fields collection** - Not supported; use Mode property for common layouts
- **QueryStringField** - Not supported; implement URL-based paging separately

## Web Forms Declarative Syntax

```html
<asp:DataPager
    ID="DataPager1"
    PagedControlID="ListView1"
    PageSize="10"
    QueryStringField="page"
    runat="server">
    <Fields>
        <asp:NextPreviousPagerField
            ButtonType="Link"
            FirstPageText="First"
            LastPageText="Last"
            NextPageText="Next"
            PreviousPageText="Previous"
            ShowFirstPageButton="true"
            ShowLastPageButton="true"
            ShowNextPageButton="true"
            ShowPreviousPageButton="true" />
        <asp:NumericPagerField
            ButtonCount="5"
            ButtonType="Link" />
    </Fields>
</asp:DataPager>
```

## Blazor Syntax

```razor
<DataPager
    TotalRowCount="@TotalItems"
    PageSize="10"
    @bind-PageIndex="CurrentPage"
    Mode="PagerButtons.NumericFirstLast"
    FirstPageText="First"
    LastPageText="Last"
    PageButtonCount="5" />
```

## Usage Notes

1. **Manage state externally** - The DataPager doesn't fetch data; you handle paging logic
2. **Use @bind-PageIndex** - For two-way binding of the current page
3. **Subscribe to events** - Use OnPageIndexChanging to cancel navigation or OnPageIndexChanged for side effects
4. **Slice your data** - Use `Skip()` and `Take()` based on StartRowIndex and MaximumRows

## Examples

### Basic Usage with ListView

```razor
@* Data paging with ListView *@
<ListView TItem="Product" DataSource="@PagedProducts">
    <ItemTemplate Context="product">
        <div>@product.Name - @product.Price.ToString("C")</div>
    </ItemTemplate>
</ListView>

<DataPager 
    TotalRowCount="@AllProducts.Count" 
    PageSize="10" 
    @bind-PageIndex="CurrentPage" />

@code {
    private List<Product> AllProducts = GetAllProducts();
    private int CurrentPage = 0;

    private IEnumerable<Product> PagedProducts => 
        AllProducts.Skip(CurrentPage * 10).Take(10);
}
```

### Next/Previous Navigation

```razor
@* Simple Previous/Next buttons *@
<DataPager 
    TotalRowCount="100" 
    PageSize="10" 
    @bind-PageIndex="PageIndex"
    Mode="PagerButtons.NextPrevious" />
```

### Full Navigation with Numbers

```razor
@* All navigation options *@
<DataPager 
    TotalRowCount="@Data.Count" 
    PageSize="20" 
    @bind-PageIndex="PageIndex"
    Mode="PagerButtons.NumericFirstLast"
    PageButtonCount="7"
    FirstPageText="<<"
    PreviousPageText="<"
    NextPageText=">"
    LastPageText=">>" />
```

### Handling Page Changes

```razor
<DataPager 
    TotalRowCount="@TotalCount" 
    PageSize="10" 
    @bind-PageIndex="PageIndex"
    OnPageIndexChanging="HandlePageChanging"
    OnPageIndexChanged="HandlePageChanged" />

@code {
    private async Task HandlePageChanging(PageChangedEventArgs args)
    {
        // Cancel if there are unsaved changes
        if (HasUnsavedChanges)
        {
            args.Cancel = true;
            ShowWarning("Please save your changes first.");
        }
    }

    private async Task HandlePageChanged(PageChangedEventArgs args)
    {
        // Log or track page navigation
        await LogPageView(args.NewPageIndex);
    }
}
```

### Custom Styling

```razor
<DataPager 
    TotalRowCount="100" 
    PageSize="10" 
    @bind-PageIndex="PageIndex"
    CssClass="pagination"
    Mode="PagerButtons.Numeric" />

<style>
    .pagination a { 
        padding: 5px 10px; 
        margin: 0 2px; 
        border: 1px solid #ccc; 
    }
    .pagination .aspNetCurrentPage { 
        font-weight: bold; 
        background: #007bff; 
        color: white; 
        padding: 5px 10px; 
    }
    .pagination .aspNetDisabled { 
        color: #ccc; 
        pointer-events: none; 
    }
</style>
```

## See Also

- [ListView](../DataControls/ListView.md)
- [GridView](../DataControls/GridView.md)
- [Repeater](../DataControls/Repeater.md)
