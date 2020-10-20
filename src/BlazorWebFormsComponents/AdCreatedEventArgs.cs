using System;
using System.Collections;

namespace BlazorWebFormsComponents
{
	public class AdCreatedEventArgs : EventArgs
	{
		public AdCreatedEventArgs(IDictionary adProperties)
		{
			AdProperties = adProperties;
		}

		public IDictionary AdProperties { get; }

		public string AlternateText { get; set; }

		public string ImageUrl { get; set; }

		public string NavigateUrl { get; set; }
	}
}
