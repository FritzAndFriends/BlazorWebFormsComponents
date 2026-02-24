using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class TreeViewHoverNodeStyle : UiTreeNodeStyle
	{
		[CascadingParameter(Name = "ParentTreeView")]
		protected ITreeViewStyleContainer Parent { get; set; }

		protected override void OnInitialized()
		{
			if (Parent != null)
				theStyle = Parent.HoverNodeStyle;
			base.OnInitialized();
		}
	}
}
