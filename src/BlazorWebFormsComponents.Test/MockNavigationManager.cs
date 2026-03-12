using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Test
{
	public class MockNavigationManager : NavigationManager
	{

		public MockNavigationManager(string baseUri = "http://localhost/", string initialUri = null)
		{

			Initialize(baseUri, initialUri ?? baseUri);

		}

		public string LastUri { get; set; }

		protected override void NavigateToCore(string uri, bool forceLoad)
		{

			LastUri = uri;

		}

	}

}
