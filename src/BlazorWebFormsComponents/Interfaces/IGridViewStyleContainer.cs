namespace BlazorWebFormsComponents.Interfaces
{
	/// <summary>
	/// Interface for components that contain GridView-specific TableItemStyle properties.
	/// </summary>
	public interface IGridViewStyleContainer
	{
		TableItemStyle RowStyle { get; }
		TableItemStyle AlternatingRowStyle { get; }
		TableItemStyle HeaderStyle { get; }
		TableItemStyle FooterStyle { get; }
		TableItemStyle EmptyDataRowStyle { get; }
		TableItemStyle PagerStyle { get; }
		TableItemStyle EditRowStyle { get; }
		TableItemStyle SelectedRowStyle { get; }
	}
}
