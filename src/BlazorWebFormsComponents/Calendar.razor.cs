using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	public partial class Calendar : BaseStyledComponent, ICalendarStyleContainer
	{
		private DateTime _visibleMonth;
		private readonly HashSet<DateTime> _selectedDays = new HashSet<DateTime>();
		private bool _initialized = false;

		public Calendar()
		{
			_visibleMonth = DateTime.Today;
			SelectedDate = DateTime.MinValue;
		}

		#region Properties

		/// <summary>
		/// Gets or sets the selected date.
		/// </summary>
		[Parameter]
		public DateTime SelectedDate { get; set; }

		/// <summary>
		/// Event raised when the selection changes.
		/// </summary>
		[Parameter]
		public EventCallback<DateTime> SelectedDateChanged { get; set; }

		/// <summary>
		/// Gets the collection of selected dates for multi-selection.
		/// </summary>
		public IReadOnlyCollection<DateTime> SelectedDates => _selectedDays.ToList().AsReadOnly();

		/// <summary>
		/// Gets or sets the month to display.
		/// </summary>
		[Parameter]
		public DateTime VisibleDate 
		{ 
			get => _visibleMonth;
			set 
			{
				if (_visibleMonth != value)
				{
					var oldDate = _visibleMonth;
					_visibleMonth = value;
					if (_initialized && OnVisibleMonthChanged.HasDelegate)
					{
						_ = OnVisibleMonthChanged.InvokeAsync(new CalendarMonthChangedArgs 
						{ 
							CurrentMonth = value, 
							PreviousMonth = oldDate 
						});
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the selection mode of the Calendar control.
		/// </summary>
		[Parameter]
		public CalendarSelectionMode SelectionMode { get; set; } = CalendarSelectionMode.Day;

		/// <summary>
		/// Gets or sets the text displayed as the caption of the calendar table.
		/// </summary>
		[Parameter]
		public string Caption { get; set; }

		/// <summary>
		/// Gets or sets the alignment of the caption relative to the calendar table.
		/// </summary>
		[Parameter]
		public TableCaptionAlign CaptionAlign { get; set; } = TableCaptionAlign.NotSet;

		/// <summary>
		/// Gets or sets whether the calendar renders accessible table headers using scope attributes.
		/// </summary>
		[Parameter]
		public bool UseAccessibleHeader { get; set; } = true;

		/// <summary>
		/// Event raised when a day is rendered, allowing customization.
		/// </summary>
		[Parameter]
		public EventCallback<CalendarDayRenderArgs> OnDayRender { get; set; }

		/// <summary>
		/// Event raised when the selection changes.
		/// </summary>
		[Parameter]
		public EventCallback OnSelectionChanged { get; set; }

		/// <summary>
		/// Event raised when the visible month changes.
		/// </summary>
		[Parameter]
		public EventCallback<CalendarMonthChangedArgs> OnVisibleMonthChanged { get; set; }

		/// <summary>
		/// Shows or hides the title section.
		/// </summary>
		[Parameter]
		public bool ShowTitle { get; set; } = true;

		/// <summary>
		/// Shows or hides grid lines around days.
		/// </summary>
		[Parameter]
		public bool ShowGridLines { get; set; } = false;

		/// <summary>
		/// Shows or hides the day names row.
		/// </summary>
		[Parameter]
		public bool ShowDayHeader { get; set; } = true;

		/// <summary>
		/// Shows or hides next/previous month navigation.
		/// </summary>
		[Parameter]
		public bool ShowNextPrevMonth { get; set; } = true;

		/// <summary>
		/// Format for displaying day names.
		/// </summary>
		[Parameter]
		public DayNameFormat DayNameFormat { get; set; } = DayNameFormat.Short;

		/// <summary>
		/// Format for the title.
		/// </summary>
		[Parameter]
		public TitleFormat TitleFormat { get; set; } = TitleFormat.MonthYear;

		/// <summary>
		/// Text for next month link.
		/// </summary>
		[Parameter]
		public string NextMonthText { get; set; } = "&gt;";

		/// <summary>
		/// Text for previous month link.
		/// </summary>
		[Parameter]
		public string PrevMonthText { get; set; } = "&lt;";

		/// <summary>
		/// Text for selecting the entire week.
		/// </summary>
		[Parameter]
		public string SelectWeekText { get; set; } = "&gt;&gt;";

		/// <summary>
		/// Text for selecting the entire month.
		/// </summary>
		[Parameter]
		public string SelectMonthText { get; set; } = "&gt;&gt;";

		/// <summary>
		/// First day of the week.
		/// </summary>
		[Parameter]
		public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Sunday;

		/// <summary>
		/// Cell padding for the table.
		/// </summary>
		[Parameter]
		public int CellPadding { get; set; } = 2;

		/// <summary>
		/// Cell spacing for the table.
		/// </summary>
		[Parameter]
		public int CellSpacing { get; set; } = 0;

		// Legacy CSS string style properties (backward compatible)
#pragma warning disable CS0618
		[Parameter]
		[Obsolete("Use <CalendarTitleStyle> sub-component instead")]
		public string TitleStyleCss { get; set; }

		[Parameter]
		[Obsolete("Use <CalendarDayHeaderStyle> sub-component instead")]
		public string DayHeaderStyleCss { get; set; }

		[Parameter]
		[Obsolete("Use <CalendarDayStyle> sub-component instead")]
		public string DayStyleCss { get; set; }

		[Parameter]
		[Obsolete("Use <CalendarTodayDayStyle> sub-component instead")]
		public string TodayDayStyleCss { get; set; }

		[Parameter]
		[Obsolete("Use <CalendarSelectedDayStyle> sub-component instead")]
		public string SelectedDayStyleCss { get; set; }

		[Parameter]
		[Obsolete("Use <CalendarOtherMonthDayStyle> sub-component instead")]
		public string OtherMonthDayStyleCss { get; set; }

		[Parameter]
		[Obsolete("Use <CalendarWeekendDayStyle> sub-component instead")]
		public string WeekendDayStyleCss { get; set; }

		[Parameter]
		[Obsolete("Use <CalendarNextPrevStyle> sub-component instead")]
		public string NextPrevStyleCss { get; set; }

		[Parameter]
		[Obsolete("Use <CalendarSelectorStyle> sub-component instead")]
		public string SelectorStyleCss { get; set; }
#pragma warning restore CS0618

		// TableItemStyle properties (ICalendarStyleContainer)
		public TableItemStyle DayStyle { get; internal set; } = new TableItemStyle();
		public TableItemStyle TitleStyle { get; internal set; } = new TableItemStyle();
		public TableItemStyle DayHeaderStyle { get; internal set; } = new TableItemStyle();
		public TableItemStyle TodayDayStyle { get; internal set; } = new TableItemStyle();
		public TableItemStyle SelectedDayStyle { get; internal set; } = new TableItemStyle();
		public TableItemStyle OtherMonthDayStyle { get; internal set; } = new TableItemStyle();
		public TableItemStyle WeekendDayStyle { get; internal set; } = new TableItemStyle();
		public TableItemStyle NextPrevStyle { get; internal set; } = new TableItemStyle();
		public TableItemStyle SelectorStyle { get; internal set; } = new TableItemStyle();

		// RenderFragment parameters for style sub-components
		[Parameter] public RenderFragment DayStyleContent { get; set; }
		[Parameter] public RenderFragment TitleStyleContent { get; set; }
		[Parameter] public RenderFragment DayHeaderStyleContent { get; set; }
		[Parameter] public RenderFragment TodayDayStyleContent { get; set; }
		[Parameter] public RenderFragment SelectedDayStyleContent { get; set; }
		[Parameter] public RenderFragment OtherMonthDayStyleContent { get; set; }
		[Parameter] public RenderFragment WeekendDayStyleContent { get; set; }
		[Parameter] public RenderFragment NextPrevStyleContent { get; set; }
		[Parameter] public RenderFragment SelectorStyleContent { get; set; }

		#endregion

		protected override void OnInitialized()
		{
			base.OnInitialized();
			_initialized = true;

			// Initialize selection if SelectedDate was provided
			if (SelectedDate != DateTime.MinValue)
			{
				_selectedDays.Add(SelectedDate.Date);
			}
		}

		private string GetTableStyle()
		{
			var baseStyle = Style ?? "";
			if (ShowGridLines)
			{
				baseStyle += "border-collapse:collapse;";
			}
			return string.IsNullOrWhiteSpace(baseStyle) ? null : baseStyle;
		}

		private string GetBorder()
		{
			return ShowGridLines ? "1" : null;
		}

		private string GetTitleText()
		{
			var format = TitleFormat == TitleFormat.Month ? "MMMM" : "MMMM yyyy";
			return _visibleMonth.ToString(format, CultureInfo.CurrentCulture);
		}

		private List<DayOfWeek> GetDayHeaders()
		{
			var days = new List<DayOfWeek>();
			var current = FirstDayOfWeek;
			for (var i = 0; i < 7; i++)
			{
				days.Add(current);
				current = (DayOfWeek)(((int)current + 1) % 7);
			}
			return days;
		}

		private string GetDayName(DayOfWeek day)
		{
			return DayNameFormat switch
			{
				Enums.DayNameFormat.Full => CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(day),
				Enums.DayNameFormat.FirstLetter => SafeSubstring(CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(day), 0, 1),
				Enums.DayNameFormat.FirstTwoLetters => SafeSubstring(CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(day), 0, 2),
				Enums.DayNameFormat.Shortest => CultureInfo.CurrentCulture.DateTimeFormat.GetShortestDayName(day),
				_ => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(day)
			};
		}

		private static string SafeSubstring(string str, int start, int length)
		{
			if (string.IsNullOrEmpty(str) || start >= str.Length)
				return str;
			
			return str.Substring(start, Math.Min(length, str.Length - start));
		}

		private List<List<DateTime>> GetCalendarWeeks()
		{
			var weeks = new List<List<DateTime>>();
			var firstOfMonth = new DateTime(_visibleMonth.Year, _visibleMonth.Month, 1);
			
			// Find the first day to display (may be from previous month)
			var startDate = firstOfMonth;
			while (startDate.DayOfWeek != FirstDayOfWeek)
			{
				startDate = startDate.AddDays(-1);
			}

			// Build 6 weeks of days
			for (var week = 0; week < 6; week++)
			{
				var weekDays = new List<DateTime>();
				for (var day = 0; day < 7; day++)
				{
					weekDays.Add(startDate);
					startDate = startDate.AddDays(1);
				}
				weeks.Add(weekDays);
			}

			return weeks;
		}

		private async Task HandlePreviousMonth()
		{
			VisibleDate = _visibleMonth.AddMonths(-1);
			await InvokeAsync(StateHasChanged);
		}

		private async Task HandleNextMonth()
		{
			VisibleDate = _visibleMonth.AddMonths(1);
			await InvokeAsync(StateHasChanged);
		}

		private async Task HandleDayClick(DateTime date)
		{
			if (SelectionMode == CalendarSelectionMode.None) return;

			_selectedDays.Clear();
			_selectedDays.Add(date.Date);
			SelectedDate = date.Date;

			if (SelectedDateChanged.HasDelegate)
			{
				await SelectedDateChanged.InvokeAsync(date.Date);
			}

			if (OnSelectionChanged.HasDelegate)
			{
				await OnSelectionChanged.InvokeAsync();
			}

			await InvokeAsync(StateHasChanged);
		}

		private async Task HandleWeekClick(List<DateTime> week)
		{
			if (SelectionMode != CalendarSelectionMode.DayWeek && SelectionMode != CalendarSelectionMode.DayWeekMonth) return;

			_selectedDays.Clear();
			foreach (var day in week)
			{
				_selectedDays.Add(day.Date);
			}
			SelectedDate = week[0].Date;

			if (SelectedDateChanged.HasDelegate)
			{
				await SelectedDateChanged.InvokeAsync(week[0].Date);
			}

			if (OnSelectionChanged.HasDelegate)
			{
				await OnSelectionChanged.InvokeAsync();
			}

			await InvokeAsync(StateHasChanged);
		}

		private async Task HandleMonthClick()
		{
			if (SelectionMode != CalendarSelectionMode.DayWeekMonth) return;

			_selectedDays.Clear();
			var firstOfMonth = new DateTime(_visibleMonth.Year, _visibleMonth.Month, 1);
			var lastOfMonth = firstOfMonth.AddMonths(1).AddDays(-1);
			
			for (var d = firstOfMonth; d <= lastOfMonth; d = d.AddDays(1))
			{
				_selectedDays.Add(d.Date);
			}
			SelectedDate = firstOfMonth.Date;

			if (SelectedDateChanged.HasDelegate)
			{
				await SelectedDateChanged.InvokeAsync(firstOfMonth.Date);
			}

			if (OnSelectionChanged.HasDelegate)
			{
				await OnSelectionChanged.InvokeAsync();
			}

			await InvokeAsync(StateHasChanged);
		}

		#pragma warning disable CS0612, CS0618
		private string GetDayCellCss(DateTime date)
		{
			var isToday = date.Date == DateTime.Today;
			var isSelected = _selectedDays.Contains(date.Date);
			var isOtherMonth = date.Month != _visibleMonth.Month;
			var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

			if (isSelected)
			{
				var css = GetEffectiveCss(SelectedDayStyle, SelectedDayStyleCss);
				if (!string.IsNullOrEmpty(css)) return css;
			}
			if (isToday)
			{
				var css = GetEffectiveCss(TodayDayStyle, TodayDayStyleCss);
				if (!string.IsNullOrEmpty(css)) return css;
			}
			if (isOtherMonth)
			{
				var css = GetEffectiveCss(OtherMonthDayStyle, OtherMonthDayStyleCss);
				if (!string.IsNullOrEmpty(css)) return css;
			}
			if (isWeekend)
			{
				var css = GetEffectiveCss(WeekendDayStyle, WeekendDayStyleCss);
				if (!string.IsNullOrEmpty(css)) return css;
			}

			return GetEffectiveCss(DayStyle, DayStyleCss);
		}
		#pragma warning restore CS0612, CS0618

		private string GetDayCellStyle(DateTime date)
		{
			var isToday = date.Date == DateTime.Today;
			var isSelected = _selectedDays.Contains(date.Date);
			var isOtherMonth = date.Month != _visibleMonth.Month;
			var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

			if (isSelected)
			{
				var style = SelectedDayStyle?.ToString();
				if (!string.IsNullOrEmpty(style)) return style;
			}
			if (isToday)
			{
				var style = TodayDayStyle?.ToString();
				if (!string.IsNullOrEmpty(style)) return style;
			}
			if (isOtherMonth)
			{
				var style = OtherMonthDayStyle?.ToString();
				if (!string.IsNullOrEmpty(style)) return style;
			}
			if (isWeekend)
			{
				var style = WeekendDayStyle?.ToString();
				if (!string.IsNullOrEmpty(style)) return style;
			}

			return DayStyle?.ToString();
		}

		private static string GetEffectiveCss(TableItemStyle tableItemStyle, string legacyCss)
		{
			if (!string.IsNullOrEmpty(tableItemStyle?.CssClass))
				return tableItemStyle.CssClass;
			return legacyCss;
		}

		private static string GetEffectiveStyle(TableItemStyle tableItemStyle)
		{
			return tableItemStyle?.ToString();
		}

		/// <summary>
		/// Creates day render arguments and invokes the OnDayRender event.
		/// Note: OnDayRender is invoked synchronously during rendering. Handlers should not perform async operations or modify component state directly.
		/// </summary>
		private CalendarDayRenderArgs CreateDayRenderArgs(DateTime date)
		{
			var args = new CalendarDayRenderArgs
			{
				Date = date,
				IsSelectable = SelectionMode != CalendarSelectionMode.None,
				IsWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday,
				IsToday = date.Date == DateTime.Today,
				IsOtherMonth = date.Month != _visibleMonth.Month,
				IsSelected = _selectedDays.Contains(date.Date)
			};

			// Fire-and-forget: the handler can modify args properties synchronously
			// before InvokeAsync yields. Avoid blocking with GetAwaiter().GetResult().
			if (OnDayRender.HasDelegate)
			{
				_ = OnDayRender.InvokeAsync(args);
			}

			return args;
		}
	}

	/// <summary>
	/// Event arguments for day rendering events.
	/// </summary>
	public class CalendarDayRenderArgs : EventArgs
	{
		public DateTime Date { get; set; }
		public bool IsSelectable { get; set; }
		public bool IsWeekend { get; set; }
		public bool IsToday { get; set; }
		public bool IsOtherMonth { get; set; }
		public bool IsSelected { get; set; }
	}

	/// <summary>
	/// Event arguments for month change events.
	/// </summary>
	public class CalendarMonthChangedArgs : EventArgs
	{
		public DateTime CurrentMonth { get; set; }
		public DateTime PreviousMonth { get; set; }
	}
}
