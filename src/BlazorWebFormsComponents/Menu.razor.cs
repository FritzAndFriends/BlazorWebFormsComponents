using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorWebFormsComponents
{

  /// <summary>
  /// A menubar component capable of making hierarchical menus for navigating your application
  /// </summary>
  public partial class Menu : BaseWebFormsComponent {

		#region Injected Properties

		[Inject]
    public IJSRuntime JS { get; set; }

		#endregion

		[Parameter]
		public int DisappearAfter { get; set; }

		[Parameter]
		public int StaticDisplayLevels { get; set; }

		[Parameter]
		public int StaticSubmenuIndent { get; set; }

		[Parameter]
		public Items Items { get; set; }

		[Parameter]
		public Style DynamicHoverStyle { get; set; } = new Style();

		[Parameter]
		public Style DynamicMenuItemStyle { get; set; } = new Style();

		[Parameter]
		public Style StaticHoverStyle { get; set; } = new Style();

		[Parameter]
		public StaticMenuItemStyle StaticMenuItemStyle { get; set; } = new StaticMenuItemStyle();

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		protected override async Task OnAfterRenderAsync(bool firstRender) {

			if (firstRender)
			{
				await JS.InvokeVoidAsync("bwfc.Page.AddScriptElement", $"{StaticFilesLocation}Menu/Menu.js", $"new Sys.WebForms.Menu({{ element: '{ID}', disappearAfter: {DisappearAfter}, orientation: 'vertical', tabIndex: 0, disabled: false }});");
			}

			await base.OnAfterRenderAsync(firstRender);

		}

	}

}
