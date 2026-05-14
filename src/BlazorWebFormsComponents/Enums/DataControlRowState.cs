using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Specifies the state of a row in a data control such as GridView.
	/// Mirrors System.Web.UI.WebControls.DataControlRowState for migration compatibility.
	/// </summary>
	[Flags]
	public enum DataControlRowState
	{
		Normal = 0,
		Alternate = 1,
		Selected = 2,
		Edit = 4,
		Insert = 8
	}
}
