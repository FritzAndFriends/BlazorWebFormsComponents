using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Represents a data item in a ListView control
	/// </summary>
	public class ListViewDataItem : ListViewItem
	{
		/// <summary>
		/// Initializes a new instance of the ListViewDataItem class
		/// </summary>
		/// <param name="dataItemIndex">The index of the data item in the underlying data source</param>
		/// <param name="displayIndex">The position of the data item as displayed in the ListView control</param>
		public ListViewDataItem(int dataItemIndex, int displayIndex) 
			: base(ListViewItemType.DataItem)
		{
			DataItemIndex = dataItemIndex;
			DisplayIndex = displayIndex;
		}

		/// <summary>
		/// Gets or sets the underlying data object bound to the ListViewItem object
		/// </summary>
		public object DataItem { get; set; }

		/// <summary>
		/// Gets the index of the data item in the underlying data source
		/// </summary>
		public int DataItemIndex { get; }
	}
}
