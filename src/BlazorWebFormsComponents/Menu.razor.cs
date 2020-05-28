using System.Threading.Tasks;
using BlazorComponentUtilities;
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

		private DynamicHoverStyle _DynamicHoverStyle = new DynamicHoverStyle();
		public DynamicHoverStyle DynamicHoverStyle {
			get { return _DynamicHoverStyle;}
			set { _DynamicHoverStyle = value;
				StateHasChanged();
			}
		}

		private DynamicMenuItemStyle _DynamicMenuItemStyle = new DynamicMenuItemStyle();
		public DynamicMenuItemStyle DynamicMenuItemStyle {
			get { return _DynamicMenuItemStyle;}
			set { _DynamicMenuItemStyle = value;
				this.StateHasChanged();
			}
		}

		private StaticHoverStyle _StaticHoverStyle = new StaticHoverStyle();
		public StaticHoverStyle StaticHoverStyle {
			get { return _StaticHoverStyle;}
			set {
				_StaticHoverStyle = value;
				StateHasChanged();
			}
		}

		private StaticMenuItemStyle _StaticMenuItemStyle = new StaticMenuItemStyle();
		public StaticMenuItemStyle StaticMenuItemStyle {
			get { return _StaticMenuItemStyle;}
			set {
				_StaticMenuItemStyle = value;
				StateHasChanged();
			}
		}


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
