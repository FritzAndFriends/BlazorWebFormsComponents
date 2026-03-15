# SliderExtender

The **SliderExtender** attaches range slider behavior to a target input element. Users can drag a handle along a rail to select a value within a range. It supports horizontal and vertical orientation, bound control synchronization, discrete steps, and customizable rail/handle appearance via CSS classes or image URLs.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/SliderExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the input element to enhance with slider behavior
- `Minimum` — Minimum value of the slider range
- `Maximum` — Maximum value of the slider range
- `Steps` — Number of discrete steps (0 = continuous)
- `Value` — Current value of the slider
- `BoundControlID` — ID of another element synchronized with the slider value
- `Orientation` — Horizontal or Vertical orientation
- `RailCssClass` — CSS class for the slider rail/track
- `HandleCssClass` — CSS class for the slider handle
- `HandleImageUrl` — URL of an image to use for the handle
- `Length` — Length of the slider in pixels
- `Decimals` — Number of decimal places for the slider value
- `TooltipText` — Tooltip template with `{0}` placeholder for the current value
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## SliderOrientation Enum

```csharp
enum SliderOrientation
{
    Horizontal = 0,  // Left-to-right slider (default)
    Vertical = 1     // Bottom-to-top slider
}
```

## Web Forms Syntax

```html
<asp:TextBox ID="txtSlider" runat="server" />

<ajaxToolkit:SliderExtender
    ID="se1"
    runat="server"
    TargetControlID="txtSlider"
    Minimum="0"
    Maximum="100"
    Steps="10"
    RailCssClass="slider-rail"
    HandleCssClass="slider-handle"
    BoundControlID="lblValue"
    TooltipText="Value: {0}" />

<asp:Label ID="lblValue" runat="server" />
```

## Blazor Migration

```razor
<TextBox ID="txtSlider" />

<SliderExtender
    TargetControlID="txtSlider"
    Minimum="0"
    Maximum="100"
    Steps="10"
    RailCssClass="slider-rail"
    HandleCssClass="slider-handle"
    BoundControlID="lblValue"
    TooltipText="Value: {0}" />

<Label ID="lblValue" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Everything else stays the same!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the input element to enhance with slider behavior |
| `Minimum` | `double` | `0` | Minimum value of the slider range |
| `Maximum` | `double` | `100` | Maximum value of the slider range |
| `Steps` | `int` | `0` | Number of discrete steps in the range; 0 means continuous |
| `Value` | `double` | `0` | Current value of the slider |
| `BoundControlID` | `string` | `""` | ID of another element whose value is synchronized with the slider |
| `Orientation` | `SliderOrientation` | `Horizontal` | Slider orientation: `Horizontal` or `Vertical` |
| `RailCssClass` | `string` | `""` | CSS class applied to the slider rail/track |
| `HandleCssClass` | `string` | `""` | CSS class applied to the slider handle |
| `HandleImageUrl` | `string` | `""` | URL of an image to use for the slider handle |
| `Length` | `int` | `0` | Length of the slider in pixels |
| `Decimals` | `int` | `0` | Number of decimal places for the slider value |
| `TooltipText` | `string` | `""` | Tooltip template displayed on hover; use `{0}` for the current value |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Basic Horizontal Slider

```razor
@rendermode InteractiveServer

<TextBox ID="txtVolume" />

<SliderExtender
    TargetControlID="txtVolume"
    Minimum="0"
    Maximum="100"
    Value="50"
    TooltipText="Volume: {0}%" />
```

### Slider with Discrete Steps

```razor
@rendermode InteractiveServer

<TextBox ID="txtRating" />

<SliderExtender
    TargetControlID="txtRating"
    Minimum="1"
    Maximum="5"
    Steps="5"
    Value="3"
    TooltipText="Rating: {0} stars" />
```

### Slider with Bound Label

```razor
@rendermode InteractiveServer

<TextBox ID="txtBrightness" />

<SliderExtender
    TargetControlID="txtBrightness"
    Minimum="0"
    Maximum="100"
    BoundControlID="lblBrightness"
    RailCssClass="brightness-rail"
    HandleCssClass="brightness-handle"
    TooltipText="{0}%" />

<Label ID="lblBrightness" Text="50" />

<style>
    .brightness-rail {
        width: 300px; height: 6px;
        background: linear-gradient(to right, #333, #fff);
        border-radius: 3px;
    }
    .brightness-handle {
        width: 20px; height: 20px;
        background: #007bff; border-radius: 50%;
        cursor: pointer;
    }
</style>
```

### Vertical Slider

```razor
@rendermode InteractiveServer

<TextBox ID="txtTemperature" />

<SliderExtender
    TargetControlID="txtTemperature"
    Minimum="60"
    Maximum="90"
    Steps="0"
    Decimals="1"
    Orientation="SliderOrientation.Vertical"
    Length="200"
    TooltipText="{0}°F" />
```

### Decimal Precision Slider

```razor
@rendermode InteractiveServer

<TextBox ID="txtOpacity" />

<SliderExtender
    TargetControlID="txtOpacity"
    Minimum="0"
    Maximum="1"
    Value="0.5"
    Decimals="2"
    TooltipText="Opacity: {0}" />
```

## HTML Output

The SliderExtender produces no HTML itself — it attaches JavaScript behavior to the target input, creating a visual slider rail and handle in the DOM.

## JavaScript Interop

The SliderExtender loads `slider-extender.js` as an ES module. JavaScript handles:

- Rendering the slider rail and handle elements
- Mouse/touch drag interaction for value selection
- Snapping to discrete steps when `Steps > 0`
- Synchronizing the bound control's value
- Tooltip display with formatted value
- Horizontal and vertical orientation layout

## Render Mode Requirements

The SliderExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. The input works as a plain text field.
- **JavaScript disabled:** Same as SSR — input functions without slider behavior.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:SliderExtender
   + <SliderExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Use full enum type name for Orientation**
   ```diff
   - Orientation="Vertical"
   + Orientation="SliderOrientation.Vertical"
   ```

### Before (Web Forms)

```html
<asp:TextBox ID="txtSlider" runat="server" />

<ajaxToolkit:SliderExtender
    ID="se1"
    TargetControlID="txtSlider"
    Minimum="0"
    Maximum="100"
    Steps="10"
    BoundControlID="lblValue"
    runat="server" />
```

### After (Blazor)

```razor
<TextBox ID="txtSlider" />

<SliderExtender
    TargetControlID="txtSlider"
    Minimum="0"
    Maximum="100"
    Steps="10"
    BoundControlID="lblValue" />
```

## Best Practices

1. **Set a meaningful range** — Use `Minimum`/`Maximum` that match your data
2. **Use Steps for discrete values** — Ratings, percentages in increments, etc.
3. **Provide a tooltip** — `TooltipText` with `{0}` gives users feedback while dragging
4. **Bind to a label** — `BoundControlID` shows the current value next to the slider
5. **Style the rail and handle** — Use `RailCssClass` and `HandleCssClass` for visual consistency

## Troubleshooting

| Issue | Solution |
|---|---|
| Slider not appearing | Verify `TargetControlID` matches the input's `ID`. Ensure `@rendermode InteractiveServer` is set. |
| Handle not draggable | Check for CSS `overflow: hidden` on parent containers that may clip the slider. |
| Value not updating | Verify `BoundControlID` matches the label's `ID`. Check that `Steps` isn't preventing the desired value. |
| Tooltip not showing | Ensure `TooltipText` is set with `{0}` placeholder. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [NumericUpDownExtender](NumericUpDownExtender.md) — Spinner button alternative
- [TextBox Component](../EditorControls/TextBox.md) — The TextBox control
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
