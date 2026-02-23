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
		/// The sort expression for the column
		/// </summary>
		string SortExpression { get; set; }

		/// <summary>
		/// The parent IColumnCollection where the IColumn resides
		/// </summary>
		IColumnCollection<ItemType> ParentColumnsCollection { get; set; }
		RenderFragment Render(ItemType item);

		/// <summary>
		/// Renders the column in edit mode. Falls back to Render if not overridden.
		/// </summary>
		RenderFragment RenderEdit(ItemType item);
	}
}
