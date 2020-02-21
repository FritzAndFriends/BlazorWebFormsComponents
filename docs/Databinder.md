# DataBinder

In Web Forms applications, there is a somewhat standard approach of formatting and placing data in controls by using the DataBinder object.  The DataBinder would be used in ItemTemplate, AlternatingItemTemplate, and other control templates to indicate where data would be formatted and placed.  [Microsoft's original documentation about the DataBinder](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.databinder?view=netframework-4.8) are available.

## ASP<span></span>.NET Syntax, Support and Migration

There are several common techniques that the DataBinder was used and various levels of support are provided:

| Web Forms Syntax | Description | Blazor Support |
| --- | --- | --- |
| `DataBinder.Eval(Container.DataItem, "PropertyName")` | Output the `PropertyName` of the current item | *Fully Supported for Container.DataItem* |
| `Eval("PropertyName")` | Output the `PropertyName` of the current item | *Fully Supported when using static* |
| `DataBinder.Eval(Container.DataItem, "PropertyName", "FormatString")` | Output the formatted value of `PropertyName` of the current item with the `FormatString` | *Fully Supported for Container.DataItem* |
| `Eval("PropertyName", "FormatString")` | Output the formatted value of `PropertyName` of the current item with the `FormatString` | *Fully Supported when using static* |
| `DataBinder.GetDataItem` | Output the item currently operated on | Not supported: Replace with calls to `@context` |
| `DataBinder.GetPropertyValue(Container.DataItem, "PropertyName")` | Get the property requested as an object for further handling | Only supported when passing in `@context` for the first argument. **Recommendation**: replace with `@context.PropertyName` to directly access the property in a strongly-typed manner |

[Back to top](#DataBinder)

## Support and Migration

The DataBinder is not recommended by Microsoft in Web Forms for use in high-performance applications due to the amount of reflection used to output and format content.  Similarly, we do not recommend long-term use of the DataBinder and have marked it with an `Obsolete` flag indicating that there are methods to easily migrate syntax to be more Razor-performance-friendly.

[Back to top](#DataBinder)

### Usage

To migrate your Web Forms control that is referencing the DataBinder to Blazor, start with syntax similar to the following:

```html
<ItemTemplate>
  <li><%#: DataBinder.Eval(Container.DataItem, "Price", "{0:C}") %></li>
</ItemTemplate>
```

All of the databound components in this library support DataBinder syntax and can easily be converted by replacing the angle brackets with razor notation like the following:

```html
<ItemTemplate>
  <li>@DataBinder.Eval(Container.DataItem, "Price", "{0:C}")</li>
</ItemTemplate>
```

That's a VERY simple conversion and its clear how we can continue to deliver the same feature and formatting using Blazor.  You can even shorten the syntax to use the simple `Eval` keyword and get the same effect.  Just include a `@using static` keyword near the top of your Blazor page with this syntax and you can using the shortened format of `Eval`.

```html
@using static BlazorWebFormsComponents.DataBinder
...
<ItemTemplate>
  <li>@Eval("Price", "{0:C}")</li>
</ItemTemplate>
```

*Note:* Your Blazor application will emit compiler warnings while you continue to use the DataBinder.

[Back to top](#DataBinder)

### Moving On

Moving on from the DataBinder to a more performant and simple Razor syntax is quite easy using an `ItemContext` and referencing the iterated item directly.  This approach has the additional benefit of providing type-safety and compiler checking on the content of your Blazor pages.

Our `Eval("Price")` statement above is further simplified into:

```html
<Repeater Context="Item">
...
    <ItemTemplate>
      <li>@Item.Price.ToString("C")</li>
    </ItemTemplate>
...
</Repeater>
```

[Back to top](#DataBinder)
