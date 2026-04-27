using Microsoft.AspNetCore.Components;
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
/// Content itself renders no visible HTML. Its sole purpose is to register
/// <see cref="ChildContent"/> with the nearest ancestor
/// <see cref="MasterPageContext"/> (or, for backward compatibility, directly
/// with a <see cref="MasterPage"/> parent).
/// </para>
/// <para>
/// Registration happens in <c>OnParametersSet</c> so the fragment is pushed
/// whenever <see cref="ChildContent"/> changes (dynamic content scenarios).
/// Because <see cref="MasterPageContext.SetContent"/> only notifies when the
/// fragment reference changes, repeated calls on stable content are cheap.
/// </para>
/// <para>
/// Original Microsoft documentation:
/// https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.content
/// </para>
/// </remarks>
public partial class Content : ContentBase
{
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
/// <see cref="MasterPageLayoutBase"/>. Used for subscription-based slot
/// notification.
/// </summary>
[CascadingParameter]
private MasterPageContext MasterContext { get; set; }

/// <summary>
/// Direct reference to the parent <see cref="MasterPage"/> component (component
/// model pattern). Kept for backward compatibility; prefer
/// <see cref="MasterContext"/> when available.
/// </summary>
[CascadingParameter]
private MasterPage ParentMasterPage { get; set; }

/// <inheritdoc />
protected override void OnParametersSet()
{
if (string.IsNullOrEmpty(ContentPlaceHolderID)) return;

// Context path  preferred. Notifies the subscribed ContentPlaceHolder
// immediately (no second render required for the normal forward-order case).
if (MasterContext != null)
MasterContext.SetContent(ContentPlaceHolderID, ChildContent);

// Also update ContentSections on the parent MasterPage so the
// IReadOnlyDictionary view stays in sync for diagnostics / tests.
if (ParentMasterPage != null)
ParentMasterPage.Context.SetContent(ContentPlaceHolderID, ChildContent);
}

/// <summary>
/// Clears the registered slot so the <see cref="ContentPlaceHolder"/> falls back
/// to its default content when this <see cref="Content"/> component is removed
/// from the render tree (e.g., behind an <c>@if</c> toggle).
/// </summary>
protected override async ValueTask Dispose(bool disposing)
{
if (disposing && !string.IsNullOrEmpty(ContentPlaceHolderID))
{
MasterContext?.SetContent(ContentPlaceHolderID, null);
ParentMasterPage?.Context.SetContent(ContentPlaceHolderID, null);
}
await base.Dispose(disposing);
}
}

/// <summary>Base class for <see cref="Content"/> component.</summary>
public abstract class ContentBase : BaseWebFormsComponent
{
}
}