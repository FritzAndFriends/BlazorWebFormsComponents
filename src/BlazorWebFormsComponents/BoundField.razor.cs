using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Binds an object's property to a column by its property name 
	/// </summary>
	public partial class BoundField<ItemType> : BaseColumn<ItemType>
	{
		/// <summary>
		/// Specifies the name of the object's property bound to the column
		/// </summary>
		[Parameter]
		public string DataField { get; set; }

		/// <summary>
		/// Specifies which string format should be used.
		/// </summary>
		[Parameter]
		public string DataFormatString { get; set; }

		public override RenderFragment Render(ItemType item)
		{
			var properties = DataField.Split('.');
			object obj = item;
			foreach (var property in properties)
			{
				obj = DataBinder.GetPropertyValue(obj, property);
			}
			if (DataFormatString != null)
			{
				return RenderString(string.Format(DataFormatString, obj?.ToString()));
			}
			else
			{
				return RenderString(obj?.ToString());
			}
		}
	}
}
