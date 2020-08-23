using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using BlazorWebFormsComponents.Enums;
using BlazorComponentUtilities;

namespace BlazorWebFormsComponents
{
	public partial class AdRotator : BaseWebFormsComponent, IHasStyle
	{
		[Parameter]
		public string AdvertisementFile { get; set; }

		[Parameter]
		public string KeywordFilter { get; set; } = string.Empty;

		[Parameter]
		public string Target { get; set; }

		[Parameter]
		public WebColor BackColor { get; set; }

		[Parameter]
		public WebColor BorderColor { get; set; }

		[Parameter]
		public BorderStyle BorderStyle { get; set; }

		[Parameter]
		public Unit BorderWidth { get; set; }

		[Parameter]
		public string CssClass { get; set; }

		[Parameter]
		public WebColor ForeColor { get; set; }

		[Parameter]
		public Unit Height { get; set; }

		[Parameter]
		public Unit Width { get; set; }

		[Parameter]
		public bool Font_Bold { get; set; }

		[Parameter]
		public bool Font_Italic { get; set; }

		[Parameter]
		public string Font_Names { get; set; }

		[Parameter]
		public bool Font_Overline { get; set; }

		[Parameter]
		public FontUnit Font_Size { get; set; }

		[Parameter]
		public bool Font_Strikeout { get; set; }

		[Parameter]
		public bool Font_Underline { get; set; }

		private string CalculatedStyle => this.ToStyle().Build().NullIfEmpty();

		internal Advertisment GetActiveAdvertisment()
		{
			var advertisments = GetAdvertismentsFileContent(AdvertisementFile);

			if (advertisments == null)
			{
				return null;
			}

			if (KeywordFilter != string.Empty)
			{
				advertisments = advertisments.Where(a => a.Keyword == KeywordFilter);
			}

			if (advertisments.Count() == 0)
			{
				return null;
			}

			var rnd = new Random().Next(advertisments.Count());

			return advertisments.ElementAt(rnd);
		}

		private static IEnumerable<Advertisment> GetAdvertismentsFileContent(string fileName)
		{
			var xmlDocument = XDocument.Load(new StreamReader(fileName));

			return xmlDocument.Descendants("Ad")
				.Select(a => new Advertisment
				{
					ImageUrl = a.Descendants("ImageUrl").FirstOrDefault()?.Value,
					Height = a.Descendants("Height").FirstOrDefault()?.Value,
					Width = a.Descendants("Width").FirstOrDefault()?.Value,
					NavigateUrl = a.Descendants("NavigateUrl").FirstOrDefault()?.Value,
					AlternateText = a.Descendants("AlternateText").FirstOrDefault()?.Value,
					Impressions = a.Descendants("Impressions").FirstOrDefault()?.Value,
					Keyword = a.Descendants("Keyword").FirstOrDefault()?.Value
				});
		}
	}
}
