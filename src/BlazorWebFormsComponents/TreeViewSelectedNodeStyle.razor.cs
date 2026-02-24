using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class TreeViewSelectedNodeStyle : UiTreeNodeStyle
	{
		[CascadingParameter(Name = "ParentTreeView")]
		protected ITreeViewStyleContainer Parent { get; set; }

		protected override void OnInitialized()
		{
			if (Parent != null)
				theStyle = Parent.SelectedNodeStyle;
			base.OnInitialized();
		}
	}
}
