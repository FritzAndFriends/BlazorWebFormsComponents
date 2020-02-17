# DataBinder 

In Web Forms applications, there is a somewhat standard approach of formatting and placing data in controls by using the DataBinder object.  The DataBinder would be used in ItemTemplate, AlternatingItemTemplate, and other control templates to indicate where data would be formatted and placed.  [Microsoft's original documentation about the DataBinder](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.databinder.eval?view=netframework-4.8) are available.

## Typical ASP.NET Syntax

There are several common techniques that the DataBinder was used and various levels of support are provided:

| Web Forms Syntax | Description | Blazor Support |
| --- | --- | --- |
| `DataBinder.Eval(Container.DataItem, "PropertyName")` | Output the `PropertyName` of the current item | *Fully Supported for Container.DataItem* |
| `Eval("PropertyName")` | Output the `PropertyName` of the current item | *Fully Supported when using static* |
| `DataBinder.Eval(Container.DataItem, "PropertyName", "FormatString")` | Output the formatted value of `PropertyName` of the current item with the `FormatString` | *Fully Supported for Container.DataItem* |
| `Eval("PropertyName", "FormatString")` | Output the formatted value of `PropertyName` of the current item with the `FormatString` | *Fully Supported when using static* |

## Support and Migration

The DataBinder is not recommended by Microsoft in Web Forms for use in high-performance applications due to the amount of reflection used to output and format content.  Similarly, we do not recommend long-term use of the DataBinder and have marked it with an `Obsolete` flag indicating that there are methods to easily migrate syntax to be more Razor-performance-friendly.