# ValidatorCalloutExtender

The **ValidatorCalloutExtender** enhances ASP.NET validators by displaying validation messages in a callout or tooltip bubble instead of inline error text. It attaches to a validator control and displays its error message in a styled popup positioned near the invalid field, with an optional close button and warning icon.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/ValidatorCalloutExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the validator control to enhance
- `Width` — Callout width in pixels
- `HighlightCssClass` — CSS class to apply to invalid fields
- `WarningIconImageUrl` — URL to warning icon image
- `CloseImageUrl` — URL to close button image
- `CssClass` — CSS class for the callout container
- `PopupPosition` — Position of the callout (Left, Right, Top, Bottom, Center, TopLeft, TopRight, BottomLeft, BottomRight)
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## PopupPosition Enum

Controls where the callout appears:

```csharp
enum PopupPosition
{
    Left = 0,
    Right = 1,
    Top = 2,
    Bottom = 3,
    Center = 4,
    TopLeft = 5,
    TopRight = 6,
    BottomLeft = 7,
    BottomRight = 8
}
```

## Web Forms Syntax

```html
<asp:TextBox ID="txtEmail" runat="server" />

<asp:RequiredFieldValidator
    ID="valEmail"
    ControlToValidate="txtEmail"
    ErrorMessage="Email is required"
    runat="server" />

<ajaxToolkit:ValidatorCalloutExtender
    ID="val1"
    runat="server"
    TargetControlID="valEmail"
    PopupPosition="TopRight"
    Width="250"
    HighlightCssClass="field-error"
    WarningIconImageUrl="~/images/warning.png"
    CloseImageUrl="~/images/close.png" />
```

## Blazor Migration

```razor
<TextBox ID="txtEmail" />

<RequiredFieldValidator
    ID="valEmail"
    ControlToValidate="txtEmail"
    ErrorMessage="Email is required" />

<ValidatorCalloutExtender
    TargetControlID="valEmail"
    PopupPosition="PopupPosition.TopRight"
    Width="250"
    HighlightCssClass="field-error"
    WarningIconImageUrl="~/images/warning.png"
    CloseImageUrl="~/images/close.png" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Use enum type names for positions!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the validator control to enhance |
| `Width` | `int` | `250` | Callout width in pixels |
| `HighlightCssClass` | `string` | `""` | CSS class to apply to the invalid input field |
| `WarningIconImageUrl` | `string` | `""` | URL to the warning icon image displayed in the callout |
| `CloseImageUrl` | `string` | `""` | URL to the close button image (if provided, close button appears) |
| `CssClass` | `string` | `""` | CSS class applied to the callout container |
| `PopupPosition` | `PopupPosition` | `BottomLeft` | Position of the callout relative to the invalid field |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Basic Validator Callout

```razor
@rendermode InteractiveServer

<form>
    <div>
        <label>Email:</label>
        <TextBox ID="txtEmail" type="email" style="width: 200px;" />
        
        <RequiredFieldValidator
            ID="valEmail"
            ControlToValidate="txtEmail"
            ErrorMessage="Email address is required" />
        
        <ValidatorCalloutExtender
            TargetControlID="valEmail"
            PopupPosition="PopupPosition.TopRight"
            Width="250" />
    </div>
    
    <button type="submit" class="btn btn-primary">Submit</button>
</form>
```

### Callout with Custom Styling

```razor
@rendermode InteractiveServer

<form>
    <div>
        <label>Username:</label>
        <TextBox ID="txtUsername" style="width: 200px;" />
        
        <RequiredFieldValidator
            ID="valUsername"
            ControlToValidate="txtUsername"
            ErrorMessage="Username is required" />
        
        <ValidatorCalloutExtender
            TargetControlID="valUsername"
            PopupPosition="PopupPosition.BottomRight"
            Width="300"
            CssClass="error-callout"
            HighlightCssClass="field-with-error" />
    </div>
    
    <button type="submit" class="btn btn-primary">Register</button>
</form>

<style>
    .error-callout {
        background-color: #ffebee;
        border: 1px solid #ef5350;
        border-radius: 4px;
        box-shadow: 0 2px 8px rgba(239, 83, 80, 0.2);
    }
    
    .field-with-error {
        background-color: #ffcdd2;
        border-color: #ef5350;
    }
</style>
```

### Callout with Icons

```razor
@rendermode InteractiveServer

<form>
    <div>
        <label>Password:</label>
        <TextBox ID="txtPassword" type="password" style="width: 200px;" />
        
        <RequiredFieldValidator
            ID="valPassword"
            ControlToValidate="txtPassword"
            ErrorMessage="Password is required" />
        
        <ValidatorCalloutExtender
            TargetControlID="valPassword"
            PopupPosition="PopupPosition.TopRight"
            Width="280"
            WarningIconImageUrl="~/images/warning-icon.png"
            CloseImageUrl="~/images/close-icon.png"
            HighlightCssClass="invalid-field" />
    </div>
    
    <button type="submit" class="btn btn-primary">Submit</button>
</form>

<style>
    .invalid-field {
        background-color: #fff3cd;
        border: 2px solid #ffc107;
    }
</style>
```

### Multiple Validators with Callouts

```razor
@rendermode InteractiveServer

<form>
    <div style="margin-bottom: 15px;">
        <label>First Name:</label>
        <TextBox ID="txtFirstName" style="width: 200px;" />
        
        <RequiredFieldValidator
            ID="valFirstName"
            ControlToValidate="txtFirstName"
            ErrorMessage="First name is required" />
        
        <ValidatorCalloutExtender
            TargetControlID="valFirstName"
            PopupPosition="PopupPosition.BottomRight"
            Width="250" />
    </div>
    
    <div style="margin-bottom: 15px;">
        <label>Email:</label>
        <TextBox ID="txtEmail2" type="email" style="width: 200px;" />
        
        <RequiredFieldValidator
            ID="valEmail2"
            ControlToValidate="txtEmail2"
            ErrorMessage="Email is required" />
        
        <ValidatorCalloutExtender
            TargetControlID="valEmail2"
            PopupPosition="PopupPosition.BottomRight"
            Width="250" />
    </div>
    
    <button type="submit" class="btn btn-primary">Submit</button>
</form>
```

## HTML Output

The ValidatorCalloutExtender creates a styled callout `<div>` that appears near the validated field when validation fails. The callout includes the validator's error message, optional icon, and optional close button.

## JavaScript Interop

The ValidatorCalloutExtender loads `validator-callout-extender.js` as an ES module. JavaScript handles:

- Intercepting validator failure events
- Creating and positioning the callout popup
- Displaying the validator's error message
- Applying CSS classes to the invalid field
- Displaying warning icon if URL provided
- Close button functionality if URL provided
- Repositioning callout to stay within viewport
- Removing callout when validation passes

## Render Mode Requirements

The ValidatorCalloutExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. Validator displays inline errors.
- **JavaScript disabled:** Same as SSR — Callout doesn't appear; inline error display is used instead.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:ValidatorCalloutExtender
   + <ValidatorCalloutExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Use enum types for positions**
   ```diff
   - PopupPosition="TopRight"
   + PopupPosition="PopupPosition.TopRight"
   ```

### Before (Web Forms)

```html
<asp:TextBox ID="txtEmail" runat="server" />

<asp:RequiredFieldValidator
    ID="valEmail"
    ControlToValidate="txtEmail"
    ErrorMessage="Email is required"
    runat="server" />

<ajaxToolkit:ValidatorCalloutExtender
    ID="val1"
    TargetControlID="valEmail"
    PopupPosition="TopRight"
    Width="250"
    runat="server" />
```

### After (Blazor)

```razor
<TextBox ID="txtEmail" />

<RequiredFieldValidator
    ID="valEmail"
    ControlToValidate="txtEmail"
    ErrorMessage="Email is required" />

<ValidatorCalloutExtender
    TargetControlID="valEmail"
    PopupPosition="PopupPosition.TopRight"
    Width="250" />
```

## Best Practices

1. **Keep messages concise** — Error messages should be brief but informative
2. **Choose position carefully** — Avoid positions that would be cut off by viewport edges
3. **Use consistent styling** — Apply consistent CSS classes across all callouts
4. **Provide icons** — Warning icons help users quickly identify errors
5. **Allow closing** — Consider providing a close button with `CloseImageUrl`
6. **Highlight invalid fields** — Use `HighlightCssClass` to make invalid inputs obvious
7. **Test viewport positioning** — Ensure callouts stay visible in different screen sizes

## Troubleshooting

| Issue | Solution |
|---|---|
| Callout not appearing | Verify `TargetControlID` matches the validator's ID. Ensure `@rendermode InteractiveServer` is set. |
| Callout positioned off-screen | Try a different `PopupPosition` value (e.g., BottomLeft instead of TopRight). |
| Icon not showing | Verify `WarningIconImageUrl` points to a valid image file. Check browser console for 404 errors. |
| Multiple callouts overlap | Adjust `Width` or use different positions for different validators. |
| Close button not working | Ensure `CloseImageUrl` points to a valid image and the image is loaded correctly. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [TextBox Component](../EditorControls/TextBox.md) — The TextBox control
- RequiredFieldValidator — Validator control (documentation coming soon)
- [PasswordStrength](PasswordStrength.md) — Password feedback extender
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
