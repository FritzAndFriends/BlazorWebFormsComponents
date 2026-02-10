namespace BlazorWebFormsComponents.Enums
{
	/// <summary>
	/// Specifies the direction in which a SiteMapPath control renders navigation nodes.
	/// </summary>
	public enum PathDirection
	{
		/// <summary>
		/// Navigation nodes are rendered from root to current node (left to right).
		/// </summary>
		RootToCurrent = 0,

		/// <summary>
		/// Navigation nodes are rendered from current node to root (right to left).
		/// </summary>
		CurrentToRoot = 1
	}
}
