using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class AdRotator : BaseWebFormsComponent
	{
		[Parameter]
		public string AdvertisementFile { get; set; }

		[Parameter]
		public string KeywordFilter { get; set; } = string.Empty;

		[Parameter]
		public string Target { get; set; }

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
