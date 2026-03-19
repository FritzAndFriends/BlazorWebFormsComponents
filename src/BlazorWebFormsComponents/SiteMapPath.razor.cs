using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorWebFormsComponents.Enums;

using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Displays a breadcrumb navigation path showing the current page's location within a site hierarchy.
	/// </summary>
	public partial class SiteMapPath : BaseStyledComponent
	{
		/// <summary>
		/// Gets or sets the root node of the site map hierarchy.
		/// </summary>
		[Parameter]
		public SiteMapNode SiteMapProvider { get; set; }

		/// <summary>
		/// Gets or sets the separator string displayed between path nodes.
		/// Default is " > ".
		/// </summary>
		[Parameter]
		public string PathSeparator { get; set; } = " > ";

		/// <summary>
		/// Gets or sets a custom template for rendering the path separator.
		/// When set, PathSeparator string is ignored.
		/// </summary>
		[Parameter]
		public RenderFragment PathSeparatorTemplate { get; set; }

		/// <summary>
		/// Gets or sets the direction of path rendering.
		/// </summary>
		[Parameter]
		public EnumParameter<PathDirection> PathDirection { get; set; } = Enums.PathDirection.RootToCurrent;

		/// <summary>
		/// Gets or sets whether the current node is rendered as a hyperlink.
		/// Default is false.
		/// </summary>
		[Parameter]
		public bool RenderCurrentNodeAsLink { get; set; } = false;

		/// <summary>
		/// Gets or sets whether tooltips are shown on navigation nodes.
		/// Default is true.
		/// </summary>
		[Parameter]
		public bool ShowToolTips { get; set; } = true;

		/// <summary>
		/// Gets or sets the number of parent levels to display.
		/// -1 means display all parent levels. Default is -1.
		/// </summary>
		[Parameter]
		public int ParentLevelsDisplayed { get; set; } = -1;

		/// <summary>
		/// Gets or sets the template for rendering the current (active) node.
		/// </summary>
		[Parameter]
		public RenderFragment<SiteMapNode> CurrentNodeTemplate { get; set; }

		/// <summary>
		/// Gets or sets the template for rendering regular path nodes.
		/// </summary>
		[Parameter]
		public RenderFragment<SiteMapNode> NodeTemplate { get; set; }

		/// <summary>
		/// Gets or sets the template for rendering the root node.
		/// </summary>
		[Parameter]
		public RenderFragment<SiteMapNode> RootNodeTemplate { get; set; }

		/// <summary>
		/// Gets or sets the current URL to match against site map nodes.
		/// If not set, attempts to detect from NavigationManager.
		/// </summary>
		[Parameter]
		public string CurrentUrl { get; set; }

		/// <summary>
		/// Gets or sets the text for the accessibility skip navigation link.
		/// </summary>
		[Parameter]
		public string SkipLinkText { get; set; } = "Skip Navigation Links";

		/// <summary>
		/// Gets or sets custom child content (not typically used).
		/// </summary>
		[Parameter]
		public RenderFragment ChildContent { get; set; }

		/// <summary>
		/// Occurs when a node item is created during rendering.
		/// </summary>
		[Parameter]
		public EventCallback<SiteMapNodeItemEventArgs> ItemCreated { get; set; }

		/// <summary>
		/// Occurs when a node item is data-bound during rendering.
		/// </summary>
		[Parameter]
		public EventCallback<SiteMapNodeItemEventArgs> ItemDataBound { get; set; }

		// Style properties for different node types
		private CurrentNodeStyle _currentNodeStyle = new CurrentNodeStyle();
		private NodeStyle _nodeStyle = new NodeStyle();
		private RootNodeStyle _rootNodeStyle = new RootNodeStyle();
		private PathSeparatorStyle _pathSeparatorStyle = new PathSeparatorStyle();

		/// <summary>
		/// Gets or sets the style for the current node.
		/// </summary>
		public CurrentNodeStyle CurrentNodeStyle
		{
			get => _currentNodeStyle;
			set
			{
				_currentNodeStyle = value;
				StateHasChanged();
			}
		}

		/// <summary>
		/// Gets or sets the style for regular path nodes.
		/// </summary>
		public NodeStyle NodeStyle
		{
			get => _nodeStyle;
			set
			{
				_nodeStyle = value;
				StateHasChanged();
			}
		}

		/// <summary>
		/// Gets or sets the style for the root node.
		/// </summary>
		public RootNodeStyle RootNodeStyle
		{
			get => _rootNodeStyle;
			set
			{
				_rootNodeStyle = value;
				StateHasChanged();
			}
		}

		/// <summary>
		/// Gets or sets the style for the path separator.
		/// </summary>
		public PathSeparatorStyle PathSeparatorStyle
		{
			get => _pathSeparatorStyle;
			set
			{
				_pathSeparatorStyle = value;
				StateHasChanged();
			}
		}

		/// <summary>
		/// Gets the navigation path from root to current node.
		/// </summary>
		protected List<SiteMapNode> NavigationPath { get; private set; } = new List<SiteMapNode>();

		private string _lastBuiltUrl;

		protected override async Task OnParametersSetAsync()
		{
			await base.OnParametersSetAsync();
			BuildNavigationPath();

			// Fire lifecycle events only when the URL changes
			var url = CurrentUrl ?? string.Empty;
			if (url != _lastBuiltUrl && NavigationPath.Count > 0)
			{
				_lastBuiltUrl = url;

				if (ItemCreated.HasDelegate)
				{
					foreach (var node in NavigationPath)
					{
						await ItemCreated.InvokeAsync(new SiteMapNodeItemEventArgs(node));
					}
				}

				if (ItemDataBound.HasDelegate)
				{
					foreach (var node in NavigationPath)
					{
						await ItemDataBound.InvokeAsync(new SiteMapNodeItemEventArgs(node));
					}
				}
			}
		}

		private void BuildNavigationPath()
		{
			NavigationPath.Clear();

			if (SiteMapProvider == null)
				return;

			// Determine current URL
			var url = CurrentUrl;
			if (string.IsNullOrEmpty(url))
			{
				// If no URL provided, the path will be empty
				// In a real scenario, inject NavigationManager to get current URI
				return;
			}

			// Find the current node in the site map
			var currentNode = SiteMapProvider.FindByUrl(url);
			if (currentNode == null)
				return;

			// Build path from root to current
			var fullPath = currentNode.GetPathFromRoot();

			// Apply ParentLevelsDisplayed limit
			if (ParentLevelsDisplayed >= 0 && fullPath.Count > ParentLevelsDisplayed + 1)
			{
				// Keep root + last N parents + current
				var startIndex = fullPath.Count - ParentLevelsDisplayed - 1;
				fullPath = fullPath.Skip(startIndex).ToList();
			}

			// Apply path direction
			if (PathDirection.Value == Enums.PathDirection.CurrentToRoot)
			{
				fullPath.Reverse();
			}

			NavigationPath = fullPath;
		}

		/// <summary>
		/// Determines if the given node is the current (last) node in the path.
		/// </summary>
		protected bool IsCurrentNode(SiteMapNode node)
		{
			if (NavigationPath.Count == 0)
				return false;

			var lastNode = PathDirection.Value == Enums.PathDirection.RootToCurrent
				? NavigationPath.Last()
				: NavigationPath.First();

			return ReferenceEquals(node, lastNode);
		}

		/// <summary>
		/// Determines if the given node is the root (first) node in the path.
		/// </summary>
		protected bool IsRootNode(SiteMapNode node)
		{
			if (NavigationPath.Count == 0)
				return false;

			var firstNode = PathDirection.Value == Enums.PathDirection.RootToCurrent
				? NavigationPath.First()
				: NavigationPath.Last();

			return ReferenceEquals(node, firstNode);
		}

		/// <summary>
		/// Gets the tooltip text for a node.
		/// </summary>
		protected string GetTooltip(SiteMapNode node)
		{
			if (!ShowToolTips)
				return null;

			return !string.IsNullOrEmpty(node.Description) ? node.Description : node.Title;
		}

		/// <summary>
		/// Gets the inline style for a node based on its position.
		/// </summary>
		protected string GetNodeStyle(SiteMapNode node)
		{
			Style style;
			if (IsCurrentNode(node))
				style = _currentNodeStyle;
			else if (IsRootNode(node))
				style = _rootNodeStyle;
			else
				style = _nodeStyle;

			if (style == null)
				return null;

			var result = style.ToString();
			return string.IsNullOrEmpty(result) ? null : result;
		}

		/// <summary>
		/// Gets the inline style for the path separator.
		/// </summary>
		protected string GetSeparatorStyle()
		{
			if (_pathSeparatorStyle == null)
				return null;

			var result = _pathSeparatorStyle.ToString();
			return string.IsNullOrEmpty(result) ? null : result;
		}
	}

	// Style component classes for SiteMapPath - implement IStyle for ToStyle() extension
	public class CurrentNodeStyle : Style { }
	public class NodeStyle : Style { }
	public class RootNodeStyle : Style { }
	public class PathSeparatorStyle : Style { }
}
