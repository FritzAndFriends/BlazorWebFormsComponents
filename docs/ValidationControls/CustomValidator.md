# CustomValidator

The CustomValidator component provides custom validation logic using a server-side function. It emulates the ASP.NET Web Forms `asp:CustomValidator` control, allowing you to define arbitrary validation rules.

Original Web Forms documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.customvalidator?view=netframework-4.8

## Features Supported in Blazor

- **ServerValidate** - `Func<string, bool>` parameter that executes custom validation logic
- **ValidateEmptyText** - Controls whether empty values are validated
- **ControlRef** - Reference to the input control being validated
- **Text / ErrorMessage** - Inline and summary error messages
- **ClientValidationFunction** - Migration stub (accepted but not executed in Blazor)
- **IsValid** - Indicates whether validation passes

## Web Forms Features NOT Supported

- **ClientValidationFunction execution** - Blazor does not execute client-side JavaScript validation functions. The property is accepted for migration compatibility.
- **ValidateOnLoad** - Not supported

## Web Forms Declarative Syntax

```html
<asp:CustomValidator
    ID="CustomValidator1"
    runat="server"
    ControlToValidate="TextBox1"
    ClientValidationFunction="validateLength"
    ErrorMessage="Value must be at least 5 characters."
    OnServerValidate="CustomValidator1_ServerValidate" />
```

## Blazor Syntax

```razor
@using BlazorWebFormsComponents.Validations

<EditForm Model="@model">
    <InputText @ref="Input.Current" @bind-Value="model.Value" />
    <CustomValidator ServerValidate="@ValidateLength"
                     ControlRef="@Input"
                     Text="Must be at least 5 characters."
                     ErrorMessage="Value too short!" />
</EditForm>

@code {
    ForwardRef<InputBase<string>> Input = new();

    private bool ValidateLength(string value)
    {
        return !string.IsNullOrEmpty(value) && value.Length >= 5;
    }
}
```

## Examples

### Basic Custom Validation

```razor
@using BlazorWebFormsComponents.Validations

<EditForm Model="@model" OnValidSubmit="HandleSubmit">
    <label>Enter a value starting with 'A':</label>
    <InputText @ref="NameRef.Current" @bind-Value="model.Name" />
    <CustomValidator ServerValidate="@StartsWithA"
                     ControlRef="@NameRef"
                     Text="Must start with 'A'."
                     ErrorMessage="Value must start with 'A'!" />
    <button type="submit">Submit</button>
</EditForm>

@code {
    ForwardRef<InputBase<string>> NameRef = new();
    private MyModel model = new();

    private bool StartsWithA(string value)
        => value.StartsWith("A", StringComparison.OrdinalIgnoreCase);

    private void HandleSubmit() { }
    public class MyModel { public string Name { get; set; } }
}
```

### With ClientValidationFunction (Migration Stub)

```razor
@* ClientValidationFunction is accepted for migration but not executed in Blazor *@
<CustomValidator ServerValidate="@AlwaysValid"
                 ControlRef="@InputRef"
                 ClientValidationFunction="validateOnClient"
                 IsValid="true"
                 Text="Validation failed." />

@code {
    private bool AlwaysValid(string value) => true;
}
```

### ValidateEmptyText

```razor
@* By default, empty values pass validation. Set ValidateEmptyText to validate them. *@
<CustomValidator ServerValidate="@RequireValue"
                 ControlRef="@InputRef"
                 ValidateEmptyText="true"
                 Text="A value is required." />

@code {
    private bool RequireValue(string value) => !string.IsNullOrWhiteSpace(value);
}
```

## Key Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ServerValidate` | `Func<string, bool>` | null | Custom validation function |
| `ValidateEmptyText` | `bool` | false | Whether to validate empty/null values |
| `ClientValidationFunction` | `string` | null | Migration stub — name of client-side function (not executed) |
| `IsValid` | `bool` | true | Whether the control currently passes validation |
| `ControlRef` | `ForwardRef<InputBase<T>>` | null | Reference to the input control being validated |
| `Text` | `string` | null | Error text displayed inline |
| `ErrorMessage` | `string` | null | Error text shown in ValidationSummary |

## Migration Notes

1. Remove `asp:` prefix and `runat="server"`
2. Replace `OnServerValidate="Handler"` with `ServerValidate="@Handler"` — change from event to `Func<string, bool>`
3. Replace `ControlToValidate="TextBox1"` with `ControlRef="@ref"` using `ForwardRef`
4. `ClientValidationFunction` is accepted but has no effect in Blazor
5. `IsValid` defaults to `true` — use it to programmatically indicate validation state

## See Also

- [RequiredFieldValidator](RequiredFieldValidator.md)
- [CompareValidator](CompareValidator.md)
- [RangeValidator](RangeValidator.md)
- [RegularExpressionValidator](RegularExpressionValidator.md)
- [ValidationSummary](ValidationSummary.md)
