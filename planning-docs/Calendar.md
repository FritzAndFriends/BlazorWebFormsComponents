# Calendar — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.calendar?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.Calendar`
**Implementation Status:** ✅ Implemented

## Properties

### Control-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| SelectedDate | DateTime | ✅ Match | Two-way bindable via SelectedDateChanged |
| SelectedDates | SelectedDatesCollection | ✅ Match | `IReadOnlyCollection<DateTime>` (read-only) |
| VisibleDate | DateTime | ✅ Match | Controls displayed month |
| SelectionMode | CalendarSelectionMode | ✅ Match | Enum: None, Day, DayWeek, DayWeekMonth |
| Caption | string | ✅ Match | Table caption text |
| CaptionAlign | TableCaptionAlign | ✅ Match | Enum for caption alignment |
| UseAccessibleHeader | bool | ✅ Match | Defaults to true |
| ShowTitle | bool | ✅ Match | Title section visibility |
| ShowGridLines | bool | ✅ Match | Grid line visibility |
| ShowDayHeader | bool | ✅ Match | Day name row visibility |
| ShowNextPrevMonth | bool | ✅ Match | Navigation arrows visibility |
| DayNameFormat | DayNameFormat | ⚠️ Needs Work | Uses string instead of enum (e.g., "Short", "Full") |
| TitleFormat | TitleFormat | ⚠️ Needs Work | Uses string instead of enum (e.g., "MonthYear", "Month") |
| NextMonthText | string | ✅ Match | Default "&gt;" |
| PrevMonthText | string | ✅ Match | Default "&lt;" |
| SelectWeekText | string | ✅ Match | Default "&gt;&gt;" |
| SelectMonthText | string | ✅ Match | Default "&gt;&gt;" |
| FirstDayOfWeek | FirstDayOfWeek | ✅ Match | Uses System.DayOfWeek |
| CellPadding | int | ✅ Match | Table cell padding |
| CellSpacing | int | ✅ Match | Table cell spacing |
| ToolTip | string | ✅ Match | Tooltip text |

### Style Sub-Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| DayStyle | TableItemStyle | ⚠️ Needs Work | Implemented as `DayStyleCss` string instead of TableItemStyle object |
| TitleStyle | TableItemStyle | ⚠️ Needs Work | Implemented as `TitleStyleCss` string instead of TableItemStyle object |
| DayHeaderStyle | TableItemStyle | ⚠️ Needs Work | Implemented as `DayHeaderStyleCss` string instead of TableItemStyle object |
| TodayDayStyle | TableItemStyle | ⚠️ Needs Work | Implemented as `TodayDayStyleCss` string instead of TableItemStyle object |
| SelectedDayStyle | TableItemStyle | ⚠️ Needs Work | Implemented as `SelectedDayStyleCss` string instead of TableItemStyle object |
| OtherMonthDayStyle | TableItemStyle | ⚠️ Needs Work | Implemented as `OtherMonthDayStyleCss` string instead of TableItemStyle object |
| WeekendDayStyle | TableItemStyle | ⚠️ Needs Work | Implemented as `WeekendDayStyleCss` string instead of TableItemStyle object |
| NextPrevStyle | TableItemStyle | ⚠️ Needs Work | Implemented as `NextPrevStyleCss` string instead of TableItemStyle object |
| SelectorStyle | TableItemStyle | ⚠️ Needs Work | Implemented as `SelectorStyleCss` string instead of TableItemStyle object |

### WebControl Inherited Properties (from BaseStyledComponent)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | string | 🔴 Missing | Not in BaseStyledComponent |
| BackColor | Color | ✅ Match | From BaseStyledComponent |
| BorderColor | Color | ✅ Match | From BaseStyledComponent |
| BorderStyle | BorderStyle | ✅ Match | From BaseStyledComponent |
| BorderWidth | Unit | ✅ Match | From BaseStyledComponent |
| CssClass | string | ✅ Match | From BaseStyledComponent |
| Enabled | bool | ✅ Match | From BaseWebFormsComponent |
| Font | FontInfo | ✅ Match | From BaseStyledComponent |
| ForeColor | Color | ✅ Match | From BaseStyledComponent |
| Height | Unit | ✅ Match | From BaseStyledComponent |
| Width | Unit | ✅ Match | From BaseStyledComponent |
| TabIndex | short | ✅ Match | From BaseWebFormsComponent |
| Style | CssStyleCollection | ✅ Match | Computed from BaseStyledComponent |

### Control Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | From BaseWebFormsComponent |
| ClientID | string | ✅ Match | From BaseWebFormsComponent |
| Visible | bool | ✅ Match | From BaseWebFormsComponent |
| EnableViewState | bool | N/A | Server-only |
| ViewState | StateBag | N/A | Server-only |
| EnableTheming | bool | N/A | Server-only |
| SkinID | string | N/A | Server-only |
| Page | Page | N/A | Server-only |
| NamingContainer | Control | N/A | Server-only |
| UniqueID | string | N/A | Server-only |
| ClientIDMode | ClientIDMode | N/A | Server-only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| SelectionChanged | EventHandler | ✅ Match | `EventCallback OnSelectionChanged` |
| DayRender | DayRenderEventHandler | ✅ Match | `EventCallback<CalendarDayRenderArgs> OnDayRender` |
| VisibleMonthChanged | MonthChangedEventHandler | ✅ Match | `EventCallback<CalendarMonthChangedArgs> OnVisibleMonthChanged` |
| SelectedDateChanged | — | ✅ Match | `EventCallback<DateTime> SelectedDateChanged` (Blazor two-way binding) |
| Init | EventHandler | ✅ Match | Via base class |
| Load | EventHandler | ✅ Match | Via base class |
| PreRender | EventHandler | ✅ Match | Via base class |
| Unload | EventHandler | ✅ Match | Via base class |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| Focus() | void | N/A | Server-only |
| DataBind() | void | N/A | Server-only |

## HTML Output Comparison

Web Forms renders a `<table>` with:
- Title row with month/year and navigation arrows
- Day header row with abbreviated day names
- 6 week rows with day cells
- Optional week/month selector column

The Blazor component matches this table structure. Grid lines are controlled via `border` attribute and `border-collapse` CSS. Cell padding and spacing use HTML attributes.

Note: Web Forms uses link-based navigation (`<a href="javascript:__doPostBack(...)">`) while Blazor uses `@onclick` handlers.

## Summary

- **Matching:** 27 properties, 8 events
- **Needs Work:** 11 properties (9 style sub-properties use CSS strings instead of TableItemStyle objects, DayNameFormat and TitleFormat use strings instead of enums)
- **Missing:** 1 property (AccessKey)
- **N/A (server-only):** 7 items
