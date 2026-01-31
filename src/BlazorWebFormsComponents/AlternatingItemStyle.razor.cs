using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class AlternatingItemStyle : UiTableItemStyle
	{
		[CascadingParameter(Name = "ParentDataList")]
		protected object ParentDataList { get; set; }

		protected override void OnInitialized()
		{
			if (ParentDataList != null)
			{
				var parentType = ParentDataList.GetType();
				var alternatingItemStyleProperty = parentType.GetProperty("AlternatingItemStyle", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
				if (alternatingItemStyleProperty != null)
				{
					theStyle = alternatingItemStyleProperty.GetValue(ParentDataList) as TableItemStyle;
				}
			}
			base.OnInitialized();
		}
	}
}
