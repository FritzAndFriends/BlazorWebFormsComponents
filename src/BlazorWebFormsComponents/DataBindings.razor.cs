using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class DataBindings
	{

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		[CascadingParameter(Name = "ParentTreeView")]
		public TreeView ParentTreeView { get; set; }

	}


}



