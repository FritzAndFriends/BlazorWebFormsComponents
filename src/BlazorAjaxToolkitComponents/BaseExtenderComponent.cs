using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Base class for Ajax Control Toolkit extender components.
/// Extenders attach behavior to a target control identified by <see cref="TargetControlID"/>.
/// Full design pending — see Forge's architecture spec.
/// </summary>
public abstract class BaseExtenderComponent : ComponentBase, IAsyncDisposable
{
	[Inject]
	protected IJSRuntime JS { get; set; } = default!;

	/// <summary>
	/// The ID of the control this extender targets.
	/// </summary>
	[Parameter]
	public string TargetControlID { get; set; } = string.Empty;

	/// <inheritdoc />
	public virtual ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}
}
