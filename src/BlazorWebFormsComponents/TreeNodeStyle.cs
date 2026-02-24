using BlazorComponentUtilities;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Represents the style of a node in the TreeView control.
	/// Extends Style with TreeView-specific properties.
	/// </summary>
	public class TreeNodeStyle : Style
	{
		public TreeNodeStyle() { }

		public Unit ChildNodesPadding { get; set; }

		public Unit HorizontalPadding { get; set; }

		public string ImageUrl { get; set; }

		public Unit NodeSpacing { get; set; }

		public Unit VerticalPadding { get; set; }

		public override string ToString()
		{
			var builder = ((IStyle)this).ToStyle();

			if (HorizontalPadding != Unit.Empty)
			{
				builder = builder
					.AddStyle("padding-left", HorizontalPadding.ToString())
					.AddStyle("padding-right", HorizontalPadding.ToString());
			}

			if (VerticalPadding != Unit.Empty)
			{
				builder = builder
					.AddStyle("padding-top", VerticalPadding.ToString())
					.AddStyle("padding-bottom", VerticalPadding.ToString());
			}

			if (NodeSpacing != Unit.Empty)
			{
				builder = builder
					.AddStyle("margin-bottom", NodeSpacing.ToString());
			}

			return builder.NullIfEmpty();
		}
	}
}
