using BlazorWebFormsComponents.GridView;

namespace BlazorWebFormsComponents.Interfaces
{
	public interface IColumn<ItemType>
	{
		GridView<ItemType> GridView { get; set; }

		string HeaderText { get; set; }
	}
}
