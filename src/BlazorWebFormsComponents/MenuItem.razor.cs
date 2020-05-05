using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
  /// <summary>
  /// Individual menu item to be displayed in a Menu component
  /// </summary>
  public partial class MenuItem : BaseWebFormsComponent {

		[CascadingParameter(Name="ParentMenu")]
		public Menu ParentMenu { get; set; }

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		[Parameter]
		public string NavigateUrl { get; set; }

		[Parameter]
		public string Text { get; set; }

		[Parameter]
		public string Title { get; set; }

		[CascadingParameter(Name="Depth")]
		public int Depth { get; set; } = 1;

	}


}
