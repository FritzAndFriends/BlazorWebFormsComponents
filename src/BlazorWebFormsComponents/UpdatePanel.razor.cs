using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	// NOTE: This component does NOT apply @rendermode InteractiveServer by default.
	// While UpdatePanel in Web Forms enabled interactivity via AJAX postbacks, in Blazor
	// the render mode should be controlled by the consuming application at the page or
	// app level, not forced by library components. Pages using UpdatePanel should ensure
	// they are running under InteractiveServer mode in their App.razor or via @rendermode
	// on the page itself.
	public partial class UpdatePanel : BaseStyledComponent
	{
		/// <summary>
		/// Gets or sets a value that indicates when an UpdatePanel control's content is updated.
		/// </summary>
		[Parameter]
		public EnumParameter<UpdatePanelUpdateMode> UpdateMode { get; set; } = UpdatePanelUpdateMode.Always;

		/// <summary>
		/// Gets or sets a value that indicates whether child controls of the UpdatePanel cause an asynchronous postback.
		/// </summary>
		[Parameter]
		public bool ChildrenAsTriggers { get; set; } = true;

		/// <summary>
		/// Gets or sets a value that indicates whether the UpdatePanel renders as a div or span element.
		/// </summary>
		[Parameter]
		public EnumParameter<UpdatePanelRenderMode> RenderMode { get; set; } = UpdatePanelRenderMode.Block;

		/// <summary>
		/// The Web Forms equivalent of the ContentTemplate. Supports both Blazor's ChildContent syntax
		/// and Web Forms' ContentTemplate syntax for migration compatibility.
		/// </summary>
		[Parameter]
		public RenderFragment ContentTemplate { get; set; }

		/// <summary>
		/// Blazor's standard ChildContent parameter. Falls back to ContentTemplate if not specified.
		/// </summary>
		[Parameter]
		public RenderFragment ChildContent { get; set; }
	}
}
