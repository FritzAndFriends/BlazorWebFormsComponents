using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	public class TreeNodeBinding : ComponentBase {

		[CascadingParameter(Name ="ParentTreeView")]
		public TreeView ParentTreeView { get; set; }

		[Parameter]
		public string DataMember { get; set; }

		[Parameter]
		public string TextField { get; set; }

		protected override Task OnParametersSetAsync()
		{

			ParentTreeView.AddTreeNodeBinding(this);

			return base.OnParametersSetAsync();
		}

	}


}



