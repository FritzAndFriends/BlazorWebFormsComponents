using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class ItemStyle : UiTableItemStyle
	{

		[CascadingParameter(Name = "ParentDataList")]
		protected object ParentDataList { get; set; }

		protected override void OnInitialized()
		{
			if (ParentDataList != null)
			{
				var parentType = ParentDataList.GetType();
				var itemStyleProperty = parentType.GetProperty("ItemStyle", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
				if (itemStyleProperty != null)
				{
					theStyle = itemStyleProperty.GetValue(ParentDataList) as TableItemStyle;
				}
			}
			base.OnInitialized();
		}


	}
}
