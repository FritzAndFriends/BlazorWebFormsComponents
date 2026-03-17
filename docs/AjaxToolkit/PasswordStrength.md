# PasswordStrength

The **PasswordStrength** extender displays a visual indicator of password strength as the user types in a password field. It evaluates passwords against configurable character requirements (length, uppercase, lowercase, numbers, symbols) and displays feedback through text labels or a visual bar indicator.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/PasswordStrength

## Features Supported in Blazor

- `TargetControlID` — ID of the password TextBox to monitor
- `DisplayPosition` — Where to show the strength indicator (RightSide, LeftSide, AboveRight, AboveLeft, BelowRight, BelowLeft)
- `StrengthIndicatorType` — Text labels or BarIndicator display
- `PreferredPasswordLength` — Target minimum length
- `MinimumNumericCharacters` — Required numeric characters
- `MinimumSymbolCharacters` — Required symbol characters
- `MinimumUpperCaseCharacters` — Required uppercase letters
- `MinimumLowerCaseCharacters` — Required lowercase letters
- `RequiresUpperAndLowerCaseCharacters` — Must contain both upper and lower
- `TextStrengthDescriptions` — Semicolon-delimited strength labels
- `StrengthStyles` — CSS classes for each strength level
- `BarBorderCssClass` — CSS class for bar indicator border
- `TextCssClass` — CSS class for text display
- `HelpHandleCssClass` — CSS class for help icon
- `HelpHandlePosition` — Where to show the help handle

## DisplayPosition Enum

Controls where the indicator displays:

```csharp
enum DisplayPosition
{
    RightSide = 0,
    LeftSide = 1,
    AboveRight = 2,
    AboveLeft = 3,
    BelowRight = 4,
    BelowLeft = 5
}
```

## StrengthIndicatorType Enum

Controls the display style:

```csharp
enum StrengthIndicatorType
{
    Text = 0,        // Text labels
    BarIndicator = 1 // Visual progress bar
}
```

## Web Forms Syntax

```html
<asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />

<ajaxToolkit:PasswordStrength
    ID="pwd1"
    runat="server"
    TargetControlID="txtPassword"
    DisplayPosition="RightSide"
    StrengthIndicatorType="BarIndicator"
    PreferredPasswordLength="10"
    MinimumNumericCharacters="1"
    MinimumSymbolCharacters="1"
    MinimumUpperCaseCharacters="1"
    TextStrengthDescriptions="Very Poor;Weak;Average;Strong;Excellent" />
```

## Blazor Migration

```razor
<TextBox ID="txtPassword" type="password" />

<PasswordStrength
    TargetControlID="txtPassword"
    DisplayPosition="DisplayPosition.RightSide"
    StrengthIndicatorType="StrengthIndicatorType.BarIndicator"
    PreferredPasswordLength="10"
    MinimumNumericCharacters="1"
    MinimumSymbolCharacters="1"
    MinimumUpperCaseCharacters="1"
    TextStrengthDescriptions="Very Poor;Weak;Average;Strong;Excellent" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Use enum type names for positions and indicator types!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the password TextBox to monitor |
| `DisplayPosition` | `DisplayPosition` | `RightSide` | Where to display the strength indicator |
| `StrengthIndicatorType` | `StrengthIndicatorType` | `Text` | Text labels or BarIndicator display |
| `PreferredPasswordLength` | `int` | `10` | Target minimum password length |
| `MinimumNumericCharacters` | `int` | `0` | Minimum required numeric characters (0-9) |
| `MinimumSymbolCharacters` | `int` | `0` | Minimum required symbol characters |
| `MinimumUpperCaseCharacters` | `int` | `0` | Minimum required uppercase letters |
| `MinimumLowerCaseCharacters` | `int` | `0` | Minimum required lowercase letters |
| `RequiresUpperAndLowerCaseCharacters` | `bool` | `false` | Must contain both uppercase and lowercase |
| `TextStrengthDescriptions` | `string` | `""` | Semicolon-delimited strength labels (e.g., "Very Poor;Weak;Average;Strong;Excellent") |
| `StrengthStyles` | `string` | `""` | Semicolon-delimited CSS classes, one per strength level |
| `BarBorderCssClass` | `string` | `""` | CSS class for bar indicator border |
| `TextCssClass` | `string` | `""` | CSS class for text display element |
| `HelpHandleCssClass` | `string` | `""` | CSS class for help icon |
| `HelpHandlePosition` | `DisplayPosition` | `RightSide` | Where to display the help handle |

## Usage Examples

### Basic Password Strength with Text

```razor
@rendermode InteractiveServer

<div>
    <label>Password:</label>
    <TextBox ID="txtPass" type="password" style="width: 200px;" />
    
    <PasswordStrength
        TargetControlID="txtPass"
        DisplayPosition="DisplayPosition.RightSide"
        StrengthIndicatorType="StrengthIndicatorType.Text"
        PreferredPasswordLength="8"
        MinimumNumericCharacters="1"
        TextStrengthDescriptions="Very Weak;Weak;Good;Strong;Very Strong" />
</div>
```

### Password Strength with Bar Indicator

```razor
@rendermode InteractiveServer

<div>
    <label>New Password:</label>
    <TextBox ID="txtNewPassword" type="password" style="width: 250px;" />
    
    <PasswordStrength
        TargetControlID="txtNewPassword"
        DisplayPosition="DisplayPosition.BelowRight"
        StrengthIndicatorType="StrengthIndicatorType.BarIndicator"
        PreferredPasswordLength="12"
        MinimumNumericCharacters="1"
        MinimumSymbolCharacters="1"
        MinimumUpperCaseCharacters="1"
        MinimumLowerCaseCharacters="1"
        BarBorderCssClass="strength-bar-border"
        StrengthStyles="very-weak;weak;fair;good;excellent" />
</div>

<style>
    .strength-bar-border {
        border: 1px solid #ccc;
        height: 20px;
        margin-top: 5px;
        border-radius: 3px;
    }
    
    .very-weak { background-color: #d32f2f; }
    .weak { background-color: #f57c00; }
    .fair { background-color: #fbc02d; }
    .good { background-color: #689f38; }
    .excellent { background-color: #00796b; }
</style>
```

### Strict Password Requirements

```razor
@rendermode InteractiveServer

<form>
    <div style="margin-bottom: 20px;">
        <label>Create Password (minimum 14 characters, mixed case, numbers, and symbols):</label>
        <TextBox ID="txtStrictPass" type="password" style="width: 300px;" />
        
        <PasswordStrength
            TargetControlID="txtStrictPass"
            DisplayPosition="DisplayPosition.BelowRight"
            StrengthIndicatorType="StrengthIndicatorType.BarIndicator"
            PreferredPasswordLength="14"
            MinimumNumericCharacters="2"
            MinimumSymbolCharacters="2"
            MinimumUpperCaseCharacters="1"
            MinimumLowerCaseCharacters="1"
            RequiresUpperAndLowerCaseCharacters="true"
            TextStrengthDescriptions="Insufficient;Weak;Fair;Good;Strong;Very Strong"
            StrengthStyles="strength-0;strength-1;strength-2;strength-3;strength-4;strength-5" />
    </div>
    
    <button type="submit" class="btn btn-primary">Register</button>
</form>

<style>
    .strength-0 { background-color: #d32f2f; width: 0%; }
    .strength-1 { background-color: #f57c00; width: 20%; }
    .strength-2 { background-color: #fbc02d; width: 40%; }
    .strength-3 { background-color: #abb52e; width: 60%; }
    .strength-4 { background-color: #7cb342; width: 80%; }
    .strength-5 { background-color: #00796b; width: 100%; }
</style>
```

### Relaxed Password Requirements

```razor
@rendermode InteractiveServer

<TextBox ID="txtSimple" type="password" style="width: 200px;" />

<PasswordStrength
    TargetControlID="txtSimple"
    DisplayPosition="DisplayPosition.RightSide"
    StrengthIndicatorType="StrengthIndicatorType.Text"
    PreferredPasswordLength="6"
    TextStrengthDescriptions="Too Short;Weak;Fair;Good;Strong"
    TextCssClass="strength-label" />

<style>
    .strength-label {
        font-weight: bold;
        margin-left: 10px;
    }
</style>
```

## HTML Output

The PasswordStrength extender produces a strength indicator element (typically a `<div>`) positioned near the password field, displaying either text labels or a visual bar.

## JavaScript Interop

The PasswordStrength loads `password-strength.js` as an ES module. JavaScript handles:

- Monitoring password field input events
- Evaluating password against character requirements
- Calculating strength score based on weights
- Updating indicator display in real-time
- Managing text label visibility and styling
- Updating bar indicator width
- Positioning indicator relative to the password field

## Render Mode Requirements

The PasswordStrength requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. Password field works normally without feedback.
- **JavaScript disabled:** Same as SSR — No strength indicator appears.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:PasswordStrength
   + <PasswordStrength
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Use enum types for positions and indicator types**
   ```diff
   - DisplayPosition="RightSide"
   + DisplayPosition="DisplayPosition.RightSide"
   ```

### Before (Web Forms)

```html
<asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />

<ajaxToolkit:PasswordStrength
    ID="pwd1"
    TargetControlID="txtPassword"
    DisplayPosition="RightSide"
    StrengthIndicatorType="BarIndicator"
    PreferredPasswordLength="8"
    MinimumNumericCharacters="1"
    TextStrengthDescriptions="Weak;Fair;Good;Strong"
    runat="server" />
```

### After (Blazor)

```razor
<TextBox ID="txtPassword" type="password" />

<PasswordStrength
    TargetControlID="txtPassword"
    DisplayPosition="DisplayPosition.RightSide"
    StrengthIndicatorType="StrengthIndicatorType.BarIndicator"
    PreferredPasswordLength="8"
    MinimumNumericCharacters="1"
    TextStrengthDescriptions="Weak;Fair;Good;Strong" />
```

## Best Practices

1. **Balance security with usability** — Don't require overly strict passwords that frustrate users
2. **Show requirements upfront** — Consider displaying password requirements above the field
3. **Use BarIndicator for visual feedback** — Bars are more intuitive than text for strength
4. **Provide clear descriptions** — Use labels like "Strong" vs "Very Strong" that users understand
5. **Position wisely** — Place indicator where it won't obscure other form fields
6. **Test requirements** — Verify your character rules are achievable and reasonable
7. **Combine with validation** — Use server-side validation in addition to client feedback

## Troubleshooting

| Issue | Solution |
|---|---|
| Indicator not appearing | Verify `TargetControlID` matches the TextBox's ID. Ensure `@rendermode InteractiveServer` is set. |
| Bar indicator not filling | Define `StrengthStyles` with CSS classes and ensure corresponding styles are in your stylesheet. |
| Descriptions not showing | If using `StrengthIndicatorType.Text`, provide `TextStrengthDescriptions` separated by semicolons. |
| Requirements too strict | Adjust `PreferredPasswordLength` and character requirements to match your security policy. |
| Indicator in wrong position | Use a different `DisplayPosition` value. Try AboveRight, BelowRight, or LeftSide. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [TextBox Component](../EditorControls/TextBox.md) — The TextBox control
- [ValidatorCalloutExtender](ValidatorCalloutExtender.md) — Enhanced validation feedback
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
