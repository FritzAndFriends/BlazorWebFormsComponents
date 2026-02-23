using BlazorWebFormsComponents.DataBinding;
using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace BlazorWebFormsComponents
{
	public partial class TreeView : BaseDataBoundComponent, ITreeViewStyleContainer
	{

		[Parameter]
		public TreeNodeCollection Nodes { get; set; } = new TreeNodeCollection();

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		[Parameter]
		public DataBindings DataBindings { get; set; }

		[Parameter]
		public TreeViewImageSet ImageSet { get; set; } = TreeViewImageSet._Default;

		[Parameter]
		public TreeNodeTypes ShowCheckBoxes { get; set; }

		[Parameter]
		public bool ShowExpandCollapse { get; set; } = true;

		[Parameter]
		public bool ShowLines { get; set; } = false;

		[Parameter]
		public bool UseAccessibilityFeatures { get; set; } = false;

		#region WI-11: Node-level style sub-components

		public TreeNodeStyle NodeStyle { get; internal set; } = new TreeNodeStyle();
		public TreeNodeStyle HoverNodeStyle { get; internal set; } = new TreeNodeStyle();
		public TreeNodeStyle LeafNodeStyle { get; internal set; } = new TreeNodeStyle();
		public TreeNodeStyle ParentNodeStyle { get; internal set; } = new TreeNodeStyle();
		public TreeNodeStyle RootNodeStyle { get; internal set; } = new TreeNodeStyle();
		public TreeNodeStyle SelectedNodeStyle { get; internal set; } = new TreeNodeStyle();

		[Parameter]
		public RenderFragment NodeStyleContent { get; set; }

		[Parameter]
		public RenderFragment HoverNodeStyleContent { get; set; }

		[Parameter]
		public RenderFragment LeafNodeStyleContent { get; set; }

		[Parameter]
		public RenderFragment ParentNodeStyleContent { get; set; }

		[Parameter]
		public RenderFragment RootNodeStyleContent { get; set; }

		[Parameter]
		public RenderFragment SelectedNodeStyleContent { get; set; }

		/// <summary>
		/// Resolves the effective style for a node based on its type and selection state.
		/// Priority: SelectedNodeStyle (if selected) > type-specific style > NodeStyle (fallback).
		/// </summary>
		internal string GetNodeStyle(TreeNode node)
		{
			TreeNodeStyle typeStyle = null;

			if (node.IsRootNode)
				typeStyle = RootNodeStyle;
			else if (node.IsParentNode)
				typeStyle = ParentNodeStyle;
			else if (node.IsLeafNode)
				typeStyle = LeafNodeStyle;

			// Selected style takes highest priority
			if (node.Selected)
			{
				var selectedStr = SelectedNodeStyle?.ToString();
				if (!string.IsNullOrEmpty(selectedStr))
					return selectedStr;
			}

			// Type-specific style
			var typeStr = typeStyle?.ToString();
			if (!string.IsNullOrEmpty(typeStr))
				return typeStr;

			// Default NodeStyle fallback
			return NodeStyle?.ToString();
		}

		/// <summary>
		/// Gets the CSS class for a node based on its type and selection state.
		/// </summary>
		internal string GetNodeCssClass(TreeNode node)
		{
			TreeNodeStyle typeStyle = null;

			if (node.IsRootNode)
				typeStyle = RootNodeStyle;
			else if (node.IsParentNode)
				typeStyle = ParentNodeStyle;
			else if (node.IsLeafNode)
				typeStyle = LeafNodeStyle;

			if (node.Selected && !string.IsNullOrEmpty(SelectedNodeStyle?.CssClass))
				return SelectedNodeStyle.CssClass;

			if (!string.IsNullOrEmpty(typeStyle?.CssClass))
				return typeStyle.CssClass;

			return NodeStyle?.CssClass;
		}

		/// <summary>
		/// Gets the style image URL for a node based on its type.
		/// </summary>
		internal string GetStyleImageUrl(TreeNode node)
		{
			if (node.IsRootNode && !string.IsNullOrEmpty(RootNodeStyle?.ImageUrl))
				return RootNodeStyle.ImageUrl;
			if (node.IsParentNode && !string.IsNullOrEmpty(ParentNodeStyle?.ImageUrl))
				return ParentNodeStyle.ImageUrl;
			if (node.IsLeafNode && !string.IsNullOrEmpty(LeafNodeStyle?.ImageUrl))
				return LeafNodeStyle.ImageUrl;
			if (!string.IsNullOrEmpty(NodeStyle?.ImageUrl))
				return NodeStyle.ImageUrl;
			return null;
		}

		#endregion

		#region WI-13: Selection support

		/// <summary>
		/// Gets the currently selected TreeNode, or null if no node is selected.
		/// </summary>
		public TreeNode SelectedNode { get; private set; }

		/// <summary>
		/// Gets the Value property of the currently selected node, or null.
		/// </summary>
		public string SelectedValue => SelectedNode?.Value;

		[Parameter]
		public EventCallback<TreeNodeEventArgs> SelectedNodeChanged { get; set; }

		/// <summary>
		/// Called by TreeNode when it is clicked to become selected.
		/// </summary>
		internal async Task SelectNodeAsync(TreeNode node)
		{
			if (SelectedNode == node) return;

			// Deselect previous
			if (SelectedNode != null)
				SelectedNode.Selected = false;

			// Select new
			node.Selected = true;
			SelectedNode = node;

			await SelectedNodeChanged.InvokeAsync(new TreeNodeEventArgs(node));
			StateHasChanged();
		}

		#endregion

		#region WI-15: Expand/collapse methods + properties

		/// <summary>
		/// Pixel indent per depth level. Default 20.
		/// </summary>
		[Parameter]
		public int NodeIndent { get; set; } = 20;

		/// <summary>
		/// Limits initial expansion depth. -1 means fully expanded (default).
		/// </summary>
		[Parameter]
		public int ExpandDepth { get; set; } = -1;

		/// <summary>
		/// Separator character used in value paths. Default '/'.
		/// </summary>
		[Parameter]
		public char PathSeparator { get; set; } = '/';

		/// <summary>
		/// Expands all nodes in the tree.
		/// </summary>
		public void ExpandAll()
		{
			foreach (var node in Nodes)
			{
				ExpandNodeRecursive(node, true);
			}
			StateHasChanged();
		}

		/// <summary>
		/// Collapses all nodes in the tree.
		/// </summary>
		public void CollapseAll()
		{
			foreach (var node in Nodes)
			{
				ExpandNodeRecursive(node, false);
			}
			StateHasChanged();
		}

		private void ExpandNodeRecursive(TreeNode node, bool expand)
		{
			node.SetExpanded(expand);
			foreach (var child in node.ChildNodes)
			{
				ExpandNodeRecursive(child, expand);
			}
		}

		/// <summary>
		/// Finds a node by its PathSeparator-delimited value path.
		/// </summary>
		public TreeNode FindNode(string valuePath)
		{
			if (string.IsNullOrEmpty(valuePath)) return null;

			var segments = valuePath.Split(PathSeparator);
			var currentNodes = Nodes.AsEnumerable();

			TreeNode found = null;
			foreach (var segment in segments)
			{
				found = currentNodes.FirstOrDefault(n =>
					string.Equals(n.Value ?? n.Text, segment, StringComparison.Ordinal));
				if (found == null) return null;
				currentNodes = found.ChildNodes;
			}
			return found;
		}

		/// <summary>
		/// Gets the ValuePath for a node, using PathSeparator.
		/// </summary>
		internal string GetValuePath(TreeNode node)
		{
			var parts = new List<string>();
			var current = node;
			while (current != null)
			{
				parts.Insert(0, current.Value ?? current.Text);
				current = current.Parent;
			}
			return string.Join(PathSeparator.ToString(), parts);
		}

		#endregion

		#region Events

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{

			await base.OnAfterRenderAsync(firstRender);

			if (firstRender && DataSource != null) await DataBind();

		}

		[Parameter]
		public EventCallback<TreeNodeEventArgs> OnTreeNodeDataBound { get; set; }

		[Parameter]
		public EventCallback<TreeNodeEventArgs> OnTreeNodeCheckChanged { get; set; }

		[Parameter]
		public EventCallback<TreeNodeEventArgs> OnTreeNodeCollapsed { get; set; }

		[Parameter]
		public EventCallback<TreeNodeEventArgs> OnTreeNodeExpanded { get; set; }

		#endregion

		#region DataBinding

		public RenderFragment ChildNodesRenderFragment { get; set; }

		private new Task DataBind()
		{

			OnDataBinding.InvokeAsync(EventArgs.Empty);

			if (DataSource is XmlDocument xmlDoc)
			{

				if (xmlDoc.SelectSingleNode("/*").LocalName == "siteMap")
					DataBindSiteMap(xmlDoc);
				else
					DataBindXml((DataSource as XmlDocument).SelectNodes("/*"));

			}

			OnDataBound.InvokeAsync(EventArgs.Empty);

			return Task.CompletedTask;

		}

		private Task DataBindXml(XmlNodeList elements)
		{

			var treeNodeCounter = 0;

			ChildNodesRenderFragment = b =>
			{
				b.OpenRegion(1);

				AddElements(b, elements);

				b.CloseRegion();
			};

			StateHasChanged();

			return Task.CompletedTask;

			void AddElements(RenderTreeBuilder builder, XmlNodeList siblings)
			{

				foreach (XmlNode node in siblings)
				{


					if (!(node is XmlElement)) continue;
					var element = node as XmlElement;

					var thisBinding = _TreeNodeBindings.FirstOrDefault(b => b.DataMember == element.LocalName);

					if (thisBinding != null)
					{

						builder.OpenComponent<TreeNode>(treeNodeCounter++);

						if (!string.IsNullOrEmpty(thisBinding.ImageToolTipField))
							builder.AddAttribute(treeNodeCounter++, "ImageToolTip", element.GetAttribute(thisBinding.ImageToolTipField));
						if (!string.IsNullOrEmpty(thisBinding.ImageUrlField))
							builder.AddAttribute(treeNodeCounter++, "ImageUrl", element.GetAttribute(thisBinding.ImageUrlField));
						if (!string.IsNullOrEmpty(thisBinding.NavigateUrlField))
							builder.AddAttribute(treeNodeCounter++, "NavigateUrl", element.GetAttribute(thisBinding.NavigateUrlField));
						if (!string.IsNullOrEmpty(thisBinding.TargetField))
							builder.AddAttribute(treeNodeCounter++, "Target", element.GetAttribute(thisBinding.TargetField));

						// Text must be present, no need to test
						builder.AddAttribute(treeNodeCounter++, "Text", element.GetAttribute(thisBinding.TextField));

						if (!string.IsNullOrEmpty(thisBinding.ToolTipField))
							builder.AddAttribute(treeNodeCounter++, "ToolTip", element.GetAttribute(thisBinding.ToolTipField));


						if (element.HasChildNodes)
						{
							builder.AddAttribute(treeNodeCounter++, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((_builder) =>
							{

								AddElements(_builder, element.ChildNodes);

							}));
						}
						builder.CloseComponent();
						OnTreeNodeDataBound.InvokeAsync(new TreeNodeEventArgs(null));

					}
				}

			}

		}

		private Task DataBindSiteMap(XmlDocument src)
		{

			_TreeNodeBindings.First().DataMember = "siteMapNode";

			return DataBindXml(src.SelectNodes("/siteMap/siteMapNode"));

		}

		private HashSet<TreeNodeBinding> _TreeNodeBindings = new HashSet<TreeNodeBinding>();
		internal void AddTreeNodeBinding(TreeNodeBinding treeNodeBinding)
		{
			_TreeNodeBindings.Add(treeNodeBinding);
		}

		#endregion

	}


}



