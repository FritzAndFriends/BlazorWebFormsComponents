namespace BlazorWebFormsComponents.Interfaces
{
	/// <summary>
	/// Generic row interface
	/// </summary>
  public interface IRow
  {
		/// <summary>
		/// The parent IRowCollection where the IRow resides
		/// </summary>
		IRowCollection RowCollection { get; set; }

		/// <summary>
		/// The object bound to the row
		/// </summary>
		object DataItem { get; set; }
	}
}
