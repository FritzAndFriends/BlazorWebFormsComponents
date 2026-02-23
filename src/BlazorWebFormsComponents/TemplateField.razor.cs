using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// A template field column
	/// </summary>
	public partial class TemplateField<ItemType> : BaseColumn<ItemType>
	{
		/// <summary>
		/// The item template
		/// </summary>
		[Parameter] public RenderFragment<ItemType> ItemTemplate { get; set; }

		/// <summary>
		/// The template to display when the row is in edit mode.
		/// </summary>
		[Parameter] public RenderFragment<ItemType> EditItemTemplate { get; set; }

		public override RenderFragment Render(ItemType item)
		{
			return ItemTemplate(item);
		}

		public override RenderFragment RenderEdit(ItemType item)
		{
			if (EditItemTemplate != null)
			{
				return EditItemTemplate(item);
			}
			return Render(item);
		}
	}
}
