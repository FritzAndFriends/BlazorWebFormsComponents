# FilteredTextBoxExtender

The **FilteredTextBoxExtender** restricts input in a TextBox to specified character types. It filters keystrokes in real-time and automatically strips invalid characters on paste operations, ensuring only allowed characters appear in the input.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/FilteredTextBoxExtender

## Features Supported in Blazor

- `FilterType` — Combination of Numbers, UppercaseLetters, LowercaseLetters, and Custom
- `ValidChars` — Additional allowed characters when `FilterType` includes Custom
- `InvalidChars` — Characters to explicitly block
- `FilterMode` — Whether to use `ValidChars` (whitelist) or `InvalidChars` (blacklist)
- `FilterInterval` — Milliseconds between filter checks (default: 250ms)
- `TargetControlID` — ID of the TextBox to enhance
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## FilterType and FilterMode Enums

### FilterType (Flags Enum)

Character types can be combined using the bitwise OR operator (`|`):

```csharp
enum FilterType
{
    Custom = 0,              // No predefined characters
    Numbers = 1,             // Digits 0-9
    LowercaseLetters = 2,    // a-z
    UppercaseLetters = 4     // A-Z
}
```

### FilterMode

Determines whether ValidChars or InvalidChars takes precedence:

```csharp
enum FilterMode
{
    ValidChars = 0,    // Whitelist: only these characters allowed
    InvalidChars = 1   // Blacklist: these characters blocked
}
```

## Web Forms Syntax

```html
<asp:TextBox ID="txtPhone" runat="server" />

<ajaxToolkit:FilteredTextBoxExtender 
    ID="ftb"
    TargetControlID="txtPhone"
    FilterType="Numbers"
    runat="server" />

<!-- Phone number with dashes and parentheses -->
<asp:TextBox ID="txtPhoneFormatted" runat="server" />

<ajaxToolkit:FilteredTextBoxExtender 
    ID="ftb2"
    TargetControlID="txtPhoneFormatted"
    FilterType="Numbers, Custom"
    ValidChars="()-"
    runat="server" />
```

## Blazor Migration

```razor
<TextBox @bind-Text="phone" ID="txtPhone" />

<FilteredTextBoxExtender 
    TargetControlID="txtPhone"
    FilterType="FilterType.Numbers" />

<!-- Phone number with dashes and parentheses -->
<TextBox @bind-Text="phoneFormatted" ID="txtPhoneFormatted" />

<FilteredTextBoxExtender 
    TargetControlID="txtPhoneFormatted"
    FilterType="FilterType.Numbers | FilterType.Custom"
    ValidChars="()-" />

@code {
    private string phone = "";
    private string phoneFormatted = "";
}
```

**Migration is simple:** Just remove the `ajaxToolkit:` prefix and convert enum values to `FilterType.EnumName` syntax.

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the TextBox control to enhance |
| `FilterType` | `FilterType` | `Custom` | Types of characters to allow (use `\|` to combine) |
| `ValidChars` | `string` | `""` | Additional characters allowed when FilterType includes Custom |
| `InvalidChars` | `string` | `""` | Characters to explicitly block (only used with InvalidChars mode) |
| `FilterMode` | `FilterMode` | `ValidChars` | Whether ValidChars or InvalidChars takes precedence |
| `FilterInterval` | `int` | `250` | Milliseconds between filter checks |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Numbers Only

```razor
@rendermode InteractiveServer

<TextBox @bind-Text="quantity" ID="txtQty" placeholder="Enter quantity" />

<FilteredTextBoxExtender 
    TargetControlID="txtQty"
    FilterType="FilterType.Numbers" />

@code {
    private string quantity = "";
}
```

### Uppercase Letters Only

```razor
@rendermode InteractiveServer

<TextBox @bind-Text="code" ID="txtCode" MaxLength="10" />

<FilteredTextBoxExtender 
    TargetControlID="txtCode"
    FilterType="FilterType.UppercaseLetters" />

@code {
    private string code = "";
}
```

### Phone Number (Numbers, Dashes, Parentheses)

```razor
@rendermode InteractiveServer

<TextBox @bind-Text="phone" ID="txtPhone" placeholder="(555) 123-4567" />

<FilteredTextBoxExtender 
    TargetControlID="txtPhone"
    FilterType="FilterType.Numbers | FilterType.Custom"
    ValidChars="()-" />

@code {
    private string phone = "";
}
```

### Alphanumeric (Letters and Numbers)

```razor
@rendermode InteractiveServer

<TextBox @bind-Text="username" ID="txtUsername" placeholder="Your username" />

<FilteredTextBoxExtender 
    TargetControlID="txtUsername"
    FilterType="FilterType.Numbers | FilterType.UppercaseLetters | FilterType.LowercaseLetters" />

@code {
    private string username = "";
}
```

### Currency (Numbers and Decimal Point)

```razor
@rendermode InteractiveServer

<TextBox @bind-Text="price" ID="txtPrice" placeholder="0.00" />

<FilteredTextBoxExtender 
    TargetControlID="txtPrice"
    FilterType="FilterType.Numbers | FilterType.Custom"
    ValidChars="." />

@code {
    private string price = "";
}
```

### Product SKU (Alphanumeric and Hyphens)

```razor
@rendermode InteractiveServer

<TextBox @bind-Text="sku" ID="txtSKU" placeholder="SKU-1234-5678" />

<FilteredTextBoxExtender 
    TargetControlID="txtSKU"
    FilterType="FilterType.Numbers | FilterType.UppercaseLetters | FilterType.Custom"
    ValidChars="-" />

@code {
    private string sku = "";
}
```

### Block Certain Characters (Blacklist)

Using `InvalidChars` mode to block specific characters:

```razor
@rendermode InteractiveServer

<TextBox @bind-Text="filename" ID="txtFileName" placeholder="Enter filename" />

<FilteredTextBoxExtender 
    TargetControlID="txtFileName"
    FilterType="FilterType.Numbers | FilterType.UppercaseLetters | FilterType.LowercaseLetters"
    InvalidChars="<>:/\|?*"
    FilterMode="FilterMode.InvalidChars" />

@code {
    private string filename = "";
}
```

### Slow Filter Interval (Performance Tuning)

For high-performance scenarios, increase the filter interval:

```razor
@rendermode InteractiveServer

<TextBox @bind-Text="data" ID="txtData" />

<FilteredTextBoxExtender 
    TargetControlID="txtData"
    FilterType="FilterType.Numbers"
    FilterInterval="500" />

@code {
    private string data = "";
}
```

## HTML Output

The FilteredTextBoxExtender produces no HTML — it only attaches JavaScript behavior to the target TextBox.

**Before (TextBox markup):**
```html
<input type="text" id="txtPhone" value="" />
```

**After (with FilteredTextBoxExtender attached):**
```html
<input type="text" id="txtPhone" value="" />
<!-- JavaScript behavior injected; user cannot type invalid characters -->
```

## Filtering Behavior

### Real-Time Keystroke Filtering

- Invalid characters are prevented at keystroke
- User never sees the invalid character in the input
- Works on keyboard input, arrow keys, delete, backspace, etc.

### Paste Handling

- When user pastes content, the extender automatically strips invalid characters
- Only valid characters from the paste are inserted
- Example: Pasting "abc123def" into a Numbers-only field results in "123"

### Backspace and Delete

- Backspace and Delete work normally
- They are not subject to filtering
- Allows users to correct mistakes naturally

## Character Set Combinations

### Common Patterns

| Use Case | FilterType | ValidChars |
|---|---|---|
| Numbers only | `Numbers` | - |
| Integers and decimals | `Numbers` | `.` (with Custom) |
| Currency (USD) | `Numbers` | `.-$` (with Custom) |
| Phone (US) | `Numbers` | `()-+` (with Custom) |
| Email | `LowercaseLetters \| Numbers` | `.@-_` (with Custom) |
| Username | `UppercaseLetters \| LowercaseLetters \| Numbers` | `-_` (with Custom) |
| ZIP code | `Numbers` | `-` (with Custom, for ZIP+4) |
| License plate | `UppercaseLetters \| Numbers` | `-` (with Custom) |

### Custom Patterns

```razor
@* Portuguese characters *@
<FilteredTextBoxExtender 
    TargetControlID="txtPortuguese"
    FilterType="FilterType.LowercaseLetters | FilterType.UppercaseLetters | FilterType.Custom"
    ValidChars="áéíóúàâêôãõç" />

@* Mathematical expression *@
<FilteredTextBoxExtender 
    TargetControlID="txtMath"
    FilterType="FilterType.Numbers | FilterType.Custom"
    ValidChars="+-*/(). " />
```

## Render Mode Requirements

The FilteredTextBoxExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer

<TextBox @bind-Text="input" ID="txtInput" />
<FilteredTextBoxExtender 
    TargetControlID="txtInput"
    FilterType="FilterType.Numbers" />
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. The TextBox works normally without filtering (all input accepted).
- **JavaScript disabled:** Same as SSR — TextBox accepts all input.
- **Module import fails:** Any JavaScript errors are logged to browser console; TextBox continues to function normally.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:FilteredTextBoxExtender
   + <FilteredTextBoxExtender
   ```

2. **Convert FilterType flags** from space-separated to C# bitwise OR
   ```diff
   - FilterType="Numbers, Custom"
   + FilterType="FilterType.Numbers | FilterType.Custom"
   ```

3. **Remove `runat="server"` and `ID` attributes**
   ```diff
   - runat="server"
   - ID="ftb"
   ```

### Before (Web Forms)

```html
<asp:TextBox ID="txtPhone" runat="server" MaxLength="14" />

<ajaxToolkit:FilteredTextBoxExtender 
    ID="ftbPhone"
    TargetControlID="txtPhone"
    FilterType="Numbers, Custom"
    ValidChars="()-"
    runat="server" />
```

### After (Blazor)

```razor
<TextBox @bind-Text="phone" ID="txtPhone" MaxLength="14" />

<FilteredTextBoxExtender 
    TargetControlID="txtPhone"
    FilterType="FilterType.Numbers | FilterType.Custom"
    ValidChars="()-" />

@code {
    private string phone = "";
}
```

## Best Practices

1. **Use ValidChars mode for precision** — Whitelist known-good characters (safer than blacklist)
2. **Combine multiple FilterTypes** — Use `|` operator: `FilterType.Numbers | FilterType.LowercaseLetters`
3. **Test paste operations** — Users will try to paste; verify the behavior matches expectations
4. **Provide clear placeholders** — Show users what format is expected: `placeholder="(555) 123-4567"`
5. **Server-side validation** — Never rely on client-side filtering alone; always validate on the server
6. **Consider masking** — For structured data (phone, date, SSN), consider input masks in addition to filtering

## Troubleshooting

| Issue | Solution |
|---|---|
| Filtering not working | Verify `TargetControlID` matches TextBox's `ID`. Ensure `@rendermode InteractiveServer` is set. Check browser console for JavaScript errors. |
| Wrong characters allowed | Verify `FilterType` flags and `ValidChars` are correct. Remember: `FilterType.Custom` requires `ValidChars` to be set. |
| Pasted content not filtered | This is expected if `InvalidChars` mode is used. Switch to `ValidChars` mode (default) for stricter control. |
| Users cannot paste at all | Check that valid characters from the paste exist in your filter. If pasting "abc" into a Numbers-only field, nothing will paste (empty result). |
| Performance issues with rapid input | Increase `FilterInterval` to reduce checking frequency (default 250ms). |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [ConfirmButtonExtender](ConfirmButtonExtender.md) — Button confirmation dialogs
- [TextBox Component](../EditorControls/TextBox.md) — BWFC TextBox control documentation
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
