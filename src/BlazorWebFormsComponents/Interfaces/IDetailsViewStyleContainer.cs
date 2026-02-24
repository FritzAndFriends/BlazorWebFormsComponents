namespace BlazorWebFormsComponents.Interfaces
{
	/// <summary>
	/// Interface for components that contain DetailsView-specific TableItemStyle properties.
	/// </summary>
	public interface IDetailsViewStyleContainer
	{
		TableItemStyle RowStyle { get; }
		TableItemStyle AlternatingRowStyle { get; }
		TableItemStyle HeaderStyle { get; }
		TableItemStyle FooterStyle { get; }
		TableItemStyle CommandRowStyle { get; }
		TableItemStyle EditRowStyle { get; }
		TableItemStyle InsertRowStyle { get; }
		TableItemStyle FieldHeaderStyle { get; }
		TableItemStyle EmptyDataRowStyle { get; }
		TableItemStyle PagerStyle { get; }
	}
}
