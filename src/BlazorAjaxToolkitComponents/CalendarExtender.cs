using System;
using BlazorAjaxToolkitComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Attaches a popup calendar date picker to a target TextBox.
/// Supports configurable date formats, navigation views, and date range constraints.
/// Emulates the Ajax Control Toolkit CalendarExtender.
/// </summary>
public class CalendarExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/calendar-extender.js";

	/// <summary>
	/// The date format string used to display and parse dates (e.g., "MM/dd/yyyy").
	/// </summary>
	[Parameter]
	public string Format { get; set; } = "d";

	/// <summary>
	/// The position of the popup calendar relative to the target control.
	/// </summary>
	[Parameter]
	public CalendarPosition PopupPosition { get; set; } = CalendarPosition.BottomLeft;

	/// <summary>
	/// The default view when the calendar popup opens.
	/// </summary>
	[Parameter]
	public CalendarDefaultView DefaultView { get; set; } = CalendarDefaultView.Days;

	/// <summary>
	/// The earliest selectable date. Dates before this are disabled.
	/// </summary>
	[Parameter]
	public DateTime? StartDate { get; set; }

	/// <summary>
	/// The latest selectable date. Dates after this are disabled.
	/// </summary>
	[Parameter]
	public DateTime? EndDate { get; set; }

	/// <summary>
	/// The currently selected date.
	/// </summary>
	[Parameter]
	public DateTime? SelectedDate { get; set; }

	/// <summary>
	/// The date highlighted as today. Defaults to the current date if not set.
	/// </summary>
	[Parameter]
	public DateTime? TodaysDate { get; set; }

	/// <summary>
	/// CSS class applied to the calendar popup container.
	/// </summary>
	[Parameter]
	public string CssClass { get; set; } = string.Empty;

	/// <summary>
	/// Client-side script invoked when a date is selected.
	/// </summary>
	[Parameter]
	public string OnClientDateSelectionChanged { get; set; } = string.Empty;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		format = Format,
		popupPosition = (int)PopupPosition,
		defaultView = (int)DefaultView,
		startDate = StartDate?.ToString("o"),
		endDate = EndDate?.ToString("o"),
		selectedDate = SelectedDate?.ToString("o"),
		todaysDate = TodaysDate?.ToString("o"),
		cssClass = CssClass,
		onClientDateSelectionChanged = OnClientDateSelectionChanged
	};
}
