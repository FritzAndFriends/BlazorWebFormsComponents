using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

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
/// When a <see cref="MasterPageContext"/> is available (cascaded by
/// <see cref="MasterPage"/> or <see cref="MasterPageLayoutBase"/>), this
/// control subscribes to its named slot so it re-renders immediately when a
/// <see cref="Content"/> control registers a matching fragment  even when
/// Content appears later in the render tree.
/// </para>
/// <para>
/// When no context is available the placeholder renders its
/// <see cref="ChildContent"/> (default content) as a standalone element.
/// </para>
/// <para>
/// Original Microsoft documentation:
/// https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.contentplaceholder
/// </para>
/// </remarks>
public partial class ContentPlaceHolder : ContentPlaceHolderBase, IDisposable
{
/// <summary>Default content shown when no matching <see cref="Content"/> is registered.</summary>
[Parameter]
public RenderFragment ChildContent { get; set; }

/// <summary>Fragment injected by a matching <see cref="Content"/> control, if any.</summary>
internal RenderFragment? Content { get; private set; }

/// <summary>
/// The shared master-page context cascaded by <see cref="MasterPage"/> or
/// <see cref="MasterPageLayoutBase"/>. When present, the placeholder subscribes
/// so it is notified the instant its named slot receives content.
/// </summary>
[CascadingParameter]
private MasterPageContext MasterContext { get; set; }

/// <summary>
/// Direct reference to the parent <see cref="MasterPage"/> component, available
/// in the component-model pattern (where ContentPlaceHolder is inside
/// <see cref="MasterPage.ChildContent"/>). Kept for backward compatibility.
/// </summary>
[CascadingParameter]
private MasterPage ParentMasterPage { get; set; }

/// <inheritdoc />
protected override void OnInitialized()
{
// Register with parent for backward compat (component model).
// This is a no-op in the updated implementation but kept so any
// code calling RegisterContentPlaceHolder still compiles.
#pragma warning disable CS0618
ParentMasterPage?.RegisterContentPlaceHolder(this);
#pragma warning restore CS0618

// Subscribe to the context so we receive an immediate callback when
// Content registers our slot  handles the reverse-order case where
// ContentPlaceHolder renders before the Content control.
if (MasterContext != null && !string.IsNullOrEmpty(ID))
{
MasterContext.Subscribe(ID, OnContextContentChanged);
Content = MasterContext.GetContent(ID);
}
else if (ParentMasterPage != null && !string.IsNullOrEmpty(ID))
{
Content = ParentMasterPage.GetContentForPlaceHolder(ID);
}
}

/// <inheritdoc />
protected override void OnParametersSet()
{
// Re-read on every parameters update (covers re-renders triggered
// by MasterPage.OnAfterRenderAsync or external state changes).
if (MasterContext != null && !string.IsNullOrEmpty(ID))
Content = MasterContext.GetContent(ID);
else if (ParentMasterPage != null && !string.IsNullOrEmpty(ID))
Content = ParentMasterPage.GetContentForPlaceHolder(ID);
}

/// <summary>
/// Invoked by <see cref="MasterPageContext"/> the moment a matching
/// <see cref="Content"/> control registers its fragment.
/// </summary>
private void OnContextContentChanged()
{
Content = MasterContext?.GetContent(ID);
// InvokeAsync marshals StateHasChanged to the Blazor sync context,
// which is required when the callback originates from a Content
// control rendering on a different sync-context tick.
_ = InvokeAsync(StateHasChanged);
}

/// <inheritdoc />
public void Dispose()
{
MasterContext?.Unsubscribe(ID);
}
}

/// <summary>Base class for <see cref="ContentPlaceHolder"/> component.</summary>
public abstract class ContentPlaceHolderBase : BaseWebFormsComponent
{
}
}