namespace BlazorWebFormsComponents.Interfaces
{
	/// <summary>
	/// Interface for components that contain Calendar-specific TableItemStyle properties.
	/// </summary>
	public interface ICalendarStyleContainer
	{
		TableItemStyle DayStyle { get; }
		TableItemStyle TitleStyle { get; }
		TableItemStyle DayHeaderStyle { get; }
		TableItemStyle TodayDayStyle { get; }
		TableItemStyle SelectedDayStyle { get; }
		TableItemStyle OtherMonthDayStyle { get; }
		TableItemStyle WeekendDayStyle { get; }
		TableItemStyle NextPrevStyle { get; }
		TableItemStyle SelectorStyle { get; }
	}
}
