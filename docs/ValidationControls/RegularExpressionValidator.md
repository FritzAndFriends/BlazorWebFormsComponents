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
| `MatchTimeout` | `int?` | `null` | Timeout in milliseconds for regex evaluation (prevents ReDoS) |
| `ControlToValidate` | `string` | `null` | Model property name to validate |
| `ControlRef` | `ForwardRef<InputBase<T>>` | `null` | Typed reference to input control |
| `Text` | `string` | `null` | Error text displayed inline |
| `ErrorMessage` | `string` | `null` | Error text shown in ValidationSummary |
| `Display` | `ValidatorDisplay` | `Static` | How error message is displayed |
| `ValidationGroup` | `string` | `null` | Selective validation group |
| `Enabled` | `bool` | `true` | Enable or disable the validator |
| `ForeColor` | `WebColor` | `Red` | Text color of error message |

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
    <InputText @bind-Value="model.Phone" placeholder="(555) 123-4567" />
    <RegularExpressionValidator
        ControlToValidate="Phone"
        ValidationExpression="^(\(\d{3}\)\s?)?\d{3}-\d{4}$"
        Text="Format: (555) 123-4567"
        ErrorMessage="Invalid phone number format" />
</EditForm>
```

### Postal Code Validation with ForwardRef

```razor
@using BlazorWebFormsComponents.Validations

<EditForm Model="@model">
    <InputText @ref="ZipInput.Current" @bind-Value="model.ZipCode" placeholder="12345 or 12345-6789" />
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
    MatchTimeout="2000"
    Text="Invalid format"
    ErrorMessage="Input does not match the required pattern" />
```

### Username Validation

```razor
<EditForm Model="@model">
    <InputText @bind-Value="model.Username" placeholder="alphanumeric and underscore" />
    <RegularExpressionValidator
        ControlToValidate="Username"
        ValidationExpression="^[a-zA-Z0-9_]{3,20}$"
        Text="Username must be 3-20 characters (letters, numbers, underscore only)"
        ErrorMessage="Username format is invalid" />
</EditForm>
```

### URL Validation

```razor
<EditForm Model="@model">
    <InputText @bind-Value="model.Website" placeholder="https://example.com" />
    <RegularExpressionValidator
        ControlToValidate="Website"
        ValidationExpression="^https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)$"
        Text="Please enter a valid URL"
        ErrorMessage="The website URL is not valid" />
</EditForm>
```

### Hexadecimal Color Code Validation

```razor
<EditForm Model="@model">
    <InputText @bind-Value="model.Color" placeholder="#RRGGBB" />
    <RegularExpressionValidator
        ControlToValidate="Color"
        ValidationExpression="^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$"
        Text="Enter a valid hex color (e.g., #FF5733 or #F57)"
        ErrorMessage="Invalid color format" />
</EditForm>
```

!!! tip "Preventing ReDoS Attacks"
    Use the `MatchTimeout` property when your regex pattern could be vulnerable to Regular Expression Denial of Service (ReDoS) attacks. This sets a timeout (in milliseconds) for the regex engine, preventing catastrophic backtracking on malicious input. For example, set `MatchTimeout="2000"` for a 2-second timeout.

## HTML Output

**Email Input with Validation:**
```html
<!-- Blazor Input -->
<InputText @bind-Value="model.Email" />
<RegularExpressionValidator
    ControlToValidate="Email"
    ValidationExpression="^[\w.-]+@[\w.-]+\.\w+$"
    Text="Invalid email"
    ErrorMessage="Email format is invalid" />

<!-- Rendered HTML (valid input) -->
<input type="text" value="user@example.com" />

<!-- Rendered HTML (invalid input, Display=Dynamic) -->
<input type="text" value="invalid-email" />
<span style="color:Red;">Invalid email</span>
```

## Common Regular Expression Patterns

| Pattern | Use Case | Example |
|---------|----------|---------|
| `^[\w.-]+@[\w.-]+\.\w+$` | Email address | user@example.com |
| `^(\(\d{3}\)\s?)?\d{3}-\d{4}$` | US Phone | (555) 123-4567 |
| `^\d{5}(-\d{4})?$` | US ZIP Code | 12345 or 12345-6789 |
| `^[a-zA-Z0-9_]{3,20}$` | Username | john_doe, user123 |
| `^https?://...` | URL | https://example.com |
| `^#([A-Fa-f0-9]{6}\|[A-Fa-f0-9]{3})$` | Hex Color | #FF5733 or #F57 |
| `^\d{3}-\d{2}-\d{4}$` | SSN | 123-45-6789 |
| `^\d{4}[-/]\d{2}[-/]\d{2}$` | Date | 2023-12-25 or 2023/12/25 |

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
