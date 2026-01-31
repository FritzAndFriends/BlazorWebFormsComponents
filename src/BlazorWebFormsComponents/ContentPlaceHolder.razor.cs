using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// A component that emulates ASP.NET Web Forms ContentPlaceHolder control.
	/// Defines a region in a master page that can be replaced with content from child pages.
	/// </summary>
	/// <remarks>
	/// In Web Forms, ContentPlaceHolder controls are used in master pages to define areas
	/// that child pages can customize using Content controls.
	/// 
	/// In Blazor, this is typically done with @Body for the main content area or 
	/// @RenderSection for named sections. This component bridges the gap by providing
	/// Web Forms-style syntax while using Blazor's underlying mechanisms.
	/// 
	/// Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.contentplaceholder
	/// </remarks>
	public partial class ContentPlaceHolder : ContentPlaceHolderBase
	{
		/// <summary>
		/// Default content to display if no Content control provides a replacement
		/// </summary>
		[Parameter]
		public RenderFragment ChildContent { get; set; }

		/// <summary>
		/// Content provided by a child page's Content control
		/// </summary>
		internal RenderFragment Content { get; set; }

		[CascadingParameter]
		private MasterPage ParentMasterPage { get; set; }

		protected override async Task OnInitializedAsync()
		{
			// Register this placeholder with the parent MasterPage
			ParentMasterPage?.RegisterContentPlaceHolder(this);

			// Get content from parent MasterPage if available
			if (ParentMasterPage != null && !string.IsNullOrEmpty(ID))
			{
				Content = ParentMasterPage.GetContentForPlaceHolder(ID);
			}

			await base.OnInitializedAsync();
		}
	}

	/// <summary>
	/// Base class for ContentPlaceHolder component
	/// </summary>
	public abstract class ContentPlaceHolderBase : BaseWebFormsComponent
	{
	}
}
