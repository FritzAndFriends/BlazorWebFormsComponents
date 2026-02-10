using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Represents a node in a site navigation hierarchy used by SiteMapPath.
	/// </summary>
	public class SiteMapNode
	{
		/// <summary>
		/// Gets or sets the title of the node displayed in the breadcrumb.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the URL that the node links to.
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the description used for tooltips.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the parent node. Null for root nodes.
		/// </summary>
		public SiteMapNode ParentNode { get; set; }

		/// <summary>
		/// Gets or sets the child nodes.
		/// </summary>
		public List<SiteMapNode> ChildNodes { get; set; } = new List<SiteMapNode>();

		/// <summary>
		/// Gets a value indicating whether this node is a root node.
		/// </summary>
		public bool IsRootNode => ParentNode == null;

		/// <summary>
		/// Creates an empty SiteMapNode.
		/// </summary>
		public SiteMapNode() { }

		/// <summary>
		/// Creates a SiteMapNode with specified title and URL.
		/// </summary>
		/// <param name="title">The title to display.</param>
		/// <param name="url">The URL to navigate to.</param>
		public SiteMapNode(string title, string url)
		{
			Title = title;
			Url = url;
		}

		/// <summary>
		/// Creates a SiteMapNode with specified title, URL, and description.
		/// </summary>
		/// <param name="title">The title to display.</param>
		/// <param name="url">The URL to navigate to.</param>
		/// <param name="description">The description for tooltips.</param>
		public SiteMapNode(string title, string url, string description)
		{
			Title = title;
			Url = url;
			Description = description;
		}

		/// <summary>
		/// Adds a child node and sets its parent to this node.
		/// </summary>
		/// <param name="child">The child node to add.</param>
		public void AddChild(SiteMapNode child)
		{
			child.ParentNode = this;
			ChildNodes.Add(child);
		}

		/// <summary>
		/// Finds a node by URL in this node's hierarchy (including this node and all descendants).
		/// </summary>
		/// <param name="url">The URL to search for.</param>
		/// <returns>The matching node or null if not found.</returns>
		public SiteMapNode FindByUrl(string url)
		{
			if (MatchesUrl(url))
				return this;

			foreach (var child in ChildNodes)
			{
				var found = child.FindByUrl(url);
				if (found != null)
					return found;
			}

			return null;
		}

		/// <summary>
		/// Determines whether this node's URL matches the specified URL.
		/// Handles relative and absolute URL comparisons.
		/// </summary>
		/// <param name="url">The URL to compare.</param>
		/// <returns>True if URLs match; otherwise false.</returns>
		public bool MatchesUrl(string url)
		{
			if (string.IsNullOrEmpty(Url) || string.IsNullOrEmpty(url))
				return false;

			// Normalize URLs for comparison
			var thisUrl = NormalizeUrl(Url);
			var otherUrl = NormalizeUrl(url);

			return string.Equals(thisUrl, otherUrl, System.StringComparison.OrdinalIgnoreCase);
		}

		private static string NormalizeUrl(string url)
		{
			if (string.IsNullOrEmpty(url))
				return url;

			// Remove leading ~/ for ASP.NET style paths
			if (url.StartsWith("~/"))
				url = url.Substring(1);

			// Ensure leading slash
			if (!url.StartsWith("/"))
				url = "/" + url;

			// Remove trailing slash (except for root)
			if (url.Length > 1 && url.EndsWith("/"))
				url = url.TrimEnd('/');

			// Remove query string for matching
			var queryIndex = url.IndexOf('?');
			if (queryIndex >= 0)
				url = url.Substring(0, queryIndex);

			return url;
		}

		/// <summary>
		/// Gets the path from root to this node as a list.
		/// </summary>
		/// <returns>List of nodes from root to this node.</returns>
		public List<SiteMapNode> GetPathFromRoot()
		{
			var path = new List<SiteMapNode>();
			var current = this;

			while (current != null)
			{
				path.Insert(0, current);
				current = current.ParentNode;
			}

			return path;
		}
	}
}
