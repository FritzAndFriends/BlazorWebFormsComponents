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
    public IJSRuntime JS { get; set; }

		[Parameter]
		public MenuItemsCollection Items { get; set; } = new MenuItemsCollection();

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		protected override async Task OnAfterRenderAsync(bool firstRender) {

			if (firstRender)
			{
				await JS.InvokeVoidAsync("bwfc.Page.AddScriptElement", $"{StaticFilesLocation}Menu/Menu.js", $"new Sys.WebForms.Menu({{ element: '{ID}', disappearAfter: 2000, orientation: 'vertical', tabIndex: 0, disabled: false }});");
				// await JS.InvokeVoidAsync("eval", $"new Sys.WebForms.Menu({{ element: '{ID}', disappearAfter: 2000, orientation: 'vertical', tabIndex: 0, disabled: false }});");
				// await JsRuntime.InvokeVoidAsync("eval", "\" console.log('hello');\"");
			}

			await base.OnAfterRenderAsync(firstRender);

		}

	}

}
