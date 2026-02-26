namespace BlazorWebFormsComponents.Enums
{
	/// <summary>
	/// Specifies how ASP.NET generates the ClientID for a control.
	/// Matches System.Web.UI.ClientIDMode from Web Forms.
	/// </summary>
	public enum ClientIDMode
	{
		/// <summary>
		/// Inherit the ClientIDMode from the parent control. If no parent specifies a mode, defaults to Predictable.
		/// </summary>
		Inherit = 0,

		/// <summary>
		/// Legacy algorithm: concatenates parent naming container IDs with "ctl00" prefixes and sequential numbering.
		/// </summary>
		AutoID = 1,

		/// <summary>
		/// ClientID equals the raw ID value with no parent prefixing. Resets the naming hierarchy.
		/// </summary>
		Static = 2,

		/// <summary>
		/// Parent IDs concatenated with underscores, no "ctlxxx" numbering.
		/// </summary>
		Predictable = 3
	}
}
