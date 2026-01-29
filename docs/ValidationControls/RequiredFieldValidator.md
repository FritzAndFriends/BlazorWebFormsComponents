The RequiredFieldValidator component provides validation that another field on the form has a value provided.

Original Microsoft implementation of the ASP<span></span>.NET RequiredFieldValidator component is at:  https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.requiredfieldvalidator?view=netframework-4.8

## Features supported in Blazor

- `ControlToValidate` - Reference to the input control to validate (using `ForwardRef<InputBase<T>>`)
- `Text` - Error message displayed inline when validation fails
- `ErrorMessage` - Message shown in ValidationSummary
- `ValidationGroup` - Group name for selective validation (see ValidationGroup section below)
- `Enabled` - Enable or disable the validator
- `Display` - How the error message is displayed (Static, Dynamic, None)
- All style properties (`ForeColor`, `BackColor`, `CssClass`, etc.)

## ValidationGroup Support

The `ValidationGroup` property allows validators to participate in selective validation. When a button with a matching `ValidationGroup` is clicked, only validators in that group will validate.

**Key Points:**

1. Validators and buttons with the same `ValidationGroup` value work together
2. Validators without a `ValidationGroup` are triggered by buttons without a `ValidationGroup`
3. Must wrap form in `<ValidationGroupProvider>` to use `ValidationGroup`

**Example:**

```razor
@using BlazorWebFormsComponents.Validations

<ValidationGroupProvider>
    <EditForm Model="@model">
        @* This validator is in the "Personal" group *@
        <InputText @ref="NameInput.Current" @bind-Value="model.Name" />
        <RequiredFieldValidator ControlToValidate="@NameInput"
                                Text="Name is required"
                                ValidationGroup="Personal" />
        
        @* This validator is in the "Business" group *@
        <InputText @ref="CompanyInput.Current" @bind-Value="model.Company" />
        <RequiredFieldValidator ControlToValidate="@CompanyInput"
                                Text="Company is required"
                                ValidationGroup="Business" />
        
        @* This button only validates the "Personal" group *@
        <Button Text="Validate Personal" ValidationGroup="Personal" />
        
        @* This button only validates the "Business" group *@
        <Button Text="Validate Business" ValidationGroup="Business" />
    </EditForm>
</ValidationGroupProvider>

@code {
    ForwardRef<InputBase<string>> NameInput = new ForwardRef<InputBase<string>>();
    ForwardRef<InputBase<string>> CompanyInput = new ForwardRef<InputBase<string>>();
    private FormModel model = new FormModel();
}
```

## Web Forms Declarative Syntax

## Usage Notes

## Blazor Syntax
