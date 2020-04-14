using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Bounds an object's property to a column by its property name 
	/// </summary>
	public partial class BoundField<ItemType> :  BaseColumn<ItemType>
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
		public string DataFormatString { get; set; } = null;

		
	}
}
