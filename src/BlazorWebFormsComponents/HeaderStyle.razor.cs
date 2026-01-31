using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{

	public partial class HeaderStyle : UiTableItemStyle
	{

		[CascadingParameter(Name = "ParentDataList")]
		protected object ParentDataList { get; set; }

		protected override void OnInitialized()
		{
			if (ParentDataList != null)
			{
				var parentType = ParentDataList.GetType();
				var headerStyleProperty = parentType.GetProperty("HeaderStyle", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
				if (headerStyleProperty != null)
				{
					theStyle = headerStyleProperty.GetValue(ParentDataList) as TableItemStyle;
				}
			}
			base.OnInitialized();
		}

	}
}
