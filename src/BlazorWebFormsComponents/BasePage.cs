using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public abstract class BasePage : ComponentBase
	{

		[CascadingParameter(Name = "Page")]
		protected BaseLayout Page { get; set; }

		public string Title
		{
			get { return Page.Title; }
			set { Page.Title = value; }
		}


	}

}
