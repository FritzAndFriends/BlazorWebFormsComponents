# ModelErrorMessage

The ModelErrorMessage component displays model state error messages for a specific key, matching the ASP.NET Web Forms `<asp:ModelErrorMessage>` control. It renders a `<span>` containing the error text when errors exist for the given key, and nothing when there are no errors.

Original Microsoft documentation for the ASP<span></span>.NET ModelErrorMessage control is at: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.modelerrormessage?view=netframework-4.8

## Features Supported in Blazor

- `ModelStateKey` — The key in the model state / EditContext to display errors for
- `AssociatedControlID` — The ID of the associated input control, used with `SetFocusOnError`
- `SetFocusOnError` — When `true`, focuses the associated control when an error is displayed
- `CssClass` — CSS class applied to the rendered `<span>`
- `Style` — Inline style applied to the rendered `<span>`
- `ToolTip` — Title attribute on the rendered `<span>`
- `Enabled` — Enable or disable the component
- `Visible` — Show or hide the component

## Web Forms Features NOT Supported

- `IsValid` / `Validate()` — ModelErrorMessage is a display-only control; validation logic is handled by the EditContext
- `HeaderText` / `HeaderStyle` — Not part of the original ModelErrorMessage control

## Web Forms Declarative Syntax

```html
<asp:ModelErrorMessage
    runat="server"
    ModelStateKey="string"
    AssociatedControlID="string"
    CssClass="string"
    SetFocusOnError="True|False"
    ID="string"
    Visible="True|False"
/>
```

## Blazor Syntax

```razor
<ModelErrorMessage
    ModelStateKey="NewPassword"
    AssociatedControlID="password"
    CssClass="text-danger"
    SetFocusOnError="true" />
```

## HTML Output

When errors exist for the specified `ModelStateKey`, ModelErrorMessage renders:

```html
<span class="text-danger">The error message text</span>
```

When there are no errors, nothing is rendered.

## Usage Notes

ModelErrorMessage requires a cascading `EditContext` — it must be placed inside an `<EditForm>`. It listens for validation state changes and automatically updates when errors are added or removed.

### Adding Errors via ValidationMessageStore

In Web Forms, model state errors are added in code-behind via `ModelState.AddModelError()`. In Blazor, the equivalent pattern uses `ValidationMessageStore` on the `EditContext`:

```razor
@using BlazorWebFormsComponents.Validations

<EditForm Model="@model" OnValidSubmit="HandleSubmit">
    <input id="password" type="password" @bind="model.NewPassword" />

    <ModelErrorMessage
        ModelStateKey="NewPassword"
        AssociatedControlID="password"
        CssClass="text-danger"
        SetFocusOnError="true" />

    <Button Text="Submit" />
</EditForm>

@code {
    private MyModel model = new();
    private EditContext editContext;
    private ValidationMessageStore messageStore;

    protected override void OnInitialized()
    {
        editContext = new EditContext(model);
        messageStore = new ValidationMessageStore(editContext);
    }

    private void HandleSubmit()
    {
        messageStore.Clear();

        if (string.IsNullOrEmpty(model.NewPassword))
        {
            // Equivalent to ModelState.AddModelError("NewPassword", "...")
            messageStore.Add(editContext.Field("NewPassword"),
                "Password is required.");
            editContext.NotifyValidationStateChanged();
        }
    }
}
```

### Multiple Errors

If multiple errors are added for the same key, they are displayed separated by `<br>` tags within a single `<span>`.

## Migration Notes

Migration from Web Forms to Blazor is straightforward:

1. Remove the `asp:` prefix and `runat="server"` attribute
2. Wrap the form in an `<EditForm>` (replacing `<form runat="server">`)
3. Replace `ModelState.AddModelError()` calls with `ValidationMessageStore.Add()`

### Before (Web Forms)

```html
<asp:ModelErrorMessage runat="server"
    ModelStateKey="NewPassword"
    AssociatedControlID="password"
    CssClass="text-danger"
    SetFocusOnError="true" />
```

### After (Blazor)

```razor
<ModelErrorMessage
    ModelStateKey="NewPassword"
    AssociatedControlID="password"
    CssClass="text-danger"
    SetFocusOnError="true" />
```

!!! tip "Code-behind migration"
    The biggest change is in the code-behind: Web Forms uses `ModelState.AddModelError(key, message)` while Blazor uses `ValidationMessageStore.Add(editContext.Field(key), message)` followed by `editContext.NotifyValidationStateChanged()`.

## See Also

- [ValidationSummary](ValidationSummary.md)
- [RequiredFieldValidator](RequiredFieldValidator.md)
- [ControlToValidate](ControlToValidate.md)
- [Getting Started with Migration](../Migration/readme.md)
