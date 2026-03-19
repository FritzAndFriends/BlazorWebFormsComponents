# AlwaysVisibleControlExtender

The **AlwaysVisibleControlExtender** keeps a control visible in a fixed position on the screen even when the user scrolls the page. This is useful for floating toolbars, help buttons, notification areas, or sticky navigation elements that should remain accessible at all times.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/AlwaysVisibleControlExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the element to keep visible
- `HorizontalOffset` — Pixels from the horizontal edge
- `VerticalOffset` — Pixels from the vertical edge
- `HorizontalSide` — Which horizontal edge (Left, Center, Right)
- `VerticalSide` — Which vertical edge (Top, Middle, Bottom)
- `ScrollEffectDuration` — Animation duration in seconds
- `UseAnimation` — Whether to animate position changes
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## HorizontalSide Enum

Controls horizontal positioning:

```csharp
enum HorizontalSide
{
    Left = 0,
    Center = 1,
    Right = 2
}
```

## VerticalSide Enum

Controls vertical positioning:

```csharp
enum VerticalSide
{
    Top = 0,
    Middle = 1,
    Bottom = 2
}
```

## Web Forms Syntax

```html
<asp:Panel ID="pnlHelp" runat="server" style="width: 250px; padding: 10px; background-color: #e3f2fd; border: 1px solid #1976d2;">
    <strong>Need Help?</strong>
    <p>Click here to get started.</p>
</asp:Panel>

<ajaxToolkit:AlwaysVisibleControlExtender
    ID="sticky1"
    runat="server"
    TargetControlID="pnlHelp"
    HorizontalSide="Right"
    VerticalSide="Bottom"
    HorizontalOffset="20"
    VerticalOffset="20"
    UseAnimation="true"
    ScrollEffectDuration="0.1" />
```

## Blazor Migration

```razor
<Panel ID="pnlHelp" style="width: 250px; padding: 10px; background-color: #e3f2fd; border: 1px solid #1976d2;">
    <strong>Need Help?</strong>
    <p>Click here to get started.</p>
</Panel>

<AlwaysVisibleControlExtender
    TargetControlID="pnlHelp"
    HorizontalSide="HorizontalSide.Right"
    VerticalSide="VerticalSide.Bottom"
    HorizontalOffset="20"
    VerticalOffset="20"
    UseAnimation="true"
    ScrollEffectDuration="0.1" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Use enum type names for positioning!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the element to keep visible |
| `HorizontalOffset` | `int` | `0` | Number of pixels from the horizontal edge |
| `VerticalOffset` | `int` | `0` | Number of pixels from the vertical edge |
| `HorizontalSide` | `HorizontalSide` | `Left` | Which horizontal edge to position against (Left, Center, Right) |
| `VerticalSide` | `VerticalSide` | `Top` | Which vertical edge to position against (Top, Middle, Bottom) |
| `ScrollEffectDuration` | `float` | `0.1` | Duration of the position animation in seconds |
| `UseAnimation` | `bool` | `true` | Whether to animate position changes smoothly |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Sticky Help Button (Bottom-Right)

```razor
@rendermode InteractiveServer

<Panel ID="pnlHelp" style="width: 200px; padding: 15px; background-color: #4CAF50; color: white; border-radius: 5px; cursor: pointer;">
    <strong>?</strong> Need Help?
</Panel>

<AlwaysVisibleControlExtender
    TargetControlID="pnlHelp"
    HorizontalSide="HorizontalSide.Right"
    VerticalSide="VerticalSide.Bottom"
    HorizontalOffset="15"
    VerticalOffset="15"
    UseAnimation="true" />
```

### Sticky Navigation (Top)

```razor
@rendermode InteractiveServer

<Panel ID="pnlNav" style="width: 100%; padding: 10px; background-color: #333; color: white;">
    <a href="#" style="color: white; margin-right: 20px;">Home</a>
    <a href="#" style="color: white; margin-right: 20px;">Products</a>
    <a href="#" style="color: white; margin-right: 20px;">Contact</a>
</Panel>

<AlwaysVisibleControlExtender
    TargetControlID="pnlNav"
    HorizontalSide="HorizontalSide.Center"
    VerticalSide="VerticalSide.Top"
    VerticalOffset="0"
    UseAnimation="false" />
```

### Floating Notification (Top-Right)

```razor
@rendermode InteractiveServer

<Panel ID="pnlNotification" style="width: 300px; padding: 15px; background-color: #FFC107; border-radius: 5px; box-shadow: 0 2px 5px rgba(0,0,0,0.2);">
    <strong>Alert:</strong> You have 3 new messages.
</Panel>

<AlwaysVisibleControlExtender
    TargetControlID="pnlNotification"
    HorizontalSide="HorizontalSide.Right"
    VerticalSide="VerticalSide.Top"
    HorizontalOffset="20"
    VerticalOffset="20"
    UseAnimation="true"
    ScrollEffectDuration="0.2" />
```

### Sticky Side Panel (Left)

```razor
@rendermode InteractiveServer

<Panel ID="pnlSidebar" style="width: 250px; padding: 20px; background-color: #f5f5f5; border-right: 1px solid #ccc;">
    <h3>Quick Links</h3>
    <ul>
        <li><a href="#">Dashboard</a></li>
        <li><a href="#">Settings</a></li>
        <li><a href="#">Profile</a></li>
    </ul>
</Panel>

<AlwaysVisibleControlExtender
    TargetControlID="pnlSidebar"
    HorizontalSide="HorizontalSide.Left"
    VerticalSide="VerticalSide.Top"
    VerticalOffset="0"
    UseAnimation="true" />
```

## HTML Output

The AlwaysVisibleControlExtender produces no HTML itself — it attaches JavaScript behavior to the target element, repositioning it as the user scrolls.

## JavaScript Interop

The AlwaysVisibleControlExtender loads `always-visible-control-extender.js` as an ES module. JavaScript handles:

- Monitoring window scroll events
- Calculating fixed position based on offsets and sides
- Updating element position as user scrolls
- Optional animation of position changes
- Handling window resize events
- Maintaining element visibility within viewport

## Render Mode Requirements

The AlwaysVisibleControlExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. The element displays at its normal position without scrolling.
- **JavaScript disabled:** Same as SSR — Element does not stick to viewport.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:AlwaysVisibleControlExtender
   + <AlwaysVisibleControlExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Use enum types for positioning**
   ```diff
   - HorizontalSide="Right"
   + HorizontalSide="HorizontalSide.Right"
   ```

### Before (Web Forms)

```html
<asp:Panel ID="pnlHelp" runat="server" style="width: 250px; padding: 10px; background: #e3f2fd;">
    <p>Help content</p>
</asp:Panel>

<ajaxToolkit:AlwaysVisibleControlExtender
    ID="sticky1"
    TargetControlID="pnlHelp"
    HorizontalSide="Right"
    VerticalSide="Bottom"
    HorizontalOffset="20"
    VerticalOffset="20"
    runat="server" />
```

### After (Blazor)

```razor
<Panel ID="pnlHelp" style="width: 250px; padding: 10px; background: #e3f2fd;">
    <p>Help content</p>
</Panel>

<AlwaysVisibleControlExtender
    TargetControlID="pnlHelp"
    HorizontalSide="HorizontalSide.Right"
    VerticalSide="VerticalSide.Bottom"
    HorizontalOffset="20"
    VerticalOffset="20" />
```

## Best Practices

1. **Use appropriate sizes** — Keep sticky elements reasonably sized so they don't block content
2. **Position wisely** — Use corners or edges to avoid covering main content
3. **Add offsets** — Use `HorizontalOffset` and `VerticalOffset` to keep space from viewport edges
4. **Consider mobile** — Sticky elements on small screens can be intrusive; test responsiveness
5. **Use animation** — Enable `UseAnimation` for smooth repositioning as user scrolls
6. **Include close button** — Consider adding a close button to let users hide the sticky element

## Troubleshooting

| Issue | Solution |
|---|---|
| Element not staying visible | Verify `TargetControlID` matches the element's ID. Ensure `@rendermode InteractiveServer` is set. |
| Element covers content | Adjust `HorizontalOffset`, `VerticalOffset`, or position values. Choose a different corner. |
| Animation is jerky | Reduce `ScrollEffectDuration` (try 0.05-0.15 seconds) or disable `UseAnimation`. |
| Element disappears on scroll | Check element's `position` CSS property. The extender uses `position: fixed` internally. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- Panel Component — The Panel control (documentation coming soon)
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
