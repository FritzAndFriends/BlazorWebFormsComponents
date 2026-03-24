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

## Web Forms Features NOT Supported

- **ShowMessageBox** — JavaScript alert boxes are not supported; use inline display instead
- **EnableClientScript** — Blazor uses its own validation model
- **ShowValidationErrors** — Not implemented separately; controlled via `ShowSummary`

## Syntax Comparison

=== "Web Forms"

    ```html
    <asp:ValidationSummary
        DisplayMode="BulletList|List|SingleParagraph"
        EnableClientScript="True|False"
        Enabled="True|False"
        ForeColor="color name|#dddddd"
        HeaderText="string"
        ID="string"
        runat="server"
        ShowMessageBox="True|False"
        ShowSummary="True|False"
        ValidationGroup="string"
        Visible="True|False"
    />
    ```

=== "Blazor"

    ```razor
    <AspNetValidationSummary
        DisplayMode="ValidationSummaryDisplayMode.BulletList"
        HeaderText="Please correct the following errors:"
        ShowSummary="true"
        ValidationGroup="MyGroup"
        CssClass="validation-summary"
        ForeColor="WebColor.Red" />
    ```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DisplayMode` | `ValidationSummaryDisplayMode` | `BulletList` | How errors are rendered |
| `HeaderText` | `string` | `null` | Text shown above the error list |
| `ShowSummary` | `bool` | `true` | Whether to display the summary |
| `ValidationGroup` | `string` | `null` | Filter errors by validation group |
| `CssClass` | `string` | `null` | CSS class for the container |
| `ForeColor` | `WebColor` | `Red` | Text color of error messages |
| `Enabled` | `bool` | `true` | Enable or disable the component |
| `Visible` | `bool` | `true` | Show or hide the component |

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
```

### Display Modes

```razor
@* Bulleted list (default) *@
<AspNetValidationSummary DisplayMode="ValidationSummaryDisplayMode.BulletList" />

@* Plain list *@
<AspNetValidationSummary DisplayMode="ValidationSummaryDisplayMode.List" />

@* Single paragraph *@
<AspNetValidationSummary DisplayMode="ValidationSummaryDisplayMode.SingleParagraph" />
```

### Styled Summary

```razor
<AspNetValidationSummary
    HeaderText="Errors:"
    CssClass="alert alert-danger"
    ForeColor="WebColor.DarkRed"
    DisplayMode="ValidationSummaryDisplayMode.BulletList" />
```

## Migration Notes

1. **Change tag name** — Replace `<asp:ValidationSummary>` with `<AspNetValidationSummary>`
2. **Remove `runat="server"`** — Not needed in Blazor
3. **Remove `ShowMessageBox`** — JavaScript alert boxes are not supported in Blazor
4. **Remove `EnableClientScript`** — Not applicable in Blazor
5. **Update `DisplayMode` enum** — Change `DisplayMode="BulletList"` to `DisplayMode="ValidationSummaryDisplayMode.BulletList"`
6. **`HeaderText` and `ForeColor` work the same** — These properties transfer directly

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
- [ControlToValidate](ControlToValidate.md) — Control reference patterns
