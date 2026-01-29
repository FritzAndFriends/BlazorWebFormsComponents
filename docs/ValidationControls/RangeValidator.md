# RangeValidator

The RangeValidator component validates that user input falls within a specified range of values. This is useful for ensuring numeric values, dates, or strings are within acceptable bounds. Original Web Forms documentation is at: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.rangevalidator?view=netframework-4.8

## Blazor Features Supported

- MinimumValue and MaximumValue range boundaries
- Data type validation: String, Integer, Double, Date, Currency
- CultureInvariantValues for culture-independent comparisons
- ErrorMessage display when validation fails
- Integration with EditForm validation

## WebForms Features Not Supported

- Display property (Static, Dynamic, None) - Blazor handles rendering differently
- SetFocusOnError - Blazor has different focus management
- EnableClientScript - Blazor uses its own validation model

## WebForms Syntax

```html
<asp:RangeValidator
    ControlToValidate="string"
    CultureInvariantValues="True|False"
    Display="None|Static|Dynamic"
    EnableClientScript="True|False"
    Enabled="True|False"
    ErrorMessage="string"
    ForeColor="color name|#dddddd"
    ID="string"
    MaximumValue="string"
    MinimumValue="string"
    runat="server"
    SetFocusOnError="True|False"
    Text="string"
    Type="String|Integer|Double|Date|Currency"
    ValidationGroup="string"
    Visible="True|False"
/>
```

## Blazor Syntax

```razor
<RangeValidator TValue="string"
    MinimumValue="1"
    MaximumValue="100"
    Type="ValidationDataType.Integer"
    ErrorMessage="Value must be between 1 and 100"
    @ref="myValidator" />
```

## Usage Notes

The RangeValidator is a generic component that takes a type parameter `TValue`. It inherits from `BaseCompareValidator` which provides the core comparison logic.

### Example: Validating a Numeric Range

```razor
<EditForm Model="@model">
    <InputNumber @bind-Value="model.Quantity" />
    <RangeValidator TValue="int"
        MinimumValue="1"
        MaximumValue="999"
        Type="ValidationDataType.Integer"
        ErrorMessage="Quantity must be between 1 and 999" />
</EditForm>
```

### Example: Validating a Date Range

```razor
<EditForm Model="@model">
    <InputDate @bind-Value="model.BirthDate" />
    <RangeValidator TValue="DateTime"
        MinimumValue="1/1/1900"
        MaximumValue="12/31/2010"
        Type="ValidationDataType.Date"
        ErrorMessage="Birth date must be between 1900 and 2010" />
</EditForm>
```

### Supported Data Types

| Type | Description |
|------|-------------|
| String | Alphabetical string comparison |
| Integer | Whole number range validation |
| Double | Decimal number range validation |
| Date | Date range validation |
| Currency | Currency value range validation |

### Behavior Notes

- Empty or null values pass validation (use RequiredFieldValidator in combination if the field is required)
- Whitespace-only values pass validation
- Both MinimumValue and MaximumValue boundaries are inclusive
