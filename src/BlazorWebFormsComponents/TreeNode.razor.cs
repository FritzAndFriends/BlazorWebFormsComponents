using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	public partial class TreeNode : ComponentBase
	{

		public const string ImageLocation = "_content/Fritz.BlazorWebFormsComponents/TreeView/";

		[Parameter]
		public bool Checked { get; set; } = false;

		[Parameter]
		public byte Depth { get; set; } = 0;

		private bool _Expanded = true;
		private bool? _UserExpanded;
		[Parameter]
		public bool Expanded
		{
			get { return _UserExpanded.HasValue ? _UserExpanded.Value : _Expanded; }
			set { _Expanded = value; }
		}

		// TODO: Implement
		[Parameter]
		public string FormatString { get; set; }

		[Parameter]
		public string ImageToolTip { get; set; }

		private string _ImageUrl;
		[Parameter]
		public string ImageUrl
		{
			get
			{
				return !String.IsNullOrEmpty(_ImageUrl) ? _ImageUrl :
						string.IsNullOrEmpty(ParentTreeView.ImageSet.RootNode) ? "" :
							(IsRoot ? $"{ImageLocation}{ParentTreeView.ImageSet.RootNode}" :
								IsParent ? $"{ImageLocation}{ParentTreeView.ImageSet.ParentNode}" :
								$"{ImageLocation}{ParentTreeView.ImageSet.LeafNode}");
			}
			set { _ImageUrl = value; }
		}

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
		[CascadingParameter(Name = "ParentTreeNode")]
		public TreeNode Parent
		{
			get { return _Parent; }
			set
			{
				_Parent = value;
				Depth = (byte)((_Parent?.Depth ?? 0) + 1);
			}
		}

		[CascadingParameter(Name = "ParentTreeView")]
		public TreeView ParentTreeView { get; set; }

		private HashSet<TreeNode> _ChildNodes = new HashSet<TreeNode>();

		protected void AddChildNode(TreeNode node)
		{

			_ChildNodes.Add(node);
		}

		protected bool IsLast()
		{
			return (Parent?._ChildNodes?.Last() == this);
		}


		protected bool IsRoot
		{
			get { return Parent is null; }
		}

		protected bool IsParent
		{
			get { return !IsRoot && (ChildContent != null); }
		}

		protected string NodeImage
		{
			get
			{

				if (IsRoot)
				{

					if (ParentTreeView.ShowLines && ParentTreeView.ShowExpandCollapse)
					{
						return (Expanded ? $"{ImageLocation}Default_DashCollapse.gif" : $"{ImageLocation}Default_DashExpand.gif");
					}

					if (ParentTreeView.ShowLines)
					{
						return $"{ImageLocation}Default_Dash.gif";
					}

					return string.IsNullOrEmpty(ParentTreeView.ImageSet.Collapse) ? $"{ImageLocation}Default_NoExpand.gif" : (Expanded ? $"{ImageLocation}{ParentTreeView.ImageSet.Collapse}" : $"{ImageLocation}{ParentTreeView.ImageSet.Expand}");
					//return $"{ImageLocation}Default_NoExpand.gif";

				}

				else if (IsParent)
				{

					if (ParentTreeView.ShowLines && ParentTreeView.ShowExpandCollapse && IsLast())
					{
						return (Expanded ? $"{ImageLocation}Default_LCollapse.gif" : $"{ImageLocation}Default_LExpand.gif");
					}

					if (ParentTreeView.ShowLines && ParentTreeView.ShowExpandCollapse)
					{
						return (Expanded ? $"{ImageLocation}Default_TCollapse.gif" : $"{ImageLocation}Default_TExpand.gif");
					}

					return string.IsNullOrEmpty(ParentTreeView.ImageSet.Collapse) ? $"{ImageLocation}Default_NoExpand.gif" : (Expanded ? $"{ImageLocation}{ParentTreeView.ImageSet.Collapse}" : $"{ImageLocation}{ParentTreeView.ImageSet.Expand}");

				}

				return string.IsNullOrEmpty(ParentTreeView.ImageSet.Collapse) ? $"{ImageLocation}Default_NoExpand.gif" : (Expanded ? $"{ImageLocation}{ParentTreeView.ImageSet.Collapse}" : $"{ImageLocation}{ParentTreeView.ImageSet.Expand}");
			}
		}

		protected string NoExpandImage
		{
			get
			{

				if (ParentTreeView.ShowLines)
				{

					return IsLast() ? $"{ImageLocation}Default_L.gif" : $"{ImageLocation}Default_T.gif";

				}

				return string.IsNullOrEmpty(ParentTreeView.ImageSet.Collapse) ? "" : $"{ImageLocation}{ParentTreeView.ImageSet.NoExpand}";
			}
		}

		#region Event Handlers

		protected override Task OnInitializedAsync()
		{

			Parent?.AddChildNode(this);


			return base.OnInitializedAsync();
		}


		protected override void OnParametersSet()
		{

			if (!ParentTreeView.Nodes.Contains(this))
			{
				ParentTreeView.Nodes.Add(this);
			}

		}

		public void HandleNodeExpand()
		{

			_UserExpanded = !Expanded;

			if (Expanded) ParentTreeView.OnTreeNodeExpanded.InvokeAsync(new TreeNodeEventArgs(this));
			else ParentTreeView.OnTreeNodeCollapsed.InvokeAsync(new TreeNodeEventArgs(this));

		}

		public void HandleCheckbox(object sender, ChangeEventArgs args)
		{

			this.Checked = (bool)args.Value;

			ParentTreeView.OnTreeNodeCheckChanged.InvokeAsync(new TreeNodeEventArgs(this));

		}

		#endregion

	}
}
