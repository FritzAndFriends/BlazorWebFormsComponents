namespace BlazorWebFormsComponents.Enums
{
	/// <summary>
	/// Specifies the position of the first and last buttons in a DataPager control.
	/// </summary>
	public enum PagerButtons
	{
		/// <summary>
		/// Display Next and Previous buttons.
		/// </summary>
		NextPrevious = 0,

		/// <summary>
		/// Display numeric page buttons.
		/// </summary>
		Numeric = 1,

		/// <summary>
		/// Display Next, Previous, First, and Last buttons.
		/// </summary>
		NextPreviousFirstLast = 2,

		/// <summary>
		/// Display numeric page buttons with First and Last buttons.
		/// </summary>
		NumericFirstLast = 3
	}
}
