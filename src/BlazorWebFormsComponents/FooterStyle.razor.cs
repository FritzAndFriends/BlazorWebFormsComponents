using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class FooterStyle : UiTableItemStyle
	{
		[CascadingParameter(Name = "FooterStyle")]
		protected TableItemStyle TheFooterStyle
		{
			get { return base.theStyle; }
			set { base.theStyle = value; }
		}
	}
}
