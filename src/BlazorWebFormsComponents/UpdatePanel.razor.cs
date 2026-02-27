using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class UpdatePanel : BaseWebFormsComponent
	{
		/// <summary>
		/// Gets or sets a value that indicates when an UpdatePanel control's content is updated.
		/// </summary>
		[Parameter]
		public UpdatePanelUpdateMode UpdateMode { get; set; } = UpdatePanelUpdateMode.Always;

		/// <summary>
		/// Gets or sets a value that indicates whether child controls of the UpdatePanel cause an asynchronous postback.
		/// </summary>
		[Parameter]
		public bool ChildrenAsTriggers { get; set; } = true;

		/// <summary>
		/// Gets or sets a value that indicates whether the UpdatePanel renders as a div or span element.
		/// </summary>
		[Parameter]
		public UpdatePanelRenderMode RenderMode { get; set; } = UpdatePanelRenderMode.Block;

		/// <summary>
		/// Equivalent to ContentTemplate in Web Forms. Contains the child content of the UpdatePanel.
		/// </summary>
		[Parameter]
		public RenderFragment ChildContent { get; set; }
	}
}
