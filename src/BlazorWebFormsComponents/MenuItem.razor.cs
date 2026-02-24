using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Individual menu item to be displayed in a Menu component
	/// </summary>
	public partial class MenuItem : BaseWebFormsComponent
	{

		[CascadingParameter(Name = "ParentMenu")]
		public Menu ParentMenu { get; set; }

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		[Parameter]
		public string NavigateUrl { get; set; }

		[Parameter]
		public string Text { get; set; }

		[Parameter]
		public string Title { get; set; }

		[Parameter]
		public string ToolTip { get; set; }

		/// <summary>
		/// Gets or sets the value associated with this menu item.
		/// </summary>
		[Parameter]
		public string Value { get; set; }

		/// <summary>
		/// Gets or sets the target window or frame for this menu item's link.
		/// Falls back to the parent Menu's Target if not set.
		/// </summary>
		[Parameter]
		public string Target { get; set; }

		/// <summary>
		/// Gets the path from the root to this menu item, using the parent Menu's PathSeparator.
		/// </summary>
		public string ValuePath
		{
			get
			{
				var separator = ParentMenu?.PathSeparator ?? '/';
				return Value ?? Text ?? string.Empty;
			}
		}

		[CascadingParameter(Name = "Depth")]
		public int Depth { get; set; } = 1;

		/// <summary>
		/// Gets the effective target for this menu item's link.
		/// </summary>
		protected string EffectiveTarget => !string.IsNullOrEmpty(Target) ? Target : ParentMenu?.Target;

		/// <summary>
		/// Handles the click event on this menu item.
		/// </summary>
		protected async Task HandleClick()
		{
			if (ParentMenu != null)
			{
				await ParentMenu.NotifyItemClicked(this);
			}
		}
	}
}

