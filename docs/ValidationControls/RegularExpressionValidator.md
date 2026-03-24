# RegularExpressionValidator

The **RegularExpressionValidator** component validates that user input matches a specified regular expression pattern. It emulates the ASP.NET Web Forms `asp:RegularExpressionValidator` control, useful for enforcing formats such as email addresses, phone numbers, postal codes, and other structured input.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.regularexpressionvalidator?view=netframework-4.8

## Features Supported in Blazor

- `ValidationExpression` — The regular expression pattern to match against
- `MatchTimeout` — Timeout for regex evaluation to prevent ReDoS attacks
- `ControlToValidate` / `ControlRef` — Reference to the input control to validate
- `Text` — Error message displayed inline when validation fails
- `ErrorMessage` — Message shown in ValidationSummary
- `ValidationGroup` — Group name for selective validation
- `Display` — How the error message is displayed (Static, Dynamic, None)
- `Enabled` — Enable or disable the validator
- All style properties (`ForeColor`, `BackColor`, `CssClass`, etc.)

## Web Forms Features NOT Supported

- **EnableClientScript** — Blazor uses its own validation model instead of client-side JavaScript
- **SetFocusOnError** — Blazor has different focus management

## Syntax Comparison

=== "Web Forms"

    ```html
    <asp:RegularExpressionValidator
        ControlToValidate="string"
        Display="None|Static|Dynamic"
        EnableClientScript="True|False"
        Enabled="True|False"
        ErrorMessage="string"
        ForeColor="color name|#dddddd"
        ID="string"
        runat="server"
        SetFocusOnError="True|False"
        Text="string"
        ValidationExpression="string"
        ValidationGroup="string"
        Visible="True|False"
    />
    ```

=== "Blazor"

    ```razor
    <RegularExpressionValidator
        ControlToValidate="PropertyName"
        ValidationExpression="^[\w.-]+@[\w.-]+\.\w+$"
        Text="Invalid format"
        ErrorMessage="The field does not match the required pattern"
        Display="ValidatorDisplay.Dynamic"
        ValidationGroup="MyGroup"
        Enabled="true" />
    ```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ValidationExpression` | `string` | `null` | Regular expression pattern to validate against |
| `MatchTimeout` | `TimeSpan?` | `null` | Timeout for regex evaluation (prevents ReDoS) |
| `ControlToValidate` | `string` | `null` | Model property name to validate |
| `ControlRef` | `ForwardRef<InputBase<T>>` | `null` | Typed reference to input control |
| `Text` | `string` | `null` | Error text displayed inline |
| `ErrorMessage` | `string` | `null` | Error text shown in ValidationSummary |
| `Display` | `ValidatorDisplay` | `Static` | How error message is displayed |
| `ValidationGroup` | `string` | `null` | Selective validation group |
| `Enabled` | `bool` | `true` | Enable or disable the validator |

## Examples

### Email Validation

```razor
<EditForm Model="@model">
    <InputText @bind-Value="model.Email" />
    <RegularExpressionValidator
        ControlToValidate="Email"
        ValidationExpression="^[\w.-]+@[\w.-]+\.\w+$"
        Text="Please enter a valid email address"
        ErrorMessage="Invalid email format" />
    <button type="submit">Submit</button>
</EditForm>

@code {
    private var model = new ContactModel();

    public class ContactModel
    {
        public string Email { get; set; } = "";
    }
}
```

### Phone Number Validation

```razor
<EditForm Model="@model">
    <InputText @bind-Value="model.Phone" />
    <RegularExpressionValidator
        ControlToValidate="Phone"
        ValidationExpression="^\(\d{3}\)\s?\d{3}-\d{4}$"
        Text="Format: (555) 123-4567"
        ErrorMessage="Invalid phone number format" />
</EditForm>
```

### Postal Code Validation with ForwardRef

```razor
@using BlazorWebFormsComponents.Validations

<EditForm Model="@model">
    <InputText @ref="ZipInput.Current" @bind-Value="model.ZipCode" />
    <RegularExpressionValidator
        ControlRef="@ZipInput"
        ValidationExpression="^\d{5}(-\d{4})?$"
        Text="Enter a valid ZIP code (e.g., 12345 or 12345-6789)"
        Display="ValidatorDisplay.Dynamic" />
</EditForm>

@code {
    ForwardRef<InputBase<string>> ZipInput = new();
    private var model = new AddressModel();
}
```

### With MatchTimeout (ReDoS Prevention)

```razor
<RegularExpressionValidator
    ControlToValidate="Input"
    ValidationExpression="^(?:[a-z0-9]+\.)*[a-z0-9]+$"
    MatchTimeout="@TimeSpan.FromSeconds(2)"
    Text="Invalid format"
    ErrorMessage="Input does not match the required pattern" />
```

!!! tip "Preventing ReDoS Attacks"
    Use the `MatchTimeout` property when your regex pattern could be vulnerable to Regular Expression Denial of Service (ReDoS) attacks. This sets a timeout for the regex engine, preventing catastrophic backtracking on malicious input.

## Migration Notes

1. **Remove `asp:` prefix** — Change `<asp:RegularExpressionValidator>` to `<RegularExpressionValidator>`
2. **Remove `runat="server"`** — Not needed in Blazor
3. **Update `ControlToValidate`** — Use the model property name instead of a control ID, or use `ControlRef` with `ForwardRef`
4. **`Display` uses enum** — Change `Display="Dynamic"` to `Display="ValidatorDisplay.Dynamic"`
5. **`ValidationExpression` stays the same** — Regex patterns are identical between Web Forms and Blazor
6. **Remove `EnableClientScript`** — Not applicable in Blazor
7. **Remove `SetFocusOnError`** — Not supported in Blazor

### Migration Example

=== "Web Forms"

    ```html
    <asp:TextBox ID="txtEmail" runat="server" />
    <asp:RegularExpressionValidator
        ControlToValidate="txtEmail"
        ValidationExpression="^[\w.-]+@[\w.-]+\.\w+$"
        ErrorMessage="Invalid email"
        Text="*"
        Display="Dynamic"
        runat="server" />
    ```

=== "Blazor"

    ```razor
    <InputText @bind-Value="model.Email" />
    <RegularExpressionValidator
        ControlToValidate="Email"
        ValidationExpression="^[\w.-]+@[\w.-]+\.\w+$"
        ErrorMessage="Invalid email"
        Text="*"
        Display="ValidatorDisplay.Dynamic" />
    ```

## See Also

- [RequiredFieldValidator](RequiredFieldValidator.md) — Validate required fields
- [CompareValidator](CompareValidator.md) — Compare values
- [RangeValidator](RangeValidator.md) — Validate value ranges
- [CustomValidator](CustomValidator.md) — Custom validation logic
- [ValidationSummary](ValidationSummary.md) — Display validation errors
- [ControlToValidate](ControlToValidate.md) — Control reference patterns
