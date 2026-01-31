using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class SeparatorStyle : UiTableItemStyle
	{

		[CascadingParameter(Name = "ParentDataList")]
		protected object ParentDataList { get; set; }

		protected override void OnInitialized()
		{
			if (ParentDataList != null)
			{
				var parentType = ParentDataList.GetType();
				var separatorStyleProperty = parentType.GetProperty("SeparatorStyle", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
				if (separatorStyleProperty != null)
				{
					theStyle = separatorStyleProperty.GetValue(ParentDataList) as TableItemStyle;
				}
			}
			base.OnInitialized();
		}


	}
}
