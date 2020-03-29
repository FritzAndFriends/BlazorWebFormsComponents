using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class AdRotator : BaseWebFormsComponent
	{
		private static IEnumerable<Advertisment> _advertisments = Enumerable.Empty<Advertisment>();

		[Parameter]
		public string AdvertisementFile { get; set; }

		[Parameter]
		public string Target { get; set; }

		internal Advertisment GetActiveAdvertisment()
		{
			var rnd = new Random().Next(_advertisments.Count());

			return _advertisments.ElementAt(rnd);
		}

		protected async override void OnInitialized()
		{
			await base.OnInitializedAsync();

			_advertisments = await GetAdvertismentsFileContentAsync(AdvertisementFile);
		}

		private static async Task<IEnumerable<Advertisment>> GetAdvertismentsFileContentAsync(string fileName)
		{
			var xmlDocument = await XDocument.LoadAsync(new StreamReader(fileName), LoadOptions.None, CancellationToken.None);

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
