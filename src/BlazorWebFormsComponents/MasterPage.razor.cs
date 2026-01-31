using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// A component that emulates ASP.NET Web Forms MasterPage functionality.
	/// In Blazor, MasterPages are replaced by Layout components, but this component
	/// provides a familiar API for developers migrating from Web Forms.
	/// </summary>
	/// <remarks>
	/// Web Forms MasterPages define a template for pages, with ContentPlaceHolder
	/// controls that child pages can populate with Content controls.
	/// 
	/// In Blazor, layouts use @Body and @RenderSection to achieve similar functionality.
	/// This MasterPage component acts as a bridge, allowing Web Forms-style markup
	/// to work in Blazor by internally using Blazor's layout system.
	/// 
	/// The Head parameter allows you to define head content that will be automatically
	/// wrapped in a HeadContent component, bridging the gap between Web Forms' 
	/// &lt;head runat="server"&gt; and Blazor's HeadContent approach.
	/// 
	/// Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.masterpage
	/// </remarks>
	public partial class MasterPage : MasterPageBase
	{
		/// <summary>
		/// The content of the master page template, which contains the layout and ContentPlaceHolder controls
		/// </summary>
		[Parameter]
		public RenderFragment ChildContent { get; set; }

		/// <summary>
		/// Optional head content that will be automatically wrapped in a HeadContent component.
		/// This provides a bridge between Web Forms' &lt;head runat="server"&gt; and Blazor's HeadContent.
		/// Content defined here will be rendered in the document's &lt;head&gt; section via HeadOutlet.
		/// </summary>
		/// <example>
		/// &lt;MasterPage&gt;
		///     &lt;Head&gt;
		///         &lt;link href="css/site.css" rel="stylesheet" /&gt;
		///         &lt;meta name="description" content="My site" /&gt;
		///     &lt;/Head&gt;
		///     &lt;ChildContent&gt;
		///         &lt;!-- Page layout here --&gt;
		///     &lt;/ChildContent&gt;
		/// &lt;/MasterPage&gt;
		/// </example>
		[Parameter]
		public RenderFragment Head { get; set; }

		/// <summary>
		/// Collection of ContentPlaceHolder controls defined in this master page
		/// </summary>
		internal Dictionary<string, ContentPlaceHolder> ContentPlaceHolders { get; } = new Dictionary<string, ContentPlaceHolder>();

		/// <summary>
		/// Registers a ContentPlaceHolder with this MasterPage
		/// </summary>
		internal void RegisterContentPlaceHolder(ContentPlaceHolder placeholder)
		{
			if (!string.IsNullOrEmpty(placeholder.ID))
			{
				ContentPlaceHolders[placeholder.ID] = placeholder;
			}
		}

		/// <summary>
		/// Gets the content for a specific ContentPlaceHolder from child pages
		/// </summary>
		internal RenderFragment GetContentForPlaceHolder(string placeHolderId)
		{
			if (ContentSections != null && ContentSections.TryGetValue(placeHolderId, out var contentSection))
			{
				return contentSection;
			}
			return null;
		}

		/// <summary>
		/// Content sections provided by child pages that use this master page
		/// </summary>
		internal Dictionary<string, RenderFragment> ContentSections { get; set; } = new Dictionary<string, RenderFragment>();
	}

	/// <summary>
	/// Base class for MasterPage components
	/// </summary>
	public abstract class MasterPageBase : BaseWebFormsComponent
	{
		/// <summary>
		/// Gets or sets the title of the page. In Web Forms, this is typically set by child pages
		/// and propagated to the master page's title element.
		/// </summary>
		[Parameter, Obsolete("Use @page directive with @title in Blazor, or set Title via PageTitle component")]
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the path to a parent master page for nested master pages
		/// </summary>
		[Parameter, Obsolete("Nested master pages are not commonly used in Blazor. Use nested layouts instead by setting @layout directive")]
		public string MasterPageFile { get; set; }
	}
}
