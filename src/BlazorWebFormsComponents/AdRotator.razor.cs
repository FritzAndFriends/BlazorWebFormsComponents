using BlazorComponentUtilities;
using BlazorWebFormsComponents.DataBinding;
using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BlazorWebFormsComponents
{
	public partial class AdRotator : DataBoundComponent<Advertisment>, IStyle
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

		internal Advertisment GetActiveAdvertisment()
		{
			if (string.IsNullOrEmpty(AlternateTextField))
			{
				throw new ArgumentException("AlternateTextField can't be null or empty.", nameof(AlternateTextField));
			}

			if (string.IsNullOrEmpty(ImageUrlField))
			{
				throw new ArgumentException("ImageUrlField can't be null or empty.", nameof(ImageUrlField));
			}

			if (string.IsNullOrEmpty(NavigateUrlField))
			{
				throw new ArgumentException("NavigateUrlField can't be null or empty.", nameof(NavigateUrlField));
			}

			IEnumerable<Advertisment> advertisments;

			// Check if DataSource is provided (takes priority over AdvertisementFile)
			if (ItemsList != null && ItemsList.Any())
			{
				advertisments = ItemsList;
				DataBound(EventArgs.Empty);
			}
			else if (!string.IsNullOrEmpty(AdvertisementFile))
			{
				advertisments = GetAdvertismentsFileContent(AdvertisementFile);
			}
			else
			{
				return null;
			}

			if (advertisments == null || !advertisments.Any())
			{
				return null;
			}

			if (!string.IsNullOrEmpty(KeywordFilter))
			{
				advertisments = advertisments.Where(a => !string.IsNullOrEmpty(a.Keyword) && 
					a.Keyword.Equals(KeywordFilter, StringComparison.OrdinalIgnoreCase));
			}

			if (!advertisments.Any())
			{
				return null;
			}

			var rnd = new Random().Next(advertisments.Count());
			var advertisment = advertisments.ElementAt(rnd);
			var adProperties = new Dictionary<string, string>();
			foreach (var property in typeof(Advertisment).GetProperties())
			{
				var value = property.GetValue(advertisment);
				adProperties.Add(property.Name, value?.ToString() ?? string.Empty);
			}

			var adArgs = new AdCreatedEventArgs(adProperties)
			{
				AlternateText = advertisment.AlternateText,
				ImageUrl = advertisment.ImageUrl,
				NavigateUrl = advertisment.NavigateUrl,
				Sender = this
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

		// IStyle properties
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
		public FontInfo Font { get; set; } = new FontInfo();

		[Parameter]
		public WebColor ForeColor { get; set; }

		[Parameter]
		public Unit Height { get; set; }

		[Parameter]
		public Unit Width { get; set; }

		public string Style => this.ToStyle().Build().NullIfEmpty();
	}
}
