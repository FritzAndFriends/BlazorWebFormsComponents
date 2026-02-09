using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	public partial class Calendar : BaseStyledComponent
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
		/// Gets or sets the selection mode: None, Day, DayWeek, or DayWeekMonth.
		/// </summary>
		[Parameter]
		public string SelectionMode { get; set; } = "Day";

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
		public string DayNameFormat { get; set; } = "Short";

		/// <summary>
		/// Format for the title.
		/// </summary>
		[Parameter]
		public string TitleFormat { get; set; } = "MonthYear";

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

		/// <summary>
		/// Tooltip text.
		/// </summary>
		[Parameter]
		public string ToolTip { get; set; }

		// Style properties for different day types
		[Parameter]
		public string TitleStyleCss { get; set; }

		[Parameter]
		public string DayHeaderStyleCss { get; set; }

		[Parameter]
		public string DayStyleCss { get; set; }

		[Parameter]
		public string TodayDayStyleCss { get; set; }

		[Parameter]
		public string SelectedDayStyleCss { get; set; }

		[Parameter]
		public string OtherMonthDayStyleCss { get; set; }

		[Parameter]
		public string WeekendDayStyleCss { get; set; }

		[Parameter]
		public string NextPrevStyleCss { get; set; }

		[Parameter]
		public string SelectorStyleCss { get; set; }

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
			var format = TitleFormat == "Month" ? "MMMM" : "MMMM yyyy";
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
				"Full" => CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(day),
				"FirstLetter" => SafeSubstring(CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(day), 0, 1),
				"FirstTwoLetters" => SafeSubstring(CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(day), 0, 2),
				"Shortest" => CultureInfo.CurrentCulture.DateTimeFormat.GetShortestDayName(day),
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
			if (SelectionMode == "None") return;

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
			if (SelectionMode != "DayWeek" && SelectionMode != "DayWeekMonth") return;

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
			if (SelectionMode != "DayWeekMonth") return;

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

		private string GetDayCellCss(DateTime date)
		{
			var isToday = date.Date == DateTime.Today;
			var isSelected = _selectedDays.Contains(date.Date);
			var isOtherMonth = date.Month != _visibleMonth.Month;
			var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

			// Priority order for styling
			if (isSelected && !string.IsNullOrEmpty(SelectedDayStyleCss))
				return SelectedDayStyleCss;
			if (isToday && !string.IsNullOrEmpty(TodayDayStyleCss))
				return TodayDayStyleCss;
			if (isOtherMonth && !string.IsNullOrEmpty(OtherMonthDayStyleCss))
				return OtherMonthDayStyleCss;
			if (isWeekend && !string.IsNullOrEmpty(WeekendDayStyleCss))
				return WeekendDayStyleCss;
			
			return DayStyleCss;
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
				IsSelectable = SelectionMode != "None",
				IsWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday,
				IsToday = date.Date == DateTime.Today,
				IsOtherMonth = date.Month != _visibleMonth.Month,
				IsSelected = _selectedDays.Contains(date.Date)
			};

			// Invoke synchronously to allow handler to modify day properties before rendering
			if (OnDayRender.HasDelegate)
			{
				OnDayRender.InvokeAsync(args).GetAwaiter().GetResult();
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
