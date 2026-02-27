using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class UpdateProgress : BaseStyledComponent
	{
		/// <summary>
		/// Gets or sets the ID of the UpdatePanel control that this UpdateProgress displays status for.
		/// </summary>
		[Parameter]
		public string AssociatedUpdatePanelID { get; set; }

		/// <summary>
		/// Gets or sets the time in milliseconds before the progress template is displayed. Default is 500 ms.
		/// </summary>
		[Parameter]
		public int DisplayAfter { get; set; } = 500;

		/// <summary>
		/// When true, the progress content is rendered with display:none (removes from layout).
		/// When false, rendered with visibility:hidden (reserves layout space).
		/// </summary>
		[Parameter]
		public bool DynamicLayout { get; set; } = true;

		/// <summary>
		/// The template that is displayed during an asynchronous postback.
		/// </summary>
		[Parameter]
		public RenderFragment ProgressTemplate { get; set; }
	}
}
