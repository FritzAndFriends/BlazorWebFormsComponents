namespace BlazorWebFormsComponents.Interfaces
{
	/// <summary>
	/// Interface for components that contain TableItemStyle properties
	/// </summary>
	public interface IDataListStyleContainer
	{
		TableItemStyle HeaderStyle { get; }
		TableItemStyle FooterStyle { get; }
		TableItemStyle ItemStyle { get; }
		TableItemStyle AlternatingItemStyle { get; }
		TableItemStyle SeparatorStyle { get; }
	}
}
