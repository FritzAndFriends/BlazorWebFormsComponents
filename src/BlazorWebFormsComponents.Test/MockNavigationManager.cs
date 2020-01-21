using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Test
{
  public class MockNavigationManager : NavigationManager
  {

		public MockNavigationManager()
		{

			Initialize("http://localhost/", "http://localhost/");

		}

		public string LastUri { get; set; }

		protected override void NavigateToCore(string uri, bool forceLoad)
		{

		  LastUri = uri;

		}

  }

}
