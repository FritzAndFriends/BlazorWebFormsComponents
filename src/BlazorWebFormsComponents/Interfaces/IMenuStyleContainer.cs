namespace BlazorWebFormsComponents.Interfaces
{
	/// <summary>
	/// Interface for components that contain Menu-specific style properties.
	/// </summary>
	public interface IMenuStyleContainer
	{
		MenuItemStyle DynamicMenuStyle { get; set; }
		MenuItemStyle StaticMenuStyle { get; set; }
		MenuItemStyle DynamicMenuItemStyle { get; set; }
		MenuItemStyle StaticMenuItemStyle { get; set; }
	}
}
