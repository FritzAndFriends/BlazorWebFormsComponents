using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Abstract base for TreeNodeStyle sub-components.
	/// Follows the same pattern as UiTableItemStyle for GridView.
	/// </summary>
	public abstract class UiTreeNodeStyle : UiStyle<TreeNodeStyle>
	{
		[Parameter]
		public Unit ChildNodesPadding { get; set; }

		[Parameter]
		public Unit HorizontalPadding { get; set; }

		[Parameter]
		public string ImageUrl { get; set; }

		[Parameter]
		public Unit NodeSpacing { get; set; }

		[Parameter]
		public Unit VerticalPadding { get; set; }

		protected override void OnInitialized()
		{
			if (theStyle != null)
			{
				base.OnInitialized();

				theStyle.ChildNodesPadding = ChildNodesPadding;
				theStyle.HorizontalPadding = HorizontalPadding;
				theStyle.ImageUrl = ImageUrl;
				theStyle.NodeSpacing = NodeSpacing;
				theStyle.VerticalPadding = VerticalPadding;

				theStyle.SetFontsFromAttributes(AdditionalAttributes);
			}
		}
	}
}
