using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class ItemStyle : UiTableItemStyle
	{

		[CascadingParameter(Name = "ItemStyle")]
		protected TableItemStyle theItemStyle
		{
			get { return base.theStyle; }
			set { base.theStyle = value; }
		}


	}
}
