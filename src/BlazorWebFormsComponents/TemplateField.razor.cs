using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// A template field column
	/// </summary>
	public partial class TemplateField<ItemType>: BaseColumn<ItemType>
	{
		/// <summary>
		/// The item template
		/// </summary>
		[Parameter] public RenderFragment<ItemType> ItemTemplate { get; set; }

		[Parameter] public ItemType Item { get; set; }

	}

}
