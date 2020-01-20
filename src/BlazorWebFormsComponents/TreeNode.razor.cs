using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	public partial class TreeNode : BaseWebFormsComponent
	{

		[Parameter]
		public string NavigateUrl { get; set; }

		[Parameter]
		public string Target { get; set; }

		[Parameter]
		public string Text { get; set; }

		[Parameter]
		public string ToolTip { get; set; } = null;

		[Parameter]
		public string Value { get; set; }

		[Parameter]
		public bool Expanded { get; set; } = true;

		protected byte Depth { get; set; } = 0;

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		private TreeNode _Parent;
		[CascadingParameter(Name ="ParentTreeNode")]
		public TreeNode Parent {
			get { return _Parent; }
			set {
				_Parent = value;
				Depth = (byte)((_Parent?.Depth ?? 0) + 1);
			}
		}

		[CascadingParameter(Name ="ParentTreeView")]
		public TreeView ParentTreeView { get; set; }

	}
}
