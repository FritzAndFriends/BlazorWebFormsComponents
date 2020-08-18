namespace BlazorWebFormsComponents.Enums
{
  public enum TreeNodeSelectAction
  {

	// Raises the SelectedNodeChanged event when a node is selected.
	Select = 0,

	// Raises the TreeNodeExpanded event when a node is selected.
	Expand = 1,

	// Raises both the SelectedNodeChanged and TreeNodeExpanded events when a node is selected.
	SelectExpand = 2,

	// Raises no events when a node is selected.
	None = 3
  }
}
