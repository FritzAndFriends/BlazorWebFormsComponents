using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BlazorWebFormsComponents;

/// <summary>
/// C# wrapper for Chart.js interop calls via the chart-interop.js ES module.
/// </summary>
public sealed class ChartJsInterop : IAsyncDisposable
{
	private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

	public ChartJsInterop(IJSRuntime jsRuntime)
	{
		_moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
			"import", "./_content/Fritz.BlazorWebFormsComponents/js/chart-interop.js").AsTask());
	}

	/// <summary>
	/// Creates a new Chart.js chart on the specified canvas element.
	/// </summary>
	public async ValueTask CreateChartAsync(string canvasId, object config)
	{
		var module = await _moduleTask.Value;
		await module.InvokeVoidAsync("createChart", canvasId, config);
	}

	/// <summary>
	/// Updates an existing Chart.js chart with new configuration.
	/// </summary>
	public async ValueTask UpdateChartAsync(string canvasId, object config)
	{
		var module = await _moduleTask.Value;
		await module.InvokeVoidAsync("updateChart", canvasId, config);
	}

	/// <summary>
	/// Destroys a Chart.js chart instance.
	/// </summary>
	public async ValueTask DestroyChartAsync(string canvasId)
	{
		var module = await _moduleTask.Value;
		await module.InvokeVoidAsync("destroyChart", canvasId);
	}

	public async ValueTask DisposeAsync()
	{
		if (_moduleTask.IsValueCreated)
		{
			var module = await _moduleTask.Value;
			await module.DisposeAsync();
		}
	}
}
