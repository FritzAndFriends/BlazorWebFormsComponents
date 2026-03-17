# PopupControlExtender

The **PopupControlExtender** attaches a popup panel to a target control, displaying it when the target is clicked. It is lighter than the ModalPopupExtender — no overlay backdrop, no focus trap. The popup supports positional placement, commit property/script support, and outside-click dismissal.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/PopupControlExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the control that triggers the popup on click
- `PopupControlID` — ID of the panel element to display as a popup
- `Position` — Position of the popup relative to the target control
- `OffsetX` — Horizontal offset in pixels from the calculated position
- `OffsetY` — Vertical offset in pixels from the calculated position
- `CommitProperty` — Property on the target control to set when popup is committed
- `CommitScript` — JavaScript to execute when the popup is committed
- `ExtenderControlID` — ID for programmatic access to the extender
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## PopupPosition Enum

```csharp
enum PopupPosition
{
    Left = 0,     // To the left of target
    Right = 1,    // To the right of target
    Top = 2,      // Above target
    Bottom = 3,   // Below target (default)
    Center = 4    // Centered on target
}
```

## Web Forms Syntax

```html
<asp:TextBox ID="txtColor" runat="server" />

<asp:Panel ID="pnlColorPicker" runat="server" style="display: none;">
    <div class="color-grid">
        <span style="background: red;" onclick="commitColor('red')"></span>
        <span style="background: blue;" onclick="commitColor('blue')"></span>
        <span style="background: green;" onclick="commitColor('green')"></span>
    </div>
</asp:Panel>

<ajaxToolkit:PopupControlExtender
    ID="pce1"
    runat="server"
    TargetControlID="txtColor"
    PopupControlID="pnlColorPicker"
    Position="Bottom"
    CommitProperty="value"
    CommitScript="onColorCommit()" />
```

## Blazor Migration

```razor
<TextBox ID="txtColor" />

<div ID="pnlColorPicker" style="display: none;">
    <div class="color-grid">
        <span style="background: red;" onclick="commitColor('red')"></span>
        <span style="background: blue;" onclick="commitColor('blue')"></span>
        <span style="background: green;" onclick="commitColor('green')"></span>
    </div>
</div>

<PopupControlExtender
    TargetControlID="txtColor"
    PopupControlID="pnlColorPicker"
    Position="PopupPosition.Bottom"
    CommitProperty="value"
    CommitScript="onColorCommit()" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Use full enum type name for `Position`. Everything else stays the same!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the control that triggers the popup on click |
| `PopupControlID` | `string` | `""` | ID of the panel element to display as a popup |
| `Position` | `PopupPosition` | `Bottom` | Position of the popup relative to the target: `Left`, `Right`, `Top`, `Bottom`, `Center` |
| `OffsetX` | `int` | `0` | Horizontal offset in pixels from the calculated position |
| `OffsetY` | `int` | `0` | Vertical offset in pixels from the calculated position |
| `CommitProperty` | `string` | `""` | Property name on the target control to set when the popup is committed (e.g., "value") |
| `CommitScript` | `string` | `""` | JavaScript to execute when the popup is committed |
| `ExtenderControlID` | `string` | `""` | ID for programmatic access to the extender |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Basic Popup

```razor
@rendermode InteractiveServer

<TextBox ID="txtInput" />

<div ID="popupPanel" style="display: none; background: white; border: 1px solid #ccc; padding: 10px; border-radius: 4px;">
    <p>Select an option:</p>
    <button onclick="selectOption('Option A')">Option A</button>
    <button onclick="selectOption('Option B')">Option B</button>
</div>

<PopupControlExtender
    TargetControlID="txtInput"
    PopupControlID="popupPanel"
    Position="PopupPosition.Bottom" />
```

### Popup with Offset

```razor
@rendermode InteractiveServer

<TextBox ID="txtNote" />

<div ID="notePopup" style="display: none; background: #fffbcc; border: 1px solid #e0d870; padding: 15px; width: 250px;">
    <h4>Quick Notes</h4>
    <textarea rows="3" style="width: 100%;"></textarea>
    <button onclick="saveNote()">Save</button>
</div>

<PopupControlExtender
    TargetControlID="txtNote"
    PopupControlID="notePopup"
    Position="PopupPosition.Right"
    OffsetX="10"
    OffsetY="-5" />
```

### Popup with Commit Property

When a value is selected in the popup, it can be automatically set on the target control:

```razor
@rendermode InteractiveServer

<TextBox ID="txtCategory" />

<div ID="categoryPicker" style="display: none; background: white; border: 1px solid #ddd; padding: 8px;">
    <ul style="list-style: none; padding: 0; margin: 0;">
        <li style="padding: 4px 8px; cursor: pointer;" onclick="commitCategory('Electronics')">Electronics</li>
        <li style="padding: 4px 8px; cursor: pointer;" onclick="commitCategory('Clothing')">Clothing</li>
        <li style="padding: 4px 8px; cursor: pointer;" onclick="commitCategory('Books')">Books</li>
    </ul>
</div>

<PopupControlExtender
    TargetControlID="txtCategory"
    PopupControlID="categoryPicker"
    Position="PopupPosition.Bottom"
    CommitProperty="value" />
```

## HTML Output

The PopupControlExtender produces no HTML itself — it attaches JavaScript behavior to show/hide the popup panel and position it relative to the target control.

## JavaScript Interop

The PopupControlExtender loads `popup-control-extender.js` as an ES module. JavaScript handles:

- Showing the popup when the target control is clicked
- Positioning the popup relative to the target using the configured position and offsets
- Dismissing the popup on outside click
- Setting the commit property on the target when a value is committed
- Executing the commit script

## Render Mode Requirements

The PopupControlExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. The popup panel remains hidden.
- **JavaScript disabled:** Same as SSR — target control works normally without popup.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:PopupControlExtender
   + <PopupControlExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Use full enum type name for Position**
   ```diff
   - Position="Bottom"
   + Position="PopupPosition.Bottom"
   ```

### Before (Web Forms)

```html
<asp:TextBox ID="txtColor" runat="server" />

<asp:Panel ID="pnlPicker" runat="server" style="display: none;">
    <div>Color picker content</div>
</asp:Panel>

<ajaxToolkit:PopupControlExtender
    ID="pce1"
    TargetControlID="txtColor"
    PopupControlID="pnlPicker"
    Position="Bottom"
    CommitProperty="value"
    runat="server" />
```

### After (Blazor)

```razor
<TextBox ID="txtColor" />

<div ID="pnlPicker" style="display: none;">
    <div>Color picker content</div>
</div>

<PopupControlExtender
    TargetControlID="txtColor"
    PopupControlID="pnlPicker"
    Position="PopupPosition.Bottom"
    CommitProperty="value" />
```

## Best Practices

1. **Hide the popup initially** — Set `style="display: none;"` on the popup element
2. **Use appropriate positioning** — Choose `Bottom` for dropdowns, `Right` for sidebars
3. **Fine-tune with offsets** — Use `OffsetX`/`OffsetY` to avoid visual overlap
4. **Keep popup content focused** — Avoid overloading the popup with too much content
5. **Use CommitProperty for form integration** — Automatically set values on the target control

## Troubleshooting

| Issue | Solution |
|---|---|
| Popup not appearing | Verify `TargetControlID` and `PopupControlID` match their elements' `ID`s. Ensure `@rendermode InteractiveServer` is set. |
| Popup positioned incorrectly | Try a different `Position` value or adjust `OffsetX`/`OffsetY`. |
| Popup not dismissing | Ensure there are no event propagation issues (e.g., `stopPropagation` calls) in the popup content. |
| Value not committed | Verify `CommitProperty` matches the target's property name (typically "value" for inputs). |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [ModalPopupExtender](ModalPopupExtender.md) — Full modal popup with overlay
- [HoverMenuExtender](HoverMenuExtender.md) — Hover-triggered popup
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
