# CalendarExtender

The **CalendarExtender** attaches a popup calendar date picker to a target TextBox. When the user focuses or clicks the TextBox, a calendar dropdown appears allowing date selection. It supports configurable date formats, navigation views, and date range constraints.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/CalendarExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the TextBox to attach the calendar to
- `Format` — Date format string used to display and parse dates
- `PopupPosition` — Position of the popup calendar relative to the target
- `DefaultView` — The default view when the calendar opens (Days, Months, Years)
- `StartDate` — The earliest selectable date
- `EndDate` — The latest selectable date
- `SelectedDate` — The currently selected date
- `TodaysDate` — The date highlighted as today
- `CssClass` — CSS class applied to the calendar popup container
- `OnClientDateSelectionChanged` — Client-side script invoked when a date is selected
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## CalendarPosition Enum

Controls the position of the popup calendar:

```csharp
enum CalendarPosition
{
    BottomLeft = 0,   // Below target, left-aligned (default)
    BottomRight = 1,  // Below target, right-aligned
    TopLeft = 2,      // Above target, left-aligned
    TopRight = 3,     // Above target, right-aligned
    Right = 4,        // To the right of target
    Left = 5          // To the left of target
}
```

## CalendarDefaultView Enum

Controls the initial view when the calendar opens:

```csharp
enum CalendarDefaultView
{
    Days = 0,    // Day grid (default)
    Months = 1,  // Month picker
    Years = 2    // Year picker
}
```

## Web Forms Syntax

```html
<asp:TextBox ID="txtDate" runat="server" />

<ajaxToolkit:CalendarExtender
    ID="cal1"
    runat="server"
    TargetControlID="txtDate"
    Format="MM/dd/yyyy"
    PopupPosition="BottomLeft"
    DefaultView="Days"
    StartDate="01/01/2020"
    EndDate="12/31/2030"
    OnClientDateSelectionChanged="onDateSelected" />
```

## Blazor Migration

```razor
<TextBox ID="txtDate" />

<CalendarExtender
    TargetControlID="txtDate"
    Format="MM/dd/yyyy"
    PopupPosition="CalendarPosition.BottomLeft"
    DefaultView="CalendarDefaultView.Days"
    StartDate="@(new DateTime(2020, 1, 1))"
    EndDate="@(new DateTime(2030, 12, 31))"
    OnClientDateSelectionChanged="onDateSelected" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Date constraints use `DateTime?` instead of strings. Everything else stays the same!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the TextBox to attach the calendar to |
| `Format` | `string` | `"d"` | Date format string used to display and parse dates (e.g., "MM/dd/yyyy") |
| `PopupPosition` | `CalendarPosition` | `BottomLeft` | Position of the popup calendar relative to the target |
| `DefaultView` | `CalendarDefaultView` | `Days` | The default view when the calendar opens |
| `StartDate` | `DateTime?` | `null` | The earliest selectable date; dates before this are disabled |
| `EndDate` | `DateTime?` | `null` | The latest selectable date; dates after this are disabled |
| `SelectedDate` | `DateTime?` | `null` | The currently selected date |
| `TodaysDate` | `DateTime?` | `null` | The date highlighted as today (defaults to current date if not set) |
| `CssClass` | `string` | `""` | CSS class applied to the calendar popup container |
| `OnClientDateSelectionChanged` | `string` | `""` | Client-side script invoked when a date is selected |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Basic Date Picker

```razor
@rendermode InteractiveServer

<TextBox ID="txtBirthDate" />

<CalendarExtender
    TargetControlID="txtBirthDate"
    Format="MM/dd/yyyy" />
```

### Date Picker with Range Constraints

```razor
@rendermode InteractiveServer

<TextBox ID="txtBooking" />

<CalendarExtender
    TargetControlID="txtBooking"
    Format="yyyy-MM-dd"
    StartDate="@DateTime.Today"
    EndDate="@DateTime.Today.AddYears(1)"
    PopupPosition="CalendarPosition.BottomRight" />
```

### Date Picker with Year View

```razor
@rendermode InteractiveServer

<TextBox ID="txtYear" />

<CalendarExtender
    TargetControlID="txtYear"
    Format="yyyy"
    DefaultView="CalendarDefaultView.Years" />
```

### Date Picker with JavaScript Callback

```razor
@rendermode InteractiveServer

<TextBox ID="txtEvent" />

<CalendarExtender
    TargetControlID="txtEvent"
    Format="MM/dd/yyyy"
    OnClientDateSelectionChanged="onDatePicked" />

<script>
    function onDatePicked() {
        var dateField = document.getElementById('txtEvent');
        console.log('Date selected:', dateField.value);
    }
</script>
```

## HTML Output

The CalendarExtender produces no HTML itself — it attaches JavaScript behavior to the target TextBox. When activated, the JavaScript module creates and manages the calendar popup in the DOM.

## JavaScript Interop

The CalendarExtender loads `calendar-extender.js` as an ES module. JavaScript handles:

- Rendering the calendar popup with day/month/year views
- Date navigation (previous/next month, year)
- Date selection and formatting
- Date range enforcement (disabling out-of-range dates)
- Popup positioning relative to the target control
- Closing the calendar on outside click or Escape key

## Render Mode Requirements

The CalendarExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. The TextBox works as a plain text input.
- **JavaScript disabled:** Same as SSR — TextBox functions without a calendar popup.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:CalendarExtender
   + <CalendarExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Date parameters use `DateTime?`** instead of strings
   ```diff
   - StartDate="01/01/2020"
   + StartDate="@(new DateTime(2020, 1, 1))"
   ```

4. **Enum values use full type names**
   ```diff
   - PopupPosition="BottomLeft"
   + PopupPosition="CalendarPosition.BottomLeft"
   ```

### Before (Web Forms)

```html
<asp:TextBox ID="txtDate" runat="server" />

<ajaxToolkit:CalendarExtender
    ID="cal1"
    TargetControlID="txtDate"
    Format="MM/dd/yyyy"
    runat="server" />
```

### After (Blazor)

```razor
<TextBox ID="txtDate" />

<CalendarExtender
    TargetControlID="txtDate"
    Format="MM/dd/yyyy" />
```

## Best Practices

1. **Specify a clear format** — Use `Format` to match your application's date convention
2. **Set date ranges** — Use `StartDate`/`EndDate` to prevent invalid date selection
3. **Position wisely** — Choose `PopupPosition` to avoid clipping by page edges
4. **Provide fallback** — The TextBox still accepts typed input when the calendar isn't available

## Troubleshooting

| Issue | Solution |
|---|---|
| Calendar not appearing | Verify `TargetControlID` matches the TextBox's `ID`. Ensure `@rendermode InteractiveServer` is set. |
| Wrong date format | Check the `Format` string matches your expected output (e.g., "MM/dd/yyyy"). |
| Dates outside range selectable | Verify `StartDate` and `EndDate` are set correctly as `DateTime?` values. |
| Calendar positioned off-screen | Try a different `PopupPosition` value or ensure the target has enough viewport space. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [MaskedEditExtender](MaskedEditExtender.md) — Input masking for date/time fields
- [TextBox Component](../EditorControls/TextBox.md) — The TextBox control
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
