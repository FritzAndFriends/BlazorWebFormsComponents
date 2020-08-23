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
		private static readonly string DefaultAlternateTextField = "AlternateText";
		private static readonly string DefaultImageUrlField = "AlternateText";
		private static readonly string DefaultNavigateUrlField = "AlternateText";

		[Parameter]
		public string AlternateTextField { get; set; } = DefaultAlternateTextField;

		[Parameter]
		public string AdvertisementFile { get; set; }

		[Parameter]
		public string ImageUrlField { get; set; } = DefaultImageUrlField;

		[Parameter]
		public string NavigateUrlField { get; set; } = DefaultNavigateUrlField;

		[Parameter]
		public string Target { get; set; }

		internal Advertisment GetActiveAdvertisment()
		{
			if (string.IsNullOrEmpty(AlternateTextField))
			{
				throw new ArgumentException("AlternateTextField can't be null or empty.", nameof(AlternateTextField));
			}

			if (string.IsNullOrEmpty(ImageUrlField))
			{
				throw new ArgumentException("AlternateTextField can't be null or empty.", nameof(ImageUrlField));
			}

			if (string.IsNullOrEmpty(NavigateUrlField))
			{
				throw new ArgumentException("AlternateTextField can't be null or empty.", nameof(NavigateUrlField));
			}

			var advertisments = GetAdvertismentsFileContent(AdvertisementFile);

			if (advertisments == null || advertisments.Count() == 0)
			{
				return null;
			}

			var rnd = new Random().Next(advertisments.Count());

			return advertisments.ElementAt(rnd);
		}

		private IEnumerable<Advertisment> GetAdvertismentsFileContent(string fileName)
		{
			var xmlDocument = XDocument.Load(new StreamReader(fileName));

			return xmlDocument.Descendants("Ad")
				.Select(a => new Advertisment
				{
					ImageUrl = a.Descendants(ImageUrlField).FirstOrDefault()?.Value,
					Height = a.Descendants("Height").FirstOrDefault()?.Value,
					Width = a.Descendants("Width").FirstOrDefault()?.Value,
					NavigateUrl = a.Descendants(NavigateUrlField).FirstOrDefault()?.Value,
					AlternateText = a.Descendants(AlternateTextField).FirstOrDefault()?.Value,
					Impressions = a.Descendants("Impressions").FirstOrDefault()?.Value,
					Keyword = a.Descendants("Keyword").FirstOrDefault()?.Value
				});
		}
	}
}
