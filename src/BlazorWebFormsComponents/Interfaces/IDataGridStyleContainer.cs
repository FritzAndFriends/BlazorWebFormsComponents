namespace BlazorWebFormsComponents.Interfaces
{
	/// <summary>
	/// Interface for components that contain DataGrid-specific TableItemStyle properties.
	/// </summary>
	public interface IDataGridStyleContainer
	{
		TableItemStyle AlternatingItemStyle { get; }
		TableItemStyle ItemStyle { get; }
		TableItemStyle HeaderStyle { get; }
		TableItemStyle FooterStyle { get; }
		TableItemStyle PagerStyle { get; }
		TableItemStyle SelectedItemStyle { get; }
		TableItemStyle EditItemStyle { get; }
	}
}
