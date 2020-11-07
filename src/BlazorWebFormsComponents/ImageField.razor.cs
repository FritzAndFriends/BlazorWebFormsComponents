using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Linq;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Image field for use in a GridView
	/// </summary>
	public partial class ImageField<ItemType> : BaseColumn<ItemType> {

		[Parameter] public string DataAlternateTextField { get; set; }

		[Parameter] public string DataAlternateTextFormatString { get; set; }

		[Parameter] public string DataImageUrlField { get; set; }

		[Parameter] public string DataImageUrlFormatString { get; set; }

		[Parameter] public string NullDisplayText { get; set; }

		[Parameter] public string NullImageUrl { get; set; }

		public override RenderFragment Render(ItemType item)
		{

			string altText;
			if (string.IsNullOrEmpty(DataAlternateTextFormatString))
			{
				altText = GetDataFields(item, DataAlternateTextField)[0]?.ToString() ?? NullDisplayText;
			}
			else
			{
				var textArgs = GetDataFields(item, DataAlternateTextField);
				altText = textArgs.Any(a => a is null) ? NullDisplayText : string.Format(CultureInfo.CurrentCulture, DataAlternateTextFormatString, textArgs);
			}

			string imageUrl;
			if (string.IsNullOrEmpty(DataImageUrlFormatString))
			{
				imageUrl = GetDataFields(item, DataImageUrlField)[0]?.ToString() ?? NullImageUrl;
			}
			else
			{
				var urlArgs = GetDataFields(item, DataImageUrlField);
				imageUrl = urlArgs.Any(a => a is null) ? NullImageUrl : string.Format(CultureInfo.CurrentCulture, DataImageUrlFormatString, urlArgs);
			}

			return RenderImage(altText, imageUrl);

		}

	}
}
