# BalloonPopupExtender

The **BalloonPopupExtender** displays a balloon or tooltip-style popup with a pointer arrow when the user hovers over, focuses on, or clicks a target element. The popup displays content from a specified container element and can be customized with different styles (Rectangle, Cloud, Custom), sizes, shadows, and scrollbar settings.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/BalloonPopupExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the element that triggers the balloon
- `BalloonPopupControlID` — ID of the element containing balloon content
- `Position` — Where the balloon appears (TopLeft, TopRight, BottomLeft, BottomRight, Auto)
- `BalloonStyle` — Visual style (Rectangle, Cloud, Custom)
- `BalloonSize` — Size preset (Small, Medium, Large)
- `UseShadow` — Whether to display a drop shadow
- `ScrollBars` — Scrollbar behavior (Auto, None, Horizontal, Vertical, Both)
- `DisplayOnMouseOver` — Show balloon on hover
- `DisplayOnFocus` — Show balloon on focus
- `DisplayOnClick` — Show balloon on click
- `OffsetX` — Horizontal offset in pixels
- `OffsetY` — Vertical offset in pixels
- `CustomCssUrl` — URL to custom CSS (for Custom style)

## Position Enum

Controls balloon position:

```csharp
enum BalloonPosition
{
    TopLeft = 0,
    TopRight = 1,
    BottomLeft = 2,
    BottomRight = 3,
    Auto = 4  // Automatically position to stay in viewport
}
```

## BalloonStyle Enum

Controls visual appearance:

```csharp
enum BalloonStyle
{
    Rectangle = 0,  // Simple rectangle with border
    Cloud = 1,      // Cloud-like rounded shape
    Custom = 2      // Custom styling via CSS
}
```

## BalloonSize Enum

Controls balloon dimensions:

```csharp
enum BalloonSize
{
    Small = 0,    // Compact size
    Medium = 1,   // Standard size
    Large = 2     // Spacious size
}
```

## Web Forms Syntax

```html
<asp:TextBox ID="txtEmail" runat="server" />

<div id="balloonContent" style="display: none;">
    <p>Enter a valid email address (e.g., user@example.com)</p>
</div>

<ajaxToolkit:BalloonPopupExtender
    ID="balloon1"
    runat="server"
    TargetControlID="txtEmail"
    BalloonPopupControlID="balloonContent"
    Position="BottomRight"
    BalloonStyle="Cloud"
    BalloonSize="Medium"
    UseShadow="true"
    DisplayOnFocus="true" />
```

## Blazor Migration

```razor
<TextBox ID="txtEmail" />

<div id="balloonContent" style="display: none;">
    <p>Enter a valid email address (e.g., user@example.com)</p>
</div>

<BalloonPopupExtender
    TargetControlID="txtEmail"
    BalloonPopupControlID="balloonContent"
    Position="BalloonPosition.BottomRight"
    BalloonStyle="BalloonStyle.Cloud"
    BalloonSize="BalloonSize.Medium"
    UseShadow="true"
    DisplayOnFocus="true" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Use enum type names!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the element that triggers the balloon popup |
| `BalloonPopupControlID` | `string` | (required) | ID of the element containing the balloon content |
| `Position` | `BalloonPosition` | `Auto` | Position of the balloon relative to target (TopLeft, TopRight, BottomLeft, BottomRight, Auto) |
| `BalloonStyle` | `BalloonStyle` | `Rectangle` | Visual style of the balloon (Rectangle, Cloud, Custom) |
| `BalloonSize` | `BalloonSize` | `Medium` | Size preset (Small, Medium, Large) |
| `UseShadow` | `bool` | `true` | Whether to display a drop shadow on the balloon |
| `ScrollBars` | `ScrollBars` | `Auto` | Scrollbar behavior when content exceeds balloon size |
| `DisplayOnMouseOver` | `bool` | `true` | Show balloon when mouse hovers over target |
| `DisplayOnFocus` | `bool` | `false` | Show balloon when target receives focus |
| `DisplayOnClick` | `bool` | `false` | Show balloon when target is clicked |
| `OffsetX` | `int` | `0` | Horizontal offset in pixels from calculated position |
| `OffsetY` | `int` | `0` | Vertical offset in pixels from calculated position |
| `CustomCssUrl` | `string` | `""` | URL to custom CSS file (only used with BalloonStyle.Custom) |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Help Tooltip on Input

```razor
@rendermode InteractiveServer

<form>
    <label>Email Address:</label>
    <TextBox ID="txtEmail" type="email" style="width: 200px;" />
    
    <div id="emailHelp" style="display: none; padding: 10px;">
        <p><strong>Email Format:</strong></p>
        <p>user@domain.com</p>
    </div>
    
    <BalloonPopupExtender
        TargetControlID="txtEmail"
        BalloonPopupControlID="emailHelp"
        Position="BalloonPosition.BottomRight"
        BalloonStyle="BalloonStyle.Cloud"
        BalloonSize="BalloonSize.Small"
        DisplayOnFocus="true"
        DisplayOnMouseOver="true" />
</form>
```

### Cloud-Style Information Popup

```razor
@rendermode InteractiveServer

<button id="btnInfo" style="background: #2196F3; color: white; padding: 8px 16px; border-radius: 4px; cursor: pointer;">
    ? Info
</button>

<div id="infoContent" style="display: none; padding: 15px;">
    <h4>How to Use:</h4>
    <ol>
        <li>Fill in all required fields</li>
        <li>Click the Submit button</li>
        <li>Confirmation will be sent via email</li>
    </ol>
</div>

<BalloonPopupExtender
    TargetControlID="btnInfo"
    BalloonPopupControlID="infoContent"
    Position="BalloonPosition.Auto"
    BalloonStyle="BalloonStyle.Cloud"
    BalloonSize="BalloonSize.Large"
    DisplayOnClick="true"
    UseShadow="true" />
```

### Rectangle-Style Note

```razor
@rendermode InteractiveServer

<label>Password:</label>
<TextBox ID="txtPassword" type="password" style="width: 200px;" />

<div id="passwordNote" style="display: none; padding: 10px; background: #fffacd; border-left: 3px solid #ffc107;">
    <strong>Password Requirements:</strong>
    <ul style="margin: 5px 0; padding-left: 20px;">
        <li>Minimum 8 characters</li>
        <li>At least one number</li>
        <li>At least one symbol</li>
    </ul>
</div>

<BalloonPopupExtender
    TargetControlID="txtPassword"
    BalloonPopupControlID="passwordNote"
    Position="BalloonPosition.BottomRight"
    BalloonStyle="BalloonStyle.Rectangle"
    BalloonSize="BalloonSize.Medium"
    DisplayOnFocus="true"
    OffsetX="10"
    OffsetY="5" />
```

### Multi-Purpose Tooltips

```razor
@rendermode InteractiveServer

<table>
    <tr>
        <td>
            <label>First Name:</label>
            <input id="firstNameInput" type="text" />
        </td>
        <td>
            <span id="firstNameHelp" style="cursor: help; color: #2196F3; font-weight: bold;">?</span>
            <div id="firstNamePopup" style="display: none; padding: 8px;">
                Your legal first name
            </div>
            
            <BalloonPopupExtender
                TargetControlID="firstNameHelp"
                BalloonPopupControlID="firstNamePopup"
                Position="BalloonPosition.TopRight"
                BalloonStyle="BalloonStyle.Cloud"
                BalloonSize="BalloonSize.Small"
                DisplayOnMouseOver="true" />
        </td>
    </tr>
    <tr>
        <td>
            <label>Last Name:</label>
            <input id="lastNameInput" type="text" />
        </td>
        <td>
            <span id="lastNameHelp" style="cursor: help; color: #2196F3; font-weight: bold;">?</span>
            <div id="lastNamePopup" style="display: none; padding: 8px;">
                Your legal last name
            </div>
            
            <BalloonPopupExtender
                TargetControlID="lastNameHelp"
                BalloonPopupControlID="lastNamePopup"
                Position="BalloonPosition.TopRight"
                BalloonStyle="BalloonStyle.Cloud"
                BalloonSize="BalloonSize.Small"
                DisplayOnMouseOver="true" />
        </td>
    </tr>
</table>
```

## HTML Output

The BalloonPopupExtender creates or repositions the balloon popup element based on the target's position and configured settings.

## JavaScript Interop

The BalloonPopupExtender loads `balloon-popup-extender.js` as an ES module. JavaScript handles:

- Detecting trigger events (hover, focus, click) on target
- Positioning balloon relative to target and viewport
- Displaying/hiding the balloon popup
- Applying balloon style and size classes
- Managing shadow display
- Scrollbar handling for overflow content
- Repositioning on window scroll/resize
- Closing balloon on outside click
- Offset calculations

## Render Mode Requirements

The BalloonPopupExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. Content remains hidden.
- **JavaScript disabled:** Same as SSR — Balloon doesn't appear.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:BalloonPopupExtender
   + <BalloonPopupExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Use enum types for position, style, and size**
   ```diff
   - Position="BottomRight"
   + Position="BalloonPosition.BottomRight"
   - BalloonStyle="Cloud"
   + BalloonStyle="BalloonStyle.Cloud"
   ```

### Before (Web Forms)

```html
<asp:TextBox ID="txtField" runat="server" />

<div id="popupContent" style="display: none;">
    <p>Help text</p>
</div>

<ajaxToolkit:BalloonPopupExtender
    ID="balloon1"
    TargetControlID="txtField"
    BalloonPopupControlID="popupContent"
    Position="BottomRight"
    BalloonStyle="Cloud"
    runat="server" />
```

### After (Blazor)

```razor
<TextBox ID="txtField" />

<div id="popupContent" style="display: none;">
    <p>Help text</p>
</div>

<BalloonPopupExtender
    TargetControlID="txtField"
    BalloonPopupControlID="popupContent"
    Position="BalloonPosition.BottomRight"
    BalloonStyle="BalloonStyle.Cloud" />
```

## Best Practices

1. **Use Auto position when possible** — Automatically keeps balloon in viewport
2. **Hide content initially** — Keep balloon content hidden with `display: none` until needed
3. **Keep content brief** — Balloons work best for short help text or quick info
4. **Test viewport positioning** — Ensure balloons don't get cut off on small screens
5. **Use Cloud style for friendly UI** — Cloud style works well for help and tips
6. **Consider accessibility** — Provide alternative ways to access information for users who don't hover
7. **Limit trigger events** — Choose specific trigger events (hover, focus, or click) rather than all

## Troubleshooting

| Issue | Solution |
|---|---|
| Balloon not appearing | Verify both `TargetControlID` and `BalloonPopupControlID` match actual element IDs. Ensure content div has `display: none`. Check `@rendermode InteractiveServer`. |
| Balloon positioned off-screen | Use `Position="BalloonPosition.Auto"` for automatic viewport-aware positioning, or adjust `OffsetX`/`OffsetY`. |
| Content not visible | Ensure the content div (`BalloonPopupControlID`) is not accidentally hidden or has no height. |
| Multiple balloons interfering | Each target should have its own unique balloon content div with unique ID. |
| Trigger events not working | Verify `DisplayOnMouseOver`, `DisplayOnFocus`, or `DisplayOnClick` are enabled as desired. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [TextBox Component](../EditorControls/TextBox.md) — The TextBox control
- [HoverMenuExtender](HoverMenuExtender.md) — Hover-activated menus
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
