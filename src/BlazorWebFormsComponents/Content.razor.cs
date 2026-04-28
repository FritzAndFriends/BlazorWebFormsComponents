using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// A component that emulates ASP.NET Web Forms Content control.
	/// Provides a named content fragment to the <see cref="ContentPlaceHolder"/>
	/// whose <see cref="BaseWebFormsComponent.ID"/> matches
	/// <see cref="ContentPlaceHolderID"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Content renders no visible HTML. It simply registers <see cref="ChildContent"/>
	/// with the nearest ancestor <see cref="MasterPageContext"/>.
	/// </para>
	/// <para>
	/// This keeps the migration-facing <c>&lt;Content&gt;</c> tag while shifting the
	/// implementation toward a layout-like named-section registry.
	/// </para>
	/// </remarks>
	public partial class Content : ContentBase
	{
		private string _registeredContentPlaceHolderId;

		/// <summary>The content fragment to inject into the matching placeholder.</summary>
		[Parameter]
		public RenderFragment ChildContent { get; set; }

		/// <summary>
		/// The <see cref="BaseWebFormsComponent.ID"/> of the
		/// <see cref="ContentPlaceHolder"/> that should receive this content.
		/// </summary>
		[Parameter]
		public string ContentPlaceHolderID { get; set; }

		/// <summary>
		/// The shared master-page context cascaded by <see cref="MasterPage"/> or
		/// <see cref="MasterPageLayoutBase"/>.
		/// </summary>
		[CascadingParameter]
		private MasterPageContext MasterContext { get; set; }

		/// <summary>
		/// Direct reference to the parent <see cref="MasterPage"/> component.
		/// Retained for backward compatibility when older markup only cascades the parent.
		/// </summary>
		[CascadingParameter]
		private MasterPage ParentMasterPage { get; set; }

		/// <inheritdoc />
		protected override void OnParametersSet()
		{
			if (string.IsNullOrWhiteSpace(ContentPlaceHolderID))
			{
				return;
			}

			var context = MasterContext ?? ParentMasterPage?.Context;
			if (string.Equals(_registeredContentPlaceHolderId, ContentPlaceHolderID, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}

			if (!string.IsNullOrWhiteSpace(_registeredContentPlaceHolderId))
			{
				context?.SetContent(_registeredContentPlaceHolderId, null);
			}

			context?.SetContent(ContentPlaceHolderID, ChildContent);
			_registeredContentPlaceHolderId = ContentPlaceHolderID;
		}

		/// <summary>
		/// Clears the registered slot so the <see cref="ContentPlaceHolder"/> falls back
		/// to its default content when this <see cref="Content"/> component is removed
		/// from the render tree.
		/// </summary>
		protected override async ValueTask Dispose(bool disposing)
		{
			if (disposing && !string.IsNullOrWhiteSpace(_registeredContentPlaceHolderId))
			{
				var context = MasterContext ?? ParentMasterPage?.Context;
				context?.SetContent(_registeredContentPlaceHolderId, null);
			}

			await base.Dispose(disposing);
		}
	}

	/// <summary>Base class for <see cref="Content"/> component.</summary>
	public abstract class ContentBase : BaseWebFormsComponent
	{
	}
}
