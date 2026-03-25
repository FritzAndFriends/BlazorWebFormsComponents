# ValidationSummary

The **ValidationSummary** component displays a summary of all validation errors on a form. In BlazorWebFormsComponents, this component is named `AspNetValidationSummary` to avoid conflicts with Blazor's built-in `ValidationSummary` component. This tag name may change in future versions to align more closely with the original ASP.NET control.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.validationsummary?view=netframework-4.8

!!! note "Component Name"
    Use `<AspNetValidationSummary>` in Blazor markup. The name differs from the Web Forms `<asp:ValidationSummary>` to avoid conflicts with Blazor's built-in `<ValidationSummary>` component.

## Features Supported in Blazor

- `DisplayMode` — How errors are displayed: `BulletList`, `List`, or `SingleParagraph`
- `HeaderText` — Text displayed above the error list
- `ShowSummary` — Whether to display the summary (default: `true`)
- `ValidationGroup` — Only show errors from validators in the specified group
- `CssClass` — CSS class applied to the summary container
- `ForeColor` — Text color for error messages
- `Enabled` — Enable or disable the summary
- `Visible` — Show or hide the component
- All style properties (`BackColor`, `BorderColor`, `BorderStyle`, `BorderWidth`, `Width`, `Height`, `Font`)

## Web Forms Features NOT Supported

- **ShowMessageBox** — JavaScript alert boxes are not supported; use inline display instead
- **EnableClientScript** — Blazor uses its own validation model (stub parameter exists for migration compatibility)

## Syntax Comparison

=== "Web Forms"

    ```html
    <asp:ValidationSummary
        BackColor="color name|#dddddd"
        BorderColor="color name|#dddddd"
        BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|Inset|Outset"
        BorderWidth="size"
        CssClass="string"
        DisplayMode="BulletList|List|SingleParagraph"
        EnableClientScript="True|False"
        Enabled="True|False"
        Font-Bold="True|False"
        ForeColor="color name|#dddddd"
        HeaderText="string"
        Height="size"
        ID="string"
        runat="server"
        ShowMessageBox="True|False"
        ShowSummary="True|False"
        ValidationGroup="string"
        Visible="True|False"
        Width="size"
    />
    ```

=== "Blazor"

    ```razor
    <AspNetValidationSummary
        BackColor="WebColor.LightYellow"
        BorderColor="WebColor.Red"
        BorderStyle="BorderStyle.Solid"
        CssClass="validation-summary"
        DisplayMode="ValidationSummaryDisplayMode.BulletList"
        Enabled="true"
        ForeColor="WebColor.Red"
        HeaderText="Please correct the following errors:"
        ShowSummary="true"
        ValidationGroup="MyGroup"
        Visible="true"
    />
    ```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DisplayMode` | `ValidationSummaryDisplayMode` | `BulletList` | How errors are rendered |
| `HeaderText` | `string` | `null` | Text shown above the error list |
| `ShowSummary` | `bool` | `true` | Whether to display the summary |
| `ValidationGroup` | `string` | `null` | Filter errors by validation group |
| `CssClass` | `string` | `null` | CSS class for the container |
| `ForeColor` | `WebColor` | `default` | Text color of error messages |
| `BackColor` | `WebColor` | `default` | Background color |
| `BorderColor` | `WebColor` | `default` | Border color |
| `BorderStyle` | `BorderStyle` | `NotSet` | Border style |
| `BorderWidth` | `Unit` | `default` | Border width |
| `Width` | `Unit` | `default` | Container width |
| `Height` | `Unit` | `default` | Container height |
| `Font` | `FontInfo` | `new FontInfo()` | Font properties (Bold, Italic, etc.) |
| `Enabled` | `bool` | `true` | Enable or disable the component |
| `Visible` | `bool` | `true` | Show or hide the component |
| `EnableClientScript` | `bool` | `true` | Stub for migration compatibility (no effect) |
| `ShowMessageBox` | `bool` | `false` | Stub for migration compatibility (no effect) |

### DisplayMode Values

| Value | Description | Rendered As |
|-------|-------------|-------------|
| `BulletList` | Bulleted list of errors (default) | `<ul><li>...</li></ul>` |
| `List` | Plain list of errors | Line-break separated text |
| `SingleParagraph` | All errors in one paragraph | Space-separated text |

## Examples

### Basic Validation Summary

```razor
<EditForm Model="@model" OnValidSubmit="HandleSubmit">
    <InputText @bind-Value="model.Name" />
    <RequiredFieldValidator ControlToValidate="Name"
                            Text="*"
                            ErrorMessage="Name is required" />

    <InputText @bind-Value="model.Email" />
    <RequiredFieldValidator ControlToValidate="Email"
                            Text="*"
                            ErrorMessage="Email is required" />

    <AspNetValidationSummary HeaderText="Please fix the following errors:" />
    <button type="submit">Submit</button>
</EditForm>

@code {
    private var model = new FormModel();
    
    private void HandleSubmit()
    {
        // Process form submission
    }
}
```

### With ValidationGroup

```razor
@using BlazorWebFormsComponents.Validations

<ValidationGroupProvider>
    <EditForm Model="@model">
        <InputText @bind-Value="model.Name" />
        <RequiredFieldValidator ControlToValidate="Name"
                                Text="*"
                                ErrorMessage="Name is required"
                                ValidationGroup="Personal" />

        <AspNetValidationSummary
            HeaderText="Personal Info Errors:"
            ValidationGroup="Personal"
            DisplayMode="ValidationSummaryDisplayMode.BulletList" />

        <Button Text="Validate" ValidationGroup="Personal" />
    </EditForm>
</ValidationGroupProvider>

@code {
    private var model = new FormModel();
}
```

### Display Modes

```razor
@* Bulleted list (default) *@
<AspNetValidationSummary 
    HeaderText="Errors:"
    DisplayMode="ValidationSummaryDisplayMode.BulletList" />

@* Plain list *@
<AspNetValidationSummary 
    HeaderText="Errors:"
    DisplayMode="ValidationSummaryDisplayMode.List" />

@* Single paragraph *@
<AspNetValidationSummary 
    HeaderText="Errors:"
    DisplayMode="ValidationSummaryDisplayMode.SingleParagraph" />
```

### Styled Summary

```razor
<AspNetValidationSummary
    HeaderText="Errors occurred:"
    CssClass="alert alert-danger"
    ForeColor="WebColor.DarkRed"
    BackColor="WebColor.LightPink"
    BorderColor="WebColor.Red"
    BorderStyle="BorderStyle.Solid"
    BorderWidth="1px"
    DisplayMode="ValidationSummaryDisplayMode.BulletList" />
```

### Multiple Validation Groups

```razor
@using BlazorWebFormsComponents.Validations

<ValidationGroupProvider>
    <EditForm Model="@model" OnValidSubmit="HandleSubmit">
        @* Personal Information Section *@
        <h3>Personal Information</h3>
        <InputText @bind-Value="model.FirstName" />
        <RequiredFieldValidator ControlToValidate="FirstName"
                                ErrorMessage="First name is required"
                                ValidationGroup="Personal" />

        <InputText @bind-Value="model.LastName" />
        <RequiredFieldValidator ControlToValidate="LastName"
                                ErrorMessage="Last name is required"
                                ValidationGroup="Personal" />

        <AspNetValidationSummary
            HeaderText="Personal Info Errors:"
            ValidationGroup="Personal"
            CssClass="alert alert-danger" />

        @* Business Information Section *@
        <h3>Business Information</h3>
        <InputText @bind-Value="model.Company" />
        <RequiredFieldValidator ControlToValidate="Company"
                                ErrorMessage="Company is required"
                                ValidationGroup="Business" />

        <InputText @bind-Value="model.JobTitle" />
        <RequiredFieldValidator ControlToValidate="JobTitle"
                                ErrorMessage="Job title is required"
                                ValidationGroup="Business" />

        <AspNetValidationSummary
            HeaderText="Business Info Errors:"
            ValidationGroup="Business"
            CssClass="alert alert-danger" />

        <Button Text="Validate Personal" ValidationGroup="Personal" />
        <Button Text="Validate Business" ValidationGroup="Business" />
        <Button Text="Submit All" CausesValidation="false" OnClick="HandleSubmit" />
    </EditForm>
</ValidationGroupProvider>

@code {
    private var model = new FormModel();
    
    private void HandleSubmit()
    {
        // Process form submission
    }
}
```

### With Multiple Validators

```razor
<EditForm Model="@model" OnValidSubmit="HandleSubmit">
    <div>
        <Label Text="Email:" AssociatedControlID="email" />
        <InputText id="email" @bind-Value="model.Email" />
        <RequiredFieldValidator ControlToValidate="Email"
                                ErrorMessage="Email is required"
                                Text="*" />
        <RegularExpressionValidator ControlToValidate="Email"
                                    ValidationExpression="^[\w.-]+@[\w.-]+\.\w+$"
                                    ErrorMessage="Email format is invalid"
                                    Text="Invalid format" />
    </div>

    <AspNetValidationSummary
        HeaderText="Please correct the following errors:"
        DisplayMode="ValidationSummaryDisplayMode.BulletList"
        CssClass="validation-summary alert alert-danger" />

    <button type="submit">Submit</button>
</EditForm>

@code {
    private var model = new FormModel();
    
    private void HandleSubmit()
    {
        // Form is valid and ready to submit
    }
}
```

## HTML Output

**Bulleted List (default):**
```html
<!-- Blazor Input -->
<AspNetValidationSummary 
    HeaderText="Errors:"
    DisplayMode="ValidationSummaryDisplayMode.BulletList" />

<!-- Rendered HTML (with errors) -->
<div title="">
    <b>Errors:</b>
    <ul>
        <li>Name is required</li>
        <li>Email format is invalid</li>
    </ul>
</div>
```

**Plain List:**
```html
<!-- Blazor Input -->
<AspNetValidationSummary 
    DisplayMode="ValidationSummaryDisplayMode.List" />

<!-- Rendered HTML -->
<div title="">
    Name is required<br>
    Email format is invalid<br>
</div>
```

## Migration Notes

1. **Change tag name** — Replace `<asp:ValidationSummary>` with `<AspNetValidationSummary>`
2. **Remove `runat="server"`** — Not needed in Blazor
3. **Remove `ShowMessageBox`** — JavaScript alert boxes are not supported in Blazor
4. **Remove `EnableClientScript`** — Not applicable in Blazor (though the parameter exists as a stub)
5. **Update `DisplayMode` enum** — Change `DisplayMode="BulletList"` to `DisplayMode="ValidationSummaryDisplayMode.BulletList"`
6. **Colors use `WebColor` enum** — Instead of color name strings
7. **Must be inside `<EditForm>`** — Requires a cascading `EditContext`

### Migration Example

=== "Web Forms"

    ```html
    <asp:ValidationSummary
        ID="vs1"
        HeaderText="The following errors occurred:"
        DisplayMode="BulletList"
        ForeColor="Red"
        ShowMessageBox="false"
        ShowSummary="true"
        runat="server" />
    ```

=== "Blazor"

    ```razor
    <AspNetValidationSummary
        HeaderText="The following errors occurred:"
        DisplayMode="ValidationSummaryDisplayMode.BulletList"
        ForeColor="WebColor.Red"
        ShowSummary="true" />
    ```

!!! tip "ErrorMessage vs Text"
    Validators have both `Text` (displayed inline next to the field) and `ErrorMessage` (displayed in the ValidationSummary). Set both for the best user experience — a short indicator like `"*"` for `Text` and a descriptive message for `ErrorMessage`.

## See Also

- [RequiredFieldValidator](RequiredFieldValidator.md) — Validate required fields
- [CompareValidator](CompareValidator.md) — Compare values
- [RangeValidator](RangeValidator.md) — Validate value ranges
- [RegularExpressionValidator](RegularExpressionValidator.md) — Pattern validation
- [CustomValidator](CustomValidator.md) — Custom validation logic
- [BaseValidator](BaseValidator.md) — Base class for all validators
- [ControlToValidate](ControlToValidate.md) — Control reference patterns
