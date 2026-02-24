using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Represents the properties of the paging controls in a control that supports pagination.
	/// Matches the System.Web.UI.WebControls.PagerSettings class from ASP.NET Web Forms.
	/// </summary>
	public class PagerSettings
	{
		/// <summary>
		/// Gets or sets the mode of the pager buttons.
		/// </summary>
		public PagerButtons Mode { get; set; } = PagerButtons.Numeric;

		/// <summary>
		/// Gets or sets the number of numeric page buttons to display in the pager.
		/// </summary>
		public int PageButtonCount { get; set; } = 10;

		/// <summary>
		/// Gets or sets the text displayed for the first page button.
		/// </summary>
		public string FirstPageText { get; set; } = "...";

		/// <summary>
		/// Gets or sets the text displayed for the last page button.
		/// </summary>
		public string LastPageText { get; set; } = "...";

		/// <summary>
		/// Gets or sets the text displayed for the next page button.
		/// </summary>
		public string NextPageText { get; set; } = ">";

		/// <summary>
		/// Gets or sets the text displayed for the previous page button.
		/// </summary>
		public string PreviousPageText { get; set; } = "<";

		/// <summary>
		/// Gets or sets the URL of the image displayed for the first page button.
		/// </summary>
		public string FirstPageImageUrl { get; set; }

		/// <summary>
		/// Gets or sets the URL of the image displayed for the last page button.
		/// </summary>
		public string LastPageImageUrl { get; set; }

		/// <summary>
		/// Gets or sets the URL of the image displayed for the next page button.
		/// </summary>
		public string NextPageImageUrl { get; set; }

		/// <summary>
		/// Gets or sets the URL of the image displayed for the previous page button.
		/// </summary>
		public string PreviousPageImageUrl { get; set; }

		/// <summary>
		/// Gets or sets the position of the pager in the control.
		/// </summary>
		public PagerPosition Position { get; set; } = PagerPosition.Bottom;

		/// <summary>
		/// Gets or sets whether the pager is visible.
		/// </summary>
		public bool Visible { get; set; } = true;
	}
}
