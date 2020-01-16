using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class HyperLinkStyle : UiStyle
	{
		[CascadingParameter(Name = "HyperLinkStyle")]
		protected TableItemStyle theHyperLinkStyle
		{
			get { return theStyle; }
			set { theStyle = value; }
		}
	}
}
