using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Interfaces
{
	/// <summary>
	/// Generic column interface
	/// </summary>
	public interface IColumn<ItemType>
	{

		/// <summary>
		/// The footer text of the column
		/// </summary>
		string FooterText { get; set; }

		/// <summary>
		/// The style to apply to the Footer of the column
		/// </summary>
		TableItemStyle FooterStyle { get; set; }

		/// <summary>
		/// The header text of the column
		/// </summary>
		string HeaderText { get; set; }

		/// <summary>
		/// The style to apply to the Header of the column
		/// </summary>
		TableItemStyle HeaderStyle { get; set; }

		/// <summary>
		/// The parent IColumnCollection where the IColumn resides
		/// </summary>
		IColumnCollection<ItemType> ParentColumnsCollection { get; set; }
		RenderFragment Render(ItemType item);
	}
}
