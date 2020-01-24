using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	public partial class DataBindings 
	{

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		[CascadingParameter(Name ="ParentTreeView")]
		public TreeView ParentTreeView { get; set; }

	}


}



