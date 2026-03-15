# HoverMenuExtender

The **HoverMenuExtender** displays a popup panel when the user hovers over a target control. It supports configurable show/hide delays, positional placement, and hover CSS styling. The popup remains visible while the mouse is over either the target or the popup itself.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/HoverMenuExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the control that triggers the hover menu
- `PopupControlID` — ID of the panel element to display as a hover menu
- `PopupPosition` — Position of the popup relative to the target
- `OffsetX` — Horizontal offset in pixels from the calculated position
- `OffsetY` — Vertical offset in pixels from the calculated position
- `PopDelay` — Delay in milliseconds before showing the popup
- `HoverDelay` — Delay in milliseconds before hiding the popup
- `HoverCssClass` — CSS class applied to the target while the hover menu is visible
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## PopupPosition Enum

```csharp
enum PopupPosition
{
    Left = 0,     // To the left of target
    Right = 1,    // To the right of target (default for HoverMenuExtender)
    Top = 2,      // Above target
    Bottom = 3,   // Below target
    Center = 4    // Centered on target
}
```

## Web Forms Syntax

```html
<asp:Panel ID="pnlRow" runat="server" CssClass="data-row">
    <span>Row content here</span>
</asp:Panel>

<asp:Panel ID="pnlActions" runat="server" style="display: none;" CssClass="hover-actions">
    <asp:Button ID="btnEdit" Text="Edit" runat="server" />
    <asp:Button ID="btnDelete" Text="Delete" runat="server" />
</asp:Panel>

<ajaxToolkit:HoverMenuExtender
    ID="hme1"
    runat="server"
    TargetControlID="pnlRow"
    PopupControlID="pnlActions"
    PopupPosition="Right"
    HoverDelay="200"
    PopDelay="100"
    HoverCssClass="row-hover"
    OffsetX="5" />
```

## Blazor Migration

```razor
<div ID="pnlRow" class="data-row">
    <span>Row content here</span>
</div>

<div ID="pnlActions" style="display: none;" class="hover-actions">
    <Button ID="btnEdit" Text="Edit" />
    <Button ID="btnDelete" Text="Delete" />
</div>

<HoverMenuExtender
    TargetControlID="pnlRow"
    PopupControlID="pnlActions"
    PopupPosition="PopupPosition.Right"
    HoverDelay="200"
    PopDelay="100"
    HoverCssClass="row-hover"
    OffsetX="5" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Use full enum type name for `PopupPosition`. Everything else stays the same!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the control that triggers the hover menu |
| `PopupControlID` | `string` | `""` | ID of the panel element to display as a hover menu |
| `PopupPosition` | `PopupPosition` | `Right` | Position of the popup relative to the target: `Left`, `Right`, `Top`, `Bottom`, `Center` |
| `OffsetX` | `int` | `0` | Horizontal offset in pixels from the calculated position |
| `OffsetY` | `int` | `0` | Vertical offset in pixels from the calculated position |
| `PopDelay` | `int` | `0` | Delay in milliseconds before showing the popup after mouse enters the target |
| `HoverDelay` | `int` | `300` | Delay in milliseconds before hiding the popup after mouse leaves the target and popup |
| `HoverCssClass` | `string` | `""` | CSS class applied to the target control while the hover menu is visible |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Basic Hover Menu

```razor
@rendermode InteractiveServer

<div ID="targetItem" style="padding: 10px; border: 1px solid #ddd; cursor: pointer;">
    Hover over me to see actions
</div>

<div ID="hoverPanel" style="display: none; background: white; border: 1px solid #ccc; padding: 10px; box-shadow: 0 2px 8px rgba(0,0,0,0.15);">
    <button>Edit</button>
    <button>Delete</button>
    <button>Share</button>
</div>

<HoverMenuExtender
    TargetControlID="targetItem"
    PopupControlID="hoverPanel"
    PopupPosition="PopupPosition.Right"
    HoverDelay="200" />
```

### Row Actions on Hover

```razor
@rendermode InteractiveServer

@foreach (var item in items)
{
    <div ID="@($"row-{item.Id}")" class="list-row">
        <span>@item.Name</span>
    </div>

    <div ID="@($"actions-{item.Id}")" style="display: none;" class="action-panel">
        <button onclick="@(() => Edit(item.Id))">✏️ Edit</button>
        <button onclick="@(() => Delete(item.Id))">🗑️ Delete</button>
    </div>

    <HoverMenuExtender
        TargetControlID="@($"row-{item.Id}")"
        PopupControlID="@($"actions-{item.Id}")"
        PopupPosition="PopupPosition.Right"
        HoverDelay="300"
        HoverCssClass="row-highlighted"
        OffsetX="10" />
}

<style>
    .list-row { padding: 10px; border-bottom: 1px solid #eee; }
    .row-highlighted { background: #f0f8ff; }
    .action-panel { background: white; border: 1px solid #ddd; padding: 8px; border-radius: 4px; }
</style>

@code {
    private List<Item> items = new();
    void Edit(int id) { /* Edit logic */ }
    void Delete(int id) { /* Delete logic */ }
}
```

### Tooltip-Style Hover Menu

```razor
@rendermode InteractiveServer

<span ID="helpIcon" style="cursor: help; font-size: 1.2em;">ℹ️</span>

<div ID="helpTooltip" style="display: none; background: #333; color: white; padding: 12px; border-radius: 6px; max-width: 250px; font-size: 0.9em;">
    <p>This field accepts values between 1 and 100. Leave blank for the default value.</p>
</div>

<HoverMenuExtender
    TargetControlID="helpIcon"
    PopupControlID="helpTooltip"
    PopupPosition="PopupPosition.Bottom"
    PopDelay="200"
    HoverDelay="500"
    OffsetY="5" />
```

### Hover Menu with Show Delay

```razor
@rendermode InteractiveServer

<div ID="menuTrigger" style="padding: 8px 16px; background: #007bff; color: white; display: inline-block; border-radius: 4px;">
    More Options ▼
</div>

<div ID="menuPopup" style="display: none; background: white; border: 1px solid #ddd; min-width: 150px; box-shadow: 0 4px 12px rgba(0,0,0,0.1);">
    <div style="padding: 8px 16px; cursor: pointer;">Option 1</div>
    <div style="padding: 8px 16px; cursor: pointer;">Option 2</div>
    <div style="padding: 8px 16px; cursor: pointer;">Option 3</div>
</div>

<HoverMenuExtender
    TargetControlID="menuTrigger"
    PopupControlID="menuPopup"
    PopupPosition="PopupPosition.Bottom"
    PopDelay="100"
    HoverDelay="400" />
```

## HTML Output

The HoverMenuExtender produces no HTML itself — it attaches JavaScript behavior to show/hide the popup panel on mouse enter/leave events.

## JavaScript Interop

The HoverMenuExtender loads `hover-menu-extender.js` as an ES module. JavaScript handles:

- Monitoring mouse enter/leave events on the target and popup elements
- Applying show/hide delays (`PopDelay` and `HoverDelay`)
- Positioning the popup relative to the target with offsets
- Adding/removing the `HoverCssClass` on the target element
- Keeping the popup visible when the mouse moves between target and popup

## Render Mode Requirements

The HoverMenuExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. The popup panel remains hidden.
- **JavaScript disabled:** Same as SSR — target control functions normally without hover behavior.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:HoverMenuExtender
   + <HoverMenuExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Use full enum type name for PopupPosition**
   ```diff
   - PopupPosition="Right"
   + PopupPosition="PopupPosition.Right"
   ```

### Before (Web Forms)

```html
<asp:Panel ID="pnlTarget" runat="server">
    <span>Hover me</span>
</asp:Panel>

<asp:Panel ID="pnlMenu" runat="server" style="display: none;">
    <a href="#">Edit</a> | <a href="#">Delete</a>
</asp:Panel>

<ajaxToolkit:HoverMenuExtender
    ID="hme1"
    TargetControlID="pnlTarget"
    PopupControlID="pnlMenu"
    PopupPosition="Right"
    HoverDelay="200"
    runat="server" />
```

### After (Blazor)

```razor
<div ID="pnlTarget">
    <span>Hover me</span>
</div>

<div ID="pnlMenu" style="display: none;">
    <a href="#">Edit</a> | <a href="#">Delete</a>
</div>

<HoverMenuExtender
    TargetControlID="pnlTarget"
    PopupControlID="pnlMenu"
    PopupPosition="PopupPosition.Right"
    HoverDelay="200" />
```

## Best Practices

1. **Set appropriate delays** — `PopDelay` of 100–200ms prevents accidental triggers; `HoverDelay` of 200–500ms gives users time to reach the popup
2. **Hide the popup initially** — Set `style="display: none;"` on the popup element
3. **Use HoverCssClass** — Highlight the target to show the hover menu is active
4. **Position wisely** — Use `Right` or `Bottom` for most use cases; adjust with offsets
5. **Keep popup content interactive** — Users should be able to interact with buttons/links in the popup

## Troubleshooting

| Issue | Solution |
|---|---|
| Popup not appearing | Verify `TargetControlID` and `PopupControlID` match their elements' `ID`s. Ensure `@rendermode InteractiveServer` is set. |
| Popup disappears too quickly | Increase `HoverDelay` to give users more time (300–500ms). |
| Popup appears too slowly | Decrease `PopDelay` (0–100ms). |
| Popup positioned incorrectly | Try a different `PopupPosition` value or adjust `OffsetX`/`OffsetY`. |
| HoverCssClass not applied | Verify the CSS class name is correct and the styles are defined. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [PopupControlExtender](PopupControlExtender.md) — Click-triggered popup
- [ModalPopupExtender](ModalPopupExtender.md) — Full modal popup with overlay
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
