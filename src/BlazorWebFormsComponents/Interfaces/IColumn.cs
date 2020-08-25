using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Interfaces
{
  /// <summary>
  /// Generic column interface
  /// </summary>
  public interface IColumn<ItemType>
  {
	/// <summary>
	/// The header text of the column
	/// </summary>
	string HeaderText { get; set; }

	/// <summary>
	/// The parent IColumnCollection where the IColumn resides
	/// </summary>
	IColumnCollection<ItemType> ParentColumnsCollection { get; set; }
		RenderFragment Render(ItemType item);
  }
}
