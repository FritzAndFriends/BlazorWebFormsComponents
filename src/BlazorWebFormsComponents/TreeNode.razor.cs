using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents
{
	public partial class TreeNode : BaseWebFormsComponent
	{

		public const string ImageLocation = "_content/Fritz.BlazorWebFormsComponents/TreeView/";

		[Parameter]
		public bool Checked { get; set; } = false;

		// TODO: Implement
		[Parameter]
		public byte Depth { get; set; } = 0;

		[Parameter]
		public bool Expanded { get; set; } = true;

		// TODO: Implement
		[Parameter]
		public string FormatString { get; set; }

		[Parameter]
		public string ImageToolTip { get; set; }

		[Parameter]
		public string ImageUrl { get; set; }

		[Parameter]
		public string NavigateUrl { get; set; }

		// TODO: Implement
		[Parameter]
		public bool PopulateOnDemand { get; set; }

		// TODO: Implement
		[Parameter]
		public TreeNodeSelectAction SelectAction { get; set; }

		// TODO: Implement
		[Parameter]
		public bool Selected { get; set; }

		[Parameter]
		public bool ShowCheckBox { get; set; } = true;

		[Parameter]
		public string Target { get; set; }

		[Parameter]
		public string Text { get; set; }

		[Parameter]
		public string ToolTip { get; set; } = null;

		[Parameter]
		public string Value { get; set; }

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

		#region Event Handlers

		protected override void OnParametersSet() {

			if (!ParentTreeView.Nodes.Contains(this))
			{
				ParentTreeView.Nodes.Add(this);
			}
		
		}

		public void HandleNodeExpand() {

			Expanded = !Expanded;

			if (Expanded) ParentTreeView.OnTreeNodeExpanded.InvokeAsync(new TreeNodeEventArgs(this));
			else ParentTreeView.OnTreeNodeCollapsed.InvokeAsync(new TreeNodeEventArgs(this));

		}

		public void HandleCheckbox(object sender, ChangeEventArgs args) {

			this.Checked = (bool)args.Value;

			ParentTreeView.OnTreeNodeCheckChanged.InvokeAsync(new TreeNodeEventArgs(this));

		}

		#endregion

	}
}
