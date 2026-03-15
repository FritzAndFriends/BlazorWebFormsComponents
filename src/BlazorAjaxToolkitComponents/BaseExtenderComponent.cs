using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Base class for Ajax Control Toolkit extender components.
/// Extenders attach JavaScript behavior to a target control identified by
/// <see cref="TargetControlID"/> without rendering any HTML of their own.
/// JS modules are loaded as ES modules and manage behavior lifecycle.
/// </summary>
public abstract class BaseExtenderComponent : ComponentBase, IAsyncDisposable
{
	private IJSObjectReference _module;
	private IJSObjectReference _behaviorInstance;
	private bool _initialized;
	private bool _disposed;

	[Inject]
	protected IJSRuntime JS { get; set; } = default!;

	/// <summary>
	/// The ID of the control this extender targets.
	/// Passed to JavaScript for DOM resolution via document.getElementById().
	/// </summary>
	[Parameter]
	public string TargetControlID { get; set; } = string.Empty;

	/// <summary>
	/// Optional behavior identifier for JS-side lookup.
	/// Defaults to TargetControlID if not set.
	/// </summary>
	[Parameter]
	public string BehaviorID { get; set; }

	/// <summary>
	/// Whether the extender behavior is active. Default is true.
	/// </summary>
	[Parameter]
	public bool Enabled { get; set; } = true;

	/// <summary>
	/// Relative path to the JS ES module for this extender.
	/// Example: "./_content/BlazorAjaxToolkitComponents/js/confirm-button-extender.js"
	/// </summary>
	protected abstract string JsModulePath { get; }

	/// <summary>
	/// The exported JS function name that creates the behavior.
	/// </summary>
	protected abstract string JsCreateFunction { get; }

	/// <summary>
	/// Builds the property bag passed to the JS create/update functions.
	/// </summary>
	protected abstract object GetBehaviorProperties();

	/// <summary>
	/// Resolved behavior ID: uses BehaviorID parameter if set, otherwise TargetControlID.
	/// </summary>
	protected string ResolvedBehaviorID => BehaviorID ?? TargetControlID;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && Enabled)
		{
			await InitializeBehaviorAsync();
		}
	}

	private async Task InitializeBehaviorAsync()
	{
		if (string.IsNullOrEmpty(TargetControlID))
		{
			throw new InvalidOperationException(
				$"{GetType().Name} requires TargetControlID to be set.");
		}

		try
		{
			_module = await JS.InvokeAsync<IJSObjectReference>("import", JsModulePath);

			var config = new
			{
				targetId = TargetControlID,
				behaviorId = ResolvedBehaviorID,
				properties = GetBehaviorProperties()
			};

			_behaviorInstance = await _module.InvokeAsync<IJSObjectReference>(
				JsCreateFunction, config);

			_initialized = true;
		}
		catch (JSException ex)
		{
			// SSR/prerender — JS interop not available
			System.Diagnostics.Debug.WriteLine(
				$"[{GetType().Name}] JS init skipped (SSR): {ex.Message}");
		}
		catch (JSDisconnectedException)
		{
			// Circuit disconnected — expected during disposal/navigation
		}
	}

	/// <summary>
	/// Pushes updated properties to the JS behavior.
	/// Called by subclasses when parameters change after initial render.
	/// </summary>
	protected async Task UpdateBehaviorAsync()
	{
		if (!_initialized || _module == null) return;

		try
		{
			await _module.InvokeVoidAsync("updateBehavior",
				ResolvedBehaviorID, GetBehaviorProperties());
		}
		catch (JSException) { }
		catch (JSDisconnectedException) { }
	}

	private async Task DisposeBehaviorAsync()
	{
		if (_module != null && _initialized)
		{
			try
			{
				await _module.InvokeVoidAsync("disposeBehavior", ResolvedBehaviorID);
			}
			catch (JSException) { }
			catch (JSDisconnectedException) { }
			catch (ObjectDisposedException) { }

			_initialized = false;
		}

		if (_behaviorInstance != null)
		{
			try { await _behaviorInstance.DisposeAsync(); }
			catch (JSDisconnectedException) { }
			catch (ObjectDisposedException) { }
			_behaviorInstance = null;
		}
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		if (_disposed) return;
		_disposed = true;

		await DisposeBehaviorAsync();

		if (_module != null)
		{
			try { await _module.DisposeAsync(); }
			catch (JSDisconnectedException) { }
			catch (ObjectDisposedException) { }
			_module = null;
		}

		GC.SuppressFinalize(this);
	}
}
