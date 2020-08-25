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

		public override RenderFragment Render(ItemType item)
		{
			return ItemTemplate(item);
		}
	}
}
