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
		public string NavigateUrlField { get; set; }

		[Parameter]
		public string TextField { get; set; }

		protected override Task OnParametersSetAsync()
		{

			ParentTreeView.AddTreeNodeBinding(this);

			return base.OnParametersSetAsync();
		}

		public override string ToString() {

			return $"<TreeNodeBinding DataMember=\"{DataMember}\" TextField=\"{TextField}\" NavigateUrlField=\"{NavigateUrlField}\"/>";

		}

	}


}



