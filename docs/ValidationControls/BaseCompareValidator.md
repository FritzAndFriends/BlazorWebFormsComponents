# BaseCompareValidator

The BaseCompareValidator component is an abstract base class that extends BaseValidator with type conversion and comparison logic. It provides the core functionality for validators that need to compare values, such as CompareValidator and RangeValidator.

BaseCompareValidator is not used directly in markup. Instead, use its concrete implementations:

- CompareValidator — compares a field value to a constant or another field
- RangeValidator — validates that a value falls within a range

## Overview

BaseCompareValidator adds type conversion and comparison capabilities to the BaseValidator framework. It enables validators to safely compare values of different types (integers, dates, currency, etc.) by converting them to a common type before comparison.

## Properties

### Type

Specifies the data type for conversion and comparison. All values are converted to this type before comparison occurs.

```razor
<CompareValidator Type="ValidationDataType.Integer" />
<RangeValidator Type="ValidationDataType.Date" />
```

Supported types:

| Type | Description | Example |
|------|-------------|---------|
| **String** | Text comparison (default) | "apple", "banana" |
| **Integer** | Whole number comparison | 42, 100, -5 |
| **Double** | Decimal number comparison | 3.14, 99.99 |
| **Date** | Date comparison | "2024-01-15" |
| **Currency** | Currency value comparison | 19.99, 100.00 |

### CultureInvariantValues

Controls whether values are converted using culture-invariant or culture-aware parsing.

- `true` — Values are parsed using invariant culture (e.g., always use "." as decimal separator)
- `false` (default) — Values are parsed using the current UI culture (e.g., "," in many European locales)

```razor
<CompareValidator Type="ValidationDataType.Double" 
                  CultureInvariantValues="true"
                  ValueToCompare="3.14" />
```

This is useful when comparing numeric or currency values across different locales.

## Type Conversion

BaseCompareValidator provides the `Compare()` method to safely convert and compare values:

```csharp
protected bool Compare(string leftText, 
                      bool cultureInvariantLeftText, 
                      string rightText, 
                      bool cultureInvariantRightText, 
                      ValidationCompareOperator op, 
                      ValidationDataType type)
```

The comparison logic:

1. **Convert Left Value** — Converts the field value to the specified type
2. **Check Operator** — If operator is `DataTypeCheck`, validation passes (value is valid type)
3. **Convert Right Value** — Converts the comparison value to the specified type
4. **Compare** — Performs the comparison and returns the result

If either value cannot be converted to the specified type, validation fails (except for `DataTypeCheck`, which only validates the type itself).

## Examples

### Integer Comparison

```razor
<EditForm Model="@model">
    <InputNumber @bind-Value="model.Age" />
    <RangeValidator TValue="int"
                    Type="ValidationDataType.Integer"
                    MinimumValue="18"
                    MaximumValue="120"
                    ErrorMessage="Age must be between 18 and 120" />
</EditForm>

@code {
    class FormModel { public int Age { get; set; } }
    var model = new FormModel();
}
```

### Date Comparison

```razor
<EditForm Model="@model">
    <InputDate @bind-Value="model.BirthDate" />
    <RangeValidator TValue="DateTime"
                    Type="ValidationDataType.Date"
                    MinimumValue="1900-01-01"
                    MaximumValue="2023-12-31"
                    ErrorMessage="Birth date must be between 1900 and 2023" />
</EditForm>
```

### Currency Comparison

```razor
<EditForm Model="@model">
    <InputNumber @bind-Value="model.Price" />
    <CompareValidator TValue="decimal"
                      Type="ValidationDataType.Currency"
                      ValueToCompare="100.00"
                      Operator="ValidationCompareOperator.GreaterThanEqual"
                      ErrorMessage="Price must be at least $100.00" />
</EditForm>
```

### Culture-Invariant Parsing

```razor
<CompareValidator Type="ValidationDataType.Double"
                  CultureInvariantValues="true"
                  ValueToCompare="3.14"
                  ErrorMessage="Must be greater than 3.14" />
```

In a German locale, the UI shows "3,14" but validation parses both sides as "3.14" (US format) due to `CultureInvariantValues="true"`.

## Syntax Comparison

=== "Web Forms"

    ```html
    <asp:RangeValidator
        ControlToValidate="AgeInput"
        Type="Integer"
        MinimumValue="18"
        MaximumValue="120"
        ErrorMessage="Age must be between 18 and 120"
        Text="Invalid age"
        runat="server" />
    ```

=== "Blazor"

    ```razor
    <RangeValidator TValue="int"
        ControlToValidate="Age"
        Type="ValidationDataType.Integer"
        MinimumValue="18"
        MaximumValue="120"
        ErrorMessage="Age must be between 18 and 120"
        Text="Invalid age" />
    ```

**Key Differences:**

- `Type` uses the `ValidationDataType` enum instead of string values
- Generic type parameter `TValue` is required in Blazor
- No `runat="server"` needed
- Culture-aware vs. culture-invariant parsing is more explicit with `CultureInvariantValues`

## Child Validators

BaseCompareValidator has the following concrete implementations:

- CompareValidator — compares a field value to a constant or another field
- RangeValidator — validates that a value falls within a minimum and maximum range

See the individual validator pages for specific usage examples and property details.

## Inherited Properties

BaseCompareValidator inherits all properties from BaseValidator:

- `ControlToValidate` — reference to the input control
- `ControlRef` — Blazor ForwardRef to input control
- `Display` — how error message is displayed
- `Text` — inline error message
- `ErrorMessage` — summary error message
- `ValidationGroup` — selective validation group
- `Enabled` — enable/disable the validator
- Style properties (`ForeColor`, `BackColor`, `CssClass`, etc.)

See the BaseValidator page for details on these inherited properties.
