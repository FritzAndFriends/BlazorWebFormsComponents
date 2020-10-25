using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class SeparatorStyle : UiTableItemStyle
	{

		[CascadingParameter(Name = "SeparatorStyle")]
		protected TableItemStyle theSeparatorStyle
		{
			get { return base.theStyle; }
			set { base.theStyle = value; }
		}


	}
}
