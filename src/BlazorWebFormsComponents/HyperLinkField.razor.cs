using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Linq;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Bounds an object's property to a column by its property name 
	/// </summary>
	public partial class HyperLinkField<ItemType> : BaseColumn<ItemType>
	{
		/// <summary>
		/// An ordered, comma-separated list of data fields to use as string substitutes for the URL to navigate. The format string is specified by the <see cref="DataNavigateUrlFormatString"/> property.
		/// </summary>
		[Parameter] public string DataNavigateUrlFields { get; set; }
		/// <summary>
		/// A format string for generating the URL to navigate. The string substitution fields are specified by the <see cref="DataNavigateUrlFields"/> property.
		/// </summary>
		[Parameter] public string DataNavigateUrlFormatString { get; set; }
		/// <summary>
		/// The data field to use for the text of the hyperlink.
		/// </summary>
		[Parameter] public string DataTextField { get; set; }
		/// <summary>
		/// The format string to apply to the data specified by the <see cref="DataTextField"/> property.
		/// </summary>
		[Parameter] public string DataTextFormatString { get; set; }
		/// <summary>
		/// The static URL to navigate to. If <see cref="DataNavigateUrlFormatString"/> is specified, that will override the value of this property.
		/// </summary>
		[Parameter] public string NavigateUrl { get; set; }
		/// <summary>
		/// The target window or frame for the hyperlink.
		/// </summary>
		[Parameter] public string Target { get; set; }
		/// <summary>
		/// The text for the hyperlink. If <see cref="DataTextFormatString"/> is specified, that will override the value of this property.
		/// </summary>
		[Parameter] public string Text { get; set; }

		public override RenderFragment Render(ItemType item)
		{
			string text;
			if (string.IsNullOrEmpty(DataTextFormatString))
			{
				text = Text;
			}
			else
			{
				var textArgs = GetDataFields(item, DataTextField);
				text = string.Format(CultureInfo.CurrentCulture, DataTextFormatString, textArgs);
			}

			string url;
			if (string.IsNullOrEmpty(DataNavigateUrlFormatString))
			{
				url = NavigateUrl;
			}
			else
			{
				var urlArgs = GetDataFields(item, DataNavigateUrlFields);
				url = string.Format(CultureInfo.CurrentCulture, DataNavigateUrlFormatString, urlArgs);
			}

			var target = Target;

			return RenderAnchor(text, url, target);
		}

		private object[] GetDataFields(ItemType item, string dataFieldNames)
		{
			var dataFields = dataFieldNames.Split(',').Select(s => s.Trim()).ToList();
			var fields = new object[dataFields.Count];
			for (var i = 0; i < dataFields.Count; i++)
			{
				fields[i] = DataBinder.GetPropertyValue(item, dataFields[i]);
			}
			return fields;
		}
	}
}
