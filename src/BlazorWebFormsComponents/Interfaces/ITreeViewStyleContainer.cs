namespace BlazorWebFormsComponents.Interfaces
{
	/// <summary>
	/// Interface for components that contain TreeView-specific TreeNodeStyle properties.
	/// </summary>
	public interface ITreeViewStyleContainer
	{
		TreeNodeStyle NodeStyle { get; }
		TreeNodeStyle HoverNodeStyle { get; }
		TreeNodeStyle LeafNodeStyle { get; }
		TreeNodeStyle ParentNodeStyle { get; }
		TreeNodeStyle RootNodeStyle { get; }
		TreeNodeStyle SelectedNodeStyle { get; }
	}
}
