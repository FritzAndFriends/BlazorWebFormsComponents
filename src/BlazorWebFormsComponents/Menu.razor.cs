using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{

  /// <summary>
  /// A menubar component capable of making hierarchical menus for navigating your application
  /// </summary>
  public partial class Menu : BaseWebFormsComponent {

		[Parameter]
		public MenuItemsCollection Items { get; set; } = new MenuItemsCollection();

		[Parameter]
		public RenderFragment ChildContent { get; set; }

	}

}
