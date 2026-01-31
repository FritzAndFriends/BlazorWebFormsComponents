using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class FooterStyle : UiTableItemStyle
	{
		[CascadingParameter(Name = "ParentDataList")]
		protected object ParentDataList { get; set; }

		protected override void OnInitialized()
		{
			if (ParentDataList != null)
			{
				var parentType = ParentDataList.GetType();
				var footerStyleProperty = parentType.GetProperty("FooterStyle", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
				if (footerStyleProperty != null)
				{
					theStyle = footerStyleProperty.GetValue(ParentDataList) as TableItemStyle;
				}
			}
			base.OnInitialized();
		}
	}
}
