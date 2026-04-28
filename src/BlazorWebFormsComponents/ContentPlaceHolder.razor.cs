using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// A component that emulates ASP.NET Web Forms ContentPlaceHolder control.
	/// Defines a named content slot in a master page that child pages can fill
	/// with a <see cref="Content"/> control that targets the same
	/// <see cref="BaseWebFormsComponent.ID"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// ContentPlaceHolder is intentionally a thin reader over the shared
	/// <see cref="MasterPageContext"/> registry. The master-page host is responsible
	/// for re-rendering when the registry changes, which keeps this control aligned
	/// with Blazor layout semantics while preserving the original Web Forms tag name.
	/// </para>
	/// </remarks>
	public partial class ContentPlaceHolder : ContentPlaceHolderBase
	{
		/// <summary>Default content shown when no matching <see cref="Content"/> is registered.</summary>
		[Parameter]
		public RenderFragment ChildContent { get; set; }

		/// <summary>
		/// The shared master-page context cascaded by <see cref="MasterPage"/> or
		/// <see cref="MasterPageLayoutBase"/>.
		/// </summary>
		[CascadingParameter]
		private MasterPageContext MasterContext { get; set; }

		/// <summary>
		/// Direct reference to the parent <see cref="MasterPage"/> component, kept for
		/// backward compatibility.
		/// </summary>
		[CascadingParameter]
		private MasterPage ParentMasterPage { get; set; }

		/// <summary>Fragment injected by a matching <see cref="Content"/> control, if any.</summary>
		internal RenderFragment Content =>
			string.IsNullOrWhiteSpace(ID)
				? null
				: MasterContext?.GetContent(ID) ?? ParentMasterPage?.GetContentForPlaceHolder(ID);
	}

	/// <summary>Base class for <see cref="ContentPlaceHolder"/> component.</summary>
	public abstract class ContentPlaceHolderBase : BaseWebFormsComponent
	{
	}
}
