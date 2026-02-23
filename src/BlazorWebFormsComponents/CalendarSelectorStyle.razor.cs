using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class CalendarSelectorStyle : UiTableItemStyle
	{
		[CascadingParameter(Name = "ParentCalendar")]
		protected ICalendarStyleContainer ParentCalendar { get; set; }

		protected override void OnInitialized()
		{
			if (ParentCalendar != null)
			{
				theStyle = ParentCalendar.SelectorStyle;
			}
			base.OnInitialized();
		}
	}
}
