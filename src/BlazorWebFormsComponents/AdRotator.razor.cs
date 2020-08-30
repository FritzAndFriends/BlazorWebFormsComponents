using Microsoft.AspNetCore.Components;
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
		private static readonly string DefaultAlternateTextField = "AlternateText";
		private static readonly string DefaultImageUrlField = "ImageUrl";
		private static readonly string DefaultNavigateUrlField = "NavigateUrl";

		[Parameter]
		public string AdvertisementFile { get; set; }

		[Parameter]
		public string AlternateTextField { get; set; } = DefaultAlternateTextField;

		[Parameter]
		public string ImageUrlField { get; set; } = DefaultImageUrlField;

		[Parameter]
		public string NavigateUrlField { get; set; } = DefaultNavigateUrlField;

		[Parameter]
		public string KeywordFilter { get; set; } = string.Empty;

		[Parameter]
		public string Target { get; set; }

		[Parameter]
		public EventCallback<AdCreatedEventArgs> OnAdCreated { get; set; }

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
		public FontInfo Font { get; set; } = new FontInfo();

		private string CalculatedStyle => this.ToStyle().Build().NullIfEmpty();

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

			if (KeywordFilter != string.Empty)
			{
				advertisments = advertisments.Where(a => a.Keyword.Equals(KeywordFilter, StringComparison.OrdinalIgnoreCase));
			}

			if (advertisments.Count() == 0)
			{
				return null;
			}

			var rnd = new Random().Next(advertisments.Count());
			var advertisment = advertisments.ElementAt(rnd);
			var adProperties = new Dictionary<string, string>();
			foreach (var property in typeof(Advertisment).GetProperties())
			{
				adProperties.Add(property.Name, property.GetValue(advertisment).ToString());
			}

			var adArgs = new AdCreatedEventArgs(adProperties)
			{
				AlternateText = advertisment.AlternateText,
				ImageUrl = advertisment.ImageUrl,
				NavigateUrl = advertisment.NavigateUrl
			};

			AdCreated(adArgs);

			// Override Ad properties before render
			advertisment.AlternateText = adArgs.AlternateText;
			advertisment.ImageUrl = adArgs.ImageUrl;
			advertisment.NavigateUrl = adArgs.NavigateUrl;

			return advertisment;
		}

		protected void AdCreated(AdCreatedEventArgs e)
		{
			OnAdCreated.InvokeAsync(e);
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
