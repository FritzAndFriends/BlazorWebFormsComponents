using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorWebFormsComponents
{

  /// <summary>
  /// A menubar component capable of making hierarchical menus for navigating your application
  /// </summary>
  public partial class Menu : BaseWebFormsComponent {

		[Inject]
    public IJSRuntime jSRuntime { get; set; }

		[Parameter]
		public MenuItemsCollection Items { get; set; } = new MenuItemsCollection();

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		protected override async Task OnInitializedAsync() {

			await JsRuntime.InvokeVoidAsync("bwfc.Page.AddScriptElement", $"{StaticFilesLocation}Menu/Menu.js");

			await base.OnInitializedAsync();

		}

	}

}
