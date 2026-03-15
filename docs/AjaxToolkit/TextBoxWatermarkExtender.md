# TextBoxWatermarkExtender

The **TextBoxWatermarkExtender** displays a placeholder or watermark text in a TextBox when it is empty. The watermark text appears in a distinct style (via CSS class) and automatically disappears when the user focuses the field or begins typing. It reappears when the field is cleared.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/TextBoxWatermarkExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the TextBox to attach the watermark to
- `WatermarkText` — The placeholder text to display when empty
- `WatermarkCssClass` — CSS class applied to the TextBox when showing the watermark
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## Web Forms Syntax

```html
<asp:TextBox ID="txtSearch" runat="server" />

<ajaxToolkit:TextBoxWatermarkExtender
    ID="watermark1"
    runat="server"
    TargetControlID="txtSearch"
    WatermarkText="Search..."
    WatermarkCssClass="watermark-style" />
```

## Blazor Migration

```razor
<TextBox ID="txtSearch" />

<TextBoxWatermarkExtender
    TargetControlID="txtSearch"
    WatermarkText="Search..."
    WatermarkCssClass="watermark-style" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Everything else stays the same!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the TextBox to attach the watermark to |
| `WatermarkText` | `string` | `""` | The placeholder/watermark text to display when the TextBox is empty |
| `WatermarkCssClass` | `string` | `""` | CSS class applied to the TextBox when showing the watermark |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Basic Watermark

```razor
@rendermode InteractiveServer

<TextBox ID="txtEmail" />

<TextBoxWatermarkExtender
    TargetControlID="txtEmail"
    WatermarkText="Enter your email..." />
```

### Watermark with Custom Styling

```razor
@rendermode InteractiveServer

<TextBox ID="txtComment" />

<TextBoxWatermarkExtender
    TargetControlID="txtComment"
    WatermarkText="Add a comment..."
    WatermarkCssClass="input-watermark" />

<style>
    .input-watermark {
        color: #999;
        font-style: italic;
        opacity: 0.7;
    }
</style>
```

### Multiple Watermarks

```razor
@rendermode InteractiveServer

<TextBox ID="txtFirst" />
<TextBoxWatermarkExtender
    TargetControlID="txtFirst"
    WatermarkText="First name"
    WatermarkCssClass="watermark" />

<TextBox ID="txtLast" />
<TextBoxWatermarkExtender
    TargetControlID="txtLast"
    WatermarkText="Last name"
    WatermarkCssClass="watermark" />
```

## HTML Output

The TextBoxWatermarkExtender produces no HTML itself — it attaches JavaScript behavior to the target TextBox. The watermark text is managed entirely by JavaScript.

## JavaScript Interop

The TextBoxWatermarkExtender loads `textbox-watermark-extender.js` as an ES module. JavaScript handles:

- Displaying/hiding watermark text on focus
- Clearing watermark when user types
- Re-displaying watermark when field is cleared
- Applying/removing CSS classes based on watermark state
- Preserving the user's actual input value

## Render Mode Requirements

The TextBoxWatermarkExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. The TextBox works as a plain text input without watermark.
- **JavaScript disabled:** Same as SSR — TextBox functions without watermark display.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:TextBoxWatermarkExtender
   + <TextBoxWatermarkExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **CSS classes stay the same**
   ```diff
   - WatermarkCssClass="watermark-style"
   + WatermarkCssClass="watermark-style"
   ```

### Before (Web Forms)

```html
<asp:TextBox ID="txtSearch" runat="server" />

<ajaxToolkit:TextBoxWatermarkExtender
    ID="watermark1"
    TargetControlID="txtSearch"
    WatermarkText="Type to search..."
    WatermarkCssClass="watermark"
    runat="server" />
```

### After (Blazor)

```razor
<TextBox ID="txtSearch" />

<TextBoxWatermarkExtender
    TargetControlID="txtSearch"
    WatermarkText="Type to search..."
    WatermarkCssClass="watermark" />
```

## Best Practices

1. **Use descriptive watermark text** — Make it clear what the field expects (e.g., "Enter email address" vs "Email")
2. **Apply distinguishing CSS** — Use color, opacity, or font-style to visually separate watermark from user input
3. **Keep it short** — Long watermark text may be cut off in small fields
4. **Test with empty forms** — Ensure watermark appears correctly on page load

## Troubleshooting

| Issue | Solution |
|---|---|
| Watermark not appearing | Verify `TargetControlID` matches the TextBox's `ID`. Ensure `@rendermode InteractiveServer` is set. |
| Watermark text stays with input | Check CSS class styling. The JavaScript manages visibility, but CSS should distinguish watermark visually. |
| Watermark doesn't disappear when typing | Ensure JavaScript is enabled and `textbox-watermark-extender.js` loads successfully. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [TextBox Component](../EditorControls/TextBox.md) — The TextBox control
- [FilteredTextBoxExtender](FilteredTextBoxExtender.md) — Input filtering for TextBox
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
