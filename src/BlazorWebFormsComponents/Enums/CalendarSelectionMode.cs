namespace BlazorWebFormsComponents.Enums
{
	/// <summary>
	/// Specifies the selection mode of the Calendar control.
	/// </summary>
	public enum CalendarSelectionMode
	{
		/// <summary>
		/// No dates can be selected.
		/// </summary>
		None = 0,

		/// <summary>
		/// A single date can be selected.
		/// </summary>
		Day = 1,

		/// <summary>
		/// A single date or an entire week can be selected.
		/// </summary>
		DayWeek = 2,

		/// <summary>
		/// A single date, an entire week, or an entire month can be selected.
		/// </summary>
		DayWeekMonth = 3
	}
}
