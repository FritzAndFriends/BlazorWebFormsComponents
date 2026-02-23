# Calendar

The Calendar component provides a Blazor implementation of the ASP.NET Web Forms Calendar control, enabling users to select dates and navigate through months.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.calendar?view=netframework-4.8

## Features Supported in Blazor

- `SelectedDate` property for single date selection
- `SelectedDates` collection for multi-date selection (read-only)
- `VisibleDate` property to set the displayed month
- `SelectionMode` (None, Day, DayWeek, DayWeekMonth)
- Two-way binding with `@bind-SelectedDate`
- Month/year navigation with next/previous buttons
- `OnSelectionChanged` event when a date is selected
- `OnDayRender` event for customizing individual days
- `OnVisibleMonthChanged` event when the month changes
- Customizable display options:
  - `ShowTitle` - Show/hide title bar
  - `ShowDayHeader` - Show/hide day name headers
  - `ShowGridLines` - Show/hide grid borders
  - `ShowNextPrevMonth` - Show/hide navigation
- Day name formatting (`DayNameFormat`: Full, Short, FirstLetter, FirstTwoLetters, Shortest)
- Title formatting (`TitleFormat`: Month, MonthYear)
- Customizable navigation text (`NextMonthText`, `PrevMonthText`, `SelectWeekText`, `SelectMonthText`)
- First day of week configuration (`FirstDayOfWeek`)
- Cell padding and spacing options
- **TableItemStyle sub-components** for rich styling (preferred):
  - `<CalendarDayStyle>` - Regular day cells
  - `<CalendarTitleStyle>` - Title bar
  - `<CalendarDayHeaderStyle>` - Day name headers
  - `<CalendarTodayDayStyle>` - Today's date cell
  - `<CalendarSelectedDayStyle>` - Selected date cell
  - `<CalendarOtherMonthDayStyle>` - Days from adjacent months
  - `<CalendarWeekendDayStyle>` - Weekend day cells
  - `<CalendarNextPrevStyle>` - Next/previous navigation links
  - `<CalendarSelectorStyle>` - Week/month selector column
- Each TableItemStyle sub-component supports: `CssClass`, `BackColor`, `ForeColor`, `BorderColor`, `BorderStyle`, `BorderWidth`, `Height`, `Width`, `HorizontalAlign`, `VerticalAlign`, `Wrap`, `Font-Bold`, `Font-Italic`, `Font-Size`, `Font-Name`, etc.
- Legacy CSS string properties (deprecated but still functional):
  - `TitleStyleCss`, `DayHeaderStyleCss`, `DayStyleCss`, `TodayDayStyleCss`, `SelectedDayStyleCss`, `OtherMonthDayStyleCss`, `WeekendDayStyleCss`, `NextPrevStyleCss`, `SelectorStyleCss`
- `Visible` property to show/hide the calendar
- `CssClass` for custom CSS styling
- `ToolTip` for accessibility

## Web Forms Features NOT Supported

- `DayRender` event cannot add custom controls to cells (Blazor limitation)
- `Caption` and `CaptionAlign` properties not implemented
- `TodaysDate` property not implemented (use `DateTime.Today`)
- `UseAccessibleHeader` not implemented

!!! note "Style Properties Migration"
    Individual style objects (`DayStyle`, `TitleStyle`, etc.) from Web Forms are now supported via **TableItemStyle sub-components** such as `<CalendarDayStyle>`, `<CalendarTitleStyle>`, etc. The older CSS string properties (`DayStyleCss`, `TitleStyleCss`, etc.) still work but are **deprecated**. See the [migration example](#migrating-from-css-string-properties-to-tableitemstyle) below.

## Web Forms Declarative Syntax

```html
<asp:Calendar
    CellPadding="2"
    CellSpacing="0"
    DayNameFormat="Short|Full|FirstLetter|FirstTwoLetters|Shortest"
    FirstDayOfWeek="Sunday|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Default"
    ID="string"
    NextMonthText="string"
    NextPrevFormat="ShortMonth|FullMonth|CustomText"
    OnDayRender="DayRenderEventHandler"
    OnSelectionChanged="EventHandler"
    OnVisibleMonthChanged="MonthChangedEventHandler"
    PrevMonthText="string"
    SelectedDate="DateTime"
    SelectionMode="None|Day|DayWeek|DayWeekMonth"
    SelectMonthText="string"
    SelectWeekText="string"
    ShowDayHeader="True|False"
    ShowGridLines="True|False"
    ShowNextPrevMonth="True|False"
    ShowTitle="True|False"
    TitleFormat="Month|MonthYear"
    VisibleDate="DateTime"
    runat="server">
    <DayStyle />
    <TodayDayStyle />
    <SelectedDayStyle />
    <OtherMonthDayStyle />
    <WeekendDayStyle />
    <TitleStyle />
    <NextPrevStyle />
    <DayHeaderStyle />
    <SelectorStyle />
</asp:Calendar>
```

## Blazor Declarative Syntax

```razor
<Calendar
    CellPadding="2"
    CellSpacing="0"
    DayNameFormat="Short|Full|FirstLetter|FirstTwoLetters|Shortest"
    FirstDayOfWeek="Sunday|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday"
    ID="string"
    NextMonthText="string"
    OnDayRender="DayRenderEventHandler"
    OnSelectionChanged="EventHandler"
    OnVisibleMonthChanged="MonthChangedEventHandler"
    PrevMonthText="string"
    @bind-SelectedDate="dateVariable"
    SelectionMode="None|Day|DayWeek|DayWeekMonth"
    SelectMonthText="string"
    SelectWeekText="string"
    ShowDayHeader="True|False"
    ShowGridLines="True|False"
    ShowNextPrevMonth="True|False"
    ShowTitle="True|False"
    TitleFormat="Month|MonthYear"
    VisibleDate="DateTime"
    CssClass="string">

    @* TableItemStyle sub-components (preferred) *@
    <CalendarDayStyle CssClass="string" BackColor="string" ForeColor="string" />
    <CalendarTitleStyle CssClass="string" BackColor="string" ForeColor="string" Font-Bold="true" />
    <CalendarDayHeaderStyle CssClass="string" BackColor="string" ForeColor="string" />
    <CalendarTodayDayStyle CssClass="string" BackColor="string" ForeColor="string" />
    <CalendarSelectedDayStyle CssClass="string" BackColor="string" ForeColor="string" />
    <CalendarOtherMonthDayStyle CssClass="string" ForeColor="string" />
    <CalendarWeekendDayStyle CssClass="string" BackColor="string" />
    <CalendarNextPrevStyle CssClass="string" ForeColor="string" />
    <CalendarSelectorStyle CssClass="string" BackColor="string" />

</Calendar>
```

!!! warning "Deprecated CSS String Properties"
    The following properties still work but are deprecated. Use the TableItemStyle sub-components above instead:
    `TitleStyleCss`, `DayStyleCss`, `TodayDayStyleCss`, `SelectedDayStyleCss`, `OtherMonthDayStyleCss`, `WeekendDayStyleCss`, `DayHeaderStyleCss`, `NextPrevStyleCss`, `SelectorStyleCss`

## Usage Examples

### Basic Calendar

```razor
@page "/calendar-demo"

<h3>Select a Date</h3>
<Calendar @bind-SelectedDate="selectedDate" />
<p>You selected: @selectedDate.ToShortDateString()</p>

@code {
    private DateTime selectedDate = DateTime.Today;
}
```

### Calendar with Week Selection

```razor
<Calendar SelectionMode="DayWeek" 
          @bind-SelectedDate="weekStart"
          OnSelectionChanged="HandleSelection" />

@code {
    private DateTime weekStart = DateTime.Today;
    
    private void HandleSelection()
    {
        // User selected a week starting at weekStart
    }
}
```

### Calendar with Month Selection

```razor
<Calendar SelectionMode="DayWeekMonth" 
          @bind-SelectedDate="monthStart" />

@code {
    private DateTime monthStart = DateTime.Today;
}
```

### Customized Calendar

```razor
<Calendar ShowGridLines="true"
          DayNameFormat="Full"
          TitleFormat="MonthYear"
          FirstDayOfWeek="DayOfWeek.Monday"
          NextMonthText="Next →"
          PrevMonthText="← Prev"
          @bind-SelectedDate="selectedDate" />
```

### Styled Calendar (using TableItemStyle sub-components)

```razor
<Calendar @bind-SelectedDate="selectedDate">
    <CalendarTitleStyle BackColor="#007bff" ForeColor="White" Font-Bold="true" />
    <CalendarSelectedDayStyle BackColor="#28a745" ForeColor="White" />
    <CalendarTodayDayStyle BackColor="#ffc107" Font-Bold="true" />
    <CalendarWeekendDayStyle BackColor="#f8f9fa" />
    <CalendarOtherMonthDayStyle ForeColor="#ccc" />
    <CalendarDayHeaderStyle BackColor="#e9ecef" Font-Bold="true" />
    <CalendarNextPrevStyle ForeColor="White" />
</Calendar>
```

### Styled Calendar (legacy CSS string properties — deprecated)

```razor
<Calendar CssClass="my-calendar"
          TitleStyleCss="calendar-title"
          SelectedDayStyleCss="selected-date"
          TodayDayStyleCss="today-date"
          WeekendDayStyleCss="weekend-date"
          @bind-SelectedDate="selectedDate" />

<style>
.my-calendar {
    border: 1px solid #ccc;
}
.calendar-title {
    background-color: #007bff;
    color: white;
    font-weight: bold;
}
.selected-date {
    background-color: #28a745;
    color: white;
}
.today-date {
    background-color: #ffc107;
    font-weight: bold;
}
.weekend-date {
    background-color: #f8f9fa;
}
</style>
```

### Calendar with Event Handlers

```razor
<Calendar @bind-SelectedDate="selectedDate"
          OnSelectionChanged="HandleSelectionChanged"
          OnVisibleMonthChanged="HandleMonthChanged"
          OnDayRender="HandleDayRender" />

<p>Selection count: @selectionCount</p>
<p>Current month: @currentMonth.ToString("MMMM yyyy")</p>

@code {
    private DateTime selectedDate = DateTime.Today;
    private int selectionCount = 0;
    private DateTime currentMonth = DateTime.Today;

    private void HandleSelectionChanged()
    {
        selectionCount++;
    }

    private void HandleMonthChanged(CalendarMonthChangedArgs args)
    {
        currentMonth = args.CurrentMonth;
    }

    private void HandleDayRender(CalendarDayRenderArgs args)
    {
        // Disable Sundays
        if (args.Date.DayOfWeek == DayOfWeek.Sunday)
        {
            args.IsSelectable = false;
        }

        // Disable past dates
        if (args.Date < DateTime.Today)
        {
            args.IsSelectable = false;
        }
    }
}
```

### Display Specific Month

```razor
<Calendar VisibleDate="specificMonth"
          @bind-SelectedDate="selectedDate" />

@code {
    private DateTime specificMonth = new DateTime(2024, 12, 1);
    private DateTime selectedDate = DateTime.Today;
}
```

### Read-Only Calendar (No Selection)

```razor
<Calendar SelectionMode="None" 
          VisibleDate="@DateTime.Today" />
```

## Migration Notes

### From Web Forms to Blazor

**Web Forms:**
```aspx
<asp:Calendar ID="Calendar1" 
              runat="server" 
              OnSelectionChanged="Calendar1_SelectionChanged">
    <SelectedDayStyle BackColor="Blue" ForeColor="White" />
</asp:Calendar>
```

```csharp
protected void Calendar1_SelectionChanged(object sender, EventArgs e)
{
    DateTime selected = Calendar1.SelectedDate;
}
```

**Blazor:**
```razor
<Calendar @bind-SelectedDate="selectedDate"
          OnSelectionChanged="HandleSelectionChanged">
    <CalendarSelectedDayStyle BackColor="Blue" ForeColor="White" />
</Calendar>
```

```csharp
@code {
    private DateTime selectedDate = DateTime.Today;

    private void HandleSelectionChanged()
    {
        // Date is available in selectedDate variable
    }
}
```

### Migrating from CSS String Properties to TableItemStyle

If you previously used the CSS string properties, migrate to the new sub-components:

**Before (deprecated):**
```razor
<Calendar TitleStyleCss="calendar-title"
          SelectedDayStyleCss="selected-date"
          TodayDayStyleCss="today-date"
          WeekendDayStyleCss="weekend-date"
          DayHeaderStyleCss="header-style"
          @bind-SelectedDate="selectedDate" />
```

**After (preferred):**
```razor
<Calendar @bind-SelectedDate="selectedDate">
    <CalendarTitleStyle CssClass="calendar-title" />
    <CalendarSelectedDayStyle CssClass="selected-date" />
    <CalendarTodayDayStyle CssClass="today-date" />
    <CalendarWeekendDayStyle CssClass="weekend-date" />
    <CalendarDayHeaderStyle CssClass="header-style" />
</Calendar>
```

!!! tip "Best Practice"
    The TableItemStyle sub-components also support inline style properties like `BackColor`, `ForeColor`, and `Font-Bold` — matching the Web Forms `<DayStyle>`, `<TitleStyle>`, etc. child elements. This makes migration from Web Forms markup even more direct.

### Key Differences

1. **Style Properties**: Use TableItemStyle sub-components (`<CalendarDayStyle>`, `<CalendarTitleStyle>`, etc.) for a direct match to Web Forms style child elements, or use `CssClass` for CSS-based styling. Legacy CSS string properties (`DayStyleCss`, etc.) are deprecated.
2. **DayNameFormat / TitleFormat**: Use enum values directly — `DayNameFormat="Full"`, `DayNameFormat="Short"`, `DayNameFormat="FirstLetter"`, `DayNameFormat="FirstTwoLetters"`, `DayNameFormat="Shortest"` for day names; `TitleFormat="Month"` or `TitleFormat="MonthYear"` for the title.
3. **Event Handlers**: Use EventCallback pattern instead of event delegates
4. **Data Binding**: Use `@bind-SelectedDate` for two-way binding
5. **Day Rendering**: The `OnDayRender` event provides day information but cannot inject custom HTML into cells

## Common Scenarios

### Date Range Picker

```razor
<h4>Start Date</h4>
<Calendar @bind-SelectedDate="startDate" />

<h4>End Date</h4>
<Calendar @bind-SelectedDate="endDate" OnDayRender="HandleEndDateDayRender" />

@code {
    private DateTime startDate = DateTime.Today;
    private DateTime endDate = DateTime.Today.AddDays(7);

    private void HandleEndDateDayRender(CalendarDayRenderArgs args)
    {
        // Disable dates before start date
        if (args.Date < startDate)
        {
            args.IsSelectable = false;
        }
    }
}
```

### Holiday Calendar

```razor
<Calendar @bind-SelectedDate="selectedDate"
          OnDayRender="HandleHolidayRender"
          WeekendDayStyleCss="weekend"
          OtherMonthDayStyleCss="other-month" />

@code {
    private DateTime selectedDate = DateTime.Today;
    private List<DateTime> holidays = new List<DateTime>
    {
        new DateTime(2024, 1, 1),  // New Year
        new DateTime(2024, 7, 4),  // Independence Day
        new DateTime(2024, 12, 25) // Christmas
    };

    private void HandleHolidayRender(CalendarDayRenderArgs args)
    {
        if (holidays.Contains(args.Date))
        {
            args.IsSelectable = false;
        }
    }
}
```

## See Also

- [TextBox](TextBox.md) - For alternative date input using `TextBoxMode.Date`
- [Button](Button.md) - For submitting forms with selected dates
- [Panel](Panel.md) - For grouping calendar with related controls
