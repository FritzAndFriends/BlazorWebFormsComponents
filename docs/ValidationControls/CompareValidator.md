# CompareValidator

The CompareValidator component validates user input by comparing it to a constant value or to the value of another control using a comparison operator (equals, greater than, less than, etc.). Original Web Forms documentation is at: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.comparevalidator?view=netframework-4.8

## Blazor Features Supported

- Comparison operators: Equal, NotEqual, GreaterThan, GreaterThanEqual, LessThan, LessThanEqual, DataTypeCheck
- Data type validation: String, Integer, Double, Date, Currency
- ValueToCompare property for comparing against a constant value
- CultureInvariantValues for culture-independent comparisons
- ErrorMessage display when validation fails
- Integration with EditForm validation

## WebForms Features Not Supported

- ControlToCompare is not directly supported; use ValueToCompare with binding instead
- Display property (Static, Dynamic, None) - Blazor handles rendering differently
- SetFocusOnError - Blazor has different focus management

## WebForms Syntax

```html
<asp:CompareValidator
    ControlToCompare="string"
    ControlToValidate="string"
    CultureInvariantValues="True|False"
    Display="None|Static|Dynamic"
    EnableClientScript="True|False"
    Enabled="True|False"
    ErrorMessage="string"
    ForeColor="color name|#dddddd"
    ID="string"
    Operator="Equal|NotEqual|GreaterThan|GreaterThanEqual|LessThan|
        LessThanEqual|DataTypeCheck"
    runat="server"
    SetFocusOnError="True|False"
    Text="string"
    Type="String|Integer|Double|Date|Currency"
    ValidationGroup="string"
    ValueToCompare="string"
    Visible="True|False"
/>
```

## Blazor Syntax

```razor
<CompareValidator TValue="string"
    ValueToCompare="100"
    Operator="ValidationCompareOperator.GreaterThan"
    Type="ValidationDataType.Integer"
    ErrorMessage="Value must be greater than 100"
    @ref="myValidator" />
```

## Usage Notes

The CompareValidator is a generic component that takes a type parameter `TValue`. It inherits from `BaseCompareValidator` which provides the core comparison logic.

### Example: Comparing to a Constant Value

```razor
<EditForm Model="@model">
    <InputNumber @bind-Value="model.Age" />
    <CompareValidator TValue="int"
        ValueToCompare="18"
        Operator="ValidationCompareOperator.GreaterThanEqual"
        Type="ValidationDataType.Integer"
        ErrorMessage="You must be at least 18 years old" />
</EditForm>
```

### Supported Operators

| Operator | Description |
|----------|-------------|
| Equal | Values must be equal |
| NotEqual | Values must not be equal |
| GreaterThan | Input must be greater than comparison value |
| GreaterThanEqual | Input must be greater than or equal to comparison value |
| LessThan | Input must be less than comparison value |
| LessThanEqual | Input must be less than or equal to comparison value |
| DataTypeCheck | Validates that input can be converted to the specified data type |

### Supported Data Types

| Type | Description |
|------|-------------|
| String | Text comparison |
| Integer | Whole number comparison |
| Double | Decimal number comparison |
| Date | Date comparison |
| Currency | Currency value comparison |
