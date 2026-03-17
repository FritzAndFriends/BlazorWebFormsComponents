# MaskedEditExtender

The **MaskedEditExtender** applies an input mask to a target TextBox, restricting and formatting user input according to a mask pattern. It supports number, date, time, and custom masks with configurable prompt characters, input direction, and validation behavior.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/MaskedEditExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the TextBox to apply the mask to
- `Mask` — The mask pattern string
- `MaskType` — The type of mask (None, Number, Date, Time, DateTime)
- `InputDirection` — Direction of text input (LeftToRight, RightToLeft)
- `PromptCharacter` — Character displayed for unfilled mask positions
- `AutoComplete` — Whether to enable browser autocomplete
- `AutoCompleteValue` — Value to use for autocomplete
- `Filtered` — Additional characters allowed beyond the mask definition
- `ClearMaskOnLostFocus` — Remove mask characters when input loses focus
- `ClearTextOnInvalid` — Clear text when it doesn't match the mask on blur
- `AcceptAMPM` — Accept AM/PM designator for time masks
- `AcceptNegative` — How negative values are displayed for number masks
- `ErrorTooltipEnabled` — Show tooltip on validation failure
- `ErrorTooltipCssClass` — CSS class for the error tooltip
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## Mask Characters

| Character | Description |
|---|---|
| `9` | Digit (0–9) |
| `L` | Letter (a–z, A–Z) |
| `$` | Digit or space |
| `C` | Any character |
| `A` | Letter or digit |
| `N` | Digit or letter (same as A) |
| `?` | Any character (optional) |

## MaskType Enum

```csharp
enum MaskType
{
    None = 0,      // Custom mask (default)
    Number = 1,    // Numeric mask
    Date = 2,      // Date mask
    Time = 3,      // Time mask
    DateTime = 4   // Date and time mask
}
```

## InputDirection Enum

```csharp
enum InputDirection
{
    LeftToRight = 0,  // Standard left-to-right input (default)
    RightToLeft = 1   // Right-to-left input (for currency, etc.)
}
```

## AcceptNegative Enum

```csharp
enum AcceptNegative
{
    None = 0,   // No negative values allowed (default)
    Left = 1,   // Negative sign on the left (e.g., -123)
    Right = 2   // Negative sign on the right (e.g., 123-)
}
```

## Web Forms Syntax

```html
<asp:TextBox ID="txtPhone" runat="server" />

<ajaxToolkit:MaskedEditExtender
    ID="mee1"
    runat="server"
    TargetControlID="txtPhone"
    Mask="(999) 999-9999"
    MaskType="None"
    InputDirection="LeftToRight"
    PromptCharacter="_"
    ClearMaskOnLostFocus="true" />
```

## Blazor Migration

```razor
<TextBox ID="txtPhone" />

<MaskedEditExtender
    TargetControlID="txtPhone"
    Mask="(999) 999-9999"
    MaskType="MaskType.None"
    InputDirection="InputDirection.LeftToRight"
    PromptCharacter="_"
    ClearMaskOnLostFocus="true" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Use full enum type names for `MaskType` and `InputDirection`. Everything else stays the same!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the TextBox to apply the mask to |
| `Mask` | `string` | `""` | Mask pattern string (e.g., "999-999-9999" for phone) |
| `MaskType` | `MaskType` | `None` | Type of mask: `None`, `Number`, `Date`, `Time`, `DateTime` |
| `InputDirection` | `InputDirection` | `LeftToRight` | Direction of text input: `LeftToRight` or `RightToLeft` |
| `PromptCharacter` | `string` | `"_"` | Character displayed for unfilled mask positions |
| `AutoComplete` | `bool` | `false` | Whether to enable browser autocomplete on the target input |
| `AutoCompleteValue` | `string` | `""` | Value to use for autocomplete when enabled |
| `Filtered` | `string` | `""` | Additional characters allowed beyond the mask definition |
| `ClearMaskOnLostFocus` | `bool` | `true` | Remove mask characters when input loses focus |
| `ClearTextOnInvalid` | `bool` | `false` | Clear text when it doesn't match the mask on blur |
| `AcceptAMPM` | `bool` | `false` | Accept AM/PM designator for time masks |
| `AcceptNegative` | `AcceptNegative` | `None` | How negative values are displayed: `None`, `Left`, `Right` |
| `ErrorTooltipEnabled` | `bool` | `false` | Show tooltip with error info on validation failure |
| `ErrorTooltipCssClass` | `string` | `""` | CSS class applied to the error tooltip |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Phone Number Mask

```razor
@rendermode InteractiveServer

<TextBox ID="txtPhone" />

<MaskedEditExtender
    TargetControlID="txtPhone"
    Mask="(999) 999-9999"
    ClearMaskOnLostFocus="true" />
```

### Date Mask

```razor
@rendermode InteractiveServer

<TextBox ID="txtDate" />

<MaskedEditExtender
    TargetControlID="txtDate"
    Mask="99/99/9999"
    MaskType="MaskType.Date"
    ClearMaskOnLostFocus="false" />
```

### Currency Mask with Negative Support

```razor
@rendermode InteractiveServer

<TextBox ID="txtAmount" />

<MaskedEditExtender
    TargetControlID="txtAmount"
    Mask="999,999.99"
    MaskType="MaskType.Number"
    InputDirection="InputDirection.RightToLeft"
    AcceptNegative="AcceptNegative.Left" />
```

### Time Mask with AM/PM

```razor
@rendermode InteractiveServer

<TextBox ID="txtTime" />

<MaskedEditExtender
    TargetControlID="txtTime"
    Mask="99:99"
    MaskType="MaskType.Time"
    AcceptAMPM="true" />
```

### Social Security Number Mask

```razor
@rendermode InteractiveServer

<TextBox ID="txtSSN" />

<MaskedEditExtender
    TargetControlID="txtSSN"
    Mask="999-99-9999"
    PromptCharacter="#"
    ClearMaskOnLostFocus="true"
    ClearTextOnInvalid="true"
    ErrorTooltipEnabled="true" />
```

## HTML Output

The MaskedEditExtender produces no HTML itself — it attaches JavaScript behavior to the target TextBox that intercepts keystrokes and enforces the mask pattern.

## JavaScript Interop

The MaskedEditExtender loads `masked-edit-extender.js` as an ES module. JavaScript handles:

- Intercepting keystrokes and enforcing mask patterns
- Positioning the cursor within the mask
- Handling paste operations (stripping invalid characters)
- Displaying prompt characters for unfilled positions
- Clearing/showing the mask on focus/blur
- Error tooltip display on validation failure

## Render Mode Requirements

The MaskedEditExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. The TextBox works as a plain text input without masking.
- **JavaScript disabled:** Same as SSR — TextBox functions without mask enforcement.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:MaskedEditExtender
   + <MaskedEditExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Use full enum type names**
   ```diff
   - MaskType="Number"
   + MaskType="MaskType.Number"
   - InputDirection="RightToLeft"
   + InputDirection="InputDirection.RightToLeft"
   ```

### Before (Web Forms)

```html
<asp:TextBox ID="txtPhone" runat="server" />

<ajaxToolkit:MaskedEditExtender
    ID="mee1"
    TargetControlID="txtPhone"
    Mask="(999) 999-9999"
    ClearMaskOnLostFocus="true"
    runat="server" />
```

### After (Blazor)

```razor
<TextBox ID="txtPhone" />

<MaskedEditExtender
    TargetControlID="txtPhone"
    Mask="(999) 999-9999"
    ClearMaskOnLostFocus="true" />
```

## Best Practices

1. **Choose the right MaskType** — Use `Number`, `Date`, or `Time` for built-in validation
2. **Set ClearMaskOnLostFocus wisely** — `true` shows clean values; `false` shows the mask structure
3. **Use ClearTextOnInvalid** — Prevents partial entries from being submitted
4. **Pair with validators** — Combine with RequiredFieldValidator or CustomValidator for server-side validation
5. **Test with paste** — Ensure pasted values are properly filtered

## Troubleshooting

| Issue | Solution |
|---|---|
| Mask not appearing | Verify `TargetControlID` matches the TextBox's `ID`. Ensure `@rendermode InteractiveServer` is set. |
| Wrong characters accepted | Check `Mask` pattern uses correct mask characters (9, L, $, C, A). |
| Cursor jumping unexpectedly | Verify `InputDirection` matches your intended input flow. |
| Negative values not accepted | Set `AcceptNegative` to `Left` or `Right` and ensure `MaskType` is `Number`. |
| Error tooltip not showing | Set `ErrorTooltipEnabled="true"` and optionally provide `ErrorTooltipCssClass`. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [FilteredTextBoxExtender](FilteredTextBoxExtender.md) — Character type filtering
- [CalendarExtender](CalendarExtender.md) — Date picker for date fields
- [TextBox Component](../EditorControls/TextBox.md) — The TextBox control
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
