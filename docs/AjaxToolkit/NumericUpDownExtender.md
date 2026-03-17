# NumericUpDownExtender

The **NumericUpDownExtender** adds numeric up/down spinner buttons to a target TextBox. Users can increment or decrement the value using the spinner buttons or by typing directly. It supports min/max range constraints, configurable step increments, and cycling through a reference value list.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/NumericUpDownExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the TextBox to enhance with spinner buttons
- `Width` — Width of the spinner control in pixels
- `Minimum` — The minimum allowed value
- `Maximum` — The maximum allowed value
- `Step` — Amount to increment/decrement on each step
- `RefValues` — Semicolon-separated list of values to cycle through
- `ServiceUpPath` — URL of the web service for the up action
- `ServiceUpMethod` — Web service method name for the up action
- `ServiceDownPath` — URL of the web service for the down action
- `ServiceDownMethod` — Web service method name for the down action
- `Tag` — Arbitrary string passed to the web service methods
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## Web Forms Syntax

```html
<asp:TextBox ID="txtQuantity" Text="1" runat="server" />

<ajaxToolkit:NumericUpDownExtender
    ID="nud1"
    runat="server"
    TargetControlID="txtQuantity"
    Width="120"
    Minimum="1"
    Maximum="100"
    Step="1" />
```

## Blazor Migration

```razor
<TextBox ID="txtQuantity" Text="1" />

<NumericUpDownExtender
    TargetControlID="txtQuantity"
    Width="120"
    Minimum="1"
    Maximum="100"
    Step="1" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Everything else stays the same!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the TextBox to enhance with spinner buttons |
| `Width` | `int` | `100` | Width of the spinner control in pixels |
| `Minimum` | `double` | `double.MinValue` | The minimum allowed value |
| `Maximum` | `double` | `double.MaxValue` | The maximum allowed value |
| `Step` | `double` | `1` | Amount to increment or decrement on each step |
| `RefValues` | `string` | `""` | Semicolon-separated list of values to cycle through instead of numeric stepping |
| `ServiceUpPath` | `string` | `""` | URL of the web service for the up action |
| `ServiceUpMethod` | `string` | `""` | Web service method name for the up action |
| `ServiceDownPath` | `string` | `""` | URL of the web service for the down action |
| `ServiceDownMethod` | `string` | `""` | Web service method name for the down action |
| `Tag` | `string` | `""` | Arbitrary string passed to the web service methods |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Basic Numeric Spinner

```razor
@rendermode InteractiveServer

<TextBox ID="txtQty" Text="1" />

<NumericUpDownExtender
    TargetControlID="txtQty"
    Minimum="0"
    Maximum="99"
    Step="1" />
```

### Decimal Step Spinner

```razor
@rendermode InteractiveServer

<TextBox ID="txtPrice" Text="0.00" />

<NumericUpDownExtender
    TargetControlID="txtPrice"
    Minimum="0"
    Maximum="999.99"
    Step="0.50"
    Width="150" />
```

### Cycling Through Reference Values

Instead of numeric stepping, cycle through a list of predefined values:

```razor
@rendermode InteractiveServer

<TextBox ID="txtMonth" Text="January" />

<NumericUpDownExtender
    TargetControlID="txtMonth"
    RefValues="January;February;March;April;May;June;July;August;September;October;November;December"
    Width="150" />
```

### Spinner with Custom Width

```razor
@rendermode InteractiveServer

<label>Quantity:</label>
<TextBox ID="txtOrderQty" Text="1" />

<NumericUpDownExtender
    TargetControlID="txtOrderQty"
    Minimum="1"
    Maximum="50"
    Step="1"
    Width="80" />
```

## HTML Output

The NumericUpDownExtender produces no HTML itself — it attaches JavaScript behavior to the target TextBox, adding spinner buttons and keyboard increment/decrement support.

## JavaScript Interop

The NumericUpDownExtender loads `numeric-updown-extender.js` as an ES module. JavaScript handles:

- Rendering up/down spinner buttons adjacent to the TextBox
- Incrementing/decrementing the value on button click
- Enforcing min/max range constraints
- Cycling through reference values when `RefValues` is set
- Optional web service calls for custom up/down logic

## Render Mode Requirements

The NumericUpDownExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. The TextBox works as a plain text input.
- **JavaScript disabled:** Same as SSR — TextBox functions without spinner buttons.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:NumericUpDownExtender
   + <NumericUpDownExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Update service paths** if using web service integration
   ```diff
   - ServiceUpPath="~/Services/Counter.asmx"
   + ServiceUpPath="/api/counter"
   ```

### Before (Web Forms)

```html
<asp:TextBox ID="txtQty" Text="1" runat="server" />

<ajaxToolkit:NumericUpDownExtender
    ID="nud1"
    TargetControlID="txtQty"
    Minimum="1"
    Maximum="100"
    Step="1"
    Width="120"
    runat="server" />
```

### After (Blazor)

```razor
<TextBox ID="txtQty" Text="1" />

<NumericUpDownExtender
    TargetControlID="txtQty"
    Minimum="1"
    Maximum="100"
    Step="1"
    Width="120" />
```

## Best Practices

1. **Set sensible min/max** — Prevent out-of-range values
2. **Choose an appropriate step** — Match the precision your field requires
3. **Use RefValues for non-numeric data** — Months, sizes, priorities, etc.
4. **Set Width to match** — Ensure the TextBox is wide enough for the value plus spinner buttons

## Troubleshooting

| Issue | Solution |
|---|---|
| Spinner not appearing | Verify `TargetControlID` matches the TextBox's `ID`. Ensure `@rendermode InteractiveServer` is set. |
| Value exceeds range | Verify `Minimum` and `Maximum` are set correctly. |
| RefValues not cycling | Ensure values are separated by semicolons (`;`). |
| Decimal stepping inaccurate | Use appropriate `Step` values (e.g., 0.01, 0.1, 0.5). |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [SliderExtender](SliderExtender.md) — Range slider alternative
- [MaskedEditExtender](MaskedEditExtender.md) — Input masking for numeric fields
- [TextBox Component](../EditorControls/TextBox.md) — The TextBox control
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
