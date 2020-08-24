using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorWebFormsComponents
{
	public partial class AdRotator : BaseWebFormsComponent
	{
		[Parameter]
		public string AdvertisementFile { get; set; }

		[Parameter]
		public string Target { get; set; }

		[Parameter]
		public EventCallback<AdCreatedEventArgs> OnAdCreated { get; set; }

		internal Advertisment GetActiveAdvertisment()
		{
			var advertisments = GetAdvertismentsFileContent(AdvertisementFile);

			if (advertisments == null || advertisments.Count() == 0)
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
