namespace BlazorWebFormsComponents.Interfaces
{
	/// <summary>
	/// Generic row interface
	/// </summary>
  public interface IRow<ItemType>
  {
		/// <summary>
		/// The parent IRowCollection where the IRow resides
		/// </summary>
		IRowCollection<ItemType> RowCollection { get; set; }

		/// <summary>
		/// The object bound to the row
		/// </summary>
		ItemType DataItem { get; set; }
	}
}
