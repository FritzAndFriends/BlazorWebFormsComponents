using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Linq;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Bounds an object's property to a column by its property name 
	/// </summary>
	public partial class CheckBoxField<ItemType> : BaseColumn<ItemType>
	{
		/// <summary>
		/// </summary>
		[Parameter] public string DataField { get; set; }


		[Parameter] public string Text { get; set; }

		public override RenderFragment Render(ItemType item)
		{

			var value = (bool)(base.GetDataFields(item, DataField)[0]);

			return RenderCheckbox(value);
		}

	}
}
