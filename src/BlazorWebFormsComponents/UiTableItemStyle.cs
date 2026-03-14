using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public abstract class UiTableItemStyle : UiStyle<TableItemStyle>, IHasLayoutTableItemStyle
	{

		[Parameter]
		public EnumParameter<HorizontalAlign> HorizontalAlign { get; set; }

		[Parameter]
		public EnumParameter<VerticalAlign> VerticalAlign { get; set; }

		[Parameter]
		public bool Wrap { get; set; } = true;


		protected override void OnInitialized()
		{

			if (theStyle != null)
			{

				base.OnInitialized();

				theStyle.HorizontalAlign = HorizontalAlign;
				theStyle.VerticalAlign = VerticalAlign;
				theStyle.Wrap = Wrap;

				theStyle.SetFontsFromAttributes(AdditionalAttributes);

			}

		}

	}

}
