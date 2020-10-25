using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class AlternatingItemStyle : UiTableItemStyle
	{
		[CascadingParameter(Name = "AlternatingItemStyle")]
		protected TableItemStyle theAlternatingItemStyle
		{
			get { return base.theStyle; }
			set { base.theStyle = value; }
		}
	}
}
