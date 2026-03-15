# RoundedCornersExtender

The **RoundedCornersExtender** applies rounded corners to an element using CSS border-radius. It supports selecting which corners to round and optionally sets a background color for the rounded area, giving elements a modern, polished appearance.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/RoundedCornersExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the element to apply rounded corners to
- `Radius` — Corner radius in pixels
- `Corners` — Which corners to round (All, Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight, None)
- `Color` — Background color for the rounded area
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## BoxCorners Enum

Specifies which corners to round (can be combined):

```csharp
[Flags]
enum BoxCorners
{
    None = 0,
    TopLeft = 1,
    TopRight = 2,
    BottomLeft = 4,
    BottomRight = 8,
    Top = TopLeft | TopRight,      // Both top corners
    Bottom = BottomLeft | BottomRight,  // Both bottom corners
    Left = TopLeft | BottomLeft,   // Both left corners
    Right = TopRight | BottomRight, // Both right corners
    All = TopLeft | TopRight | BottomLeft | BottomRight  // All corners
}
```

## Web Forms Syntax

```html
<asp:Panel ID="pnlBox" runat="server" style="width: 300px; padding: 20px; background-color: white; border: 1px solid #ccc;">
    <p>This panel has rounded corners.</p>
</asp:Panel>

<ajaxToolkit:RoundedCornersExtender
    ID="rounded1"
    runat="server"
    TargetControlID="pnlBox"
    Radius="10"
    Corners="All"
    Color="#FFFFFF" />
```

## Blazor Migration

```razor
<Panel ID="pnlBox" style="width: 300px; padding: 20px; background-color: white; border: 1px solid #ccc;">
    <p>This panel has rounded corners.</p>
</Panel>

<RoundedCornersExtender
    TargetControlID="pnlBox"
    Radius="10"
    Corners="BoxCorners.All"
    Color="#FFFFFF" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Use enum type names for corners!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the element to apply rounded corners to |
| `Radius` | `int` | `5` | Corner radius in pixels |
| `Corners` | `BoxCorners` | `All` | Specifies which corners to round |
| `Color` | `string` | `""` | Background color for the rounded area (e.g., "#FFFFFF"); if empty, preserves element's background |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### All Corners Rounded

```razor
@rendermode InteractiveServer

<Panel ID="pnlCard" style="width: 300px; padding: 20px; background-color: white; border: 1px solid #ddd;">
    <h3>Card Title</h3>
    <p>This panel has all corners rounded.</p>
</Panel>

<RoundedCornersExtender
    TargetControlID="pnlCard"
    Radius="10"
    Corners="BoxCorners.All" />
```

### Top Corners Only

```razor
@rendermode InteractiveServer

<Panel ID="pnlHeader" style="width: 100%; padding: 15px; background: linear-gradient(to right, #2196F3, #1976D2); color: white;">
    <h2>Section Header</h2>
</Panel>

<RoundedCornersExtender
    TargetControlID="pnlHeader"
    Radius="8"
    Corners="BoxCorners.Top" />
```

### Bottom Corners with Color

```razor
@rendermode InteractiveServer

<Panel ID="pnlFooter" style="width: 100%; padding: 15px; background-color: #f5f5f5; border-top: 1px solid #ccc;">
    <p>&copy; 2024 My Company. All rights reserved.</p>
</Panel>

<RoundedCornersExtender
    TargetControlID="pnlFooter"
    Radius="10"
    Corners="BoxCorners.Bottom"
    Color="#f5f5f5" />
```

### Individual Corners

```razor
@rendermode InteractiveServer

<Panel ID="pnlCustom" style="width: 250px; padding: 20px; background-color: white; border: 2px solid #FF9800;">
    <p>Only bottom-right corner is rounded.</p>
</Panel>

<RoundedCornersExtender
    TargetControlID="pnlCustom"
    Radius="15"
    Corners="BoxCorners.BottomRight" />
```

### Large Radius for Button-Like Appearance

```razor
@rendermode InteractiveServer

<Panel ID="pnlButton" style="width: 200px; padding: 15px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; text-align: center; cursor: pointer;">
    <strong>Click Me!</strong>
</Panel>

<RoundedCornersExtender
    TargetControlID="pnlButton"
    Radius="25"
    Corners="BoxCorners.All" />
```

## HTML Output

The RoundedCornersExtender produces no HTML itself — it applies CSS border-radius styling to the target element.

## JavaScript Interop

The RoundedCornersExtender loads `rounded-corners-extender.js` as an ES module. JavaScript handles:

- Applying CSS border-radius based on specified corners
- Setting background color if provided
- Handling older browsers that don't support border-radius
- Calculating corner values based on the `BoxCorners` flags

## Render Mode Requirements

The RoundedCornersExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. Use CSS `border-radius` as fallback.
- **JavaScript disabled:** Same as SSR — Corners appear square unless CSS `border-radius` is present.

## Modern Alternative (CSS-Only)

For most use cases, you can apply rounded corners directly with CSS without needing this extender:

```razor
<!-- Modern approach: Use CSS border-radius directly -->
<Panel ID="pnlBox" style="width: 300px; padding: 20px; background-color: white; border: 1px solid #ccc; border-radius: 10px;">
    <p>Rounded corners via CSS.</p>
</Panel>
```

Use the RoundedCornersExtender primarily for dynamic scenarios or legacy component compatibility.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:RoundedCornersExtender
   + <RoundedCornersExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Use enum types for corners**
   ```diff
   - Corners="All"
   + Corners="BoxCorners.All"
   ```

### Before (Web Forms)

```html
<asp:Panel ID="pnlBox" runat="server" style="width: 300px; padding: 20px; background: white;">
    <p>Content</p>
</asp:Panel>

<ajaxToolkit:RoundedCornersExtender
    ID="rounded1"
    TargetControlID="pnlBox"
    Radius="10"
    Corners="All"
    runat="server" />
```

### After (Blazor)

```razor
<Panel ID="pnlBox" style="width: 300px; padding: 20px; background: white;">
    <p>Content</p>
</Panel>

<RoundedCornersExtender
    TargetControlID="pnlBox"
    Radius="10"
    Corners="BoxCorners.All" />
```

## Best Practices

1. **Use CSS border-radius for modern browsers** — The native CSS approach is simpler and more performant
2. **Choose appropriate radius** — 5-10px for subtle, 15-25px for more prominent rounding
3. **Match corner selection to design** — Use `Top` for headers, `Bottom` for footers, `All` for cards
4. **Test across browsers** — Most modern browsers support border-radius natively
5. **Combine with other effects** — Rounded corners pair well with shadows and borders

## Troubleshooting

| Issue | Solution |
|---|---|
| Corners not rounding | Verify `TargetControlID` matches the element's ID. Ensure `@rendermode InteractiveServer` is set. |
| Color shows gaps | The `Color` property is optional; ensure it matches or complements the element's background. |
| Performance issues | This extender is rarely needed. Use CSS `border-radius` instead for better performance. |
| Old browser support | Modern browsers support `border-radius` natively. This extender adds no value for modern web apps. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [Panel Component](../LayoutControls/Panel.md) — The Panel control
- [DropShadowExtender](DropShadowExtender.md) — Add drop shadows to elements
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
