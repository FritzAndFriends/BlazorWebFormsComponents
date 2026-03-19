# BaseValidator

The BaseValidator component serves as the abstract base class for all validation controls in BlazorWebFormsComponents. It provides the shared properties, lifecycle, and integration with Blazor's EditForm validation system that all validator components inherit.

Original Microsoft documentation for ASP.NET BaseValidator: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.basevalidator?view=netframework-4.8

## Overview

BaseValidator is not used directly in markup—instead, you use its concrete implementations:

- RequiredFieldValidator — validates that a field has a value
- CompareValidator — compares a field value to a constant or another field
- RangeValidator — validates that a value falls within a range
- RegularExpressionValidator — validates against a pattern
- CustomValidator — calls a custom validation function

All validators inherit the same set of properties, display modes, and validation group support from BaseValidator.

## Properties

### ControlToValidate

Reference to the input control to validate. You can specify this in two ways:

1. **String ID** (Web Forms style):
   ```razor
   <InputText @bind-Value="model.Name" />
   <RequiredFieldValidator ControlToValidate="Name" Text="Name is required" />
   ```
   The string matches the property name on your EditForm model.

2. **ForwardRef** (Blazor native):
   ```razor
   <InputText @ref="NameInput.Current" @bind-Value="model.Name" />
   <RequiredFieldValidator ControlRef="@NameInput" Text="Name is required" />
   
   @code {
       ForwardRef<InputBase<string>> NameInput = new ForwardRef<InputBase<string>>();
   }
   ```
   When both `ControlToValidate` and `ControlRef` are set, `ControlRef` takes precedence.

### Display

Controls how the error message is displayed:

| Value | Behavior |
|-------|----------|
| **Static** (default) | Error message always takes up space (visibility: hidden when valid) |
| **Dynamic** | Error message only appears when invalid (display: none when valid) |
| **None** | Error message never displayed |

```razor
<RequiredFieldValidator Display="ValidatorDisplay.Dynamic" Text="Name is required" />
```

### Text

The error message displayed inline next to the validator. Shown when validation fails.

```razor
<RequiredFieldValidator Text="This field is required" />
```

### ErrorMessage

The error message displayed in the ValidationSummary component. Can be different from Text.

```razor
<RequiredFieldValidator ErrorMessage="Name field is required" />
```

### ValidationGroup

Groups validators for selective validation. When a button with a matching ValidationGroup is clicked, only validators in that group validate. Requires wrapping your form in `<ValidationGroupProvider>`.

```razor
<ValidationGroupProvider>
    <EditForm Model="@model">
        <InputText @ref="NameInput.Current" @bind-Value="model.Name" />
        <RequiredFieldValidator ControlRef="@NameInput" 
                                Text="Name is required" 
                                ValidationGroup="Personal" />
    </EditForm>
</ValidationGroupProvider>
```

### Enabled

Enable or disable the validator. When disabled, the validator does not perform validation.

```razor
<RequiredFieldValidator Enabled="@isEnabled" Text="Name is required" />
```

### Style Properties

BaseValidator inherits from BaseStyledComponent and supports all standard Web Forms styling:

- `ForeColor` — text color of the error message
- `BackColor` — background color
- `CssClass` — CSS class name(s)
- `Font.Bold`, `Font.Italic`, `Font.Name`, `Font.Size`, `Font.Strikeout`, `Font.Underline` — font styling

```razor
<RequiredFieldValidator ForeColor="Red" CssClass="validation-error" Text="Required" />
```

### IsValid

A read-only property that indicates whether the field is currently valid. Useful for conditionally rendering UI based on validation state.

```razor
@if (!myValidator.IsValid)
{
    <p>Please fix the validation errors above.</p>
}

@code {
    RequiredFieldValidator myValidator;
}
```

## Validation Lifecycle

BaseValidator integrates with Blazor's EditForm validation system:

1. **Registration** — When a validator initializes, it registers with the cascading EditContext
2. **Validation Request** — When the form validates (e.g., button submit), the EditContext fires `OnValidationRequested`
3. **Value Resolution** — The validator retrieves the current value from the input control
4. **Validation** — The validator checks the value and updates the message store
5. **State Notification** — The EditContext is notified of validation state changes
6. **Cleanup** — When the validator is disposed, it unregisters from the EditContext

## EditContext Integration

All validators require a cascading EditContext, typically provided by the EditForm component:

```razor
<EditForm Model="@model">
    <InputText @bind-Value="model.Name" />
    <RequiredFieldValidator Text="Name is required" />
</EditForm>
```

The EditContext coordinates validation across all validators on the form.

## Web Forms → Blazor Comparison

### Web Forms Syntax

```html
<asp:RequiredFieldValidator
    ControlToValidate="NameInput"
    Display="Dynamic"
    ErrorMessage="Name is required"
    Text="Name is required"
    ForeColor="Red"
    ValidationGroup="Personal"
    runat="server" />
```

### Blazor Syntax

```razor
<RequiredFieldValidator
    ControlToValidate="Name"
    Display="ValidatorDisplay.Dynamic"
    ErrorMessage="Name is required"
    Text="Name is required"
    ForeColor="Red"
    ValidationGroup="Personal" />
```

**Key Differences:**

- `ControlToValidate` in Blazor uses the model property name instead of a control ID
- `Display` uses the `ValidatorDisplay` enum instead of string values
- No `runat="server"` needed
- Optional `ControlRef` parameter for Blazor-native ForwardRef approach
- Removed: `EnableClientScript`, `SetFocusOnError` (Web Forms client-side scripting features not needed in Blazor)

## Child Validators

BaseValidator has the following concrete implementations:

- RequiredFieldValidator — validates that a field has a value
- CompareValidator — compares a field value to a constant or another field
- RangeValidator — validates that a value falls within a range
- RegularExpressionValidator — validates against a pattern
- CustomValidator — calls a custom validation function

See the individual validator pages for specific usage examples.
