using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// A component that emulates ASP.NET Web Forms Content control.
	/// Provides content for a ContentPlaceHolder in a master page.
	/// </summary>
	/// <remarks>
	/// In Web Forms, Content controls are used in child pages to provide content
	/// for ContentPlaceHolder controls defined in the master page.
	/// 
	/// In Blazor, this is done by placing content within layout sections or the @Body area.
	/// This component provides Web Forms-style syntax for developers migrating to Blazor.
	/// 
	/// Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.content
	/// </remarks>
	public partial class Content : ContentBase
	{
		/// <summary>
		/// The content to be rendered in the associated ContentPlaceHolder
		/// </summary>
		[Parameter]
		public RenderFragment ChildContent { get; set; }

		/// <summary>
		/// The ID of the ContentPlaceHolder this content is for
		/// </summary>
		[Parameter]
		public string ContentPlaceHolderID { get; set; }

		[CascadingParameter]
		private MasterPage ParentMasterPage { get; set; }

		protected override async Task OnInitializedAsync()
		{
			// Register this content with the parent MasterPage
			if (ParentMasterPage != null && !string.IsNullOrEmpty(ContentPlaceHolderID))
			{
				ParentMasterPage.ContentSections[ContentPlaceHolderID] = ChildContent;
			}

			await base.OnInitializedAsync();
		}
	}

	/// <summary>
	/// Base class for Content component
	/// </summary>
	public abstract class ContentBase : BaseWebFormsComponent
	{
	}
}
