namespace BlazorWebFormsComponents.Interfaces
{
	/// <summary>
	/// Interface for components that contain FormView-specific TableItemStyle properties.
	/// </summary>
	public interface IFormViewStyleContainer
	{
		TableItemStyle RowStyle { get; }
		TableItemStyle EditRowStyle { get; }
		TableItemStyle InsertRowStyle { get; }
		TableItemStyle HeaderStyle { get; }
		TableItemStyle FooterStyle { get; }
		TableItemStyle EmptyDataRowStyle { get; }
		TableItemStyle PagerStyle { get; }
	}
}
