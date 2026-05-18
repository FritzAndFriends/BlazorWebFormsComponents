# RequiredFieldValidator

The **RequiredFieldValidator** component validates that another field on the form has a value provided. It emulates the ASP.NET Web Forms `asp:RequiredFieldValidator` control.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.requiredfieldvalidator?view=netframework-4.8

## Features Supported in Blazor

- `ControlToValidate` — Reference to the input control to validate (model property name)
- `ControlRef` — Typed reference using `ForwardRef<InputBase<T>>`
- `Text` — Error message displayed inline when validation fails
- `ErrorMessage` — Message shown in ValidationSummary
- `InitialValue` — The initial value to treat as "empty" (default: empty string)
- `ValidationGroup` — Group name for selective validation
- `Display` — How the error message is displayed (Static, Dynamic, None)
- `Enabled` — Enable or disable the validator
- All style properties (`ForeColor`, `BackColor`, `CssClass`, etc.)

## Web Forms Features NOT Supported

- **EnableClientScript** — Blazor uses its own validation model
- **SetFocusOnError** — Blazor has different focus management

## Syntax Comparison

=== "Web Forms"

    ```html
    <asp:RequiredFieldValidator
        ControlToValidate="string"
        Display="None|Static|Dynamic"
        EnableClientScript="True|False"
        Enabled="True|False"
        ErrorMessage="string"
        ForeColor="color name|#dddddd"
        ID="string"
        InitialValue="string"
        runat="server"
        SetFocusOnError="True|False"
        Text="string"
        ValidationGroup="string"
        Visible="True|False"
    />
    ```

=== "Blazor"

    ```razor
    <RequiredFieldValidator
        ControlToValidate="PropertyName"
        Text="This field is required"
        ErrorMessage="Field is required"
        Display="ValidatorDisplay.Dynamic"
        ValidationGroup="MyGroup"
        Enabled="true" />
    ```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ControlToValidate` | `string` | `null` | Model property name to validate |
| `ControlRef` | `ForwardRef<InputBase<T>>` | `null` | Typed reference to input control |
| `Text` | `string` | `null` | Error text displayed inline |
| `ErrorMessage` | `string` | `null` | Error text shown in ValidationSummary |
| `InitialValue` | `string` | `""` | Value treated as "not entered" |
| `Display` | `ValidatorDisplay` | `Static` | How error message is displayed |
| `ValidationGroup` | `string` | `null` | Selective validation group |
| `Enabled` | `bool` | `true` | Enable or disable the validator |
| `ForeColor` | `WebColor` | `Red` | Text color of error message |

## Usage Notes

- The validator checks that the input value differs from `InitialValue` (default: empty string)
- Both `ControlToValidate` (string) and `ControlRef` (ForwardRef) patterns are supported — see [ControlToValidate](ControlToValidate.md) for details
- When `ControlRef` and `ControlToValidate` are both set, `ControlRef` takes precedence
- Requires a cascading `EditContext` — must be placed inside an `<EditForm>`

## ValidationGroup Support

The `ValidationGroup` property allows validators to participate in selective validation. When a button with a matching `ValidationGroup` is clicked, only validators in that group will validate.

**Key Points:**

1. Validators and buttons with the same `ValidationGroup` value work together
2. Validators without a `ValidationGroup` are triggered by buttons without a `ValidationGroup`
3. Must wrap form in `<ValidationGroupProvider>` to use `ValidationGroup`

## Examples

### Basic Required Field

```razor
<EditForm Model="@model" OnValidSubmit="HandleSubmit">
    <InputText @bind-Value="model.Name" />
    <RequiredFieldValidator ControlToValidate="Name"
                            Text="Name is required"
                            ErrorMessage="Please enter your name" />
    <button type="submit">Submit</button>
</EditForm>

@code {
    private var model = new FormModel();
    private void HandleSubmit() { }
}
```

### Using ForwardRef

```razor
@using BlazorWebFormsComponents.Validations

<EditForm Model="@model">
    <InputText @ref="NameInput.Current" @bind-Value="model.Name" />
    <RequiredFieldValidator ControlRef="@NameInput"
                            Text="Name is required"
                            Display="ValidatorDisplay.Dynamic" />
</EditForm>

@code {
    ForwardRef<InputBase<string>> NameInput = new();
    private var model = new FormModel();
}
```

### With ValidationGroup

```razor
@using BlazorWebFormsComponents.Validations

<ValidationGroupProvider>
    <EditForm Model="@model">
        <InputText @ref="NameInput.Current" @bind-Value="model.Name" />
        <RequiredFieldValidator ControlToValidate="@NameInput"
                                Text="Name is required"
                                ValidationGroup="Personal" />

        <InputText @ref="CompanyInput.Current" @bind-Value="model.Company" />
        <RequiredFieldValidator ControlToValidate="@CompanyInput"
                                Text="Company is required"
                                ValidationGroup="Business" />

        <Button Text="Validate Personal" ValidationGroup="Personal" />
        <Button Text="Validate Business" ValidationGroup="Business" />
    </EditForm>
</ValidationGroupProvider>

@code {
    ForwardRef<InputBase<string>> NameInput = new();
    ForwardRef<InputBase<string>> CompanyInput = new();
    private var model = new FormModel();
}
```

### InitialValue

```razor
@* Validates that the user changed the dropdown from its default value *@
<InputSelect @bind-Value="model.Country">
    <option value="">-- Select Country --</option>
    <option value="US">United States</option>
    <option value="CA">Canada</option>
</InputSelect>
<RequiredFieldValidator ControlToValidate="Country"
                        InitialValue=""
                        Text="Please select a country" />
```

## Migration Notes

1. **Remove `asp:` prefix** — Change `<asp:RequiredFieldValidator>` to `<RequiredFieldValidator>`
2. **Remove `runat="server"`** — Not needed in Blazor
3. **Update `ControlToValidate`** — Use model property name instead of control ID, or use `ControlRef` with `ForwardRef`
4. **`Display` uses enum** — Change `Display="Dynamic"` to `Display="ValidatorDisplay.Dynamic"`
5. **Remove `EnableClientScript`** — Not applicable in Blazor
6. **Remove `SetFocusOnError`** — Not supported in Blazor
7. **`InitialValue`, `Text`, `ErrorMessage` transfer directly** — No changes needed

### Migration Example

=== "Web Forms"

    ```html
    <asp:TextBox ID="txtName" runat="server" />
    <asp:RequiredFieldValidator
        ControlToValidate="txtName"
        Text="* Required"
        ErrorMessage="Name is required"
        Display="Dynamic"
        ForeColor="Red"
        runat="server" />
    ```

=== "Blazor"

    ```razor
    <InputText @bind-Value="model.Name" />
    <RequiredFieldValidator
        ControlToValidate="Name"
        Text="* Required"
        ErrorMessage="Name is required"
        Display="ValidatorDisplay.Dynamic"
        ForeColor="WebColor.Red" />
    ```

## See Also

- [CompareValidator](CompareValidator.md) — Compare values
- [RangeValidator](RangeValidator.md) — Validate value ranges
- [RegularExpressionValidator](RegularExpressionValidator.md) — Pattern validation
- [CustomValidator](CustomValidator.md) — Custom validation logic
- [ValidationSummary](ValidationSummary.md) — Display validation errors
- [ControlToValidate](ControlToValidate.md) — Control reference patterns
- [BaseValidator](BaseValidator.md) — Base class for all validators
