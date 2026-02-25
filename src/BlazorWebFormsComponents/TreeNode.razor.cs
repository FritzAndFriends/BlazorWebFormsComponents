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
				// Check style-level ImageUrl first
				var styleImageUrl = ParentTreeView?.GetStyleImageUrl(this);
				if (!string.IsNullOrEmpty(_ImageUrl)) return _ImageUrl;
				if (!string.IsNullOrEmpty(styleImageUrl)) return styleImageUrl;
				return string.IsNullOrEmpty(ParentTreeView.ImageSet.RootNode) ? "" :
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

		/// <summary>
		/// Gets the ValuePath for this node using the TreeView's PathSeparator.
		/// </summary>
		public string ValuePath => ParentTreeView?.GetValuePath(this);

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

		/// <summary>
		/// Gets the child nodes of this node (used by ExpandAll/CollapseAll/FindNode).
		/// </summary>
		public IEnumerable<TreeNode> ChildNodes => _ChildNodes;

		protected void AddChildNode(TreeNode node)
		{

			_ChildNodes.Add(node);
		}

		protected bool IsLast()
		{
			return (Parent?._ChildNodes?.Last() == this);
		}

		// Public node-type properties for style resolution
		public bool IsRootNode => Parent is null;
		public bool IsParentNode => !IsRootNode && (ChildContent != null);
		public bool IsLeafNode => ChildContent is null && Parent != null;

		protected bool IsRoot
		{
			get { return Parent is null; }
		}

		protected bool IsParent
		{
			get { return !IsRoot && (ChildContent != null); }
		}

		/// <summary>
		/// Sets the expanded state programmatically (used by ExpandAll/CollapseAll).
		/// </summary>
		internal void SetExpanded(bool expanded)
		{
			_UserExpanded = expanded;
		}

		/// <summary>
		/// Gets the computed indent width in pixels using NodeIndent from TreeView.
		/// </summary>
		protected int IndentWidth => ParentTreeView?.NodeIndent ?? 20;

		/// <summary>
		/// Gets the computed node style from the parent TreeView.
		/// </summary>
		protected string ComputedNodeStyle => ParentTreeView?.GetNodeStyle(this);

		/// <summary>
		/// Gets the computed CSS class from the parent TreeView.
		/// </summary>
		protected string ComputedNodeCssClass => ParentTreeView?.GetNodeCssClass(this);

		/// <summary>
		/// Determines if this node should be initially expanded based on ExpandDepth.
		/// </summary>
		private bool ShouldExpandByDepth
		{
			get
			{
				if (ParentTreeView == null) return true;
				if (ParentTreeView.ExpandDepth < 0) return true; // -1 means fully expanded
				return Depth < ParentTreeView.ExpandDepth;
			}
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

			// Apply ExpandDepth on initialization
			if (!_UserExpanded.HasValue && !ShouldExpandByDepth)
			{
				_Expanded = false;
			}

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

		/// <summary>
		/// Handles click on the node text to select it.
		/// </summary>
		public async Task HandleNodeSelect()
		{
			await ParentTreeView.SelectNodeAsync(this);
		}

		public void HandleCheckbox(object sender, ChangeEventArgs args)
		{

			this.Checked = (bool)args.Value;

			ParentTreeView.OnTreeNodeCheckChanged.InvokeAsync(new TreeNodeEventArgs(this));

		}

		public async Task HandleKeyDown(Microsoft.AspNetCore.Components.Web.KeyboardEventArgs e)
		{
			if (!ParentTreeView.UseAccessibilityFeatures) return;

			switch (e.Key)
			{
				case "ArrowRight":
					// Expand node if it has children and is collapsed
					if (ChildContent != null && !Expanded)
					{
						HandleNodeExpand();
					}
					break;

				case "ArrowLeft":
					// Collapse node if it has children and is expanded
					if (ChildContent != null && Expanded)
					{
						HandleNodeExpand();
					}
					break;

				case "Enter":
				case " ": // Space key
					// Select the node, or expand/collapse if no navigate URL
					if (string.IsNullOrEmpty(NavigateUrl))
					{
						await HandleNodeSelect();
						if (ChildContent != null)
						{
							HandleNodeExpand();
						}
					}
					break;
			}

			await Task.CompletedTask;
		}

		#endregion

	}
}
