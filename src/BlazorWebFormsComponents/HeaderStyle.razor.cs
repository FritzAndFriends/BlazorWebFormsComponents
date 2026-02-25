using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{

	public partial class HeaderStyle : UiTableItemStyle
	{

		[CascadingParameter(Name = "ParentDataList")]
		protected IDataListStyleContainer ParentDataList { get; set; }

		protected override void OnInitialized()
		{
			if (ParentDataList != null)
			{
				theStyle = ParentDataList.HeaderStyle;
			}
			base.OnInitialized();
		}

	}
}
