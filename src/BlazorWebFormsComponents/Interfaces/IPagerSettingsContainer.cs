namespace BlazorWebFormsComponents.Interfaces
{
	/// <summary>
	/// Interface for components that contain a PagerSettings configuration.
	/// </summary>
	public interface IPagerSettingsContainer
	{
		/// <summary>
		/// Gets the pager settings for the control.
		/// </summary>
		PagerSettings PagerSettings { get; }
	}
}
