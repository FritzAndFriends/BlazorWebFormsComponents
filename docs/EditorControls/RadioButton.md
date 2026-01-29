# RadioButton

The RadioButton component emulates the ASP.NET Web Forms RadioButton control, allowing users to select one option from a group of mutually exclusive choices. Radio buttons with the same `GroupName` are automatically grouped together, enabling browser-native mutual exclusion.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.radiobutton?view=netframework-4.8

## Features Supported in Blazor

- `Text` - Label text displayed next to the radio button
- `GroupName` - Groups radio buttons for mutual exclusion (maps to HTML `name` attribute)
- `Checked` - Boolean state of the radio button
- `TextAlign` - Position of label (Left or Right)
- `Enabled` - Enables or disables the radio button
- `CheckedChanged` - Event callback for two-way binding
- `OnCheckedChanged` - Web Forms compatible event handler
- All style properties (BackColor, ForeColor, CssClass, etc.)

## Web Forms Features NOT Supported

- `AutoPostBack` - Not needed in Blazor. Use the `OnCheckedChanged` event instead.
- `GroupName` with client-side validation - Use Blazor's validation components instead.

## Web Forms Declarative Syntax

```html
<asp:RadioButton 
    ID="string"
    Text="string"
    GroupName="string"
    Checked="True|False"
    TextAlign="Left|Right"
    Enabled="True|False"
    AutoPostBack="True|False"
    OnCheckedChanged="CheckedChanged event handler"
    BackColor="color name|#dddddd"
    ForeColor="color name|#dddddd"
    CssClass="string"
    runat="server" 
/>
```

## Blazor Syntax

```razor
<!-- Simple radio button -->
<RadioButton Text="Option A" />

<!-- Grouped radio buttons (mutually exclusive) -->
<RadioButton Text="Small" GroupName="Size" Checked="@(size == "Small")" 
             OnCheckedChanged="@(() => size = "Small")" />
<RadioButton Text="Medium" GroupName="Size" Checked="@(size == "Medium")" 
             OnCheckedChanged="@(() => size = "Medium")" />
<RadioButton Text="Large" GroupName="Size" Checked="@(size == "Large")" 
             OnCheckedChanged="@(() => size = "Large")" />

<!-- Text alignment -->
<RadioButton Text="Label on left" TextAlign="Enums.TextAlign.Left" GroupName="Align" />
<RadioButton Text="Label on right" TextAlign="Enums.TextAlign.Right" GroupName="Align" />

<!-- Two-way binding -->
<RadioButton Text="Subscribe" Checked="@isSubscribed" 
             CheckedChanged="@((value) => isSubscribed = value)" />

<!-- Disabled radio button -->
<RadioButton Text="Cannot select" Enabled="false" />

<!-- Styled radio button -->
<RadioButton Text="Styled" CssClass="custom-radio" BackColor="LightBlue" GroupName="Styled" />

<!-- Radio button without text (no span wrapper) -->
<RadioButton GroupName="NoLabel" />
```

## Key Behaviors

### GroupName and Mutual Exclusion

Radio buttons with the same `GroupName` share the same HTML `name` attribute, which enables browser-native mutual exclusion. When one radio button in a group is selected, all others in the same group are automatically deselected.

```razor
<!-- These three radio buttons are mutually exclusive -->
<RadioButton Text="Option 1" GroupName="Options" />
<RadioButton Text="Option 2" GroupName="Options" />
<RadioButton Text="Option 3" GroupName="Options" />
```

### HTML Output

**With Text (TextAlign=Right, default):**
```html
<span class="custom-class">
    <input id="guid" type="radio" name="GroupName" checked />
    <label for="guid">Option A</label>
</span>
```

**With Text (TextAlign=Left):**
```html
<span>
    <label for="guid">Option A</label>
    <input id="guid" type="radio" name="GroupName" />
</span>
```

**Without Text:**
```html
<input id="guid" type="radio" name="GroupName" />
```

## Migration Tips

1. **Remove `asp:` prefix and `runat="server"`**: Change `<asp:RadioButton runat="server" />` to `<RadioButton />`
2. **Update GroupName usage**: The property works the same way in Blazor
3. **Replace AutoPostBack**: Use `OnCheckedChanged` event handler instead
4. **Two-way binding**: Use `CheckedChanged` callback for `@bind`-like behavior
5. **Update TextAlign**: Change `TextAlign="Left"` to `TextAlign="Enums.TextAlign.Left"`

## Example Migration

### Before (Web Forms)

```html
<asp:RadioButton ID="rbSmall" Text="Small" GroupName="Size" 
                 AutoPostBack="true" OnCheckedChanged="Size_Changed" runat="server" />
<asp:RadioButton ID="rbMedium" Text="Medium" GroupName="Size" 
                 AutoPostBack="true" OnCheckedChanged="Size_Changed" runat="server" />
<asp:RadioButton ID="rbLarge" Text="Large" GroupName="Size" 
                 AutoPostBack="true" OnCheckedChanged="Size_Changed" runat="server" />
```

### After (Blazor)

```razor
<RadioButton Text="Small" GroupName="Size" Checked="@(selectedSize == "Small")" 
             OnCheckedChanged="@(() => selectedSize = "Small")" />
<RadioButton Text="Medium" GroupName="Size" Checked="@(selectedSize == "Medium")" 
             OnCheckedChanged="@(() => selectedSize = "Medium")" />
<RadioButton Text="Large" GroupName="Size" Checked="@(selectedSize == "Large")" 
             OnCheckedChanged="@(() => selectedSize = "Large")" />

@code {
    private string selectedSize = "Medium";
}
```

## See Also

- [Button](Button.md) - For submit/command buttons
- [LinkButton](LinkButton.md) - For hyperlink-style buttons
- [Validation Controls](../ValidationControls/) - For form validation
