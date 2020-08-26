using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{

	public partial class HeaderStyle : UiTableItemStyle
	{

		[CascadingParameter(Name = "HeaderStyle")]
		protected TableItemStyle theHeaderStyle
		{
			get { return base.theStyle; }
			set { base.theStyle = value; }
		}

	}
}
