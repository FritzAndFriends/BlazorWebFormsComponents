# ControlToValidate - Validator Control References

All validator components in BlazorWebFormsComponents support two patterns for referencing the input control to validate: a **string-based ID** (`ControlToValidate`) for Web Forms migration, and a **typed reference** (`ControlRef`) for Blazor-native development.

Original Microsoft implementation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.basevalidator.controltovalidate?view=netframework-4.8

## Supported Validators

This dual-pattern support is available in all validators:

| Validator | Base Class | Additional Parameters |
|-----------|-----------|----------------------|
| RequiredFieldValidator | `BaseValidator<T>` | — |
| CompareValidator | `BaseCompareValidator<T>` | `Operator`, `ValueToCompare` |
| RangeValidator | `BaseCompareValidator<T>` | `MinimumValue`, `MaximumValue` |
| RegularExpressionValidator | `BaseValidator<string>` | `ValidationExpression`, `MatchTimeout` |
| CustomValidator | `BaseValidator<string>` | `ValidateEmptyText`, `ServerValidate` |

## Pattern 1: ControlToValidate (String ID)

The `ControlToValidate` parameter accepts a string that maps to a **property name on the EditContext model**. This is the recommended pattern for Web Forms migration because it mirrors the original Web Forms attribute syntax.

```razor
@using BlazorWebFormsComponents.Validations

<EditForm Model="@model">
    <InputText @bind-Value="model.Name" />
    <RequiredFieldValidator ControlToValidate="Name"
                            Text="Name is required" />

    <InputText @bind-Value="model.Email" />
    <RegularExpressionValidator ControlToValidate="Email"
                                 ValidationExpression="^[\w.-]+@[\w.-]+\.\w+$"
                                 Text="Invalid email format" />
</EditForm>

@code {
    private FormModel model = new();

    public class FormModel
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
```

!!! tip "Migration from Web Forms"
    In Web Forms, `ControlToValidate` referenced a server control ID.
    In Blazor, it references the **model property name** bound to the input field.

## Pattern 2: ControlRef (Blazor-native ForwardRef)

The `ControlRef` parameter accepts a `ForwardRef<InputBase<T>>`, providing compile-time type safety. Use this pattern in new Blazor code.

```razor
@using BlazorWebFormsComponents.Validations

<EditForm Model="@model">
    <InputText @ref="NameInput.Current" @bind-Value="model.Name" />
    <RequiredFieldValidator ControlRef="@NameInput"
                            Text="Name is required" />

    <InputNumber @ref="AgeInput.Current" @bind-Value="model.Age" />
    <RangeValidator ControlRef="@AgeInput"
                    MinimumValue="1"
                    MaximumValue="120"
                    Text="Age must be between 1 and 120" />
</EditForm>

@code {
    ForwardRef<InputBase<string>> NameInput = new();
    ForwardRef<InputBase<int>> AgeInput = new();
    private FormModel model = new();
}
```

## Precedence Rules

When both `ControlRef` and `ControlToValidate` are provided, **`ControlRef` takes precedence**.

If neither is set, an `InvalidOperationException` is thrown at runtime.

| Configuration | Behavior |
|--------------|----------|
| `ControlRef` only | Uses typed reference to resolve field and value |
| `ControlToValidate` only | Uses string ID to resolve field from EditContext model |
| Both set | `ControlRef` wins |
| Neither set | Throws `InvalidOperationException` |

## Migrating from ForwardRef to String ID

If you are migrating Web Forms markup and want to keep it as close to the original as possible, use the `ControlToValidate` string pattern:

=== "Web Forms (Before)"

    ```html
    <asp:TextBox ID="txtName" runat="server" />
    <asp:RequiredFieldValidator
        ControlToValidate="txtName"
        Text="Name is required"
        runat="server" />
    ```

=== "Blazor with ControlToValidate (After)"

    ```razor
    <InputText @bind-Value="model.Name" />
    <RequiredFieldValidator ControlToValidate="Name"
                            Text="Name is required" />
    ```

=== "Blazor with ControlRef (Alternative)"

    ```razor
    <InputText @ref="NameInput.Current" @bind-Value="model.Name" />
    <RequiredFieldValidator ControlRef="@NameInput"
                            Text="Name is required" />
    ```

!!! note "Key Difference"
    In Web Forms, `ControlToValidate` referenced the **control's ID**. In Blazor, it references the **model property name** that the input is bound to.

## Complete Example: Registration Form

```razor
@using BlazorWebFormsComponents.Validations

<ValidationGroupProvider>
    <EditForm Model="@model">
        <div>
            <Label Text="Username:" />
            <InputText @bind-Value="model.Username" />
            <RequiredFieldValidator ControlToValidate="Username"
                                    Text="* Required"
                                    ValidationGroup="Registration" />
        </div>

        <div>
            <Label Text="Email:" />
            <InputText @bind-Value="model.Email" />
            <RequiredFieldValidator ControlToValidate="Email"
                                    Text="* Required"
                                    ValidationGroup="Registration" />
            <RegularExpressionValidator ControlToValidate="Email"
                                         ValidationExpression="^[\w.-]+@[\w.-]+\.\w+$"
                                         Text="Invalid email"
                                         ValidationGroup="Registration" />
        </div>

        <div>
            <Label Text="Age:" />
            <InputNumber @bind-Value="model.Age" />
            <RangeValidator ControlToValidate="Age"
                            MinimumValue="13"
                            MaximumValue="120"
                            Text="Must be 13-120"
                            ValidationGroup="Registration" />
        </div>

        <Button Text="Register" ValidationGroup="Registration" />
        <ValidationSummary ValidationGroup="Registration" />
    </EditForm>
</ValidationGroupProvider>

@code {
    private RegistrationModel model = new();

    public class RegistrationModel
    {
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public int Age { get; set; }
    }
}
```

## See Also

- [RequiredFieldValidator](RequiredFieldValidator.md)
- [CompareValidator](CompareValidator.md)
- [RangeValidator](RangeValidator.md)
- [RegularExpressionValidator](RegularExpressionValidator.md)
- [CustomValidator](CustomValidator.md)
- [ValidationSummary](ValidationSummary.md)
