using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Represents an item in a ListView control
	/// </summary>
	public class ListViewItem
	{
		/// <summary>
		/// Initializes a new instance of the ListViewItem class
		/// </summary>
		/// <param name="itemType">The type of the item</param>
		public ListViewItem(ListViewItemType itemType)
		{
			ItemType = itemType;
		}

		/// <summary>
		/// Gets the type of the item in the ListView control
		/// </summary>
		public ListViewItemType ItemType { get; }

		/// <summary>
		/// Gets or sets the index of the item in the ListView control
		/// </summary>
		public int DisplayIndex { get; set; }
	}
}
