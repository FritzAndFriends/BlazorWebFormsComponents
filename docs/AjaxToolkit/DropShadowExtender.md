# DropShadowExtender

The **DropShadowExtender** adds a drop shadow effect to an element, giving it a raised or floating appearance. The shadow can be customized with opacity, width, corner rounding, and can optionally track the element's position as it moves.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/DropShadowExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the element to add a drop shadow to
- `Opacity` — Shadow opacity from 0.0 (transparent) to 1.0 (opaque)
- `Width` — Shadow width in pixels
- `Rounded` — Whether to round the corners of the target element
- `Radius` — Corner radius in pixels when `Rounded` is true
- `TrackPosition` — Whether the shadow tracks the element's position if it moves
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## Web Forms Syntax

```html
<asp:Panel ID="pnlBox" runat="server" style="width: 300px; padding: 15px; background-color: white;">
    <h3>Floating Panel</h3>
    <p>This panel has a drop shadow.</p>
</asp:Panel>

<ajaxToolkit:DropShadowExtender
    ID="shadow1"
    runat="server"
    TargetControlID="pnlBox"
    Opacity="0.5"
    Width="5"
    Rounded="true"
    Radius="10" />
```

## Blazor Migration

```razor
<Panel ID="pnlBox" style="width: 300px; padding: 15px; background-color: white;">
    <h3>Floating Panel</h3>
    <p>This panel has a drop shadow.</p>
</Panel>

<DropShadowExtender
    TargetControlID="pnlBox"
    Opacity="0.5"
    Width="5"
    Rounded="true"
    Radius="10" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Everything else stays the same!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the element to add a drop shadow to |
| `Opacity` | `double` | `0.5` | Shadow opacity from 0.0 (fully transparent) to 1.0 (fully opaque) |
| `Width` | `int` | `5` | Shadow width in pixels |
| `Rounded` | `bool` | `false` | Whether to round the corners of the target element |
| `Radius` | `int` | `0` | Corner radius in pixels when `Rounded` is true |
| `TrackPosition` | `bool` | `false` | Whether the shadow should track the element's position if it moves |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Basic Drop Shadow

```razor
@rendermode InteractiveServer

<Panel ID="pnlCard" style="width: 250px; padding: 20px; background-color: white;">
    <h3>Card Title</h3>
    <p>This card has a subtle drop shadow.</p>
</Panel>

<DropShadowExtender
    TargetControlID="pnlCard"
    Opacity="0.5"
    Width="5" />
```

### Shadow with Rounded Corners

```razor
@rendermode InteractiveServer

<Panel ID="pnlRounded" style="width: 300px; padding: 20px; background-color: #f9f9f9;">
    <h3>Rounded Panel</h3>
    <p>This panel has both rounded corners and a drop shadow.</p>
</Panel>

<DropShadowExtender
    TargetControlID="pnlRounded"
    Opacity="0.6"
    Width="8"
    Rounded="true"
    Radius="15" />
```

### Prominent Shadow Effect

```razor
@rendermode InteractiveServer

<Panel ID="pnlHighlight" style="width: 350px; padding: 25px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white;">
    <h2>Featured Content</h2>
    <p>This panel uses a prominent shadow for emphasis.</p>
</Panel>

<DropShadowExtender
    TargetControlID="pnlHighlight"
    Opacity="0.8"
    Width="12"
    Rounded="true"
    Radius="10" />
```

### Shadow with Position Tracking

```razor
@rendermode InteractiveServer

<Panel ID="pnlDynamic" style="width: 300px; padding: 20px; background-color: white; position: absolute; top: 50px; left: 50px;">
    <h3>Dynamic Position</h3>
    <p>This panel's shadow follows its position.</p>
</Panel>

<DropShadowExtender
    TargetControlID="pnlDynamic"
    Opacity="0.7"
    Width="6"
    TrackPosition="true" />

<script>
    // Move the panel around and the shadow follows
    var panel = document.getElementById('pnlDynamic');
    var offset = 10;
    function movePanel() {
        panel.style.top = (50 + offset) + 'px';
        panel.style.left = (50 + offset) + 'px';
        offset = (offset === 10) ? 20 : 10;
    }
</script>
```

## HTML Output

The DropShadowExtender produces a shadow element (typically a `<div>`) positioned behind the target element. The original element structure remains unchanged.

## JavaScript Interop

The DropShadowExtender loads `drop-shadow-extender.js` as an ES module. JavaScript handles:

- Creating shadow DOM elements
- Positioning shadows relative to the target
- Applying opacity and width settings
- Optional corner rounding with CSS
- Tracking element position if `TrackPosition` is enabled
- Responsive shadow updates on window resize

## Render Mode Requirements

The DropShadowExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. The element displays without a shadow.
- **JavaScript disabled:** Same as SSR — Shadow is not rendered.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:DropShadowExtender
   + <DropShadowExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Shadow properties stay the same**
   ```diff
   - Opacity="0.5"
   + Opacity="0.5"
   ```

### Before (Web Forms)

```html
<asp:Panel ID="pnlBox" runat="server" style="width: 300px; padding: 15px; background: white;">
    <p>Card content</p>
</asp:Panel>

<ajaxToolkit:DropShadowExtender
    ID="shadow1"
    TargetControlID="pnlBox"
    Opacity="0.5"
    Width="5"
    Rounded="true"
    Radius="10"
    runat="server" />
```

### After (Blazor)

```razor
<Panel ID="pnlBox" style="width: 300px; padding: 15px; background: white;">
    <p>Card content</p>
</Panel>

<DropShadowExtender
    TargetControlID="pnlBox"
    Opacity="0.5"
    Width="5"
    Rounded="true"
    Radius="10" />
```

## Best Practices

1. **Use subtle opacity** — Opacity between 0.4 and 0.6 typically looks best
2. **Match shadow width to design** — Narrow shadows (3-5px) for subtle effects, wider (8-12px) for emphasis
3. **Enable rounding for modern look** — Combine `Rounded="true"` with appropriate `Radius`
4. **Use TrackPosition for dynamic elements** — Enable if the element will change position via JavaScript
5. **Test on different backgrounds** — Shadows appear differently on light vs. dark backgrounds

## Troubleshooting

| Issue | Solution |
|---|---|
| Shadow not visible | Verify `TargetControlID` matches the element's ID. Ensure `@rendermode InteractiveServer` is set. |
| Shadow too faint | Increase `Opacity` (try 0.7 or 0.8) or increase `Width` for a more visible effect. |
| Shadow not following element | If the element moves via JavaScript, enable `TrackPosition="true"`. |
| Performance issues | Reduce `Opacity` and `Width` values, or disable `TrackPosition` if not needed. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [Panel Component](../LayoutControls/Panel.md) — The Panel control
- [RoundedCornersExtender](RoundedCornersExtender.md) — Apply rounded corners to elements
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
